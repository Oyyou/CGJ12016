using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PleaseThem.Actors;
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
    public override string[] Content => new string[] { $"Training: {CurrentMinions}/{MaxMinions}" };

    public override Rectangle CollisionRectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, FrameWidth, 96);
      }
    }

    public SwordSchool(GameState parent, Texture2D texture, int frameCount)
      : base(parent, texture, frameCount)
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

      _buildingPositions = new List<PatrolPoints>();
    }

    public override void Initialise()
    {
      base.Initialise();

      _buildingPositions.Add(
        new PatrolPoints()
        {
          PositionsV2 = new List<PatrolPoints.KILLMENOW>()
          {
            new PatrolPoints.KILLMENOW()
            {
              IsActive = true,
              Value = new Vector2(Position.X + 0, Position.Y + 96)
            },
          },
        });

      _buildingPositions.Add(
        new PatrolPoints()
        {
          PositionsV2 = new List<PatrolPoints.KILLMENOW>()
          {
            new PatrolPoints.KILLMENOW()
            {
              IsActive = true,
              Value = new Vector2(Position.X + 32, Position.Y + 128),
            },
          },
        });

      _buildingPositions.Add(
        new PatrolPoints()
        {
          PositionsV2 = new List<PatrolPoints.KILLMENOW>()
          {
            new PatrolPoints.KILLMENOW()
            {
              IsActive = true,
              Value = new Vector2(Position.X + 0, Position.Y + 160),
            },
          },
        });

      _buildingPositions.Add(
        new PatrolPoints()
        {
          PositionsV2 = new List<PatrolPoints.KILLMENOW>()
          {
            new PatrolPoints.KILLMENOW()
            {
              IsActive = true,
              Value = new Vector2(Position.X + 96, Position.Y + 96),
            },
          },
        });

      _buildingPositions.Add(
        new PatrolPoints()
        {
          PositionsV2 = new List<PatrolPoints.KILLMENOW>()
          {
            new PatrolPoints.KILLMENOW()
            {
              IsActive = true,
              Value = new Vector2(Position.X + 128, Position.Y + 128),
            },
          },
        });

      _buildingPositions.Add(
        new PatrolPoints()
        {
          PositionsV2 = new List<PatrolPoints.KILLMENOW>()
          {
            new PatrolPoints.KILLMENOW()
            {
              IsActive = true,
              Value = new Vector2(Position.X + 96, Position.Y + 160),
            },
          },
        });
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
    }

    public override void Work(object sender, EventArgs e)
    {
      var minion = sender as Minion;

      var bPositions = _buildingPositions.Where(c => c.Minion == minion).FirstOrDefault();

      if (bPositions == null)
      {
        bPositions = _buildingPositions.Where(c => c.Minion == null).FirstOrDefault();
        bPositions.Minion = minion;
      }

      bPositions.MoveMinion(minion);
    }
  }
}
