using Godot;
using System;

public static class Loader 
{
    public static T LoadScene<T> (string scenePath){
        var packedScene = ResourceLoader.Load(scenePath) as PackedScene;
        var instance = packedScene.Instance();
        return (T)Convert.ChangeType(instance, typeof(T));
    }

}
