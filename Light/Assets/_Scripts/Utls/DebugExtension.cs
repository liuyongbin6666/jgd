using System.Runtime.CompilerServices;
using UnityEngine;

public static class DebugExtension
{
    public static void Log(this Object obj, object message) => Debug.Log($"{obj.name} : {message}");
    public static void Log(this Object obj, [CallerMemberName] string message = null)
    {
        object messageObj = $"{message} Invoke()!";
        obj.Log(messageObj);
    }
}