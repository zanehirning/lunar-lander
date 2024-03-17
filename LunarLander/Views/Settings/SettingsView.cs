using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunarLander.Input;
using LunarLander.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LunarLander.Views.Settings
{
    public class SettingsView : GameStateView
    {
        private SpriteFont m_menuFont;
        private SpriteFont m_menuSelectFont;
        
        private Texture2D m_texPlanet;
        private Rectangle m_rectPlanet;
        private SettingsStateEnum m_currentSelection = SettingsStateEnum.Thrust;
        private KeyboardInput m_inputKeyboard;
        private bool m_isSettingKeybind = false;
        private bool m_isSettingThrust = false;
        private bool m_isSettingRotateLeft = false;
        private bool m_isSettingRotateRight = false;
        private string m_thrustKeybindText = " ";
        private string m_rotateLeftKeybindText = " ";
        private string m_rotateRightKeybindText = " ";

        public override void loadContent(ContentManager contentManager)
        {
            m_menuFont = contentManager.Load<SpriteFont>("Fonts/anta-regular");
            m_menuSelectFont = contentManager.Load<SpriteFont>("Fonts/anta-regular-select");
            m_texPlanet = contentManager.Load<Texture2D>("Images/Brahe_Sprite");

            m_inputKeyboard = new KeyboardInput();
            m_inputKeyboard.registerCommand(Keys.Down, true, new IInputDevice.CommandDelegate(menuDown));
            m_inputKeyboard.registerCommand(Keys.Up, true, new IInputDevice.CommandDelegate(menuUp));

            m_thrustKeybindText = m_keybindingsDAO.loadedKeybindingState.keys["thrust"].ToString();
            m_rotateLeftKeybindText = m_keybindingsDAO.loadedKeybindingState.keys["RotateLeft"].ToString();
            m_rotateRightKeybindText = m_keybindingsDAO.loadedKeybindingState.keys["RotateRight"].ToString();
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_inputKeyboard.Update();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) 
            {
                if (m_isSettingKeybind)
                {
                    m_isSettingKeybind = false;
                    m_isSettingThrust = false;
                    m_isSettingRotateLeft = false;
                    m_isSettingRotateRight = false;
                }
                else
                {
                    return GameStateEnum.MainMenu;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                switch (m_currentSelection)
                {
                    case SettingsStateEnum.Thrust: 
                        {
                            m_isSettingKeybind = true;
                            m_isSettingThrust = true;
                            return GameStateEnum.Settings;
                        };
                    case SettingsStateEnum.RotateLeft: 
                        {
                            m_isSettingKeybind = true;
                            m_isSettingRotateLeft = true;
                            return GameStateEnum.Settings;
                        };
                    case SettingsStateEnum.RotateRight: 
                        {
                            m_isSettingKeybind = true;
                            m_isSettingRotateRight = true;
                            return GameStateEnum.Settings;
                        };
                }
            }
            return GameStateEnum.Settings;
        }

        public override void render(GameTime gameTime)
        {
            m_graphics.GraphicsDevice.Clear(Color.Black);
            m_spriteBatch.Begin();
            drawPlanet();
            if (m_keybindingsDAO.loadedKeybindingState != null) 
            {
                float bottom = drawMenuItem(m_currentSelection == SettingsStateEnum.Thrust ? m_menuSelectFont : m_menuFont, $"Thrust: {m_thrustKeybindText}", m_graphics.PreferredBackBufferHeight / 2, m_currentSelection == SettingsStateEnum.Thrust ? new Color(1, 59, 89) : Color.White);
                bottom = drawMenuItem(m_currentSelection == SettingsStateEnum.RotateLeft ? m_menuSelectFont : m_menuFont, $"Rotate Left: {m_rotateLeftKeybindText}", bottom, m_currentSelection == SettingsStateEnum.RotateLeft ? new Color(1, 59, 89) : Color.White);
                bottom = drawMenuItem(m_currentSelection == SettingsStateEnum.RotateRight ? m_menuSelectFont : m_menuFont, $"Rotate Right: {m_rotateRightKeybindText}", bottom, m_currentSelection == SettingsStateEnum.RotateRight ? new Color(1, 59, 89) : Color.White);
            }
            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
            m_keybindingsDAO.loadKeybinds();
            if (!m_isSettingKeybind)
            {
                m_isSettingThrust = false;
                m_isSettingRotateRight = false;
                m_isSettingRotateLeft = false;
            }
            if (m_isSettingThrust) 
            {
                setKeybinding("thrust", m_isSettingThrust);
            }
            if (m_isSettingRotateRight)
            {
                setKeybinding("RotateRight", m_isSettingRotateRight);
            }
            if (m_isSettingRotateLeft)
            {
                setKeybinding("RotateLeft", m_isSettingRotateLeft);
            }

            m_thrustKeybindText = m_isSettingThrust ? " " : m_keybindingsDAO.loadedKeybindingState.keys["thrust"].ToString();
            m_rotateLeftKeybindText = m_isSettingRotateLeft ? " " : m_keybindingsDAO.loadedKeybindingState.keys["RotateLeft"].ToString();
            m_rotateRightKeybindText = m_isSettingRotateRight ? " " : m_keybindingsDAO.loadedKeybindingState.keys["RotateRight"].ToString();
        }

        private void setKeybinding(String control, bool controlBeingSet) 
        {
            m_inputKeyboard.unregisterCommand(Keys.Up);
            m_inputKeyboard.unregisterCommand(Keys.Down);
            if (controlBeingSet) {
                foreach (Keys key in Keyboard.GetState().GetPressedKeys())
                {
                    Debug.WriteLine(key);
                    if (key != Keys.Escape && key != Keys.Enter) 
                    {
                        Debug.WriteLine(key);
                        Dictionary<String, Keys> prevBindings = m_keybindingsDAO.loadedKeybindingState.keys;
                        prevBindings[control] = key;
                        m_keybindingsDAO.saveKeybind(prevBindings);
                        m_keybindingsDAO.loadKeybinds();
                        m_isSettingKeybind = false;
                        break;
                    }
                    else if (key == Keys.Escape)
                    {
                        m_isSettingKeybind = false;
                        break;
                    }
                }
            } 
            m_inputKeyboard.registerCommand(Keys.Down, true, new IInputDevice.CommandDelegate(menuDown));
            m_inputKeyboard.registerCommand(Keys.Up, true, new IInputDevice.CommandDelegate(menuUp));
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

        #region Input
        private void menuDown()
        {
            if (m_currentSelection != SettingsStateEnum.RotateRight)
            {
                m_currentSelection = m_currentSelection + 1;
            }
        }
        private void menuUp()
        {
            if (m_currentSelection != SettingsStateEnum.Thrust)
            {
                m_currentSelection = m_currentSelection - 1;
            }
        }
        #endregion
    }
}
