using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LunarLander.Input;
using LunarLander.State;
using LunarLander.Views.Game.Ship;
using LunarLander.Views.Game.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LunarLander.Views.Game
{
    public class GameView : GameStateView
    {
        TerrainGenerator terrain;
        List<TerrainGenerator.Point> points;
        private VertexPositionColor[] m_vertsLineStrip;
        private int[] m_indexLineStrip;
        private VertexPositionColor[] m_vertsTriStrip;
        private int[] m_indexTriStrip;
        private KeyboardInput m_inputKeyboard;
        private PlayerShip ship;

        private Texture2D m_texShip;
        private Rectangle m_rectShip;

        public GameView()
        {
        }
        public override void loadContent(ContentManager contentManager)
        {
            m_texShip = contentManager.Load<Texture2D>("Images/ship");
            m_rectShip = new Rectangle(50, 50, 60, 60);

            ship = new PlayerShip(new Vector2(50, 50));
            setupTerrain();            
            m_inputKeyboard = new KeyboardInput();
            m_inputKeyboard.registerCommand(Keys.Right, false, new IInputDevice.CommandDelegate(ship.rotateRight));
            m_inputKeyboard.registerCommand(Keys.Left, false, new IInputDevice.CommandDelegate(ship.rotateLeft));
            m_inputKeyboard.registerCommand(Keys.Up, false, new IInputDevice.CommandDelegate(ship.applyThrust));
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            return GameStateEnum.Game;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(
                m_texShip,
                new Rectangle(Convert.ToInt32(ship.position.X), Convert.ToInt32(ship.position.Y), m_rectShip.Width, m_rectShip.Height),
                null,
                Color.White,
                Convert.ToSingle(ship.rotation),
                new Vector2(m_texShip.Width / 2, m_texShip.Height / 2),
                SpriteEffects.None,
                0
            );
            
            foreach(EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                m_graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    m_vertsTriStrip, 0, points.Count + 1,
                    m_indexTriStrip, 0, points.Count + 1
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
            m_inputKeyboard.Update();
            ship.update(gameTime);
        }

        private void setupTerrain()
        {
            terrain = new TerrainGenerator(1, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight);
            points = terrain.getPoints();
            points.Sort((point1, point2) => point1.x.CompareTo(point2.x));

            // Triangle Stuff
            m_vertsTriStrip = new VertexPositionColor[2 * points.Count];
            m_indexTriStrip = new int[2 * points.Count];
            
            for (int i = 0; i < points.Count - 1; i+=2)
            {
                Debug.WriteLine($"{Convert.ToSingle(points[i].y)}, {m_graphics.PreferredBackBufferHeight - Convert.ToSingle(points[i].y)}");
                Debug.WriteLine($"{Convert.ToSingle(points[i].y)}, {Convert.ToSingle(m_graphics.PreferredBackBufferHeight - points[i].y)}");

                m_vertsTriStrip[i].Position = new Vector3(Convert.ToSingle(points[i].x), Convert.ToSingle(m_graphics.PreferredBackBufferHeight), 0); // point under triangle
                m_vertsTriStrip[i].Color = Color.Gray;
                m_indexTriStrip[i] = i;

                // Actual Point
                m_vertsTriStrip[i + 1].Position = new Vector3(Convert.ToSingle(points[i].x), m_graphics.PreferredBackBufferHeight - Convert.ToSingle(points[i].y), 0);
                m_vertsTriStrip[i + 1].Color = Color.Gray;
                m_indexTriStrip[i + 1] = i + 1;

                //Connector point
                m_vertsTriStrip[i + 2].Position = new Vector3(Convert.ToSingle(points[i + 1].x), m_graphics.PreferredBackBufferHeight - Convert.ToSingle(points[i + 1].y), 0);
                m_vertsTriStrip[i + 2].Color = Color.Gray;
                m_indexTriStrip[i + 2] = i + 2;
            }

            m_vertsTriStrip[points.Count].Position = new Vector3(m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight, 0);
            m_vertsTriStrip[points.Count].Color = Color.Gray;
            m_indexTriStrip[points.Count] = points.Count;

            m_vertsTriStrip[points.Count + 1].Position = new Vector3(m_graphics.PreferredBackBufferWidth - 5, m_graphics.PreferredBackBufferHeight, 0);
            m_vertsTriStrip[points.Count + 1].Color = Color.Gray;
            m_indexTriStrip[points.Count + 1] = points.Count + 1;

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
    }
}
