using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Texas_Poker_Server
{
    class Winner : Game_Round
    {
        public Winner(int location)
        {
            Player_money[location] += Total_money;
        }

        public Winner()
        {
            Console.WriteLine("Total Money = {0}", Total_money);
            for (int i = 0; i < Now_sit.Length; i++)
                if(Now_sit[i] !=0)
                Console.WriteLine("Player {0} money = {1}", i, Player_money[i]);
            PokerAlgorithm PK = new PokerAlgorithm();
            PK.Public_card = Public_card;
            PK.CC = 1;
            PK.Main_Process();
            public_card_result = PK.score_result;
            public_card_score = PK.score;
            Console.WriteLine("Public card  score = {0}  result = {1}", public_card_score, public_card_result);
            max_score = 0;
            for (int i = 0; i < Now_sit.Length; i++)
            {
                if (Now_sit[i] == 1)
                {
                    PokerAlgorithm PokerA = new PokerAlgorithm();
                    PokerA.User_card[0] = User_card[i, 0];
                    PokerA.User_card[1] = User_card[i, 1];
                    PokerA.Public_card = Public_card;
                    PokerA.CC = 0;
                    PokerA.Main_Process();
                    score[i] = PokerA.score;
                    score_result[i] = PokerA.score_result;
                    if (score[i] < public_card_score)
                    {
                        score[i] = public_card_score;
                        score_result[i] = public_card_result;
                    }
                }
                if (Now_sit[i] == 2)
                    score[i] = 0;
                Console.WriteLine("Player {0}  score = {1}  result = {2}", i, score[i], score_result[i]);
            }
            max_score = score.Max();
            List<int> Winner = new List<int>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Now_sit.Length; i++)
                if (score[i] == max_score)
                {
                    Winner.Add(i);
                    sb.Append(i.ToString());
                }
            for (int i = 0; i < Now_sit.Length; i++)
            {
                if (Now_sit[i] != 0)
                {
                    byte[] Send_Winner_Inf = new byte[1024];
                    Send_Winner_Inf = Encoding.ASCII.GetBytes(((score[i] == max_score) ? (Winner.Count >= 2) ? "Tie" + " " + sb.ToString() + " " + (Player_money[i] += (Total_money / 2)).ToString() : "Win" + " " +  (Player_money[i] += Total_money).ToString() : "Lose" + " " + sb.ToString()));
                    sClient[i].Send(Send_Winner_Inf);

                    sClient[i].Receive(Send_Winner_Inf);
                    Thread.Sleep(500);
                }
            }
        }
    }
}
