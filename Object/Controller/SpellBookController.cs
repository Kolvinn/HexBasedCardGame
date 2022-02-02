using Godot;
using System;
using System.Collections.Generic;
using System.Runtime;
using Newtonsoft.Json;
using System.Reflection;
public class SpellBookController : Node2D, ControllerBase {
    

    [Signal]
    public delegate void AddedSpell();
    public List<Card> cardList = new List<Card>();

    public List<CardModel> cardModels = new List<CardModel>();


    public Control BookUI;
    //private List<CardModel> cardModels;

   // private DeckObject deck;

    //private Texture topTex;

    //private HandObject hand;

    private Resource mousePointer;

    public List<Card> eventQueue =  new List<Card>();

    //private CardEventHandler eventHandler;

    public bool busyThread {get;set;}

    private Card cardHoverObject;

    public Dictionary<SpellSlot, Card> spellSlots = new Dictionary<SpellSlot, Card>();

    private Dictionary<Card, Tween> cardAnimations = new Dictionary<Card, Tween>();


    private Card dragCard = null;


    // public CardController() {
    //     // this.topTex = GD.Load<Texture>("res://Helpers/Pixel Fantasy Playing Cards/Playing Cards/card-back2.png");
    //     // LoadCards();
    //     // LoadHand();
    //     // SetDeck();

    // }
    public SpellBookController (){
        
    }

    public override void _Ready()
    {       
        this.cardModels = LoadCardModels();
        

        busyThread = false;
        eventQueue = new List<Card>();


        Input.SetCustomMouseCursor(mousePointer,Input.CursorShape.CanDrop);
        Input.SetCustomMouseCursor(mousePointer,Input.CursorShape.Arrow);
        Input.SetCustomMouseCursor(mousePointer,Input.CursorShape.Drag);

    }


