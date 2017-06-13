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
  public class Building : Models.Sprite
  {
    #region Fields

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

    public TileType TileType { get; protected set; }

    #endregion

    #region Methods

    public Building(GameState parent, Texture2D texture)
      : base(parent, texture)
    {
      Minions = new List<Actors.Minion>();

      DefaultLayer = 0.7f;

      Layer = DefaultLayer;
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
      
      minion.Workk += Work;

      Minions.Add(minion);
    }

    public virtual void Initialise()
    {

    }

    public void Unemploy()
    {
      if (CurrentMinions == 0)
        return;

      var minion = Minions.Last();

      minion.Colour = Color.White;
      minion.Workplace = null;
      
      minion.Workk -= Work;

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

            if (this is Farm)
              ((Farm)this).FarmPositions.Where(c => c.HasWorker).Last().HasWorker = false;

            if (this is SwordSchool)
              ((SwordSchool)this).BuildingPositions.Where(c => c.HasWorker).Last().HasWorker = false;
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
