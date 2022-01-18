using Godot;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

using System.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections;

public class GameController : Node, ControllerBase

{   
    public static Rect2 envBounds = new Rect2(new Vector2(1273,436),new Vector2(1712,1213));
    private static bool freed = true;

    public List<CharacterController> CharacterControllers;

    public static Control WoodLabel, StoneLabel, EssenceLabel,GrassLabel;
    private CardController cardController;
    private CanvasLayer canvasLayer;

    private Timer InputFlush;

    private NewHexMapTests hexMap;

    public BuildController buildController;

    public static State.GameState gameState = State.GameState.Default;
    
    private Polygon2D poly;
    private Card c;

    private YSort map, env;

    private SaveInstance s;

    private Button saveButton;

    private HexGrid grid;

    private PlayerController playerController;


    public Player player;

    [PostLoad]
    private YSort tileMap;

    [PostLoad]
    public Dictionary<HexCell1,int> tiles;

    [PostLoad]
    Line2D path;

    [PostLoad]
    private HexCell1 startingTile;

    private NewUIController newUIController;
    private ResourcePostController resourcePostController;

    private List<Node> toFree = new List<Node>();

    private Interactable currentInteractive = null;

    private static string test = "this is a test";


    private Building BuildingAction;
    
    private List<BuildingModel> buildingModels;
    public GameController(){
        PopulateCardStates();
        CharacterControllers = new List<CharacterController>();
    }
 
    
    public override void _Ready()
    {   
        buildingModels = CSVReader.LoadBuildingCSV();
        CSVReader.LoadResourceCosts(buildingModels.Cast<AbstractObjectModel>().ToList());

        LoadUI();
        
        //cardController = this.GetNode<CardController>("UIController/Control/CardController");
        //canvasLayer = this.GetNode<CanvasLayer>("UIController");
        hexMap = this.GetNode<NewHexMapTests>("NewHexMapTests");
        saveButton = this.GetNode<Button>("UIController/Control/Button");
        map = this.GetNode<YSort>("Node2D/HexLayer");
        env = this.GetNode<YSort>("Node2D/EnvLayer");
        playerController = new PlayerController();
        playerController.grid = this.grid;
        this.AddChild(playerController);
        this.playerController.Connect("TriggerInteractive", this,nameof(On_TriggerInteractive));
        this.playerController.Connect("TriggerBuildingAction", this, nameof(On_TriggerBuildingAction));
        this.SaveCard();
        this.LoadCard();
        
        WoodLabel = GetNode<Control>("CanvasLayer/Control3/VBoxContainer/TextureRect/WoodLabel");
        
        StoneLabel = GetNode<Control>("CanvasLayer/Control3/VBoxContainer/TextureRect/StoneLabel");
        
        EssenceLabel = GetNode<Control>("CanvasLayer/Control3/VBoxContainer/TextureRect/EssenceLabel");
        GrassLabel = GetNode<Control>("CanvasLayer/Control3/VBoxContainer/TextureRect/GrassLabel");

        this.GetNode<Timer>("CanvasLayer/Control3/VBoxContainer/TextureRect/TimerLabel/Timer").Start(500);
        //this.resourcePostController = new ResourcePostController();
       // this.AddChild(resourcePostController);
    } 

    private void LoadUI(){
        newUIController = GetNode<NewUIController>("CanvasLayer");
        newUIController.Connect("TriggerBuild", this, nameof(TriggerBuild));
        newUIController.buildingModels = this.buildingModels;
        newUIController.RunUI();
        newUIController.Connect("BuildingMenuExited", this, nameof(On_BuildMenuExited));
    }

    public void On_TriggerBuildingAction(Building model)
    {
        GD.Print("On_TriggerBuildingAction");
        gameState = State.GameState.BuildingAction;
        this.playerController.state = ControllerInstance.ControllerState.Wait;
        this.BuildingAction = model;
    }

    /// <summary>
    /// When a player clicks on an interactable object. Is emitted via PlayerController
    /// </summary>
    /// <param name="itv"></param>
    public void On_TriggerInteractive(Interactable itv)
    {
        //GD.Print("On_TriggerInteractive", itv);
        currentInteractive = itv;
        this.RemoveChild(this.GetNode<Area2D>("Node2D/EnvLayer/Light2D"));
        this.newUIController.GetNode<Control>("Control3/FoundBook").Visible = true;
        this.newUIController.GetNode<Button>("Control3/FoundBook/TextureRect/Button").Connect("pressed", this, nameof(OnTriggerAccept));
    }

