using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.Controllers;
using PleaseThem.Core;
using PleaseThem.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Models
{
  public class Sprite : Component
  {
    protected readonly AnimationController _animationController;

    protected AnimationPlayer _animationPlayer;

    protected readonly GameState _parent;

    protected readonly Texture2D _texture;

    public virtual Rectangle CollisionRectangle
    {
      get
      {
        var width = _texture != null ? _texture.Width : _animationPlayer.FrameWidth;
        var height = _texture != null ? _texture.Height : _animationPlayer.FrameHeight;

        return new Rectangle((int)Position.X, (int)Position.Y, width, height);
      }
    }

    public Color Colour { get; set; }

    public int Height
    {
      get
      {
        return _texture.Height;
      }
    }

    public bool IsVisible { get; protected set; }

    public float Layer { get; set; }

    public Vector2 Position { get; set; }
    
    public Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
      }
    }

    public float Rotation { get; protected set; }

    public int Width
    {
      get
      {
        return _texture.Width;
      }
    }

    public Sprite(GameState parent, Texture2D texture)
    {
      _parent = parent;
      _texture = texture;

      Initialize();
    }

    public Sprite(GameState parent, AnimationController animationController)
    {
      _parent = parent;
      _animationController = animationController;

      // Set the default animation to walking down, but stop it from animating
      _animationPlayer.PlayAnimation(_animationController.WalkDown);
      _animationPlayer.Stop = true;

      Initialize();
    }

    private void Initialize()
    {
      IsVisible = true;
    }

    public override void Update(GameTime gameTime)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (!IsVisible)
        return;

      if (_texture != null)
      {
        spriteBatch.Draw(_texture, Position, null, Colour, Rotation, new Vector2(0, 0), 1, SpriteEffects.None, Layer);
      }
      else if (_animationController != null)
      {
        _animationPlayer.Draw(gameTime, spriteBatch, Position);
      }
    }
  }
}
