using LunarLander.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LunarLander.Views.MainMenu;
using Microsoft.Xna.Framework.Input;

namespace LunarLander.Views.Credits
{
    public class CreditsView : GameStateView
    {
        private SpriteFont m_menuFont;
        private MenuStateEnum m_currentSelection = MenuStateEnum.Credits;
        public override void loadContent(ContentManager contentManager)
        {
            m_menuFont = contentManager.Load<SpriteFont>("Fonts/anta-regular");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) 
            {
                return GameStateEnum.MainMenu;
            }
            return GameStateEnum.Credits;
        }

        public override void render(GameTime gameTime)
        {

            m_spriteBatch.Begin();
            float position = drawMenuItem(m_menuFont, "Credits", m_graphics.PreferredBackBufferHeight/2, new Color(1, 59, 89));
            drawMenuItem(m_menuFont, "Created By: Zane Hirning", position, new Color(1, 59, 89));
            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }
        
        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {
            Vector2 stringSize = font.MeasureString(text);
            m_spriteBatch.DrawString(
                font,
                text,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, y),
                color
            );
            return y + stringSize.Y;
        }
    }
}
