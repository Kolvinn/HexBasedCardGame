using Godot;
using System;
using System.Collections.Generic;
public class InteractionQueue
{
    private static List<GameObject> queue = new List<GameObject>();

    public static void AddObject(GameObject obj)
    {
        //order by biggesst y
        for(int i =0; i < queue.Count ; i++)
        {
            if( ((Node2D)obj).Position.y < ((Node2D)queue[i]).Position.y )
            {
                queue.Insert(i, obj);
                return;
            }
        }
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