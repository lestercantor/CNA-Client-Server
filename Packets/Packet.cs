using System;

namespace Packets
{
    public enum PacketType
    {
        CHAT_MESSAGE = 0,
        PRIVATE_MESSAGE,
        CLIENT_NAME,
    };
    [Serializable]
    public class Packet
    {
        public PacketType packetType { get; protected set; }
    }
    [Serializable]
    public class ChatMessagePacket : Packet
    {
        public string _message;
        public ChatMessagePacket(string message)
        {
            _message = message;
            packetType = PacketType.CHAT_MESSAGE;
        }
    }
}