    public void AddSpellSlots(SpellSlot slot)
    {
        spellSlots.Add(slot,null);
    }

    
    public void On_CardEvent(Card c)
    {
        GD.Print("Adding to card event queue with state: ",c.cardState);
        this.eventQueue.Add(c);
        //this.eventQueue.Remove(c);

    }

    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("ui_cancel"))
        {
            this.BookUI.Visible =false;
        }
        if(this.dragCard != null && !Input.IsActionPressed("left_click")){
            TriggerCardDrop(dragCard);
        }
        
        else if(eventQueue.Count >0)
        {
            Card card = eventQueue[0];
            eventQueue.RemoveAt(0);
            if(!this.busyThread)
               ProcessCardState(card);
        }

    }

    private void ProcessCardState(Card card){
        switch(card.cardState){
            case State.CardState.Default:
                break;
            case State.CardState.Discard:
                break;
            case State.CardState.Drag:
                TriggerCardDrag(card);
                break;
            case State.CardState.Draw:
                break;
            case State.CardState.Drop:
                TriggerCardDrop(card);
                break;
            case State.CardState.DropCancel:
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


    private void TriggerCardDrag(Card card){
        GD.Print("trigging card drag");
        this.dragCard = card;
        busyThread = true;
       // card.Visible = false;
        card.ZIndex = 0;
        //Input.SetCustomMouseCursor(this.mousePointer);
        //card.SetVisible(false);

    }

    private void TriggerCardDrop(Card card){

        GD.Print("Card drop triggered with :",card.model);
        this.dragCard = null;
        busyThread = false;
        card.Visible = true;

        Card newCard = Params.LoadScene<Card>("res://Object/GameObject/Card/Card.tscn");
        

        

        SpellSlot spellSlot = null;;

        foreach(SpellSlot ss in this.spellSlots.Keys){
            if(ss.slotState == State.MouseEventState.Entered){
                GD.Print("Found spell slot");
                spellSlot = ss;
                break;
            }
        }
        if(spellSlot != null &&  spellSlot.card != null){
            return;
        }

        //card.ResetCardState();

        

        if(spellSlot != null){
            TryAddToSpellSlot(spellSlot,newCard);
            newCard.LoadModel(card.model);
        }




    }



    public void _on_Control_pressed(){
        ////GD.Print("pressed control");
        foreach(SpellSlot ss in this.spellSlots.Keys){
                ss.RemoveChild(ss.card);
                ss.card = null;
        }
    }

    private bool TryAddToSpellSlot(SpellSlot spellSlot, Card card){
        ////GD.Print("Adding card: " +card+" to spell slot: "+spellSlot);

        if(spellSlot.card != null){
            GD.Print("Removing card: " +spellSlot.card+" from spell slot: "+spellSlot);

            spellSlot.RemoveChild(spellSlot.card);
        }
        card.Scale = new Vector2(0.2f,0.2f);
        spellSlot.AddChild(card);
        spellSlot.BoundCard = card;
        spellSlot.card = card;

        ////GD.Print(spellSlot.GetChildren());
        EmitSignal(nameof(AddedSpell));
        return true;
    }



    /// <summary>
    /// Checks against stored cards for corresponding tweens.
    /// Returns active tween if exists, otherwise returns new tween object and stores it against this card.
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private Tween GetActiveTween(Card card){
        Tween anim = null;
        this.cardAnimations.TryGetValue(card, out anim);

        if(anim == null){
            anim =  new Tween();
            this.cardAnimations.Add(card, anim);
            this.AddChild(anim);
        }

        return anim;
    }

    private void TriggerCardHover(Card card){
        GD.Print("hover for card:  "+card);

        Tween tween = ResetTweenState("add",card);
        //ignore if it's active
        //ResetTweenState("add",card);
        //cardHoverObject = card;

        Vector2 newPos = new Vector2(card.Position.x, GetCardPositionContext(card) - 30);
        tween.InterpolateProperty(card, "position", card.Position, newPos, 0.1f, Tween.TransitionType.Quart, Tween.EaseType.Out);
        tween.InterpolateProperty(card, "scale", card.Scale, new Vector2(Params.CardHoverScale,Params.CardHoverScale), 0.05f, Tween.TransitionType.Quart, Tween.EaseType.Out);
        tween.Start();
        //this.RemoveChild(tween);
        //tween.Dispose();
        

        card.ZIndex = 999;
    }

    /// <summary>
    /// Check if tween animations are active, if they are, stop and reset position to start
    /// </summary>
    /// <param name="card"></param>
    private Tween ResetTweenState(string method, Card card){
        Tween tween = GetActiveTween(card);

        if(tween.IsActive()){
            ////GD.Print("stopping tween success: ",tween.Stop(card) + "\n   from method: "+method + "\n   for card: "+card);           
            
            //card.Position = new Vector2(card.Position.x, -700);

            //card.Scale = new Vector2(1,1);

        }

        return tween;
        //this.RemoveChild(tween);
        //tw?.Dispose();
        
    }
    /// <summary>
    /// 
    /// Simply returns the correct y offset of the card depending on it's parent.
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private float GetCardPositionContext(Card card){

        if(card.GetParent() != null && card.GetParent().GetType() == typeof(SpellSlot)){
            ////GD.Print("YES BBY");
            return 0f;
        }

        return 0;
    }

    
    private void TriggerCardHoverRemove(Card card){
        GD.Print("hover remove for card:  "+card);
        Tween tween = ResetTweenState("remove",card);

        Vector2 newPos = new Vector2(card.Position.x, GetCardPositionContext(card));
        tween.InterpolateProperty(card, "position", card.Position, newPos, 0.1f, Tween.TransitionType.Quart, Tween.EaseType.Out);
        tween.InterpolateProperty(card, "scale", card.Scale, new Vector2(Params.CardDefaultScale,Params.CardDefaultScale), 0.05f, Tween.TransitionType.Quart, Tween.EaseType.Out);
        tween.Start();
        //tween.Dispose();
        
        card.ZIndex = 0;
        card.cardState = State.CardState.Default;
    }

   
    private List<CardModel> LoadCardModels(){
        List<CardModel> models = CSVReader.LoadCardCSV();
        ////GD.Print("Loaded ", models.Count, " models");
        return models;
    }



    
}