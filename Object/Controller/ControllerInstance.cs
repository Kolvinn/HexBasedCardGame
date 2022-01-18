using Godot;
using System;
public class ControllerInstance : Node2D, ControllerBase{

    
    public enum ControllerState{
        AcceptAllInput,
        AcceptPartialInput,
        Wait
    }

    public ControllerState state;
}