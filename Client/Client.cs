using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Packets;

namespace Client
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        //private StreamWriter writer;
        //private StreamReader reader;

        private BinaryFormatter _formatter;
        private BinaryReader _reader;
        private BinaryWriter _writer;

        private ClientForm clientForm;

        public Client()
        {
            tcpClient = new TcpClient();
            _formatter = new BinaryFormatter();
        }
        public bool Connect(string ipAddress, int port)
        {
            try
            {
                tcpClient.Connect(ipAddress, port);
                stream = tcpClient.GetStream();

                _reader = new BinaryReader(stream, Encoding.UTF8);
                _writer = new BinaryWriter(stream, Encoding.UTF8);

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

            if (!tcpClient.Connected)
            {
                tcpClient.Close();
                thread.Abort();
            }
                //tcpClient.Close();

        }
        private void ProcessServerResponse()
        {
            int numOfBytes;
            while ((numOfBytes = _reader.ReadInt32()) != 0)
            {
                byte[] buffer = _reader.ReadBytes(numOfBytes);
                MemoryStream memStream = new MemoryStream(buffer);

                Console.WriteLine("Server says: " + _formatter.Deserialize(memStream));
                Console.WriteLine();
            }
        }
        public void SendMessage(Packet message)
        {
            MemoryStream memStream = new MemoryStream();

            _formatter.Serialize(memStream, message);
            byte[] buffer = memStream.GetBuffer();

            _writer.Write(buffer.Length);
            _writer.Write(buffer);
            _writer.Flush();
        }
    }
}
