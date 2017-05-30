using System.Net.Sockets;
using UnityEngine;

namespace Assets.Source.LoginClient.entity
{
    public class InitPocket : ReceivablePacket
    {
        public int SessionId { get; private set; }
        public int Size { get; private set; }

        public InitPocket(NetworkStream connection) : base(connection)
        {
        }

        public override bool Read()
        {
            base.Read();
            SessionId = GetInt(new byte[] {Response[3], Response[4], Response[5], Response[6]});
            Size = GetInt(new byte[] {Response[0], Response[1], 0, 0});
            return true;
        }

        public override bool Run()
        {
            return true;
        }

        public new int GetResponseSize()
        {
            return 1024;
        }

        public override byte GetCommand()
        {
            return 0;
        }
    }
}