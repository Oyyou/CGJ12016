using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using PleaseThem.Controls;
using PleaseThem.Buildings;
using Microsoft.Xna.Framework.Input;
using PleaseThem.Core;
using PleaseThem.Actors;
using PleaseThem.Managers;
using PleaseThem.Buildings.Government;

namespace PleaseThem.States
{
  public class GameState : State
  {
    #region Managers

    public ResourceManager ResourceManager;

    public MinionManager MinionManager;

    #endregion

    private BuildingList _buildingList;

    private Camera _camera;

    private KeyboardState _currentKeyboard;

    private MouseState _currentMouse;

    private bool _isPaused = false;

    private KeyboardState _previousKeyboard;

    private MouseState _previousMouse;

    private float _timer;

    public Map Map { get; private set; }

    public Pathfinder Pathfinder { get; private set; }

    public List<Component> Components { get; private set; }

    public ContentManager Content;

    public List<Component> GUIComponents { get; private set; }

    public int MaximumMinions { get; private set; }

    public int MinionCount
    {
      get { return Components.Where(c => c is Minion).Count(); }
    }

    public Rectangle MouseRectangle
    {
      get
      {
        return new Rectangle(_currentMouse.X + (int)_camera.Position.X, _currentMouse.Y + (int)_camera.Position.Y, 1, 1);
      }
    }

    public int UnemploymentCount
    {
      get
      {
        return Components.Where(c => c is Minion).Where(c => ((Minion)c).Employment == null).Count();
      }
    }

    public GameState(ContentManager content)
      : base(content)
    {
      Content = content;

      _buildingList = new BuildingList(Content, this);

      Map = new Map(Content, 100, 100);

      ResourceManager = new ResourceManager();

      MinionManager = new MinionManager();

      float x = Game1.Random.Next(64, (Map.Width * Map.TileSize) - 224); // 224 = HallWidth + 64. 64 = SpaceAroundEdges
      float y = Game1.Random.Next(64, (Map.Height * Map.TileSize) - 288);

      x = (float)Math.Floor(x / Map.TileSize) * Map.TileSize;
      y = (float)Math.Floor(y / Map.TileSize) * Map.TileSize;

      Vector2 hallPosition = new Vector2(x, y);
      
      Components = new List<Component>()
      {
        new Hall(this, content.Load<Texture2D>("Buildings/Hall"))
        {
          Position = hallPosition,
        },
      };

      Map.CleanArea(new Vector2(hallPosition.X + 80, hallPosition.Y + 80), 10);

      foreach (var component in Components.ToArray())
      {
        if (component is Models.Sprite)
        {
          var sprite = component as Models.Sprite;

          if (sprite is Building)
          {
            var building = sprite as Building;

            Map.Add(sprite.CollisionRectangle);
            if (sprite is Hall)
              AddMinions(building); // Adding minions in-front of the 'hall' 
          }
        }
      }

      GUIComponents = new List<Component>()
      {
        new ResourceList(Content.Load<Texture2D>("Controls/ResourceList"),
                         Content.Load<SpriteFont>("Fonts/Arial08pt"), this),
        _buildingList,
      };

      _camera = new Camera(new Vector2(hallPosition.X - 400, hallPosition.Y - 240));
      Pathfinder = new Pathfinder(Map);

      MaximumMinions = 10;
    }

    public override void PostUpdate(GameTime gameTime)
    {
      for (int i = 0; i < Components.Count; i++)
      {
        if (Components[i].IsRemoved)
        {
          Components.RemoveAt(i);
          i--;
        }
      }
    }

