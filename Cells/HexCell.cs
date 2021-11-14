using Godot;
using System;

public class HexCell : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    private Label label;

    public override void _Ready()
    {

        this.label  =GetNode<Label>("Viewport/Control/ColorRect/Label");
    }

    public void SetText(String text){
        this.label.RectMinSize = label.GetParentControl().RectSize;
        this.label.Text = text;
        this.label.RectMinSize = label.GetParentControl().RectSize;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
