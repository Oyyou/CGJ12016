using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.States;

namespace PleaseThem.Models
{
  public class Building : Sprite
  {
    public Occuptations Occuptation { get; set; }

    public int MinionCount { get; set; }

    public int MinionCountMax { get; set; }

    public Building(GameState parent, Texture2D texture) : base(parent, texture)
    {
    }
  }
}
