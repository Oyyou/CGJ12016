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

namespace PleaseThem.States
{
  public class GameState : State
  {
    #region Managers

    public ResourceManager ResourceManager;

    public MinionManager MinionManager;

    #endregion

    private Camera _camera;

    private ResourceList _resourceList;
    private BuildingList _buildingSelector;

    public Map Map { get; private set; }
    public Pathfinder Pathfinder { get; private set; }

    private List<Building> _buildings = new List<Building>();
    private Building _selectedBuilding = null;

    public List<Minion> _minions = new List<Minion>();

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
      get { return _minions.Count; }
    }

    public int UnemploymentCount
    {
      get
      {
        return _minions.Where(c => c.Employment == null).Count();
      }
    }

    public int MaximumMinions { get; private set; }

    private ContentManager _content;

    private Random _random;

    private bool _isPaused = false;

    public GameState(ContentManager content)
      : base(content)
    {
      _content = content;

      _resourceList = new ResourceList(_content.Load<Texture2D>("Controls/ResourceList"),
                                       _content.Load<SpriteFont>("Fonts/Arial08pt"), this);
      _buildingSelector = new BuildingList(_content, this);

      Map = new Map(_content, 100, 100);
      _random = new Random();

      ResourceManager = new ResourceManager();

      MinionManager = new MinionManager();

      float x = _random.Next(64, (Map.Width * Map.TileSize) - 224); // 224 = HallWidth + 64. 64 = SpaceAroundEdges
      float y = _random.Next(64, (Map.Height * Map.TileSize) - 288);

      while (x % 32 != 0)
        x--;
      while (y % 32 != 0)
        y--;

      Vector2 hallPosition = new Vector2(x, y);
      _buildings.Add(new Hall(_content, hallPosition, this));
      Map.CleanArea(new Vector2(hallPosition.X + 80, hallPosition.Y + 80), 10);
      Map.Add(_buildings.FirstOrDefault().CollisionRectangle);

      AddMinions(_buildings.FirstOrDefault()); // Adding minions in-front of the 'hall' 


      _camera = new Camera(new Vector2(hallPosition.X - 400, hallPosition.Y - 240));
      Pathfinder = new Pathfinder(Map);

      MaximumMinions = 10;
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

      _buildingSelector.Update(gameTime);
      _selectedBuilding = _buildingSelector.SelectedBuilding;

      // If we're not on the interface stuff
      if (_currentMouse.Y >= 32 && _currentMouse.Y < 480 - 64)
      {
        PlaceBuilding();

        //if (_currentMouse.LeftButton == ButtonState.Pressed &&
        //    _previousMouse.LeftButton == ButtonState.Released)
        //{
        //  int x = MouseRectangle.X;
        //  int y = MouseRectangle.Y;

        //  while (x % 32 != 0) x--;
        //  while (y % 32 != 0) y--;

        //  Console.WriteLine($"X: {x}  Y: {y}");
        //}
      }

      foreach (var building in _buildings)
      {
        building.Update(gameTime);
        if (_currentMouse.Y >= 32 && _currentMouse.Y < 480 - 64)
        {
          if (building.LeftClicked)
          {
            if (building.CanHaveWorkers)
            {
              if (UnemploymentCount > 0)
              {
                _minions.Where(c => c.Employment == null).FirstOrDefault().Employ(building);
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
              var minion = _minions.Where(c => c.Employment == building).LastOrDefault();
              minion.Unemploy();
              building.CurrentMinions--;
              if (building is Farm)
                ((Farm)building).FarmPositions.Where(c => c.Working).Last().Working = false;
            }
          }
        }
      }

      foreach (var minion in _minions)
      {
        minion.Update(gameTime);
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
          foreach (var building in _buildings)
          {
            if (building.Rectangle.Intersects(_selectedBuilding.Rectangle))
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
              _buildings.Add(_selectedBuilding);
              _selectedBuilding.Initialise();

              Map.Add(_selectedBuilding.CollisionRectangle);
              Pathfinder.InitializeSearchNodes(Map); // We can't possibly call this everytime a minion enters a new tile..?

              ResourceManager.Use(_selectedBuilding.Resources);

              if (_selectedBuilding is House)
              {
                AddMinions(_selectedBuilding);
              }

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
        _minions.Add(new Minion(_content, new Vector2(building.Position.X + (x * 32), building.Rectangle.Bottom - 32), this));
      }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      // Camera stuff
      spriteBatch.Begin(/*SpriteSortMode.FrontToBack,*/ transformMatrix: _camera.Transform);

      Map.Draw(spriteBatch);

      foreach (var building in _buildings)
        building.Draw(spriteBatch);

      foreach (var minion in _minions)
        minion.Draw(gameTime, spriteBatch);

      if (_selectedBuilding != null)
      {
        _selectedBuilding.Color = Color.Green;
        _selectedBuilding.Draw(spriteBatch);
      }

      spriteBatch.End();

      spriteBatch.Begin();
      _resourceList.Draw(spriteBatch);
      _buildingSelector.Draw(spriteBatch);
      spriteBatch.End();
    }
  }
}
