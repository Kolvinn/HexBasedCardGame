using Godot;

using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GameController : Node, ControllerBase

{
    [Signal]
    public delegate void ChangeScene();
    public List<CharacterController> CharacterControllers;
    public BuildController buildController;
    public CardController cardController;
    public PlayerController playerController;
    public NewUIController newUIController;
    public SpellBookController spellBookController;
    public SpellController spellController;
    public ResourcePostController resourcePostController;

    public GameUpdateQueue gameUpdateQueue;

    public AIController aiController;

    public ThreadController threadController;

    public FireController fireController;

    public BiomeSpawner biomeSpawner = new BiomeSpawner();

    public TextureRect HealthBar, ManaBar;

    bool hasFire = false;
    bool hasSleepingSpot = false;

    bool isFireLit = false;
    public static Control WoodLabel, StoneLabel, EssenceLabel,GrassLabel;

    private CanvasLayer canvasLayer;

    private Timer InputFlush;

    private NewHexMapTests hexMap;

    private int currentTask = -1;

    public static State.GameState gameState = State.GameState.Default;

    public static Dictionary<Building, CharacterController> RestSpots = new Dictionary<Building,CharacterController>();

    public static List<CharacterController> IdleWorkers = new List<CharacterController>();
    private YSort map, env;

    private Button saveButton;

    private HexGrid grid;

    public HexGrid BattleGrid;



    public Player player;


    private Interactable currentInteractive = null;


    private Building BuildingAction;

    static TextureRect WarningMessage;

    private enum GameControllerState
    {
        BuildMenuOpen,
        Build,
        BuildMenuClose,
        Default,

    }

    private GameControllerState gameControllerState = GameControllerState.Default;

    //private List<BuildingModel> buildingModels;


    public GameController(){
        threadController = new ThreadController();
        PopulateCardStates();
        CharacterControllers = new List<CharacterController>();
    }


    public override void _Ready()
    {
        aiController = new AIController();
        this.AddChild(aiController);
        newUIController = GetNode<NewUIController>("CanvasLayer");
        //newUIController.GetBuildButton().Connect("pressed", this, nameof(BuildPressed));
        gameUpdateQueue = new GameUpdateQueue();
        buildController = new BuildController(newUIController.GetNode<BuildingUIMenu>("Control3/BuildingUIMenu"));
        this.AddChild(buildController);
        //buildingModels = CSVReader.LoadBuildingCSV();
        fireController = new FireController();

        //CSVReader.LoadResourceCosts(buildingModels.Cast<AbstractObjectModel>().ToList());
        WarningMessage  = this.GetNode<TextureRect>("CanvasLayer/WarningMessage");
        //hexMap = this.GetNode<NewHexMapTests>("NewHexMapTests");
        saveButton = this.GetNode<Button>("UIController/Control/Button");
        map = this.GetNode<YSort>("Node2D/HexLayer");
        env = this.GetNode<YSort>("Node2D/EnvLayer");
        WoodLabel = GetNode<Control>("CanvasLayer/Control3/VBoxContainer/TextureRect/WoodLabel");
        StoneLabel = GetNode<Control>("CanvasLayer/Control3/VBoxContainer/TextureRect/StoneLabel");
        EssenceLabel = GetNode<Control>("CanvasLayer/Control3/VBoxContainer/TextureRect/EssenceLabel");
        GrassLabel = GetNode<Control>("CanvasLayer/Control3/VBoxContainer/TextureRect/GrassLabel");

        //this.GetNode<Timer>("CanvasLayer/Control3/VBoxContainer/TextureRect/TimerLabel/Timer").Start(500);
        this.AddChild(fireController);
        LoadUI();
        LoadSpellControllers();

        LoadHexGrid();
        LoadPlayerController();
        LoadCard();
        //HealthBar = newUIController.GetNode<TextureRect>("Control3/HealthBar");

        ManaBar = newUIController.GetNode<TextureRect>("Control3/ManaBar");

        spellController.player = player;
        spellController.environmentLayer = this.env;
        FetchNextTask();

       // threadController.AvailableCharacters.Add(SpawnRandomCharacter());

        //threadController.AvailableCharacters.Add(SpawnRandomCharacter());

       // threadController.AvailableCharacters.Add(SpawnRandomCharacter());

        //SpawnRandomCharacter();
        threadController.grid = grid;
        threadController.player = this.playerController;
        //SpawnRandomCharacter();
        //SpawnRandomCharacter
        //SpawnRandomCharacter
       // this.newUIController.GetNode<UpdateQueue>("Control3/UpdateQueue").PushUpdate("THIS IS A TEST LABEL. LETS SEE WHAY HAPPES.\nNEWLINEBBY");
        //this.newUIController.GetNode<UpdateQueue>("Control3/UpdateQueue").PushUpdate("asdfasdfasdfasdfasdf.\nNEWLINEBBY");
       // this.newUIController.GetNode<UpdateQueue>("Control3/UpdateQueue").PushUpdate("hjgkghjkghjkghjkghjkghjkS.\nNEWLINEBBY");
        playerController.UpdateUIQueue = this.newUIController.GetNode<UpdateQueue>("Control3/UpdateQueue");
    }

    public void BuildPressed()
    {
        this.gameControllerState = GameControllerState.BuildMenuOpen;

    }


    public void FetchNextTask()
    {

        if(Params.Quests.Count >= currentTask  +1)
        {
            currentTask ++;
            string text ="Current Task\n";
            text += Params.Quests[currentTask];
            this.newUIController.GetNode<TextureRect>("Control3/TaskBar").GetNode<Label>("MarginContainer/Label").Text = text;
        }
    }


    public void _on_Button_pressed()
    {
        WarningMessage.Visible = false;
    }
    public static void ShowWarning(CharacterController c)
    {
        WarningMessage.Visible = true;
        IdleWorkers.Add(c);
    }



    private void LoadPlayerController()
    {
        playerController = new PlayerController();
        
        
        playerController.SetHealth(this.newUIController.GetNode<HealthBar>("Control3/HealthBar"));
        this.AddChild(playerController);
        
        playerController.inventory = new InventoryController();
        playerController.inventory.UI = this.newUIController.GetNode<InventoryUI>("Control3/InventoryUI");
        playerController.playerIcon  = this.newUIController.GetNode<PlayerIcon>("Control3/PlayerIcon");
        playerController.AddChild(playerController.inventory);

        playerController.buildingUI = buildController.buildingUIMenu;
        playerController.TryAddToInventory(new BasicResource(){ResourceType="Wood"},100);
        
        playerController.TryAddToInventory(new BasicResource(){ResourceType="Stone"},100);
        
        playerController.TryAddToInventory(new BasicResource(){ResourceType="Dark Essence"},100);
        
        playerController.TryAddToInventory(new BasicResource(){ResourceType="Grass Fiber"},100);
        //this.playerController.Connect("TriggerInteractive", this,nameof(On_TriggerInteractive));
        //this.playerController.Connect("TriggerBuildingAction", this, nameof(On_TriggerBuildingAction));
    }

    private void LoadSpellControllers()
    {
        this.spellBookController =  LoadSpellBook();
        //this.newUIController.GetNode<BuildingIcon>("Control3/Control/SpellBookButton").Connect("ButtonPress", this, nameof(On_SpellBookButtonPressed));
        spellController = new SpellController();
        this.AddChild(spellController);
        this.AddChild(spellBookController);
        spellController.spells = spellBookController.spellSlots;
        this.spellBookController.Connect("AddedSpell", this, "On_AddedSpell");
        this.spellController.Connect("SpellCompleted", this, "On_SpellCompleted");
        this.spellController.fire = this.fireController;
    }
    public void On_SpellCompleted()
    {
        isFireLit = true;
    }

    public void On_AddedSpell()
    {
        FetchNextTask();
    }
    private void LoadUI(){
        newUIController = GetNode<NewUIController>("CanvasLayer");
        //newUIController.Connect("TriggerBuild", this, nameof(TriggerBuild));
        //newUIController.RunUI();
        //newUIController.Connect("BuildingMenuExited", this, nameof(On_BuildMenuExited));
    }

    private SpellBookController LoadSpellBook()
    {
        Control r = newUIController.GetNode<Control>("Control3/SpellBook");

        SpellBookController book = new SpellBookController();
        this.AddChild(book);
        SpellSlot slot = newUIController.GetNode<SpellSlot>("Control3/Spell Slots/SpellSlot");
        SpellSlot slot2 = newUIController.GetNode<SpellSlot>("Control3/Spell Slots/SpellSlot2");
        slot.BoundAction = "1";
        slot2.BoundAction = "2";

        //GD.Print("slot :", slot);
        book.AddSpellSlots(slot);
        book.AddSpellSlots(slot2);
        Vector2 offset = new Vector2(112,164);

        foreach(CardModel model in book.cardModels)
        {
            Control control = new Control();
            control.SizeFlagsHorizontal = 3;
            control.SizeFlagsVertical = 3;

            Card card = Params.LoadScene<Card>("res://Object/GameObject/Card/Card.tscn");
            card.Connect("CardEvent", book, nameof(book.On_CardEvent));

            book.cardList.Add(card);
            control.AddChild(card);
            card.Position = offset;
            r.GetNode<GridContainer>("TextureRect/MarginContainer/GridContainer").AddChild(control);
            card.LoadModel(model);
        }
        book.BookUI = r;
        return book;
    }

    private void On_SpellBookButtonPressed(string label)
    {
        this.spellBookController.BookUI.Visible = !this.spellBookController.BookUI.Visible;
        GD.Print("SpellBookController is now visible: ",this.spellBookController.Visible);
        //this.spellBookController.Update();
    }


    /// <summary>
    /// When a player clicks on an interactable object. Is emitted via PlayerController
    /// </summary>
    /// <param name="itv"></param>
    public void On_TriggerInteractive(Interactable itv)
    {
        ////GD.Print("On_TriggerInteractive", itv);
        currentInteractive = itv;
        this.env.RemoveChild(this.GetNode<Area2D>("Node2D/EnvLayer/Light2D"));
        this.newUIController.GetNode<Control>("Control3/FoundBook").Visible = true;
        this.newUIController.GetNode<Button>("Control3/FoundBook/TextureRect/Button").Connect("pressed", this, nameof(OnTriggerAccept));
    }

    public void OnTriggerAccept()
    {
        this.FetchNextTask();

        this.newUIController.GetNode<BuildingIcon>("Control3/Control/SpellBookButton").Visible = true;
        this.newUIController.GetNode<Control>("Control3/FoundBook").Visible = false;
        this.newUIController.GetNode<BuildingIcon>("Control3/Control/SpellBookButton").button.Disabled =false;
    }


#region BuildController
    // public void On_TriggerBuildingAction(Building model)
    // {

    //     //gameState = State.GameState.BuildingAction;
    //         //set input to wait for building action to complete

    //     gameState = State.GameState.Wait;

    //    // this.playerController.state = ControllerInstance.ControllerState.Wait;

    //     ParseBuildingAction(model.model);
    //     // GD.Print("On_TriggerBuildingAction");
    //     // gameState = State.GameState.BuildingAction;
    //     // this.playerController.state = ControllerInstance.ControllerState.Wait;
    //     // this.BuildingAction = model;
    // }
    // public void TriggerBuild(string buildingName)
    // {
    //     foreach(KeyValuePair<int, HexHorizontalTest> p in grid.storedHexes){
    //         p.Value.topBorder.Visible =true;
    //     }

    //     var building = buildingModels.FirstOrDefault(item => item.Name == buildingName);

    //     BuildBuilding(building);
    // }

    // public void BuildBuilding(BuildingModel building)
    // {
    //     //////GD.Print("processing tent click");
    //     gameState = State.GameState.Build;
    //     //Resource tent = ResourceLoader.Load("res://Assets/UI/pointer.bmp");
    //     if( IsInstanceValid(this.buildController) &&  this.buildController != null){
    //         this.RemoveChild(this.buildController);

    //     }
    //     BuildController b = new BuildController(building);
    //     b.Connect("BuildCancel", this, nameof(TriggerBuildCancel));
    //     b.Connect("BuildBuildingComplete", this, nameof(BuildBuildingComplete));
    //     this.AddChild(b);

    //     //IMPORTANT -- make sure that the buildcontroller is the first child processed in the heirarchy
    //     //^^ this guy is an idiot
    //     this.MoveChild(b,0);
    //     this.buildController = b;
    // }

    // public void BuildBuildingComplete(Building building)
    // {

    //     //this.buildController.buildState = State.BuildState.Default;

    //     this.env.AddChild(building.buildingAsset);
    //     if(building.model.Name == "Leaf-Sleep-Roll")
    //     {
    //         // this.hasSleepingSpot = true;
    //         // RestSpots.Add(building,null);
    //         // if(IdleWorkers.Count >0){
    //         //     CharacterController c = IdleWorkers[0];
    //         //     c.Rest();
    //         //     IdleWorkers.RemoveAt(0);
    //         // }
    //     }
    //     else if(building.model.Name == "Fire")
    //     {
    //         this.hasFire =true;
    //     }

    //     else if(this.buildController.selectedModel.Name == "Gathering Post")
    //     {

    //         // OptionBox b = this.newUIController.CreateOption("Update worker amount at this Resource Post");
    //         // this.resourcePostController = new ResourcePostController(b,grid, buildController.ConstructedBuilding);

    //         // this.resourcePostController.Connect("CloseDialog",this,nameof( On_ResourcePostCloseDialog));
    //         // this.resourcePostController.Connect("ResourceDrop", this, nameof(AddBulkResource));
    //         // this.resourcePostController.Connect("ResourceTaskAssignmentComplete", this, "On_ResourceTaskAssignmentComplete");
    //         // this.AddChild(resourcePostController);

    //         // //Character c = SpawnRandomCharacter();
    //         // foreach(CharacterController c in this.CharacterControllers)
    //         // {
    //         //     resourcePostController.AvailableWorkers.Add(c);
    //         // }
    //         // // resourcePostController.AvailableWorkers.Add(SpawnRandomCharacter());
    //         // resourcePostController.AvailableWorkers.Add(SpawnRandomCharacter());
    //         // resourcePostController.AvailableWorkers.Add(SpawnRandomCharacter());
    //         // resourcePostController.AvailableWorkers.Add(SpawnRandomCharacter());
    //         // resourcePostController.AvailableWorkers.Add(SpawnRandomCharacter());
    //         // resourcePostController.AvailableWorkers.Add(SpawnRandomCharacter());
    //         // resourcePostController.AvailableWorkers.Add(SpawnRandomCharacter());
    //         // resourcePostController.AvailableWorkers.Add(SpawnRandomCharacter());

    //     }

    //     foreach(var item in building.model?.RequiredResources.ResourceCostList)
    //     {
    //         this.playerController.ResourceUpdate[item.Key] -= item.Value;
    //     }

    //     // if(hasFire && hasSleepingSpot && currentTask == 2)
    //     // {
    //     //     FetchNextTask();
    //     // }

    //     // if(RestSpots.Count == 3 && currentTask == 4)
    //     // {
    //     //     this.CharacterControllers.Add(SpawnRandomCharacter());
    //     //     this.CharacterControllers.Add(SpawnRandomCharacter());
    //     //     this.CharacterControllers.Add(SpawnRandomCharacter());
    //     //     FetchNextTask();
    //     // }
    // }

    // public void On_ResourceTaskAssignmentComplete()
    // {
    //     FetchNextTask();
    // }


    // public void TriggerBuildCancel(BuildController controller)
    // {
    //     //////GD.Print("processing build cancel");
    //     gameState = State.GameState.Default;
    //     controller.buildState = State.BuildState.Default;
    //     this.RemoveChild(controller);
    //     controller.QueueFree();

    // }

#endregion BuildController

    public void ParseGameUpdate(Interaction intr)
    {   //int start = System.DateTime.Now.Millisecond;
        //GD.Print("Parsing update : ", intr.GetType());
        if(!intr.FinalizeInteraction(playerController))
        {
            if(intr.ValidateInteraction(playerController)  &&  intr.FinalizeInteraction(playerController))
            {

            }
            else
            {
                //this is where you deal with an invalid interaction and game update
                return;
            }

        }

        if(typeof(ResourceInteraction).IsInstanceOfType(intr))
        {
            buildController.UpdateBuildingButtonStates(playerController.ResourceUpdate);
            //UpdateResourceValues();
        }
        else if(typeof(BuildCompleteInteraction).IsInstanceOfType(intr))
        {
            var b = ((BuildCompleteInteraction)intr).building;
            //UpdateResourceValues();
            
            var cost = buildController.FetchModel(((BuildCompleteInteraction)intr).name).RequiredResources;
            playerController.RemovefromInventory(cost);

            //this.env.AddChild(b);

            this.newUIController.AddBuildingUI(b.UI);
            b.ConnectExitButton();
            //buildController.SetSelectedBuilding(b);
            //((BuildCompleteInteraction)intr).building.Scale = new Vector2(3,3);
        }
        else if(typeof(BuildingInteraction).IsInstanceOfType(intr))
        {
            buildController.SetSelectedBuilding(((BuildingInteraction)intr).building);
        }

        else if(intr is BuildingUpdateInteraction)
        {
            // string s = "";
            
            // foreach(Node n in this.env.GetChildren())
            //     s+= " " +n.Name;
            GD.Print(this.env.Name);
            GD.Print(((BuildingUpdateInteraction)intr).building);
            //threadController.ChangeWorkerAmount(((BuildingUpdateInteraction)intr).building,((BuildingUpdateInteraction)intr).WorkerAmountChange);
            this.env.AddChild(((BuildingUpdateInteraction)intr).building);
        }
        else if(intr is BuildingMenuInteraction)
        {
            buildController.UpdateBuildingButtonStates(playerController.ResourceUpdate);
        }
        else if (intr is TauntInteraction)
        {
            //aiController.PushUp
        }
        else if( intr is CombatInteraction)
        {
            buildController.UpdateBuildingButtonStates(playerController.ResourceUpdate);
            CombatInteraction cint = ((CombatInteraction)intr);
            GD.Print("combat interaction object is: ", cint.target);
            aiController.HandleInteraction(cint.target, this.GetNode<YSort>("Node2D/TerrainLayer"));
            
            if(aiController.AreaJustCleared())
                this.newUIController.GetNode<AnimationPlayer>("Control3/Control2/TextureRect/AnimationPlayer").Play("Show");

        
        }
        //int end = System.DateTime.Now.Millisecond;
        //GD.Print("Update cycle takes roughly ", end-start, "milliseconds");
    }

    private void ParseCharacterUpdate(Interaction intr, CharacterController character)
    {
        if(!intr.FinalizeInteraction(character))
        {
            if(intr.ValidateInteraction(character)  &&  intr.FinalizeInteraction(character))
            {

            }
            else
            {
                //this is where you deal with an invalid interaction and game update
                return;
            }

        }

        if(typeof(ResourceInteraction).IsInstanceOfType(intr))
        {
            UpdateResourceValues(character);
        }
        else if(typeof(ResourceDropInteraction).IsInstanceOfType(intr))
        {

        }

        else if(intr is BuildingUpdateInteraction)
        {

        }
        character.UnLock();
    }





    public override void _PhysicsProcess(float delta)
    {
        if(gameUpdateQueue.HasUpdate())
        {
            ParseGameUpdate(gameUpdateQueue.FetchQueue().Dequeue());
        }
        if(gameUpdateQueue.HasCharacterUpdate())
        {
            var intr = gameUpdateQueue.FetchCharacterQueue().Dequeue();
            ParseCharacterUpdate(intr, gameUpdateQueue.FetchCharacter(intr));
        }

        if(gameControllerState == GameControllerState.BuildMenuOpen)
        {
           // buildController.UpdateBuildingButtonStates(playerController.ResourceUpdate);
           // buildController.HandleBuildMenuState();
           // gameControllerState = GameControllerState.Build;
            //buildController.buildingState = BuildController.BuildingState.BuildMenu;


            //LOCK PLAYER CONTROLS HERE
        }
        if(gameControllerState == GameControllerState.Build)
        {
            //exit
            if(buildController.buildingState == BuildController.BuildingState.BuildMenuExited)
            {

                gameControllerState = GameControllerState.Default;
            }
        }
        // //UpdateResourceValues();

        // //finished frame continue, reset input state
        // if(gameState == State.GameState.Continue)
        // {
        //     gameState = State.GameState.Default;
        //     this.playerController.state = ControllerInstance.ControllerState.AcceptAllInput;

        // }
        // //we are in building menu
        // if(this.newUIController?.currentMenu != null && this.newUIController?.currentMenu != this.newUIController?.mainMenu){
        //     UpdateBuildingButtonStates();
        //     this.playerController.state = ControllerInstance.ControllerState.AcceptPartialInput;
        // }


        // if(gameState == State.GameState.Wait){

        // }

        // if(gameState == State.GameState.BuildingAction)
        // {

        // }


    }


    public void On_ResourcePostCloseDialog(){
        SetContinueState();
    }

    private void ParseBuildingAction(BuildingModel model)
    {
        if(model.Name == "Leaf-Sleep-Roll")
        {
            ////GD.Print("Creating option box and connecting");
            this.newUIController.CreateOption("Do you want to rest and recover?").Connect("OnResponse",this,nameof(BuildingOption));

        }
        else if(model.Name == "Gathering Post")
        {

            //this.resourcePostController.OnClick();
            //SetContinueState();
        }
        else{
            gameState = State.GameState.Continue;
        }
    }

    private CharacterController SpawnRandomCharacter(){

        //Character character = Params.LoadScene<Character>("res://Object/GameObject/Characters/Character.tscn");
        var chara = Params.LoadScene<Character>("res://Object/GameObject/Characters/Character.tscn");


        //character.Position = playerController.player.Position;
        chara.Scale = new Vector2(2,2);//player.Scale;
        //GD.Print("player position: ", this.playerController.player.Position);
        CharacterController c1 = new CharacterController(chara);

        //c1.character = character;
        //c1.grid = grid;
        foreach(KeyValuePair<int, HexHorizontalTest> p in HexGrid.storedHexes){
            if(p.Key>50 && !p.Value.isBasicResource){
                c1.character.Position = p.Value.Position;
                break;
            }
        }


        this.env.AddChild(c1.character);
        //this.AddChild(c1);
        //CharacterControllers.Add(c1);

        //GD.Print("spawning character at: ",character.Position);
        //GD.Print(character.ZIndex);
        //GD.Print(character.Scale);
        return c1;
    }


    private void SetContinueState()
    {
        gameState = State.GameState.Continue;
    }


    public void BuildingOption(bool accepted, OptionBox option)
    {
        if(accepted)
        {
            this.newUIController.Connect("AnimationCompleted", this, nameof(SetContinueState));
            this.newUIController.FadeToDark();
            FetchNextTask();
        }
        else{
            SetContinueState();
        }
        if(option != null){
            //GD.Print("removing current option: ", option);
            this.newUIController.RemoveChild(option);
            option.QueueFree();
        }
    }

    public void UpdateResourceValues()
    {
        
        foreach(KeyValuePair<string, int> pair in this.playerController.ResourceUpdate)
        {
            if(pair.Key == "Wood")
                WoodLabel.GetNode<Label>("Label").Text = "Wood: "+ pair.Value;
            if(pair.Key == "Leaves"){

                GrassLabel.GetNode<Label>("Label").Text = "Leaves: "+ pair.Value;
                GrassLabel.Update();
            }if(pair.Key == "Stone")
                StoneLabel.GetNode<Label>("Label").Text = "Stone: "+ pair.Value;

        
        }
    }

    public void UpdateResourceValues(CharacterController character)
    {
        foreach(KeyValuePair<string, int> pair in character.ResourceUpdate)
        {
                this.playerController.ResourceUpdate[pair.Key] +=pair.Value;
            // if(pair.Key == "Wood")
            // if(pair.Key == "Leaves"){

            //     GrassLabel.GetNode<Label>("Label").Text = "Leaves: "+ pair.Value;
            //     GrassLabel.Update();
            // }if(pair.Key == "Stone")
            //     StoneLabel.GetNode<Label>("Label").Text = "Stone: "+ pair.Value;

        //     woodp.GetNode<Label>("Label").Text = "+1 Leaves";
        }//     woodp.GetNode<Label>("Label").Text = "+1 Wood";
        //     ResourceUpdate.Add("Wood", ++WoodAmount);
        UpdateResourceValues();
    }





    public void AddBulkResource(BasicResource res, int amount){
        //GD.Print("Addin bulk resource of type: ",res.ResourceType, " with amount: ", amount);
        if(res.ResourceType == "Wood"){
            this.playerController.ResourceUpdate["Wood"]   = this.playerController.ResourceUpdate["Wood"]  + amount;
            WoodLabel.GetNode<Label>("Label").Text = "Wood: "+ this.playerController.ResourceUpdate["Wood"];
        }
        if(res.ResourceType == "Leaves"){
            this.playerController.ResourceUpdate["Leaves"]  = this.playerController.ResourceUpdate["Leaves"] + amount;
            GrassLabel.GetNode<Label>("Label").Text = "Leaves: "+ this.playerController.ResourceUpdate["Leaves"];

        }
        if(res.ResourceType == "Stone"){
            this.playerController.ResourceUpdate["Stone"] = this.playerController.ResourceUpdate["Stone"] + amount;
            StoneLabel.GetNode<Label>("Label").Text = "Stone: "+ this.playerController.ResourceUpdate["Stone"];


        }

    }

    public void RecursiveChildPrint(Node node){
        foreach(Node n in node.GetChildren()){
            RecursiveChildPrint(n);
            if(n.GetType() == typeof(TextureRect)){
                ////GD.Print("TextureRect ",n.Name," with RectPosition:",((TextureRect)n).RectPosition, " and global Position: ",((TextureRect)n).RectGlobalPosition);


            }
            if(n.GetType() == typeof(CardListener)){
                ////GD.Print("TextureRect ",n.Name," with RectPosition:",((TextureRect)n).RectPosition, " and global Position: ",((TextureRect)n).RectGlobalPosition);


            }
       }
    }

    /// <summary>
    /// Writes the given object instance to a binary file.
    /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
    /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
    /// </summary>
    /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
    /// <param name="filePath">The file path to write the object instance to.</param>
    /// <param name="objectToWrite">The object instance to write to the XML file.</param>
    /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
    public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
    {
        using (Stream stream = System.IO.File.Open(filePath, append ? FileMode.Append : FileMode.Create))
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
        }
    }

    /// <summary>
    /// Reads an object instance from a binary file.
    /// </summary>
    /// <typeparam name="T">The type of object to read from the XML.</typeparam>
    /// <param name="filePath">The file path to read the object instance from.</param>
    /// <returns>Returns a new instance of the object read from the binary file.</returns>
    public static T ReadFromBinaryFile<T>(string filePath)
    {
        using (Stream stream = System.IO.File.Open(filePath, FileMode.Open))
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (T)binaryFormatter.Deserialize(stream);
        }
    }

    /// <summary>
    /// Loads Cards from save and sets starting state
    /// </summary>
    private void PopulateCardStates(){
        //cardController = this.GetNode<CardController>("CardController");
        //this.cardController.Owner = this;
    }




    public void LoadHexGrid(){

        foreach(Node node in this.map.GetChildren())
        {
            node.QueueFree();
            this.map.RemoveChild(node);

        }
        float width = 1200;
        float height =600;



        System.Random rand = new System.Random();
        bool hasAdded = false;
        var hex = this.GetNode<HexHorizontalTest>("Hex");
        var bounds  = hex.topPoly.Polygon.Select(item => item * hex.Scale).ToArray();

        foreach(var x in this.GetNode<HexHorizontalTest>("Hex").topPoly.Polygon)
            GD.Print("sdlfjsldjflsjdljf             ", x*this.GetNode<HexHorizontalTest>("Hex").Scale);
        grid = new HexGrid(bounds);
        foreach(KeyValuePair<int,HexHorizontalTest> entry in HexGrid.storedHexes)
        {
            //GD.Print("hex position: ", entry.Value.Position);
            {
                map.AddChild(entry.Value);
                entry.Value.Position = HexGrid.storedVectors[entry.Key];

                if(entry.Value.HexEnv ==null && !hasAdded && entry.Value.GetNode<Sprite>("suelo_sin_linea").Visible && entry.Value.Position != HexGrid.lastY)
                {
                    hasAdded = true;
                    var chest = Params.LoadScene<Chest>("res://Object/GameObject/Storage/Chest.tscn");
                    chest.Position = entry.Value.Position;
                    this.env.AddChild(chest);
                }
            }
        }
        this.RemoveChild(hex);

        if(this.Name == "GameController")
        {
            HexHorizontalTest ExitTile = new HexHorizontalTest();
            //ExitTile.HexEnv
            ExitTile.Position= HexGrid.lastY;//new Vector2(grid.lastY.x+(HexMetrics.outerRadius * 1.5f), (grid.lastY.y-(HexMetrics.innerRadius * ExitTile.Scale.y)));
            this.GetNode<Sprite>("Node2D/EnvLayer/TileMap2").Position = ExitTile.Position;
            //this.GetNode<Sprite>("Node2D/EnvLayer/LogInTheWay").Position = ExitTile.Position;
            GD.Print("exit tile position: ",ExitTile.Position);
            if(HexGrid.storedHexes[HexGrid.IndexOfVec(ExitTile.Position)].HexEnv != null)
            {
                HexGrid.storedHexes[HexGrid.IndexOfVec(ExitTile.Position)].HexEnv.GetParent().RemoveChild(HexGrid.storedHexes[HexGrid.IndexOfVec(ExitTile.Position)].HexEnv);
            }

            //HexGrid.storedHexes[HexGrid.IndexOfVec(ExitTile.Position)].HexEnv = this.GetNode<Sprite>("Node2D/EnvLayer/LogInTheWay");


        }
        else{
            foreach(var v in HexGrid.storedHexes)
            {
                v.Value.topBorder.Visible = true;
            }
        }

        grid.GenEnvironment();


    }

    public void _on_ExitMainArea_body_entered(Node body)
    {

        GD.Print("exiting main area, spawning new biome");
        var biome =  biomeSpawner.SpawnGrassyBiome();
        //biome.hexLayer.Position = biome.baseLayer.Position = this.env.Position;
        Node2D node = this.GetNode<Node2D>("Node2D");
        player.Position = HexGrid.smallestX;


        foreach(Node n in this.env.GetChildren())
        {
            if(n != this.player)
                n.QueueFree();
        }

        this.GetNode<YSort>("Node2D/HexLayer").QueueFree();
        //this.GetNode<YSort>("Node2D/EnvLayer").QueueFree();
        aiController.player = player;
        aiController.SpawnEnemiesForLevel(biome);
        CallDeferred("TryMethod", biome.baseLayer, biome.hexLayer, biome.terrainLayer, node);

        GD.Print("base layer hex count: ",biome.baseLayer.GetChildCount(), " ", node);
        //this.GetNode<Node2D>("Node2D").CallDeferred("add_child",biome.hexLayer);





        // foreach(Node n in this.map.GetChildren())
        // {
        //     if(n != this.player)
        //         n.QueueFree();
        // }
        // map = this.GetNode<YSort>("Node2D/HexLayer");
        // env = this.GetNode<YSort>("Node2D/EnvLayer");
        // GD.Print("asndflnasdlf");


    }

    public void TryMethod(YSort baselayer, YSort hexlayer, YSort terrainLayer, Node2D node)
    {
        
        terrainLayer.Position = env.Position;
        terrainLayer.Name = "TerrainLayer";

        node.CallDeferred("remove_child", this.env);

        node.CallDeferred("add_child",baselayer);

        node.CallDeferred("add_child",hexlayer);

        node.CallDeferred("add_child",terrainLayer);

        CallDeferred("RepopulateEnv", terrainLayer, node);

        foreach(GameObject enemy in aiController.MasterList)
        {
            if(enemy is Enemy1)
                terrainLayer.CallDeferred("add_child", enemy);
        }
        
        //this.env = terrainLayer;


    }
    

    public void RepopulateEnv(YSort envv, Node2D node)
    {
        this.env.RemoveChild(player);
        envv.AddChild(player);
        this.env = envv;
        node.AddChild(this.env);
        // foreach(var n in envv.GetChildren())
        // {
        //     this.env.AddChild((Node2D)n);
        // }
        // node.AddChild(this.env);
    }



    public void LoadCard(){
        this.map.RemoveChild(player);
        player?.QueueFree();
        this.player = Params.LoadScene<Player>("res://Object/GameObject/Player/Player2.tscn");
        player.Connect(nameof(Player.PlayerSwordHit), this.playerController, nameof(PlayerController.On_Player_SwordHit));
        player.Connect(nameof(Player.PowerTileInteraction), this.playerController, nameof(PlayerController.On_PowerTileInteraction));
        playerController.PowerEffect = this.newUIController.GetNode<TextureRect>("Control3/PowerEffect");
        playerController.PowerAbility = this.newUIController.GetNode<TextureRect>("Control3/TextureRect");
        this.playerController.player = this.player;

        this.player.Scale = new Vector2(4f,4f);

        int index = (int)(new System.Random().NextDouble() * HexGrid.storedVectors.Count);
        ////GD.Print("getting index: ",index);
        HexHorizontalTest test = null;

        // while(player.currentTestTile == null)
        // {
        //     grid.storedHexes.TryGetValue(index, out test);
        //     if(test.Visible)
        //         player.currentTestTile = test;
        // }
        // foreach(Node n in this.env.GetChildren())
        // {
        //     n.QueueFree();
        // }
        this.env.AddChild(player);

        foreach(KeyValuePair<Vector2, Node2D> pair in HexGrid.resourcePositions)
        {
            pair.Value.Position = pair.Key;
            this.env.AddChild(pair.Value);
        }
        Camera2D cam = Params.LoadScene<Camera2D>("res://Main/Camera.tscn");
        player.AddChild(cam);
        player.Position = new Vector2(1000,1720);

        // cam.LimitRight = (int)HexGrid.biggestX.x +300;
        // cam.LimitBottom = (int)grid.biggestY.y +300;
        // cam.LimitLeft = cam.LimitRight - 1920;
        // cam.LimitTop = cam.LimitBottom - 1080;
        player.Position = HexGrid.smallestX;
        player.Connect("ExitMainArea", this, nameof(On_ExitMainArea));


        if(this.Name == "GameController2")
        {
           // this.playerController.state = ControllerInstance.ControllerState.BattleInput;
            int biggestYIndex = HexGrid.IndexOfVec(grid.biggestY);
            int smallestYIndex = HexGrid.IndexOfVec(HexGrid.lastY);
            GD.Print("index of biggestY (",grid.biggestY,") is ",HexGrid.IndexOfVec(grid.biggestY));

            GD.Print("index of lastY (",HexGrid.lastY,") is ",HexGrid.IndexOfVec(HexGrid.lastY));
            player.currentTestTile = HexGrid.storedHexes[biggestYIndex];
            player.Position  = grid.biggestY;//player.currentTestTile.Position;

            Skeleton skeleton = Params.LoadScene<Skeleton>("res://Assets/Sprites/Skeleton/Node2D.tscn");
            skeleton.Position = HexGrid.lastY;
            skeleton.currentHex = HexGrid.storedHexes[smallestYIndex];
            this.env.AddChild(skeleton);
            cam.LimitRight = (int)player.Position.x + 400 ;
            cam.LimitBottom = (int)player.Position.y + 400 ;

            GD.Print("player: ",player.Position, " skeleton: ", skeleton.Position);


            BattleController battle = new BattleController();
            battle.TurnLabel = this.newUIController.GetNode<Label>("Control3/BattleControl/TurnLabel");
            battle.skeleton = skeleton;
            battle.grid = grid;
            battle.player = this.playerController;
            battle.turn = "skeleton";
            gameState = State.GameState.Wait;
            battle.battleState = BattleController.BattleState.TurnChange;
            this.AddChild(battle);




        }



        //player.Position = test.Position;

        //cam.Position = player.Position;

        ////GD.Print("Getting tile: ", test);

        //path = new Line2D();
        // var testObj = ReadFromBinaryFile<SaveInstance>("D:/testfile.txt");
        //this.AddChild(path);

        // //////GD.Print(n.player.animationState);
        // this.hexMap = (NewHexMapTests)SaveLoader.LoadGame(testObj, this);
        // ////GD.Print("Player current tile: ",this.hexMap.player.currentTile);

        // foreach(HexCell1 cell in this.hexMap.tiles.Keys){
        //     ////GD.Print(cell);
        // }


        // ////GD.Print(testObj, "  ",testObj.childBook);
    }

    public void On_ExitMainArea()
    {
        this.GetNode<Node2D>("Node2D").CallDeferred("remove_child",this.map);
        this.GetNode<Node2D>("Node2D").CallDeferred("remove_child",this.env);
        this.CallDeferred("PopulateBattleMap");

    }

    // public void PopulateBattleMap()
    // {
    //     this.map = new YSort();
    //     this.GetNode<Node2D>("Node2D").AddChild(this.map);


    //     // foreach(Node node in this.map.GetChildren())
    //     // {
    //     //     this.map.RemoveChild(node);
    //     //     //node.QueueFree();
    //     // }

    //     //this.GetNode<Node2D>("Node2D").CallDeferred("remove_child",this.env);
    //     this.env = new YSort();
    //     this.GetNode<Node2D>("Node2D").AddChild(this.env);
    //     // foreach(Node node in this.env.GetChildren())
    //     // {
    //     //     this.env.RemoveChild(node);
    //     //     //node.QueueFree();
    //     // }

    //     float width = 1200;
    //     float height = 700;

    //     System.Random rand = new System.Random();

    //     BattleGrid = new HexGrid(new Vector2(width,height), map, this.GetNode<ReferenceRect>("ResourceArea/ReferenceRect"), true);
    //     foreach(KeyValuePair<int,HexHorizontalTest> entry in this.BattleGrid.storedHexes)
    //     {
    //         {
    //             map.AddChild(entry.Value);
    //             entry.Value.Position = HexGrid.storedVectors[entry.Key];
    //         }
    //     }
    //    // this.RemoveChild(Fire)
    //     FireController c = new FireController();
    //     this.AddChild(c);
    //     this.fireController = c;
    //     this.spellController.fire = c;
    //     foreach(var v in BattleGrid.storedHexes)
    //     {
    //         GD.Print(v);
    //         v.Value.topBorder.Visible = true;
    //     }
    //     player.GetParent().RemoveChild(player);
    //     this.env.AddChild(player);

    //     //this.playerController.state = ControllerInstance.ControllerState.BattleInput;
    //     int biggestYIndex = HexGrid.IndexOfVec(BattleGrid.biggestY);
    //     int smallestYIndex = HexGrid.IndexOfVec(HexGrid.lastY);
    //     GD.Print("index of biggestY (",BattleGrid.biggestY,") is ",HexGrid.IndexOfVec(BattleGrid.biggestY));

    //     GD.Print("index of lastY (",HexGrid.lastY,") is ",HexGrid.IndexOfVec(HexGrid.lastY));
    //     player.currentTestTile = BattleGrid.storedHexes[biggestYIndex];
    //     player.Position  = BattleGrid.biggestY;//player.currentTestTile.Position;

    //     Skeleton skeleton = Params.LoadScene<Skeleton>("res://Assets/Sprites/Skeleton/Node2D.tscn");
    //     skeleton.Position = HexGrid.lastY;
    //     skeleton.currentHex = BattleGrid.storedHexes[smallestYIndex];
    //     this.env.AddChild(skeleton);
    //     //cam.LimitRight = (int)player.Position.x + 400 ;
    //     //cam.LimitBottom = (int)player.Position.y + 400 ;

    //     GD.Print("player: ",player.Position, " skeleton: ", skeleton.Position);

    //     foreach(KeyValuePair<Vector2, Node2D> pair in BattleGrid.resourcePositions)
    //     {
    //         pair.Value.Position = pair.Key;
    //         this.env.AddChild(pair.Value);
    //     }


    //     playerController.ManaBar = this.ManaBar;
    //     playerController.HealthBar = this.HealthBar;
    //     spellController.environmentLayer = this.env;
    //     spellController.spellState = SpellController.SpellState.Battle;
    //     BattleController battle = new BattleController();
    //     spellController.Connect("FireSpellCompleted", battle, "On_FireSpellCompleted");
    //     battle.TurnLabel = this.newUIController.GetNode<Label>("Control3/BattleControl/TurnLabel");
    //     battle.skeleton = skeleton;
    //     battle.grid = BattleGrid;
    //     battle.fireController = this.fireController;
    //     battle.spellController = this.spellController;
    //     battle.newUIController = this.newUIController;
    //     battle.player = this.playerController;
    //     battle.turn = "skeleton";
    //     gameState = State.GameState.Wait;
    //     battle.battleState = BattleController.BattleState.TurnChange;
    //     this.AddChild(battle);
    //     CardController cControl  = Params.LoadScene<CardController>("res://Object/Controller/CardController.tscn");
    //     this.newUIController.GetNode<Control>("Control3/CardControllerContainer").AddChild(cControl);
    //     cControl.Position = new Vector2(959,-57);
    //     //this.newUIController.

    //     //EmitSignal("ChangeScene");
    //     //this.GetTree().ChangeSceneTo(packedScene);
    // }
}

