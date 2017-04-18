using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PleaseThem.Controllers;
using PleaseThem.States;
using Microsoft.Xna.Framework;
using PleaseThem.Tiles;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.Core;

namespace PleaseThem.Models
{
  public enum Occuptations
  {
    Unemployed,
    Lumber,
    Miner,
    Farmer,
  }

  public class Minion : Sprite
  {
    private Vector2 _currentTarget;

    private Vector2 _velocity;

    public Occuptations Occuptation { get; set; }

    // skills?
    public int Lumbering { get; set; }

    public int Mining { get; set; }

    public int Farming { get; set; }

    public Resources Resources { get; private set; }

    public Building Workplace { get; set; }

    public Building Home { get; set; }

    public Minion(GameState parent, AnimationController animationController) : base(parent, animationController)
    {

    }

    private void Move(Vector2 target)
    {
      float speed = 2;

      var node = target;

      // Needs to change target if reached current
      var needsTarget = Position.X % Map.TileSize == 0 &&
                        Position.Y % Map.TileSize == 0;

      if (needsTarget)
      {
        var paths = _parent.Pathfinder.FindPath(Position, target);

        if (paths.Count > 0)
          node = paths.FirstOrDefault() * Map.TileSize;

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

    public override void Update(GameTime gameTime)
    {
      _velocity = Vector2.Zero;

      Work(gameTime);

      ReturnHome(gameTime);

      SetAnimation();

      Position += _velocity;
    }

    /// <summary>
    /// Go home when unemployed
    /// </summary>
    /// <param name="gameTime"></param>
    private void ReturnHome(GameTime gameTime)
    {
      // If the minion is employed, leave this method
      if (Occuptation != Occuptations.Unemployed)
        return;

      var doorPosition = new Vector2(Home.Position.X + (Map.TileSize * 1), Home.Position.Y + Home.Height - Map.TileSize);

      if (Position == doorPosition)
        IsVisible = false;

      // If they're already home, leave this method
      if (!IsVisible)
        return;

      // Go to the door position of 'Home'
      Move(doorPosition);
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

    private void Work(GameTime gameTime)
    {
      if (Occuptation == Occuptations.Unemployed)
        return;

      IsVisible = true;

      // Walk to resource (if there is space around it)
      // Collect to maximum resources (or until it's depleted)
      // Walk to workplace
      // Rinse and repeat

      TileType tileType;
      switch (Occuptation)
      {
        case Occuptations.Lumber:
          tileType = TileType.Tree;
          break;
        case Occuptations.Miner:
          tileType = TileType.Stone;
          break;
        case Occuptations.Farmer:
          tileType = TileType.Farm;
          break;
        default:
          throw new NotImplementedException($"Please implement Occuption '{Occuptation.ToString()}'");
      }

      var resource = _parent.Map.ResourceTiles
        .Where(c => c.TileType == tileType)
        .OrderBy(c => Vector2.Distance(Workplace.Position, c.Position))
        .FirstOrDefault();

      // Need a way to not walk through minions.
    }
  }
}
