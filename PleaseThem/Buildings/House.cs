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

    public House(ContentManager Content, Vector2 position, GameState parent)
    {
      _texture = Content.Load<Texture2D>("Buildings/House");
      Position = position;
      Parent = parent;

      Menu = new Menu(Content);

      FoodCost = 10;
      WoodCost = 25;
      StoneCost = 0;
      GoldCost = 0;

      MaxMinions = 2;

      Color = Color.Brown;
      TileType = Tiles.TileType.Tree;
    }

    public override void Update(GameTime gameTime)
    {
      Menu.Update($"Minions: {CurrentMinions}/{MaxMinions}");
    }
  }
}
