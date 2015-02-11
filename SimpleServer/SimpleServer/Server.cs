using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace SimpleServer
{
    public class Server
    {
        private TcpListener _tcpListener;
        private Thread _listenThread;

        public Server()
        {
            this._tcpListener = new TcpListener(IPAddress.Any, 9097);
            this._listenThread = new Thread(new ThreadStart(ListenForClients));
            this._listenThread.Start();
        }

        private void ListenForClients()
        {
            this._tcpListener.Start();

            while(true)
            {
                // blocks until a client has connected to the server
                TcpClient client = this._tcpListener.AcceptTcpClient();

                // create a thread to handle communication with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while(true)
            {
                bytesRead = 0;

                try
                {
                    // blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch(Exception ex)
                {
                    // a socket error has occurred
                    Console.WriteLine("ERROR: {0}\n{1}", ex.Message, ex.ToString());
                    break;
                }

                if(bytesRead == 0)
                {
                    // the client has disconnected from the server
                    break;
                }

                // message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();
                Console.WriteLine("RCV MSG:{0}", encoder.GetString(message, 0, bytesRead));

                byte[] buffer = encoder.GetBytes("Hello Client!");

                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
            }

            tcpClient.Close();
        }
    }
}
