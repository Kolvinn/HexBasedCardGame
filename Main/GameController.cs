using Godot;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

using System.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections;

public class GameController : Node, ControllerBase
{   
    private CardController cardController;
    private CanvasLayer canvasLayer;

    private NewHexMapTests hexMap;
    
    private Card c;

    private SaveInstance s;
    
    private Dictionary<string,string> dict;
    public GameController(){
        PopulateCardStates();
    }
    
    public override void _Ready()
    {   
        
        
        cardController = this.GetNode<CardController>("UIController/Control/CardController");
        canvasLayer = this.GetChild<CanvasLayer>(1);
        hexMap = this.GetNode<NewHexMapTests>("NewHexMapTests");
        //new SaveInstance(this, this.GetClass());
        //c = Params.LoadScene<Card>("res://Object/GameObject/Card/Card.tscn");
       // this.AddChild(c);
        //c.GlobalPosition = new Vector2(200,200);

        //AddChild(c);

        //ins.PrintHeirarchy("");
        //ins.LoadSceneData(this);
        
        //Activator.CreateInstance
    }

    

    public void RecursiveChildPrint(Node node){
        foreach(Node n in node.GetChildren()){
            RecursiveChildPrint(n);
            if(n.GetType() == typeof(TextureRect)){
                GD.Print("TextureRect ",n.Name," with RectPosition:",((TextureRect)n).RectPosition, " and global Position: ",((TextureRect)n).RectGlobalPosition);
               

            }
            if(n.GetType() == typeof(CardListener)){
                GD.Print("TextureRect ",n.Name," with RectPosition:",((TextureRect)n).RectPosition, " and global Position: ",((TextureRect)n).RectGlobalPosition);
               

            }
       }
    }

    /// <summary>
    /// Writes the given object instance to a binary file.
    /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
    /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
    /// </summary>
    /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
    /// <param name="filePath">The file path to write the object instance to.</param>
    /// <param name="objectToWrite">The object instance to write to the XML file.</param>
    /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
    public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
    {
        using (Stream stream = System.IO.File.Open(filePath, append ? FileMode.Append : FileMode.Create))
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
        }
    }

    /// <summary>
    /// Reads an object instance from a binary file.
    /// </summary>
    /// <typeparam name="T">The type of object to read from the XML.</typeparam>
    /// <param name="filePath">The file path to read the object instance from.</param>
    /// <returns>Returns a new instance of the object read from the binary file.</returns>
    public static T ReadFromBinaryFile<T>(string filePath)
    {
        using (Stream stream = System.IO.File.Open(filePath, FileMode.Open))
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (T)binaryFormatter.Deserialize(stream);
        }
    }

    /// <summary>
    /// Loads Cards from save and sets starting state
    /// </summary>
    private void PopulateCardStates(){
        //cardController = this.GetNode<CardController>("CardController");
        //this.cardController.Owner = this;
    }
    
    public void _on_Button_pressed(){
       SaveCard();
        
        
    }
    public void _on_Button2_pressed(){
        LoadCard();
    }
    

    public void SaveCard(){
        //c.GlobalPosition = new Vector2(500,600);
        //s = new SaveInstance(this.hexMap);
        SaveInstance s = SaveLoader.SaveGame(this.hexMap);
        RemoveChild(this.hexMap);
        this.hexMap = null;
        //s.PrintHeirarchy("");
        //this.RemoveChild(c);
        //ins.PrintHeirarchy("");
        //ins.LoadSceneData(this);
        WriteToBinaryFile("D:/testfile.txt",s,false);
       // s.PrintHeirarchy("");
        
    }

    public void LoadCard(){
        

        var testObj = ReadFromBinaryFile<SaveInstance>("D:/testfile.txt");
        
        
        //GD.Print(n.player.animationState);
        this.hexMap = (NewHexMapTests)SaveLoader.LoadGame(testObj, this);      
        this.AddChild(hexMap);
        //GD.Print(n.Position, n.GlobalPosition);
        //n.GlobalPosition =  Vector2.Zero;
        //((Card)n).cardListener.RectPosition = new Vector2(200,200);
        //((Card)n).cardListener.RectGlobalPosition = new Vector2(200,200);
       //RecursiveChildPrint(n);
        
        GD.Print(testObj, "  ",testObj.childBook);
    }

    
        // RecursiveSetOwner(this);
        // var packed_scene = new PackedScene();
        // packed_scene.Pack(this);
        // ResourceSaver.Save("res://testSaveGame.tscn", packed_scene);
        //var test = new Serializer.KeysJsonConverter<GetClass()>(null);
    //     CardModel c = new CardModel();
    //     var jsonSerializerSettings = new JsonSerializerSettings() { 
    //         TypeNameHandling = TypeNameHandling.All,
    //         Formatting = Formatting.Indented
    //     };
    //     //new KeysJsonConverter(typeof(CardModel), typeof(TestModel))
    //     string json = JsonConvert.SerializeObject(c, jsonSerializerSettings);
    //     GD.Print("json: ",json);
    // }

    

}
