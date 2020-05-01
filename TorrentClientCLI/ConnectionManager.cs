using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static TorrentClientCLI.ActiveTorrent;

namespace TorrentClientCLI
{
    class ConnectionManager
    {
        private List<Peer> peers;
        private ActiveTorrent torrent;

        public ConnectionManager(ActiveTorrent torrent)
        {
            this.peers = new List<Peer>();
            this.torrent = torrent;
        }

        public void Add(List<Peer> peers)
        {
            Console.WriteLine("Adding new peers.");
            for(int i = 0; i < peers.Count; i++)
            {
                Peer peer = peers[i];
                TcpClient client = PeerHandshake(peer);
                if (client != null)
                {
                    peer.SetHandle(client);
                    peer.NextPacket();
                    this.peers.Add(peer);
                    Console.WriteLine("Found valid peer");
                }
            }
            Console.WriteLine("Finished adding peers.");
        }

        public void Start()
        {
            
        }

        private TcpClient PeerHandshake(Peer peer)
        {
            try
            {
                TcpClient client = new TcpClient();
                Console.WriteLine("Polling " + peer.address + ":" + peer.port);
                if (client.ConnectAsync(peer.address, peer.port).Wait(300))
                {
                    NetworkStream stream = client.GetStream();
                    string pStr = "BitTorrent protocol";
                    HandshakePacket handshake = new HandshakePacket(pStr, torrent.torrent.GetInfoHashBytes(), Encoding.ASCII.GetBytes(torrent.peerID));
                    stream.Write(handshake.Serialize(), 0, handshake.Length());
                    byte[] data = new byte[handshake.Length()];
                    stream.Read(data, 0, data.Length);
                    HandshakePacket response = HandshakePacket.Read(data);
                    peer.handle = client;

                    return Encoding.ASCII.GetString(response.InfoHash()).Equals(Encoding.ASCII.GetString(torrent.torrent.GetInfoHashBytes())) ? client : null;
                }
            }
            catch (Exception e) { Console.WriteLine("Error connecting to peer: " + e); }

            return null;
        }

        ~ConnectionManager()
        {
            for(int i = 0; i < peers.Capacity; i++)
            {
                if (peers[i].handle != null) peers[i].handle.Close();
            }
        }
    }
}
