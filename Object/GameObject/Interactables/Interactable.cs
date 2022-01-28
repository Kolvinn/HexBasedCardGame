
using Godot;
using System;
public class Interactable : Area2D, GameObject
{

    
    public State.MouseEventState mouseEventState;
    public void _on_Area2D_mouse_entered()
    {
        ////GD.Print("Mouse entered for the light2d");
        this.mouseEventState =  State.MouseEventState.Entered;
        PlayerController.TryAddObjectToQueue(this);
    }

    public void _on_Area2D_mouse_exited()
    {
        this.mouseEventState =  State.MouseEventState.Exited;
        PlayerController.TryremoveObjectFromQueue(this);
    }
}