using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static TorrentClientCLI.ActiveTorrent;

namespace TorrentClientCLI
{
    class MessagePacket
    {
        public enum MessageType
        {
            Choke = 0,
            Unchoke = 1,
            Interested = 2,
            Uninterested = 3,
            Have = 4,
            Bitfield = 5,
            Request = 6,
            Piece = 7,
            Cancel = 8
        }

        public MessageType Type { get; }
        public byte[] Data { get; }

        public MessagePacket(MessageType type, byte[] data)
        {
            this.Type = type;
            this.Data = data;
        }

        public byte[] Serialize()
        {
            int len = Data.Length + 1;
            byte[] ret = new byte[len];
            Array.Copy(BitConverter.GetBytes(len), 0, ret, 0, 4);
            ret[4] = (byte) Type;
            Array.Copy(Data, 0, ret, 5, Data.Length);
            return ret;
        }

        public static MessagePacket Read(Peer peer)
        {
            NetworkStream stream = peer.handle.GetStream();
            byte[] lenBuf = new byte[4];
            stream.Read(lenBuf, 0, 4);
            int len = BitConverter.ToInt32(lenBuf, 0);
            byte[] data = new byte[len];
            stream.Read(data, 0, len);
            return new MessagePacket((MessageType) data[0], data.Skip(1).ToArray());
        }
    }
}
