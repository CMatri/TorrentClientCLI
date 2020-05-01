using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClientCLI
{
    class Peer
    {
        public string address { get; }
        public int port;
        public TcpClient handle;
        public bool choked;
        private byte[] pieces;

        public Peer(string address, int port, int bitmapSize)
        {
            this.address = address;
            this.port = port;
            this.choked = true;
            this.pieces = new byte[bitmapSize];
        }

        public void SetHandle(TcpClient handle) { this.handle = handle; }

        public void SetPiece(int i) { pieces[i / 8] |= (byte) (1 << (7 - (i % 8))); }

        public bool HasPiece(int i) { return (pieces[i / 8] >> (7 - (i % 8)) & 0x1) != 0; }

        public void NextPacket()
        {
            NetworkStream stream = handle.GetStream();
            byte[] lenBuf = new byte[4];
            stream.Read(lenBuf, 0, 4);
            int len = BitConverter.ToInt32(lenBuf, 0);
            byte[] data = new byte[len];
            stream.Read(data, 0, len);
            if (len == 0) return; // keep-alive packet
            

            switch ((MessagePacket.MessageType) data[0])
            {
                case MessagePacket.MessageType.Choke: break;
                case MessagePacket.MessageType.Unchoke: break;
                case MessagePacket.MessageType.Interested: break;
                case MessagePacket.MessageType.Uninterested: break;
                case MessagePacket.MessageType.Have:
                    SetPiece(BitConverter.ToInt32(data, 0));
                    Console.WriteLine("Has idx " + BitConverter.ToInt32(data, 0));
                    break;
                case MessagePacket.MessageType.Bitfield:
                    for(int i = 0; i < pieces.Length; i++)
                    {
                        Array.Copy(data, pieces, pieces.Length);
                    }
                    break;
                case MessagePacket.MessageType.Request: break;
                case MessagePacket.MessageType.Piece: break;
                case MessagePacket.MessageType.Cancel: break;
            }
        }
    }
}
