using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.States;
using System.Linq;
using PleaseThem.Tiles;

namespace PleaseThem.Models.Labour
{
  public class LabourBuilding : Models.Building
  {
    public List<Resource> Resources { get; private set; }

    public TileType TileType { get; set; }

    public LabourBuilding(GameState parent, Texture2D texture) : base(parent, texture)
    {
      Resources = new List<Resource>();
    }

    public override void Update(GameTime gameTime)
    {
      // Do a check for more resources in the area
      if (Resources.Count < 2)
      {
        var amount = _parent.Map.Resources
          .Where(c => c.TileType == this.TileType)
          .ToList()
          .GetRange(0, _parent.Map.Resources.Count > 5 ? 5 : _parent.Map.Resources.Count);

        Resources = _parent.Map.Resources
          .Where(c => c.TileType == this.TileType)
          .OrderBy(c => Vector2.Distance(this.Position, c.Position)).ToList().GetRange(0, _parent.Map.Resources.Count > 5 ? 5 : _parent.Map.Resources.Count);
      }
    }
  }
}
