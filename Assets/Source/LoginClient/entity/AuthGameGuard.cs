using System.Net.Sockets;
using UnityEngine;

namespace Assets.Source.LoginClient.entity
{
    public class AuthGameGuard : ReceivablePacket
    {
        public int SessionId { set; get; }

        public AuthGameGuard(NetworkStream connection) : base(connection)
        {
        }

        public override bool Run()
        {
            byte[] sessionBites = GetBites(SessionId);
            Request[3] = sessionBites[0];
            Request[4] = sessionBites[1];
            Request[5] = sessionBites[2];
            Request[6] = sessionBites[3];
            SendReuqest();
            return true;
        }

        public override int GetRquestSize()
        {
            return 20;
        }

        public override byte GetCommand()
        {
            return 7;
        }
    }
}