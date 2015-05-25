using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Poker_Server_Client
{
    class Package_Process : Game_start
    {
        public Package_Process (String rev_message,Socket client)
        {
            //MainWindow mw = new MainWindow();
            String[] b = rev_message.Split(' ');
            Back(client,b[0]);

            Thread.Sleep(200);
            
            switch(b[0])
            {
                case "Call_Inf":
                    Raise_money = int.Parse(b[1]);
                    Who_Raise = int.Parse(b[2]);
                    break;

                case "Blind_Inf":
                    Raise_money = int.Parse(b[2]);
                    Who_Raise = int.Parse(b[1]);
                    break;

                case "Raise_Inf":
                    Raise_money = int.Parse(b[1]);
                    Who_Raise = int.Parse(b[2]);
                    break;

                case "Total_Money":
                    total_money = int.Parse(b[1]);
                    break;

                case "Enemy_Inf":
                    total_Player = int.Parse(b[1]);
                    break;

                case "Player_Card":
                    Player_number = int.Parse(b[3]);
                    Player_Inf[1] = b[1];
                    Player_Inf[2] = b[2];
                    break;
                case "Money_Inf":
                    Player_money = int.Parse(b[b.Length - 2]);
                    break;

                case "Small_Blind":
                    Player_Big_Blid_state = 0;
                    Big_Blind = int.Parse(b[1]);
                    //total_Player = int.Parse(Inf[Inf.Length - 1]);
                    Player_money -= Big_Blind / 2;
                    tmp_Raise_money = Big_Blind / 2;
                    break;

                case "Big_Blind":
                    Big_Blind = int.Parse(b[1]);
                    //total_Player = int.Parse(Inf[Inf.Length - 1]);
                    Player_money -= Big_Blind;
                    tmp_Raise_money = Big_Blind;
                    Player_Big_Blid_state = 1;
                    break;

                case "Normal":
                    Player_Big_Blid_state = 2;
                    Big_Blind = int.Parse(b[1]);
                    //total_Player = int.Parse(Inf[Inf.Length - 1]);
                    break;

                case "GameRound1":
                    Game_State = b[0];
                    Raise_money = int.Parse(b[1]);
                    break;

                case "GameRound2":
                case "GameRound3":
                case "GameRound4":
                    Game_State = b[0];
                    Raise_money = int.Parse(b[1]);
                    Need_money = Raise_money - tmp_Raise_money;
                    break;

                case "Win":
                    Player_money = int.Parse(b[1]);
                    break;

                case "Tie":    
                    Player_money = int.Parse(Inf[2]);                  
                    break;

                case "Lose":
                    break;

                case "Public_Card_1-3":
                    Public_Card[0] = int.Parse(b[1]);
                    Public_Card[1] = int.Parse(b[2]);
                    Public_Card[2] = int.Parse(b[3]);
                    tmp_Raise_money = 0;
                    break;

                case "Public_Card_4":
                    Public_Card[3] = int.Parse(b[1]);
                    Raise_money = 0;
                    break;

                case "Public_Card_5":
                    Public_Card[4] = int.Parse(b[1]);
                    Raise_money = 0;
                    break;

                case "New_Round":
                    Player_money = int.Parse(b[1]);
                    Clean_Data();
                    break;
            }
        }

        /// <summary>
        /// 回傳至Server 証明收到了
        /// </summary>
        /// <param name="client"></param>
        public new void Back(Socket client,String a)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("OK end");
            client.Send(data);
        }
    }
}
