using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PleaseThem.Controls;
using PleaseThem.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Buildings
{
  public class FarmPosition
  {
    public bool Working { get; set; }
    public List<Vector2> Positions { get; set; }
  }

  public class Farm : Building
  {
    public List<FarmPosition> FarmPositions { get; set; }

    public override Rectangle CollisionRectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, 64, 96);
      }
    }

    public Farm(GameState parent, Texture2D texture)
      : base(parent, texture)
    {
      Parent = parent;

      Menu = new Menu(parent.Content);

      Resources = new Models.Resources()
      {
        Food = 25,
        Wood = 50,
        Stone = 0,
        Gold = 0,
      };

      MinionColor = Color.Olive;
      TileType = Tiles.TileType.Farm;
      MaxMinions = 3;

      FarmPositions = new List<FarmPosition>();
    }

    public override void Initialise()
    {
      base.Initialise();

      FarmPositions.Add(new FarmPosition()
      {
        Working = false,
        Positions = new List<Vector2>()
        {
          new Vector2(Position.X + 32, Rectangle.Top + 96),
          new Vector2(Position.X + 32, Rectangle.Bottom - 32)
        },
      });

      FarmPositions.Add(new FarmPosition()
      {
        Working = false,
        Positions = new List<Vector2>()
        {
          new Vector2(Position.X + 96, Rectangle.Top),
          new Vector2(Position.X + 96, Rectangle.Bottom - 32)
        },
      });

      FarmPositions.Add(new FarmPosition()
      {
        Working = false,
        Positions = new List<Vector2>()
        {
          new Vector2(Position.X + 160, Rectangle.Top),
          new Vector2(Position.X + 160, Rectangle.Bottom - 32)
        },
      });
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      Menu.Update($"Workers: {CurrentMinions}/{MaxMinions}");
    }
  }
}
