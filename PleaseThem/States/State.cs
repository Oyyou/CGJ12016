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
    #region Fields 

    protected ContentManager _content;

    protected GraphicsDevice _graphicsDevice;

    protected Game1 _game;

    #endregion

    #region Methods

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    public abstract void PostUpdate(GameTime gameTime);

    public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager Content)
    {
      _game = game;

      _graphicsDevice = graphicsDevice;

      _content = Content;
    }

    public abstract void UnloadContent();

    public abstract void Update(GameTime gameTime);

    #endregion
  }
}
