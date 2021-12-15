using Godot;
using System;

public class testpath : Path2D
{
    PathFollow2D path;
    Player player;
    public override void _Ready()
    {
        path = GetNode<PathFollow2D>("PathFollow2D");

        player = path.GetNode<Player>("Player");
    }

    public override void _Process(float delta){
        this.path.Offset += 250 * delta;
        this.player.animationState.Travel("Walk");

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
