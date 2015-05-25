using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texas_Poker_Server
{
    class Blind : Server
    {
        public Blind()
        {
            int count = 0;
            for (int i = Blind_position; i < Now_sit.Length + Blind_position; i++)
            {
                int j = i % Now_sit.Length;
                if (Now_sit[j] == 1)
                {
                    byte[] data = new byte[1024];
                    if (count == 0 )
                    {
                        data = Encoding.ASCII.GetBytes("Small_Blind" + " " + Big_Blind.ToString() + " " + j.ToString() + " end");
                        Player_Raise_Money[j] = Big_Blind / 2;
                        count++;
                        UI_Inf ui = new UI_Inf(j, "Small_Blind");
                    }
                    else if (count == 1)
                    {
                        data = Encoding.ASCII.GetBytes("Big_Blind" + " " + Big_Blind.ToString() + " " + j.ToString() + " end");
                        Player_Raise_Money[j] = Big_Blind;
                        Raise_position =( j % Now_sit.Length)+1;
                        count++;
                        UI_Inf ui = new UI_Inf(j, "Big_Blind");
                    }
                    else
                        data = Encoding.ASCII.GetBytes("Normal" + " " + Big_Blind.ToString() + " " + j.ToString()+ " end");
                    sClient[j].Send(data);
                    sClient[j].Receive(data);
                    Console.WriteLine("Send Blind to{0}", j);
                    //Thread.Sleep(500);
                }
            }
            Blind_position++;
            Console.WriteLine("End Blind");
        }
    }
}
