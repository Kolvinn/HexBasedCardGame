using Godot;
using System.Collections.Generic;
using System;

public class DeckObject  : Node2D
{


    Texture toptex ;

    List<CardObject> cards = new List<CardObject>();

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

    public void SetDeck( List<CardObject> cardz){
        this.cards =cardz;
    }

    public List<CardObject> GetDeck(){
        
        return this.cards;
    }


}
