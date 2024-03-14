using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Input;

namespace LunarLander.Storage 
{
    [DataContract(Name = "Keybindings")]
    public class Keybindings
    {
        public Keybindings() {
        }

        public Keybindings(String control, Keys key)
        {
            this.Control = control;
            this.Key = key;

            this.keys.Add(control, key);
        }

        [DataMember()]
        public string Control { get; set; }
        [DataMember()] 
        public Keys Key { get; set; }
        [DataMember()]
        public Dictionary<String, Keys> keys = new Dictionary<String, Keys>();
    }
}

