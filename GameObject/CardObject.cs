using Godot;
using System;
public class CardObject : Node, GameObject
{
    private CardView view;
    private CardModel model;

    public Texture frontImage, backImage;

    
    
    public enum CardState{
        Flip,
        Draw,
        Discard,
        Drag,
        Drop,
        DropCancel,
        Hover, 
        HoverRemove,
        Default

    }
    
    public CardState cardState;

    public void SetVisible(bool visible){
        this.view.Visible = visible;
    }
    public CardObject( Texture front, Texture back){
        this.frontImage = front;
        this.backImage = back;
        this.cardState = CardState.Default;
    }

    public CardObject(){

    }

    public CardView GetCardView(){
        return this.view;
    }

    public CardView CreateCardView(){
        CardView cardView = Params.LoadScene<CardView>("res://View/CardView.tscn");
        cardView.SetParams(ref this.cardState,frontImage,backImage,frontImage);
        //this.AddChild(cardView);//new CardView(this.cardState,frontImage,backImage,frontImage);
        this.view = cardView;
        this.view.Connect("TriggerStateChange",this,nameof(TriggerCardStateChange));
        return this.view;
    }

    public void TriggerCardStateChange(CardState state){
        this.cardState = state;
    }

    public void BringToFront(){
        
    }


    public bool LoadObject()
    {
        throw new NotImplementedException();
    }

    public bool SaveObject()
    {
        throw new NotImplementedException();
    }
}