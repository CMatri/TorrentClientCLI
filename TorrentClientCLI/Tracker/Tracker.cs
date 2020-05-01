using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClientCLI.Tracker
{
    abstract class Tracker
    {
        protected bool isHandshaken { get; set; }
        protected string addr { get; }
        protected ActiveTorrent torrent{ get; }

        public Tracker(string addr, ActiveTorrent torrent)
        {
            this.addr = addr;
            this.isHandshaken = false;
            this.torrent = torrent;
        }

        public abstract bool Handshake();
        public abstract List<Peer> Scrape();

        protected List<Peer> ParsePeerList(byte[] data)
        {
            List<Peer> ret = new List<Peer>();
            for (int i = 0; i + 6 < data.Length;)
            {
                Peer p = new Peer(data[i++] + "." + data[i++] + "." + data[i++] + "." + data[i++], data[i++] * 256 + data[i], torrent.numPieces / 8);
                ret.Add(p);
            }
            return ret;
        }
    }
}
