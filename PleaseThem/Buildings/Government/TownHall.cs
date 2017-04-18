using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.States;
using Microsoft.Xna.Framework;

namespace PleaseThem.Buildings.Government
{
  public class TownHall : Models.Building
  {
    public override Rectangle CollisionRectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y + 32, _texture.Width, 96);
      }
    }

    public TownHall(GameState parent, Texture2D texture)
      : base(parent, texture)
    {
    }
  }
}
