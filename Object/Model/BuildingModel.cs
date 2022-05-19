using System.Collections.Generic;
public class BuildingModel : AbstractObjectModel
{
    public string TextureResource {get;set;}

    public string BuildingResource {get;set;}

    public string Type {get;set;}

    //public Dictionary<string, int> ResourceCost {get;set;}
    public BuildingModel()
    {
        //string s = nameof(textureResource);
    }
}