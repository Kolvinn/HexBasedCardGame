using Godot;
using System;
using System.Collections.Generic;

public class HandView : Node2D
{
    private float handOffset = 700f;

    private float rotationLimit =40;
    float halfRotationLimit;

    
    private float cardRotationMax = 7, cardRotationMin = 3;
    Vector2 startPos;

    private Area2D cardContainer;

    public Params.MouseEventState eventState {
        get;
        set;
    }


    List<Node2D> holders = new List<Node2D>();
    public override void _Ready()
    {   
        this.halfRotationLimit = rotationLimit/2;
        startPos = new Vector2(0,-handOffset);
        this.cardContainer = GetNode<Area2D>("Area2D");
    }

    public bool AddCardAndRotate(Node2D cardHolder, Card cardView, int cardCount,  float handLimit){
               
        this.AddChild(cardHolder);
        cardHolder.AddChild(cardView);
        cardView.Position = startPos;
        cardView.startingPosition = startPos;
        this.holders.Add(cardHolder);

        //if we are just adding one card, then we don't need to wory about rotation   
        if(cardCount == 1){
            return true;

        }
    
        if(Params.Debug){
            GD.Print("Card holder postition: ", cardHolder.Position);

            GD.Print("Card view postition: ", cardView.Position);

            GD.Print("CardHolder child: ",cardHolder.GetChild(0));

        }
        //always need to make sure our rotation matches our card split
        float nextRotation = rotationLimit/(cardCount-1);
        
        //if we are equal, or less than, continue
        nextRotation = nextRotation <= cardRotationMax ? nextRotation : cardRotationMax;
        
        //rotate first card by half
        float nextPos = (this.holders[0].RotationDegrees - (nextRotation/2));

        //set hard limit on first card position if rotated more than half limit
        nextPos = Math.Abs(nextPos) > rotationLimit/2 ? -rotationLimit/2: nextPos;

        float finalCardPos = nextPos + (holders.Count * nextRotation);

        //GD.Print("next rotation amount: "+nextRotation+ " card rot: "+this.holders[0].RotationDegrees+ "   next pos: "+nextPos);

        float maxH =0, maxV =0;
        float minH= float.MaxValue, minV = float.MaxValue;
        
        //float nextPos = startingPos + nextRotation;
        for(int i =0; i <holders.Count;i++){
            Tween tween = new Tween();
            this.AddChild(tween);
            tween.InterpolateProperty(this.holders[i], "rotation_degrees", this.holders[i].RotationDegrees, nextPos, 0.5f, Tween.TransitionType.Quart, Tween.EaseType.Out);
            tween.Start();
            nextPos +=nextRotation;
            tween.Dispose();

        }

        //add some useful debugging
        return true;

        
    }


    public bool RemoveCardAndRotate(Node2D cardHolder, Card cardView, int cardCount,  float handLimit){
        
        this.RemoveChild(cardHolder);
        cardHolder.Dispose();

        int index =  this.holders.IndexOf(cardHolder);
        this.holders.Remove(cardHolder);

        if(cardCount ==0 )
            return true;
    
        //always need to make sure our rotation matches our card split
        float nextRotation = rotationLimit/(cardCount-1);
        
        //if we are equal, or less than, continue
        nextRotation = nextRotation <= cardRotationMax ? nextRotation : cardRotationMax;
        
        //rotate first card by half - rotate to left if the first card was removed
        float nextPos;
        if(index ==0)
            nextPos = (this.holders[0].RotationDegrees - (nextRotation/2));
        else
            nextPos = (this.holders[0].RotationDegrees + (nextRotation/2));

        //find if this new place has offset the final card too much (we are over card rotation limit)
        //if we are then ignore nextpos changes
        float finalCardPos = nextPos + ((cardCount-1) * nextRotation);
        if(finalCardPos > rotationLimit/2){
            nextPos = this.holders[0].RotationDegrees;
        }
        else{
            //set hard limit on first card position if rotated more than half limit
            nextPos = Math.Abs(nextPos) > rotationLimit/2 ? -rotationLimit/2: nextPos;
        }


       

        //GD.Print("next rotation amount: "+nextRotation+ " card rot: "+this.holders[0].RotationDegrees+ "   next pos: "+nextPos);

        
        //float nextPos = startingPos + nextRotation;
        for(int i =0; i <holders.Count;i++){
            Tween tween = new Tween();
            this.AddChild(tween);
            tween.InterpolateProperty(this.holders[i], "rotation_degrees", this.holders[i].RotationDegrees, nextPos, 0.5f, Tween.TransitionType.Quart, Tween.EaseType.Out);
            tween.Start();
            nextPos +=nextRotation;
            tween.Dispose();
        }

        //add some useful debugging
        return true;

        
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

    public void _on_Area2D_mouse_entered(){
        this.eventState  = Params.MouseEventState.Entered;       
    }

    public void _on_Area2D_mouse_exited(){
        this.eventState  = Params.MouseEventState.Exited;
    }
}
