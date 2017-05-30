using System.Collections;
using System.Collections.Generic;
using Assets.Source.LoginClient;
using UnityEngine;

public class Context
{

    public static LoginClient loginClient = new LoginClient();
    public static LoginCrypt loginCrypt = new LoginCrypt();

    static Context()
    {
        loginClient.LoginCrypt = loginCrypt;
    }
}
