using Godot;
using System;

public class BuildingUI : Control
{
    //public OptionButton WorkerList;
    public GridContainer WorkerContainer;

    public delegate void WorkerSelected(string workerName);

    [Signal]
    public delegate void WorkerAmountChange(int amount);

    [Signal]
    public delegate void ExitPressed();


    private bool closeModal = false;

    private TextureButton AddButton, RemoveButton;

    public override void _Ready()
    {
        //WorkerList = this.GetNode<OptionButton>("MarginContainer/GridContainer/NinePatchRect3/VBoxContainer/OptionButton");
        WorkerContainer = this.GetNode<GridContainer>("MarginContainer/GridContainer/NinePatchRect3/VBoxContainer/ScrollContainer/GridContainer");
        AddButton = this.GetNode<TextureButton>("MarginContainer/GridContainer/NinePatchRect3/VBoxContainer/GridContainer2/GridContainer/Add");
        RemoveButton = this.GetNode<TextureButton>("MarginContainer/GridContainer/NinePatchRect3/VBoxContainer/GridContainer2/GridContainer/Remove");
        

        //WorkerList.AddItem("Worker 1");
        //WorkerList.AddItem("Worker 2");
       // AddWorkerToContainer("Worker 1");
        //AddWorkerToContainer("Worker 2");

    }

    public void _on_Add_pressed()
    {
        EmitSignal(nameof(WorkerAmountChange), 1);
    }

    public void _on_Remove_pressed()
    {        
        EmitSignal(nameof(WorkerAmountChange), -1);
    }
    public void AddWorkerToContainer(string worker)
    {
        Label label = new Label();
        label.Text = worker;
        label.Align = Label.AlignEnum.Center;
        label.Valign = Label.VAlign.Center;
        label.SizeFlagsHorizontal = 3;
        label.SizeFlagsVertical = 3;
        WorkerContainer.AddChild(label);                   
    }

    public void UpdateAssignedTotal(int total, int max)
    {
        this.GetNode<Label>("MarginContainer/GridContainer/NinePatchRect3/VBoxContainer/Label2").Text = "Workers (" +total+"/"+max+")";
    }

    public void UpdateWorkerTotal(int total)
    {
        this.GetNode<Label>("MarginContainer/GridContainer/NinePatchRect3/VBoxContainer/GridContainer2/VBoxContainer/Label").Text = "Workers (" +total +")";
    }

    // public void _on_OptionButton_item_selected(int index)
    // {
    //     for(int i =0; i< WorkerList.GetItemCount(); i++)
    //     {
    //         if(i == index){
    //             string s = WorkerList.GetItemText(i);
    //             WorkerList.RemoveItem(i);
    //             Label label = new Label();
    //             label.Text = s;
    //             label.Align = Label.AlignEnum.Center;
    //             label.Valign = Label.VAlign.Center;
    //             label.SizeFlagsHorizontal = 3;
    //             label.SizeFlagsVertical = 3;
    //             WorkerContainer.AddChild(label);
    //             WorkerList.Text = "Add Worker";
    //             EmitSignal("WorkerSelected", s);
    //             break;
    //         }
    //     }
    // }

    // public void AddWorkerToList(string worker)
    // {
    //    // this.WorkerList.AddItem(worker);
    // }



}
