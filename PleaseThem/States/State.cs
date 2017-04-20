using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.States
{
  public abstract class State
  {
    public bool Next { get; set; }
    public bool Quit { get; set; }
    public bool IsActive { get; set; }

    public State(ContentManager Content)
    {
      Next = false;
      Quit = false;
      IsActive = false;
    }

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    public abstract void PostUpdate(GameTime gameTime);

    public abstract void Update(GameTime gameTime);
  }
}
