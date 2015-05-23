using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Poker_Server_Client
{
    class Resend_package
    {
        public Resend_package(Socket client,String a)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(a);
            Console.WriteLine(Encoding.ASCII.GetString(data));
            client.Send(data);
        }
    }
}
