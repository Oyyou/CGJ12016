using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Controls
{
  public class Button
  {
    private Texture2D _texture;
    private SpriteFont _font;
    private Vector2 _position;
    public Rectangle Rectangle { get; private set; }

    public string Text { get; set; }
    public Color Color { get; set; }
    public bool Clicked { get; private set; }
    public bool Selected { get; set; }

    private MouseState _currentMouse;
    private MouseState _previousMouse;

    public Button(Texture2D texture, SpriteFont font, Vector2 position, string text = "")
    {
      _texture = texture;
      _font = font;
      _position = position;

      Rectangle = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
      Color = Color.Black;
      Text = text;
      Selected = false;
    }

    public void Update()
    {
      _previousMouse = _currentMouse;
      _currentMouse = Mouse.GetState();

      var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

      Clicked = false;

      if (mouseRectangle.Intersects(Rectangle))
      {
        if (_currentMouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
        {
          Clicked = true;
        }
      }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      if (!Selected)
        spriteBatch.Draw(_texture, Rectangle, Color.White);
      else
        spriteBatch.Draw(_texture, Rectangle, Color.Yellow);

      if (!string.IsNullOrEmpty(Text))
      {
        float x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
        float y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

        spriteBatch.DrawString(_font, Text, new Vector2(x, y), Color);
      }
    }
  }
}
