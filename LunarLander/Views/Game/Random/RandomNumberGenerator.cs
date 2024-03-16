using System;
using Microsoft.Xna.Framework;

namespace LunarLander.Views.Game.RandomGenerator
{
    public class RandomNumberGenerator : Random
    {

        public float nextRange(float min, float max)
        {
            return MathHelper.Lerp(min, max, (float)this.NextDouble());
        }

        public Vector2 nextCircleVector()
        {
            float angle = (float)(this.NextDouble() * 2.0 * Math.PI);
            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);

            return new Vector2(x, y);
        }

        public Vector2 nextVectorInDirection(Vector2 direction) 
        {
            direction.X += Convert.ToSingle(this.nextGaussian(2, 5));
            direction.Y += Convert.ToSingle(this.nextGaussian(2, 5));
            direction.Normalize();
            return direction;
        }

        public double nextGaussian(double mean, double stdDev)
        {
            if (this.usePrevious)
            {
                this.usePrevious = false;
                return mean + y2 * stdDev;
            }
            this.usePrevious = true;

            double x1 = 0.0;
            double x2 = 0.0;
            double y1 = 0.0;
            double z = 0.0;

            do
            {
                x1 = 2.0 * this.NextDouble() - 1.0;
                x2 = 2.0 * this.NextDouble() - 1.0;
                z = (x1 * x1) + (x2 * x2);
            }
            while (z >= 1.0);

            z = Math.Sqrt((-2.0 * Math.Log(z)) / z);
            y1 = x1 * z;
            y2 = x2 * z;

            return mean + y1 * stdDev;
        }

        private double y2;
        private bool usePrevious { get; set; }
    }
}

