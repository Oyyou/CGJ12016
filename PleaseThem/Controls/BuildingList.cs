﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PleaseThem.Buildings;
using PleaseThem.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Controls
{
  public class BuildingList : Component
  {
    private ContentManager Content;
    private GameState _parent;

    private Texture2D _texture;
    private Rectangle _rectangle;

    private Button _lumber;
    private Button _mining;
    private Button _farm;
    private Button _house;
    private List<Button> _buttons = new List<Button>();

    public Building SelectedBuilding;

    public BuildingList(ContentManager Content, GameState parent)
    {
      this.Content = Content;
      _parent = parent;

      _texture = Content.Load<Texture2D>("Controls/BuildingList");
      _rectangle = new Rectangle(0, 480 - _texture.Height, _texture.Width, _texture.Height);

      Texture2D buttonTexture = Content.Load<Texture2D>("Controls/Button");
      SpriteFont font = Content.Load<SpriteFont>("Fonts/Arial08pt");

      _buttons.Add(_lumber = new Button(buttonTexture, font, new Vector2(10, 470 - buttonTexture.Height), "Lumber"));
      _buttons.Add(_mining = new Button(buttonTexture, font, new Vector2(_lumber.Rectangle.Right + 10, 470 - buttonTexture.Height), "Mining"));
      _buttons.Add(_farm = new Button(buttonTexture, font, new Vector2(_mining.Rectangle.Right + 10, 470 - buttonTexture.Height), "Farm"));
      _buttons.Add(_house = new Button(buttonTexture, font, new Vector2(_farm.Rectangle.Right + 10, 470 - buttonTexture.Height), "House"));
    }

    public override void Update(GameTime gameTime)
    {
      foreach (var button in _buttons)
        button.Update();

      if (Keyboard.GetState().IsKeyDown(Keys.Escape) || SelectedBuilding == null)
      {
        if (SelectedBuilding != null)
          SelectedBuilding.IsRemoved = true;

        SelectedBuilding = null;

        foreach (var b in _buttons)
          b.Selected = false;
      }

      int x = (int)Math.Floor(_parent.MouseRectangle.X / 32m) * 32;
      int y = (int)Math.Floor(_parent.MouseRectangle.Y / 32m) * 32;

      var position = new Vector2(x, y);

      if (_lumber.Clicked)
      {
        ButtonClick(_lumber, new Lumber(_parent, Content.Load<Texture2D>("Buildings/Lumber")));
      }
      else if (_mining.Clicked)
      {
        ButtonClick(_mining, new Mining(_parent, Content.Load<Texture2D>("Buildings/Mining")));
      }
      else if (_farm.Clicked)
      {
        ButtonClick(_farm, new Farm(_parent, Content.Load<Texture2D>("Buildings/Farm")));
      }
      else if (_house.Clicked)
      {
        ButtonClick(_house, new House(_parent, Content.Load<Texture2D>("Buildings/House")));
      }

      if (SelectedBuilding != null)
      {
        // I don't use the 'MouseRectangle' because.. Well.. It doesn't work like that, and I'm too rushed to make purty.
        if (Mouse.GetState().Y >= 32 && Mouse.GetState().Y < (480 - 32) - SelectedBuilding.Rectangle.Height &&
            Mouse.GetState().X >= 0 && Mouse.GetState().X < 832 - SelectedBuilding.Rectangle.Width)
        {
          SelectedBuilding.Position = position;
        }
      }
    }

    private void ButtonClick(Button button, Building building)
    {
      if (_parent.ResourceManager.CanAfford(building.Resources))
      {
        if (SelectedBuilding != null && button.Selected)
        {
          SelectedBuilding = null;
          button.Selected = false;
        }
        else
        {
          foreach (var b in _buttons)
            b.Selected = false;

          SelectedBuilding = building;
          button.Selected = true;
        }
      }
      else
      {
        Game1.MessageBox.Show("Not enough resources");
      }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, _rectangle, Color.White);

      foreach (var button in _buttons)
        button.Draw(spriteBatch);
    }
  }
}
