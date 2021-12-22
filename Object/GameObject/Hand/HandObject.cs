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

    public List<Card> cards = new List<Card>();

   // List<Node2D> holders = new List<Node2D>();

    Dictionary<Card,Node2D> cardMap = new Dictionary<Card, Node2D>();

    private Vector2 startPos;

    public HandView view {get;set;}

    
    public override void _Ready()
    {
        this.view = GetChild<HandView>(0);
    }


    public override void _Process(float delta)
    {
        
        // if(cards != null){
        //     foreach(Card card in this.cards){
        //         if(State.CardState != State.CardState.Default){
        //             ProcessCardState(card);
        //         }
        //     }
        // }
    }

    private void ProcessCardState(Card card){
        switch(card.cardState){
            case State.CardState.Default:
                break;
            case State.CardState.Discard:
                break;
            case State.CardState.Drag:

                break;
            case State.CardState.Draw:
                break;
            case State.CardState.Drop:
                break;
            case State.CardState.Hover:
                //TriggerCardHover(card);
                break;
            case State.CardState.HoverRemove:
                //TriggerCardHoverRemove(card);
                break;
            case State.CardState.Flip:
                break;
        }
    }

    private void TriggerCardHover(Card card){
        card.ZIndex = 999;
    }
    private void TriggerCardHoverRemove(Card card){
        card.ZIndex = 0;
        card.cardState = State.CardState.Default;
    }




    /// <summary>
    /// Determine if we can add a card to the hand. Adds cardview to handview if allowed.
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public bool AddCard(Card card){
        
        //check hand limit
        if(this.handLimit < this.cards.Count+1)
            return false;

        //create card holder and give refs
        Node2D cardHolder = Loader.LoadScene<Node2D>("res://Helpers/CardHolder.tscn");
        this.cardMap.Add(card,cardHolder);

        this.cards.Add(card);

        this.view.AddCardAndRotate(cardHolder,card,this.cards.Count,handLimit); 

        GD.Print(card.GetRect());   
        return true;
    }


    /// <summary>
    /// /// Determine if we can remove a card in hand. Adds cardview to handview if allowed.
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public bool RemoveCard(Card card){
        
        Node2D cardHolder;

        //check hand contains that card and we have a cardholder ref
        if(!this.cards.Contains(card) || !this.cardMap.TryGetValue(card, out cardHolder))
            return false;

        GD.Print("card holder to remove: ", cardHolder);
        cardHolder.RemoveChild(card);
        this.cards.Remove(card);
        this.cardMap.Remove(card);
        //create card holder and give refs
        //Node2D cardHolder = Loader.LoadScene<Node2D>("res://Helpers/CardHolder.tscn");

        //the holder attached to the scene


        return this.view.RemoveCardAndRotate(cardHolder,card,this.cards.Count,handLimit);   
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
