﻿using System;
using LunarLander.Views.Game.RandomGenerator;
using System.Collections.Generic;
using System.Diagnostics;

namespace LunarLander.Views.Game.Terrain
{
    public class TerrainGenerator
    {
        private List<Point> terrainPoints = new List<Point>();
        public List<LandingStrip> landingStrips = new List<LandingStrip>();
        private int bufferWidth;
        private int bufferHeight;
        private int landingPointWidth; 
        private int shipWidth;
        public TerrainGenerator(int level, int screenWidth, int screenHeight) 
        {
            bufferWidth = screenWidth;
            bufferHeight = screenHeight;
            shipWidth = bufferWidth / 45;
            landingPointWidth = bufferWidth / 15;
            RandomNumberGenerator  rand = new RandomNumberGenerator();
            createEndpoints(rand);
            createLandingPoints(level, rand);

            terrainPoints.Sort((point1, point2) => point1.x.CompareTo(point2.x));
            List<Point> sortedPoints = terrainPoints;

            midpointDisplacement(sortedPoints[0], sortedPoints[1], 11, .8,  rand); //left end, start landing
            midpointDisplacement(sortedPoints[2], sortedPoints[3], 11, .8, rand); // end landing, start landing 2 or right end
            if (level == 1) 
            {
                midpointDisplacement(sortedPoints[4], sortedPoints[5], 11, .8, rand);
            }
        }

        public void midpointDisplacement(Point left, Point right, int count, double s, RandomNumberGenerator rand)
        {
            if (count == 0) return;

            double midPointX = (left.x + right.x) / 2;
            double midPointY = (left.y + right.y) / 2;
            double r = s * (rand.nextGaussian(0, 1) * Math.Abs(left.x - right.x));
            r = r > bufferHeight / 4 ? bufferHeight / 4 : r; //put a max displacement
            r = r < -bufferHeight / 4 ? -bufferHeight / 45 : r;
            double pointHeight = midPointY + r;
            pointHeight = pointHeight > bufferHeight - 80 ? bufferHeight - 80 : pointHeight;
            Point newPoint = new Point(midPointX, pointHeight);
            this.terrainPoints.Add(newPoint);

            midpointDisplacement(left, newPoint, count - 1, s - .1, rand);
            midpointDisplacement(right, newPoint, count - 1, s - .1, rand);
        }

        public void createEndpoints(RandomNumberGenerator rand)
        {
            int leftEndY = Convert.ToInt32(bufferHeight - rand.nextRange(bufferHeight / 3, bufferHeight / 2) + 80);
            Point leftEndPoint = new Point(0, leftEndY);
            int rightEndY = Convert.ToInt32(bufferHeight - rand.nextRange(bufferHeight / 3, bufferHeight / 2) + 80);
            Point rightEndPoint = new Point(bufferWidth, rightEndY);
            terrainPoints.Add(leftEndPoint);
            terrainPoints.Add(rightEndPoint);
        }

        public void createLandingPoints(int level, RandomNumberGenerator rand)
        {
            int buffer = Convert.ToInt32(bufferWidth * .15);
            // create two, make sure they are some distance apart, but not too far
            if (level == 1)
            {
                // Place anywhere in the first half
                int firstLeftX = rand.Next(bufferWidth / 2 - buffer - landingPointWidth) + buffer;
                int firstRightX = firstLeftX + landingPointWidth;
                int firstY = bufferHeight - rand.Next(bufferHeight / 2) + 80;
                firstY = firstY > bufferHeight - 80 ? bufferHeight - 80 : firstY;
                Point firstLeftPoint = new Point(firstLeftX, firstY);
                Point firstRightPoint = new Point(firstRightX, firstY);
                terrainPoints.Add(firstLeftPoint);
                terrainPoints.Add(firstRightPoint);
                landingStrips.Add(new LandingStrip(firstLeftPoint, firstRightPoint, shipWidth));

                // Second landing strip. Place anywhere in the second half, plus small buffer
                int secondLeftX = rand.Next((bufferWidth / 2) - (2 * buffer) - landingPointWidth) + buffer + (bufferWidth / 2);
                int secondRightX = secondLeftX+ landingPointWidth;
                int secondY = bufferHeight - rand.Next(bufferHeight / 2) + 80;
                secondY = secondY > bufferHeight - 80 ? bufferHeight - 80 : secondY;
                Point secondLeftPoint = new Point(secondLeftX, secondY);
                Point secondRightPoint = new Point(secondRightX, secondY);
                terrainPoints.Add(secondLeftPoint);
                terrainPoints.Add(secondRightPoint);
                landingStrips.Add(new LandingStrip(secondLeftPoint, secondRightPoint, shipWidth));
            }
            else
            {
                int leftX = rand.Next(bufferWidth - (2 * buffer) - landingPointWidth) + buffer;
                int rightX = leftX + landingPointWidth;
                int landingY = bufferHeight - rand.Next(bufferHeight / 2) + 80;
                landingY = landingY > bufferHeight - 80 ? bufferHeight - 80 : landingY;
                Point leftPoint = new Point(leftX, landingY);
                Point rightPoint = new Point(rightX, landingY);
                terrainPoints.Add(leftPoint);
                terrainPoints.Add(rightPoint);
                landingStrips.Add(new LandingStrip(leftPoint, rightPoint, shipWidth));
            }
        }

        public bool isIntersecting(Point pt1, Point pt2, Circle circle) 
        {
            Point v1 = new Point(pt2.x - pt1.x, pt2.y - pt1.y);
            Point v2 = new Point(pt1.x - circle.center.x, pt1.y - circle.center.y);
            double b = -2 * (v1.x * v2.x + v1.y * v2.y);
            double c = 2 * (v1.x * v1.x + v1.y * v1.y); 
            double d = Math.Sqrt(b * b - 2 * c * (v2.x * v2.x + v2.y * v2.y - circle.radius * circle.radius));
            if (Double.IsNaN(d))
            {
                return false;
            }

            double u1 = (b - d) / c;
            double u2 = (b + d) / c;
            if (u1 <= 1 && u1 >= 0) 
            {
                return true;
            }
            if (u2 <= 1 && u2 >= 0) 
            {
                return true;
            }
            return false;
        }

        public List<Point> getPoints()
        {
            return this.terrainPoints;
        }

        public List<LandingStrip> getLandingStrips()
        {
            return this.landingStrips;
        }

        #region Structs
        public struct LandingStrip
        {
            public Point leftPoint { get; set; }
            public Point rightPoint { get; set; }
            public int shipWidth { get; set; }
            public LandingStrip(Point left, Point right, int shipWidth)
            {
                this.leftPoint = left;
                this.rightPoint = right;
                this.shipWidth = shipWidth;
            }

            public bool isBetweenPoints(float x) 
            {
                return leftPoint.x + (shipWidth / 2) <= x && rightPoint.x - (shipWidth / 2) >= x;
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

        public struct Circle
        {
            public Point center { get; set; }
            public double radius { get; set; }

            public Circle(Point center, double radius)
            {
                this.center = center;
                this.radius = radius;
            }
        }
        #endregion
    }
}
