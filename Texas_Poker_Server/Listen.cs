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
                Console.WriteLine("waiting");
                //sClient[sclient_idx] = ss.Accept();


                sClient[location] = ss.Accept();
                sit[location] = 1;
                location = site();

                connect_ppl++;
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
