using System;
using System.IO;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using LunarLander.Storage;
using System.Diagnostics;

namespace LunarLander.State
{
    public abstract class GameStateView : IGameState
    {
        protected GraphicsDeviceManager m_graphics;
        protected SpriteBatch m_spriteBatch;
        protected BasicEffect m_effect;
        protected KeybindingsDAO m_keybindingsDAO;
        protected HighScoresDAO m_highScoresDAO;

        public void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);
            m_graphics.GraphicsDevice.RasterizerState = new RasterizerState
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.CullCounterClockwiseFace,   // CullMode.None If you want to not worry about triangle winding order
                MultiSampleAntiAlias = true,
            };

            m_effect = new BasicEffect(m_graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up),

                Projection = Matrix.CreateOrthographicOffCenter(
                    0, m_graphics.GraphicsDevice.Viewport.Width,
                    m_graphics.GraphicsDevice.Viewport.Height, 0,   // doing this to get it to match the default of upper left of (0, 0)
                    0.1f, 2)
            };
            m_keybindingsDAO = new KeybindingsDAO();
            m_keybindingsDAO.loadKeybinds();
            if (m_keybindingsDAO.loadedKeybindingState == null) 
            {
                Dictionary<String, Keys> keybindings = new Dictionary<String, Keys>();
                keybindings["thrust"] = Keys.Up;
                keybindings["RotateLeft"] = Keys.Left;
                keybindings["RotateRight"] = Keys.Right;
                m_keybindingsDAO.saveKeybind(keybindings);
            }
            m_keybindingsDAO.loadKeybinds();

            m_highScoresDAO = new HighScoresDAO();
            m_highScoresDAO.loadHighScores();
        }
        public abstract void loadContent(ContentManager contentManager);
        public abstract GameStateEnum processInput(GameTime gameTime);
        public abstract void render(GameTime gameTime);
        public abstract void update(GameTime gameTime);
    }
}
