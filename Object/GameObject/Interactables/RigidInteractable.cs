using Godot;
using System;
public class RigidInteractable : StaticBody2D, IInteractable
{
    public CollisionPolygon2D collisionPoly;

    public Area2D InteractableArea;

    public override void _Ready()
    {
        InteractableArea = this.GetNode<Area2D>("Area2D");
        InteractableArea.Connect("mouse_entered", this,nameof(_on_Area2D_mouse_entered));
        
    }
    public void InteractionAreaEntered(){
        //PlayerController.
    }

    public void _on_Area2D_mouse_entered(){
        InteractionAreaEntered();
    }
    public void ValidateInteraction(string action)
    {
        
    }
}