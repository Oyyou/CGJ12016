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
    #region Fields

    private SpriteFont _font;

    private GameState _parent;

    private Rectangle _rectangle;

    private Texture2D _texture;

    #endregion

    #region Methods

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, _rectangle, Color.White);
      spriteBatch.DrawString(_font, $"Food: {_parent.ResourceManager.Food} | Wood: {_parent.ResourceManager.Wood} | Stone: {_parent.ResourceManager.Stone} | Gold: {_parent.ResourceManager.Gold}", new Vector2(5, 3), Color.Red);
    }

    public override string GetSaveData()
    {
      return "";
    }

    public ResourceList(GraphicsDevice graphicsDevice, SpriteFont font, GameState parent)
    {
      _texture = new Texture2D(graphicsDevice, 1, 1);
      _texture.SetData(new Color[] { Color.AliceBlue, });

      _font = font;
      _parent = parent;
    }

    public override void Update(GameTime gameTime)
    {
      _rectangle = new Rectangle(0, 0, Game1.ScreenWidth, 16);
    }

    #endregion
  }
}
