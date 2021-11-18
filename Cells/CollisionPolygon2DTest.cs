using Godot;
using System;

public class CollisionPolygon2DTest: Godot.CollisionPolygon2D
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
        GD.Print("draw color: ", this.outline );
        DrawPolyline(this.Polygon,outline,Width, false);
        DrawLine(this.Polygon[0], this.Polygon[this.Polygon.Length-1], outline, Width, true);
        
        
    }

    public void ChangeColourHover(bool hover){
         
        if(hover)
            outline = new Color(214,31,31, 0f);
        else
            outline = new Color(0,0,0, 0f);
        this.Update();
        GD.Print("Changing color to: ", outline);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
