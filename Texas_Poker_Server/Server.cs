using System;
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
        public static int Now_connect_ppl { get; private set; }
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

            Array.Copy(sit, Now_sit, sit.Length);
            GetsCard GC = new GetsCard(0);                  //玩家牌


        }

        public virtual  void Send_Package(int location)
        {

        }
    }

}
