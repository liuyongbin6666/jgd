using System.Runtime.CompilerServices;
using UnityEngine;

public static class DebugExtension
{
    public static void Log(this Object obj, object message, [CallerMemberName] string methodName = null)
    {
        var msg = message?.ToString();
        if (string.IsNullOrWhiteSpace(msg)) msg = "Invoke()!";
        Debug.Log($"{obj.name} {methodName} : {msg}");
    }
    public static void Log(this string message, Object obj, [CallerMemberName] string methodName = null) =>
        obj.Log(message, methodName);
    public static void Log(this Object obj, [CallerMemberName] string methodName = null) => obj.Log(string.Empty, methodName);
}