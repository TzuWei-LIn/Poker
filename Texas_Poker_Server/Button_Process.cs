using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texas_Poker_Server
{
    class Button_Process : Game_Round
    {
        public String Event_Process(String a,int Raise_money,int location)
        {
            String[] b = a.Split(' ');
            switch (b[0])
            {
                case "Call":
                    if (int.Parse(b[1]) > Raise_money)
                        Raise_money = int.Parse(b[1]);
                    Total_money += int.Parse(b[1]);
                    Console.WriteLine("GG = {0}",call_ppl);
                    call_ppl++;

                    Player_money[location] = int.Parse(b[2]);
                    return Raise_money.ToString() + " call";
                case "Raise":
                    Raise_money = int.Parse(b[1]);
                    Total_money += int.Parse(b[2]);
                    //Player_money[location] = Player_money[location] - int.Parse(b[2]);
                    Player_money[location] = int.Parse(b[3]);
                    call_ppl = 0;
                    return Raise_money.ToString() + " rasie";
                case "Fold":
                    //Player_state.Remove(location);
                    return Raise_money.ToString() + " fold";
                default:
                    byte[] data = new byte[1024];
                    data = Encoding.ASCII.GetBytes("Resend");
                    Console.WriteLine(Encoding.ASCII.GetString(data));
                    sClient[location].Send(data);
                    Console.WriteLine("Send resend to{0}", location);
                    return "GG";
            }
        }

    }
}
