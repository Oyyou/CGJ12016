using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.States;
using Microsoft.Xna.Framework;
using PleaseThem.Tiles;

namespace PleaseThem.Models
{
  public class Resource : Sprite
  {
    public List<Vector2> FreeSpaces { get; set; }

    public TileType TileType { get; set; }

    public Resource(GameState parent, Texture2D texture)
      : base(parent, texture)
    {
      FreeSpaces = new List<Vector2>();
    }
  }
}
