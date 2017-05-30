using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLogHandler : ILogHandler
{
    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        Debug.logger.logHandler.LogFormat(logType, context, format, args);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        Debug.logger.LogException(exception, context);
    }
}
