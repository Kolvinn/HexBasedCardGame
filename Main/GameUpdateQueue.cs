using System;
using Godot;
using System.Collections.Generic;
public class GameUpdateQueue 
{
    private static Queue<Interaction> updateQueue;

    private static Queue<Interaction> CharacterUpdateQueue;

    private static Dictionary<Interaction, CharacterController> charDict;

    public GameUpdateQueue()
    {
        updateQueue = new Queue<Interaction>();
        CharacterUpdateQueue = new Queue<Interaction>();
        charDict = new Dictionary<Interaction, CharacterController>();
    }

    public static void TryPushUpdate(Interaction intr)
    {   
        updateQueue.Enqueue(intr);
    }

    public static void TryPushCharacterUpdate(Interaction intr, CharacterController character)
    {
        CharacterUpdateQueue.Enqueue(intr);
        charDict.Add(intr, character);
    }
    public bool HasCharacterUpdate()
    {
        return CharacterUpdateQueue?.Count >0;
    }

    public Queue<Interaction> FetchQueue()
    {
        return updateQueue;
    }

    public Queue<Interaction> FetchCharacterQueue()
    {
        return CharacterUpdateQueue;
    }

    public CharacterController FetchCharacter(Interaction intr)
    {
        var xch = charDict[intr];
        charDict.Remove(intr);
        return xch;
    }

    public bool HasUpdate()
    {
        return updateQueue != null && updateQueue.Count> 0;
    }
}