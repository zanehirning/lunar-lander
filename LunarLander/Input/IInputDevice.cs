﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarLander.Input
{
    public interface IInputDevice
    {
        public delegate void CommandDelegate();
        public delegate void CommandDelegatePosition();

        void Update();
    }
}
