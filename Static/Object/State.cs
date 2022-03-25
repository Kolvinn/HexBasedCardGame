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

    [JsonConverter(typeof(StringEnumConverter))] 
    public enum CardRarity{
        Bronze,
        Silver,
        Gold,
        Platnum
    }

    [JsonConverter(typeof(StringEnumConverter))] 
    public enum GameState
    {
        Default,
        Build,
        BuildingAction,
        Wait,
        Continue
        
    }

    [JsonConverter(typeof(StringEnumConverter))] 
    public enum BuildState
    {
        Default,
        Building,
        BuildingMenu,
        BuildFinish,        
    }

    public enum ResourceType 
    {
        Wood,
        Stone,
        Leaves,
        Essence
    }

    public enum WorkerState 
    {
        Gather,
        GatherFinish,
        Dropping,
        DroppingFinish,
        Default,
        Idle,
        Travel,
        TargetReached,
        WaitingOnMovement
    }

    public enum WorkerJobType
    {
        Gather,
        Build,

    }

    public enum ControllerState 
    {
        
    }

    public enum ToolLevel
    {
        Basic,
        Intermediate,
        Advanced,
        MasterWork
    }
    public static object GetEnumType(string name){
        //CardState cardState = CardState.Default;
        if(Enum.TryParse(name, true, out State.CardState parsedEnumValue))
            return parsedEnumValue;

        //MouseEventState mouseState = MouseEventState.Exited;
        if(Enum.TryParse(name, true, out State.MouseEventState mouseState))
            return mouseState;

        if(Enum.TryParse(name, true, out State.CardRarity cardRarity))
            return cardRarity;
        // foreach(object o in Enum.GetValues(typeof(MouseEventState))){
        //     if(o.ToString() == name)
        //         return o;
        //     ////GD.Print(o.ToString(), "   ",name);
        // }

        // foreach(object o in Enum.GetValues(typeof(CardState))){
        //     if(o.GetType()+"+"+o.ToString() == name)
        //         return o;
        //     ////GD.Print(o.ToString(), "   ",o.GetType(), "    ",name);
        // }

        return null;
    }



}