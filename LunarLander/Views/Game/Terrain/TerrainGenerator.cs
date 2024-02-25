using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarLander.Views.Game.Terrain
{
    public class TerrainGenerator
    {
        private List<Point> terrainPoints;
        private List<LandingStrip> landingStrips;
        //Hard coded until I decide how I want to handle it
        private int BUFFER_WIDTH = 1920;
        private int BUFFER_HEIGHT = 1080;
        private int LANDING_POINT_WIDTH = 1920 / 15; // arbitrary, will be tuned
        public TerrainGenerator(int level) 
        {
            this.terrainPoints = new List<Point>();
            this.landingStrips = new List<LandingStrip>();

            //If level one, add two landing points
            //Else, add one random landing points

            //landing points should have some sort of buffer for x. Do not want it too close to the screen edge
            //landing points should be random
            //Terrain should have a maximum y value. Not sure how I want to decide this or implement this. I don't want it to have a line at the maximum.

            //Get landing point via random numbers, add 10% buffer to each side
            

        }

        public void createLandingPoints(int level)
        {
            int buffer = BUFFER_WIDTH * .15;
            Random random = new Random();
            // We subtract 2 * buffer. We will need to add the buffer once to ensure our x is > buffer. Subtract two so x wont be == BUFFER_WIDTH
            int firstLandingXLeft = random.Next(BUFFER_WIDTH - (2 * buffer)) + buffer;
            int firstLandingXRight = firstLandingXLeft + LANDING_POINT_WIDTH;
            int firstLandingY = random.Next(BUFFER_HEIGHT / 4) + 20
            Point firstLeftPoint = new Point(firstLandingXLeft, firstLandingY);
            Point firstRightPoint = new Point(firstLandingXRight, firstLandingY);
            LandingStrip landingStrip = new LandingStrip(firstLeftPoint, firstRightPoint);
            terrainPoints.Add(firstLeftPoint);
            terrainPoints.Add(firstRightPoint);
            landingStrips.Add(landingStrip);
            if (level == 1)
            {
                //generate second point
                int secondLandingXLeft = random.Next(BUFFER_WIDTH - (2 * buffer)) + buffer;
                int secondLandingXRight = secondLandingXLeft + LANDING_POINT_WIDTH;
                int secondLandingY = random.Next(BUFFER_HEIGHT / 4) + 20
                Point secondLeftPoint = new Point(secondLandingXLeft, secondLandingY);
                Point secondRightPoint = new Point(secondLandingXRight, secondLandingY);
                LandingStrip landingStrip = new LandingStrip(secondLeftPoint, secondRightPoint);
                terrainPoints.Add(firstLeftPoint);
                terrainPoints.Add(firstRightPoint);
                landingStrips.Add(landingStrip);

            }
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
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
                this.z = 0;
            }
        }
    }
}
