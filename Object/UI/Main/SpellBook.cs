using Godot;
using System;

public class SpellBook : Control
{
    
    public override void _Ready()
    {
        
    }

    public void _on_Button_pressed()
    {
        this.GetNode<TextureRect>("Instructions").Visible = false;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
