using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.Core;
using PleaseThem.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.FX
{
    class AnimatedParticle
    {
        private AnimationPlayer _animationPlayer;
        private Animation _animation;
        private bool isVisible;
        private float _timer;
        private float _lifeTime;

        public Vector2 Position;

        public AnimatedParticle(GameState parent, Texture2D spriteSheet, int frames, float lifetime, bool isLooping)
        {
            Position = new Vector2();
            _lifeTime = lifetime;
            _animation = new Animation(spriteSheet, frames, lifetime, isLooping);
            _animationPlayer = new AnimationPlayer();    
        }

        public void Initialize()
        {
            _timer = 0;
            isVisible = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(isVisible)
            _animationPlayer.Draw(gameTime, spriteBatch, Position);
        }

        public void Update(GameTime gameTime)
        {
            if(isVisible)
            _animationPlayer.PlayAnimation(_animation);

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > _lifeTime)
            {
                isVisible = false;
            }
        }

    }
}
