using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace LunarLander.Input
{
    public class KeyboardInput : IInputDevice
    {

        private Dictionary<Keys, CommandEntry> m_commandEntries = new Dictionary<Keys, CommandEntry>();
        private KeyboardState m_statePrevious;

        public void registerCommand(Keys key, bool keyPressOnly, IInputDevice.CommandDelegate callback)
        {
            if (m_commandEntries.ContainsKey(key))
            {
                m_commandEntries.Remove(key);
            }
            m_commandEntries.Add(key, new CommandEntry(key, keyPressOnly, callback));
        }
        public void unregisterCommand(Keys key)
        {
            if (m_commandEntries.ContainsKey(key))
            {
                m_commandEntries.Remove(key);
            }    
        }

        private struct CommandEntry
        {
            public CommandEntry(Keys key, bool keyPressOnly, IInputDevice.CommandDelegate callback)
            {
                this.key = key;
                this.keyPressOnly = keyPressOnly;
                this.callback = callback;
            }
            public Keys key;
            public bool keyPressOnly;
            public IInputDevice.CommandDelegate callback;
        }

        public void Update()
        {
            KeyboardState state = Keyboard.GetState();
            foreach (CommandEntry entry in this.m_commandEntries.Values)
            {
                if (entry.keyPressOnly && keyPressed(entry.key))
                {
                    entry.callback();
                }
                else if (!entry.keyPressOnly && state.IsKeyDown(entry.key))
                {
                    entry.callback();
                }
            }
            m_statePrevious = state;
        }
        private bool keyPressed(Keys key)
        {
            return (Keyboard.GetState().IsKeyDown(key) && !m_statePrevious.IsKeyDown(key));
        }

    }
}
