using BencodeNET.Parsing;
using BencodeNET.Torrents;
using System;
using System.Collections.Generic;

namespace TorrentClientCLI
{
    class Program
    {
        private string fileName;
        private ActiveTorrent activeTorrent;

        public Program()
        {
            fileName = "C:\\Users\\Connor\\Downloads\\bunny.torrent";
            activeTorrent = new ActiveTorrent(fileName);
            activeTorrent.StartDownload();
            Console.ReadLine();
        }

        

        static void Main(string[] args)
        {
            new Program();            
        }
    }
}
