using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



[Serializable]
public class SaveInstance{

    private List<SaveInstance> children;


    public Type objectType;

    string classRef;

    string ResPath;
    //lass objectClass;

    public  Dictionary<string, object> childBook;


    /// <summary>
    /// There are three types of save instance,
    /// one is a generic node
    /// one is 
    /// </summary>
    /// <param name="o"></param>
    /// <param name="endNode"></param>
    public SaveInstance(object o, bool endNode = false, bool isEnum = false){

        SaveObjectData(o, endNode, isEnum);

    }


    /// <summary>
    /// Save the object for this SaveInstance
    /// </summary>
    /// <param name="o"></param>
    private void SaveObjectData(object o, bool endNode, bool isEnum){
        this.objectType = o.GetType();

        if(isEnum){

        }
        this.childBook = new Dictionary<string, object>();

        if(typeof(Node).IsInstanceOfType(o)){
            this.ResPath = ((Node)o).Filename;
        }

        if(endNode){
            TryWriteToJson(o,this.childBook);
        }
        else{
       
            var fields = o.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic |  BindingFlags.Instance);
            List<string> fieldNames = new List<string>();
            foreach(FieldInfo f in fields){
                    if(f.Name == "ptr" || f.Name == "memoryOwn" || f.GetValue(o) == null)
                        break;
                    else{
                        fieldNames.Add(f.Name);
                        ////GD.Print(" field value: ",f.Name, "   ",f.GetValue(o)?.GetType(),"    ", f.GetValue(o));
                        SaveChildData(f,f.GetValue(o), this.childBook);
                        //childBook.Add(f.Name, new SaveInstance(f.GetValue(o),null));
                }      //SaveObjectData(f.GetValue(o));
            }

            TryWriteToJson(o,childBook,fields);
        }
    }

    // public SaveInstance(Godot.Node something){

    // }

    private void SaveChildData(FieldInfo f, object o, Dictionary<string, object> parentDict){

        //if this is a node, but it is not a gameobject, it must be last node in chain.
        //so save it's refs.
        if(typeof(Node).IsInstanceOfType(o) && !typeof(GameObject).IsInstanceOfType(o)){
            parentDict.Add(f.Name, new SaveInstance(o, true));
        }
        else if(typeof(Node).IsInstanceOfType(o) && typeof(GameObject).IsInstanceOfType(o)){
            parentDict.Add(f.Name, new SaveInstance(o));
        }

        //if we are not a node
        else  if(!typeof(Node).IsInstanceOfType(o)){
            

            //if we are a model, create a new thing
            if(typeof(AbstractObjectModel).IsInstanceOfType(o)){
                parentDict.Add(f.Name,new SaveInstance(o));
            }
            else if(typeof(Enum).IsInstanceOfType(o)){
                
                //GD.Print(o.GetType(), "  ",o, "   ",f);
                parentDict.Add(f.Name, o);
                ////GD.Print(Enum.Format)
                //TryWriteSingleToJson(f,o,parentDict);
            }
            else if(typeof(Vector2).IsInstanceOfType(o)){
                //GD.Print("creating an vector 2 save obj");
                TryWriteSingleToJson(f,new Vector2Save((Vector2)o),parentDict);
                //parentDict.Add(f.Name, new Vector2Save);
            }
            else{
                TryWriteSingleToJson(f,o,parentDict);
            }

        }


    
    }

    /// <summary>
    /// Try write all public properties to json for this object aren't defined in fields, and are not null.
    /// Store these in child nodes
    /// </summary>
    /// <param name="o"></param>
    private void TryWriteToJson(object o, Dictionary<string, object> dict, FieldInfo[] fields = null){

        List<string> fieldNames = new List<string>();
        if(fields!=null){
            foreach(FieldInfo f in fields){
                fieldNames.Add(f.Name);
            }    
        }

        foreach(PropertyInfo s in objectType.GetProperties( BindingFlags.NonPublic | BindingFlags.Public| BindingFlags.Instance | BindingFlags.Static)){
                
                //ignore fields already dealt with
                if(fieldNames.Contains(s.Name) || s.GetValue(o) == null)
                    continue;
                    //Params.Print("property: {0} {1} {2}",s.Name,s.GetValue(o,null),s.PropertyType);
                try{
                    if(typeof(Vector2).IsInstanceOfType(s.GetValue(o,null))){
                        string obj = JsonConvert.SerializeObject(new Vector2Save((Vector2)s.GetValue(o,null)));
                        dict.Add(s.Name, obj);
                        //parentDict.Add(f.Name, new Vector2Save);
                    }
                    else{
                        dict.Add(s.Name, JsonConvert.SerializeObject(s.GetValue(o,null)));
                    }
                    
                }catch(Exception e){
                    ////GD.Print("EXCEPTION");
                }
            }
    }

    /// <summary>
    /// Try write a single property a dictionary
    /// </summary>
    /// <param name="o"></param>
    private void TryWriteSingleToJson(FieldInfo f,object o, Dictionary<string, object> dict, bool enumParse =false){
       
            try{
               // //GD.Print("serializing: ",JsonConvert.SerializeObject(o));
                dict.Add(f.Name, JsonConvert.SerializeObject(o));
            }catch(Exception e){
               // //GD.Print("EXCEPTION");
            }
            
    }
    public void SaveNodeObject(Node obj, string sceneRef = null){
        
        //an instanced scene
        

    }

    public void SaveModelObject(AbstractObjectModel obj){

    }

    public void PrintHeirarchy(string indent){
        //GD.Print(indent+"ObjectType: ",this.objectType);
        //GD.Print(indent+"Resource Path: "+this.ResPath);
        indent += "     ";
        foreach(string s in this.childBook?.Keys){
            object o = null;
            this.childBook?.TryGetValue(s, out o);

            if(o.GetType() == typeof(SaveInstance)){
                SaveInstance saveInstance = (SaveInstance)o;
                saveInstance.PrintHeirarchy(indent);
            }
            else{
                //GD.Print(indent+s+"  "+o);
            }
        }
    }


    public Node LoadSceneData(Node parent = null){
        //GD.Print("Attemping to convert node: ", this.objectType, "  with res path: ", this.ResPath);
        //GD.Print("CHild length: ",this.childBook.Count);
        //we have a scene to load
        Node scene = new Node();
        if(!String.IsNullOrEmpty(this.ResPath)){
            
            scene = Params.LoadScene(this.ResPath);
            Convert.ChangeType(scene,this.objectType);

            

            //add scene to tree so that we don't have variables overridden later.
            parent.AddChild(scene);
            foreach(string name in this.childBook?.Keys){
                
                object obj = null;
                this.childBook.TryGetValue(name, out obj);
                //GD.Print("testing for name, ",name);
                PopulateChildData(name, obj, scene);
            }    

        }
        return scene;


    }

    public void LoadNodeData(object parent = null){

    }

    public void PopulateChildData(string fieldName, object fieldObject, Node parent){
        //GD.Print(parent);
        var fields = parent.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic |  BindingFlags.Instance);
        var properties = parent.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic |  BindingFlags.Instance);
        List<string> fieldNames = new List<string>();

        foreach(FieldInfo f in fields){
            if(f.Name == fieldName){
                //GD.Print("found name: ", fieldName);
            }
            //if it's not null, then use it to set the value
            //we assume all scenes have been loaded in at this point and referenced by _Ready or similar
            object originObject = f.GetValue(parent);
            if(originObject != null && f.Name ==fieldName)
            {   
                //GD.Print("Found non null field: ", fieldName, ": ",fieldObject, "  ",f.GetValue(parent).GetType());
                //unpack Saveinstance for nodes
                if(typeof(SaveInstance) == fieldObject.GetType()){
                    //GD.Print("is type of SaveINstance");
                    SaveInstance si = ((SaveInstance)fieldObject);

                    foreach(string name in si.childBook?.Keys){
                
                        object obj = null;
                        si.childBook.TryGetValue(name, out obj);

                        PopulateChildData(name,obj, (Node)originObject);
                    }

                }
                else if(f.GetValue(parent) is Enum){
                    object o = State.GetEnumType(fieldObject.ToString());
                    //GD.Print("Found an enum. Setting variable to: ",o, "  from string: ", fieldObject.ToString());
                    f.SetValue(parent,o);
                }   
                else if(f.GetValue(parent) is Vector2){

                    JObject conv = (JObject)JsonConvert.DeserializeObject((string)fieldObject);

                    f.SetValue(parent,conv.ToObject<Vector2Save>().ToVector());
                }
                else{
                    try{
                        //GD.Print("      Attempting to deserialize field: " ,fieldName," with type: ",fieldObject.GetType(), "  on parent", parent);
                        string s = (string)fieldObject;
                        f.SetValue(parent,JsonConvert.DeserializeObject(s));
                    }
                    catch(Exception e){
                        //GD.Print(e);
                        //GD.Print("Cannot convert from ", fieldObject, " to ", (string)fieldObject);
                    }
                }
            }
            else if(originObject ==null && f.Name == fieldName){
                //GD.Print("Foudn null field: ", fieldName);
            }
            else{
                
            }
        }                     

        foreach(PropertyInfo p in properties){
            if(p.Name == fieldName){
               // GD.Print("found name: ", fieldName, " for object: ",parent);
            }
            //if it's not null, then use it to set the value
            //we assume all scenes have been loaded in at this point and referenced by _Ready or similar
            object originObject = p.GetValue(parent);
            if(originObject != null && p.Name ==fieldName)
            {   
                //GD.Print("Found non null field: ", fieldName, ": ",fieldObject, "  ",f.GetValue(parent).GetType());
                //unpack Saveinstance for nodes
                if(typeof(SaveInstance) == fieldObject.GetType()){
                    //GD.Print("is type of SaveINstance");
                    SaveInstance si = ((SaveInstance)fieldObject);

                    foreach(string name in si.childBook?.Keys){
                
                        object obj = null;
                        si.childBook.TryGetValue(name, out obj);

                        PopulateChildData(name,obj, (Node)originObject);
                    }

                }
                else if(p.GetValue(parent) is Enum){
                    try{
                        object o = State.GetEnumType(fieldObject.ToString());
                        //GD.Print("Found an enum. Setting variable to: ",o, "  from string: ", fieldObject.ToString());
                        p.SetValue(parent,o);
                    }
                    catch(Exception e){

                    }
                }   
                else if(p.GetValue(parent) is Vector2){
                    if(parent.GetParent() != null){
                        GD.Print("set position relative to parent, not global");
                        if(p.Name.Contains("Global")){
                            continue;
                        }
                    }
                    //if(p.Name == "RectGlobalPosition")
                        //continue;
                    JObject conv = (JObject)JsonConvert.DeserializeObject((string)fieldObject);

                    GD.Print("Found ",parent.Name, " Vector2 ",p.Name, ": ",conv);

                    p.SetValue(parent,conv.ToObject<Vector2Save>().ToVector());
                    if(typeof(Control).IsInstanceOfType(parent)){
                        GD.Print("New control positions are: ,",((Control)parent).RectPosition,((Control)parent).RectGlobalPosition);
                    }
                }
                else if(p.GetValue(parent) is AnimationPlayer){
                    continue;
                }
                else{
                    try{
                        //GD.Print("      Attempting to deserialize field: " ,fieldName," with type: ",fieldObject.GetType(), "  on parent", parent);
                        string s = (string)fieldObject;
                        p.SetValue(parent,JsonConvert.DeserializeObject(s));
                    }
                    catch(Exception e){
                        //GD.Print(e);
                        //GD.Print("Cannot convert from ", fieldObject, " to ", (string)fieldObject);
                    }
                }
            }
            else if(originObject ==null && p.Name == fieldName){
                //GD.Print("Foudn null field: ", fieldName);
            }
            else{
                
            }
        }                     

        
    }

    
}