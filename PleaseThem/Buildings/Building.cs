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
    #region Fields
    
    protected MouseState _currentMouse;

    protected Menu _menu { get; private set; }
    
    protected MouseState _previousMouse;
    
    #endregion
    
    #region Properties

    public bool CanHaveWorkers
    {
      get
      {
        return CurrentMinions < MaxMinions;
      }
    }

    public int CurrentMinions { get { return Minions.Count; } }

    public bool LeftClicked { get; private set; }

    public int MaxMinions = 5;

    public Color MinionColor { get; protected set; } = Color.White;

    public List<Actors.Minion> Minions { get; private set; }

    public Resources Resources { get; protected set; }

    public bool RightClicked { get; private set; }

    public TileType TileType { get; protected set; }
    
    #endregion
    
    #region Methods

    public Building(GameState parent, Texture2D texture)
      : base(parent, texture)
    {
      Minions = new List<Actors.Minion>();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.8f);

      _menu.Draw(spriteBatch, Rectangle);
    }

    public void Employ(Actors.Minion minion)
    {
      minion.Employment = this;
      minion.Colour = this.MinionColor;
      minion.IsVisible = true;

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
      minion.Employment = null;

      Minions.Remove(minion);
    }

    public override void Update(GameTime gameTime)
    {
      if (MinionColor == Color.White)
        throw new Exception("Please set 'Color' on this building: " + this.GetType().ToString());

      _previousMouse = _currentMouse;
      _currentMouse = Mouse.GetState();
      LeftClicked = false;
      RightClicked = false;

      if (_parent.MouseRectangle.Intersects(Rectangle))
      {
        if (_currentMouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
        {
          LeftClicked = true;
        }

        if (_currentMouse.RightButton == ButtonState.Pressed && _previousMouse.RightButton == ButtonState.Released)
        {
          RightClicked = true;
        }

        _menu.IsVisible = true;
      }
      else
      {
        _menu.IsVisible = false;
      }

      _menu.Update($"Workers: {CurrentMinions}/{MaxMinions}");
    }
    
    #endregion
  }
}
