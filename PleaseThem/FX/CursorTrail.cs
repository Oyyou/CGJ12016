using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PleaseThem.Models;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using PleaseThem.States;

namespace PleaseThem.FX
{
    /// <summary>
    /// Creates and manages a trail of objects
    /// </summary>
    public class CursorTrail:Component
    {
        private MouseState _currentMouse;
        private MouseState _previousMouse;
        private GameTime _previousGameTime;
        private List<Particle> _trailObjects;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _content;
        private GameState _gameState;
        private Texture2D _particleTexture;
        private int _currentParticle;

        public CursorTrail(GraphicsDevice _graphicsDevice, ContentManager content, GameState gameState)
        {
            this._graphicsDevice = _graphicsDevice;
            this._content = content;
            this._gameState = gameState;

            _trailObjects = new List<Particle>();
            //create objects
            _particleTexture = content.Load<Texture2D>("Fx/square");

            for (int i = 0; i < 10; i++)
            {
                var temp = new Particle(gameState, _particleTexture,0.1f);
                _trailObjects.Add(temp);
            }
        }
              

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw the visable objects 
            foreach (var item in _trailObjects)
            {
                item.Draw(gameTime, spriteBatch);
            }
        }

        public override string GetSaveData()
        {
            return "";
        }

        public override void Update(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            //Cursor is moving
            if(_currentMouse.Position != _previousMouse.Position)
            {
                // gameTime.ElapsedGameTime.Milliseconds
                if (_currentParticle < 10)
                {
                    _trailObjects[_currentParticle].Position = new Vector2( _currentMouse.Position.X -25, _currentMouse.Y-25);
                    _trailObjects[_currentParticle].Initialize();
                    _currentParticle++;
                }
                else
                {
                    _currentParticle = 0;
                }
            }

            //Draw the visable objects 
            foreach (var item in _trailObjects)
            {
                item.Update(gameTime);
            }
        }
    }
}
