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
        public float thrust = .075f;
        public float GRAVITY = .025f;


        public PlayerShip(Vector2 position) 
        {
            this.position = position;
            velocity = Vector2.Zero;
        }

        public void rotateRight()
        {
            rotation += .02f;
            rotation = rotation % 360;
        }
        public void rotateLeft()
        {
            rotation -= .02f;
            rotation = rotation % 360;
        }

        public void applyThrust()
        {
            float xThrust = thrust * (float)Math.Sin(rotation);
            float yThrust = thrust * (float)Math.Cos(rotation);

            velocity.X += xThrust;
            velocity.Y -= yThrust;

        }

        public void update(GameTime gametime)
        {
            velocity.Y += GRAVITY;
            position = Vector2.Add(velocity, position);
        }

    }
}
