using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarLander.Views.Game.Terrain
{
    public class TerrainGenerator
    {
        private List<Point> terrainPoints = new List<Point>();
        private List<LandingStrip> landingStrips = new List<LandingStrip>();
        //Hard coded until I decide how I want to handle it
        private int BUFFER_WIDTH = 1920;
        private int BUFFER_HEIGHT = 1080;
        private int LANDING_POINT_WIDTH = 1920 / 15; // arbitrary, will be tuned
        public TerrainGenerator(int level) 
        {
            Random rand = new Random();
            createEndpoints(rand);
            createLandingPoint(rand);
            if (level == 1)
            {
                createLandingPoint(rand);
            }

            terrainPoints.Sort((point1, point2) => point1.x.CompareTo(point2.x));
            List<Point> sortedPoints = terrainPoints;

            midpointDisplacement(sortedPoints[0], sortedPoints[1], 10, rand); //left end, start landing
            midpointDisplacement(sortedPoints[2], sortedPoints[3], 10, rand); // end landing, start landing 2 or right end
            if (level == 1) 
            {
                midpointDisplacement(sortedPoints[4], sortedPoints[5], 10, rand);
            }
        }

        public void midpointDisplacement(Point left, Point right, int count, Random rand)
        {
            if (count == 0) return;

            double midPointX = (left.x + right.x) / 2;
            double midPointY = (left.y + right.y) / 2;

            double s = .7; //surface roughness
            double r = s * (gaussianRnd(rand) * Math.Abs(left.x - right.x));

            double pointHeight = midPointY + r;

            pointHeight = pointHeight < 50 ? 50 : pointHeight;
            Debug.WriteLine(pointHeight);

            Point newPoint = new Point(midPointX, pointHeight);
            this.terrainPoints.Add(newPoint);
            midpointDisplacement(left, newPoint, count-1, rand);
            midpointDisplacement(right, newPoint, count-1, rand);
        }

        public void createEndpoints(Random rand)
        {
            int leftEndY = rand.Next(BUFFER_HEIGHT / 2) + 20;
            Point leftEndPoint = new Point(0, leftEndY);
            int rightEndY = rand.Next(BUFFER_HEIGHT / 2) + 20;
            Point rightEndPoint = new Point(BUFFER_WIDTH, rightEndY);
            terrainPoints.Add(leftEndPoint);
            terrainPoints.Add(rightEndPoint);
        }

        public void createLandingPoint(Random rand)
        {
            int buffer = Convert.ToInt32(BUFFER_WIDTH * .15);
            // We subtract 2 * buffer. We will need to add the buffer once to ensure our x is > buffer. Subtract two so x wont be == BUFFER_WIDTH
            int leftX = rand.Next(BUFFER_WIDTH - (2 * buffer)) + buffer;
            int rightX = leftX + LANDING_POINT_WIDTH;
            int landingY = rand.Next(BUFFER_HEIGHT / 2) + 20;
            Point leftPoint = new Point(leftX, landingY);
            Point rightPoint = new Point(rightX, landingY);
            terrainPoints.Add(leftPoint);
            terrainPoints.Add(rightPoint);
            landingStrips.Add(new LandingStrip(leftPoint, rightPoint));
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
        public struct LandingStrip
        {
            public Point leftPoint { get; set;}
            public Point rightPoint { get; set;}
            public LandingStrip(Point left, Point right)
            {
                this.leftPoint = left;
                this.rightPoint = right;
            }
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
