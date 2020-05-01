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
        protected int port { get; }
        protected ActiveTorrent torrent{ get; }

        public Tracker(string addr, ActiveTorrent torrent)
        {
            string[] t0 = addr.Split(':');
            this.addr = addr;

            if (addr.StartsWith("udp"))
            {
                string tempPort = "";
                if (t0.Length == 2)
                {
                    this.addr = t0[0];
                    tempPort = t0[1];
                }
                else if (t0.Length == 3)
                {
                    this.addr = t0[0] + ":" + t0[1];
                    tempPort = t0[2];
                }
                if (tempPort.Contains('/'))
                {
                    string[] t1 = tempPort.Split("/".ToArray(), 2);
                    this.port = Convert.ToInt32(t1[0]);
                    this.addr += "/" + t1[1];
                }
                else this.port = Convert.ToInt32(tempPort);
            }

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
