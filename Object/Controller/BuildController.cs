
using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
public class BuildController: Node
{   
    [Signal]
    public delegate void BuildCancel(BuildController controller);
    //[Signal]
    //public delegate void BuildBuildingComplete(Building b);

    public Node2D buildSprite;
    public BuildingModel selectedModel = null;

    bool something = true;

    private Building selectedBuilding = null;


    

    public List<BuildingModel> buildingModels;

    public enum BuildingState 
    {
        BuildMenu,
        BuildMode,
        BuildFinished,
        BuildPending,
        BuildFail,
        Default,
        BuildMenuExited,
        BuildingSelected,
    }

    public BuildingControllerUI buildUI;
    public BuildingState buildingState = BuildingState.Default;


    

    public override void _Ready()
    {

    }
    public BuildController(BuildingControllerUI buildUI)
    {
        buildingModels = CSVReader.LoadBuildingCSV();
        CSVReader.LoadResourceCosts(buildingModels.Cast<AbstractObjectModel>().ToList());
        this.buildUI = buildUI;
        var menu  = this.buildUI.LoadMainBuildMenu();
        var btn = buildUI.CreateMenuExitButton();
        //btn.Connect(nameof(BuildingControllerUI.BuildingMenuExited),this, nameof(On_BuildingMenuExited));
        menu.AddChild(btn);
        menu.Visible =false;
        LoadBuildingUIMenus();
        this.buildUI.Connect(nameof(BuildingControllerUI.BuildingMenuExited), this, nameof(On_BuildingMenuExited));
    }

    public BuildController()
    {
        buildingModels = CSVReader.LoadBuildingCSV();
        CSVReader.LoadResourceCosts(buildingModels.Cast<AbstractObjectModel>().ToList());
    }


    public void UpdateBuildingButtonStates(Dictionary<string, int> Resources)
    {
        foreach(var item in this.buildUI.buildingButtons)
        {
            CheckCanBuild(item, Resources);
        }
    }

    private void CheckCanBuild(KeyValuePair<string, TextureButton> button,Dictionary<string, int> Resources){
        ResourceCost buildingCost = buildingModels.First(item => item.Name == button.Key).RequiredResources;
        
        
        //make sure that the resource cost of building is <= stored resources.
        if(buildingCost.GetResourceCosts().All(item => Resources.ContainsKey(item.Key) 
            && Resources[item.Key] >= item.Value))
            {    
                  
                button.Value.Disabled =  false;
                //button.Value.Visible = false;
            }
            else
            {
                button.Value.Disabled =  true;
            }
    }
    /// <summary>
    /// Method that is triggered when a building button, that the player has resources for, is pressed.
    /// Triggers build mode and Build state
    /// </summary>
    /// <param name="button"></param>
    public void BuildingButtonPressed(string buildingName)
    {
        var buildingModel = buildingModels.FirstOrDefault(item => item.Name == buildingName);
        //can't do anything with a null building
        if(buildingModel == null)
            return;
        selectedModel = buildingModel;
        Sprite tex = new Sprite();
        tex.Texture = Params.LoadScene<TextureButton>(buildingModel.TextureResource).TextureNormal;  
        AddChild(tex);
        tex.ZIndex =6;
        buildSprite = tex;
        //model = buildingModel;
        buildSprite.Scale = new Vector2(3,3);
        buildSprite.ZIndex = 50;   


        this.buildingState = BuildingState.BuildMode;
    }



    public void LoadBuildingUIMenus()
    {
        BuildMenu menu = Params.LoadScene<BuildMenu>("res://Object/UI/BuildMenu.tscn");
        BuildMenu menu2 = Params.LoadScene<BuildMenu>("res://Object/UI/BuildMenu.tscn");

        foreach(BuildingModel model in buildingModels)
        {
            BuildingIcon icon;
            if(model.Type == "Housing")
            {
                icon = buildUI.CreateMenuItem(menu, model.Name, model.RequiredResources.GetFormattedResource(),model.TextureResource);
            }
            else 
            {              
                icon = buildUI.CreateMenuItem(menu2, model.Name, model.RequiredResources.GetFormattedResource(),model.TextureResource);
            }
 
            icon.Connect("ButtonPress", this, nameof(BuildingButtonPressed));
        
        }
        

        //PUT THIS SOMEWHERE ELSE IT HURTS TO LOOK AT
        menu.Columns = menu.GetChildCount() +1;
        menu2.Columns = menu2.GetChildCount()+1;
        buildUI.resourceMenu = menu2;
        buildUI.housingMenu = menu;
        buildUI.AddChild(buildUI.housingMenu);
        buildUI.AddChild(buildUI.resourceMenu);
        Button btn  = buildUI.CreateMenuExitButton();
        //btn.Connect(nameof(BuildingControllerUI.BuildingMenuExited),this, nameof(On_BuildingMenuExited));
        Button btn2  = buildUI.CreateMenuExitButton();
        //btn2.Connect(nameof(BuildingControllerUI.BuildingMenuExited),this, nameof(On_BuildingMenuExited));
        buildUI.housingMenu.AddChild(btn);
        buildUI.resourceMenu.AddChild(btn2);
       // this.housingMenu.AddChild(exitButton);
        //////GD.Print(exitButton.si)
        buildUI.housingMenu.Visible =false;
        buildUI.resourceMenu.Visible = false;
    }
    


