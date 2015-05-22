using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Poker_Server_Client
{
    class Game_start
    {
        static byte[] data = new byte[4096];
        static IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1235);
        static int rev = 0;
        static String Ip = "127.0.0.1";
        static int Port = 1235;
        static TcpClient tcpclient = null;
        static NetworkStream ns = null;
        public static Socket client;
        static AutoResetEvent arEvent = new AutoResetEvent(false);

        public String[] Player_Inf = new string[4];                    //0.rdy 1.玩家牌1 2.玩家牌2 3.玩家編號
        public int[] Public_Card = new int[5];
        public int Player_number = -1;       //玩家編號
        public int Big_Blind = 0;               //盲注金額
        public int Call_money = 0;          //call money if u want to play
        public int Player_money = 0;         //玩家起始擁有金額
        public int Player_Big_Blid_state = -1;   //判斷使用者是否為大小盲注 -> 0.小盲1.大盲2.不是
        public int Raise_money = 0;                 //下注金額


        public int Player_State = 0;                //1=不玩
        public String Game_State = null;                  //遊戲狀態第幾回合
        public int total_money = 0;            //獎池
        public String[] Inf;
        public int tmp_Raise_money = 0;        //暫存下注金額
        public int Need_money = 0;                 //玩家須下注金額
        protected int max_leangh = 0;                 //debug


        protected int total_Player = 0;                //總玩家人數



        public virtual void Back(Socket client)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("OK");
            client.Send(data);
        }
    }
}
