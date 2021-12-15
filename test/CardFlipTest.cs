using Godot;
using System;
using System.Collections.Generic;

public class CardFlipTest : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private Card card;
    private AnimationPlayer animationPlayer;  

    private Deck deck;

    private Vector2 end, start;  
    // Called when the node enters the scene tree for the first time.


    public override void _Ready()
    {   

        this.card = this.GetNode<Card>("CanvasLayer/Card");
        this.animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer2");
        
    }
    public void _on_Button_pressed(){

        // Tween tween = new Tween();
        // this.AddChild(tween);
        // tween.InterpolateProperty(this.card, "position", start, end, 0.8f, Tween.TransitionType.Quad, Tween.EaseType.Out);
        // tween.Start();
        // tween.Dispose();
        this.animationPlayer.Play("DrawCard");
        //this.card.FlipCard(true);
    }

  

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
