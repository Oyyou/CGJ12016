using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Tiles
{
  public enum TileType
  {
    Grass,
    Tree,
    Stone,
    Farm,
    Militia,
    Occupied,
  }

  public class Tile
  {
    protected Texture2D Texture;
    public Vector2 Position { get; protected set; }
    public Rectangle Rectangle
    {
      get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
    }

    protected float Layer = 0.0f;
    public TileType TileType { get; protected set; }

    public Tile(Texture2D texture, Vector2 position, TileType tileType)
    {
      Texture = texture;
      Position = position;
      TileType = tileType;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);
    }
  }
}
