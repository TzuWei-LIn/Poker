using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Texas_Poker_Server
{
    class Account_Process : Server
    {

        List<String> Account_Inf = new List<string>();

        public Account_Process()
        {
            Account_Inf.Clear();
            String Filename = @Directory.GetCurrentDirectory() + @"\Account.txt";
            FileStream fs = new FileStream(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //fs.Seek(0, SeekOrigin.End);
            using (StreamReader sr = new StreamReader(fs))
            {
                while (!sr.EndOfStream)
                {
                    String[] b = sr.ReadLine().Split(' ');
                    foreach (String o in b)
                    {
                        Console.WriteLine("Data = {0}", o);
                        Account_Inf.Add(o);
                    }
                }
                sr.Close();
            }

            fs.Close();
        }
        public String AccountProcess(String a)
        {
            Console.WriteLine("Rev = {0}", a);
            String[] b = a.Split(' ');
            String title = b[0];
            switch (title)
            {
                case "Login":
                    for (int i = 0; i < Account_Inf.Count; i += 3)
                    {
                        if (b[1].Equals(Account_Inf[i]))
                            if (b[2].Equals(Account_Inf[i + 1]))
                                return "Login_Sucess" + " " + Account_Inf[i + 2];
                    }
                    return "Login_Fail";
                case "Register":
                    String Filename = @Directory.GetCurrentDirectory() + @"\Account.txt";
                    FileStream fs = new FileStream(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Seek(0, SeekOrigin.End);
                    byte[] data = new byte[1024];

                    data = Encoding.ASCII.GetBytes(b[1] + " " + b[2] + " " + 3000 + " end");
                    fs.Write(data, 0, data.Length);
                    fs.Close();
                    return "Register_Sucess";
            }
            return "Login";
        }

        public void UpdateAccount(int location)
        {
            String[] tmp = new string[3];
            int count = 0;
            String Filename = @Directory.GetCurrentDirectory() + @"\Account.txt";
            FileStream fs = new FileStream(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //fs.Seek(0, SeekOrigin.End);
            using (StreamReader sr = new StreamReader(fs))
            {
                while (!sr.EndOfStream)
                {
                    String[] b = sr.ReadLine().Split(' ');
                    if (b[0].Equals(AC_money[location]))
                    {
                        tmp[0] = b[0];
                        tmp[1] = b[1];
                        tmp[2] = b[2];
                        break;
                    }
                    count++;
                }
                sr.Close();
            }
            String[] data = File.ReadAllLines(Filename);
            for (int i = 0; i < data.Length;i++ )
            {
                string[] b = data[i].Split(' ');
                if (b[0].Equals(tmp[0]) && b[1].Equals(tmp[1]) && b[2].Equals(tmp[2]))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(AC_money[location] + " " + tmp[1] + " " + Player_money[location].ToString());
                    data[i] = sb.ToString();
                    Console.WriteLine("update sucess");
                }
            }
            File.WriteAllLines(Filename, data);
            fs.Close();

        }
    }
}
