using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public class ResourceCost{
    public string ObjectId {get;set;}
    public int Wood {get;set;}
    public int Stone {get;set;}
    public int Essence {get;set;}
    public int Leaves {get;set;}

    public int Leaf_Bed_Roll {get;set;}

    public int Basic_Tent {get;set;}

    public ResourceCost(){
        
    }
    public Dictionary<string,int> GetResourceCosts()
    {
        Dictionary<string,int> resources = new Dictionary<string, int>();
        PropertyInfo[] properties = typeof(ResourceCost).GetProperties( BindingFlags.NonPublic | BindingFlags.Public| BindingFlags.Instance );

        foreach(PropertyInfo p in properties)
        {   
            if(p.Name != "ObjectId" && (int)p.GetValue(this) > 0)
            {
                
                resources.Add(p.Name,(int)p.GetValue(this));
            }
        }
        return resources;
    }

    public string GetFormattedResource(){
        
        string s ="";
        
        PropertyInfo[] properties = typeof(ResourceCost).GetProperties( BindingFlags.NonPublic | BindingFlags.Public| BindingFlags.Instance );

        foreach(PropertyInfo p in properties)
        {   
            if(p.Name != "ObjectId" && (int)p.GetValue(this) > 0)
            {
                s = s+ p.Name +": " + p.GetValue(this) +"\n";
                //resources.Add(p.Name,(int)p.GetValue(this));
            }
        }
        return s;
    }
}