using Microsoft.Xna.Framework;
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

    private int _height;

    private Texture2D _texture;
    private Rectangle _rectangle;

    private Button _lumber;
    private Button _mining;
    private Button _farm;
    private Button _house;
    private List<Button> _buttons = new List<Button>();

    public Building SelectedBuilding;

    public BuildingList(GraphicsDevice graphiceDevice, ContentManager Content, GameState parent)
    {
      this.Content = Content;
      _parent = parent;

      Texture2D buttonTexture = Content.Load<Texture2D>("Controls/Button");

      _height = buttonTexture.Height + 20;

      _texture = new Texture2D(graphiceDevice, 1, 1); // Content.Load<Texture2D>("Controls/BuildingList");
      _texture.SetData(new Color[] { new Color(255, 255, 255), });

      _rectangle = new Rectangle(0, Game1.ScreenHeight - _height, Game1.ScreenWidth, _height);
      SpriteFont font = Content.Load<SpriteFont>("Fonts/Arial08pt");

      _buttons.Add(_lumber = new Button(buttonTexture, font, new Vector2(10, 470 - buttonTexture.Height), "Lumber"));
      _buttons.Add(_mining = new Button(buttonTexture, font, new Vector2(_lumber.Rectangle.Right + 10, 470 - buttonTexture.Height), "Mining"));
      _buttons.Add(_farm = new Button(buttonTexture, font, new Vector2(_mining.Rectangle.Right + 10, 470 - buttonTexture.Height), "Farm"));
      _buttons.Add(_house = new Button(buttonTexture, font, new Vector2(_farm.Rectangle.Right + 10, 470 - buttonTexture.Height), "House"));

      //_buttons.Add(_lumber = new Button(parent, buttonTexture, font) { Position = new Vector2(10, 470 - buttonTexture.Height), Text = "Lumber" });
      //_buttons.Add(_mining = new Button(parent, buttonTexture, font) { Position = new Vector2(_lumber.Rectangle.Right + 10, 470 - buttonTexture.Height), Text = "Mining" });
      //_buttons.Add(_farm = new Button(parent, buttonTexture, font) { Position = new Vector2(_mining.Rectangle.Right + 10, 470 - buttonTexture.Height), Text = "Farm" });
      //_buttons.Add(_house = new Button(parent, buttonTexture, font) { Position = new Vector2(_farm.Rectangle.Right + 10, 470 - buttonTexture.Height), Text = "House" });
    }

    public override void Update(GameTime gameTime)
    {
      _rectangle = new Rectangle(0, Game1.ScreenHeight - _height, Game1.ScreenWidth, _height);

      foreach (var button in _buttons)
      {
        button.Position = new Vector2(button.Position.X, Game1.ScreenHeight - 10 - button.Rectangle.Height);
        button.Update();
      }

      if (Keyboard.GetState().IsKeyDown(Keys.Escape) || SelectedBuilding == null)
      {
        if (SelectedBuilding != null)
          SelectedBuilding.IsRemoved = true;

        SelectedBuilding = null;

        foreach (var b in _buttons)
          b.Selected = false;
      }

      int x = (int)Math.Floor(_parent.MouseRectangle.X / 32m) * Map.TileSize;
      int y = (int)Math.Floor(_parent.MouseRectangle.Y / 32m) * Map.TileSize;

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
        SelectedBuilding.Color = Color.Green;

        // I don't use the 'MouseRectangle' because.. Well.. It doesn't work like that, and I'm too rushed to make purty.
        if (Mouse.GetState().Y >= 32 && Mouse.GetState().Y < (Game1.ScreenHeight - _texture.Height) &&
            Mouse.GetState().X >= 0 && Mouse.GetState().X < Game1.ScreenWidth)
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
          SelectedBuilding.IsRemoved = true;
          SelectedBuilding = null;
          button.Selected = false;
        }
        else
        {
          foreach (var b in _buttons)
            b.Selected = false;

          if (SelectedBuilding != null)
            SelectedBuilding.IsRemoved = true;

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