    // public BuildController(BuildingModel buildingModel)
    // { 
    //     Sprite tex = new Sprite();
    //     tex.Texture = Params.LoadScene<TextureButton>(buildingModel.TextureResource).TextureNormal;  
    //     AddChild(tex);
    //     buildSprite= tex;
    //     model = buildingModel;
    //     buildSprite.Scale = new Vector2(3,3);
    //     buildSprite.ZIndex = 50;
        
    // }

    

    public void SetNodeParams(Node2D node){
        node.Scale = buildSprite.Scale;
        node.Position = buildSprite.Position;
        node.ZIndex = 0;
    }

    public void On_BuildingMenuExited()
    {
        this.buildUI.Visible = false;
        this.buildUI.selectedMenu = null;
        this.buildingState = BuildingState.BuildMenuExited;
        GD.Print("Setting building state to exited");
    }

    public void SetSelectedBuilding(Building b)
    {
        selectedBuilding = b;
        buildingState = BuildingState.BuildingSelected;
    }
    public override void _PhysicsProcess(float delta)
    {
        switch(buildingState)
        {
            case BuildingState.BuildMenu:
                HandleBuildMenuState();
                break;
            case BuildingState.BuildMode:
                HandleBuildModeState();
                break;
            case BuildingState.BuildingSelected:
                if(selectedBuilding!= null && !selectedBuilding.IsUIOpen())
                {
                    if(selectedBuilding.HasChanges())
                    {
                        var bud = new BuildingUpdateInteraction(selectedBuilding);
                        GameUpdateQueue.TryPushUpdate(bud);
                    }
                }
                break;
        }
        
    }

    public void BuildCompleted()
    {
        this.RemoveChild(buildSprite); 
        buildSprite.ZIndex = 0;
        selectedModel = null;
        buildSprite = null;
        this.buildingState = BuildingState.BuildMenu;
        GD.Print("BuildComplete");
    }


    public void HandleBuildMenuState()
    {
        //if we have no current menu, then we know we need to open main menu
        if(this.buildUI.selectedMenu == null)
        {
            
            this.buildUI.Visible = true;
            GD.Print("Setting menu to being something becausse selcted is null");//GridContainer n  = this.GetNode<GridContainer>("CanvasLayer/Main Building Menu");
            
            buildUI.selectedMenu = this.buildUI.mainMenu; 
            buildUI.selectedMenu.Visible =true;
            GD.Print(buildUI.selectedMenu);
            GD.Print(buildUI.mainMenu);
            GD.Print(buildUI.selectedMenu.RectPosition);
            Tween tween = new Tween();
            tween.InterpolateProperty(buildUI.selectedMenu, "rect_position", buildUI.selectedMenu.RectPosition, new Vector2(buildUI.selectedMenu.RectPosition.x, buildUI.selectedMenu.RectPosition.y - 150), 0.08f, Tween.TransitionType.Quart, Tween.EaseType.Out);
            this.AddChild(tween);
            tween.Start();
            tween.Dispose();
            //buildingState = BuildingState.BuildMode;
        }

    }

    private void HandleBuildModeState()
    {
        if(Input.GetActionStrength("right_click")>0)
        {
            buildSprite?.QueueFree();
            buildingState = BuildingState.BuildMenu;
        }
        else 
        {
            if(HexGrid.hoveredHex !=null){
                buildSprite.Position = HexGrid.hoveredHex.Position;
            }
            if(Input.IsActionJustPressed("left_click"))
            {
                //TODO - check for valid amount of resources
                HandleBuild(null);             
            }
        }
    }


    private void HandleBuild (BuildingModel buildingModel)
    {
        Building b = Params.LoadScene<Building>("res://Object/GameObject/Buildings/" + selectedModel.Name +".tscn");
        GD.Print("Building : ",b);       
    
        if(selectedModel.Name == "Fire")
        {
            b.buildingAsset = Params.LoadScene<AnimatedSprite>("res://Assets/Sprites/20.09 - Traveler's Camp 1.1/StaticFire.tscn");
            SetNodeParams(b.buildingAsset);
        }
        else
            b.buildingAsset  = buildSprite;
        
        
        b.model = this.selectedModel;
        var intr = new BuildCompleteInteraction(b); 
        GameUpdateQueue.TryPushUpdate(intr);
            
        this.buildingState = BuildingState.BuildPending;    
    }
}