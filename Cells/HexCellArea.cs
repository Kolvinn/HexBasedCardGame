using Godot;
using System;

public class HexCellArea : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void _on_Area2D_mouse_entered(){
        //GD.Print("ented with the mouse");
        CollisionPolygon2DTest poly  = GetNode<CollisionPolygon2DTest>("CollisionPolygon2D");
        poly.ChangeColourHover(true);

    }

    public void _on_Area2D_mouse_exited(){
        //GD.Print("exited with the mouse");
        CollisionPolygon2DTest poly  = GetNode<CollisionPolygon2DTest>("CollisionPolygon2D");
        poly.ChangeColourHover(false);
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
