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

    private Camera _camera;

    private BuildingList _buildingSelector;

    public Map Map { get; private set; }
    public Pathfinder Pathfinder { get; private set; }

    public List<Component> _components;

    private List<Component> _guiComponents;

    private Building _selectedBuilding = null;

    private MouseState _currentMouse;
    private MouseState _previousMouse;

    private KeyboardState _currentKeyboard;
    private KeyboardState _previousKeyboard;

    private float _timer;

    public Rectangle MouseRectangle
    {
      get
      {
        return new Rectangle(_currentMouse.X + (int)_camera.Position.X, _currentMouse.Y + (int)_camera.Position.Y, 1, 1);
      }
    }

    public int MinionCount
    {
      get { return _components.Where(c => c is Minion).Count(); }
    }

    public int UnemploymentCount
    {
      get
      {
        return _components.Where(c => c is Minion).Where(c => ((Minion)c).Employment == null).Count();
      }
    }

    public int MaximumMinions { get; private set; }

    public ContentManager Content;

    private Random _random;

    private bool _isPaused = false;

    public GameState(ContentManager content)
      : base(content)
    {
      Content = content;

      _buildingSelector = new BuildingList(Content, this);

      Map = new Map(Content, 100, 100);
      _random = new Random();

      ResourceManager = new ResourceManager();

      MinionManager = new MinionManager();

      float x = _random.Next(64, (Map.Width * Map.TileSize) - 224); // 224 = HallWidth + 64. 64 = SpaceAroundEdges
      float y = _random.Next(64, (Map.Height * Map.TileSize) - 288);

      x = (float)Math.Floor(x / Map.TileSize) * Map.TileSize;
      y = (float)Math.Floor(y / Map.TileSize) * Map.TileSize;

      Vector2 hallPosition = new Vector2(x, y);
      
      _components = new List<Component>()
      {
        new Hall(this, content.Load<Texture2D>("Buildings/Hall"))
        {
          Position = hallPosition,
        },
      };

      Map.CleanArea(new Vector2(hallPosition.X + 80, hallPosition.Y + 80), 10);

      foreach (var component in _components.ToArray())
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

      _guiComponents = new List<Component>()
      {
        new ResourceList(Content.Load<Texture2D>("Controls/ResourceList"),
                         Content.Load<SpriteFont>("Fonts/Arial08pt"), this),
        _buildingSelector,
      };

      _camera = new Camera(new Vector2(hallPosition.X - 400, hallPosition.Y - 240));
      Pathfinder = new Pathfinder(Map);

      MaximumMinions = 10;
    }

    public override void PostUpdate(GameTime gameTime)
    {
      for (int i = 0; i < _components.Count; i++)
      {
        if (_components[i].IsRemoved)
        {
          _components.RemoveAt(i);
          i--;
        }
      }
    }

    public override void Update(GameTime gameTime)
    {
      _previousKeyboard = _currentKeyboard;
      _currentKeyboard = Keyboard.GetState();

      _previousMouse = _currentMouse;
      _currentMouse = Mouse.GetState();

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

      _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
      if (_timer > 1)
      {
        _timer = 0;
        ResourceManager.Increment();
      }

      foreach (var compontent in _guiComponents)
        compontent.Update(gameTime);

      //foreach (var component in _components)
      //  component.Update(gameTime);

      _selectedBuilding = _buildingSelector.SelectedBuilding;

      if (_buildingSelector.SelectedBuilding != null && !_components.Contains(_buildingSelector.SelectedBuilding))
      {
        _components.Add(_selectedBuilding);
      }

      if(_selectedBuilding != null)
      {

      }

      foreach (var component in _components)
      {
        component?.Update(gameTime);

        if (component is Building)
        {
          var building = component as Building;

          if (_currentMouse.Y >= 32 && _currentMouse.Y < 480 - 64)
          {
            if (building.LeftClicked)
            {
              if (building.CanHaveWorkers)
              {
                if (UnemploymentCount > 0)
                {
                  ((Minion)_components.Where(c => c is Minion).Where(c => ((Minion)c).Employment == null).FirstOrDefault()).Employ(building);
                  building.CurrentMinions++;
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
                var minion = _components.Where(c => c is Minion).Where(c => ((Minion)c).Employment == building).LastOrDefault() as Minion;
                minion.Unemploy();
                building.CurrentMinions--;
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

    private void PlaceBuilding()
    {
      if (_selectedBuilding != null)
      {
        if (_currentMouse.LeftButton == ButtonState.Pressed &&
            _previousMouse.LeftButton == ButtonState.Released)
        {
          bool canBuild = true;

          foreach (var component in _components)
          {
            if (component is Building)
            {
              if (component == _selectedBuilding)
                break;

              var building = component as Building;
              if (building.Rectangle.Intersects(_selectedBuilding.Rectangle))
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
              if (resource.Rectangle.Intersects(_selectedBuilding.Rectangle))
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
              _selectedBuilding.Color = Color.White;
              _components.Add((Building)_selectedBuilding.Clone());
              _selectedBuilding.Initialise();

              Map.Add(_selectedBuilding.CollisionRectangle);
              Pathfinder.InitializeSearchNodes(Map); // We can't possibly call this everytime a minion enters a new tile..?

              ResourceManager.Use(_selectedBuilding.Resources);

              if (_selectedBuilding is House)
              {
                AddMinions(_selectedBuilding);
              }

              _buildingSelector.SelectedBuilding.IsRemoved = true;
              _buildingSelector.SelectedBuilding = null;
              _selectedBuilding = null;
            }
          }
        }
      }
    }

    private void AddMinions(Building building)
    {
      for (int x = 0; x < building.Rectangle.Width / 32; x++)
      {
        _components.Add(new Minion(this, 
          new Controllers.AnimationController()
          {
            WalkDown = new Animation(Content.Load<Texture2D>("Actors/Minion/WalkingDown"), 4, 0.2f, true),
            WalkRight = new Animation(Content.Load<Texture2D>("Actors/Minion/WalkingRight"), 4, 0.2f, true),
            WalkUp = new Animation(Content.Load<Texture2D>("Actors/Minion/WalkingUp"), 4, 0.2f, true),
          })
        {
          Position = new Vector2(building.Position.X + (x * 32), building.Rectangle.Bottom - 32),
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
      // Camera stuff
      spriteBatch.Begin(/*SpriteSortMode.FrontToBack,*/ transformMatrix: _camera.Transform);

      Map.Draw(spriteBatch);

      foreach (var component in _components)
        component?.Draw(gameTime, spriteBatch);

      //foreach (var building in _buildings)
      //  building.Draw(spriteBatch);

      //if (_selectedBuilding != null)
      //{
      //  _selectedBuilding.Color = Color.Green;
      //  _selectedBuilding.Draw(spriteBatch);
      //}

      spriteBatch.End();
    }

    private void DrawGUI(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin();

      foreach (var component in _guiComponents)
        component?.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }
  }
}
