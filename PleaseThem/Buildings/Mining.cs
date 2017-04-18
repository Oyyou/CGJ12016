using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PleaseThem.Controls;
using PleaseThem.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Buildings
{
  public class Mining : Building
  {
    public override Rectangle CollisionRectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height - 32);
      }
    }

    public Mining(ContentManager Content, Vector2 position, GameState parent)
    {
      _texture = Content.Load<Texture2D>("Buildings/Mining");
      Position = position;
      Parent = parent;

      Menu = new Menu(Content);

      Resources = new Models.Resources()
      {
        Food = 25,
        Wood = 50,
        Stone = 0,
        Gold = 0,
      };

      MinionColor = Color.DarkGray;
      TileType = Tiles.TileType.Stone;
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      Menu.Update($"Workers: {CurrentMinions}/{MaxMinions}");
    }
  }
}