    public override void Update(GameTime gameTime)
    {
      SetInput();

      if (_currentKeyboard.IsKeyUp(Keys.Space) && _previousKeyboard.IsKeyDown(Keys.Space))
      {
        Game1.MessageBox.IsVisible = false;
        _isPaused = !_isPaused;
      }

      _camera.Update(Map);

      if (_isPaused)
      {
        Game1.MessageBox.Show("PAUSED");
        return;
      }

      // Add resources every second
      _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
      if (_timer > 1)
      {
        _timer = 0;
        ResourceManager.Increment();
      }

      foreach (var compontent in GUIComponents)
        compontent.Update(gameTime);

      if (_buildingList.SelectedBuilding != null && !Components.Contains(_buildingList.SelectedBuilding))
      {
        Components.Add(_buildingList.SelectedBuilding);
      }

      foreach (var component in Components)
      {
        component?.Update(gameTime);

        if (component is Building)
        {
          if (component == _buildingList.SelectedBuilding)
            continue;

          var building = component as Building;

          if (_currentMouse.Y >= 32 && _currentMouse.Y < 480 - 64)
          {
            if (building.LeftClicked)
            {
              if (building.CanHaveWorkers)
              {
                if (UnemploymentCount > 0)
                {
                  var minion = Components.Where(c => c is Minion).Where(c => ((Minion)c).Employment == null).FirstOrDefault() as Minion;
                  building.Employ(minion);
                }
                else
                {
                  Game1.MessageBox.Show("There are no unemployed minions");
                }
              }
              else
              {
                Game1.MessageBox.Show("Building can't employ");
              }
            }

            if (building.RightClicked)
            {
              if (building.CurrentMinions > 0)
              {
                //var minion = _components.Where(c => c is Minion).Where(c => ((Minion)c).Employment == building).LastOrDefault() as Minion;
                //minion.Unemploy();
                building.Unemploy();
                if (building is Farm)
                  ((Farm)building).FarmPositions.Where(c => c.Working).Last().Working = false;
              }
            }
          }
        }
      }

      // If we're not on the interface stuff
      if (_currentMouse.Y >= 32 && _currentMouse.Y < 480 - 64)
      {
        PlaceBuilding();
      }
    }

    private void SetInput()
    {
      _previousKeyboard = _currentKeyboard;
      _currentKeyboard = Keyboard.GetState();

      _previousMouse = _currentMouse;
      _currentMouse = Mouse.GetState();
    }

    private void PlaceBuilding()
    {
      if (_buildingList.SelectedBuilding != null)
      {
        if (_currentMouse.LeftButton == ButtonState.Pressed &&
            _previousMouse.LeftButton == ButtonState.Released)
        {
          bool canBuild = true;

          foreach (var component in Components)
          {
            if (component is Building)
            {
              if (component == _buildingList.SelectedBuilding)
                break;

              var building = component as Building;
              if (building.Rectangle.Intersects(_buildingList.SelectedBuilding.Rectangle))
              {
                canBuild = false;
                break;
              }
            }
          }

          if (!canBuild)
          {
            Game1.MessageBox.Show("Are you some kind of idiot?");
          }
          else
          {
            foreach (var resource in Map.ResourceTiles)
            {
              if (resource.Rectangle.Intersects(_buildingList.SelectedBuilding.Rectangle))
              {
                canBuild = false;
                break;
              }
            }

            if (!canBuild)
            {
              Game1.MessageBox.Show("Are you some kind of idiot?");
            }
            else if (canBuild)
            {
              _buildingList.SelectedBuilding.Color = Color.White;
              Components.Add((Building)_buildingList.SelectedBuilding.Clone());
              _buildingList.SelectedBuilding.Initialise();

              Map.Add(_buildingList.SelectedBuilding.CollisionRectangle);
              Pathfinder.InitializeSearchNodes(Map); // We can't possibly call this everytime a minion enters a new tile..?

              ResourceManager.Use(_buildingList.SelectedBuilding.Resources);

              if (_buildingList.SelectedBuilding is House)
              {
                AddMinions(_buildingList.SelectedBuilding);
              }

              _buildingList.SelectedBuilding.IsRemoved = true;
              _buildingList.SelectedBuilding = null;
            }
          }
        }
      }
    }

    private void AddMinions(Building building)
    {
      for (int x = 0; x < building.Rectangle.Width / 32; x++)
      {
        Components.Add(new Minion(this, 
          new Controllers.AnimationController()
          {
            WalkDown = new Animation(Content.Load<Texture2D>("Actors/Minion/WalkingDown"), 4, 0.2f, true),
            WalkRight = new Animation(Content.Load<Texture2D>("Actors/Minion/WalkingRight"), 4, 0.2f, true),
            WalkUp = new Animation(Content.Load<Texture2D>("Actors/Minion/WalkingUp"), 4, 0.2f, true),
          })
        {
          Position = new Vector2(building.Position.X + (x * 32), building.Rectangle.Bottom - 32),
          Home = building,
        });
      }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      DrawGame(gameTime, spriteBatch);

      DrawGUI(gameTime, spriteBatch);
    }

    private void DrawGame(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: _camera.Transform);

      Map.Draw(gameTime, spriteBatch);

      foreach (var component in Components)
        component?.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }

    private void DrawGUI(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin();

      foreach (var component in GUIComponents)
        component?.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }
  }
}
