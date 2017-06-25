using PleaseThem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.States;
using Microsoft.Xna.Framework;

namespace PleaseThem.FX
{
    public class Particle : Sprite
    {
        private float _timer;

        private float _lifeTime;

        public void Initialize()
        {
            _timer = 0;

            IsVisible = true;
        }

        public Particle(GameState parent, Texture2D texture, float lifetime) : base(parent, texture)
        {
            _lifeTime = lifetime;
            IsVisible = false;
        }

        public override void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > _lifeTime)
            {
               IsVisible = false;
            }
        }
    }
}
