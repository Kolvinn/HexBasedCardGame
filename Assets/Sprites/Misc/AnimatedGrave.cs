using Godot;
using System;

public class AnimatedGrave : StaticInteractable
{
    public int Health = 20;
    public override void _Ready()
    {
        
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        this.GetNode<AnimationPlayer>("AnimationPlayer").Play("Hurt");

    }
    
}
