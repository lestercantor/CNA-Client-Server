using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace Server
{
    class Server
    {
        private TcpListener tcpListener;
        private ConcurrentBag<Client> clients;
        public Server(string ipAddress, int port)
        {
            tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
        }

        public void Start()
        {
            clients = new ConcurrentBag<Client>();
            tcpListener.Start();

            while(true)
            {
                Socket localSocket = tcpListener.AcceptSocket();
                Console.WriteLine("Connection accepted");

                Client _client = new Client(localSocket);
                clients.Add(_client);
                Thread thread = new Thread(() => { ClientMethod(_client); });
                thread.Start();
            }


        }

        public void Stop()
        {
            tcpListener.Stop();
        }
        private void ClientMethod(Client client)
        {
            string receivedMessage;

            client.Send("You have connected to the server");

            while ((receivedMessage = client.Read()) != null)
            {
                if(receivedMessage.ToLower() == "bye")
                {
                    client.Send("Goodbye");
                    break;
                }
                else
                {
                    client.Send(GetReturnMessage(receivedMessage));
                }
            };

            clients.TryTake(out client);
        }
        private static string GetReturnMessage(string code)
        {
            string message;

            if (code.ToLower() == "hi")
            {
                message = "Hello";
            }
            else
            {
                message = "Invalid";
            }
            return message;
        }
    }
}
