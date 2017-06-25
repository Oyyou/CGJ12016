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
    #region Fields
      
    private bool _beenSeen = false;

    protected Texture2D Texture;
    
    #endregion
      
    #region Properties

    public bool IsVisible = false;

    protected float Layer = 0.0f;
    
    public Vector2 Position { get; protected set; }
    
    public Rectangle Rectangle
    {
      get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
    }

    public TileType TileType { get; protected set; }
    
    #endregion
    
    #region Methods

    public void Draw(SpriteBatch spriteBatch)
    {
      var opcity = 0f;

      if (!IsVisible)
      {
        if (!_beenSeen)
          return;
        else
          opcity = 0.5f;
      }
      else
      {
        opcity = 1f;
        _beenSeen = true;
      }

      spriteBatch.Draw(Texture, Position, null, Color.White * opcity, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);
    }

    public Tile(Texture2D texture, Vector2 position, TileType tileType)
    {
      Texture = texture;
      
      Position = position;
      
      TileType = tileType;
    }
    
    #endregion
  }
}
