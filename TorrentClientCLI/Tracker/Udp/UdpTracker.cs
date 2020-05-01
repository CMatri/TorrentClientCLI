using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClientCLI.Tracker.Udp
{
    class UdpTracker : Tracker
    {
        public UdpTracker(string addr, ActiveTorrent torrent) : base(addr, torrent)
        {

        }

        public override bool Handshake()
        {
            return false;
        }

        public override List<Peer> Scrape()
        {
            return null;
        }
    }
}
