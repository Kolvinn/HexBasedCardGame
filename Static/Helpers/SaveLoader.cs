using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections;
public static class SaveLoader
{

    public static SaveInstance SaveGame(Node rootNode){
        return CreateSaveInstance(rootNode);
    }

#region SaveGame
    public static SaveInstance CreateSaveInstance(object o, bool endNode = false, bool isEnum = false){
        Type objectType = o.GetType();
        SaveInstance s = new SaveInstance(objectType);
         
        if(typeof(Node).IsInstanceOfType(o))
        {
            s.ResPath = ((Node)o).Filename;
        }
       
        //this.objectType = o.GetType();
        //this.childBook = new Dictionary<string, object>();

        

        FieldInfo[] fields = objectType.GetFields( BindingFlags.Public| BindingFlags.NonPublic |  BindingFlags.Instance);
        PropertyInfo[] properties = objectType.GetProperties( BindingFlags.NonPublic | BindingFlags.Public| BindingFlags.Instance | BindingFlags.Static);
        List<FieldInfo> fieldList = fields.OfType<FieldInfo>().ToList();
        int loopNum = 0;

        //if not endnode, we need to store private and public fields
        if(!endNode)
        {          
            foreach(FieldInfo f in fieldList)
            {
                //don't store null fields!
                if(f.Name == "ptr" || f.Name == "memoryOwn" 
                || f.GetValue(o) == null
                || (PersistAttribute)f.GetCustomAttribute(typeof(PersistAttribute)) != null)
                    continue;
                else
                {      
                    
                    //GD.Print("      custom attributes: ", f.Attributes,"      ",f.CustomAttributes);
                    object ret = ParseSaveObject(f.Name,f.GetValue(o),o);
                    loopNum++;

                    //make sure that we aren't storing something null that we can't parse!
                    if(ret !=null)
                    {
                        s.childBook.Add(f.Name,ParseSaveObject(f.Name,f.GetValue(o), o));   
                        GD.Print("writing field: ",f.Name,": ", ret, " as ",ret.GetType(), " under Namespace: ",ret.GetType().Namespace);
                    } 
                } 
            }

        }

        //always store public properties if you can for this SaveInstance
        foreach(PropertyInfo p in properties)
        {
            //just make sure we aren't storing the same thing twice.
            if(fieldList.FindIndex(item => item.Name == p.Name)>=0 
            || p.GetValue(o) == null || p.GetValue(o) == o
            || new List<string>(){"Owner","NativeInstance"}.Contains(p.Name) 
            || (PersistAttribute)p.GetCustomAttribute(typeof(PersistAttribute)) != null)
            {
                continue;
            }
            else
            {
                
                object ret = ParseSaveObject(p.Name,p.GetValue(o),o);

                //make sure that we aren't storing something null that we can't parse!
                if(ret !=null){
                    s.childBook.Add(p.Name,ParseSaveObject(p.Name,p.GetValue(o),o));    
                    //GD.Print("writing property: ",p.Name,": ", ret, " as ",ret.GetType(), " under Namespace: ",ret.GetType().Namespace);
                }
            }
                
        }
        return s;
    }


    public static object ParseSaveObject(string fieldName, object o, object parent)
    {      
        object returnValue = null;
        
        //GD.Print("parsing ",fieldName,": ", o, " as ",o.GetType());
        //need to expand on nodes that are GameObjects, otherwise we can just store a generic Node
        if(typeof(Node).IsInstanceOfType(o) && o.GetType() != typeof(Godot.AnimationPlayer))
        {           
            returnValue = !typeof(GameObject).IsInstanceOfType(o) ?  CreateSaveInstance(o, endNode:true) :  CreateSaveInstance(o);
        }
        
