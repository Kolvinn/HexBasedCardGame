using Godot;
using System;
using System.Collections.Generic;
using System.Runtime;
using Newtonsoft.Json;
using System.Reflection;
public class CardController : Node2D, ControllerBase {
    
    public List<Card> cardList;
    //private List<CardModel> cardModels;

    private DeckObject deck;

    private Texture topTex;

    private HandObject hand;

    private Resource mousePointer;

    public Queue<Card> eventQueue;

    //private CardEventHandler eventHandler;

    public bool busyThread {get;set;}

    private Card cardHoverObject;

    public Dictionary<SpellSlot, Card> spellSlots;

    private Dictionary<Card, Tween> cardAnimations;


    private Card dragCard = null;


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
        //this.eventHandler = new CardEventHandler();
        busyThread = false;
        this.eventQueue = new Queue<Card>();
        this.mousePointer = ResourceLoader.Load("res://Assets/UI/pointer.bmp");

        this.cardAnimations = new Dictionary<Card, Tween>();

        spellSlots = new Dictionary<SpellSlot, Card>();

        spellSlots.Add(GetNode<SpellSlot>("SpellSlot"),null);
        
        spellSlots.Add(GetNode<SpellSlot>("SpellSlot2"),null);

        
        //Input.SetCustomMouseCursor(mousePointer,Input.CursorShape.Forbidden);
        Input.SetCustomMouseCursor(mousePointer,Input.CursorShape.CanDrop);
        Input.SetCustomMouseCursor(mousePointer,Input.CursorShape.Arrow);
        Input.SetCustomMouseCursor(mousePointer,Input.CursorShape.Drag);
        this.topTex = GD.Load<Texture>("res://Assets/Sprites/Cards/0 - Back/Back-Elegant-With-Texture.png");
        LoadCards();
        LoadHand();
        SetDeck();

    }

    public override void _Process(float delta)
    {
        if(this.dragCard != null && !Input.IsActionPressed("left_click")){
            TriggerCardDrop(dragCard);
        }
        
        else if(eventQueue.Count >0){
            Card card = eventQueue.Dequeue();
            if(!this.busyThread)
                ProcessCardState(card);
            // if(this.hand != null){
            //     foreach(Card card in this.hand.cards){
            //         if(card.cardState != State.CardState.Default){
            //             ProcessCardState(card);
            //         }
            //     }
            // }
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
                TriggerCardHover(card);
                break;
            case State.CardState.HoverRemove:
                TriggerCardHoverRemove(card);
                break;
            case State.CardState.Flip:
                break;
        }
    }

    private void TriggerCardDrag(Card card){
        this.dragCard = card;
        busyThread = true;
        card.Visible = false;
        card.ZIndex = 0;
        //Input.SetCustomMouseCursor(this.mousePointer);
        //card.SetVisible(false);

    }

    private void TriggerCardDrop(Card card){

  
        this.dragCard = null;
        busyThread = false;
        card.Visible = true;

        //if we are dropping back into hand, no need to trigger card removes
        if(this.hand.view.eventState == State.MouseEventState.Entered){
            //return card to hand
            GD.Print("dragged card entered hand");
            //card.Visible = true;
            return;
        }
        //we need to remove the card and free up the child to put it somewhere else;
        else{
            this.hand.RemoveCard(card);
        }

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
        card.ResetCardState();

        

        if(spellSlot != null){
            TryAddToSpellSlot(spellSlot,card);
        }
        else if(this.hand.view.eventState == State.MouseEventState.Entered){
            //return card to hand
            
        }
        else{
            //this.hand.RemoveCard(card);
        }

        //would need to check for do
        
        //card.SetVisible(false);
        

    }



    public void _on_Control_pressed(){
        GD.Print("pressed control");
        foreach(SpellSlot ss in this.spellSlots.Keys){
                ss.RemoveChild(ss.card);
                ss.card = null;
        }
    }

    private bool TryAddToSpellSlot(SpellSlot spellSlot, Card card){
        GD.Print("Adding card: " +card+" to spell slot: "+spellSlot);

        if(spellSlot.card != null){
            GD.Print("Removing card: " +spellSlot.card+" from spell slot: "+spellSlot);

            spellSlot.RemoveChild(spellSlot.card);
        }
        spellSlot.AddChild(card);
        spellSlot.card = card;

        GD.Print(spellSlot.GetChildren());
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
        tween.InterpolateProperty(card, "scale", card.Scale, new Vector2(1.2f,1.2f), 0.05f, Tween.TransitionType.Quart, Tween.EaseType.Out);
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
            GD.Print("stopping tween success: ",tween.Stop(card) + "\n   from method: "+method + "\n   for card: "+card);           
            
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
            GD.Print("YES BBY");
            return 0f;
        }

        return Params.CardOffsetY;
    }

    
    private void TriggerCardHoverRemove(Card card){
        GD.Print("hover remove for card:  "+card);
        Tween tween = ResetTweenState("remove",card);

        Vector2 newPos = new Vector2(card.Position.x, GetCardPositionContext(card));
        tween.InterpolateProperty(card, "position", card.Position, newPos, 0.1f, Tween.TransitionType.Quart, Tween.EaseType.Out);
        tween.InterpolateProperty(card, "scale", card.Scale, new Vector2(1.0f,1.0f), 0.05f, Tween.TransitionType.Quart, Tween.EaseType.Out);
        tween.Start();
        //tween.Dispose();
        
        card.ZIndex = 0;
        card.cardState = State.CardState.Default;
    }

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
        Card card = deck.GetDeck()[rand];
        //eventHandler.BindCardListen(card.GetCardView());
        
        GD.Print("adding random card to hand from deck: ",card.frontImage);

        if(this.hand.AddCard(card)){
            GD.Print(this.hand.cards[0] == deck.GetDeck()[rand]);
            //GD.Print(deck.GetDeck()[rand].ID);
            this.deck.GetDeck().Remove(card);
        }

        
    }
