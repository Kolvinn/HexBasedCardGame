using Godot;
using System;
using System.Collections.Generic;

public class Hand : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    private Sprite card;
    private float handOffset = 700;

    private float rotationLimit =40;
    
    private float cardRotationMax = 7, cardRotationMin = 3;

    private float handLimit;

    List<Card> cards = new List<Card>();

    List<Node2D> holders = new List<Node2D>();

    private Vector2 startPos;


    public override void _Ready()
    {
        startPos = new Vector2(0,-handOffset);
        this.handLimit = rotationLimit/cardRotationMin;
        // card = new Sprite();
        
        // card.Texture = GD.Load<Texture>("res://Helpers/Pixel Fantasy Playing Cards/Playing Cards/card-hearts-13.png");
        // this.AddChild(card);
        // card.Position = startPos;
    }

    public void _on_Button2_pressed(){
        AddCard(Loader.LoadScene<Card>("res://Helpers/Card.tscn"));
    }

    public void AddCard(Card card){
        
        Node2D cardHolder = Loader.LoadScene<Node2D>("res://Helpers/CardHolder.tscn");
        this.AddChild(cardHolder);
        cardHolder.AddChild(card);
        card.Position = startPos;
        ////GD.Print("Card holder postition: ", cardHolder.Position);

        if(cards.Count == 0){
            this.cards.Add(card);
            this.holders.Add(cardHolder);


        }
        else if(cards.Count< this.handLimit){
            this.cards.Add(card);
            this.holders.Add(cardHolder);
            //card.Position = startPos;
            //this.cards.Add(card);
           // float currentRotation = rotationLimit/cards.Count;
            float nextRotation = rotationLimit/(cards.Count +1);
            
            //if we are equal, or less than, continue
            if(nextRotation <= cardRotationMax){

            }
            else{
                nextRotation = cardRotationMax;
            }
            
            float nextPos = (this.holders[0].RotationDegrees - (nextRotation/2));

            float finalCardPos = nextPos + (holders.Count * nextRotation);

            ////GD.Print("card rot: "+this.holders[0].RotationDegrees+ "   next pos: "+nextPos);

            
            //float nextPos = startingPos + nextRotation;
            for(int i =0; i <holders.Count;i++){
                Tween tween = new Tween();
                this.AddChild(tween);
                tween.InterpolateProperty(this.holders[i], "rotation_degrees", this.holders[i].RotationDegrees, nextPos, 0.5f, Tween.TransitionType.Quart, Tween.EaseType.Out);
                tween.Start();
                nextPos +=nextRotation;
                
                //tween.Dispose();
            }
            
        }

        
    }
    
    private int GetCardRotation(){
        return 0;
    }
    private void RotateCardToPoint(Card card, int rotation){

    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
