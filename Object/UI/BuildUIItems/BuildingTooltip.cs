using Godot;
using System;

public class BuildingTooltip : TextureRect
{
    public string description;
    public string resources;

    public Label descLabel, resLabel;
    public override void _Ready()
    {
        this.resLabel = this.GetNode<Label>("VBoxContainer/ResourceLabel");
        this.descLabel = this.GetNode<Label>("VBoxContainer/MarginContainer/DescriptionLabel");

    }

    public void UpdateTooltip(string description, string resources)
    {
        this.description = description;
        this.resources = resources;
       // if(this.resLabel !=null && this.descLabel != null){
           // this.resLabel.Text = resources;
           // this.descLabel.Text = description;
       // }
    }

    public void UpdateTooltip(Vector2 pos)
    {
        if(this.resLabel !=null && this.descLabel != null){
           this.resLabel.Text = resources;
           this.descLabel.Text = description;
       }
       this.RectPosition = pos;
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
