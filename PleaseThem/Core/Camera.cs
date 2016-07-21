using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Core
{
  public class Camera
  {
    private int levelWidth, levelHeight;

    private Matrix transform = new Matrix();

    /// <summary>
    /// Currently not in use
    /// </summary>
    public float Scale { get; private set; }

    private Vector2 _position;
    public Vector2 Position
    {
      get { return _position; }
      private set { _position = value; }
    }

    public Matrix Transform
    {
      get { return transform; }
      private set { transform = value; }
    }

    private int _previousScroll;
    private int _currentScroll;

    public Camera(Vector2 startPosition)
    {
      Scale = 0.5f;
      Position = startPosition;
    }

    public void Update(Map map)
    {
      float speed = 3f;

      if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
        speed *= 2;

      if (Keyboard.GetState().IsKeyDown(Keys.W))
        _position.Y -= speed;
      else if (Keyboard.GetState().IsKeyDown(Keys.S))
        _position.Y += speed;

      if (Keyboard.GetState().IsKeyDown(Keys.A))
        _position.X -= speed;
      else if (Keyboard.GetState().IsKeyDown(Keys.D))
        _position.X += speed;

      _previousScroll = _currentScroll;
      _currentScroll = Mouse.GetState().ScrollWheelValue;

      if (_previousScroll < _currentScroll)
        Scale += 0.1f;
      else if (_previousScroll > _currentScroll)
        Scale -= 0.1f;

      //Scale = MathHelper.Clamp(Scale, 0.1f, 2f);

      _position.X = MathHelper.Clamp(_position.X, 0, (map.Width * Map.TileSize) - 800);
      _position.Y = MathHelper.Clamp(_position.Y, -32, ((map.Height * Map.TileSize) + 64) - 480);

      transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                  Matrix.CreateScale(1.0f);
    }
  }
}

