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
    private Texture2D _texture;
    private Vector2 _position;

    private Button _newGame;
    private Button _loadGame;
    private Button _quit;

    public MenuState(ContentManager Content)
      : base(Content)
    {
      _texture = Content.Load<Texture2D>("Background");
      _position = new Vector2(0, 0);

      Texture2D buttonTexture = Content.Load<Texture2D>("Controls/Button");
      SpriteFont font = Content.Load<SpriteFont>("Fonts/Arial08pt");

      _newGame = new Button(buttonTexture, font, new Vector2(336, 300));
      _loadGame = new Button(buttonTexture, font, new Vector2(336, 350));
      _quit = new Button(buttonTexture, font, new Vector2(336, 400));

      _newGame.Text = "New Game";
      _loadGame.Text = "New Game";
      _quit.Text = "Quit";

      _newGame.Click += _newGame_Click;
    }

    private void _newGame_Click(object sender, EventArgs e)
    {
      throw new NotImplementedException();
    }

    public override void PostUpdate(GameTime gameTime)
    {
      
    }

    public override void Update(GameTime gameTime)
    {
      _newGame.Update();
      _loadGame.Update();
      _quit.Update();

      Next = _newGame.IsClicked;

      Quit = _quit.IsClicked;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin();

      spriteBatch.Draw(_texture, _position, Color.White);

      _newGame.Draw(spriteBatch);
      _quit.Draw(spriteBatch);

      spriteBatch.End();
    }
  }
}
