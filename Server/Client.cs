using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using Packets;

namespace Server
{
    class Client
    {
        private Socket _socket;
        private NetworkStream _stream;
        private BinaryReader _reader;
        private BinaryWriter _writer;
        private BinaryFormatter _formatter;

        private object _readLock;
        private object _writeLock;

        public Client(Socket socket)
        {
            _writeLock = new object();
            _readLock = new object();
            _socket = socket;

            _formatter = new BinaryFormatter();

            _stream = new NetworkStream(socket);
            _reader = new BinaryReader(_stream, Encoding.UTF8);
            _writer = new BinaryWriter(_stream, Encoding.UTF8);
        }
        public void Close()
        {
            _stream.Close();
            _reader.Close();
            _writer.Close();
            _socket.Close();
        }
        public Packet Read()
        {
            lock (_readLock)
            {
                //return _reader.ReadLine();

                int numOfBytes;
                if ((numOfBytes = _reader.ReadInt32()) != -1)
                {
                    byte[] buffer = _reader.ReadBytes(numOfBytes);
                    MemoryStream memStream = new MemoryStream(buffer);
                    return _formatter.Deserialize(memStream) as Packet;
                }
                else
                {
                    return null;
                }

            }
        }
        public void Send(Packet message)
        {
            lock (_writeLock)
            {
                //_writer.WriteLine(message);
                //_writer.Flush();

                MemoryStream memStream = new MemoryStream();

                _formatter.Serialize(memStream, message);
                byte[] buffer = memStream.GetBuffer();
                _writer.Write(buffer.Length);
                _writer.Write(buffer);
                _writer.Flush();
            }
        }
    }
}
