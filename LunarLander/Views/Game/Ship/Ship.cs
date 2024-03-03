using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace LunarLander.Views.Game.Ship
{
    public class PlayerShip
    {
        public Vector2 position;
        public Vector2 velocity;
        public double rotation { get; set; } = 90;
        public float thrust = .02f;
        public float GRAVITY = .01f;
        public double fuel = 20;

        public PlayerShip(Vector2 position) 
        {
            this.position = position;
            velocity = Vector2.Zero;
        }

        public void rotateRight()
        {
            rotation += 1.14f;
            rotation = (360 + rotation) % 360;
        }
        public void rotateLeft()
        {
            rotation -= 1.14f;
            rotation = (360 + rotation) % 360;
        }

        public void applyThrust()
        {
            if (fuel > 0)
            {
                float xThrust = thrust * (float)Math.Sin((rotation / 180) * Math.PI);
                float yThrust = thrust * (float)Math.Cos((rotation / 180) * Math.PI);

                velocity.X += xThrust;
                velocity.Y -= yThrust;
            }
        }

        public void update(GameTime gameTime)
        {
            fuel = fuel > 0 ? fuel - gameTime.ElapsedGameTime.TotalMilliseconds / 1000 : 0;
            velocity.Y += GRAVITY;
            position = Vector2.Add(velocity, position);
        }

    }
}
