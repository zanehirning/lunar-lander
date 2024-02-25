using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunarLander.State;

namespace LunarLander.Input
{
    public interface IInputDevice
    {
        public delegate void CommandDelegate();
        public delegate GameStateEnum StateDelegate();
        public delegate void CommandDelegatePosition();

        void Update();
    }
}
