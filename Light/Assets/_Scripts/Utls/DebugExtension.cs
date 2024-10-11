using System.Runtime.CompilerServices;
using UnityEngine;

public static class DebugExtension
{
    public static void Log(this Object obj, object message, [CallerMemberName] string methodName = null)
    {
        Debug.Log($"{obj.name} {methodName} : {message}");
    }

    public static void Log(this Object obj, [CallerMemberName] string methodName = null)
    {
        object messageObj = "Invoke()!";
        obj.Log(messageObj, methodName);
    }
}