using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Texas_Poker_Server
{
    class GetsCard : Server
    {
        public GetsCard(int n)
        {
            Random rd = new Random();
            switch (n)
            {
                case 0:
                    for (int i = 0; i < Now_sit.Length; i++)
                        if (Now_sit[i] == 1)
                        { 
                            int a = rd.Next(0, 52);
                            while (Check(a) == false)
                                a = rd.Next(0, 52);
                            User_card[i, 0] = a;
                            while (Check(a) == false)
                                a = rd.Next(0, 52);
                            User_card[i, 1] = a;
                            Send_Package(i);
                        }
                    break;
            }
        }

        /// <summary>
        /// 發送玩家手牌
        /// </summary>
        /// <param name="location"></param>
        public  override void Send_Package(int location)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("Player_Card" + " " + User_card[location, 0].ToString() + " " + User_card[location, 1].ToString() + " " +location.ToString() + " ");
            Console.WriteLine(Encoding.ASCII.GetString(data));
            sClient[location].Send(data);

            sClient[location].Receive(data);
            Console.WriteLine("Send card to{0}", location);
        }

        /// <summary>
        /// 判斷是否發過牌
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
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