        else if(!typeof(Node).IsInstanceOfType(o))
        {           
            //if we are a model, create a new thing
            if(typeof(AbstractObjectModel).IsInstanceOfType(o))
            {
                returnValue = CreateSaveInstance(o);
            }
            else if(typeof(Enum).IsInstanceOfType(o))
            {               
                returnValue = o;
            }
            else if(typeof(Vector2).IsInstanceOfType(o))
            {
                returnValue = new Vector2Save((Vector2)o);
            }
            else if(o.GetType().IsGenericType)
            {
                if(o.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>)
                || o.GetType().GetGenericTypeDefinition() == typeof(List<>)
                || o.GetType().GetGenericTypeDefinition() == typeof(Queue<>))
                {
                    returnValue = StoreCollection(fieldName,o,parent);      
                }         
            }
            else if (o.GetType().IsPrimitive || o.GetType() == typeof(Decimal) || o.GetType() == typeof(String))
            {
                return o;
            }    
            //need to put final exceptions here if we need to parse them at some point
            //currently skip everything under Godot namespace (except for Vector2)
            else if(o.GetType().Namespace!= "Godot")
            {
                returnValue =  TryWriteObjectToJson(o);
            }

        }
        return returnValue;

    }

    private static string TryWriteObjectToJson(object o){
       string returnValue = null;
        try{
            // //GD.Print("serializing: ",JsonConvert.SerializeObject(o));
            returnValue = JsonConvert.SerializeObject(o);
        }catch(Exception e){
            // //GD.Print("EXCEPTION");
        }
        return returnValue;
            
    }
#endregion

