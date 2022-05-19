using System;
using Newtonsoft;
using Godot;
public class AbstractObjectModel {
    
    public string ObjectId {get;set;}

    public string Description {get;set;}

    public string Name {get;set;}

    public ResourceCost RequiredResources {get;set;}

    public bool Disabled {get;set;}





    public void SaveData(){
        //store data as json or something
    }

    /// <summary>
    /// Loads data from json (stringified) into a object model. Check data handler for 
    /// what format is required
    /// </summary>
    /// <param name="json"></param>
    public void LoadData(string json){

    }
}