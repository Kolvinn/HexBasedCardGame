using Godot;
using System.Collections.Generic;
using System;

public class DeckObject  : Node2D
{


    Texture toptex ;

    //List<Card> cards = new List<Card>();

    public List<CardModel> modelDeck;

    DeckView view;

    public DeckObject(){

    }
    public override void _Ready()
    {
        this.view = GetNode<DeckView>("DeckView");
        this.modelDeck = new List<CardModel>();
    }

    public void SetTopTex(Texture tex){
        this.toptex =  tex;
    }

    // public void SetDeck( List<Card> cardz){
    //     this.cards =cardz;
    // }

    // public List<Card> GetDeck(){
        
    //     return this.cards;
    // }


}
