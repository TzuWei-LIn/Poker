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
            Back(client);
            Thread.Sleep(500);
            
            switch(b[0])
            {
                case "Player_Card":
                    Player_number = int.Parse(b[3]);
                    Player_Inf[1] = b[1];
                    Player_Inf[2] = b[2];
                    break;
                case "Money_Inf":
                    Player_money = int.Parse(b[b.Length - 1]);
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


                case "Win":
                    Player_money = int.Parse(b[1]);
                    break;
            }
        }

        /// <summary>
        /// 回傳至Server 証明收到了
        /// </summary>
        /// <param name="client"></param>
        public override void Back(Socket client)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("OK");
            client.Send(data);
        }
    }
}
