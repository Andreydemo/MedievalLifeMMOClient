using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Assets.Source.LoginClient.entity;
using UnityEngine;


namespace Assets.Source.LoginClient
{
    public class LoginClient
    {
        private TcpClient _tcpClient;
        NetworkStream _connection;
        private bool _ipconfiged = false;

        //read external data
        public string ServerIp = "";

        public System.Int32 ServerPort;

        public LoginCrypt LoginCrypt;

        public void CreateLoginSession()
        {
            ReadTcpInfo();
        }

        void ReadTcpInfo()
        {
            string path = Application.dataPath + "/TCPconfig/ip_port.txt";
            Debug.Log("path " + path);
            string tempString = File.ReadAllText(path);
            string[] configString = tempString.Split(';');
            ServerIp = configString[0];
            ServerPort = int.Parse(configString[1]);

            _ipconfiged = true;

            Debug.Log("server ip: " + ServerIp + "    server port: " + ServerPort);

            SetupTCP();

            var initPocket = new InitPocket(_connection);
            initPocket.Read();
            var authGameGuard = new AuthGameGuard(_connection) {SessionId = initPocket.SessionId};
            authGameGuard.Run();
            authGameGuard.Read();
        }

        public Boolean Login(string login, string password)
        {
            byte[] loginRequest = new byte[128];

            var loginPocket = new RequestAuthLogin(_connection)
            {
                Login = login,
                Password = password
            };

            loginPocket.Run();
            loginPocket.Read();

            return true;
        }


        public void SetupTCP()
        {
            try
            {
                if (_ipconfiged)
                {
                    _tcpClient = new TcpClient(ServerIp, ServerPort);
                    _connection = _tcpClient.GetStream();

                    Debug.Log("Successfully created TCP client and open the NetworkStream.");
                }
            }
            catch (Exception e)
            {
                Debug.Log("Unable to connect...");
                Debug.Log("Reason: " + e);
            }
        }
    }
}