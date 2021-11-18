using Godot;
using System;

public class Polygon2D : Godot.Polygon2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    Color outline, outline2;
    public float Width = 2.0f;
    public override void _Ready()
    {
        this.outline = new Color(0,0,0,1f);

    }

    public override void _Draw()
    {
        

        //base._Draw();
        // Vector2 end = new Vector2();
        // for( int i =1; i < Polygon.Length; i++){
        //     GD.Print(Polygon[i-1]);
        //     end = Polygon[i];
        //     DrawPolyline(Polygon[i-1], Polygon[i],outline, Width);
        //     DrawLine(Polygon[i-1], Polygon[i],outline, Width+1);
        // }
        // DrawLine(end, Polygon[0],outline, Width);
        // DrawLine(end, Polygon[0],outline, Width+1);
        DrawPolyline(this.Polygon,outline,Width, false);
        DrawLine(this.Polygon[0], this.Polygon[this.Polygon.Length-1], outline, Width, true);
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
