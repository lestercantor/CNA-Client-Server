using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Client
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;

        private ClientForm clientForm;

        public Client()
        {
            tcpClient = new TcpClient();
        }
        public bool Connect(string ipAddress, int port)
        {
            try
            {
                tcpClient.Connect(ipAddress, port);
                stream = tcpClient.GetStream();

                reader = new StreamReader(stream, Encoding.UTF8);
                writer = new StreamWriter(stream, Encoding.UTF8);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }
        public void Run()
        {
            if (!tcpClient.Connected)
            {
                throw new Exception();
            }

            //string userInput;
            //ProcessServerResponse();

            //while ((userInput = Console.ReadLine()) != null)
            //{
            //    writer.WriteLine(userInput);
            //    writer.Flush();
            //    ProcessServerResponse();

            //    if (userInput.ToLower() == "bye")
            //        break;
            //}

            clientForm = new ClientForm(this);

            Thread thread = new Thread(() => { ProcessServerResponse(); });
            thread.Start();

            clientForm.ShowDialog();

            tcpClient.Close();
        }
        private void ProcessServerResponse()
        {
            while (reader.ReadLine() != null)
            {
                Console.WriteLine("Server says: " + reader.ReadLine());
                Console.WriteLine();
            }
        }
        public void SendMessage(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }
    }
}
