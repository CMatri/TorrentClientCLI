using BencodeNET.Objects;
using BencodeNET.Parsing;
using BencodeNET.Torrents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using TorrentClientCLI.Tracker;

namespace TorrentClientCLI
{
    class ActiveTorrent
    {
        public Torrent torrent { get; }
        public BencodeParser parser { get; }
        public int port { get; }
        public int numPieces { get; }
        public string peerID { get; }
        private ConnectionManager connectionManager;
        private TrackerManager trackerManager;
        private long pieceSize;
        private byte[] currConnectionID; 
        protected readonly byte[] initialConnectionID = { 0x05, 0x1D, 0xE1, 0x76, 0x67, 0xAA, 0xF0, 0xFF };

        public ActiveTorrent(string fileName)
        {
            parser = new BencodeParser();
            torrent = parser.Parse<Torrent>(fileName);
            currConnectionID = initialConnectionID;
            numPieces = torrent.NumberOfPieces;
            pieceSize = torrent.PieceSize;
            peerID = RandomID();
            connectionManager = new ConnectionManager(this);
            trackerManager = new TrackerManager(this);
            port = 6881;
        }

        public void StartDownload()
        {
            List<string> trackers = new List<string>();
            var keys = torrent.ToBDictionary();
            var announce = keys["announce"];
            var announceList = (BList)keys["announce-list"];
            if (announceList == null && announce != null)
                connectionManager.Add(trackerManager.RequestPeersFromTracker(announce.ToString()));
            else if (announce != null)
                connectionManager.Add(trackerManager.RequestPeersFromTrackers(announceList));
            else
                Console.WriteLine("Failed to download - invalid tracker data.");
        }      

        private string RandomID()
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrsABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 20)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
