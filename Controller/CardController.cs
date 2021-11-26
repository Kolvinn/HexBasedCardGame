using Godot;
using System;
using System.Collections.Generic;
using System.Runtime;
public class CardController : Node2D {
    
    private List<CardObject> cardList;
    private List<CardModel> cardModels;

    private DeckObject deck;

    private Texture topTex;

    private HandObject hand;

    private CardEventHandler eventHandler;


    private CardObject dragCard = null;


    // public CardController() {
    //     // this.topTex = GD.Load<Texture>("res://Helpers/Pixel Fantasy Playing Cards/Playing Cards/card-back2.png");
    //     // LoadCards();
    //     // LoadHand();
    //     // SetDeck();

    // }
    public CardController(){

    }

    public override void _Ready()
    {
        this.eventHandler = new CardEventHandler();
        this.topTex = GD.Load<Texture>("res://Helpers/Pixel Fantasy Playing Cards/Playing Cards/card-back2.png");
        LoadCards();
        LoadHand();
        SetDeck();

    }

    public override void _Process(float delta)
    {
     
        
        if(this.hand != null){
            foreach(CardObject card in this.hand.cards){
                if(card.cardState != CardObject.CardState.Default){
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
                TriggerCardDrag(card);
                break;
            case CardObject.CardState.Draw:
                break;
            case CardObject.CardState.Drop:
                break;
            case CardObject.CardState.DropCancel:
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

    private void TriggerCardDrag(CardObject card){
        this.dragCard = card;
        //card.SetVisible(false);

    }
    private void TriggerCardHover(CardObject card){
        card.GetCardView().ZIndex = 999;
    }
    private void TriggerCardHoverRemove(CardObject card){
        card.GetCardView().ZIndex = 0;
        card.cardState = CardObject.CardState.Default;
    }

    public void Deal(int players){
        for(int i=0; i< players; i++){
            
        }
    }










    /// <summary>
    /// TODO Create a eventhandler that is triggered from either cardview or card object
    /// it dones't really matter which one. Just make sure that the event trigger is handed in the event class,
    /// which the pushes the event onto a queue along with the actual card object =
    ///  the cardcontroller handles the logic of the events.
    /// </summary>
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    private void LoadHand(){
        //this.hand = new HandObject();
        this.hand = GetNode<HandObject>("HandObject");
    }
    private List<CardModel> LoadCardModels(){
        return null;
    }

    
    /// <summary>
    /// Adds random card from UI button, remove later.
    /// Also bind to card listener 
    /// </summary>
    public void AddRandomCardToHand(){

        int rand = new Random().Next(deck.GetDeck().Count);
        CardObject card = deck.GetDeck()[rand];
        //eventHandler.BindCardListen(card.GetCardView());
        
        GD.Print("adding random card to hand from deck: ",card.frontImage);

        if(this.hand.AddCard(card)){
            GD.Print(this.hand.cards[0] == deck.GetDeck()[rand]);
            //GD.Print(deck.GetDeck()[rand].ID);
            this.deck.GetDeck().Remove(card);
        }
    }

    public void _on_Button_pressed(){
        AddRandomCardToHand();
    }



    /// <summary>
    /// Loads CardObjects, will not create CardViews
    /// </summary>
    private void LoadCards( ){
        List<CardObject> cards = new List<CardObject>();
        
        Texture back = GD.Load<Texture>(Params.CardDirectory+ "card-back2.png");

        Directory directory = new Directory();
        
        directory.Open(Params.CardDirectory);

        directory.ListDirBegin(true,true);

        string file  = directory.GetNext();
        
        while(!String.IsNullOrEmpty(file)){

            if(!file.Contains("back") && !file.Contains("blank") && !file.Contains("import")){
                Texture front  = GD.Load<Texture>(Params.CardDirectory + file);
                CardObject card = new CardObject(front, back);
                cards.Add(card);
            }
            
            
            file = directory.GetNext();
        }
        this.cardList = cards;

    }

    private void SetDeck(){
        //this.deck = new DeckObject();
        this.deck = GetNode<DeckObject>("DeckObject");
       
        deck.SetDeck(cardList);
        deck.SetTopTex(this.topTex);
        
    }

    
}