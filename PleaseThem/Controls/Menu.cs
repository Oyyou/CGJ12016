using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PleaseThem.Controls
{
  public class Menu
  {
    private string[] _content;

    private SpriteFont _font;

    private Rectangle _rectangle;

    private Texture2D _texture;

    public bool IsVisible { get; set; }

    public Menu(ContentManager Content)
    {
      _texture = Content.Load<Texture2D>("Controls/Menu");
      _font = Content.Load<SpriteFont>("Fonts/Arial08pt");

      IsVisible = false;
    }

    public void Update(params string[] content)
    {
      _content = content;
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle parent)
    {
      if (!IsVisible)
        return;

      _rectangle = new Rectangle(parent.X + (Math.Abs(parent.Width - _texture.Width) / 2),
                                 parent.Y - 5 - _texture.Height, _texture.Width, _texture.Height);

      if (_rectangle.Y < 32)
        _rectangle.Y = (parent.Bottom - 32) + 5; // The 32 is the 'whitespace' I have under buildings.

      spriteBatch.Draw(_texture, _rectangle, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0.85f);

      Vector2 fontPosition = new Vector2(_rectangle.X + 10, _rectangle.Y + 10);
      foreach (var content in _content)
      {
        spriteBatch.DrawString(_font, content, fontPosition, Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.9f);

        var y = _font.MeasureString(content).Y;

        fontPosition.Y += y + 5;
      }
    }
  }
}
