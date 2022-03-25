using Godot;
using System.Runtime.Serialization.Configuration;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections;
using System.Collections.Generic;


[Serializable]
public class DictSave{
    public float x, y;
    public DictSave(Dictionary<object,object> dict){
        this.x = x;
        this.y = y;
    }

    public DictSave(Vector2 vec){
        this.x = vec.x;
        this.y = vec.y;
    }



    public Vector2 ToVector(){
        return new Vector2(x,y);
    }

}