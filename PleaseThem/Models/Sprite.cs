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
  public class Sprite
  {
    protected readonly AnimationController _animationController;

    protected AnimationPlayer _animationPlayer;

    protected readonly GameState _parent;

    protected readonly Texture2D _texture;

    public Vector2 Position { get; protected set; }

    public float Rotation { get; protected set; }

    public float Layer { get; set; }

    public Color Colour { get; set; }

    public Sprite(GameState parent, Texture2D texture)
    {
      _parent = parent;
      _texture = texture;
    }

    public Sprite(GameState parent, AnimationController animationController)
    {
      _parent = parent;
      _animationController = animationController;

      // Set the default animation to walking down, but stop it from animating
      _animationPlayer.PlayAnimation(_animationController.WalkDown);
      _animationPlayer.Stop = true;
    }

    public virtual void Update(GameTime gameTime)
    {

    }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
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
