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

    private YSort map;

    private SaveInstance s;

    private Button saveButton;
    
    private Dictionary<string,string> dict;
    public GameController(){
        PopulateCardStates();
    }
    
    public override void _Ready()
    {   
        
        
        cardController = this.GetNode<CardController>("UIController/Control/CardController");
        canvasLayer = this.GetChild<CanvasLayer>(0);
        hexMap = this.GetNode<NewHexMapTests>("NewHexMapTests");
        saveButton = this.GetNode<Button>("Button");
        HexGrid grid = new HexGrid(new Vector2(1500,500));
        map = this.GetNode<YSort>("YSort");

        foreach(KeyValuePair<int,HexHorizontalTest> entry in grid.storedHexes)
        {
            map.AddChild(entry.Value);
            entry.Value.Position = grid.storedVectors[entry.Key];
            GD.Print("adding hex: ", entry.Value, " at position: ",grid.storedVectors[entry.Key]);
        }
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
  
        GD.Print("Player current tile: ",this.hexMap.player.currentTile);

        foreach(HexCell1 cell in this.hexMap.tiles.Keys){
            GD.Print(cell);
        }
        SaveInstance s = SaveLoader.SaveGame(this.hexMap);
        RemoveChild(this.hexMap);
        this.hexMap = null;

        WriteToBinaryFile("D:/testfile.txt",s,false);
        // PropertyInfo[] fields = saveButton.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic |  BindingFlags.Instance);
        // List<PropertyInfo> fieldList = fields.OfType<PropertyInfo>().ToList();
        
        // foreach(PropertyInfo f in fieldList){
            
        //     if(f.Name =="RectSize"){
        //         GD.Print(f.Name);
        //         GD.Print(f.GetValue(this.saveButton));
        //         object fieldObject = f.GetValue(this.saveButton);
        //         object newObject = new Vector2(300,300);
                
        //         if(fieldObject !=null){

        //         }
        //         object o = f.GetValue(this.saveButton);
        //         o = new Vector2(300,300);
        //         GD.Print(f.GetValue(this.saveButton));
        //     }
        
        
    }

    private void MapBuilder()
    {
        Vector2 mapSpace = new Vector2(1000,1000);
        Vector2 screenSize = new Vector2(1920,1080);

        float mapX = (screenSize.x/2) - (mapSpace.x/2);
        float mapY = (screenSize.y/2) - (mapSpace.y/2);

        //side radius =140
        //top radius = 85.4
     

        Vector2 hexPos  = new Vector2(mapX +140, mapY +85.4f);

        //now we go column by column because our hexes are rotated!
        float tileHeight = 85.4f * 2;
        float tileYOffset = 85.4f;
        float tileYCount =0 ;

        float limitY = mapY + mapSpace.y;
        while (tileYOffset + (tileYCount * tileHeight ) <=limitY){
            ++tileYCount;
        }

        /*

	637 *205
	427 * 291.4

	637 * x = 427
	x = 0.67032967033

	427 * y = 291.4
	y = 0.68243559719


	*/
        float tileWidth = 140f * 2;
        float tileXOffset = 140;
        float tileXCount =0 ;

        float limitX = mapX + mapSpace.x;

        while (tileXOffset + (tileXCount * tileWidth ) <=limitX){
            ++tileXCount;
        }



    }

    public void LoadCard(){
        

        var testObj = ReadFromBinaryFile<SaveInstance>("D:/testfile.txt");
        
        
        //GD.Print(n.player.animationState);
        this.hexMap = (NewHexMapTests)SaveLoader.LoadGame(testObj, this);
        GD.Print("Player current tile: ",this.hexMap.player.currentTile);

        foreach(HexCell1 cell in this.hexMap.tiles.Keys){
            GD.Print(cell);
        }
        //hexMap.Owner;
       // this.AddChild(hexMap);
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
