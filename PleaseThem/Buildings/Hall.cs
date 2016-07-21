using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.Controls;
using PleaseThem.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Buildings
{
  public class Hall : Building
  {
    public override Rectangle CollisionRectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y + 32, _texture.Width, 96);
      }
    }

    public Hall(ContentManager Content, Vector2 position, GameState parent)
    {
      _texture = Content.Load<Texture2D>("Buildings/Hall");
      Position = position;
      Parent = parent;

      Menu = new Menu(Content);

      FoodCost = 0;
      WoodCost = 0;
      StoneCost = 0;
      GoldCost = 0;

      MaxMinions = 0;

      Color = Color.White;
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      Menu.Update($"Unemployed Minions: {Parent.UnemploymentCount}",
                   $"Total Minions: {Parent.MinionCount}");
    }
  }
}