#region LoadGame
    /// <summary>
    /// This will fail if the scene that's saved isn't a root scene that can load data in
    /// </summary>
    /// <param name="save"></param>
    /// <param name="root"></param>
    public static Node LoadGame(SaveInstance save, Node root){
        Node n = Params.LoadScene(save.ResPath);
        Convert.ChangeType(n,save.objectType);
        root.AddChild(n);        
        List<FieldInfo> fields = LoadFields(save,n);   
        LoadProperties(save,n,fields);
        return n;
    }


    // /// <summary>
    // /// This will fail if the scene that's saved isn't a root scene that can load data in
    // /// </summary>
    // /// <param name="save"></param>
    // /// <param name="root"></param>
    // public static Node LoadSceneRoot(SaveInstance save, Node root){
    //     Node n = Params.LoadScene(save.ResPath);
    //     Convert.ChangeType(n,save.objectType);
    //     root.AddChild(n);        
    //     List<FieldInfo> fields = LoadFields(save,n);   
    //     LoadProperties(save,n,fields);
    //     return n;
    // }

    public static object LoadSaveInstance(SaveInstance save, object loadedObject){
        //Node parentNode = new Node();
        Node saveInstanceNode = new Node();
        //AbstractObjectModel parentModel = new AbstractObjectModel();
        AbstractObjectModel saveInstanceModel = new AbstractObjectModel();
        object returnobj = null;


        if(typeof(Node).IsAssignableFrom(save.objectType))
        {
            
            if(loadedObject == null && !String.IsNullOrEmpty(save.ResPath))
            {
                //check that this scene has a parent 
                    saveInstanceNode = Params.LoadScene(save.ResPath);
                    returnobj = saveInstanceNode;
                    Convert.ChangeType(saveInstanceNode,save.objectType);               
                // else
                // {
                //     //returnobj
                // }
            }
            else if(loadedObject != null){
                returnobj = loadedObject;
            }
            else{
                //GD.Print("Attempting to change ",saveInstanceNode, " with type ",saveInstanceNode.GetType(), " to " ,save.objectType);
                returnobj = Activator.CreateInstance(save.objectType);
            }



            //returnobj = saveInstanceNode;
        }
        else if(typeof(AbstractObjectModel).IsAssignableFrom(save.objectType))
        {
            Convert.ChangeType(saveInstanceModel,save.objectType);
            returnobj = saveInstanceModel;
        }

        //we now have a non-null object that we can set properties and fields.
        // if(save.objectType == typeof(Player)){
        //     GD.Print("starting to parse player with animation state: ", ((Player)returnobj).animationState); 
        // }
        List<FieldInfo> fields = LoadFields(save,returnobj);   
        LoadProperties(save,returnobj,fields);
        //LoadFields(save,returnobj);
        return returnobj;


    }

    private static object TryReadObjectFromJson(string json){
       object returnValue = null;
        try{
            GD.Print("attempting to deserialize: ",json);
            returnValue = JsonConvert.DeserializeObject(json);
            GD.Print("deserialized: ",json, " to: ",returnValue);
        }catch(Exception e){
            GD.Print("EXCEPTION: ");
        }
        return returnValue;
            
    }

    /// <summary>
    /// Compares a field object to it's saved value and populates it with the neccessary data
    /// </summary>
    /// <param name="fieldObject"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static object LoadSingleProperty(object fieldObject, object value){
        //GD.Print("Loading single property: ",value, " of type: ",value.GetType());
        object o = null;
        // if(fieldObject !=null){
        //     //this means we've loaded in a gameobject via _Ready()
        //     if(typeof(GameObject).IsInstanceOfType(fieldObject))
        //     {
        //         o = LoadSaveInstance(value,(GameObject)fieldObject,false);
        //     }
        // }
        if(typeof(SaveInstance).IsInstanceOfType(value))
        {           
            o = LoadSaveInstance((SaveInstance)value,fieldObject);
        }
        else if(typeof(Enum).IsInstanceOfType(value))
        {           
            o = State.GetEnumType(value.ToString());
        }
        else if(typeof(Vector2Save).IsInstanceOfType(value))
        {
            o = ((Vector2Save)value).ToVector();
        }
        else if (value.GetType().IsPrimitive || value.GetType() == typeof(Decimal) || value.GetType() == typeof(String))
        {
            return value;
        }
        else
        {
            try
            {
                o =  TryReadObjectFromJson((string)value);
            }
            catch (Exception e){
                GD.Print(e);
                GD.Print(fieldObject,"   ",value);
            }
        }
        return o;
    }

    private static void LoadProperties(SaveInstance save, object saveInstanceObject, List<FieldInfo> fieldList){
        PropertyInfo[] properties = saveInstanceObject.GetType().GetProperties( BindingFlags.NonPublic | BindingFlags.Public| BindingFlags.Instance | BindingFlags.Static);
              
        foreach(PropertyInfo p in properties)
        {
            object value = null;
            try{
                object classField = p.GetValue(saveInstanceObject);
            }catch(Exception e){
                GD.Print("FOUND THE EXCEPTION");
            }//GD.Print("   p:",p.Name);

            //if the field exists in our save instance object, but not already parsed by from our fields
            if(save.childBook.TryGetValue(p.Name, out value) && !fieldList.Any(item => item.Name == p.Name))
            {
                GD.Print("Properties loader: ",p.Name, "  ", saveInstanceObject, "  ", saveInstanceObject.GetType());
               // GD.Print("         ---> ",value);
                //f.SetValue(saveInstanceObject, LoadProperty(f.Name, classField ,value, saveInstanceObject));
                
                if(saveInstanceObject.GetType() == typeof(Player)){
                    //GD.Print("assinging property ",p.Name,"    ",p.GetValue(saveInstanceObject));
                }
                object o = null;
                if(value.GetType().IsGenericType)
                {
                    o = LoadGenericCollection(p.Name,value,saveInstanceObject);
                }        
                else {
                    o = LoadSingleProperty(p.GetValue(saveInstanceObject),value);
                }
                if(o !=null)
                    p.SetValue(saveInstanceObject,o);
            }
        }
    }

    private static List<FieldInfo> LoadFields(SaveInstance save, object saveInstanceObject){
        
         FieldInfo[] fields = saveInstanceObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic |  BindingFlags.Instance);
         List<FieldInfo> fieldList = fields.OfType<FieldInfo>().ToList();
        
        foreach(FieldInfo f in fieldList)
        {
            object value = null;
            object classField = f.GetValue(saveInstanceObject);
            //if the field exists in our save instance object
            if(save.childBook.TryGetValue(f.Name, out value))
            {
                GD.Print("Field Loader: ",f.Name,"   ",classField, "  ", saveInstanceObject, "  ", saveInstanceObject.GetType());
               // GD.Print(f.Name, " ---> ",value);
                //f.SetValue(saveInstanceObject, LoadProperty(f.Name, classField ,value, saveInstanceObject));
                
                object o = null;
                if(value.GetType().IsGenericType)
                {
                    o = LoadGenericCollection(f.Name,value,saveInstanceObject);
                }        
                else {
                    o = LoadSingleProperty(f.GetValue(saveInstanceObject),value);
                }

                if(o !=null)
                {
                    f.SetValue(saveInstanceObject,o);
                    if(typeof(Player).IsInstanceOfType(f.GetValue(saveInstanceObject))){
                        Player p = (Player)f.GetValue(saveInstanceObject);
                        
                        GD.Print("got the player ", p.animationState," ",p.animationPlayer," ",p.animationTree);
                    }
                }
            }
        }
        return fieldList;
    }
