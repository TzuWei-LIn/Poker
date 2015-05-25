using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texas_Poker_Server
{
    class Button_Process : Game_Round
    {
        public String Event_Process(String a,int location)
        {
            Console.WriteLine("GGGGGGGGG = {0}", a);
            String[] b = a.Split(' ');
            switch (b[0])
            {
                case "Call":
                    if (int.Parse(b[1]) > Raise_Money)
                        Raise_Money = int.Parse(b[1]);
                    Total_money += int.Parse(b[1]);
                    Console.WriteLine("GG = {0}",call_ppl);
                    call_ppl++;
                    Player_money[location] = int.Parse(b[2]);
                    UI_Inf ui = new UI_Inf(location, "Call");
                    return Raise_Money.ToString() + " call";
                case "Raise":
                    Raise_Money = int.Parse(b[1]);
                    Total_money += int.Parse(b[2]);
                    //Player_money[location] = Player_money[location] - int.Parse(b[2]);
                    Player_money[location] = int.Parse(b[3]);
                    call_ppl = 0;
                    ui = new UI_Inf(location, "Raise");
                    return Raise_Money.ToString() + " rasie";
                case "Fold":
                    //Player_state.Remove(location);
                    return Raise_Money.ToString() + " fold";
                default:
                    byte[] data = new byte[1024];
                    data = Encoding.ASCII.GetBytes("Resend end");
                    Console.WriteLine(Encoding.ASCII.GetString(data));
                    sClient[location].Send(data);
                    Console.WriteLine("Send resend to{0}", location);
                    return "GG";
            }
        }

    }
}
