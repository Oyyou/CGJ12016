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
using System.IO;
using PleaseThem.Models;

namespace PleaseThem.States
{
  public class GameState : State
  {
    #region Managers

    public ResourceManager ResourceManager;

    #endregion

    #region Fields

    private BuildingList _buildingList;

    private Camera _camera;

    private KeyboardState _currentKeyboard;

    private bool _isPaused = false;

    private Menu _menu;

    private KeyboardState _previousKeyboard;

    private float _timer;

    #endregion

    #region Properties

    public List<Component> Components { get; private set; }

    public MouseState CurrentMouse { get; private set; }

    public List<Component> GUIComponents { get; private set; }

    public Map Map { get; private set; }

    public int MaximumMinions { get; private set; }

    public int MinionCount
    {
      get { return Components.Where(c => c is Minion).Count(); }
    }

    public Rectangle MouseRectangleWithCamera
    {
      get
      {
        return new Rectangle(CurrentMouse.X + (int)_camera.Position.X, CurrentMouse.Y + (int)_camera.Position.Y, 1, 1);
      }
    }

    public Rectangle MouseRectangle
    {
      get
      {
        return new Rectangle(CurrentMouse.X, CurrentMouse.Y, 1, 1);
      }
    }

    public Pathfinder Pathfinder { get; private set; }

    public MouseState PreviousMouse { get; private set; }

    public int UnemploymentCount
    {
      get
      {
        return Components.Where(c => c is Minion).Where(c => ((Minion)c).Workplace == null).Count();
      }
    }

    #endregion

    #region Methods

    private void AddMinions(Building building)
    {
      // Spawn minions at the 'door' position of the building
      for (int x = 0; x < building.Rectangle.Width / Map.TileSize; x++)
      {
        Components.Add(new Minion(this,
          new Controllers.AnimationController()
          {
            WalkDown = new Animation(_content.Load<Texture2D>("Actors/Minion/WalkingDown"), 4, 0.2f, true),
            WalkRight = new Animation(_content.Load<Texture2D>("Actors/Minion/WalkingRight"), 4, 0.2f, true),
            WalkUp = new Animation(_content.Load<Texture2D>("Actors/Minion/WalkingUp"), 4, 0.2f, true),
          })
        {
          Position = building.DoorPosition,
          Home = building,
          IsVisible = false,
        });
      }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      DrawGame(gameTime, spriteBatch);

      DrawGUI(gameTime, spriteBatch);
    }

    private void DrawGUI(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin();

      foreach (var component in GUIComponents)
        component?.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }

    private void DrawGame(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: _camera.Transform);

      Map.Draw(gameTime, spriteBatch);

      foreach (var component in Components)
        component?.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }

    public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
      : base(game, graphicsDevice, content)
    {

      _buildingList = new BuildingList(_graphicsDevice, base._content, this);

      Map = new Map(base._content, 100, 100);

      ResourceManager = new ResourceManager();

      float x = Game1.Random.Next(64, (Map.Width * Map.TileSize) - 224); // 224 = HallWidth + 64. 64 = SpaceAroundEdges
      float y = Game1.Random.Next(64, (Map.Height * Map.TileSize) - 288);

      x = (float)Math.Floor(x / Map.TileSize) * Map.TileSize;
      y = (float)Math.Floor(y / Map.TileSize) * Map.TileSize;

      Vector2 hallPosition = new Vector2(x, y);

      _menu = new Menu(content, this);

      Components = new List<Component>()
      {
        new Hall(this, content.Load<Texture2D>("Buildings/Hall"))
        {
          Position = hallPosition,
        },
        _menu,
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
        new ResourceList(_graphicsDevice, content.Load<SpriteFont>("Fonts/Arial08pt"), this),
        _buildingList,
        new FX.CursorTrail(_graphicsDevice, content, this)
      };

      _camera = new Camera(new Vector2(hallPosition.X - (Game1.ScreenWidth / 2), hallPosition.Y - (Game1.ScreenHeight / 2)));
      Pathfinder = new Pathfinder(Map);

      MaximumMinions = 10;
    }

    public void LoadGame()
    {
      using (var reader = new StreamReader("data.txt"))
      {
        var line = reader.ReadLine();


      }
    }

    private void PlaceBuilding()
    {
      if (_buildingList.SelectedBuilding != null)
      {
        if (CurrentMouse.LeftButton == ButtonState.Pressed &&
            PreviousMouse.LeftButton == ButtonState.Released)
        {
          bool canBuild = true;

          foreach (var component in Components)
          {
            if (component is Building)
            {
              if (component == _buildingList.SelectedBuilding)
                break;
            }

            if (component is Menu)
              break;

            var building = component as Models.Sprite;
            if (building.Rectangle.Intersects(_buildingList.SelectedBuilding.Rectangle))
            {
              canBuild = false;
              break;
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

            foreach (var tile in Map.BackgroundTiles)
            {
              if (tile.IsVisible)
                continue;

              if (tile.Rectangle.Intersects(_buildingList.SelectedBuilding.Rectangle))
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
              var newBuilding = _buildingList.SelectedBuilding.Clone() as Building;

              newBuilding.Color = Color.White;
              newBuilding.Layer = _buildingList.SelectedBuilding.DefaultLayer;

              Components.Add(newBuilding);

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

      var validComponents = Components
        .Where(c => !(c.Equals(_buildingList.SelectedBuilding)))
        .Where(c => c is Sprite);

      var distance = 448;

      // Sigh - this is where 'Origin' would be useful!

      foreach (var tile in Map.BackgroundTiles)
      {
        tile.IsVisible = false;

        if (validComponents.Any(c => Vector2.Distance(tile.Position, ((Sprite)c).Position) < distance))
          tile.IsVisible = true;
      }

      foreach (var tile in Map.ResourceTiles)
      {
        tile.IsVisible = false;

        if (validComponents.Any(c => Vector2.Distance(tile.Position, ((Sprite)c).Position) < distance))
          tile.IsVisible = true;
      }
    }

    public void SaveGame()
    {
      using (var writer = new StreamWriter("data.txt"))
      {
        foreach (var component in Components)
        {
          var data = component.GetSaveData();

          if (string.IsNullOrEmpty(data))
            continue;

          var values = data.Split('\n');

          foreach (var value in values)
          {
            writer.WriteLine(value);
          }
        }
      }
    }

    private void SetInput()
    {
      _previousKeyboard = _currentKeyboard;
      _currentKeyboard = Keyboard.GetState();

      PreviousMouse = CurrentMouse;
      CurrentMouse = Mouse.GetState();
    }

    public override void Update(GameTime gameTime)
    {
      if (_currentKeyboard.IsKeyUp(Keys.F) && _previousKeyboard.IsKeyDown(Keys.F))
      {
        SaveGame();
      }

      if (_currentKeyboard.IsKeyUp(Keys.J) && _previousKeyboard.IsKeyDown(Keys.J))
      {
        LoadGame();
      }

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
        if (component != _buildingList.SelectedBuilding)
          component?.Update(gameTime);
      }

      // If we're not on the interface stuff
      if (CurrentMouse.Y >= 16 && CurrentMouse.Y < (Game1.ScreenHeight - 64))
      {
        PlaceBuilding();
      }
    }

    #endregion
  }
}
