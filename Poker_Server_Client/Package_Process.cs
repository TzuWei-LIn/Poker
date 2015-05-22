using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Poker_Server_Client
{
    class Package_Process
    {
        public int Player_number = 0;
        public String[] Player_Inf = new string[4];                    //0.rdy 1.玩家牌1 2.玩家牌2 3.玩家編號
        public Package_Process (String rev_message,Socket client)
        {
            //MainWindow mw = new MainWindow();
            Back(client);
            String[] b = rev_message.Split(' ');
            switch(b[0])
            {
                case "Player_Card":
                    Player_number = int.Parse(b[3]);
                    Player_Inf[1] = b[1];
                    Player_Inf[2] = b[2];
                    break;
            }
        }

        /// <summary>
        /// 回傳至Server 証明收到了
        /// </summary>
        /// <param name="client"></param>
        public void Back(Socket client)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("OK");
            client.Send(data);
        }
    }
}
