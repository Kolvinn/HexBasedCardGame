using Godot;
using System;

public class BuildMenu : GridContainer
{
    
    public override void _Ready()
    {       
        MouseFilter = MouseFilterEnum.Stop;
        this.RectMinSize = new Vector2(86,0);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
