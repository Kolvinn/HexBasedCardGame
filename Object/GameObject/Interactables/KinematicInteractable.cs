using Godot;
using System;
public class KinematicInteractable : StaticBody2D, IInteractable
{

    public Area2D InteractableArea;

    public override void _Ready()
    {
        InteractableArea = this.GetNode<Area2D>("Area2D");
        if(InteractableArea != null)
        {
            InteractableArea.Connect("on_mouse_entered",this, nameof(InteractionAreaEntered));
        }
        
    }
    public void InteractionAreaEntered()
    {
        //PlayerController.
    }
    
   

    public void ValidateInteraction(string action)
    {
        
    }
}