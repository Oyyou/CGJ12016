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
  public class BuildingPosition
  {
    public bool HasWorker { get; set; }

    public Minion Minion { get; set; }

    public List<Vector2> Positions { get; set; }

    public class KILLMENOW
    {
      public bool IsActive { get; set; }

      public Vector2 Value { get; set; }
    }

    public List<KILLMENOW> PositionsV2 { get; set; }
  }

  public class Farm : Building
  {
    public List<BuildingPosition> FarmPositions { get; set; }

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

      FarmPositions = new List<BuildingPosition>();
    }

    public override void Initialise()
    {
      base.Initialise();

      FarmPositions.Add(new BuildingPosition()
      {
        HasWorker = false,
        Positions = new List<Vector2>()
        {
          new Vector2(Position.X + 32, Rectangle.Top + 96),
          new Vector2(Position.X + 32, Rectangle.Bottom - 32)
        },
        PositionsV2 = new List<BuildingPosition.KILLMENOW>()
        {
          new Buildings.BuildingPosition.KILLMENOW()
          {
            Value = new Vector2(Position.X + 32, Rectangle.Top + 96),
            IsActive= true,
          },
          new Buildings.BuildingPosition.KILLMENOW()
          {
            Value = new Vector2(Position.X + 32, Rectangle.Bottom - 32),
            IsActive= false,
          }
        }
      });

      FarmPositions.Add(new BuildingPosition()
      {
        HasWorker = false,
        Positions = new List<Vector2>()
        {
          new Vector2(Position.X + 96, Rectangle.Top),
          new Vector2(Position.X + 96, Rectangle.Bottom - 32)
        },
        PositionsV2 = new List<BuildingPosition.KILLMENOW>()
        {
          new Buildings.BuildingPosition.KILLMENOW()
          {
            Value = new Vector2(Position.X + 96, Rectangle.Top),
            IsActive= true,
          },
          new Buildings.BuildingPosition.KILLMENOW()
          {
            Value = new Vector2(Position.X + 96, Rectangle.Bottom - 32),
            IsActive= false,
          }
        }
      });

      FarmPositions.Add(new BuildingPosition()
      {
        HasWorker = false,
        Positions = new List<Vector2>()
        {
          new Vector2(Position.X + 160, Rectangle.Top),
          new Vector2(Position.X + 160, Rectangle.Bottom - 32)
        },
        PositionsV2 = new List<BuildingPosition.KILLMENOW>()
        {
          new Buildings.BuildingPosition.KILLMENOW()
          {
            Value = new Vector2(Position.X + 160, Rectangle.Top),
            IsActive= true,
          },
          new Buildings.BuildingPosition.KILLMENOW()
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

      var farmPosition = FarmPositions.Where(c => c.Minion == minion).FirstOrDefault();

      if (farmPosition == null && !FarmPositions.All(c => c.Minion != null && c.Minion.Equals(minion)))
      {
        farmPosition = FarmPositions.Where(c => c.Minion == null).FirstOrDefault();
        farmPosition.Minion = minion;
      }

      for (int i = 0; i < farmPosition.PositionsV2.Count; i++)
      {
        var currentPosition = farmPosition.PositionsV2[i];

        if (currentPosition.IsActive)
        {
          minion.Move(currentPosition.Value);
        }

        if (minion.Position == currentPosition.Value)
        {
          var index = i == farmPosition.PositionsV2.Count - 1 ? 0 : i + 1;

          currentPosition.IsActive = false;
          farmPosition.PositionsV2[index].IsActive = true;
        }
      }

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
