using System;
using Godot;
using System.Collections.Generic;
public class GameUpdateQueue 
{
    private static Queue<Interaction> updateQueue;

    public GameUpdateQueue()
    {
        updateQueue = new Queue<Interaction>();
    }

    public static void TryPushUpdate(Interaction intr)
    {   
        updateQueue.Enqueue(intr);
    }

    public Queue<Interaction> FetchQueue()
    {
        return updateQueue;
    }

    public bool HasUpdate()
    {
        return updateQueue != null && updateQueue.Count> 0;
    }
}