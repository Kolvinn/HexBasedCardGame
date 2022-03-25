using Godot;
using System.Runtime.Serialization.Configuration;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;


[Serializable]
public class Vector2Save{
    public float x, y;
    public Vector2Save(float x, float y){
        this.x = x;
        this.y = y;
    }

    public Vector2Save(Vector2 vec){
        this.x = vec.x;
        this.y = vec.y;
    }

    public Vector2Save(){

    }

    public Vector2 ToVector(){
        return new Vector2(x,y);
    }

}