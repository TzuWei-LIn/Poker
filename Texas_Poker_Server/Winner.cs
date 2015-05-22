using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texas_Poker_Server
{
    class Winner : Game_Round
    {
        public Winner(int location)
        {
            Player_money[location] += Total_money;
        }
    }
}
