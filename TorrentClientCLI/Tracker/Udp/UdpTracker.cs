using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;

namespace TorrentClientCLI.Tracker.Udp
{
    class UdpTracker : Tracker
    {
        private const Int64 ProtocolId = 0x0000041727101980;
        private const Int32 ActionConnect = 0;
        private const Int32 ActionAnnounce = 1;
        private const Int32 ActionScrape = 2;
        private const Int32 ActionErrorCheck = 3;
        
        public UdpTracker(string addr, ActiveTorrent torrent) : base(addr, torrent)
        {

        }

        public override bool Handshake()
        {
            bool hasFailed = false;
            int curState = ActionConnect;
            int nextState = -1;
            int n = 0;
            uint receivedAction = 99;
            Int32 transactionId;
            UdpClient client;
            
            while (!hasFailed)
            {
                switch(curState)
                {
                    case ActionConnect:
                        Console.WriteLine("Attempting udp tracker connect " + addr);
                        try { client = new UdpClient("tracker.publicbt.com/announce", 80); }
                        catch(Exception)
                        {
                            Console.WriteLine("Location doesn't exist.");
                            hasFailed = true;
                            break;
                        }

                        transactionId = new Random().Next(0, 65535);
                        byte[] buf = Pack.Int64(ProtocolId, Pack.Endianness.Big).Concat(Pack.Int32(ActionConnect, Pack.Endianness.Big)).Concat(Pack.Int32(transactionId, Pack.Endianness.Big)).ToArray();

                        client.Send(buf, buf.Length);

                        var result = client.BeginReceive(null, null);
                        result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(15 * n ^ 2));
                        if (result.IsCompleted)
                        {
                            IPEndPoint endPoint = null;
                            buf = client.EndReceive(result, ref endPoint);

                            if (buf == null || buf.Length < 16)
                            {
                                nextState = ActionErrorCheck;
                            }
                            else
                            {
                                receivedAction = Unpack.UInt32(buf, 0, Unpack.Endianness.Big);
                                UInt32 receivedTransactionId = Unpack.UInt32(buf, 4, Unpack.Endianness.Big);
                                if (receivedAction != transactionId || receivedAction != 0)
                                {
                                    Console.WriteLine("PRogress?");
                                }
                            }
                        } else
                        {
                            nextState = ActionErrorCheck;
                        }
                        break;
                    case ActionAnnounce: break;
                    case ActionScrape: break;
                    case ActionErrorCheck:
                        if(receivedAction == 3)
                        {
                            if (n++ >= 8) break;
                            nextState = ActionConnect;
                        }
                        break;
                }
                switch(nextState)
                {

                }

                curState = nextState;
            }

            return false;
        }

        public override List<Peer> Scrape()
        {
            return null;
        }
    }
}
