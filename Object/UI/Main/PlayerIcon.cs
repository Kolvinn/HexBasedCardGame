using Godot;
using System;

public class PlayerIcon : Control
{
    public AnimationPlayer animationPlayer;
    public override void _Ready()
    {
        animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
    }

    
}
