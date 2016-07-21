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
  public class ResourceList
  {
    private Texture2D _texture;
    private SpriteFont _font;
    private Vector2 _position;

    private GameState _parent;

    public ResourceList(Texture2D texture, SpriteFont font, GameState parent)
    {
      _texture = texture;
      _font = font;
      _parent = parent;

      _position = new Vector2(0, 0);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, _position, Color.White);
      spriteBatch.DrawString(_font, $"Food: {_parent.FoodCount} | Wood: {_parent.WoodCount} | Stone: {_parent.StoneCount} | Gold: {_parent.GoldCount}", new Vector2(5, 5), Color.Red);
    }
  }
}
