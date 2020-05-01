using BencodeNET.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorrentClientCLI.Tracker.Http;
using TorrentClientCLI.Tracker.Udp;

namespace TorrentClientCLI.Tracker
{
    class TrackerManager
    {
        private List<Tracker> trackers { get; }
        private ActiveTorrent torrent;

        public TrackerManager(ActiveTorrent torrent)
        {
            this.torrent = torrent;
            this.trackers = new List<Tracker>();
        }

        public List<Peer> RequestPeersFromTracker(string addr)
        {
            Tracker tracker;

            if (addr.StartsWith("udp")) tracker = new UdpTracker(addr, torrent);
            else if (addr.StartsWith("http")) tracker = new HttpTracker(addr, torrent);
            else { Console.WriteLine("Unimplemented tracker protocol: " + addr); return null; }

            if (tracker.Handshake())
            {
                Console.WriteLine("Handshake ok with " + addr);
                trackers.Add(tracker);
            } 
            else Console.WriteLine("Handshake failed with " + addr);

            return tracker.Scrape();
        }

        public List<Peer> RequestPeersFromTrackers(BList trackerList)
        {
            List<Peer> peers = new List<Peer>();
            foreach (BList tracker_strs in trackerList)
            {
                foreach (BString tracker_str in tracker_strs)
                {
                    List<Peer> newPeers = RequestPeersFromTracker(tracker_str.ToString());
                    if (newPeers != null) peers.AddRange(newPeers);
                }
            }
            return peers.Count > 0 ? peers : null;
        }

        private List<Peer> ParsePeerList(byte[] data)
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
