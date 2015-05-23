using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texas_Poker_Server
{
    class UI_Inf : Server
    {
        public UI_Inf(String a)
        {
            switch(a)
            {
                case "Enemy_Inf":
                    for (int i = 0; i < Now_sit.Length; i++)
                        if (Now_sit[i] != 0)
                            Send_Package(i, "Enemy_Inf" + " " + Now_connect_ppl.ToString());
                        break;
                case "Total_Money":
                   for (int i = 0; i < Now_sit.Length; i++)
                        if (Now_sit[i] != 0)
                            Send_Package(i, "Total_Money" + " " + Total_money.ToString());
                        break;

            }
        }

        public void Send_Package(int location,String a)
        {
        
                    byte[] data = new byte[1024];
                    data = Encoding.ASCII.GetBytes(a);
                    Console.WriteLine(Encoding.ASCII.GetString(data));
                    sClient[location].Send(data);

                    sClient[location].Receive(data);
                    Console.WriteLine("Send Inf to{0}", location);
        }

    }
}
