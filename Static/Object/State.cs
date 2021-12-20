using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
public static class State{

    [JsonConverter(typeof(StringEnumConverter))] 
    public enum CardState{
        Flip,
        Draw,
        Discard,
        Drag,
        Drop,
        DropCancel,
        Hover, 
        HoverRemove,
        Default

    }

    [JsonConverter(typeof(StringEnumConverter))] 
    public enum MouseEventState{
        Entered,
        Exited,
        Pressed,
        Released
    }

    public static object GetEnumType(string name){
        //CardState cardState = CardState.Default;
        if(Enum.TryParse(name, true, out State.CardState parsedEnumValue))
            return parsedEnumValue;

        //MouseEventState mouseState = MouseEventState.Exited;
        if(Enum.TryParse(name, true, out State.MouseEventState mouseState))
            return mouseState;
        // foreach(object o in Enum.GetValues(typeof(MouseEventState))){
        //     if(o.ToString() == name)
        //         return o;
        //     GD.Print(o.ToString(), "   ",name);
        // }

        // foreach(object o in Enum.GetValues(typeof(CardState))){
        //     if(o.GetType()+"+"+o.ToString() == name)
        //         return o;
        //     GD.Print(o.ToString(), "   ",o.GetType(), "    ",name);
        // }

        return null;
    }



}