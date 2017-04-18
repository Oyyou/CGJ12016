using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PleaseThem.Buildings;
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
  public class Minion
  {
    private AnimationPlayer _animationPlayer;
    private Animation _walkingRight;
    private Animation _walkingUp;
    private Animation _walkingDown;
    private Vector2 _velocity;
    private Vector2 _idlePosition;

    public Building Employment { get; private set; }

    private GameState _parent;

    private int _resourceCount = 0;
    private int _resourceMax = 20;
    private float _resourceCollection = 0f;

    public Vector2 Position { get; private set; }

    public Vector2 Target { get; private set; }

    private ResourceTile _resource = null;

    public Minion(ContentManager Content, Vector2 position, GameState parent)
    {
      _animationPlayer = new AnimationPlayer();
      _walkingRight = new Animation(Content.Load<Texture2D>("Actors/Minion/WalkingRight"), 4, 0.2f, true);
      _walkingUp = new Animation(Content.Load<Texture2D>("Actors/Minion/WalkingUp"), 4, 0.2f, true);
      _walkingDown = new Animation(Content.Load<Texture2D>("Actors/Minion/WalkingDown"), 4, 0.2f, true);
      Position = position;
      _idlePosition = Position;
      _parent = parent;

      _animationPlayer.PlayAnimation(_walkingDown);
      _animationPlayer.Color = Color.White;
    }

    public void Update(GameTime gameTime)
    {
      _velocity = new Vector2();
      Work(gameTime);
      Return(gameTime);

      if (_velocity.X > 0)
      {
        _animationPlayer.PlayAnimation(_walkingRight);
        _animationPlayer.Direction = SpriteEffects.None;
      }
      else if (_velocity.X < 0)
      {
        _animationPlayer.PlayAnimation(_walkingRight);
        _animationPlayer.Direction = SpriteEffects.FlipHorizontally;
      }
      else if (_velocity.Y < 0)
        _animationPlayer.PlayAnimation(_walkingUp);
      else if (_velocity.Y > 0)
        _animationPlayer.PlayAnimation(_walkingDown);

      Position += _velocity;
    }

    private void Work(GameTime gameTime)
    {
      if (Employment == null)
        return;

      if (Employment.TileType == Tiles.TileType.Farm)
      {
        Farming(gameTime);
        return;
      }

      var changeTarget = (_resource != null && _resource.ResourceCount <= 0) || 
                         (_resourceCount == 0 && Target == Vector2.Zero);

      // More efficent way
      // Give each building a list of potential positions
      // Update when it starts to run out

      if (changeTarget)
      {
        Target = Vector2.Zero;

        int i = 0;
        while (Target == Vector2.Zero)
        {
          _resource = _parent.Map.ResourceTiles
            .Where(c => c.TileType == Employment.TileType)
            .OrderBy(c => Vector2.Distance(Employment.Position, c.Position)).ToArray()[i]; // Should be the closest

          var left = _resource.Position - new Vector2(32, 0);
          var right = _resource.Position + new Vector2(32, 0);
          var up = _resource.Position - new Vector2(0, 32);
          var down = _resource.Position + new Vector2(0, 32);

          // Check to see if either of the 4 sides are accessible
          SetTarget(left);
          SetTarget(right);
          SetTarget(up);
          SetTarget(down);

          i++;
        }
      }

      if (_resourceCount < _resourceMax) // Can we get moar resources!?
      {
        if (Vector2.Distance(Position, Target) > 0) // Only move if we're 1+ tile away from the resource
        {
          Move(Target);
        }
        else
        {
          _resourceCollection += (float)gameTime.ElapsedGameTime.TotalSeconds;

          if (Position.Y > _resource.Position.Y)
          {
            _animationPlayer.PlayAnimation(_walkingUp);
          }
          else if (Position.Y < _resource.Position.Y)
          {
            _animationPlayer.PlayAnimation(_walkingDown);
          }
          else if (Position.X > _resource.Position.X)
          {
            _animationPlayer.PlayAnimation(_walkingRight);
            _animationPlayer.Direction = SpriteEffects.FlipHorizontally;
          }
          else if (Position.X < _resource.Position.X)
          {
            _animationPlayer.PlayAnimation(_walkingRight);
            _animationPlayer.Direction = SpriteEffects.None;
          }

          if (_resourceCollection > 2.5f)
          {
            _resourceCollection = 0.0f;
            _resourceCount++;
            _resource.ResourceCount -= 1;

            if (_resource.ResourceCount <= 0)
            {
              _parent.Map.ResourceTiles.Remove(_resource);
              _parent.Map.Remove(_resource.Position);
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
          switch (Employment.TileType)
          {
            case Tiles.TileType.Tree:
              _parent.ResourceManager.Add(new Models.Resources() { Wood = _resourceCount });
              break;
            case Tiles.TileType.Stone:
              _parent.ResourceManager.Add(new Models.Resources() { Stone = _resourceCount });
              break;
            default:
              break;
          }

          _resourceCount = 0;
        }
      }
    }

    private void SetTarget(Vector2 side)
    {
      if (Target != Vector2.Zero)
        return;

      var available = _parent._minions.All(c => c.Target != side);

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

      _resourceCollection += (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (_resourceCollection > 2.5f)
      {
        _resourceCollection = 0.0f;
        _parent.ResourceManager.Add(new Models.Resources() { Food = 1, });
      }
    }

    private void Return(GameTime gameTime)
    {
      if (Employment != null)
        return;

      if (Position == _idlePosition)
      {
        _animationPlayer.PlayAnimation(_walkingDown);
        return;
      }

      Move(_idlePosition);
    }

    private float _distanceTravelled = 0f;
    private Vector2 _currentTarget;
    private void Move(Vector2 target)
    {
      float speed = 2;

      var node = target;

      if (_distanceTravelled == 0)
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

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      _animationPlayer.Draw(gameTime, spriteBatch, Position);
    }
  }
}
