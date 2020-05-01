using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClientCLI
{
    class HandshakePacket
    {
        private int pStrLen, len;
        private string pStr;
        private byte[] infoHash, peerId;

        public HandshakePacket(string pStr, byte[] infoHash, byte[] peerId)
        {
            this.pStrLen = pStr.Length;
            this.pStr = pStr;
            this.infoHash = infoHash;
            this.peerId = peerId;
            this.len = 49 + pStr.Length;
        }

        public byte[] Serialize()
        {
            byte[] handshake = new byte[len];
            handshake[0] = (byte)pStr.Length;
            Array.Copy(Encoding.ASCII.GetBytes(pStr), 0, handshake, 1, pStr.Length);
            Array.Copy(infoHash, 0, handshake, pStr.Length + 9, infoHash.Length); // 9 = 8 reserve bytes + 1 pStr len byte
            Array.Copy(peerId, 0, handshake, pStr.Length + 9 + infoHash.Length, peerId.Length);
            return handshake;
        }

        public static HandshakePacket Read(byte[] data)
        {
            int pStrLen = data[0];
            string pStr = Encoding.ASCII.GetString(data.Skip(1).Take(pStrLen).ToArray());
            byte[] infoHash = data.Skip(pStrLen + 9).Take(20).ToArray();
            byte[] peerId = data.Skip(pStrLen + 30).Take(20).ToArray();
            return new HandshakePacket(pStr, infoHash, peerId);
        }

        public int Length()
        {
            return len;
        }

        public byte[] InfoHash()
        {
            return infoHash;
        }
    }
}
