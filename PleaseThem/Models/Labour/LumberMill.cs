using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.States;

namespace PleaseThem.Models.Labour
{
  public class LumberMill : LabourBuilding
  {
    public LumberMill(GameState parent, Texture2D texture) 
     : base(parent, texture)
    {
    }
  }
}
