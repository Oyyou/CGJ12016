using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PleaseThem.Core
{
  public struct AnimationPlayer
  {
    private Animation _animation;
    private int _frameIndex;
    private float _timer;
    private Vector2 _position;
    private Rectangle _rectangle;

    public bool Stop { get; set; }

    public Animation Animation
    {
      get { return _animation; }
    }

    public int FrameIndex
    {
      get { return _frameIndex; }
      set { _frameIndex = value; }
    }

    public int FrameWidth
    {
      get { return _animation.FrameWidth; }
    }

    public int FrameHeight
    {
      get { return _animation.FrameHeight; }
    }

    public Vector2 Origin
    {
      get { return new Vector2(0, 0); }
    }

    public float Rotation { get; set; }
    public int LoopCount { get; set; }

    public SpriteEffects Direction;
    public Color Color;

    public void PlayAnimation(Animation animation)
    {
      Stop = false;
      LoopCount = 0;

      if (this._animation == animation)
        return;

      this._animation = animation;
      _frameIndex = 0;
      _timer = 0;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position)
    {
      _position = position;

      if (_animation == null)
        return;
        //throw new NotSupportedException("Nope");

      _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

      while (_timer >= _animation.FrameTime)
      {
        _timer -= _animation.FrameTime;

        if (_animation.IsLooping)
          _frameIndex = (_frameIndex + 1) % _animation.FrameCount;
        else _frameIndex = Math.Min(_frameIndex + 1, _animation.FrameCount - 1);

        if (_frameIndex == 0)
          LoopCount++;
      }

      if (Stop)
        _frameIndex = 0;

      _rectangle = new Rectangle(_frameIndex * _animation.FrameWidth, 0, _animation.FrameWidth, _animation.FrameHeight);

      spriteBatch.Draw(_animation.Texture, _position, _rectangle, Color, Rotation, Origin, 1, Direction, 0.9f);
    }
  }
}
