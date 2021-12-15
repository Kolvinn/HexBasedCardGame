public abstract class AbstractObjectModel{
    
    public string Name {get;set;}
    public string Id {get;set;}
    public string Description {get;set;}

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