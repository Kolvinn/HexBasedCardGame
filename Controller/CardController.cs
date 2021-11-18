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
        this.topTex = GD.Load<Texture>("res://Helpers/Pixel Fantasy Playing Cards/Playing Cards/card-back2.png");
        LoadCards();
        LoadHand();
        SetDeck();

    }

    public override void _Process(float delta)
    {
        
    }

    public void Deal(int players){
        for(int i=0; i< players; i++){
            
        }
    }

    private void LoadHand(){
        //this.hand = new HandObject();
        this.hand = GetNode<HandObject>("HandObject");
    }
    private List<CardModel> LoadCardModels(){
        return null;
    }

    public void AddRandomCardToHand(){

        int rand = new Random().Next(deck.GetDeck().Count);
        CardObject card = deck.GetDeck()[rand];
        
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