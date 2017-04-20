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
  public class House : Building
  {
    public override Rectangle CollisionRectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height - 32);
      }
    }

    public House(GameState parent, Texture2D texture)
      : base(parent, texture)
    {
      Parent = parent;

      Menu = new Menu(Parent.Content);

      Resources = new Models.Resources()
      {
        Food = 10,
        Wood = 25,
        Stone = 0,
        Gold = 0,
      };

      MaxMinions = 2;

      MinionColor = Color.Brown;
      TileType = Tiles.TileType.Tree;
    }

    public override void Update(GameTime gameTime)
    {
      Menu.Update($"Minions: {CurrentMinions}/{MaxMinions}");
    }
  }
}
