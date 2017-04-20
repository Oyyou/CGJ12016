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
    private Vector2 _velocity;
    private Vector2? _idlePosition = null;

    public Building Employment { get; private set; }

    private int _resourceMax = 20;
    private float _resourceCollectionTimer = 0f;
    
    public Vector2 Target { get; private set; }

    private ResourceTile _resourceTile = null;

    private Models.Resources _resources = null;

    public Minion(GameState parent, AnimationController animationController)
      : base(parent, animationController)
    {
      _animationPlayer = new AnimationPlayer();

      _animationPlayer.Color = Color.White;

      _resources = new Models.Resources();
    }

    public override void Update(GameTime gameTime)
    {
      if (_idlePosition == null)
        _idlePosition = Position;

      _velocity = new Vector2();
      Work(gameTime);
      Return(gameTime);

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

      Position += _velocity;
    }

    private void Work(GameTime gameTime)
    {
      if (Employment == null)
      {
        _resourceTile = null; // When the minion isn't working, it's targetted resource is set to null
        return;
      }

      if (Employment.TileType == Tiles.TileType.Farm)
      {
        Farming(gameTime);
        return;
      }

      var changeTarget = _resourceTile == null ||
                         (_resourceTile != null && _resourceTile.ResourceCount <= 0) ||
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
            .Where(c => c.TileType == Employment.TileType)
            .OrderBy(c => Vector2.Distance(Employment.Position, c.Position)).ToArray()[i]; // Should be the closest

          var left = _resourceTile.Position - new Vector2(32, 0);
          var right = _resourceTile.Position + new Vector2(32, 0);
          var up = _resourceTile.Position - new Vector2(0, 32);
          var down = _resourceTile.Position + new Vector2(0, 32);

          // Check to see if either of the 4 sides are accessible
          SetTarget(left);
          SetTarget(right);
          SetTarget(up);
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

          if (_resourceCollectionTimer > 2.5f)
          {
            _resourceCollectionTimer = 0.0f;

            switch (Employment.TileType)
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
        var doorPosition = new Vector2(Employment.Position.X + 64, Employment.Position.Y + 96);

        if (Vector2.Distance(Position, doorPosition) > 32)
          Move(doorPosition); // return to resource building
        else
        {
          _parent.ResourceManager.Add(_resources);
        }
      }
    }

    private void SetTarget(Vector2 side)
    {
      if (Target != Vector2.Zero)
        return;

      var available = _parent._components.Where(c => c is Minion).All(c => ((Minion)c).Target != side);

      if (!available)
        return;

      var path = _parent.Pathfinder.FindPath(Position, side);

      if (path.Count > 0)
        Target = side;

    }

    private Vector2 _farmPos1;
    private Vector2 _farmPos2;
    private bool _farmingDown;
    private void Farming(GameTime gameTime)
    {
      var farm = Employment as Farm;

      if (_farmPos1 == Vector2.Zero)
      {
        var farmPosition = farm.FarmPositions.Where(c => !c.Working).FirstOrDefault();
        farmPosition.Working = true;

        _farmPos1 = farmPosition.Positions[0];
        _farmPos2 = farmPosition.Positions[1];
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

    private void Return(GameTime gameTime)
    {
      if (Employment != null)
        return;

      if (Position == _idlePosition)
      {
        _animationPlayer.PlayAnimation(_animationController.WalkDown);
        return;
      }

      Move((Vector2)_idlePosition);
    }

    private float _distanceTravelled = 0f;
    private Vector2 _currentTarget;
    private void Move(Vector2 target)
    {
      float speed = 2;

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

      _distanceTravelled += speed;
      if (_distanceTravelled > 32)
        _distanceTravelled = 0;
    }

    public void Employ(Building building)
    {
      Employment = building;
      _animationPlayer.Color = building.MinionColor;
    }

    public void Unemploy()
    {
      Employment = null;
      _animationPlayer.Color = Color.White;
      _farmPos1 = Vector2.Zero;
      _farmPos2 = Vector2.Zero;
    }

    //public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    //{
    //  _animationPlayer.Draw(gameTime, spriteBatch, Position);
    //}
  }
}
