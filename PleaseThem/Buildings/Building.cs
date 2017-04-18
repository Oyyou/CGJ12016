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
  public class Building
  {
    protected Texture2D _texture;
    public Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
      }
    }

    public virtual Rectangle CollisionRectangle
    {
      get
      {
        throw new Exception("Implement 'CollisionRectangle'");
      }
    }

    public Vector2 Position;
    protected GameState Parent;

    protected MouseState currentMouse;
    protected MouseState previousMouse;

    //private float _hoverTimer; // How long the mouse has been hovering over the building;
    //private bool _hovering; // Have we been hovering for long enough?
    public bool LeftClicked { get; private set; } // Whether or not the building has been clicked
    public bool RightClicked { get; private set; } // Whether or not the building has been clicked
    public bool CanHaveWorkers
    {
      get
      {
        return CurrentMinions < MaxMinions;
      }
    }

    protected Menu Menu;

    public Color MinionColor { get; protected set; }

    public Color Color = Color.White;

    public int CurrentMinions = 0;
    public int MaxMinions = 5;

    public Resources Resources { get; protected set; }

    public TileType TileType { get; protected set; }

    public virtual void Initialise()
    {

    }

    public virtual void Update(GameTime gameTime)
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

    public void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Position, null, Color, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.9f);

      Menu.Draw(spriteBatch, this.Rectangle);
    }
  }
}
