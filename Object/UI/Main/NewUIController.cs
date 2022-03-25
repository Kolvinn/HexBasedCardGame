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

   // [Signal]
    //public delegate void BuildingMenuExited();
    //public BuildMenu housingMenu, mainMenu, resourceMenu;
    public Control BuildingUILayer;

    //Vector2 originPos;

    public static int i = 0;



    public OptionBox currentOption;


    public override void _Ready()
    {
        BuildingUILayer = this.GetNode<Control>("Control3/BuildingUILayer");

    }
    public BuildingControllerUI GetBuildUI()
    {
        return GetNode<BuildingControllerUI>("Control3/BuildingControllerUI");
    }

    public void AddBuildingUI(BuildingUI ui)
    {
        this.BuildingUILayer.AddChild(ui);
    }

    public void FadeToDark(){
        GD.Print("Fading to night");

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

    public Button GetBuildButton()
    {
        return this.GetNode<Button>("Control3/Control/Build");
    }
   


    public void LoadPlayerTurnUI(){
        
        foreach(Node n in this.GetNode<Control>("Control3").GetChildren())
        {
            GD.Print("loading UI item ",n.Name);
            if(n.Name== "DayLight" || n.Name== "TaskBar" || n.Name== "SpellBook" || n.Name== "FoundBook")
            {
                ((Control)n).Visible  = false;
            }
            else if(n.Name== "Control"){
                ((Control)n).Visible = true;
                GD.Print(n.Name, "   ", n.GetNode<Button>("Build"));
                n.GetNode<Button>("Build").Visible = false;
            }
            else{
                GD.Print("Setting ", n.Name ," to visible");
                ((Control)n).Visible  = true;
            }

        }
        this.GetNode<Control>("Control3").Visible = true;
    }

    public void LoadEnemyTurnUI(){
        foreach(Node n in this.GetNode<Control>("Control3").GetChildren())
        {
            if(n.Name!= "BattleControl")
                ((Control)n).Visible  = false;
            else 
                ((Control)n).Visible  = true;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
