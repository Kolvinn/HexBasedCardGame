using Godot;
using System.Collections.Generic;
using System;

public class DeckObject  : Node2D
{


    Texture toptex ;

    List<Card> cards = new List<Card>();

    DeckView view;

    public DeckObject(){

    }
    public override void _Ready()
    {
        this.view = GetNode<DeckView>("DeckView");
    }

    public void SetTopTex(Texture tex){
        this.toptex =  tex;
    }

    public void SetDeck( List<Card> cardz){
        this.cards =cardz;
    }

    public List<Card> GetDeck(){
        
        return this.cards;
    }


}
