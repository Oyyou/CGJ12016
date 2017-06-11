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
  public class SwordSchool : Building
  {
    public List<BuildingPosition> BuildingPositions { get; set; }

    public override string[] Content => new string[] { $"Training: {CurrentMinions}/{MaxMinions}" };

    public override Rectangle CollisionRectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, 160, 96);
      }
    }

    public SwordSchool(GameState parent, Texture2D texture)
      : base(parent, texture)
    {
      Resources = new Models.Resources()
      {
        Food = 80,
        Wood = 150,
        Stone = 40,
        Gold = 0,
      };

      MinionColor = Color.Red;
      TileType = Tiles.TileType.Militia;
      MaxMinions = 6;

      BuildingPositions = new List<BuildingPosition>();
    }

    public override void Initialise()
    {
      base.Initialise();

      BuildingPositions.Add(
        new BuildingPosition()
        {
          HasWorker = false,
          Positions = new List<Vector2>()
          {
            new Vector2(Position.X + 0, Position.Y + 96),
          }
        });

      BuildingPositions.Add(
        new BuildingPosition()
        {
          HasWorker = false,
          Positions = new List<Vector2>()
          {
            new Vector2(Position.X + 32, Position.Y + 128),
          }
        });

      BuildingPositions.Add(
        new BuildingPosition()
        {
          HasWorker = false,
          Positions = new List<Vector2>()
          {
            new Vector2(Position.X + 0, Position.Y + 160),
          }
        });

      BuildingPositions.Add(
        new BuildingPosition()
        {
          HasWorker = false,
          Positions = new List<Vector2>()
          {
            new Vector2(Position.X + 96, Position.Y + 96),
          }
        });

      BuildingPositions.Add(
        new BuildingPosition()
        {
          HasWorker = false,
          Positions = new List<Vector2>()
          {
            new Vector2(Position.X + 128, Position.Y + 128),
          }
        });

      BuildingPositions.Add(
        new BuildingPosition()
        {
          HasWorker = false,
          Positions = new List<Vector2>()
          {
            new Vector2(Position.X + 96, Position.Y + 160),
          }
        });
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
    }
  }
}
