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
        private ConcurrentDictionary<int, Client> clients;
        public Server(string ipAddress, int port)
        {
            tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
        }

        public void Start()
        {
            clients = new ConcurrentDictionary<int, Client>();
            int clientIndex = 0;
            tcpListener.Start();

            while(true)
            {
                int index = clientIndex;
                clientIndex++;

                Socket socket = tcpListener.AcceptSocket();

                Client client = new Client(socket);
                clients.TryAdd(index, client);

                Thread thread = new Thread(() => { ClientMethod(index); });
                thread.Start();
            }


        }

        public void Stop()
        {
            tcpListener.Stop();
        }
        private void ClientMethod(int index)
        {
            string receivedMessage;

            clients[index].Send("You have connected to the server");

            while ((receivedMessage = clients[index].Read()) != null)
            {
                if(receivedMessage.ToLower() == "bye")
                {
                    clients[index].Send("Goodbye");
                    break;
                }
                else
                {
                    clients[index].Send(GetReturnMessage(receivedMessage));
                }
            };

            clients[index].Close();
            Client c;
            clients.TryRemove(index, out c);
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
