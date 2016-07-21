using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Controls
{
  public class MessageBox
  {
    private Texture2D _texture;
    private SpriteFont _font;
    private Vector2 _position;
    private string _text;
    private float _timer;
    private bool _isVisible = false;

    public MessageBox(ContentManager Content)
    {
      _texture = Content.Load<Texture2D>("Controls/MessageBox");
      _font = Content.Load<SpriteFont>("Fonts/Arial08pt");
      _position = new Vector2(400 - (_texture.Width / 2), 48);
    }

    public void Show(string text)
    {
      _text = text;
      _timer = 0;
      _isVisible = true;
    }

    public void Update(GameTime gameTime)
    {
      if (!_isVisible)
        return;

      _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (_timer > 3.0f)
      {
        _timer = 0.0f;
        _isVisible = false;
      }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      if (!_isVisible)
        return;

      spriteBatch.Draw(_texture, _position, Color.White);

      if (!string.IsNullOrEmpty(_text))
      {
        float x = (_position.X + (_texture.Width / 2)) - (_font.MeasureString(_text).X / 2);
        float y = (_position.Y + (_texture.Height / 2)) - (_font.MeasureString(_text).Y / 2);

        spriteBatch.DrawString(_font, _text, new Vector2(x, y), Color.Black);
      }
    }
  }
}
