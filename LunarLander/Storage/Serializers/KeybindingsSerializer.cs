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

        public Keybindings(Dictionary<String, Keys> keybindings)
        {

            this.keys = keybindings;
        }

        [DataMember()]
        public Dictionary<String, Keys> keys = new Dictionary<String, Keys>();
    }
}

