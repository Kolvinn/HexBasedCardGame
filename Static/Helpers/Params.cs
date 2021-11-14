using Godot;
using System;
public static class Params{

    public static string CardDirectory = "res://Helpers/Pixel Fantasy Playing Cards/Playing Cards/";

     public static T LoadScene<T> (string scenePath){
        var packedScene = ResourceLoader.Load(scenePath) as PackedScene;
        var instance = packedScene.Instance();
        return (T)Convert.ChangeType(instance, typeof(T));
    }

    public static T LoadScene<T> (PackedScene packedScene){
        var instance = packedScene.Instance();
        return (T)Convert.ChangeType(instance, typeof(T));
    }


}