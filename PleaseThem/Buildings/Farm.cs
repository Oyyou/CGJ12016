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
      });

      FarmPositions.Add(new BuildingPosition()
      {
        HasWorker = false,
        Positions = new List<Vector2>()
        {
          new Vector2(Position.X + 96, Rectangle.Top),
          new Vector2(Position.X + 96, Rectangle.Bottom - 32)
        },
      });

      FarmPositions.Add(new BuildingPosition()
      {
        HasWorker = false,
        Positions = new List<Vector2>()
        {
          new Vector2(Position.X + 160, Rectangle.Top),
          new Vector2(Position.X + 160, Rectangle.Bottom - 32)
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
      
      BuildingPosition farmPosition = null;
      
      if (!FarmPositions.All(c => c.Minion != null && c.Minion.Equals(minion)))
      {
        farmPosition = FarmPositions.Where(c => c.Minion == null).FirstOrDefault();
        farmPosition.Minion = minion;
      }
      else
      {
        farmPosition = FarmPositions.Where(c => c.Minion == minion).FirstOrDefault();
      }
      
      var farmPosition1 = farmPosition.Positions[0];
      var farmPosition2 = farmPosition.Positions[1];
      
      if(minion.Velocity == Vector2.Zero &&
         minion.Position != farmPosition1 &&
         minion.Position != farmPosition2)
      {
        minion.Move(farmPosition1);
        
        return;
      }

      var speed = 2f;
      
      if(minion.Position == farmPosition1)
      {
        if(farmPosition2.Y > farmPosition1.Y)
          minion.Velocity = new Vector2(0, speed);
        else
          minion.Velocity = new Vector2(0, -speed);
      }
      else if(minion.Position == farmPosition1)
      {
        if(farmPosition1.Y > farmPosition2.Y)
          minion.Velocity = new Vector2(0, speed);
        else
          minion.Velocity = new Vector2(0, -speed);
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
