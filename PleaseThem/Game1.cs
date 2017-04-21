using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PleaseThem.Controls;
using PleaseThem.Core;
using PleaseThem.States;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PleaseThem
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class Game1 : Game
  {
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    public GameState GameState;

    public MenuState MenuState;

    public static MessageBox MessageBox;

    public static Random Random;

    public static int ScreenHeight;

    public static int ScreenWidth;

    public Game1()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      IsMouseVisible = true;

      ScreenHeight = graphics.PreferredBackBufferHeight;

      ScreenWidth = graphics.PreferredBackBufferWidth;

      Window.AllowUserResizing = true;
      Window.ClientSizeChanged += Window_ClientSizeChanged;

      base.Initialize();
    }

    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
      if (graphics.PreferredBackBufferHeight < 480)
      {
        graphics.PreferredBackBufferHeight = 480;
        graphics.ApplyChanges();
      }

      if (graphics.PreferredBackBufferHeight > 1440)
      {
        graphics.PreferredBackBufferHeight = 1440;
        graphics.ApplyChanges();
      }

      if (graphics.PreferredBackBufferWidth < 800)
      {
        graphics.PreferredBackBufferWidth = 800;
        graphics.ApplyChanges();
      }

      if (graphics.PreferredBackBufferWidth > 2560)
      {
        graphics.PreferredBackBufferWidth = 2560;
        graphics.ApplyChanges();
      }

      ScreenHeight = graphics.PreferredBackBufferHeight;
      ScreenWidth = graphics.PreferredBackBufferWidth;
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      spriteBatch = new SpriteBatch(GraphicsDevice);

      MessageBox = new MessageBox(Content);

      Random = new Random();

      MenuState = new MenuState(Content);
      GameState = new GameState(graphics.GraphicsDevice, Content);

      MenuState.IsActive = true;
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      if (MenuState.IsActive)
      {
        MenuState.Update(gameTime);

        MenuState.PostUpdate(gameTime);

        if (MenuState.Next)
        {
          MenuState.IsActive = false;
          GameState.IsActive = true;
        }

        if (MenuState.Quit)
          this.Exit();
      }
      else if(GameState.IsActive)
      {
        GameState.Update(gameTime);

        GameState.PostUpdate(gameTime);

        if (GameState.Quit)
        {
          GameState.IsActive = false;
          MenuState.IsActive = true;
        }
      }

      MessageBox.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      if (MenuState.IsActive)
      {
        MenuState.Draw(gameTime, spriteBatch);
      }
      else if (GameState.IsActive)
      {
        GameState.Draw(gameTime, spriteBatch);
      }

      spriteBatch.Begin();
      MessageBox.Draw(spriteBatch);
      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
