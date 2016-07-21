using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PleaseThem.Core
{
  public class Animation
  {
    private Texture2D _texture;
    private int _frameCount;
    private float _frameTime;
    private bool _isLooping;

    public Texture2D Texture
    {
      get { return _texture; }
    }

    public int FrameWidth
    {
      get { return _texture.Width / _frameCount; }
    }

    public int FrameHeight
    {
      get { return _texture.Height; }
    }

    public float FrameTime
    {
      get { return _frameTime; }
    }

    public int FrameCount
    {
      get { return _frameCount; }
    }

    public bool IsLooping
    {
      get { return _isLooping; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="texture">The image used for the animation</param>
    /// <param name="frameCount">How many different frames are in the image</param>
    /// <param name="frameTime">How quickly the animation changes</param>
    /// <param name="isLooping">Whether or not the animation loops</param>
    public Animation(Texture2D texture, int frameCount, float frameTime, bool isLooping)
    {
      _texture = texture;
      _frameCount = frameCount;
      _frameTime = frameTime;
      _isLooping = isLooping;
    }
  }
}
