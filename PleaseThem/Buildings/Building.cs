using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PleaseThem.Controls;
using PleaseThem.Models;
using PleaseThem.States;
using PleaseThem.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Buildings
{
  public class Building : Sprite
  {
    protected GameState Parent;

    protected MouseState currentMouse;
    protected MouseState previousMouse;

    public bool LeftClicked { get; private set; } // Whether or not the building has been clicked

    public bool RightClicked { get; private set; } // Whether or not the building has been clicked

    public bool CanHaveWorkers
    {
      get
      {
        return CurrentMinions < MaxMinions;
      }
    }

    public List<Actors.Minion> Minions { get; private set; }

    protected Menu Menu;

    public Color MinionColor { get; protected set; }

    public Color Color = Color.White;

    public int CurrentMinions { get { return Minions.Count; } }

    public int MaxMinions = 5;

    public Building(GameState parent, Texture2D texture)
      : base(parent, texture)
    {
      Minions = new List<Actors.Minion>();
    }

    public Resources Resources { get; protected set; }

    public TileType TileType { get; protected set; }

    public virtual void Initialise()
    {

    }

    public void Employ(Actors.Minion minion)
    {
      minion.Employment = this;
      minion.Colour = this.MinionColor;
      minion.IsVisible = true;

      Minions.Add(minion);
    }

    public void Unemploy()
    {
      if (CurrentMinions == 0)
        return;

      var minion = Minions.Last();

      minion.Colour = Color.White;
      minion.Employment = null;

      Minions.Remove(minion);
    }

    public override void Update(GameTime gameTime)
    {
      if (this.MinionColor == new Color(0, 0, 0, 0))
        throw new Exception("Please set 'Color' on this building: " + this.GetType().ToString());

      previousMouse = currentMouse;
      currentMouse = Mouse.GetState();
      LeftClicked = false;
      RightClicked = false;

      if (Parent.MouseRectangle.Intersects(Rectangle) && !LeftClicked)
      {
        if (currentMouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released)
        {
          LeftClicked = true;
        }

        if (currentMouse.RightButton == ButtonState.Pressed && previousMouse.RightButton == ButtonState.Released)
        {
          RightClicked = true;
        }

        Menu.IsVisible = true;
      }
      else
      {
        Menu.IsVisible = false;
      }

      Menu.Update($"Workers: {CurrentMinions}/{MaxMinions}");
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Position, null, Color, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.8f);

      Menu.Draw(spriteBatch, this.Rectangle);
    }
  }
}
