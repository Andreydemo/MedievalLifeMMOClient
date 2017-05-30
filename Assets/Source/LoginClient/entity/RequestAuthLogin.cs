using System.Net.Sockets;
using UnityEngine;

namespace Assets.Source.LoginClient.entity
{
    public class RequestAuthLogin : ReceivablePacket {
        
        public string Login { set; get; }
        public string Password { set; get; }

        public RequestAuthLogin(NetworkStream connection) : base(connection)
        {
        }

        public override int GetRquestSize()
        {
            return 128;
        }


        public override bool Run()
        {
            WriteToRequest(GetBites(Login), 0x5E);
            WriteToRequest(GetBites(Password), 0x6c);
            SendReuqest();
            return true;
        }

        public override byte GetCommand()
        {
            return 0x00;
        }
    }
}
