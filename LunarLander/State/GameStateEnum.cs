using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarLander.State
{
    [Flags]
    public enum GameStateEnum
    {
        MainMenu,
        Game,
        HighScores,
        Credits,
        Settings,
        Exit
    }
}
