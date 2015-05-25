using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace Texas_Poker_Server
{
    class Game_Round : Server
    {
        protected int call_ppl { get; set; }
        protected List<int> Player_state = new List<int>();

        public void GameRound()
        {
            for (int Game_Round = 1; Game_Round < 5 ; Game_Round++)
            {
                UI_Inf ui = new UI_Inf("Total_Money");
                int Raise_position_tmp = Raise_position;
                call_ppl = 0;
                if (Game_Round != 1)
                {
                    ui = new UI_Inf("Enemy_Hide");
                    Raise_Money = 0;
                    if (Game_Round == 2)
                    {
                        Public_Card pc = new Public_Card(1);
                    }
                    else
                    {
                        Public_Card pc = new Public_Card(Game_Round);
                    }
                }
                else
                    for (int p = 0; p < Now_sit.Length; p++)
                        if (Now_sit[p] == 1)
                            Player_state.Add(p);
                while (call_ppl != Now_connect_ppl && Player_state.Count > 1)
                {
                    for (int i = Raise_position_tmp; i < Now_sit.Length + Raise_position_tmp && Player_state.Count >1 && call_ppl != Now_connect_ppl; i++)
                    {
                        int j = i % Now_sit.Length;
                        if (Now_sit[j] == 1 && Player_state.Contains(j))
                            Raise_Money = Send_Package(j, Game_Round, Raise_Money);
                    }//end for 
                    Console.WriteLine("Size = {0}", Player_state.Count());
                }//end while (end for call)
                if (Player_state.Count < 2)
                    break;
                Console.WriteLine("Finish GameROund{0}", Game_Round);
            }//end for Game_round
                Winner wr = new Winner();
        }

        public override void Send_Package(int location)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("Win " + Player_money[location] + " end");
            Console.WriteLine(Encoding.ASCII.GetString(data));
            sClient[location].Send(data);
        }

        public new int Send_Package(int location, int Game_Round, int Raise_Money)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("GameRound" + Game_Round.ToString() + " " + Raise_Money.ToString() + " end");
            Console.WriteLine(Encoding.ASCII.GetString(data));
            sClient[location].Send(data);

            String test = Encoding.ASCII.GetString(data, 0, (int)sClient[location].Receive(data));

            Array.Clear(data, 0, data.Length);
            data = Encoding.ASCII.GetBytes("WaitAnswer end");
            sClient[location].Send(data);

            data = new byte[1024];
            int rev = sClient[location].Receive(data);                        //receive Call or not...      
            Button_Process bp = new Button_Process();
            String a = bp.Event_Process(Encoding.ASCII.GetString(data, 0, rev), location);

            //while(a.Equals("GG"))
            //    a = bp.Event_Process(Encoding.ASCII.GetString(data, 0, rev), Raise_Money, location);

            String[] b = a.Split(' ');
            Raise_Money = int.Parse(b[0]);
            if (b[1].Equals("call"))
                call_ppl++;
            else if (b[1].Equals("fold"))
            {
                Player_state.Remove(location);
                Now_sit[location] = 2;
            }
            else
                call_ppl = 1;
            Console.WriteLine("Call ppl = {0}", call_ppl);
            return Raise_Money;

        }
    }
}
