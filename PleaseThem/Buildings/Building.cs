using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PleaseThem.Actors;
using PleaseThem.Controls;
using PleaseThem.States;
using PleaseThem.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Buildings
{
  public class PatrolPoints
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

    public void MoveMinion(Minion minion)
    {
      for (int i = 0; i < PositionsV2.Count; i++)
      {
        var currentPosition = PositionsV2[i];

        if (currentPosition.IsActive)
        {
          minion.Move(currentPosition.Value);
        }

        if (minion.Position == currentPosition.Value)
        {
          var index = i == PositionsV2.Count - 1 ? 0 : i + 1;

          currentPosition.IsActive = false;
          PositionsV2[index].IsActive = true;
        }
      }
    }
  }

  public class TargetPoints
  {
    /// <summary>
    /// The building that found the point
    /// </summary>
    public Building Building { get; set; }

    public bool InUse { get; set; }

    public Vector2 Position { get; set; }

    public TileType TileType { get; set; }
  }

  public class Building : Models.Sprite
  {
    #region Fields

    protected List<PatrolPoints> _buildingPositions { get; set; }

    #endregion

    #region Properties

    public bool CanHaveWorkers
    {
      get
      {
        return CurrentMinions < MaxMinions;
      }
    }

    public Color Color { get; set; } = Color.White;

    public virtual string[] Content
    {
      get
      {
        return new string[] { $"Workers: {CurrentMinions}/{MaxMinions}" };
      }
    }

    public int CurrentMinions { get { return Minions.Count; } }

    public virtual Vector2 DoorPosition
    {
      get
      {
        var width = (int)Math.Floor((this.Rectangle.Width / Map.TileSize) / 2m);

        return new Vector2(Position.X + (width * Map.TileSize), Rectangle.Bottom - 32);
      }
    }

    public bool LeftClicked { get; private set; }

    public int MaxMinions = 5;

    public Color MinionColor { get; protected set; }

    public List<Actors.Minion> Minions { get; private set; }

    public Models.Resources Resources { get; protected set; }

    public bool RightClicked { get; private set; }

    /// <summary>
    /// A list of points our minions can travel to for resources
    /// </summary>
    public static List<TargetPoints> TargetPoints = new List<Buildings.TargetPoints>();

    public TileType TileType { get; protected set; }

    #endregion

    #region Methods

    public Building(GameState parent, Texture2D texture)
      : base(parent, texture)
    {
      Minions = new List<Actors.Minion>();

      DefaultLayer = 0.7f;

      Layer = DefaultLayer;

      _buildingPositions = new List<PatrolPoints>();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Position, null, Color, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);
    }

    public void Employ(Actors.Minion minion)
    {
      minion.Workplace = this;
      minion.Colour = this.MinionColor;
      minion.IsVisible = true;

      minion.WorkEvent += Work;

      Minions.Add(minion);
    }

    public virtual void Initialise()
    {

    }

    public void SetTargetPoints()
    {
      var targetPoints = new List<TargetPoints>();

      var maxTargetCount = 4;

      int attempts = 0;

      while (targetPoints.Count < maxTargetCount && attempts < 100)
      {
        var resourceTile = _parent.Map.ResourceTiles
            .Where(c => c.TileType == TileType)
            .OrderBy(c => Vector2.Distance(DoorPosition, c.Position)).ToArray()[attempts];

        var positions = new List<Vector2>()
        {
          resourceTile.Position - new Vector2(32, 0),
          resourceTile.Position + new Vector2(32, 0),
          resourceTile.Position - new Vector2(0, 32),
          resourceTile.Position + new Vector2(0, 32),
        };

        foreach (var position in positions)
        {
          if (targetPoints.Count == maxTargetCount)
            break;

          if (TargetPoints.Any(c => c.Position == position))
            continue;

          var path = _parent.Pathfinder.FindPath(DoorPosition, position);

          if (path.Count > 0)
          {
            targetPoints.Add(
              new TargetPoints()
              {
                Building = this,
                InUse = false,
                Position = position,
                TileType = this.TileType,
              });
          }
        }
        attempts++;
      }

      TargetPoints.AddRange(targetPoints);
    }

    public void Unemploy()
    {
      if (CurrentMinions == 0)
        return;

      var minion = Minions.Last();

      minion.Colour = Color.White;
      minion.Workplace = null;

      minion.WorkEvent -= Work;

      var buildingPosition = _buildingPositions.Where(c => c.Minion == minion).FirstOrDefault();

      if (buildingPosition != null)
        buildingPosition.Minion = null;

      Minions.Remove(minion);
    }

    public override void Update(GameTime gameTime)
    {
      if (MinionColor == new Color(0, 0, 0, 0))
        throw new Exception("Please set 'Color' on this building: " + this.GetType().ToString());

      LeftClicked = false;
      RightClicked = false;

      if (_parent.MouseRectangleWithCamera.Intersects(Rectangle))
      {
        if (_parent.CurrentMouse.LeftButton == ButtonState.Pressed && _parent.PreviousMouse.LeftButton == ButtonState.Released)
          LeftClicked = true;

        if (_parent.CurrentMouse.RightButton == ButtonState.Pressed && _parent.PreviousMouse.RightButton == ButtonState.Released)
          RightClicked = true;
      }

      if (_parent.MouseRectangle.Y >= 32 && _parent.MouseRectangle.Y < (Game1.ScreenHeight - 64))
      {
        if (LeftClicked)
        {
          if (CanHaveWorkers)
          {
            if (_parent.UnemploymentCount > 0)
            {
              var minion = _parent.Components.Where(c => c is Minion).Where(c => ((Minion)c).Workplace == null).FirstOrDefault() as Minion;
              Employ(minion);
            }
            else
            {
              Game1.MessageBox.Show("There are no unemployed minions");
            }
          }
          else
          {
            Game1.MessageBox.Show("Building can't employ");
          }
        }

        if (RightClicked)
        {
          if (CurrentMinions > 0)
          {
            Unemploy();
          }
        }
      }
    }

    public virtual void Work(object sender, EventArgs e)
    {
      throw new NotImplementedException("Need to implement 'Work' for building.");
    }

    #endregion
  }
}
