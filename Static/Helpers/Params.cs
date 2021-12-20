using Godot;
using System;
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

    public static float CardOffsetY = -700f;


    public static void Print(string str, params object[] o){

        GD.Print(String.Format(str,o));
    }   


}