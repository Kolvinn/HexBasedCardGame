using Godot;
using System;
using System.Collections.Generic;

public class BuildingControllerUI : Control
{
    public Vector2 originPos;
    public BuildMenu housingMenu, mainMenu, resourceMenu, selectedMenu;

    public Dictionary<string, TextureButton> buildingButtons = new Dictionary<string, TextureButton>();

    [Signal]
    public delegate void BuildingMenuExited();

    public override void _Ready()
    {
        
    }

    public Button CreateMenuExitButton()
    {
        Button exitButton = new Button(){
            Text = "Exit",
            SizeFlagsHorizontal = 3,
            SizeFlagsVertical = 3
        };
        exitButton.Connect("pressed", this, nameof(on_exit_pressed));
        return exitButton;
    }

    public void on_exit_pressed()
    {
        selectedMenu.Visible = false;
        selectedMenu.RectPosition = this.originPos;
        mainMenu.RectPosition = originPos;
        
        housingMenu.RectPosition = originPos;
        
        resourceMenu.RectPosition = originPos;
        
        
        //mainMenu.Visible = false;
        //mainMenu.RectPosition = this.originPos;
        //selectedMenu = null;
        GD.Print("ON EXIT PRESSED");
        EmitSignal(nameof(BuildingMenuExited));

    }

    // public BuildMenu LoadMainBuildMenu()
    // {
    //     BuildMenu menu = Params.LoadScene<BuildMenu>("res://Object/UI/BuildMenu.tscn");
    //     originPos = menu.RectPosition;
    //     menu.Columns = 4;
        

    //     Button btn = new Button(){
    //         Text ="Housing",
    //         SizeFlagsHorizontal = 3,
    //         SizeFlagsVertical = 3
    //     };
    //     btn.Connect("pressed", this, nameof(_on_BuildHousing_pressed));
    //     menu.AddChild(btn);
    //     //btn.SizeFlagsHorizontal = SizeFlagsHorizontal.Fill;
    //     btn = new Button(){
    //         Text = "Resource",
    //         SizeFlagsHorizontal = 3,
    //         SizeFlagsVertical = 3
            
    //     };
    //     btn.Connect("pressed", this, nameof(_on_BuildResource_pressed));
    //     menu.AddChild(btn);
    //     //Godot.Control.SizeFlags.Fill;
        
    //     menu.AddChild(new Button(){
    //         Text = "Something",
    //         SizeFlagsHorizontal = 3,
    //         SizeFlagsVertical = 3
    //     });

        
       
    //     //exitButton.RectMinSize = new Vector2(100,100);
    //     mainMenu = menu;
    //     this.AddChild(mainMenu);
    //     //mainMenu.Visible = false;
    //     return mainMenu;


    // }
    public void _on_BuildResource_pressed()
    {
        ChangeMenu(this.resourceMenu);
    }
    public void _on_BuildHousing_pressed()
    {
        ChangeMenu(this.housingMenu);

    }

    public void ChangeMenu(BuildMenu menu)
    {
        //this.mainMenu.RemoveChild(exitButton);
        
        //GridContainer n  = this.GetNode<GridContainer>("CanvasLayer/Main Building Menu");
        //GD.Print("changing menu to "+nameof(menu), " with children :", menu.GetChildren().Count);
        //GD.Print(menu.RectPosition);
        
        selectedMenu.Visible =false;
        Vector2 pos = selectedMenu.RectPosition;
        menu.Visible = true;
        menu.RectPosition = pos;
        selectedMenu = menu;
    }

    public BuildingIcon CreateMenuItem(BuildMenu menu, string name, string resourceString, string textureResource)
    {
        BuildingIcon icon  = Params.LoadScene<BuildingIcon>("res://Object/UI/BuildingIcon.tscn");
        icon.button = icon.GetNode<TextureButton>("TextureButton");
        icon.label = icon.GetNode<Label>("Label");

        menu.AddChild(icon);
        icon.button.HintTooltip = resourceString;

        // //p.Value.Connect("ButtonPress", this,nameof(ButtonPressed));
        TextureButton b = null;
        try{
            b = Params.LoadScene<TextureButton>(textureResource);
        }catch(Exception e)
        {
            b = Params.LoadScene<TextureButton>("res://Assets/Sprites/20.09 - Traveler's Camp 1.1/ImageNotFound.tscn");
            GD.Print("found this: ",b);
        }
        //GD.Print("B ", b.Name);
        icon.button.Disabled =true;

        buildingButtons.Add(name, icon.button);
        icon.UpdateIcon(name,b);
        return icon;
       
    }

    // public void LoadBuildingMenus()
    // {
    //     BuildMenu menu = Params.LoadScene<BuildMenu>("res://Object/UI/BuildMenu.tscn");
    //     BuildMenu menu2 = Params.LoadScene<BuildMenu>("res://Object/UI/BuildMenu.tscn");
        
    //     int menuCol =1, menu2Col = 1;
    //     foreach(BuildingModel model in buildingModels)
    //     {
            
    //         BuildingIcon icon  = Params.LoadScene<BuildingIcon>("res://Object/UI/BuildingIcon.tscn");
    //         icon.button = icon.GetNode<TextureButton>("TextureButton");
    //         icon.label = icon.GetNode<Label>("Label");

    //         if(model.Type == "Housing"){
    //             ++menuCol;
    //             menu.AddChild(icon);
    //         }
    //         else 
    //         {
    //             ++menu2Col;
    //             //GD.Print("adding icon to resource menu");
    //             menu2.AddChild(icon);
    //         }
    //         //var item = buildingModels.FirstOrDefault(building => building.Name == p.Key);
    //         icon.button.HintTooltip = model.RequiredResources.GetFormattedResource();
    //         //////GD.Print("first button time: ", icon.button);
    //         icon.Connect("ButtonPress", this, nameof(ButtonPressed));
    //         // //p.Value.Connect("ButtonPress", this,nameof(ButtonPressed));
    //         TextureButton b = Params.LoadScene<TextureButton>(model.TextureResource);
    //         //GD.Print("B ", b.Name);
    //         icon.button.Disabled =true;
    //         GD.Print("Adding button: ", model.Name, " and ", icon.button);
    //         this.buildingButtons.Add(model.Name, icon.button);
    //         icon.UpdateIcon(model.Name,b);          
    //     }
        
    //     menu.Columns = menuCol;
    //     menu2.Columns = menu2Col;
    //     this.resourceMenu = menu2;
    //     this.housingMenu = menu;
    //     this.AddChild(housingMenu);
    //     this.AddChild(resourceMenu);
    //     this.housingMenu.AddChild(CreateMenuExitButton());
    //     this.resourceMenu.AddChild(CreateMenuExitButton());
    //    // this.housingMenu.AddChild(exitButton);
    //     //////GD.Print(exitButton.si)
    //     housingMenu.Visible =false;
    //     resourceMenu.Visible = false;
    // }
    




    // public void on_BuildingMenuPressed()
    // {
    //     if(this.currentMenu ==null)
    //     {//GridContainer n  = this.GetNode<GridContainer>("CanvasLayer/Main Building Menu");
    //         mainMenu.Visible = true;
    //         currentMenu = mainMenu;
    //         Tween tween = new Tween();
    //         tween.InterpolateProperty(mainMenu, "rect_position", mainMenu.RectPosition, new Vector2(mainMenu.RectPosition.x, mainMenu.RectPosition.y - 150), 0.08f, Tween.TransitionType.Quart, Tween.EaseType.Out);
    //         this.AddChild(tween);
    //         tween.Start();
    //         tween.Dispose();
    //     }
    // }
}
