using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarLander.Views.Settings
{
    [Flags]
    public enum SettingsStateEnum
    {
        Thrust,
        RotateLeft,
        RotateRight,
    }
}
