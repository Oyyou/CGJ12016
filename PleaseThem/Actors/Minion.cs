using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PleaseThem.Buildings;
using PleaseThem.Controllers;
using PleaseThem.Core;
using PleaseThem.States;
using PleaseThem.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Actors
{
  public class Minion : Models.Sprite
  {
    #region Fields

    private bool _atFarm;

    private Vector2 _currentTarget;

    private bool _farmingDown;

    private Vector2 _farmPos1;

    private Vector2 _farmPos2;

    private float _resourceCollectionTimer = 0f;

    private int _resourceMax = 20;

    private Models.Resources _resources = null;

    private ResourceTile _resourceTile = null;

    private Vector2 _velocity;

    #endregion

    #region Properties

    public Building Home { get; set; }

    public Vector2 Target { get; private set; }

    public event EventHandler Work { get; set; }

    public Building Workplace { get; set; }
    
    public Vector2 Velocity
    {
      get { return _velocity; }
      set { _velocity = value; }
    }

    #endregion

    #region Methods

    private void CombatTraining(GameTime gameTime)
    {
      var swordSchool = Workplace as SwordSchool;

      if (Target == Vector2.Zero)
      {
        var position = swordSchool.BuildingPositions.Where(c => !c.HasWorker).FirstOrDefault();
        position.HasWorker = true;

        Target = position.Positions[0];
      }

      if (Target != Position)
        Move(Target);
    }

    private void Farming(GameTime gameTime)
    {
      var farm = Workplace as Farm;

      if (_farmPos1 == Vector2.Zero)
      {
        var farmPosition = farm.FarmPositions.Where(c => !c.HasWorker).FirstOrDefault();
        farmPosition.HasWorker = true;

        _farmPos1 = farmPosition.Positions[0];
        _farmPos2 = farmPosition.Positions[1];
      }

      if (!_atFarm)
      {
        Move(_farmPos1);

        if (Position == _farmPos1)
          _atFarm = true;

        return;
      }

      if (Position == _farmPos1 ||
          Position == _farmPos2)
        _farmingDown = !_farmingDown;

      var node = _farmPos1;

      if (!_farmingDown)
        node = _farmPos2;

      float speed = 2;

      if (node != null)
      {
        if (Position.Y > node.Y)
          _velocity = new Vector2(0, -speed);
        else if (Position.Y < node.Y)
          _velocity = new Vector2(0, speed);
        else if (Position.X > node.X)
          _velocity = new Vector2(-speed, 0);
        else if (Position.X < node.X)
          _velocity = new Vector2(speed, 0);
      }

      _resourceCollectionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (_resourceCollectionTimer > 2.5f)
      {
        _resourceCollectionTimer = 0.0f;

        _resources.Food++;

        _parent.ResourceManager.Add(_resources);
      }
    }

    public Minion(GameState parent, AnimationController animationController)
      : base(parent, animationController)
    {
      _animationPlayer.Colour = Color.White;

      _resources = new Models.Resources();

      Layer = 0.8f;
    }

    public void Move(Vector2 target)
    {
      float speed = 2f;

      var node = target;

      var needsTarget = Position.X % Map.TileSize == 0 &&
                        Position.Y % Map.TileSize == 0;

      if (needsTarget)
      {
        var paths = _parent.Pathfinder.FindPath(Position, target);

        if (paths.Count > 0)
          node = paths.FirstOrDefault() * 32;

        _currentTarget = node;
      }
      else node = _currentTarget;

      if (node != null)
      {
        if (Position.Y > node.Y)
        {
          _velocity = new Vector2(0, -speed);
        }
        else if (Position.Y < node.Y)
        {
          _velocity = new Vector2(0, speed);
        }
        else if (Position.X > node.X)
        {
          _velocity = new Vector2(-speed, 0);
        }
        else if (Position.X < node.X)
        {
          _velocity = new Vector2(speed, 0);
        }
      }
    }

    private void Reset()
    {
      _velocity = new Vector2();

      if (Workplace != null)
        return;

      _animationPlayer.Colour = Color.White;
      _farmPos1 = Vector2.Zero;
      _farmPos2 = Vector2.Zero;
      _atFarm = false;
    }

    /// <summary>
    /// Go home when unemployed
    /// </summary>
    /// <param name="gameTime"></param>
    private void ReturnHome(GameTime gameTime)
    {
      // If the minion is employed, leave this method
      if (Workplace != null)
        return;

      if (Position == Home.DoorPosition)
        IsVisible = false;

      // If they're already home, leave this method
      if (!IsVisible)
        return;

      // Go to the door position of 'Home'
      Move(Home.DoorPosition);
    }

    private void SetAnimation()
    {
      if (_velocity.X > 0)
      {
        _animationPlayer.PlayAnimation(_animationController.WalkRight);
        _animationPlayer.Direction = SpriteEffects.None;
      }
      else if (_velocity.X < 0)
      {
        _animationPlayer.PlayAnimation(_animationController.WalkRight);
        _animationPlayer.Direction = SpriteEffects.FlipHorizontally;
      }
      else if (_velocity.Y < 0)
      {
        _animationPlayer.PlayAnimation(_animationController.WalkUp);
      }
      else if (_velocity.Y > 0)
      {
        _animationPlayer.PlayAnimation(_animationController.WalkDown);
      }
    }

    private void SetTarget(Vector2 side)
    {
      if (Target != Vector2.Zero)
        return;

      var available = _parent.Components.Where(c => c is Minion).All(c => ((Minion)c).Target != side);

      if (!available)
        return;

      var path = _parent.Pathfinder.FindPath(Position, side);

      if (path.Count > 0)
        Target = side;

    }

    public override void Update(GameTime gameTime)
    {
      Reset();

      Work(gameTime);

      ReturnHome(gameTime);

      SetAnimation();

      Position += _velocity;
    }

    private void Work(GameTime gameTime)
    {
      if (Workplace == null)
      {
        _resourceTile = null; // When the minion isn't working, it's targetted resource is set to null
        return;
      }

      if (Workplace.TileType == Tiles.TileType.Farm)
      {
        Farming(gameTime);

        return;
      }

      if (Workplace.TileType == TileType.Militia)
      {
        CombatTraining(gameTime);

        return;
      }

      var changeTarget = _resourceTile == null ||
                         (_resourceTile != null && _resourceTile.ResourceCount <= 0) || // if the resource was emptied by a previous minion
                         (_resources.GetTotal() == 0 && Target == Vector2.Zero);

      // More efficent way
      // Give each building a list of potential positions
      // Update when it starts to run out

      if (changeTarget)
      {
        Target = Vector2.Zero;

        int i = 0;
        while (Target == Vector2.Zero)
        {
          _resourceTile = _parent.Map.ResourceTiles
            .Where(c => c.TileType == Workplace.TileType)
            .OrderBy(c => Vector2.Distance(Workplace.DoorPosition, c.Position)).ToArray()[i]; // Should be the closest

          var left = _resourceTile.Position - new Vector2(32, 0);
          var right = _resourceTile.Position + new Vector2(32, 0);
          var up = _resourceTile.Position - new Vector2(0, 32);
          var down = _resourceTile.Position + new Vector2(0, 32);

          // Check to see if either of the 4 sides are accessible]
          if (left.X >= 0)
            SetTarget(left);

          if (right.X < (_parent.Map.Width * 32))
            SetTarget(right);

          if (up.Y >= 0)
            SetTarget(up);

          if (down.Y < (_parent.Map.Height * 32))
            SetTarget(down);

          i++;
        }
      }

      if (_resources.GetTotal() < _resourceMax) // Can we get moar resources!?
      {
        if (Vector2.Distance(Position, Target) > 0) // Only move if we're 1+ tile away from the resource
        {
          Move(Target);
        }
        else
        {
          _resourceCollectionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

          if (Position.Y > _resourceTile.Position.Y)
          {
            _animationPlayer.PlayAnimation(_animationController.WalkUp);
          }
          else if (Position.Y < _resourceTile.Position.Y)
          {
            _animationPlayer.PlayAnimation(_animationController.WalkDown);
          }
          else if (Position.X > _resourceTile.Position.X)
          {
            _animationPlayer.PlayAnimation(_animationController.WalkRight);
            _animationPlayer.Direction = SpriteEffects.FlipHorizontally;
          }
          else if (Position.X < _resourceTile.Position.X)
          {
            _animationPlayer.PlayAnimation(_animationController.WalkRight);
            _animationPlayer.Direction = SpriteEffects.None;
          }

          if (_resourceCollectionTimer > 0.5f)
          {
            _resourceCollectionTimer = 0.0f;

            switch (Workplace.TileType)
            {
              case Tiles.TileType.Tree:
                _resources.Wood++;
                break;
              case Tiles.TileType.Stone:
                _resources.Stone++;
                break;
              default:
                break;
            }

            _resourceTile.ResourceCount -= 1;

            if (_resourceTile.ResourceCount <= 0)
            {
              _parent.Map.ResourceTiles.Remove(_resourceTile);
              _parent.Map.Remove(_resourceTile.Position);
              _parent.Pathfinder.InitializeSearchNodes(_parent.Map);
            }
          }
        }
      }
      else
      {
        Target = Vector2.Zero;

        if (Position != Workplace.DoorPosition)
          Move(Workplace.DoorPosition); // return to resource building
        else
        {
          _parent.ResourceManager.Add(_resources);
        }
      }
    }

    #endregion
  }
}
