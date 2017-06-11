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
    private MouseState _currentMouse;

    private SpriteFont _font;

    private MouseState _previousMouse;

    private Texture2D _texture;

    public event EventHandler Click;

    public Color Color { get; set; }

    public bool IsClicked { get; private set; }

    public bool IsHovering { get; private set; }

    public Vector2 Position;

    public Rectangle Rectangle { get; private set; }

    public bool Selected { get; set; }

    public string Text { get; set; }

    public Button(Texture2D texture, SpriteFont font, Vector2 position, string text = "")
    {
      _texture = texture;

      _font = font;

      Position = position;

      Rectangle = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

      Color = Color.Black;

      Text = text;

      Selected = false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      if (!Selected)
      {
        if (IsHovering)
        {
          spriteBatch.Draw(_texture, Rectangle, Color.Gray);
        }
        else
        {
          spriteBatch.Draw(_texture, Rectangle, Color.White);
        }
      }
      else
      {
        spriteBatch.Draw(_texture, Rectangle, Color.Yellow);
      }

      if (!string.IsNullOrEmpty(Text))
      {
        float x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
        float y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

        spriteBatch.DrawString(_font, Text, new Vector2(x, y), Color);
      }
    }

    public void Update()
    {
      Rectangle = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

      _previousMouse = _currentMouse;
      _currentMouse = Mouse.GetState();

      var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

      IsClicked = false;

      IsHovering = false;

      if (mouseRectangle.Intersects(Rectangle))
      {
        IsHovering = true;

        if (_currentMouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
        {
          IsClicked = true;
                    
          Click?.Invoke(this, new EventArgs());
        }
      }
    }
  }
}
