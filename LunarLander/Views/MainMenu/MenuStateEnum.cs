using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarLander.Views.MainMenu
{
    [Flags]
    public enum MenuStateEnum
    {
        StartGame,
        HighScores,
        Credits,
        Settings,
        Exit
    }
}