    public void OnTriggerAccept()
    {
        this.newUIController.GetNode<Control>("Control3/FoundBook").Visible = false; 
        this.newUIController.GetNode<BuildingIcon>("Control3/Control/BuildingIcon").button.Disabled =false;
    }

    public void TriggerBuild(string buildingName)
    {
        foreach(KeyValuePair<int, HexHorizontalTest> p in grid.storedHexes){
            p.Value.topBorder.Visible =true;
        }

        var building = buildingModels.FirstOrDefault(item => item.Name == buildingName);

        BuildBuilding(building);
    }

    public void BuildBuilding(BuildingModel building)
    {
        ////GD.Print("processing tent click");
        gameState = State.GameState.Build;
        //Resource tent = ResourceLoader.Load("res://Assets/UI/pointer.bmp");
        if(this.buildController!= null){
            this.RemoveChild(this.buildController);
            
        }
        BuildController b = new BuildController();
        b.Connect("BuildCancel", this, nameof(TriggerBuildCancel));

        Sprite r = new Sprite();
        r.Texture = Params.LoadScene<TextureButton>(building.TextureResource).TextureNormal;

        this.AddChild(b);
        this.MoveChild(b,0);
        //this.AddChild(r);
        b.AddChild(r);
        b.buildSprite= r;
        b.grid = this.grid;
        b.playerController = this.playerController;
        b.model = building;
        b.player = this.player;
        r.Scale = new Vector2(3,3);
        r.ZIndex = 50;
        b.environmentLayer = this.env;
        this.buildController = b;
    }
    

    
    public void TriggerBuildCancel(BuildController controller)
    {
        ////GD.Print("processing build cancel");
        gameState = State.GameState.Default;
        this.RemoveChild(controller);
        controller.QueueFree();
        
    }

    public override void _Process(float delta)
    {
        
    }


    public void On_BuildMenuExited(){
        this.playerController.state = ControllerInstance.ControllerState.AcceptAllInput;
    }

    public override void _PhysicsProcess(float delta)
    {   
        UpdateResourceValues();
        
        //finished frame continue, reset input state
        if(gameState == State.GameState.Continue)
        {
            gameState = State.GameState.Default;
            this.playerController.state = ControllerInstance.ControllerState.AcceptAllInput;
            
        }
        //we are in building menu
        if(this.newUIController?.currentMenu != null && this.newUIController?.currentMenu != this.newUIController?.mainMenu){
            UpdateBuildingButtonStates();
            this.playerController.state = ControllerInstance.ControllerState.AcceptPartialInput;
        }

        if(this.buildController?.buildState == State.BuildState.BuildFinish){
            ParseBuildingFinish();
            this.buildController.buildState = State.BuildState.Default;
            gameState = State.GameState.Continue;
        }
         if(gameState == State.GameState.Wait){

        }
        
         if(gameState == State.GameState.BuildingAction)
        {
            //set input to wait for building action to complete
            ParseBuildingAction();
            gameState = State.GameState.Wait;
            this.playerController.state = ControllerInstance.ControllerState.Wait;
        }

        
    }

    private void ParseBuildingFinish()
    {
        if(this.buildController.model.Name == "Gathering Post")
        {
            
            OptionBox b = this.newUIController.CreateOption("Update worker amount at this Resource Post");
            this.resourcePostController = new ResourcePostController(b,grid, buildController.ConstructedBuilding);
            
            this.resourcePostController.Connect("CloseDialog",this,nameof( On_ResourcePostCloseDialog));
            this.resourcePostController.Connect("ResourceDrop", this, nameof(AddBulkResource));
            this.AddChild(resourcePostController);
            
            //Character c = SpawnRandomCharacter();
            resourcePostController.AvailableWorkers.Add(SpawnRandomCharacter());

        }
    }

    public void On_ResourcePostCloseDialog(){
        SetContinueState();
    }

