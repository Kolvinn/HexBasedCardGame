
using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
public class ObjectWrapper : IComparable<ObjectWrapper>{
    public object item{
        get;
        set;
    }

    public ObjectWrapper(object item){
        this.item = item;
    }

    public T V<T>(T caster){
        return (T)item;
    }
    public int CompareTo(ObjectWrapper that)
    {   
        //GD.Print("kjsndfkjanskdjfnajksdf");
        if (this.item != that) return -1;
        if (this.item == that.item) return 0;
        return 1;
    }
}