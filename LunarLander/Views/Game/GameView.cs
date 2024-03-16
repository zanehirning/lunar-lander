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
using LunarLander.Views.Game.Particles;

namespace LunarLander.Views.Game
{
    public class GameView : GameStateView
    {
        TerrainGenerator m_terrain;
        List<TerrainGenerator.Point> m_points;
        private bool m_gameOver = false;
        private bool m_shipLanded = false;
        private VertexPositionColor[] m_vertsLineStrip;
        private int[] m_indexLineStrip;
        private VertexPositionColor[] m_vertsTriStrip;
        private int[] m_indexTriStrip;
        private VertexPositionColor[] m_vertsCircleStrip;
        private int[] m_indexCircleStrip;
        private KeyboardInput m_inputKeyboard;
        private PlayerShip m_ship;
        private bool m_isBackgroundRendered;
        private String m_fuelString;
        private String m_speedString;
        private String m_angleString;
        private SpriteFont m_antaFont;
        private int m_shipSize;
        private TerrainGenerator.Circle m_shipCircle;

        private Texture2D m_texShip;
        private Rectangle m_rectShip;
        private Texture2D m_texBackground;
        private Rectangle m_rectBackground;

        private ParticleSystem m_particleSystemFire;
        private ParticleSystem m_particleSystemSmoke;
        private ParticleSystemRenderer m_renderFire;
        private ParticleSystemRenderer m_renderSmoke;
        private ParticleSystem m_particleSystemCrash;
        private ParticleSystemRenderer m_renderCrash;

        private Vector2 m_thrusterPos;
        private Vector2 m_rotationDirection;

        public GameView()
        {
        }
        public override void loadContent(ContentManager contentManager)
        {
            m_shipSize = m_graphics.PreferredBackBufferWidth / 45;
            m_texShip = contentManager.Load<Texture2D>("Images/ship-2");
            m_rectShip = new Rectangle(50, 50, m_shipSize, m_shipSize);
            m_texBackground = contentManager.Load<Texture2D>("Images/background-2");
            m_rectBackground = new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight);

            m_ship = new PlayerShip(new Vector2(50 + m_shipSize / 2, 50 + m_shipSize / 2));
            m_antaFont = contentManager.Load<SpriteFont>("Fonts/anta-regular");
            m_shipCircle = new TerrainGenerator.Circle(new TerrainGenerator.Point(m_ship.position.X, m_ship.position.Y), m_shipSize / 2);
            //Particles
            m_thrusterPos = new Vector2(0, (m_shipSize / 2));
            m_rotationDirection =  Vector2.Transform(m_thrusterPos, Matrix.CreateRotationZ(MathHelper.ToRadians(Convert.ToSingle(m_ship.rotation))));
            m_particleSystemFire = new ParticleSystem(
                    new Vector2(m_ship.position.X, m_ship.position.Y),
                    m_rotationDirection,
                    10, 4,
                    0.2f, 0.05f,
                    300, 50);
            m_renderFire = new ParticleSystemRenderer("Particles/fire");
            m_renderFire.LoadContent(contentManager);

            m_particleSystemSmoke = new ParticleSystem(
                    new Vector2(m_ship.position.X, m_ship.position.Y),
                    m_rotationDirection,
                    10, 4,
                    0.16f, 0.05f,
                    300, 50);
            m_renderSmoke = new ParticleSystemRenderer("Particles/smoke");
            m_renderSmoke.LoadContent(contentManager);

            m_particleSystemCrash = new ParticleSystem(
                    new Vector2(m_ship.position.X, m_ship.position.Y),
                    10, 4,
                    0.2f, 0.05f,
                    300, 50);
            m_renderCrash = new ParticleSystemRenderer("Particles/fire");
            m_renderCrash.LoadContent(contentManager);

            m_indexCircleStrip = new int[360];
            m_vertsCircleStrip = new VertexPositionColor[360];

            m_isBackgroundRendered = false;
            m_fuelString = $"Fuel: {m_ship.fuel.ToString("F2")} s";
            m_speedString = $"Speed: {m_ship.convertToMeters()} m/s";
            m_angleString = $"Angle: {m_ship.rotation.ToString("F1")}";