    private void ParseBuildingAction()
    {
        if(BuildingAction.model.Name == "Leaf-Sleep-Roll")
        {
            //GD.Print("Creating option box and connecting");
            this.newUIController.CreateOption("Do you want to rest and recover?").Connect("OnResponse",this,nameof(BuildingOption));
            
        }
        else if(BuildingAction.model.Name == "Gathering Post")
        {
            
            this.resourcePostController.OnClick();
            //SetContinueState();
        }
    }

    private CharacterController SpawnRandomCharacter(){
        Character character = Params.LoadScene<Character>("res://Object/GameObject/Characters/Character.tscn");
        //character.Position = playerController.player.Position;
        character.Scale = player.Scale;
        GD.Print("player position: ", this.playerController.player.Position);
        CharacterController c1 = new CharacterController();

        c1.character = character;

        foreach(KeyValuePair<int, HexHorizontalTest> p in grid.storedHexes){
            if(p.Key>50 && !p.Value.isBasicResource){
                c1.character.Position = p.Value.Position;
                break;
            }
        }
        this.env.AddChild(c1.character);
        this.AddChild(c1);
        CharacterControllers.Add(c1);
        
        GD.Print("spawning character at: ",character.Position);
        GD.Print(character.ZIndex);
        GD.Print(character.Scale);
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
        }
        else{
            SetContinueState();                      
        }
        if(option != null){
            GD.Print("removing current option: ", option);
            this.newUIController.RemoveChild(option);
            option.QueueFree();
        }
    }
    
    public void UpdateBuildingButtonStates()
    {
        foreach(var item in this.newUIController.buildingButtons)
        {
            CheckCanBuild(item);
        }
    }

    private void CheckCanBuild(KeyValuePair<string, TextureButton> button){
        ////GD.Print("Checking if can build");
        ResourceCost buildingCost = buildingModels.First(item => item.Name == button.Key).RequiredResources;

        //make sure that the resource cost of building is <= stored resources.
        if(buildingCost.GetResourceCosts().All(item => this.playerController.ResourceUpdate.ContainsKey(item.Key) 
            && this.playerController.ResourceUpdate[item.Key] >= item.Value))
            {               
                button.Value.Disabled =  false;
            }
            else{
                button.Value.Disabled =  true;
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
            }if(pair.Key == "Rock")
                StoneLabel.GetNode<Label>("Label").Text = "Stone: "+ pair.Value;
            
        //     woodp.GetNode<Label>("Label").Text = "+1 Leaves";
        }//     woodp.GetNode<Label>("Label").Text = "+1 Wood";
        //     ResourceUpdate.Add("Wood", ++WoodAmount);
    }

    public void AddBulkResource(BasicResource res, int amount){
        GD.Print("Addin bulk resource of type: ",res.ResourceType, " with amount: ", amount);
        if(res.ResourceType == "Wood"){
            this.playerController.ResourceUpdate["Wood"]   = this.playerController.ResourceUpdate["Wood"]  + amount;
            WoodLabel.GetNode<Label>("Label").Text = "Wood: "+ this.playerController.ResourceUpdate["Wood"];
        }
        if(res.ResourceType == "Leaves"){
            this.playerController.ResourceUpdate["Leaves"]  = this.playerController.ResourceUpdate["Leaves"] + amount;
            GrassLabel.GetNode<Label>("Label").Text = "Leaves: "+ this.playerController.ResourceUpdate["Leaves"];
            
        }
        if(res.ResourceType == "Rock"){
            this.playerController.ResourceUpdate["Stone"] = this.playerController.ResourceUpdate["Stone"] + amount;
            StoneLabel.GetNode<Label>("Label").Text = "Stone: "+ this.playerController.ResourceUpdate["Stone"];
        

        }

    }

    public void RecursiveChildPrint(Node node){
        foreach(Node n in node.GetChildren()){
            RecursiveChildPrint(n);
            if(n.GetType() == typeof(TextureRect)){
                //GD.Print("TextureRect ",n.Name," with RectPosition:",((TextureRect)n).RectPosition, " and global Position: ",((TextureRect)n).RectGlobalPosition);
               

            }
            if(n.GetType() == typeof(CardListener)){
                //GD.Print("TextureRect ",n.Name," with RectPosition:",((TextureRect)n).RectPosition, " and global Position: ",((TextureRect)n).RectGlobalPosition);
               

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
    
    
    public void ThreadProc(){
        foreach(KeyValuePair<int,HexHorizontalTest> entry in this.grid.storedHexes)
        {
            map.AddChild(entry.Value);
            entry.Value.Position = grid.storedVectors[entry.Key];
            ////GD.Print("adding hex: ", entry.Value, " at position: ",grid.storedVectors[entry.Key]);
            //System.Threading.Thread.Sleep(300);
        }
    }
    public void SaveCard(){

        //this.GetNode<Node>("Area2D").RemoveChild(poly);
        
        foreach(Node node in this.map.GetChildren())
        {
            node.QueueFree();
            this.map.RemoveChild(node);

        }
        float width = 3000;
        float height = 3000;

        System.Random rand = new System.Random();
        //height = (float)rand.NextDouble()*height;
        //width = (float)rand.NextDouble()*width;
        grid = new HexGrid(new Vector2(width,height), map, this.GetNode<ReferenceRect>("ResourceArea/ReferenceRect"));
        
        ThreadProc();
        //GenEnvironment();
        //poly = new Polygon2D();
       // poly.Polygon = grid.BoundingBox();
        //this.GetNode<Node>("Area2D").AddChild(poly);
       // System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc));
        //t.Start();

    }
  
    public void GenEnvironment()
    {
        System.Random rand = new System.Random();
        foreach(Node n in this.env.GetChildren())
            env.RemoveChild(n);
       // height = (float)rand.NextDouble()*height;
        foreach(Vector2 vec in grid.storedVectors){
            if(rand.NextDouble()>0.3)
            {
                HexHorizontalTest test = null;
                grid.storedHexes.TryGetValue(grid.storedVectors.IndexOf(vec), out test);
                if(test.Visible)
                {   
                    YSort grass = Params.LoadScene<YSort>("res://Assets/Environment/EnvHex.tscn");
                    this.env.AddChild(grass);
                    grass.Position = vec;

                    if(rand.NextDouble() > 0.5)
                    {
                        YSort fire  = Params.LoadScene<YSort>("res://Assets/Sprites/Environment/FireCell.tscn");
                        
                        this.env.AddChild(fire);
                        fire.Position = grass.Position;
                    }
                }
            }

        }
    }
    public void LoadCard(){
        this.map.RemoveChild(player);   
        player?.QueueFree();     
        this.player = Params.LoadScene<Player>("res://Object/GameObject/Player/Player.tscn");
        this.playerController.player = this.player;
        
        this.player.Scale = new Vector2(2f,2f);

        int index = (int)(new System.Random().NextDouble() * this.grid.storedVectors.Count);
        //GD.Print("getting index: ",index);
        HexHorizontalTest test = null;

        // while(player.currentTestTile == null)
        // {
        //     grid.storedHexes.TryGetValue(index, out test);
        //     if(test.Visible)
        //         player.currentTestTile = test;
        // }
        foreach(Node n in this.env.GetChildren())
        {
            n.QueueFree();
        }
        this.env.AddChild(player);

        foreach(KeyValuePair<Vector2, Node2D> pair in grid.resourcePositions)
        {
            pair.Value.Position = pair.Key;
            this.env.AddChild(pair.Value);
        }
        player.Position = new Vector2(1000,1720);
       
        //player.Position = test.Position;
        Camera2D cam = Params.LoadScene<Camera2D>("res://Main/Camera.tscn");
        player.AddChild(cam);
        //cam.Position = player.Position;
 
        //GD.Print("Getting tile: ", test);
        
        //path = new Line2D();
        // var testObj = ReadFromBinaryFile<SaveInstance>("D:/testfile.txt");
        //this.AddChild(path);
        
        // ////GD.Print(n.player.animationState);
        // this.hexMap = (NewHexMapTests)SaveLoader.LoadGame(testObj, this);
        // //GD.Print("Player current tile: ",this.hexMap.player.currentTile);

        // foreach(HexCell1 cell in this.hexMap.tiles.Keys){
        //     //GD.Print(cell);
        // }

        
        // //GD.Print(testObj, "  ",testObj.childBook);
    }
}
    