//TextureRect TopBanner with RectPosition:(90, 20) and global Position: (162, 122)
//TextureRect MidBanner with RectPosition:(60, 0) and global Position: (156, 142.8)
//TextureRect TextureRect2 with RectPosition:(-264, -392) and global Position: (147.2, 121.6)
    public void _on_Button_pressed(){
        AddRandomCardToHand();
    }



    /// <summary>
    /// Loads Cards, will not create CardViews
    /// </summary>
    private void LoadCards( ){
        // List<Card> cards = new List<Card>();

       
        //  foreach(System.Reflection.PropertyInfo s in typeof(CardController).GetProperties()){
        //      Params.Print("property: {0} {1} {2}",s.Name,s.GetValue(this,null),s.PropertyType);
        //  }
        
        // var something = typeof(CardController).GetFields(BindingFlags.Public |  BindingFlags.NonPublic | BindingFlags.Public |  BindingFlags.Instance);

        // foreach(FieldInfo f in something){
        //     GD.Print(f.GetValue(this));
        // }
        // CardModel model = new CardModel();
        // //

         //JsonConvert.SerializeObject(this);
        // Dictionary<CardModel, string> testDic = new Dictionary<CardModel, string>()
         //{
           // {model, "hi"}
        /// };

         //var vec = Vector2.Zero;
        // string vecString = JsonConvert.SerializeObject(vec);

        // GD.Print(vecString);

         //GD.Print(JsonConvert.DeserializeObject<Vector2>(vecString));
         //string json = JsonConvert.SerializeObject(testDic, Formatting.Indented, new KeysJsonConverter(typeof(Dictionary<CardModel, string>)));
        // GD.Print(json);
        //GD.Print(ss);
       // Dictionary<CardModel, string> obj = JsonConvert.DeserializeObject<Dictionary<CardModel, string>>(json);
        // GD.Print(obj.TryGetValue((model)=>{

        // }).GetAnotherName());
        // Texture back = GD.Load<Texture>(Params.CardDirectory+ "card-back2.png");

        // Directory directory = new Directory();
        
        // directory.Open(Params.CardDirectory);

        // directory.ListDirBegin(true,true);

        // string file  = directory.GetNext();
        
        // while(!String.IsNullOrEmpty(file)){

        //     if(!file.Contains("back") && !file.Contains("blank") && !file.Contains("import")){
        //         Texture front  = GD.Load<Texture>(Params.CardDirectory + file);
        //         //Card card = Loader.LoadScene<Card>("res://GameObject/Card/Card.tscn");
        //         card.SetTextures(front,back);
        //         card.SetEventQueue(this.eventQueue);
        //         //Card card = new Card(front, back);
        //         cards.Add(card);
        //     }
            
            
        //     file = directory.GetNext();
        // }
        // this.cardList = cards;

    }

    private void SetDeck(){
        //this.deck = new DeckObject();
        this.deck = GetNode<DeckObject>("DeckObject");
       
        deck.SetDeck(cardList);
        deck.SetTopTex(this.topTex);
        
    }
    

    
}