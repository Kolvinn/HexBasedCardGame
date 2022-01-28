using Godot;
using System;
using System.Collections.Generic;
public static class Params{

    public static string CardDirectory = "res://Helpers/Pixel Fantasy Playing Cards/Playing Cards/";

    public static string CardDirectory2 = "res://Assets/Sprites/Cards/";

     public static T LoadScene<T> (string scenePath){
        var packedScene = ResourceLoader.Load(scenePath) as PackedScene;
        var instance = packedScene.Instance();
        return (T)Convert.ChangeType(instance, typeof(T));
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
        "Gather enough resources to craft a Fire and sleeping spot and rest the night (to heal and regain mana)",
        "Investigate the glowing light",
        "Prepare for the hu"
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