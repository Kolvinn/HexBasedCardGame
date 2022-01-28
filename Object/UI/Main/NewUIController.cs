using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public class NewUIController : CanvasLayer
{

    
    //private List<BuildMenu> menus;
    [Signal]
    public delegate void TriggerBuild(string building);

    [Signal]
    public delegate void AnimationCompleted();

    [Signal]
    public delegate void BuildingMenuExited();
    public BuildMenu housingMenu, mainMenu, resourceMenu;


    Vector2 originPos;

    public static int i = 0;

    public List<BuildingModel> buildingModels;

    public Dictionary<string, TextureButton> buildingButtons = new Dictionary<string, TextureButton>();

    public BuildMenu currentMenu;

    public OptionBox currentOption;


    public override void _Ready()
    {


    }

    public void FadeToDark(){
        //GD.Print("Fading to night");

        Tween t = new Tween();
        ColorRect c = this.GetNode<ColorRect>("Control3/DayLight");
        Color color = c.SelfModulate;

        t.Connect("tween_all_completed", this, nameof(FadeToLight));
        color.a = 1;
        t.InterpolateProperty(c,"self_modulate",c.SelfModulate,color, 1);
        this.AddChild(t);
        t.Start();
    }

    public void FadeToLight()
    {
        
        //GD.Print("Fading to light");
        Tween t = new Tween();
        ColorRect c = this.GetNode<ColorRect>("Control3/DayLight");
        Color color = c.SelfModulate;
        color.a = 0;
        t.InterpolateProperty(c,"self_modulate",c.SelfModulate,color, 1);     
        this.AddChild(t);
        t.Start();
        t.Connect("tween_all_completed", this, nameof(AnimationFinished));
        //t.QueueFree();

    }

    public void AnimationFinished()
    {
        EmitSignal(nameof(AnimationCompleted));
    }
    
    public OptionBox CreateOption (String text){
        
        OptionBox ob = Params.LoadScene<OptionBox>("res://Object/UI/Helpers/OptionBox.tscn");
        //GD.Print("Creating option: ",ob ," at loop count: ",++i);
        ob.SetOptionText(text);
        this.currentOption = ob;
        this.AddChild(ob);
        return ob;
    }
    public void RunUI(){

        LoadMainBuildMenu();
        LoadBuildingMenus();
        this.GetNode<Button>("Control3/Control/Build").Connect("pressed", this, nameof(on_BuildingMenuPressed));
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
        currentMenu.Visible = false;
        currentMenu.RectPosition = this.originPos;
        
        mainMenu.Visible = false;
        mainMenu.RectPosition = this.originPos;
        currentMenu = null;
        EmitSignal(nameof(BuildingMenuExited));

    }

    public void LoadMainBuildMenu()
    {
        BuildMenu menu = Params.LoadScene<BuildMenu>("res://Object/UI/BuildMenu.tscn");
        originPos = menu.RectPosition;
        menu.Columns = 4;
        

        Button btn = new Button(){
            Text ="Housing",
            SizeFlagsHorizontal = 3,
            SizeFlagsVertical = 3
        };
        btn.Connect("pressed", this, nameof(_on_BuildHousing_pressed));
        menu.AddChild(btn);
        //btn.SizeFlagsHorizontal = SizeFlagsHorizontal.Fill;
        btn = new Button(){
            Text = "Resource",
            SizeFlagsHorizontal = 3,
            SizeFlagsVertical = 3
            
        };
        btn.Connect("pressed", this, nameof(_on_BuildResource_pressed));
        menu.AddChild(btn);
        //Godot.Control.SizeFlags.Fill;
        
        menu.AddChild(new Button(){
            Text = "Something",
            SizeFlagsHorizontal = 3,
            SizeFlagsVertical = 3
        });

        
        menu.AddChild(CreateMenuExitButton());
        //exitButton.RectMinSize = new Vector2(100,100);
        mainMenu = menu;
        this.AddChild(mainMenu);
        mainMenu.Visible = false;
        //////GD.Print(mainMenu.RectPosition);
        //////GD.Print(mainMenu.RectSize);

    }
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
        
        mainMenu.Visible =false;
        Vector2 pos = mainMenu.RectPosition;
        menu.Visible = true;
        menu.RectPosition = pos;
        currentMenu = menu;
    }

    public void LoadBuildingMenus()
    {
        BuildMenu menu = Params.LoadScene<BuildMenu>("res://Object/UI/BuildMenu.tscn");
        BuildMenu menu2 = Params.LoadScene<BuildMenu>("res://Object/UI/BuildMenu.tscn");
        
        int menuCol =1, menu2Col = 1;
        foreach(BuildingModel model in buildingModels)
        {
            
            BuildingIcon icon  = Params.LoadScene<BuildingIcon>("res://Object/UI/BuildingIcon.tscn");
            icon.button = icon.GetNode<TextureButton>("TextureButton");
            icon.label = icon.GetNode<Label>("Label");

            if(model.Type == "Housing"){
                ++menuCol;
                menu.AddChild(icon);
            }
            else 
            {
                ++menu2Col;
                //GD.Print("adding icon to resource menu");
                menu2.AddChild(icon);
            }
            //var item = buildingModels.FirstOrDefault(building => building.Name == p.Key);
            icon.button.HintTooltip = model.RequiredResources.GetFormattedResource();
            //////GD.Print("first button time: ", icon.button);
            icon.Connect("ButtonPress", this, nameof(ButtonPressed));
            // //p.Value.Connect("ButtonPress", this,nameof(ButtonPressed));
            TextureButton b = Params.LoadScene<TextureButton>(model.TextureResource);
            //GD.Print("B ", b.Name);
            icon.button.Disabled =true;
            this.buildingButtons.Add(model.Name, icon.button);
            icon.UpdateIcon(model.Name,b);          
        }
        
        menu.Columns = menuCol;
        menu2.Columns = menu2Col;
        this.resourceMenu = menu2;
        this.housingMenu = menu;
        this.AddChild(housingMenu);
        this.AddChild(resourceMenu);
        this.housingMenu.AddChild(CreateMenuExitButton());
        this.resourceMenu.AddChild(CreateMenuExitButton());
       // this.housingMenu.AddChild(exitButton);
        //////GD.Print(exitButton.si)
        housingMenu.Visible =false;
        resourceMenu.Visible = false;
    }
    




    public void on_BuildingMenuPressed()
    {
        if(this.currentMenu ==null)
        {//GridContainer n  = this.GetNode<GridContainer>("CanvasLayer/Main Building Menu");
            mainMenu.Visible = true;
            currentMenu = mainMenu;
            Tween tween = new Tween();
            tween.InterpolateProperty(mainMenu, "rect_position", mainMenu.RectPosition, new Vector2(mainMenu.RectPosition.x, mainMenu.RectPosition.y - 150), 0.08f, Tween.TransitionType.Quart, Tween.EaseType.Out);
            this.AddChild(tween);
            tween.Start();
            tween.Dispose();
        }
    }


    public void ButtonPressed (string button)
    {
        EmitSignal(nameof(TriggerBuild),button);
    }

  

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
