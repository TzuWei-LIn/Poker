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
using System.Windows.Shapes;

namespace Poker_Server_Client
{
    /// <summary>
    /// Log_UI.xaml 的互動邏輯
    /// </summary>
    public partial class Log_UI : Window
    {
        public Log_UI()
        {
            InitializeComponent();
        }

        public String Account = "";
        public String PassWord = "";
        public String States = "";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Account = Account_Box.Text;
            PassWord = Password_Box.Text;
            States = "Register";
            this.Close();
        }

        private void Login_Btm_Click(object sender, RoutedEventArgs e)
        {
            Account = Account_Box.Text;
            PassWord = Password_Box.Text;
            States = "Login";
            this.Close();
        }
    }
}
