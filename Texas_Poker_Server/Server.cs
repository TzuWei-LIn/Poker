﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Texas_Poker_Server
{
    class Server
    {
        public static int connect_ppl { get; set; }                 //連線人數
        public static int Now_connect_ppl { get; set; }
        public static int[] sit = new int[10];                          //坐位
        protected static int[] Now_sit = new int[10];

        public static int port = 1235;         //port
        public static String ip = "127.0.0.1"; //ip
        public static IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), port);
        public static Socket ss;
        //public static List<Socket> sClient = new List<Socket>();
        public static Socket[] sClient = new Socket[10];
        public static AutoResetEvent arEvent = new AutoResetEvent(false);

        public static int[,] User_card = new int[10, 2];                           //紀錄玩家的牌 10人2張
        public static int[] total_card = new int[52];                              //總共牌數 判斷哪些有發過
        protected static int[] Player_money = new int[10];                            //玩家金錢
        protected static int Raise_position = 0;                                           //下注起始編號
        protected static int Blind_position = 0;                                        //小盲注起始位置
        protected static int Big_Blind = 100;                                         //大盲注
        protected static int[] Player_Raise_Money = new int[10];              //每個玩家下注金額(更新資訊用)
        protected static int Total_money = 0;                                        //獎池
        protected static int Raise_Money = Big_Blind;                           //下注金額
        protected static int[] Public_card = new int[5];                              //公牌
        protected static int card_public_total = 0;                                   //公牌張數
        protected static int public_card_score = 0;                                 //公牌分數
        protected static String public_card_result = null;                          //公牌牌形
        protected static int[] score = new int[10];                                      //玩家分數
        protected static String[] score_result = new string[10];                  //玩家牌形
        protected static int max_score { get; set; }                                     //結束所有回合後 最高分數
        protected static String[] AC_money = new string[10];                    //紀錄玩家帳號 已變更新資料庫
        protected static StringBuilder Winner_Inf = new StringBuilder();           //顯示贏家資訊

        static void Main(string[] args)
        {
            ss = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ss.Bind(ipep);
            ss.Listen(10);
            Console.WriteLine("Sever start");
            Console.WriteLine("Server ip=" + ipep);
            Console.WriteLine("Sevrer port = " + port.ToString());

            Thread tt = new Thread(Listen.Listener);
            tt.Start();
            arEvent.WaitOne();              //waiting here until enough player
            while (true)
            {
                Array.Copy(sit, Now_sit, sit.Length);
                Now_connect_ppl = connect_ppl;

                UI_Inf ui = new UI_Inf("Enemy_Name");
                GetsCard GC = new GetsCard(0);                  //發牌給玩家(0)
                ui = new UI_Inf("Enemy_Inf");
                Send_Player_Money SM = new Send_Player_Money();     //發錢給玩家
                Blind bd = new Blind();                         //大小盲注
                Total_money = Big_Blind + (Big_Blind / 2);
                Game_Round GR = new Game_Round();
                GR.GameRound();
                Console.WriteLine("Finish Game");
                Broadcasting();
                EndGame();
                Clean_Data();

                //Console.ReadKey();
                Thread.Sleep(2000);
            }

        }

        private static void Clean_Data()
        {
            User_card = new int[10, 2];                           //紀錄玩家的牌 10人2張
            total_card = new int[52];                              //總共牌數 判斷哪些有發過
            Player_Raise_Money = new int[10];              //每個玩家下注金額(更新資訊用)
            Total_money = 0;                                        //獎池
            Public_card = new int[5];                              //公牌
            card_public_total = 0;                                   //公牌張數
            public_card_score = 0;                                 //公牌分數
            public_card_result = null;                          //公牌牌形
            score = new int[10];                                      //玩家分數
            score_result = new string[10];                  //玩家牌形
            Raise_Money = Big_Blind;
            max_score = 0;                                     //結束所有回合後 最高分數
        }

       //private static void Exit()
       // {
       //     for (int i = 0; i < Now_sit.Length; i++)
       //     {
       //         if (Now_sit[i] != 0)
       //         {
       //             byte[] data = new byte[1024];
       //             data = Encoding.ASCII.GetBytes("Continue?" + " end");
       //             sClient[i].Send(data);
       //             String a = Package_Rev2(i);
       //             String[] b = a.Split(' ');

       //             switch (b[0])
       //             {
       //                 case "Leave":
       //                     connect_ppl--;
       //                     sit[i] = 0;
       //                     if (connect_ppl < 2)
       //                         arEvent = new AutoResetEvent(false);
       //                     break;
       //                 default:
       //                     break;
       //             }
       //         }
       //         Send_Package_Server(i);
       //     }
       //     Console.WriteLine("Send Finish to{0}", i);
       // }

        private static void Broadcasting()
        {
            for (int i = 0; i < Now_sit.Length; i++)
            {
                if (Now_sit[i] != 0)
                {
                    byte[] data = new byte[1024];
                    data = Encoding.ASCII.GetBytes(Winner_Inf.ToString() + " end");
                    Console.WriteLine(Encoding.ASCII.GetString(data));
                    sClient[i].Send(data);

                    Package_Rev2(i);

                    Console.WriteLine("Send Finish Winner_Inf to{0}", i);
                }
            }
        }
        private static void EndGame()
        {
            for (int i = 0; i < Now_sit.Length; i++)
            {
                if (Now_sit[i] != 0)
                {
                    byte[] data = new byte[1024];
                    data = Encoding.ASCII.GetBytes("New_Round" + " " + Player_money[i].ToString() + " end");
                    Console.WriteLine(Encoding.ASCII.GetString(data));
                    sClient[i].Send(data);
                    Package_Rev2(i);
                }
            }
        }

        public virtual void Send_Package(int location)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("OK end");
            sClient[location].Send(data);
        }

        public static void Send_Package_Server(int location)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("OK end");
            sClient[location].Send(data);
        }

        protected virtual void Send_Package(int location, int Game_Round, int Raise_Money)
        {

        }

        public static String Package_Rev2(int location)
        {
            byte[] data = new byte[1024];
            int rev = sClient[location].Receive(data);
            StringBuilder sb = new StringBuilder();
            String a = Encoding.ASCII.GetString(data, 0, rev);
            sb.Append(a);
            String[] b = a.Split(' ');
            /////防止封包接收不完整
            while (!b[b.Length - 1].Equals("end"))
            {
                data = new byte[1024];
                rev = sClient[location].Receive(data);
                a = Encoding.ASCII.GetString(data, 0, rev);
                sb.Append(a);
                b = sb.ToString().Split(' ');
            }
            return sb.ToString();
        }

        public virtual String Package_Rev(int location)
        {
            byte[] data = new byte[1024];
            int rev = sClient[location].Receive(data);
            StringBuilder sb = new StringBuilder();
            String a = Encoding.ASCII.GetString(data, 0, rev);
            sb.Append(a);
            String[] b = sb.ToString().Split(' ');
            /////防止封包接收不完整
            while (!b[b.Length - 1].Equals("end"))
            {
                data = new byte[1024];
                rev = sClient[location].Receive(data);
                a = Encoding.ASCII.GetString(data, 0, rev);
                sb.Append(a);
                b = sb.ToString().Split(' ');
            }
            Console.WriteLine("AAA = {0}", sb.ToString());
            Thread.Sleep(100);
            return sb.ToString();
        }

    }

}
