using Godot;
using System;
using System.Collections.Generic;
public static class Params{

    public static string CardDirectory = "res://Helpers/Pixel Fantasy Playing Cards/Playing Cards/";

    public static string CardDirectory2 = "res://Assets/Sprites/Cards/";

     public static T LoadScene<T> (string scenePath, string name =null){         
        var packedScene = ResourceLoader.Load(scenePath) as PackedScene;
        var instance = packedScene.Instance();
        if(!string.IsNullOrEmpty(name))
            instance.Name =name;
        var jk = (T)Convert.ChangeType(instance, typeof(T));
        
        return jk;
    }
    public static Node LoadScene (string scenePath){
        var packedScene = ResourceLoader.Load(scenePath) as PackedScene;
        var instance = packedScene.Instance();
        return instance;
    }

    public static T LoadScene<T> (PackedScene packedScene){
        var instance = packedScene.Instance();
        return (T)Convert.ChangeType(instance, typeof(T));
    }

    public static bool Debug = true;


    public static float GlobalScale = 1.5f;

    public static float CardOffsetY = -3000;


    public static float CardDefaultScale = 0.4f;

    public static float CardHoverScale = 0.48f;

    public static List<string> Quests = new List<string>(){
        "Investigate the glowing light",
        "Click on your spellbook and drag and fire spell into a spellslot",
        "Gather enough resources to craft a Fire and sleeping spot.",
        "Light the fire with a fire spell and click on your sleeping spot to rest (to heal and regain mana).",       
        "Prepare for the arrival of the refugees by crafting resting/sleeping spots for all of them.",
        "Now your workers have arrived - create a resource post and assign your refugess to it to collect basic resources. Ignore the bugs pls.",
        "Now you have basic resource set up and you're rested, you can explore.\nUse the fire spell to clear a path out of the zone."
    };

    public static void Print(string str, params object[] o){

        ////GD.Print(String.Format(str,o));
    }   
    
//    public  static Dictionary<string, TextureButton> buildingMenu =new Dictionary<string, TextureButton>
//    {
//         {"Leaf-Sleep-Roll", Params.LoadScene<TextureButton>("res://Assets/Sprites/20.09 - Traveler's Camp 1.1/BedRoll.tscn")},
//         {"Basic Tent", Params.LoadScene<TextureButton>("res://Assets/Sprites/20.09 - Traveler's Camp 1.1/Tent.tscn")},
//         {"Basic Camp", Params.LoadScene<TextureButton>("res://Assets/Sprites/20.09 - Traveler's Camp 1.1/BasicCamp.tscn")}
   
//    };


   public static List<string> Buildings = new List<string>(){
       "Leaf-Bed-Roll",
       "Fire",
       "Basic Tent"
   };

   public static List<string> RawResources = new List<string>(){
       "Wood",
       "Stone",
       "Leaves",
       "Essence"
   };


    /// <summary>
    /// Clones in world paramters for newNode from baseNode to match node state
    /// </summary>
    /// <param name="baseNode"></param>
    /// <param name="newNode"></param>
   public static void CloneWorldParams(Node2D baseNode, Node2D newNode)
   {
        newNode.Scale = baseNode.Scale;
        newNode.Position = baseNode.Position;
        newNode.ZIndex = baseNode.ZIndex;
   }

}