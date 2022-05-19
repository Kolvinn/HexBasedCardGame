using Godot;
using System;
using System.Collections.Generic;
public class Building : StaticInteractable, GameObject
{   
    //public BuildingModel model;

    public Sprite buildingAsset;

    public BuildingUI UI;

    public TextureButton ExitButton;

    //public List<string> AvailableWorkers;

    public List<string> AssignedWorkers;

    private bool hasUpdated = false;

    private bool WorkerAdd = false;
    
    private bool WorkerRemove =false;

    private int WorkerChange = 0;

    private int MaxWorkers = 5;

    private int TotalWorkers = 0;


    public override void _Ready()
    {
        base._Ready();
       // AvailableWorkers = new List<string>();
        AssignedWorkers = new List<string>();
        buildingAsset = this.GetNode<Sprite>("Sprite");
        UI = Params.LoadScene<BuildingUI>("res://Object/UI/Main/BuildingUI.tscn");
        UI.Visible = false;
        //UI.Connect("WorkerAmountChange", this, nameof(On_WorkerAmountChange));
        GD.Print("loaded building UI: ",UI);
        
    }

    public Building()
    {
        
       
        
    }
    
    public void AddAvaialbleWorker(string worker)
    {
        //AvailableWorkers.Add(worker);
        //UI.WorkerList.AddItem(worker);
    }

    public void ConnectExitButton()
    {
        GD.Print("Connecting exit button");
        ExitButton = UI.GetNode<TextureButton>("ExitButton");
        ExitButton.Connect("pressed", this, nameof(_on_ExitButton_pressed));

        UI.Connect("WorkerSelected", this, "On_WorkerSelected");
    }

    public void On_WorkerSelected(string workerName)
    {
        AssignedWorkers.Add(workerName);
        hasUpdated = true;
    }


    public void On_WorkerAmountChange(int amount)
    {
        if(TotalWorkers + amount < 0 || TotalWorkers +amount > MaxWorkers)
            return;
        this.WorkerChange = amount;
        
        GD.Print("Updating worker change amount to: ",WorkerChange);
        this.UI.UpdateAssignedTotal(TotalWorkers+amount, MaxWorkers);
        this.UI.UpdateWorkerTotal(TotalWorkers+amount);
    }

    public int GetWorkerChangeAmount()
    {
        
        return this.WorkerChange;
    }

    public bool IsUIOpen()
    {
        return this.UI.Visible;
    }

    public void FetchUIChanges()
    {

    }

    

    public void _on_ExitButton_pressed()
    {
        GD.Print("on exit pressed");
        UI.Visible = false;
    }

    public void OpenUI()
    {
        this.UI.Visible = true;
    }
    public void CloseUI()
    {
        this.UI.Visible = false;
    }


    
}