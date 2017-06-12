using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem
{
  public abstract class Component : ICloneable
  {
    public bool IsRemoved { get; set; }

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    public abstract void Update(GameTime gameTime);

    public object Clone()
    {
      return this.MemberwiseClone();
    }
  }
}
