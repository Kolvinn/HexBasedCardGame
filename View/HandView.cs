using Godot;
using System;
using System.Collections.Generic;

public class HandView : Node2D
{
    private float handOffset = 700f;

    private float rotationLimit =40;
    
    private float cardRotationMax = 7, cardRotationMin = 3;
    Vector2 startPos;

    List<Node2D> holders = new List<Node2D>();
    public override void _Ready()
    {
        startPos = new Vector2(0,-handOffset);
    }

    public bool AddCardAndRotate(Node2D cardHolder, CardView cardView, int cardCount,  float handLimit){
               
        this.AddChild(cardHolder);
        cardHolder.AddChild(cardView);
        cardView.Position = startPos;
        this.holders.Add(cardHolder);

        //if we are just adding one card, then we don't need to wory about rotation   
        if(cardCount == 1){
            return true;

        }
    
        GD.Print("Card holder postition: ", cardHolder.Position);

        GD.Print("Card view postition: ", cardView.Position);

        GD.Print("CardHolder child: ",cardHolder.GetChild(0));

        //always need to make sure our rotation matches our card split
        float nextRotation = rotationLimit/(cardCount-1);
        
        //if we are equal, or less than, continue
        nextRotation = nextRotation <= cardRotationMax ? nextRotation : cardRotationMax;
        
        //rotate first card by half
        float nextPos = (this.holders[0].RotationDegrees - (nextRotation/2));

        //set hard limit on first card position if rotated more than half limit
        nextPos = Math.Abs(nextPos) > rotationLimit/2 ? -rotationLimit/2: nextPos;

        float finalCardPos = nextPos + (holders.Count * nextRotation);

        GD.Print("next rotation amount: "+nextRotation+ " card rot: "+this.holders[0].RotationDegrees+ "   next pos: "+nextPos);

        
        //float nextPos = startingPos + nextRotation;
        for(int i =0; i <holders.Count;i++){
            GD.Print("rotating card " + i +":" + this.holders[i]+ " to point: "+nextPos);
            Tween tween = new Tween();
            this.AddChild(tween);
            tween.InterpolateProperty(this.holders[i], "rotation_degrees", this.holders[i].RotationDegrees, nextPos, 0.5f, Tween.TransitionType.Quart, Tween.EaseType.Out);
            tween.Start();
            nextPos +=nextRotation;
            
            //tween.Dispose();
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
}
