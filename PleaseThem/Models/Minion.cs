using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PleaseThem.Controllers;
using PleaseThem.States;
using Microsoft.Xna.Framework;
using PleaseThem.Tiles;

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

    private void Work(GameTime gameTime)
    {
      if (Occuptation == Occuptations.Unemployed)
        return;

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
