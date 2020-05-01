using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClientCLI.Tracker.Http
{
    class HttpTracker : Tracker
    {
        private byte[] responseData;

        public HttpTracker(string addr, ActiveTorrent torrent) : base(addr, torrent)
        {
        }

        public override bool Handshake()
        {
            string address = string.Format(this.addr + "/?info_hash={0}&peer_id={1}&port={2}&uploaded={3}&downloaded={4}&compact={5}&left={6}",
                BitConverter.ToString(torrent.torrent.GetInfoHashBytes()).Replace('-', '%').Insert(0, "%"),
                torrent.peerID,
                torrent.port,
                "0",
                "0",
                "1",
                torrent.torrent.TotalSize
            );

            HttpClient httpClient = new HttpClient();
            var result = httpClient.GetAsync(address).Result;

            if (result.IsSuccessStatusCode)
            {
                responseData = result.Content.ReadAsByteArrayAsync().Result;
                var res = torrent.parser.Parse<BencodeNET.Objects.BDictionary>(responseData);
                isHandshaken = true;
            } else
            {
                isHandshaken = false; ;
            }

            httpClient.Dispose();
            return isHandshaken;
        }

        public override List<Peer> Scrape()
        {
            if (!isHandshaken) return null;
            return ParsePeerList(responseData);
        }
    }
}
