using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Controls
{
  public class ResourceList : Component
  {
    private SpriteFont _font;

    private GameState _parent;

    private Rectangle _rectangle;

    private Texture2D _texture;

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, _rectangle, Color.White);
      spriteBatch.DrawString(_font, $"Food: {_parent.ResourceManager.Food} | Wood: {_parent.ResourceManager.Wood} | Stone: {_parent.ResourceManager.Stone} | Gold: {_parent.ResourceManager.Gold}", new Vector2(5, 5), Color.Red);
    }

    public ResourceList(GraphicsDevice graphicsDevice, SpriteFont font, GameState parent)
    {
      _texture = new Texture2D(graphicsDevice, 1, 1);
      _texture.SetData(new Color[] { new Color(255, 255, 255), });

      _font = font;
      _parent = parent;
    }

    public override void Update(GameTime gameTime)
    {
      _rectangle = new Rectangle(0, 0, Game1.ScreenWidth, 32);
    }
  }
}
