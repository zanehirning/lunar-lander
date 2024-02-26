using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LunarLander.Views.Game.Terrain
{
    public class TerrainGenerator
    {
        private List<Point> terrainPoints = new List<Point>();
        //Hard coded until I decide how I want to handle it
        private int BUFFER_WIDTH = 1920;
        private int BUFFER_HEIGHT = 1080;
        private int LANDING_POINT_WIDTH = 1920 / 15; // arbitrary, will be tuned
        public TerrainGenerator(int level) 
        {
            Random rand = new Random();
            createEndpoints(rand);
            createLandingPoints(level, rand);

            terrainPoints.Sort((point1, point2) => point1.x.CompareTo(point2.x));
            List<Point> sortedPoints = terrainPoints;

            midpointDisplacement(sortedPoints[0], sortedPoints[1], 10, .8,  rand); //left end, start landing
            midpointDisplacement(sortedPoints[2], sortedPoints[3], 10, .8, rand); // end landing, start landing 2 or right end
            if (level == 1) 
            {
                midpointDisplacement(sortedPoints[4], sortedPoints[5], 10, .8, rand);
            }
        }

        public void midpointDisplacement(Point left, Point right, int count, double s, Random rand)
        {
            if (count == 0) return;

            double midPointX = (left.x + right.x) / 2;
            double midPointY = (left.y + right.y) / 2;
            double r = s * (gaussianRnd(rand) * Math.Abs(left.x - right.x));
            r = r > BUFFER_HEIGHT / 5 ? BUFFER_HEIGHT / 5 : r; //put a max displacement
            r = r < -BUFFER_HEIGHT / 5 ? -BUFFER_HEIGHT / 45 : r;
            double pointHeight = midPointY + r;
            pointHeight = pointHeight < 40 ? 40 : pointHeight;
            Point newPoint = new Point(midPointX, pointHeight);
            this.terrainPoints.Add(newPoint);

            midpointDisplacement(left, newPoint, count - 1, s - .1, rand);
            midpointDisplacement(right, newPoint, count - 1, s - .1, rand);
        }

        public void createEndpoints(Random rand)
        {
            int leftEndY = rand.Next(BUFFER_HEIGHT / 4) + 20;
            Point leftEndPoint = new Point(0, leftEndY);
            int rightEndY = rand.Next(BUFFER_HEIGHT / 4) + 20;
            Point rightEndPoint = new Point(BUFFER_WIDTH, rightEndY);
            terrainPoints.Add(leftEndPoint);
            terrainPoints.Add(rightEndPoint);
        }

        public void createLandingPoints(int level, Random rand)
        {
            int buffer = Convert.ToInt32(BUFFER_WIDTH * .15);
            // create two, make sure they are some distance apart, but not too far
            if (level == 1)
            {
                // Place anywhere in the first half
                int firstLeftX = rand.Next(BUFFER_WIDTH / 2 - buffer - LANDING_POINT_WIDTH) + buffer;
                int firstRightX = firstLeftX + LANDING_POINT_WIDTH;
                int firstY = rand.Next(BUFFER_HEIGHT / 4) + 20;
                firstY = firstY < 40 ? 40 : firstY;
                Point firstLeftPoint = new Point(firstLeftX, firstY);
                Point firstRightPoint = new Point(firstRightX, firstY);
                terrainPoints.Add(firstLeftPoint);
                terrainPoints.Add(firstRightPoint);

                // Second landing strip. Place anywhere in the second half, plus small buffer
                int secondLeftX = rand.Next((BUFFER_WIDTH / 2) - (2 * buffer) - LANDING_POINT_WIDTH) + buffer + (BUFFER_WIDTH / 2);
                int secondRightX = secondLeftX+ LANDING_POINT_WIDTH;
                int secondY = rand.Next(BUFFER_HEIGHT / 4) + 20;
                secondY = secondY < 40 ? 40 : secondY;
                Point secondLeftPoint = new Point(secondLeftX, secondY);
                Point secondRightPoint = new Point(secondRightX, secondY);
                terrainPoints.Add(secondLeftPoint);
                terrainPoints.Add(secondRightPoint);
            }
            else
            {
                int leftX = rand.Next(BUFFER_WIDTH - (2 * buffer) - LANDING_POINT_WIDTH) + buffer;
                int rightX = leftX + LANDING_POINT_WIDTH;
                int landingY = rand.Next(BUFFER_HEIGHT / 4) + 20;
                landingY = landingY < 40 ? 40 : landingY;
                Point leftPoint = new Point(leftX, landingY);
                Point rightPoint = new Point(rightX, landingY);
                terrainPoints.Add(leftPoint);
                terrainPoints.Add(rightPoint);
            }
        }

        public double gaussianRnd(Random rand)
        {
            //I've created this before, but I took the solution from https://stackoverflow.com/questions/218060/random-gaussian-variables this time
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return randStdNormal;
        }
        public List<Point> getPoints()
        {
            return this.terrainPoints;
        }

        public struct Point
        {
            public double x { get; set; }
            public double y { get; set; }
            public int z { get; set; }

            public Point(double x, double y)
            {
                this.x = x;
                this.y = y;
                this.z = 0;
            }
        }
    }
}
