using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PleaseThem.Tiles
{
  public class ResourceTile : Tile
  {
    public int ResourceCount { get; set; }

    public ResourceTile(Texture2D texture, Vector2 position, TileType tileType)
    : base(texture, position, tileType)
    {
      Layer = 0.1f;
      ResourceCount = 100;
    }
  }
}
