using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texas_Poker_Server
{
    class Public_Card : Server
    {
        public Public_Card(int Mode)
        {
            Random rd = new Random();
            switch (Mode)
            {
                case 1:
                    for (int i = 0; i < 3; i++, card_public_total++)
                    {
                        int a = rd.Next(0, 52);
                        while (Check(a) == false)
                            a = rd.Next(0, 52);
                        Public_card[i] = a;
                    }
                    Send_Package(0);
                    break;
                case 3:
                    int g = rd.Next(0, 52);
                    while (Check(g) == false)
                        g = rd.Next(0, 52);
                    Public_card[card_public_total] = g;
                    card_public_total++;
                    SS(Mode+1);
                    break;
                case 4:
                    g = rd.Next(0, 52);
                    while (Check(g) == false)
                        g = rd.Next(0, 52);
                    Public_card[card_public_total] = g;
                    card_public_total++;
                    SS(Mode + 1);
                    break;
            }
        }

        public override void Send_Package(int location)
        {
            for (int i = 0; i < Now_sit.Length; i++)
            {
                if (Now_sit[i] == 1)
                {
                    byte[] data = new byte[1024];
                    data = Encoding.ASCII.GetBytes("Public_Card_1-3" + " " + Public_card[0].ToString() + " " + Public_card[1].ToString() + " " + Public_card[2].ToString() + " end");
                    Console.WriteLine(Encoding.ASCII.GetString(data));
                    sClient[i].Send(data);

                    sClient[i].Receive(data);
                    Console.WriteLine("Send public card to{0}", i);
                }
            }
        }

        public  void SS(int count)      //count 是 配合 case porcess public 4 和 5 
        {
            for (int i = 0; i < Now_sit.Length; i++)
            {
                if (Now_sit[i] == 1)
                {
                    byte[] data = new byte[1024];
                    data = Encoding.ASCII.GetBytes("Public_Card_" + count.ToString() + " " + Public_card[count-1].ToString() + " end");
                    Console.WriteLine(Encoding.ASCII.GetString(data));
                    sClient[i].Send(data);

                    sClient[i].Receive(data);
                    Console.WriteLine("Send public card to{0}", i);
                }
            }
        }

        static Boolean Check(int card)
        {
            if (total_card[card] == 0)
            {
                total_card[card] = 1;
                return true;
            }
            else
                return false;
        }
    }
}
