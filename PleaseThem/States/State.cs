﻿using Microsoft.Xna.Framework;
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

    #region Properties

    public Color BackgroundColour { get; protected set; }

    #endregion

    #region Methods

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    public abstract void PostUpdate(GameTime gameTime);

    public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
    {
      _game = game;

      _graphicsDevice = graphicsDevice;

      _content = content;

      BackgroundColour = Color.Black;
    }

    public abstract void Update(GameTime gameTime);

    #endregion
  }
}
