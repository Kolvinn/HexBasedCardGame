using Godot;
using System;
using System.Reflection;
using System.Collections.Generic;

public class GameController : Node, ControllerBase
{   
    private CardController cardController;
    private CanvasLayer canvasLayer;
    
    
    public GameController(){
        PopulateCardStates();
    }
    
    public override void _Ready()
    {
        cardController = this.GetNode<CardController>("UIController/Control/CardController");
        canvasLayer = this.GetChild<CanvasLayer>(1);
        //new SaveInstance(this, this.GetClass());
        Card c = Params.LoadScene<Card>("res://Object/GameObject/Card/Card.tscn");
        this.AddChild(c);
        c.GlobalPosition = new Vector2(200,200);
        var ins = new SaveInstance(c);
        //ins.PrintHeirarchy("");
        ins.LoadSceneData(this);
    }

    /// <summary>
    /// Loads Cards from save and sets starting state
    /// </summary>
    private void PopulateCardStates(){
        //cardController = this.GetNode<CardController>("CardController");
        //this.cardController.Owner = this;
    }
    
    public void _on_Button_pressed(){
       // Serializer.ConvertToJson(this);
         foreach(System.Reflection.PropertyInfo s in typeof(GameController).GetProperties()){
              Params.Print("property: {0} {1} {2}",s.Name,s.GetValue(this,null),s.PropertyType);
          }
        
        
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

    private void RecursiveSetOwner(Node parent){
        foreach(Node node in parent.GetChildren()){
            node.Owner = parent;
            GD.Print("Setting owner of node: "+node + " to Parent: "+parent);
            RecursiveSetOwner(node);
        }
    }

}
