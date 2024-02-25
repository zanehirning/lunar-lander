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
        private VertexPositionColor[] m_vertsTriStrip;
        private int[] m_indexTriStrip;

        public GameView()
        {
            points = terrain.getPoints();
            points.Sort((point1, point2) => point1.x.CompareTo(point2.x));
        }
        public override void loadContent(ContentManager contentManager)
        {
            m_vertsTriStrip = new VertexPositionColor[points.Count + 2]; //Add two points at the bottom of screen to complete the piece
            m_indexTriStrip = new int[points.Count + 2];

            m_vertsTriStrip[0].Position = new Vector3(0, m_graphics.PreferredBackBufferHeight - 50, 0);
            m_vertsTriStrip[0].Color = Color.White;
            m_vertsTriStrip[points.Count + 1].Position = new Vector3(m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight - 50, 0);
            m_vertsTriStrip[points.Count + 1].Color = Color.White;
            m_indexTriStrip[0] = 0;
            m_indexTriStrip[points.Count + 1] = points.Count + 1;
            for (int i = 0; i < points.Count; i++)
            {
                TerrainGenerator.Point point = points[i];
                m_vertsTriStrip[i + 1].Position = new Vector3(Convert.ToSingle(point.x), m_graphics.PreferredBackBufferHeight - Convert.ToSingle(point.y), point.z);
                m_vertsTriStrip[i + 1].Color = Color.White;
                m_indexTriStrip[i + 1] = i + 1;
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
                    PrimitiveType.LineStrip,
                    m_vertsTriStrip, 0, m_vertsTriStrip.Length-1,
                    m_indexTriStrip, 0, m_indexTriStrip.Length - 1
                );
            }
            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
