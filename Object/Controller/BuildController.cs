
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

    //bool something = true;

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

    //public BuildingControllerUI buildUI;

    public BuildingUIMenu buildingUIMenu;
    public BuildingState buildingState = BuildingState.Default;


    public Dictionary<BuildingModel, BuildMenu> buildingModelToMenu = new Dictionary<BuildingModel, BuildMenu>();

    public override void _Ready()
    {

    }
    // public BuildController(BuildingControllerUI buildUI)
    // {
    //     GD.Print("loading building models ytknow");
    //     buildingModels = CSVReader.LoadBuildingCSV();
    //     CSVReader.LoadResourceCosts(buildingModels.Cast<AbstractObjectModel>().ToList());
    //     this.buildUI = buildUI;
    //     //var menu  = this.buildUI.LoadMainBuildMenu();
    //     //var btn = buildUI.CreateMenuExitButton();
    //     //btn.Connect(nameof(BuildingControllerUI.BuildingMenuExited),this, nameof(On_BuildingMenuExited));
    //     menu.AddChild(btn);
    //     menu.Visible =false;
    //     LoadBuildingUIMenus();
    //     this.buildUI.Connect(nameof(BuildingControllerUI.BuildingMenuExited), this, nameof(On_BuildingMenuExited));
    // }

    public BuildController(BuildingUIMenu buildingUIMenu)
    {
        GD.Print("loading building models ytknow");
        buildingModels = CSVReader.LoadBuildingCSV();
        CSVReader.LoadResourceCosts(buildingModels.Cast<AbstractObjectModel>().ToList());
        this.buildingUIMenu = buildingUIMenu;
        //var menu  = this.buildUI.LoadMainBuildMenu();
        //var btn = buildUI.CreateMenuExitButton();
        //btn.Connect(nameof(BuildingControllerUI.BuildingMenuExited),this, nameof(On_BuildingMenuExited));
        LoadBuildingUIMenus();
        //this.buildUI.Connect(nameof(BuildingControllerUI.BuildingMenuExited), this, nameof(On_BuildingMenuExited));
    }

    public BuildController()
    {
        buildingModels = CSVReader.LoadBuildingCSV();
        CSVReader.LoadResourceCosts(buildingModels.Cast<AbstractObjectModel>().ToList());
    }


    public void UpdateBuildingButtonStates(Dictionary<string, int> resources)
    {
        var selectedModels = buildingModelToMenu.Where(item => item.Value == buildingUIMenu.currentMenu);

        foreach(var item in buildingModelToMenu)
        {
            var menu = item.Value;
            var model = item.Key;
            var resCost = model.RequiredResources.ResourceCostList;
            
            //var s = "";
            //  foreach(var thing in resCost)
            //      s +=thing;
            //  GD.Print("building cost: ",s);
            //  s= "";
            //  foreach(var thing in resources)
            //      s +=thing;
            //  GD.Print("resources: ",s);

            bool enoughRes = resCost.All(cost => resources.ContainsKey(cost.Key)  && resources[cost.Key] >= cost.Value);

            var btn = menu.icons.FirstOrDefault(icon => icon.IconName == model.Name)?.button;
            if(btn !=null)
            {

                btn.Disabled = !enoughRes;
                    //btn.SelfModulate = new Color(1,1,1,0.5f);
     
                btn.SelfModulate = btn.Disabled ? new Color(1,1,1,0.5f) : new Color(1,1,1,1f);
               // GD.Print("setting model ",model.Name, " to: ", btn.SelfModulate);
                btn.Update();
                
            }
            
            

        }
        // foreach (var model in buildingModelToMenu)
        // {
        //     var resCost = model.Key.RequiredResources.ResourceCostList;

        //     //check that the resource costs for this mode has all applicable resources, and those 
        //     //resources are enough
        //     bool enoughRes = resCost.All(item => resources.ContainsKey(item.Key)  && resources[item.Key] >= item.Value);
            
        //     var btn = buildingUIMenu.menus.Join currentMenu.icons.FirstOrDefault(item =>item.IconName == model.Key.Name);

        //     if(btn !=null)
        //         btn.button.Disabled = !enoughRes;
        // }

        //foreach(var item in this.buildingUIMenu.menus)
        //{
            //for()
           // CheckCanBuild(item, Resources);
        //}

    }

    public BuildingModel FetchModel(string name)
    {
        return buildingModels.FirstOrDefault(b => b.Name == name);
    }

    // private void CheckCanBuild(KeyValuePair<string, TextureButton> button,Dictionary<string, int> Resources){

    //     //var models = buildingModels.Where(item => buildingUIMenu.currentMenu.icons.Exists(icon => icon.IconName == item.Name)).ToList();     
    //     //make sure that the resource cost of building is <= stored resources.
    //     if(buildingCost.GetResourceCosts().All(item => Resources.ContainsKey(item.Key) 
    //         && Resources[item.Key] >= item.Value))
    //         {    
                  
    //             button.Value.Disabled =  false;
    //             //button.Value.Visible = false;
    //         }
    //         else
    //         {
    //             button.Value.Disabled =  true;
    //         }
    // }
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
        try{
            tex.Texture = Params.LoadScene<TextureButton>(buildingModel.TextureResource).TextureNormal;  
        }catch(Exception e)
        {
            tex.Texture = Params.LoadScene<TextureButton>("res://Assets/Sprites/20.09 - Traveler's Camp 1.1/ImageNotFound.tscn").TextureNormal;  
        }
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
        BuildMenu livingMenu = Params.LoadScene<BuildMenu>("res://Object/UI/BuildMenu.tscn","Living");
        
        BuildMenu resourceMenu = Params.LoadScene<BuildMenu>("res://Object/UI/BuildMenu.tscn","Resource");
        BuildMenu magicMenu = Params.LoadScene<BuildMenu>("res://Object/UI/BuildMenu.tscn","Magic");
        BuildMenu techMenu = Params.LoadScene<BuildMenu>("res://Object/UI/BuildMenu.tscn","Technology");

        foreach(BuildingModel model in buildingModels)
        {
            if(model.Disabled)
                continue;
            BuildMenu selected = null;
            switch(model.Type)
            {
                case "Housing":
                    livingMenu.AddNewIcon(model.Name, model.RequiredResources.GetFormattedResource(),model.TextureResource, model.Description, model.BuildingResource);
                    selected = livingMenu;
                    break;
                case "Technology":
                    techMenu.AddNewIcon(model.Name, model.RequiredResources.GetFormattedResource(),model.TextureResource,model.Description,model.BuildingResource);
                    selected = techMenu;
                    break;
                case "Magic":
                    magicMenu.AddNewIcon(model.Name, model.RequiredResources.GetFormattedResource(),model.TextureResource,model.Description,model.BuildingResource);
                    selected = magicMenu;
                    break;
                case "Resource":
                    resourceMenu.AddNewIcon(model.Name, model.RequiredResources.GetFormattedResource(),model.TextureResource,model.Description,model.BuildingResource);
                    selected = resourceMenu;
                    break;
            }
            buildingModelToMenu.Add(model, selected);
        } 
        buildingUIMenu.AddBuildMenus(livingMenu, techMenu, resourceMenu, magicMenu);
            // BuildingIcon icon;
            // if(model.Type == "Housing")
            // {
            //     icon = buildUI.CreateMenuItem(menu, model.Name, model.RequiredResources.GetFormattedResource(),model.TextureResource);
            // }
            // else 
            // {              
            //     icon = buildUI.CreateMenuItem(menu2, model.Name, model.RequiredResources.GetFormattedResource(),model.TextureResource);
            // }
 
            //icon.Connect("ButtonPress", this, nameof(BuildingButtonPressed));
        
        
        

        //PUT THIS SOMEWHERE ELSE IT HURTS TO LOOK AT
    //     menu.Columns = menu.GetChildCount() +1;
    //     menu2.Columns = menu2.GetChildCount()+1;
    //     buildUI.resourceMenu = menu2;
    //     buildUI.housingMenu = menu;
    //     buildUI.AddChild(buildUI.housingMenu);
    //     buildUI.AddChild(buildUI.resourceMenu);
    //     Button btn  = buildUI.CreateMenuExitButton();
    //     //btn.Connect(nameof(BuildingControllerUI.BuildingMenuExited),this, nameof(On_BuildingMenuExited));
    //     Button btn2  = buildUI.CreateMenuExitButton();
    //     //btn2.Connect(nameof(BuildingControllerUI.BuildingMenuExited),this, nameof(On_BuildingMenuExited));
    //     buildUI.housingMenu.AddChild(btn);
    //     buildUI.resourceMenu.AddChild(btn2);
    //    // this.housingMenu.AddChild(exitButton);
    //     //////GD.Print(exitButton.si)
    //     buildUI.housingMenu.Visible =false;
    //     buildUI.resourceMenu.Visible = false;
    }

    // private BuildingIcon FetchIcon()
    // {
    //     buildingUIMenu.
    //     return null;
    // }
    


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
        //this.buildUI.Visible = false;
       //this.buildUI.selectedMenu = null;
        //this.buildingState = BuildingState.BuildMenuExited;
        GD.Print("Setting building state to exited");
    }

    public void SetSelectedBuilding(Building b)
    {
        selectedBuilding = b;
        buildingState = BuildingState.BuildingSelected;
    }
    public override void _PhysicsProcess(float delta)
    {
        // switch(buildingState)
        // {
        //     case BuildingState.BuildMenu:
        //         HandleBuildMenuState();
        //         break;
        //     case BuildingState.BuildMode:
        //         HandleBuildModeState();
        //         break;
        //     case BuildingState.BuildingSelected:
        //         if(selectedBuilding!= null && selectedBuilding.IsUIOpen())
        //         {
        //             if(selectedBuilding.GetWorkerChangeAmount() != 0)
        //             {
        //                 GD.Print("Pushing update change to thingy");
        //                 var bud = new BuildingUpdateInteraction(selectedBuilding, selectedBuilding.GetWorkerChangeAmount());
                        
        //                 selectedBuilding.On_WorkerAmountChange(0); //reset amount back to 0
        //                 GameUpdateQueue.TryPushUpdate(bud);
        //             }
        //         }
        //         break;
        // }
        
    }


    // public void HandleBuildMenuState()
    // {
    //     //if we have no current menu, then we know we need to open main menu
    //     if(this.buildUI.selectedMenu == null)
    //     {
            
    //         this.buildUI.Visible = true;
    //         GD.Print("Setting menu to being something becausse selcted is null");//GridContainer n  = this.GetNode<GridContainer>("CanvasLayer/Main Building Menu");
            
    //         buildUI.selectedMenu = this.buildUI.mainMenu; 
    //         buildUI.selectedMenu.Visible =true;
    //         GD.Print(buildUI.selectedMenu);
    //         GD.Print(buildUI.mainMenu);
    //         GD.Print(buildUI.selectedMenu.RectPosition);
    //         Tween tween = new Tween();
    //         tween.InterpolateProperty(buildUI.selectedMenu, "rect_position", buildUI.selectedMenu.RectPosition, new Vector2(buildUI.selectedMenu.RectPosition.x, buildUI.selectedMenu.RectPosition.y - 150), 0.08f, Tween.TransitionType.Quart, Tween.EaseType.Out);
    //         this.AddChild(tween);
    //         tween.Start();
    //         tween.Dispose();
    //         //buildingState = BuildingState.BuildMode;
    //     }

    // }

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
        // Building b = null;
        // if(selectedModel.Name == "Gathering Post")
        // {
        //     b = Params.LoadScene<StorageBuilding>("res://Object/GameObject/Buildings/" + selectedModel.Name +".tscn");
        // }
        // else {
        //     b = Params.LoadScene<Building>("res://Object/GameObject/Buildings/" + selectedModel.Name +".tscn");
        // }
       
        // GD.Print("Building : ",b);       
    
        // if(selectedModel.Name == "Fire")
        // {
        //     b.buildingAsset = Params.LoadScene<AnimatedSprite>("res://Assets/Sprites/20.09 - Traveler's Camp 1.1/StaticFire.tscn");
        //     SetNodeParams(b.buildingAsset);
        // }
        // else
        //     b.buildingAsset  = buildSprite;
        
        
        // b.model = this.selectedModel;
        // var intr = new BuildCompleteInteraction(b); 
        // GameUpdateQueue.TryPushUpdate(intr);
            
        // this.buildingState = BuildingState.BuildPending;    
    }
}