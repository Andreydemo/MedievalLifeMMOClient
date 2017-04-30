using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class LoginClient
{

    private TcpClient tcpClient;
    private bool conReady;
    NetworkStream theStream;
    bool ipconfiged = false;

    //read external data
    public string serverIP = "";
    public System.Int32 serverPort;

    public void CreateLoginSession()
    {
        ReadTCPInfo();
    }

    void ReadTCPInfo()
    {
        string path = Application.dataPath + "/TCPconfig/ip_port.txt";
        Debug.Log("path " + path);
        string tempString = File.ReadAllText(path);
        string[] configString = tempString.Split(';');
        serverIP = configString[0];
        serverPort = System.Int32.Parse(configString[1]);

        ipconfiged = true;

        Debug.Log("server ip: " + serverIP + "    server port: " + serverPort);

        SetupTCP();
        if (!theStream.DataAvailable)
        {
            Debug.Log("data unavalible lets wait");
            while (theStream.DataAvailable == false)
            {
                Thread.Sleep(1);
            }
            Debug.Log("data became avlible");
        }

        string str;
        byte[] buffer = new byte[1024];
        int offset = 0;
        int count = 1024;

        theStream.Read(buffer, offset, count);
        str = Encoding.ASCII.GetString(buffer, 0, (int)buffer.Length);
        Debug.Log("data:" + str);


    }

    public void SetupTCP()
    {
        try
        {
            if (ipconfiged)
            {
                tcpClient = new TcpClient(serverIP, serverPort);
                theStream = tcpClient.GetStream();

                Debug.Log("Successfully created TCP client and open the NetworkStream.");

                conReady = true;

            }
        }
        catch (Exception e)
        {
            Debug.Log("Unable to connect...");
            Debug.Log("Reason: " + e);
        }
    }
}
