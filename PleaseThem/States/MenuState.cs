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

    private Button _play;
    private Button _quit;

    public MenuState(ContentManager Content)
      : base(Content)
    {
      _texture = Content.Load<Texture2D>("Background");
      _position = new Vector2(0, 0);

      Texture2D buttonTexture = Content.Load<Texture2D>("Controls/Button");
      SpriteFont font = Content.Load<SpriteFont>("Fonts/Arial08pt");

      _play = new Button(buttonTexture, font, new Vector2(336, 300));
      _quit = new Button(buttonTexture, font, new Vector2(336, 350));

      _play.Text = "Play";
      _quit.Text = "Quit";
    }

    public override void PostUpdate(GameTime gameTime)
    {
      
    }

    public override void Update(GameTime gameTime)
    {
      _play.Update();
      _quit.Update();

      Next = _play.Clicked;
      Quit = _quit.Clicked;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin();

      spriteBatch.Draw(_texture, _position, Color.White);
      _play.Draw(spriteBatch);
      _quit.Draw(spriteBatch);

      spriteBatch.End();
    }
  }
}
