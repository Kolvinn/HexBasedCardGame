using Godot;
using System;
using System.Collections.Generic;
public class AttackObjectQueue
{
    private static List<GameObject> queue = new List<GameObject>();

    public static void AddObject(GameObject obj)
    {   
        queue.Add(obj);
    }

    public static void RemoveObject(GameObject obj)
    {
        queue.Remove(obj);
    }

    public List<GameObject> FetchQueue()
    {
        return queue;
    }


}