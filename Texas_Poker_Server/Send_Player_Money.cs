using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texas_Poker_Server
{
    class Send_Player_Money : Server
    {

        public Send_Player_Money()
        {
            for (int i = 0; i < Now_sit.Length; i++)
                if (Now_sit[i] == 1)
                    Send_Package(i);
        }

        public override void Send_Package(int location)
        {

            byte[] Money = new byte[1024];
            if (Player_money[location] == 0)
                Money = Encoding.ASCII.GetBytes("Money_Inf" + " " + "3000");
            else
                Money = Encoding.ASCII.GetBytes("Money_Inf" + " " + Player_money[location].ToString());
            Console.WriteLine("Send money to{0} ={1}", location, Player_money[location]);
            sClient[location].Send(Money);

            sClient[location].Receive(Money);
        }

    }
}
