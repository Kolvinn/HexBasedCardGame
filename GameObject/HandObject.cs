using Godot;
using System;
using System.Collections.Generic;

public class HandObject : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    private Sprite card;
    private float handOffset = 700;

    private float rotationLimit =40;
    
    private float cardRotationMax = 7, cardRotationMin = 3;

    private float handLimit = 10f;

    public List<CardObject> cards = new List<CardObject>();

    List<Node2D> holders = new List<Node2D>();

    private Vector2 startPos;

    private HandView view;

    
    public override void _Ready()
    {
        this.view = GetChild<HandView>(0);
    }


    public override void _Process(float delta)
    {
        if(cards != null){
            foreach(CardObject card in this.cards){
                if(card.cardState != CardObject.CardState.Default){
                    GD.Print("processing card state with: ", card.cardState);
                    ProcessCardState(card);
                }
            }
        }
    }

    private void ProcessCardState(CardObject card){
        switch(card.cardState){
            case CardObject.CardState.Default:
                break;
            case CardObject.CardState.Discard:
                break;
            case CardObject.CardState.Drag:
                break;
            case CardObject.CardState.Draw:
                break;
            case CardObject.CardState.Drop:
                break;
            case CardObject.CardState.Hover:
                TriggerCardHover(card);
                break;
            case CardObject.CardState.HoverRemove:
                TriggerCardHoverRemove(card);
                break;
            case CardObject.CardState.Flip:
                break;
        }
    }

    private void TriggerCardHover(CardObject card){
        card.GetCardView().ZIndex = 999;
    }
    private void TriggerCardHoverRemove(CardObject card){
        card.GetCardView().ZIndex = 0;
        card.cardState = CardObject.CardState.Default;
    }




    /// <summary>
    /// Determine if we can add a card to the hand. Adds cardview to handview if allowed.
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public bool AddCard(CardObject card){
        
        //check hand limit
        if(this.handLimit < this.cards.Count+1)
            return false;

        //create card holder and give refs
        Node2D cardHolder = Loader.LoadScene<Node2D>("res://Helpers/CardHolder.tscn");

        CardView cardView = card.CreateCardView();

        GD.Print("Card View to add to hand: ", cardView);

        //cardHolder.AddChild(cardView);

        this.cards.Add(card);

        return this.view.AddCardAndRotate(cardHolder,cardView,this.cards.Count,handLimit);    
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
