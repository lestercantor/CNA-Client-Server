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
using Packets;

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
            try
            {
                Packet receivedPacket;
                while ((receivedPacket = clients[index].Read()) != null)
                {
                    switch (receivedPacket.packetType)
                    {
                        case PacketType.CHAT_MESSAGE:
                            ChatMessagePacket chatPacket = (ChatMessagePacket)receivedPacket;
                            clients[index].Send(chatPacket);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }

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