            setupTerrain();
            //KeyboardInput
            m_inputKeyboard = new KeyboardInput();
            m_keybindingsDAO.loadKeybinds();
            m_inputKeyboard.registerCommand(m_keybindingsDAO.loadedKeybindingState.keys["RotateRight"], false, new IInputDevice.CommandDelegate(m_ship.rotateRight));
            m_inputKeyboard.registerCommand(m_keybindingsDAO.loadedKeybindingState.keys["RotateLeft"], false, new IInputDevice.CommandDelegate(m_ship.rotateLeft));
            m_inputKeyboard.registerCommand(m_keybindingsDAO.loadedKeybindingState.keys["thrust"], false, new IInputDevice.CommandDelegate(m_ship.applyThrust));
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                    return GameStateEnum.MainMenu;
            }
            return GameStateEnum.Game;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            if (!m_isBackgroundRendered)
            {
                m_spriteBatch.Draw(m_texBackground, m_rectBackground, Color.White);
                m_isBackgroundRendered = true;
            }
            if (!m_gameOver)
            {
                drawShip();
            }
            drawShipStatus();
            drawTerrain();
            m_spriteBatch.End();
            m_renderSmoke.draw(m_spriteBatch, m_particleSystemSmoke);
            m_renderFire.draw(m_spriteBatch, m_particleSystemFire);
            m_renderCrash.draw(m_spriteBatch, m_particleSystemCrash);
        }

        public override void update(GameTime gameTime)
        {
            m_shipCircle.center = new TerrainGenerator.Point(m_ship.position.X, m_ship.position.Y);
            for (int i = 0; i < m_points.Count - 1; i++) 
            {
                if (m_ship.canLand && m_terrain.isIntersecting(m_points[i], m_points[i + 1], m_shipCircle)) 
                {
                    m_shipLanded = true;
                    m_gameOver = true;
                }
                else if (m_terrain.isIntersecting(m_points[i], m_points[i+1], m_shipCircle)) 
                {
                    m_particleSystemCrash.shipCrash();
                    m_gameOver = true;
                }
            }

            m_inputKeyboard.Update();

            if (m_ship.isThrusting) 
            {
                m_particleSystemFire.shipThrust();
                m_particleSystemSmoke.shipThrust();
                Debug.WriteLine("Thrusting");
            }
            m_ship.update(gameTime);
            //circle strip for testing
            m_indexCircleStrip = new int[360];
            m_vertsCircleStrip = new VertexPositionColor[360];
            for (int i = 0; i < 360; i++)
            {
                m_indexCircleStrip[i] = i;
                m_vertsCircleStrip[i].Position = new Vector3(Convert.ToSingle(m_shipCircle.center.x + (m_shipCircle.radius * Math.Cos((float) i/180 * Math.PI))), Convert.ToSingle(m_shipCircle.center.y + (m_shipCircle.radius * Math.Sin((float)i/180 * Math.PI))), 0);
                m_vertsCircleStrip[i].Color = Color.Red;
            }

            m_thrusterPos = new Vector2(0, (m_shipSize / 2));
            m_rotationDirection = Vector2.Transform(m_thrusterPos, Matrix.CreateRotationZ(MathHelper.ToRadians(Convert.ToSingle(m_ship.rotation))));
            m_particleSystemFire.center = m_ship.position + m_rotationDirection;
            m_particleSystemFire.direction = m_rotationDirection;
            m_particleSystemSmoke.center = m_ship.position + m_rotationDirection;
            m_particleSystemSmoke.direction = m_rotationDirection;
            m_particleSystemCrash.center = m_ship.position;
            m_fuelString = $"Fuel: {Math.Abs(m_ship.fuel).ToString("F2")} s";
            m_speedString = $"Speed: {m_ship.convertToMeters().ToString("F2")} m/s";
            m_angleString = $"Angle: {m_ship.rotation.ToString("F1")}";
            m_particleSystemFire.update(gameTime);
            m_particleSystemSmoke.update(gameTime);
            m_particleSystemCrash.update(gameTime);
        }

        private void setupTerrain()
        {
            m_terrain = new TerrainGenerator(1, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight);
            m_points = m_terrain.getPoints();
            m_points.Sort((point1, point2) => point1.x.CompareTo(point2.x));

            // Triangle Stuff
            m_vertsTriStrip = new VertexPositionColor[2 * m_points.Count];
            m_indexTriStrip = new int[2 * m_points.Count];

            for (int i = 0; i < m_points.Count - 1; i += 2)
            {
                m_vertsTriStrip[i].Position = new Vector3(Convert.ToSingle(m_points[i].x), Convert.ToSingle(m_graphics.PreferredBackBufferHeight), -1); // point under triangle
                m_vertsTriStrip[i].Color = new Color(46, 47, 52);
                m_indexTriStrip[i] = i;

                // Actual Point
                m_vertsTriStrip[i + 1].Position = new Vector3(Convert.ToSingle(m_points[i].x), Convert.ToSingle(m_points[i].y), -1);
                m_vertsTriStrip[i + 1].Color = new Color(46, 47, 52);
                m_indexTriStrip[i + 1] = i + 1;

                //Connector point
                m_vertsTriStrip[i + 2].Position = new Vector3(Convert.ToSingle(m_points[i + 1].x), Convert.ToSingle(m_points[i + 1].y), -1);
                m_vertsTriStrip[i + 2].Color = new Color(46, 47, 52);
                m_indexTriStrip[i + 2] = i + 2;
            }

            m_vertsTriStrip[m_points.Count].Position = new Vector3(m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight, -1);
            m_vertsTriStrip[m_points.Count].Color = new Color(46, 47, 52);
            m_indexTriStrip[m_points.Count] = m_points.Count;

            m_vertsTriStrip[m_points.Count + 1].Position = new Vector3(m_graphics.PreferredBackBufferWidth - 5, m_graphics.PreferredBackBufferHeight, -1);
            m_vertsTriStrip[m_points.Count + 1].Color = new Color(46, 47, 52);
            m_indexTriStrip[m_points.Count + 1] = m_points.Count + 1;

            //Line stuff
            m_vertsLineStrip = new VertexPositionColor[m_points.Count + 2]; //Add two m_points at the bottom of screen to complete the piece
            m_indexLineStrip = new int[m_points.Count + 2];

            m_vertsLineStrip[0].Position = new Vector3(0, m_graphics.PreferredBackBufferHeight - 50, -1);
            m_vertsLineStrip[0].Color = new Color(113, 116, 128);
            m_vertsLineStrip[m_points.Count + 1].Position = new Vector3(m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight - 50, -1);
            m_vertsLineStrip[m_points.Count + 1].Color = new Color(113, 116, 128);
            m_indexLineStrip[0] = 0;
            m_indexLineStrip[m_points.Count + 1] = m_points.Count + 1;
            for (int i = 0; i < m_points.Count; i++)
            {
                TerrainGenerator.Point point = m_points[i];
                m_vertsLineStrip[i + 1].Position = new Vector3(Convert.ToSingle(point.x), Convert.ToSingle(point.y), point.z);
                m_vertsLineStrip[i + 1].Color = new Color(113, 116, 128);
                m_indexLineStrip[i + 1] = i + 1;
            }
        }

        #region Drawing
        public void drawTerrain()
        {
            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //Circle drawing for testing
                m_graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.LineStrip,
                        m_vertsCircleStrip, 0, m_vertsCircleStrip.Length - 1,
                        m_indexCircleStrip, 0, m_indexCircleStrip.Length - 1
                );

                m_graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    m_vertsTriStrip, 0, m_points.Count + 1,
                    m_indexTriStrip, 0, m_points.Count + 1
                );

                m_graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineStrip,
                    m_vertsLineStrip, 0, m_vertsLineStrip.Length - 1,
                    m_indexLineStrip, 0, m_indexLineStrip.Length - 1
                );
            }
        }
        public void drawShip()
        {
            m_spriteBatch.Draw(
                m_texShip,
                new Rectangle(Convert.ToInt32(m_ship.position.X), Convert.ToInt32(m_ship.position.Y), m_rectShip.Width, m_rectShip.Height),
                null,
                Color.White,
                Convert.ToSingle((m_ship.rotation / 180) * Math.PI),
                new Vector2(m_texShip.Width / 2, m_texShip.Height / 2),
                SpriteEffects.None,
                0
            );
        }
        public void drawShipStatus()
        {
            float bottom = drawStatus(
                m_antaFont,
                m_fuelString,
                0f,
                m_ship.fuel > 0 ? Color.Green : Color.White
            );
            bottom = drawStatus(
                m_antaFont,
                m_speedString,
                bottom,
                Math.Abs(m_ship.convertToMeters()) < 2 ? Color.Green : Color.White
            );
            bottom = drawStatus(
                m_antaFont,
                m_angleString,
                bottom,
                m_ship.rotation >= 355 || m_ship.rotation <= 5 ? Color.Green : Color.White
            );
        }

        public float drawStatus(SpriteFont font, string text, float y, Color color)
        {
            Vector2 stringSize = font.MeasureString(text);
            drawOutlineText(
                m_spriteBatch,
                font,
                text,
                Color.Black,
                color,
                new Vector2(m_graphics.PreferredBackBufferWidth - stringSize.X - 10, y),
                1f
            );
            return y + stringSize.Y;
        }

        protected static void drawOutlineText(SpriteBatch spriteBatch, SpriteFont font, string text, Color outlineColor, Color frontColor, Vector2 position, float scale)
        {
            //Demo code outline drawing
            const float PIXEL_OFFSET = 1.0f;

            spriteBatch.DrawString(font, text, position - new Vector2(PIXEL_OFFSET * scale, 0), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(PIXEL_OFFSET * scale, 0), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position - new Vector2(0, PIXEL_OFFSET * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(0, PIXEL_OFFSET * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);

            spriteBatch.DrawString(font, text, position, frontColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
        #endregion
    }
}
