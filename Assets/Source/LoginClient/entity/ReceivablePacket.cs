using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.Source.LoginClient.entity
{
    public abstract class ReceivablePacket
    {
        public byte[] Request { get; set; }
        public byte[] Response { get; set; }
        public const int Timeout = 5;

        protected NetworkStream Connection;

        protected ReceivablePacket(NetworkStream connection)
        {
            this.Connection = connection;
            Request = new byte[GetFullRequestSize()];
            byte[] size = GetBites(GetFullRequestSize());
            Request[0] = size[0];
            Request[1] = size[1];
            Request[2] = GetCommand();
            Response = new byte[GetResponseSize()];
        }

        public virtual int GetFullRequestSize()
        {
            return GetRquestSize() + 3;
        }

        public virtual int GetRquestSize()
        {
            return 1024;
        }

        public abstract bool Run();

        public virtual bool Read()
        {
            WaitResponse();
            Connection.Read(Response, 0, GetResponseSize());
            Debug.Log(Response);
            return true;
        }

        public virtual int GetResponseSize()
        {
            return 1024;
        }

        public virtual void SendReuqest()
        {
            Connection.Write(Request, 0, GetFullRequestSize());
        }

        public int GetSize()
        {
            return BitConverter.ToInt32(new byte[] {Request[0], Request[1]}, 0);
        }

        public int GetInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        public byte[] GetBites(int intValue)
        {
            return BitConverter.GetBytes(intValue);
        }

        public byte[] GetBites(string stringValue)
        {
            return Encoding.UTF8.GetBytes(stringValue);
        }

        public abstract byte GetCommand();

        public void WaitResponse()
        {
            if (Connection.DataAvailable) return;

            Debug.Log("data unavalible lets wait");
            long time = GetCurrentTime();

            while (Connection.DataAvailable == false)
            {
                Thread.Sleep(10);
                long diff = GetCurrentTime() - time;
                if (diff > Timeout) throw new TimeoutException();
            }
            Debug.Log("data became avlible");
        }

        private static long GetCurrentTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
        }

        public void WriteToRequest(byte[] value, int offset)
        {
            for (var i = 0; i < value.Length; i++)
            {
                Request[i + offset] = value[i];
            }
        }
    }
}