﻿using Microsoft.Xna.Framework;
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
using PleaseThem.Actors;

namespace PleaseThem.Buildings
{
  public class House : Building
  {
    public override Rectangle CollisionRectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, FrameWidth, _texture.Height - 32);
      }
    }

    public override string[] Content => new string[0];

    public override void Employ(Minion minion)
    {
      
    }

    public House(GameState parent, Texture2D texture, int frameCount)
      : base(parent, texture, frameCount)
    {
      Resources = new Models.Resources()
      {
        Food = 10,
        Wood = 25,
        Stone = 0,
        Gold = 0,
      };

      MaxMinions = 2;

      MinionColor = Color.Brown;
      TileType = Tiles.TileType.Occupied;
    }

    public override void Unemploy()
    {
      
    }
  }
}
