using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.Controls;

namespace PleaseThem.States
{
  public class MenuState : State
  {
    #region Fields

    private List<Button> _buttons;

    private Vector2 _position;

    private Texture2D _texture;

    #endregion

    #region Methods

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin();

      spriteBatch.Draw(_texture, _position, Color.White);

      foreach (var button in _buttons)
        button.Draw(spriteBatch);

      spriteBatch.End();
    }

    private void LoadGameClick(object sender, EventArgs e)
    {
      throw new NotImplementedException();
    }

    public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager Content)
      : base(game, graphicsDevice, Content)
    {
      _texture = Content.Load<Texture2D>("Background");
      _position = new Vector2(0, 0);

      Texture2D buttonTexture = Content.Load<Texture2D>("Controls/Button");
      SpriteFont font = Content.Load<SpriteFont>("Fonts/Arial08pt");

      var _newGame = new Button(buttonTexture, font, new Vector2(336, 300));
      var _loadGame = new Button(buttonTexture, font, new Vector2(336, 350));
      var _quit = new Button(buttonTexture, font, new Vector2(336, 400));

      _newGame.Text = "New Game";
      _loadGame.Text = "Load Game";
      _quit.Text = "Quit";

      _newGame.Click += NewGameClick;
      _loadGame.Click += LoadGameClick;
      _quit.Click += QuitClick;

      _buttons = new List<Button>()
      {
        _newGame,
        _loadGame,
        _quit,
      };
    }

    private void NewGameClick(object sender, EventArgs e)
    {
      _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
    }

    public override void PostUpdate(GameTime gameTime)
    {

    }

    public override void Update(GameTime gameTime)
    {
      foreach (var button in _buttons)
        button.Update();      
    }

    private void QuitClick(object sender, EventArgs e)
    {
      _game.Exit();
    }

    #endregion
  }
}
