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
using System.Windows.Threading;
using System.Windows.Media.Animation;

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
            //timer.Completed += Timer_Run;
            timer.Tick += Timer_Run;
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
        protected int Who_Raise = 0;                    //誰下注的 用於 UI

        protected Boolean Player_Statue = false;               //玩家是否連線中
        protected String Game_State = null;                  //遊戲狀態第幾回合
        protected int total_money = 0;            //獎池
        protected String[] Inf;
        protected int tmp_Raise_money = 0;        //暫存下注金額
        protected int Need_money = 0;                 //玩家須下注金額
        protected int max_leangh = 0;                 //debug

        protected Image[] enemy = new Image[12];
        protected Label[] enemy_Raise = new Label[6];
        protected Label[] enemy_Name = new Label[6];

        protected String Package_tmp = null;                        //假如收到封包不正確 將會以此資料重傳

        protected int total_Player = 0;                //總玩家人數

        public int Sec = 15;                                        //計時器時間
        DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) }; //計時器
        //Storyboard timer = new Storyboard() { Duration = TimeSpan.FromSeconds(1) };

        public String Account = "";                             //玩家帳號
        public String PassWord = "";                           //玩家密碼

        public String[] enemy_name = new string[6];                           //用於顯示 其他玩家帳號 及 金錢
        public int enemy_location;
        public int[] enemy_money = new int[6];
        public StringBuilder Winner_Inf = new StringBuilder();



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

            while (Player_Statue)
            {
                String a = null;
                byte[] data = new byte[1024];
                a = Package_Rev();

                //MessageBox.Show(a);

                //////防止封包接收不完整
                if (!a.Equals("WaitAnswer end"))
                {
                    String[] b = a.Split(' ');
                    Update_Inf(b[0], a);                                                       //更新資訊 及 處理                              
                    arEvent.Set();
                }
            }

        }

        public void Update_Inf(String title, String a)
        {
            Package_Process pp = new Package_Process(a, client);             //處理資訊
            switch (title)
            {
                case "Winner":
                    Winner_Inf = pp.Winner_Inf;
                    break;

                case "Call_Inf":
                    Raise_money = pp.Raise_money;
                    Who_Raise = pp.Who_Raise;
                    enemy_money[Who_Raise] = pp.enemy_money[Who_Raise];
                    break;

                case "Raise_Inf":
                    Raise_money = pp.Raise_money;
                    Who_Raise = pp.Who_Raise;
                    enemy_money[Who_Raise] = pp.enemy_money[Who_Raise];
                    break;

                case "Blind_Inf":
                    Raise_money = pp.Raise_money;
                    Who_Raise = pp.Who_Raise;
                    enemy_money[Who_Raise] = pp.enemy_money[Who_Raise];
                    break;

                case "Enemy_Name":
                    enemy_location = pp.enemy_loaction;
                    enemy_name[enemy_location] = pp.enemy_name[enemy_location];
                    //MessageBox.Show(enemy_location.ToString());
                    enemy_money[enemy_location] = pp.enemy_money[enemy_location];
                    break;

                case "Total_Money":
                    total_money = pp.total_money;
                    break;

                case "Resend":
                    Resend_package rp = new Resend_package(client, Package_tmp);
                    break;

                case "Enemy_Inf":
                    total_Player = pp.total_Player;
                    break;

                case "Player_Card":
                    Player_number = pp.Player_number;
                    Player_Inf[1] = pp.Player_Inf[1];
                    Player_Inf[2] = pp.Player_Inf[2];
                    break;

                case "Money_Inf":
                case "New_Round":
                    Player_money = pp.Player_money;
                    total_money = 0;
                    Sec = 15;
                    break;

                case "Small_Blind":
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
                    Sec = 15;
                    break;

                case "GameRound2":
                case "GameRound3":
                case "GameRound4":
                    Game_State = pp.Game_State;
                    Raise_money = pp.Raise_money;
                    Need_money = Raise_money - tmp_Raise_money;
                    Sec = 15;
                    break;

                case "Win":
                    Player_money = pp.Player_money;
                    break;

                case "Tie":
                    Player_money = pp.Player_money;
                    break;

                case "Lose":
                    break;

                case "Public_Card_1-3":
                    Public_Card[0] = pp.Public_Card[0];
                    Public_Card[1] = pp.Public_Card[1];
                    Public_Card[2] = pp.Public_Card[2];
                    tmp_Raise_money = 0;
                    break;

                case "Public_Card_4":
                    Public_Card[3] = pp.Public_Card[3];
                    tmp_Raise_money = 0;
                    break;

                case "Public_Card_5":
                    Public_Card[4] = pp.Public_Card[4];
                    tmp_Raise_money = 0;
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
            data = Encoding.ASCII.GetBytes("OK end");
            client.Send(data);
        }

        public void Exit(String a)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(a + " end");
            client.Send(data);

            Package_Rev();
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

            enemy_Name[0] = Enemy_Name_label_1;
            enemy_Name[1] = Enemy_Name_label_2;
            enemy_Name[2] = Enemy_Name_label_3;
            enemy_Name[3] = Enemy_Name_label_4;
            enemy_Name[4] = Enemy_Name_label_5;
            enemy_Name[5] = Enemy_Name_label_6;

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

                                case "Winner":
                                    Winner_Box.Text = Winner_Inf.ToString();
                                    break;

                                case "Call_Inf":
                                case "Raise_Inf":
                                case "Blind_Inf":
                                    enemy_Raise[Who_Raise].Content = Raise_money;
                                    enemy_Raise[Who_Raise].Visibility = System.Windows.Visibility.Visible;
                                    //MessageBox.Show(enemy_money[Who_Raise].ToString());
                                    enemy_Name[Who_Raise].Content = enemy_name[Who_Raise] + ":" + enemy_money[Who_Raise];
                                    break;

                                case "Enemy_Hide":
                                    Button_Show("Enemy_label");
                                    break;

                                case "Total_Money":
                                    Total_money_Label.Content = total_money;
                                    break;

                                case "Enemy_Name":
                                    enemy_Name[enemy_location].Content = enemy_name[enemy_location] + ":" + enemy_money[enemy_location].ToString();
                                    enemy_Name[enemy_location].Visibility = System.Windows.Visibility.Visible;
                                    break;

                                case "Enemy_Inf":
                                    for (int i = 0; i < total_Player * 2; i += 2)
                                    {
                                        if (Player_number * 2 != i)
                                        {
                                            enemy[i].Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\6655.jpg"));
                                            enemy[i].Visibility = System.Windows.Visibility.Visible;
                                            enemy[i + 1].Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\6655.jpg"));
                                            enemy[i + 1].Visibility = System.Windows.Visibility.Visible;
                                        }
                                    }
                                    break;

                                case "Money_Inf":
                                case "Small_Blind":
                                case "Big_Blind":
                                case "Normal":
                                case "Win":
                                    Money_Label.Content = Player_money;
                                    break;

                                case "Tie":
                                    Clean_Data();
                                    Button_Show("Image_Hide");
                                    Button_Show("Hide");
                                    break;

                                case "Lose":
                                    Clean_Data();
                                    Button_Show("Image_Hide");
                                    Button_Show("Hide");
                                    break;

                                case "Player_Card":
                                    Server_Inf.Content = "Connect";
                                    Connect_btn.Visibility = System.Windows.Visibility.Hidden;
                                    enemy[Player_number * 2].Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Player_Inf[1] + @".GIF"));
                                    enemy[Player_number * 2].Visibility = System.Windows.Visibility.Visible;
                                    enemy[Player_number * 2 + 1].Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Player_Inf[2] + @".GIF"));
                                    enemy[Player_number * 2 + 1].Visibility = System.Windows.Visibility.Visible;
                                    Player_Number_Label.Content = Player_number;
                                    break;

                                case "GameRound1":
                                    Raise_Block.Text = (Need_money > 0) ? (Raise_money * 2).ToString() : "100";
                                    Raise_Button.Content = "Raise to" + Raise_Block.Text;
                                    Need_money = Raise_money - tmp_Raise_money;

                                    Call_Button.Content = (Need_money > 0) ? "Call" + Need_money : "Check";

                                    Timer_label.Foreground = new SolidColorBrush(Colors.Black);
                                    Timer_label.Content = Sec.ToString();

                                    Button_Show("Show");
                                    break;

                                case "GameRound2":
                                case "GameRound3":
                                case "GameRound4":
                                    if ((Raise_money - tmp_Raise_money) < Player_money)
                                        Call_Button.Content = (Raise_money != 0) ? "Call " + (Raise_money - tmp_Raise_money) : "Check";
                                    else
                                        Call_Button.Content = Player_money;
                                    Raise_Block.Text = (Raise_money != 0) ? (Raise_money * 2).ToString() : "100";
                                    Raise_Button.Content = "Raise to" + Raise_Block.Text;

                                    Timer_label.Foreground = new SolidColorBrush(Colors.Black);
                                    Timer_label.Content = Sec.ToString();

                                    Button_Show("Show");
                                    break;

                                case "Public_Card_1-3":
                                    Public_card_image1.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Public_Card[0] + ".GIF"));
                                    Public_card_image1.Visibility = System.Windows.Visibility.Visible;
                                    Public_card_image2.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Public_Card[1] + ".GIF"));
                                    Public_card_image2.Visibility = System.Windows.Visibility.Visible;
                                    Public_card_image3.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Public_Card[2] + ".GIF"));
                                    Public_card_image3.Visibility = System.Windows.Visibility.Visible;
                                    break;

                                case "Public_Card_4":
                                    Public_card_image4.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Public_Card[3] + ".GIF"));
                                    Public_card_image4.Visibility = System.Windows.Visibility.Visible;
                                    break;

                                case "Public_Card_5":
                                    Public_card_image5.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\Poker_Image\" + Public_Card[4] + ".GIF"));
                                    Public_card_image5.Visibility = System.Windows.Visibility.Visible;
                                    break;

                                case "New_Round":
                                    Money_Label.Content = Player_money;
                                    Total_money_Label.Content = total_money;
                                    Button_Show("Image_Hide");
                                    Button_Show("Enemy_label");
                                    Clean_Data();                                  
                                    break;
                            }
                        });
        }

        public void Button_Show(String a)
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
               {
                   if (a.Equals("Hide"))
                   {

                       Fold_Button.Visibility = System.Windows.Visibility.Hidden;
                       Call_Button.Visibility = System.Windows.Visibility.Hidden;
                       Raise_Button.Visibility = System.Windows.Visibility.Hidden;
                       Timer_label.Visibility = System.Windows.Visibility.Hidden;
                   }
                   else if (a.Equals("Image_Hide"))
                   {
                       Public_card_image1.Visibility = System.Windows.Visibility.Hidden;
                       Public_card_image2.Visibility = System.Windows.Visibility.Hidden;
                       Public_card_image3.Visibility = System.Windows.Visibility.Hidden;
                       Public_card_image4.Visibility = System.Windows.Visibility.Hidden;
                       Public_card_image5.Visibility = System.Windows.Visibility.Hidden;
                       Fold_Button.Visibility = System.Windows.Visibility.Hidden;
                       Call_Button.Visibility = System.Windows.Visibility.Hidden;
                       Raise_Button.Visibility = System.Windows.Visibility.Hidden;                       
                       for (int i = 0; i < enemy.Length; i++)
                           enemy[i].Visibility = System.Windows.Visibility.Hidden;
                   }
                   else if (a.Equals("Enemy_Name"))
                   {
                       for (int i = 0; i < enemy_Raise.Length; i++)
                           enemy_Name[i].Visibility = System.Windows.Visibility.Hidden;
                   }
                   else if (a.Equals("Enemy_label"))
                   {
                       for (int i = 0; i < enemy_Raise.Length; i++)
                           enemy_Raise[i].Visibility = System.Windows.Visibility.Hidden;
                   }

                   else
                   {
                       Fold_Button.Visibility = System.Windows.Visibility.Visible;
                       Call_Button.Visibility = System.Windows.Visibility.Visible;
                       Raise_Button.Visibility = System.Windows.Visibility.Visible;
                       Timer_label.Visibility = System.Windows.Visibility.Visible;
                       timer.Start();
                   }
               });
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
            Need_money = 0;
            Game_State = null;                  //遊戲狀態第幾回合
            tmp_Raise_money = 0;        //暫存下注金額
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dynamic_Test();
            Button_Show("Hide");
            Button_Show("Enemy_label");
            Button_Show("Enemy_Name");
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

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

            Package_tmp = "Call" + " " + Need_money + " " + Player_money;

            Send_State = Encoding.ASCII.GetBytes("Call" + " " + Need_money + " " + Player_money + " end");
            client.Send(Send_State);
            timer.Stop();
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
            if (int.Parse(Raise_Block.Text) < Player_money)
            {
                Raise_money = int.Parse(Raise_Block.Text);
                Need_money = Raise_money - tmp_Raise_money;
            }
            else
            {
                Need_money = Player_money;
                Raise_money = Player_money;
            }
            Player_money -= Need_money;
            //total_money += Need_money;
            tmp_Raise_money = Raise_money;
            //Call_money_Label.Content = tmp_Raise_money;
            Show_UI("Money_Inf");
            byte[] Send_State = new byte[2048];

            Package_tmp = "Raise " + Raise_money.ToString() + " " + Need_money.ToString() + " " + Player_money.ToString() + " ";

            Send_State = Encoding.ASCII.GetBytes("Raise " + Raise_money.ToString() + " " + Need_money.ToString() + " " + Player_money.ToString() + " end");
            client.Send(Send_State);
            timer.Stop();
            Button_Show("Hide");
            arEvent.Set();
        }

        private void Fold_Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] Send_State = new byte[1024];

            Package_tmp = "Fold" + " " + Raise_money.ToString() + " " + Player_money + " " + Player_number;
            Send_State = Encoding.ASCII.GetBytes("Fold" + " " + Raise_money.ToString() + " " + Player_money + " " + Player_number + " end");

            Image_card_1.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\6655.jpg"));
            Image_card_2.Source = new BitmapImage(new Uri(@Directory.GetCurrentDirectory() + @"\6655.jpg"));
            client.Send(Send_State);
            Button_Show("Hide");
            timer.Stop();
            arEvent.Set();
        }

        private void Connect_btn_Click(object sender, RoutedEventArgs e)
        {
            Thread tr = new Thread(play_round);
                try
                {
                    //Dynamic_Test();
                    Button_Show("Enemy_label");
                    Button_Show("Hide");
                    while (true)
                    {
                        Log_UI log = new Log_UI();
                        log.ShowDialog();
                        Account = log.Account;
                        PassWord = log.PassWord;
                        //////讀取帳號密碼

                        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        client.Connect(new IPEndPoint(IPAddress.Parse(Ip), Port));

                        //////完成連線

                        byte[] data = new byte[1024];
                        data = Encoding.ASCII.GetBytes(log.States + " " + Account + " " + PassWord + " end");
                        Console.WriteLine(Encoding.ASCII.GetString(data));
                        client.Send(data);
                        Array.Clear(data, 0, data.Length);

                        int rev = client.Receive(data);
                        StringBuilder sb = new StringBuilder();
                        String a = Encoding.ASCII.GetString(data, 0, rev);
                        sb.Append(a);
                        String[] b = a.Split(' ');
                        /////防止封包接收不完整
                        while (!b[b.Length - 1].Equals("end"))
                        {
                            data = new byte[1024];
                            rev = client.Receive(data);
                            a = Encoding.ASCII.GetString(data, 0, rev);
                            sb.Append(a);
                            b = sb.ToString().Split(' ');
                        }

                        //MessageBox.Show(sb.ToString());

                        if (b[0].Equals("Login_Sucess"))
                        {
                            Player_money = int.Parse(b[1]);
                            MessageBox.Show("Login Sucess");
                            Back(client);
                            break;
                        }
                        else if (b[0].Equals("Register_Sucess"))
                        {
                            MessageBox.Show("Register Sucess");
                            Back(client);
                        }
                        else
                        {
                            MessageBox.Show("log Fail");
                            Back(client);
                        }
                    }//end while


                    Server_Inf.Content = "Connect";
                    Connect_btn.Content = "DisConnect";

                    Player_Statue = true;

                    tr.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
        }

        public void Back(Socket client)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("OK end");
            client.Send(data);
        }

        /// <summary>
        /// 判斷封包是否接收完整
        /// </summary>
        /// <returns></returns>
        public String Package_Rev()
        {
            int rev = client.Receive(data);
            StringBuilder sb = new StringBuilder();
            String a = Encoding.ASCII.GetString(data, 0, rev);
            sb.Append(a);
            String[] b = a.Split(' ');
            /////防止封包接收不完整
            while (!b[b.Length - 1].Equals("end"))
            {
                data = new byte[1024];
                rev = client.Receive(data);
                a = Encoding.ASCII.GetString(data, 0, rev);
                sb.Append(a);
                b = sb.ToString().Split(' ');
            }
            return sb.ToString();
        }

        /// <summary>
        /// 計時器 時間到制動Fold
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Timer_Run(object sender, EventArgs e)
        {
            if (Sec > 0)
            {
                Sec--;
                Timer_label.Content = Sec.ToString();
                if (Sec < 6)
                    Timer_label.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                timer.Stop();
                Fold_Button_Click(new object(), new RoutedEventArgs());
            }
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(Raise_Block.Text) > 100)
                Raise_Block.Text = (int.Parse(Raise_Block.Text) - 100).ToString();
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            Raise_Block.Text = (int.Parse(Raise_Block.Text) + 100).ToString();
        }


    }
}
