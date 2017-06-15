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

    private Vector2 _currentTarget;

    private float _resourceCollectionTimer = 0f;

    private int _resourceMax = 20;

    private Models.Resources _resources = null;

    /// <summary>
    /// The tile we're gathering resources from
    /// </summary>
    private ResourceTile _resourceTile = null;

    private Vector2 _velocity;

    #endregion

    #region Properties

    public int? Home { get; set; }

    public Vector2 Target { get; private set; }

    public event EventHandler WorkEvent;

    public int? Workplace { get; set; }
    
    public Vector2 Velocity
    {
      get { return _velocity; }
      set { _velocity = value; }
    }

    #endregion

    #region Methods

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

      var home = _parent.Components.Where(c => c.Id == Home.Value).FirstOrDefault() as Building;

      if (Position == home.DoorPosition)
        IsVisible = false;

      // If they're already home, leave this method
      if (!IsVisible)
        return;

      // Go to the door position of 'Home'
      Move(home.DoorPosition);
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
        _resourceTile = null; // When the minion isn't working, it's targeted resource is set to null
        return;
      }

      var workplace = _parent.Components.Where(c => c.Id == Workplace.Value).FirstOrDefault() as Building;

      if (workplace.TileType == TileType.Farm ||
          workplace.TileType == TileType.Militia)
      {
        WorkEvent(this, new EventArgs());
        
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
            .Where(c => c.TileType == workplace.TileType)
            .OrderBy(c => Vector2.Distance(workplace.DoorPosition, c.Position)).ToArray()[i]; // Should be the closest

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

            switch (workplace.TileType)
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

        if (Position != workplace.DoorPosition)
          Move(workplace.DoorPosition); // return to resource building
        else
        {
          _parent.ResourceManager.Add(_resources);
        }
      }
    }

    #endregion
  }
}
