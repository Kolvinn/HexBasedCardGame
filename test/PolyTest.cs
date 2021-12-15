using Godot;
using System;

public class PolyTest : Polygon2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    Polygon2D child;
    public override void _Ready()
    {
        child = GetChild<Polygon2D>(0);

        Godot.Collections.Array clip = Geometry.ClipPolygons2d(this.Polygon,child.Polygon);


        // Vector2[] vec = new Vector2[clip.Count];
        //  //int i =0;
        // for(int i =0; i < clip.Count; i++){
        //     vec[i] = clip[i];
        // }


        //this.Polygon = clip;
    }

    // private Vector2[] extract_polygon(Polygon2D pol){
    //     var tpol = Vector2[];
    //     for i in pol.polygon:
    //         tpol.push_back(i + pol.position)
    //     return tpol
    // }



    



//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
