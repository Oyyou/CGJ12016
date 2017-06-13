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
  public class Farm : Building
  {
    public override string[] Content => new string[] { $"Workers: {CurrentMinions}/{MaxMinions}" };

    public override Rectangle CollisionRectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, 64, 96);
      }
    }

    public Farm(GameState parent, Texture2D texture)
      : base(parent, texture)
    {
      Resources = new Models.Resources()
      {
        Food = 25,
        Wood = 50,
        Stone = 0,
        Gold = 0,
      };

      MinionColor = Color.Olive;
      TileType = Tiles.TileType.Farm;
      MaxMinions = 3;

      _buildingPositions = new List<PatrolPoints>();
    }

    public override void Initialise()
    {
      base.Initialise();

      _buildingPositions.Add(new PatrolPoints()
      {
        HasWorker = false,
        PositionsV2 = new List<PatrolPoints.KILLMENOW>()
        {
          new Buildings.PatrolPoints.KILLMENOW()
          {
            Value = new Vector2(Position.X + 32, Rectangle.Top + 96),
            IsActive= true,
          },
          new Buildings.PatrolPoints.KILLMENOW()
          {
            Value = new Vector2(Position.X + 32, Rectangle.Bottom - 32),
            IsActive= false,
          }
        }
      });

      _buildingPositions.Add(new PatrolPoints()
      {
        HasWorker = false,
        PositionsV2 = new List<PatrolPoints.KILLMENOW>()
        {
          new Buildings.PatrolPoints.KILLMENOW()
          {
            Value = new Vector2(Position.X + 96, Rectangle.Top),
            IsActive= true,
          },
          new Buildings.PatrolPoints.KILLMENOW()
          {
            Value = new Vector2(Position.X + 96, Rectangle.Bottom - 32),
            IsActive= false,
          }
        }
      });

      _buildingPositions.Add(new PatrolPoints()
      {
        HasWorker = false,
        PositionsV2 = new List<PatrolPoints.KILLMENOW>()
        {
          new Buildings.PatrolPoints.KILLMENOW()
          {
            Value = new Vector2(Position.X + 160, Rectangle.Top),
            IsActive= true,
          },
          new Buildings.PatrolPoints.KILLMENOW()
          {
            Value = new Vector2(Position.X + 160, Rectangle.Bottom - 32),
            IsActive= false,
          }
        }
      });
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
    }

    public override void Work(object sender, EventArgs e)
    {
      var minion = sender as Minion;

      var farmPosition = _buildingPositions.Where(c => c.Minion == minion).FirstOrDefault();

      if (farmPosition == null)
      {
        farmPosition = _buildingPositions.Where(c => c.Minion == null).FirstOrDefault();
        farmPosition.Minion = minion;
      }

      farmPosition.MoveMinion(minion);

      //_resourceCollectionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

      //if (_resourceCollectionTimer > 2.5f)
      //{
      //  _resourceCollectionTimer = 0.0f;

      //  _resources.Food++;

      //  _parent.ResourceManager.Add(_resources);
      //}
    }
  }
}
