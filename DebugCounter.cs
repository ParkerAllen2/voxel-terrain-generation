using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCounter
{
    static Dictionary<string, int> counters = new Dictionary<string, int>();

    public static void AddDebug(string message)
    {
        if (counters.ContainsKey(message))
        {
            counters[message]++;
            return;
        }
        counters.Add(message, 1);
    }

    public static void LogAll()
    {
        foreach(var entry in counters)
        {
            Debug.Log(entry.Key + ": " + entry.Value);
        }
    }

    public static void Log(string message)
    {
        if (counters.ContainsKey(message))
        {
            Debug.Log(message + ": " + counters[message]);
            return;
        }
        Debug.Log("\"" + message + "\" Never occured");
    }
}
