using Godot;
using System;

public class Grass : BasicResource
{
    public AnimationPlayer player;
    
    public override void _Ready()
    {
        player = this.GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void _on_GrassArea_area_entered(Area2D area)
    {
        GD.Print(area);
        if(area.Name != "GrassArea")
            this.player.Play("Shake");
    }
//     public State.MouseEventState mouseEventState;
//     public int Capacity{
//         get;set;
//     }
//     public override void _Ready()
//     {
//         Capacity = 10;
//     }

//     public void _on_KinematicBody2D_mouse_entered()
//     {
//         //////GD.Print("Mouse entered for the wooooooood");
//         this.mouseEventState =  State.MouseEventState.Entered;
//         PlayerController.TryAddObjectToQueue(this);
//     }

//     public void _on_KinematicBody2D_mouse_exited()
//     {
//         this.mouseEventState =  State.MouseEventState.Exited;
//         PlayerController.TryremoveObjectFromQueue(this);
//     }


}
