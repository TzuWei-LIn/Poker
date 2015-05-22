using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Poker_Server_Client
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Money_Label.Content = Player_money;
        }

        static byte[] data = new byte[4096];
        static IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1235);
        static int rev = 0;
        static String Ip = "127.0.0.1";
        static int Port = 1235;
        static TcpClient tcpclient = null;
        static NetworkStream ns = null;
        protected static Socket client;
        static AutoResetEvent arEvent = new AutoResetEvent(false);

        protected String[] Player_Inf = new string[4];                    //0.rdy 1.玩家牌1 2.玩家牌2 3.玩家編號
        protected int[] Public_Card = new int[5];
        protected int Player_number = -1;       //玩家編號
        protected int Big_Blind = 0;               //盲注金額
        protected int Call_money = 0;          //call money if u want to play
        protected int Player_money = 0;         //玩家起始擁有金額
        protected int Player_Big_Blid_state = -1;   //判斷使用者是否為大小盲注 -> 0.小盲1.大盲2.不是
        protected int Raise_money = 0;                 //下注金額


        protected int Player_State = 0;                //1=不玩
        protected String Game_State = null;                  //遊戲狀態第幾回合
        protected int total_money = 0;            //獎池
        protected String[] Inf;
        protected int tmp_Raise_money = 0;        //暫存下注金額
        protected int Need_money = 0;                 //玩家須下注金額
        protected int max_leangh = 0;                 //debug
        protected Image[] enemy = new Image[12];
        protected Label[] enemy_Raise = new Label[6];

        protected int total_Player = 0;                //總玩家人數
        /// <summary>
        /// 接收:
        ///     1.大小盲注狀態  -> 扣錢
        ///     2.盲注金額
        ///     1.rdy
        ///     2.玩家牌1
        ///     3.玩家牌2
        ///     4.玩家編號 
        ///呼叫playround()
        /// </summary>
        /// <summary>
        /// 接收:
        ///     1.遊戲回合
        ///     2.須下注金額
        ///     3.提示換你
        /// 傳送:
        ///     跟注 或 加注 或 蓋牌
        /// </summary>
        public void play_round()
        {
            client.NoDelay = false;
            while (true)
            {
                Updata_Inf();
                String a = null;
                byte[] data = new byte[1024];
                int rev = client.Receive(data);
                a = Encoding.ASCII.GetString(data, 0, rev);
                String[] b = a.Split(' ');

                Update_Inf(b[0], a);                                                       //更新資訊 及 處理                              
                Updata_Inf();
                arEvent.Set();

            }
        }

        public void Update_Inf(String title, String a)
        {
            Package_Process pp = new Package_Process(a, client);             //處理資訊
            switch (title)
            {
                case "Player_Card":
                    Player_number = pp.Player_number;
                    Player_Inf[1] = pp.Player_Inf[1];
                    Player_Inf[2] = pp.Player_Inf[2];
                    break;

                case "Money_Inf":
                    Player_money = pp.Player_money;                 
                    break;

                case "Small_Blind":
                    Player_Big_Blid_state = 0;
                    Big_Blind = pp.Big_Blind;
                    Player_money -= Big_Blind / 2;
                    tmp_Raise_money = Big_Blind / 2;
                    break;

                case "Big_Blind":
                    Big_Blind = pp.Big_Blind;
                    Player_money -= Big_Blind;
                    tmp_Raise_money = Big_Blind;
                    Player_Big_Blid_state = 1;
                    break;

                case "Normal":
                    Player_Big_Blid_state = 2;
                    Big_Blind = pp.Big_Blind;
                    break;
                case "GameRound1":
                    Raise_money = pp.Raise_money;
                    Need_money = pp.Need_money;
                    break;

                case "Win":
                    Player_money = pp.Player_money;
                    break;
            }
            Show_UI(title);                                                            //更新UI
        }

        /// <summary>
        /// 回傳到Server 以證明收到資訊
        /// </summary>

        public virtual void Back()
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("OK");
            client.Send(data);
        }

        public void Dynamic_Test()
        {
            enemy[0] = Image_card_1;
            enemy[1] = Image_card_2;
            enemy[2] = Enemy_Image_1;
            enemy[3] = Enemy_Image_2;
            enemy[4] = Enemy_Image_3;
            enemy[5] = Enemy_Image_4;
            enemy[6] = Enemy_Image_5;
            enemy[7] = Enemy_Image_6;
            enemy[8] = Enemy_Image_7;
            enemy[9] = Enemy_Image_8;
            enemy[10] = Enemy_Image_9;
            enemy[11] = Enemy_Image_10;

            enemy_Raise[0] = Call_money_Label_0;
            enemy_Raise[1] = Call_money_Label_1;
            enemy_Raise[2] = Call_money_Label_2;
            enemy_Raise[3] = Call_money_Label_3;
            enemy_Raise[4] = Call_money_Label_4;
            enemy_Raise[5] = Call_money_Label_5;

            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                for (int i = 0; i < enemy_Raise.Length; i++)
                    enemy_Raise[i].Visibility = System.Windows.Visibility.Hidden;
            });

        }

        private void Show_UI(String title)
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
                        {
                            switch (title)
                            {
                                case "Player_Card":
                                    if (Player_number == 0)
                                    {
                                        Server_Inf.Content = "Connect";
                                        enemy[0].Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Player_Inf[1] + @".GIF"));
                                        enemy[0].Visibility = System.Windows.Visibility.Visible;
                                        enemy[1].Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Player_Inf[2] + @".GIF"));
                                        enemy[1].Visibility = System.Windows.Visibility.Visible;
                                        Player_Number_Label.Content = Player_number;
                                    }
                                    else
                                    {
                                        Server_Inf.Content = "Connect";
                                        enemy[Player_number + 1].Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Player_Inf[1] + @".GIF"));
                                        enemy[Player_number + 1].Visibility = System.Windows.Visibility.Visible;
                                        enemy[Player_number + 2].Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Player_Inf[2] + @".GIF"));
                                        enemy[Player_number + 2].Visibility = System.Windows.Visibility.Visible;
                                        Player_Number_Label.Content = Player_number;
                                    }
                                    break;

                                case "Money_Inf":
                                case "Small_Blind":
                                case "Big_Blind":
                                case "Normal":
                                case "Win":
                                    Money_Label.Content = Player_money;
                                    break;
                                case "GameRound1":
                                    Need_money = Raise_money - tmp_Raise_money;
                                    Call_Button.Content = (Need_money > 0) ? "Call" + Need_money : "Check";
                                    Raise_Button.Content = "Raise to" + Raise_money * 2;
                                    Button_Show("Show");
                                    break;
                            }
                        });
        }

        public void Case_Process()
        {
            switch (Inf[0])
            {

                case "Money_Inf":
                    Player_money = int.Parse(Inf[1]);
                    Updata_Inf();
                    break;
                case "Small_Blind":
                    Player_Big_Blid_state = 0;
                    Big_Blind = int.Parse(Inf[1]);
                    total_Player = int.Parse(Inf[Inf.Length - 1]);
                    Player_money -= Big_Blind / 2;
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        //Call_money_Label.Content = Big_Blind / 2;
                    });
                    tmp_Raise_money = Big_Blind / 2;
                    byte[] resp = new byte[1024];
                    resp = Encoding.ASCII.GetBytes("OK");
                    client.Send(resp);
                    break;

                case "Big_Blind":
                    Big_Blind = int.Parse(Inf[1]);
                    total_Player = int.Parse(Inf[Inf.Length - 1]);
                    Player_money -= Big_Blind;
                    tmp_Raise_money = Big_Blind;
                    Player_Big_Blid_state = 1;
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        //Call_money_Label.Content = Big_Blind;
                    });
                    resp = new byte[1024];
                    resp = Encoding.ASCII.GetBytes("OK");
                    client.Send(resp);
                    break;

                case "Normal":
                    Player_Big_Blid_state = 2;
                    Big_Blind = int.Parse(Inf[1]);
                    total_Player = int.Parse(Inf[Inf.Length - 1]);
                    resp = new byte[1024];
                    resp = Encoding.ASCII.GetBytes("OK");
                    client.Send(resp);
                    break;

                case "Player_Card":
                    Player_number = int.Parse(Inf[3]);
                    Player_Inf[1] = Inf[1];
                    Player_Inf[2] = Inf[2];
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        Server_Inf.Content = "Connect";
                        Image_card_1.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Player_Inf[1] + @".GIF"));
                        Image_card_1.Visibility = System.Windows.Visibility.Visible;
                        Image_card_2.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Player_Inf[2] + @".GIF"));
                        Image_card_2.Visibility = System.Windows.Visibility.Visible;

                        Player_Number_Label.Content = Player_number;
                    });

                    resp = new byte[1024];
                    resp = Encoding.ASCII.GetBytes("OK");
                    client.Send(resp);
                    break;

                case "GameRound1":
                    Game_State = Inf[0];
                    Raise_money = int.Parse(Inf[1]);
                    Need_money = Raise_money - tmp_Raise_money;
                    //total_money = int.Parse(Inf[2]);
                    Updata_Inf();
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        Call_Button.Content = (Need_money > 0) ? "Call" + Need_money : "Check";
                        Raise_Button.Content = "Raise to" + Raise_money * 2;
                    });
                    //MessageBox.Show("Wait ur answer");
                    Button_Show("Show");

                    arEvent.WaitOne();
                    break;
                case "GameRound2":
                case "GameRound3":
                case "GameRound4":
                    Game_State = Inf[0];
                    Raise_money = int.Parse(Inf[1]);
                    //total_money = int.Parse(Inf[2]);
                    Need_money = Raise_money - tmp_Raise_money;
                    Updata_Inf();
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        if (Raise_money != 0)
                        {
                            Call_Button.Content = "Call " + (Raise_money - tmp_Raise_money);
                            Raise_Button.Content = "Raise to" + (2 * Raise_money);
                        }
                        else
                        {
                            Call_Button.Content = "Check";
                            Raise_Button.Content = "Raise" + 100;
                        }
                    });
                    //MessageBox.Show("Wait 2 round");
                    Button_Show("Show");
                    arEvent.WaitOne();
                    break;
                case "Public_Card_1-3":
                    Public_Card[0] = int.Parse(Inf[1]);
                    Public_Card[1] = int.Parse(Inf[2]);
                    Public_Card[2] = int.Parse(Inf[3]);
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        Public_card_image1.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Public_Card[0] + ".GIF"));
                        Public_card_image1.Visibility = System.Windows.Visibility.Visible;
                        Public_card_image2.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Public_Card[1] + ".GIF"));
                        Public_card_image2.Visibility = System.Windows.Visibility.Visible;

                        Public_card_image3.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Public_Card[2] + ".GIF"));
                        Public_card_image3.Visibility = System.Windows.Visibility.Visible;
                    });
                    tmp_Raise_money = 0;
                    resp = new byte[1024];
                    resp = Encoding.ASCII.GetBytes("OK");
                    client.Send(resp);
                    break;
                case "Public_Card_4":
                    Public_Card[3] = int.Parse(Inf[1]);
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        Public_card_image4.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Public_Card[3] + ".GIF"));
                        Public_card_image4.Visibility = System.Windows.Visibility.Visible;
                    });
                    tmp_Raise_money = 0;
                    resp = new byte[1024];
                    resp = Encoding.ASCII.GetBytes("OK");
                    client.Send(resp);
                    break;
                case "Public_Card_5":
                    Public_Card[4] = int.Parse(Inf[1]);
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        Public_card_image5.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Public_Card[4] + ".GIF"));
                        Public_card_image5.Visibility = System.Windows.Visibility.Visible;
                    });
                    tmp_Raise_money = 0;
                    MessageBox.Show("MaxLength = " + max_leangh.ToString());
                    resp = new byte[1024];
                    resp = Encoding.ASCII.GetBytes("OK");
                    client.Send(resp);
                    break;
                case "Win":
                    MessageBox.Show("U Win");
                    Player_money = int.Parse(Inf[2]);
                    Clean_Data();
                    Button_Show("Image_Hide");
                    Button_Show("Hide");
                    break;
                case "Tie":
                    MessageBox.Show("Tie");
                    Player_money = int.Parse(Inf[2]);
                    Clean_Data();
                    Button_Show("Image_Hide");
                    Button_Show("Hide");
                    break;
                case "Lose":
                    MessageBox.Show("U Lose");
                    Clean_Data();
                    Button_Show("Image_Hide");
                    Button_Show("Hide");
                    break;
                case "Total_Money_Inf":
                    total_money = int.Parse(Inf[Inf.Length - 1]);
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        Total_money_Label.Content = total_money;
                        for (int i = 0; i < enemy_Raise.Length; i++)
                            enemy_Raise[i].Visibility = System.Windows.Visibility.Hidden;
                    });
                    resp = new byte[1024];
                    resp = Encoding.ASCII.GetBytes("OK");
                    client.Send(resp);
                    break;
                case "Player_Raise_Money":
                    //MessageBox.Show("Number ="+Inf[1]);
                    if (int.Parse(Inf[1]) == Player_number)
                    {
                        this.Dispatcher.BeginInvoke((Action)delegate()
                        {
                            if (int.Parse(Inf[2]) != 0)
                            {
                                enemy_Raise[0].Visibility = System.Windows.Visibility.Visible;
                                enemy_Raise[0].Content = Inf[2];
                            }
                        });
                    }
                    else
                    {
                        this.Dispatcher.BeginInvoke((Action)delegate()
                     {
                         if (int.Parse(Inf[2]) != 0)
                         {
                             enemy_Raise[(int.Parse(Inf[1])) % total_Player].Visibility = System.Windows.Visibility.Visible;
                             enemy_Raise[(int.Parse(Inf[1])) % total_Player].Content = Inf[2];
                         }
                     });
                    }
                    resp = new byte[1024];
                    resp = Encoding.ASCII.GetBytes("OK");
                    client.Send(resp);
                    break;
                case "Test":
                    break;
            }
        }

        public void Updata_Inf()
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                //Total_money_Label.Content = total_money;
                for (int i = 0; i < (total_Player - 1) * 2; i += 2)
                {
                    enemy[i].Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\6655.jpg"));
                    enemy[i + 1].Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\6655.jpg"));
                }
            });
        }

        public void Button_Show(String a)
        {
            if (a.Equals("Hide"))
            {
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    Fold_Button.Visibility = System.Windows.Visibility.Hidden;
                    Call_Button.Visibility = System.Windows.Visibility.Hidden;
                    Raise_Button.Visibility = System.Windows.Visibility.Hidden;
                });
            }
            else
            {
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    Fold_Button.Visibility = System.Windows.Visibility.Visible;
                    Call_Button.Visibility = System.Windows.Visibility.Visible;
                    Raise_Button.Visibility = System.Windows.Visibility.Visible;
                });
            }
            if (a.Equals("Image_Hide"))
            {
                this.Dispatcher.BeginInvoke((Action)delegate()
               {
                   Public_card_image1.Visibility = System.Windows.Visibility.Hidden;
                   Public_card_image2.Visibility = System.Windows.Visibility.Hidden;
                   Public_card_image3.Visibility = System.Windows.Visibility.Hidden;
                   Public_card_image4.Visibility = System.Windows.Visibility.Hidden;
                   Public_card_image5.Visibility = System.Windows.Visibility.Hidden;
                   Image_card_1.Visibility = System.Windows.Visibility.Hidden;
                   Image_card_2.Visibility = System.Windows.Visibility.Hidden;
               });
            }
        }

        public void Clean_Data()
        {
            Player_Inf = new string[4];                    //0.rdy 1.玩家牌1 2.玩家牌2 3.玩家編號
            Public_Card = new int[5];
            Big_Blind = 0;               //盲注金額
            Call_money = 0;          //call money if u want to play
            Player_Big_Blid_state = -1;   //判斷使用者是否為大小盲注 -> 0.小盲1.大盲2.不是
            Raise_money = 0;                 //下注金額
            total_money = 0;
            Player_State = 0;                //1=不玩
            Game_State = null;                  //遊戲狀態第幾回合
            String[] Inf;
            tmp_Raise_money = 0;        //暫存下注金額
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    client.Connect(new IPEndPoint(IPAddress.Parse(Ip), Port));
            //    Server_Inf.Content = "Connect";
            //    Thread tr = new Thread(play_round);
            //    tr.Start();
                Button_Show("Hide");
            //    Dynamic_Test();
            //    //play_round();
            //    // RmIp和SPort分別為string和int型態, 前者為Server端的IP, 後者為Server端的Port
            //    // 同 Server 端一樣要另外開一個執行緒用來等待接收來自 Server 端傳來的資料, 與Server概念同
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sucess");
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            client.Close();
        }
        /// <summary>
        /// 傳送
        ///     0.Raise
        ///     1.Raise Money
        ///     2.Total_Money
        ///     3.Player_Money
        ///     4.Player_Numebr
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Call_Button_Click(object sender, RoutedEventArgs e)
        {
            Need_money = Raise_money - tmp_Raise_money;
            Player_money -= Need_money;
            tmp_Raise_money = Raise_money;
            //MessageBox.Show(Raise_money.ToString());
            //
            //Call_money_Label.Content = tmp_Raise_money;
            Show_UI("Money_Inf");
            byte[] Send_State = new byte[1024];
            Send_State = Encoding.ASCII.GetBytes("Call" + " " + Need_money + " " + Player_money);
            client.Send(Send_State);
            Button_Show("Hide");
        }
        /// <summary>
        /// 傳送
        ///     0.Raise
        ///     1.Raise Money
        ///     2.Total_Money
        ///     3.Player_Money
        ///     4.Player_Numebr
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Raise_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Raise_money == 0)
                Raise_money = 100;
            else
                Raise_money *= 2;
            Need_money = Raise_money - tmp_Raise_money;
            Player_money -= Need_money;
            //total_money += Need_money;
            tmp_Raise_money = Raise_money;
            //Call_money_Label.Content = tmp_Raise_money;
            Show_UI("Money_Inf");
            byte[] Send_State = new byte[2048];
            Send_State = Encoding.ASCII.GetBytes("Raise " + Raise_money.ToString() + " " + Need_money.ToString() + " " + Player_money.ToString() + " ");
            client.Send(Send_State);
            Button_Show("Hide");
            arEvent.Set();
        }

        private void Fold_Button_Click(object sender, RoutedEventArgs e)
        {
            Updata_Inf();
            byte[] Send_State = new byte[1024];
            Send_State = Encoding.ASCII.GetBytes("Fold" + " " + Raise_money.ToString() + " " + Player_money + " " + Player_number);
            Player_State = 1;
            Image_card_1.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\6655.jpg"));
            Image_card_2.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\6655.jpg"));
            client.Send(Send_State);
            Button_Show("Hide");
            arEvent.Set();
        }

        private void Connect_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(new IPEndPoint(IPAddress.Parse(Ip), Port));
                Server_Inf.Content = "Connect";
                Connect_btn.Content = "DisConnect";
                Thread tr = new Thread(play_round);
                tr.Start();
                Button_Show("Hide");
                Dynamic_Test();
                //play_round();
                // RmIp和SPort分別為string和int型態, 前者為Server端的IP, 後者為Server端的Port
                // 同 Server 端一樣要另外開一個執行緒用來等待接收來自 Server 端傳來的資料, 與Server概念同
                //Thread SckSReceiveTd = new Thread(SckSReceiveProc);
                //SckSReceiveTd.Start();
                //ClientSend();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


    }
}
