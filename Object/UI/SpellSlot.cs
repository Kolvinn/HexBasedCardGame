using Godot;
using System;

public class SpellSlot : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    TextureRect cardTex;   


    public Params.MouseEventState slotState {get;set;}

    public Card card {get;set;}
    public override void _Ready()
    {
        this.slotState = Params.MouseEventState.Exited;
        //this.cardTex = GetNode<TextureRect>("MarginContainer/CardTex");
        //this.cardTex.Texture = null;
    }  

    public void _on_Area2D_mouse_entered(){

        GD.Print("Area 2d entered for spell slot");
        this.slotState = Params.MouseEventState.Entered;
    }

    public void _on_Area2D_mouse_exited(){
        GD.Print("Area 2d exited for spell slot");
        this.slotState = Params.MouseEventState.Exited;
    }




//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
