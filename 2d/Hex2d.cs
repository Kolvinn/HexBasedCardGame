using Godot;
using System;

public class Hex2d : Polygon2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    Polygon2D polyBase;

    Label label;
    public override void _Ready()
    {   
        //PoolVector2Array points = new PoolVector2Array(HexMetrics.corners2);

        label = GetChild<Label>(0);
        //polyBase = GetChild<Polygon2D>(1);
        //polyBase.Polygon = HexMetrics.cornersbase;
        this.Polygon= HexMetrics.corners3;
    }

    public void SetText(String text){
        label.Text = text;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
