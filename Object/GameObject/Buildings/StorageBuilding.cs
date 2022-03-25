using Godot;
using System;
using System.Collections.Generic;
public class StorageBuilding : Building
{
    public int StoredResources;

    public int Capacity;
    public List<BasicResource> AllowedResources;


    public void UpdateStorage(int amount)
    {
        //do something useful with this later on.
        this.StoredResources += amount;
        //emit a signal
    }

}