using System;
using System.Collections.Generic;
using LunarLander.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LunarLander.Views.HighScores
{
    public class HighScoresView : GameStateView
    {
        private SpriteFont m_menuFont;
        private SpriteFont m_menuSelectFont;
        
        private Texture2D m_texPlanet;
        private Rectangle m_rectPlanet;
        public override void loadContent(ContentManager contentManager)
        {
            m_highScoresDAO.loadHighScores();
            m_menuFont = contentManager.Load<SpriteFont>("Fonts/anta-regular");
            m_menuSelectFont = contentManager.Load<SpriteFont>("Fonts/anta-regular-select");
            m_texPlanet = contentManager.Load<Texture2D>("Images/Brahe_Sprite");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }
            return GameStateEnum.HighScores;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            drawPlanet();
            Vector2 headerStringSize = m_menuFont.MeasureString("High Scores");
            drawOutlineText(
                    m_spriteBatch, 
                    m_menuFont, 
                    "High Scores",
                    Color.Black,
                    new Color(1, 59, 89),
                    new Vector2(m_graphics.PreferredBackBufferWidth / 2 - (headerStringSize.X / 2), m_graphics.PreferredBackBufferHeight / 2),
                    1f);
            if (m_keybindingsDAO.loadedKeybindingState != null) 
            {
                if (m_highScoresDAO.loadedHighScoresState != null)
                {
                    List<int> highScores = m_highScoresDAO.loadedHighScoresState.Scores;
                    if (highScores.Count == 0) 
                    {
                        Vector2 stringSize = m_menuFont.MeasureString("No high scores yet");
                        drawOutlineText(
                                m_spriteBatch, 
                                m_menuFont, 
                                "No high scores yet",
                                Color.Black,
                                Color.White,
                                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - (stringSize.X / 2), m_graphics.PreferredBackBufferHeight / 2 + headerStringSize.Y),
                                1f);
                    }
                    else 
                    {
                        for (int i = 0; i < highScores.Count; i++)
                        {
                            Vector2 stringSize = m_menuFont.MeasureString((i + 1) + ". " + highScores[i]);
                            drawOutlineText(
                                    m_spriteBatch, 
                                    m_menuFont, 
                                    (i + 1) + ". " + highScores[i],
                                    Color.Black,
                                    Color.White,
                                    new Vector2(m_graphics.PreferredBackBufferWidth / 2 - (stringSize.X / 2), m_graphics.PreferredBackBufferHeight / 2 + (i * stringSize.Y + 10) + headerStringSize.Y),
                                    1f);
                        }
                    }
                }
                else
                {
                    Vector2 stringSize = m_menuFont.MeasureString("Could not load high scores");
                    drawOutlineText(
                            m_spriteBatch, 
                            m_menuFont, 
                            "Could not load high scores",
                            Color.Black,
                            Color.White,
                            new Vector2(m_graphics.PreferredBackBufferWidth / 2 - (stringSize.X / 2), m_graphics.PreferredBackBufferHeight / 2 + headerStringSize.Y),
                            1f);
                }
            }
            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }

        private void drawPlanet()
        {
            int planetSize = m_graphics.PreferredBackBufferHeight / 3; 
            m_rectPlanet = new Rectangle((m_graphics.PreferredBackBufferWidth / 2) - (planetSize / 2), (m_graphics.PreferredBackBufferHeight / 2) - planetSize - 20, planetSize, planetSize);

            m_spriteBatch.Draw(
                    m_texPlanet,
                    m_rectPlanet,
                    Color.White
                );
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
    }
}