#endregion

#region CollectionStorage
    public static object StoreCollection(String fieldName, object o, object parent){
        object returnValue = null;
        List<object> objList = new List<object>();
        Dictionary<object,object> objDict = new Dictionary<object, object>();
        Queue<object> objQueue = new Queue<object>();
        GD.Print("Attempting to store: ", fieldName, " as ",o, " in parent: ",parent.GetType());
        if(parent.GetType() == typeof(Card))
        {
            if(fieldName == "testStringList")
            {
                foreach(object thing in (List<string>)o){
                    GD.Print("      parsing string object ",thing.ToString());
                    objList.Add(ParseSaveObject(null,thing,null));
                }
                return objList;
                
            }
        }
        else if(parent.GetType() == typeof(CardController))
        {
            if(fieldName == "cards")
            {
                foreach(object thing in (List<Card>)o){
                    objList.Add(ParseSaveObject(null,thing,null));
                }
                return objList;
            }
            else if(fieldName == "spellSlots")
            {
                // foreach(KeyValuePair<SpellSlot, Card> thing in ((Dictionary<SpellSlot, Card>)o)){
                    
                //      objQueue.Enqueue(ParseSaveObject(null,thing.Key,null));
                // }
                return objList;
            }
            else if(fieldName == "eventQueue")
            {

            }
        }
        else if(parent.GetType() == typeof(HandObject)){
            if(fieldName == "cards"){
                foreach(object thing in (List<Card>)o){
                    objList.Add(ParseSaveObject(null,thing,null));
                }
                return objList;
            }
            else if(fieldName == "cardMap")
            {
                 foreach(KeyValuePair<Card,Node2D> thing in ((Dictionary<Card,Node2D>)o)){
                    
                     objDict.Add(ParseSaveObject(null,thing.Key,null),ParseSaveObject(null,thing.Value,null));
                }
                return objList;
            }
        }
        else if(parent.GetType() == typeof(HandView)){
            if(fieldName == "holders"){
                foreach(object thing in (List<Node2D>)o){
                    objList.Add(ParseSaveObject(null,thing,null));
                }
                return objList;
            }
        }
        
        return null;
    
    }

    public static object LoadGenericCollection(String fieldName, object o, object parent){
        object returnValue = null;
        List<object> objList = new List<object>();
        Dictionary<object,object> objDict = new Dictionary<object, object>();
        Queue<object> objQueue = new Queue<object>();
        if(parent.GetType() == typeof(Card))
        {
            if(fieldName == "testStringList")
            {
                List<string> list = new List<string>();
                foreach(object thing in (List<object>)o){
                    list.Add((string)(LoadSingleProperty(null,thing)));
                }
                return list;
                
            }
        }
        else if(parent.GetType() == typeof(CardController))
        {
            if(fieldName == "cardList")
            {
                foreach(object thing in (List<object>)o){
                    ((CardController)parent).cardList.Add((Card)(LoadSingleProperty(null,thing)));
                }
                return objList;
            }
            else if(fieldName == "spellSlots")
            {
                foreach(KeyValuePair<object, object> thing in ((Dictionary<object, object>)o))
                {                   
                    ((CardController)parent).spellSlots.Add((SpellSlot)(LoadSingleProperty(null,thing.Key)),(Card)(LoadSingleProperty(null,thing.Value)));
                }
                return objList;
            }
            else if(fieldName == "eventQueue")
            {

            }
        }
        else if(parent.GetType() == typeof(HandObject)){
            if(fieldName == "cards"){
                
                return objList;
            }
            else if(fieldName == "cardMap")
            {
                 
                return objList;
            }
        }
        else if(parent.GetType() == typeof(HandView)){
            if(fieldName == "holders"){
                
                return objList;
            }
        }
        
        return null;
    
    }
#endregion
}