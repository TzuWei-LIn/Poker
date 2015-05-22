using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Texas_Poker_Server
{
    class Game_Round : Server
    {
        protected int call_ppl { get; set; }
        protected List<int> Player_state = new List<int>();

        public void GameRound(int Raise_Money)
        {
            for (int Game_Round = 1; Game_Round < 5; Game_Round++)
            {
                int Raise_position_tmp = Raise_position;
                call_ppl = 0;
                if (Game_Round != 1)
                    Raise_Money = 0;
                else
                    for (int p = 0; p < Now_sit.Length; p++)
                        if (Now_sit[p] == 1)
                            Player_state.Add(p);
                while (call_ppl != Now_connect_ppl && Player_state.Count > 1)
                {
                    for (int i = Raise_position_tmp; i < Now_sit.Length + Raise_position_tmp && Player_state.Count >1; i++)
                    {
                        int j = i % Now_sit.Length;
                        if (Now_sit[j] == 1 && Player_state.Contains(j))
                            Raise_Money = Send_Package(j, Game_Round, Raise_Money);
                    }//end for 
                    Console.WriteLine("Size = {0}", Player_state.Count());
                }//end while (end for call)
                if (Player_state.Count < 2)
                    Console.WriteLine("WIN");
                    break;
                Console.ReadKey();
            }//end for Game_round
            if (Player_state.Count > 1)
            {
                //Winner wr = new Winner(10);
                //處理排的大小
            }
            else
                Player_money[Player_state[0]] += Total_money;
            Send_Package(Player_state[0]);
        }

        public override void Send_Package(int location)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("Win " + Player_money[location]);
            Console.WriteLine(Encoding.ASCII.GetString(data));
            sClient[location].Send(data);
        }

        public new int Send_Package(int location, int Game_Round, int Raise_Money)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("GameRound" + Game_Round.ToString() + " " + Raise_Money.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data));
            sClient[location].Send(data);

            //int tt = sClient[location].Receive(data);                        //receive "OK"
            String test = Encoding.ASCII.GetString(data, 0, (int)sClient[location].Receive(data));
            //if (test.Equals("OK"))
            //    Console.WriteLine("OKOKOKOK");

            data = new byte[1024];
            int rev = sClient[location].Receive(data);                        //receive Call or not...      
            Button_Process bp = new Button_Process();
            String a = bp.Event_Process(Encoding.ASCII.GetString(data, 0, rev), Raise_Money, location);
            String[] b = a.Split(' ');
            Raise_Money = int.Parse(b[0]);
            if (b[1].Equals("call"))
                call_ppl++;
            else if (b[1].Equals("fold"))
                Player_state.Remove(location);
            else
                call_ppl = 0;
            Console.WriteLine("Call ppl = {0}", call_ppl);
            return Raise_Money;

        }
    }
}
