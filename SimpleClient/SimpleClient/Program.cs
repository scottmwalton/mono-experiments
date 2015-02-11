using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SimpleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();
			try
			{
				IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9097);
				//IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("172.16.1.4"), 9097);

	            client.Connect(serverEndPoint);

	            NetworkStream clientStream = client.GetStream();

	            ASCIIEncoding encoder = new ASCIIEncoding();
	            byte[] buffer = encoder.GetBytes("Hello Server!");
	            byte[] message = new byte[4096];
	            int bytesRead = 0;
				int i = 0;
				while(i<10)
				{
		            clientStream.Write(buffer, 0, buffer.Length);
		            clientStream.Flush();
		            try
		            {
		                //blocks until a client sends a message
		                bytesRead = clientStream.Read(message, 0, 4096);
		            }
		            catch(Exception ex)
		            {
		                //a socket error has occurred
		                Console.WriteLine("ERROR: {0}\n{1}", ex.Message, ex.ToString());
		                return;
		            }

		            if (bytesRead == 0)
		            {
		                //the server was disconnected
		                Console.WriteLine("Server disconnect. :(");
		            }

		            //message has successfully been received
		            Console.WriteLine(encoder.GetString(message, 0, bytesRead));
					i++;
					Thread.Sleep(5000);
				}
			}
			catch(Exception ex) 
			{
				Console.WriteLine ("ERROR: {0}\n{1}", ex.Message, ex.ToString ());
			}
			finally 
			{
				client.Close ();
			}
        }
    }
}
