using Godot;
using System.Collections.Generic;

public class Deck : Sprite
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    Texture toptex;

    List<Card> cards;


    public override void _Ready()
    {
        
    }

    public void SetTopTex(Texture tex){
        this.toptex =  tex;
    }

    public void SetDeck(List<Card> cards){
        this.cards = cards;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
