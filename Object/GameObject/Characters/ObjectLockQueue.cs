using System;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public class ObjectLockQueue
{
    private static List<GameObject> LockedResources = new List<GameObject>();
    
    public static void TryAddObject(BasicResource gameObject)
    {
        if(!LockedResources.Contains(gameObject))
        {
            LockedResources.Add(gameObject);
        }
    }

    public static void TryAddObject(HexHorizontalTest hex)
    {
        if(!LockedResources.Contains(hex))
        {
            LockedResources.Add(hex);
        }
    }
    public static void TryRemoveObject(HexHorizontalTest hex)
    {
        if(Contains(hex))
            LockedResources.Remove(hex);
    }
    public static void TryRemoveObject(BasicResource gameObject)
    {
        if(Contains(gameObject))
            LockedResources.Remove(gameObject);
    }

    public static bool Contains(GameObject obj)
    {
        return LockedResources.ToList().Contains(obj);
    }

}