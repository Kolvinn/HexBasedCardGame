using Godot;
using System;

public class SpellSlot : Node2D
{
    public string BoundAction = "";
    TextureRect cardTex;   

    public ColorRect Border;

    public Card BoundCard;
    

    public State.MouseEventState slotState {get;set;}

    public Card card {get;set;}
    public override void _Ready()
    {
        this.slotState = State.MouseEventState.Exited;
        this.Border = GetNode<ColorRect>("TextureRect/Border");
        //this.Border.Visible = true;
        //this.cardTex = GetNode<TextureRect>("MarginContainer/CardTex");
        //this.cardTex.Texture = null;
    }  

    public void UpdateBorder(bool hidden)
    {
        this.Border.Visible = hidden;
    }
    public void _on_Area2D_mouse_entered(){

        GD.Print("Area 2d entered for spell slot");
        this.slotState = State.MouseEventState.Entered;
    }

    public void _on_Area2D_mouse_exited(){
        GD.Print("Area 2d exited for spell slot");
        this.slotState = State.MouseEventState.Exited;
    }




//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
