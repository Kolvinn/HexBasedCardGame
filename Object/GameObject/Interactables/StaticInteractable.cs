using Godot;
using System;
public class StaticInteractable : StaticBody2D, IInteractable
{
    public CollisionPolygon2D collisionPoly;

    public Area2D InteractableArea;

    public override void _Ready()
    {
        InteractableArea = this.GetNode<Area2D>("Area2D");
        InteractableArea.Connect("mouse_entered", this,nameof(_on_Area2D_mouse_entered));
        
        InteractableArea.Connect("mouse_exited", this,nameof(_on_Area2D_mouse_exited));
        
    }
    public void InteractionAreaEntered(){
        GD.Print("Pushing building to object queue");
        InteractionQueue.AddObject(this);
    }

    public void _on_Area2D_mouse_entered(){
        InteractionAreaEntered();
    }
    
    public void _on_Area2D_mouse_exited()
    {
        InteractionQueue.RemoveObject(this);
    }
    public virtual void ValidateInteraction(string action)
    {
        
    }
}