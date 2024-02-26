using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunarLander.State;
using LunarLander.Views.Game.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LunarLander.Views.Game
{
    public class GameView : GameStateView
    {
        TerrainGenerator terrain = new TerrainGenerator(1);
        List<TerrainGenerator.Point> points;
        private VertexPositionColor[] m_vertsLineStrip;
        private int[] m_indexLineStrip;
        private VertexPositionColor[] m_vertsTriStrip;
        private int[] m_indexTriStrip;

        public GameView()
        {
            points = terrain.getPoints();
            points.Sort((point1, point2) => point1.x.CompareTo(point2.x));
        }
        public override void loadContent(ContentManager contentManager)
        {
            // Triangle Stuff
            m_vertsTriStrip = new VertexPositionColor[2 * points.Count + 2];
            m_indexTriStrip = new int[2 * points.Count + 2];

            //m_vertsTriStrip[0].Position = new Vector3(0, m_graphics.PreferredBackBufferHeight, 0); //bottom left
            //m_vertsTriStrip[0].Color = Color.White;
            for (int i = 0; i < points.Count - 1; i+=2)
            {
                m_vertsTriStrip[i].Position = new Vector3(Convert.ToSingle(points[i].x), Convert.ToSingle(m_graphics.PreferredBackBufferHeight), 0); // point under triangle
                m_vertsTriStrip[i].Color = Color.Gray;
                m_indexTriStrip[i] = i;

                // Actual Point
                m_vertsTriStrip[i + 1].Position = new Vector3(Convert.ToSingle(points[i + 1].x), m_graphics.PreferredBackBufferHeight - Convert.ToSingle(points[i + 1].y), 0);
                m_vertsTriStrip[i + 1].Color = Color.Gray;
                m_indexTriStrip[i + 1] = i + 1;

                //Connector point
                m_vertsTriStrip[i + 2].Position = new Vector3(Convert.ToSingle(points[i + 2].x), m_graphics.PreferredBackBufferHeight - Convert.ToSingle(points[i + 2].y), 0);
                m_vertsTriStrip[i + 2].Color = Color.Gray;
                m_indexTriStrip[i + 2] = i + 2;
            }

            m_vertsTriStrip[m_vertsTriStrip.Length - 2].Position = new Vector3(m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight, 0);
            m_vertsTriStrip[m_vertsTriStrip.Length - 2].Color = Color.Gray;
            m_indexTriStrip[m_indexTriStrip.Length - 2] = m_indexTriStrip.Length - 2;

            m_vertsTriStrip[m_vertsTriStrip.Length - 1].Position = new Vector3(m_graphics.PreferredBackBufferWidth - 5, m_graphics.PreferredBackBufferHeight, 0);
            m_vertsTriStrip[m_vertsTriStrip.Length - 1].Color = Color.Gray;
            m_indexTriStrip[m_indexTriStrip.Length - 1] = m_indexTriStrip.Length - 1;


            //Line stuff
            m_vertsLineStrip = new VertexPositionColor[points.Count + 2]; //Add two points at the bottom of screen to complete the piece
            m_indexLineStrip = new int[points.Count + 2];

            m_vertsLineStrip[0].Position = new Vector3(0, m_graphics.PreferredBackBufferHeight - 50, 0);
            m_vertsLineStrip[0].Color = Color.Red;
            m_vertsLineStrip[points.Count + 1].Position = new Vector3(m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight - 50, 0);
            m_vertsLineStrip[points.Count + 1].Color = Color.Red;
            m_indexLineStrip[0] = 0;
            m_indexLineStrip[points.Count + 1] = points.Count + 1;
            for (int i = 0; i < points.Count; i++)
            {
                TerrainGenerator.Point point = points[i];
                m_vertsLineStrip[i + 1].Position = new Vector3(Convert.ToSingle(point.x), m_graphics.PreferredBackBufferHeight - Convert.ToSingle(point.y), point.z);
                m_vertsLineStrip[i + 1].Color = Color.Red;
                m_indexLineStrip[i + 1] = i + 1;
            }
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            return GameStateEnum.Game;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            
            foreach(EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                m_graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    m_vertsTriStrip, 0, m_vertsTriStrip.Length - 1,
                    m_indexTriStrip, 0, (m_indexTriStrip.Length / 2) - 1

                );
                m_graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineStrip,
                    m_vertsLineStrip, 0, m_vertsLineStrip.Length-1,
                    m_indexLineStrip, 0, m_indexLineStrip.Length - 1
                );
            }
            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
