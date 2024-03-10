using LunarLander.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace LunarLander.Views.Settings
{
    public class SettingsView : GameStateView
    {
        public override void loadContent(ContentManager contentManager)
        {
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            return GameStateEnum.MainMenu;
        }

        public override void render(GameTime gameTime)
        {
            
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
