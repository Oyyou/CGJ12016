using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using PleaseThem.States;
using PleaseThem.Buildings;

namespace PleaseThem.Controls
{
  public class Menu : Component
  {
    private SpriteFont _font;

    private GameState _parent;

    private Rectangle _rectangle;

    private Texture2D _texture;

    public Menu(ContentManager Content, GameState parent)
    {
      _texture = Content.Load<Texture2D>("Controls/Menu");
      _font = Content.Load<SpriteFont>("Fonts/Arial08pt");

      _parent = parent;
    }

    public override void Update(GameTime gameTime)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      var building = _parent.Components
        .Where(c => c is Building)
        .Where(c => ((Building)c).IsHovering)
        .FirstOrDefault() as Building;

      if (building == null)
        return;

      if (building.Content.Count() == 0)
        return;

      _rectangle = new Rectangle(building.Rectangle.X + ((building.Width - _texture.Width) / 2),
                                 building.Rectangle.Y - 5 - _texture.Height, _texture.Width, _texture.Height);

      if (_rectangle.Y < 32)
        _rectangle.Y = (building.Rectangle.Bottom - 32) + 5; // The 32 is the 'whitespace' I have under buildings.

      spriteBatch.Draw(_texture, _rectangle, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0.85f);

      Vector2 fontPosition = new Vector2(_rectangle.X + 10, _rectangle.Y + 10);
      foreach (var content in building.Content)
      {
        spriteBatch.DrawString(_font, content, fontPosition, Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.9f);

        var y = _font.MeasureString(content).Y;

        fontPosition.Y += y + 5;
      }
    }
  }
}
