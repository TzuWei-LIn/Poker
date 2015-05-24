using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texas_Poker_Server
{
    class Listen : Server
    {
        static int location = 0;
        public static void Listener()
        {
            while (true)
            {
                byte[] data = new byte[1024];
                Console.WriteLine("waiting");
                while (true)
                {
                    sClient[location] = ss.Accept();
                    int rev = sClient[location].Receive(data);

                    String a = Encoding.ASCII.GetString(data, 0, rev);

                    Account_Process ap = new Account_Process();
                    String answer = ap.AccountProcess(a);

                    String[] b = answer.Split(' ');
                    Console.WriteLine("Answer = {0}", answer);

                    byte[] sd = new byte[1024];
                    sd = Encoding.ASCII.GetBytes(answer +" end");
                    sClient[location].Send(sd);
                    sClient[location].Receive(sd);

                    if (!b[0].Equals("Login_Sucess"))
                        sClient[location].Close();
                    else
                    {
                        String[] c = a.Split(' ');
                        AC_money[location] = c[1];
                        Console.WriteLine("AC = {0}", AC_money[location]);
                        Player_money[location] = int.Parse(b[1]);
                        connect_ppl++;
                        break;
                    }
                }

                sit[location] = 1;
                location = site();

                if (connect_ppl == 2)
                {
                    Console.WriteLine("Start.........");
                    arEvent.Set();
                }
            }
        }

        public static int site()
        {
            for (int i = 0; i < sit.Length; i++)
            {
                Console.WriteLine("Debug = {0}", sit[i]);
                if (sit[i] == 0)
                    return i;
            }
            return 0;
        }
    }
}
