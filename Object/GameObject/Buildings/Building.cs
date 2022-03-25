using Godot;
using System;
using System.Collections.Generic;
public class Building : StaticInteractable, GameObject
{   
    public BuildingModel model;

    public Node2D buildingAsset;

    public List<string> JobIds;

    public BuildingUI UI;

    public TextureButton ExitButton;

    public List<string> AvailableWorkers;

    public List<string> AssignedWorkers;

    private bool hasUpdated = false;


    public override void _Ready()
    {
        base._Ready();
        AvailableWorkers = new List<string>();
        AssignedWorkers = new List<string>();
        UI = Params.LoadScene<BuildingUI>("res://Object/UI/Main/BuildingUI.tscn");
        UI.Visible = false;
        GD.Print("loaded building UI: ",UI);
        
    }

    public Building()
    {
        
       
        
    }
    
    public void AddAvaialbleWorker(string worker)
    {
        AvailableWorkers.Add(worker);
        UI.WorkerList.AddItem(worker);
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


    public bool IsUIOpen()
    {
        return this.UI.Visible;
    }

    public void FetchUIChanges()
    {

    }

    public bool HasChanges()
    {
        if(hasUpdated)
        {
            hasUpdated = false;
            return true;
        }
        return false;
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