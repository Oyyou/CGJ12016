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

    public State NextState { get; private set; }

    public static MessageBox MessageBox;

    public static Random Random;

    public static int ScreenHeight;

    public static int ScreenWidth;

    public State State { get; private set; }

    public void ChangeState(State state)
    {
      NextState = state;
    }

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
      //graphics.PreparingDeviceSettings += Graphics_PreparingDeviceSettings;
      Window.ClientSizeChanged += Window_ClientSizeChanged;

      base.Initialize();
    }

    private void Graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
    {
      e.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.HiDef;
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

      State = new MenuState(this, graphics.GraphicsDevice, Content);
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

      if (NextState != null)
      {
        State = NextState;
        NextState = null;
      }

      State.Update(gameTime);

      State.PostUpdate(gameTime);

      MessageBox.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      State.Draw(gameTime, spriteBatch);

      spriteBatch.Begin();
      MessageBox.Draw(spriteBatch);
      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
