using Godot;
using System;

public class BuildingUI : Control
{
    public OptionButton WorkerList;
    public GridContainer WorkerContainer;

    public delegate void WorkerSelected(string workerName);

    [Signal]
    public delegate void ExitPressed();


    private bool closeModal = false;

    public override void _Ready()
    {
        WorkerList = this.GetNode<OptionButton>("MarginContainer/GridContainer/NinePatchRect3/VBoxContainer/OptionButton");
        WorkerContainer = this.GetNode<GridContainer>("MarginContainer/GridContainer/NinePatchRect3/VBoxContainer/ScrollContainer/GridContainer");

        WorkerList.AddItem("Worker 1");
        WorkerList.AddItem("Worker 2");
       // AddWorkerToContainer("Worker 1");
        //AddWorkerToContainer("Worker 2");

    }


    public void AddWorkerToContainer(string worker)
    {
        for(int i =0; i< WorkerList.GetItemCount(); i++)
        {
            if(WorkerList.GetItemText(i) == worker){
                WorkerList.RemoveItem(i);
                Label label = new Label();
                label.Text = worker;
                label.Align = Label.AlignEnum.Center;
                label.Valign = Label.VAlign.Center;
                label.SizeFlagsHorizontal = 3;
                label.SizeFlagsVertical = 3;
                WorkerContainer.AddChild(label);
                WorkerList.Text = "Add Worker";
                break;
            }
        }
    }

    public void _on_OptionButton_item_selected(int index)
    {
        for(int i =0; i< WorkerList.GetItemCount(); i++)
        {
            if(i == index){
                string s = WorkerList.GetItemText(i);
                WorkerList.RemoveItem(i);
                Label label = new Label();
                label.Text = s;
                label.Align = Label.AlignEnum.Center;
                label.Valign = Label.VAlign.Center;
                label.SizeFlagsHorizontal = 3;
                label.SizeFlagsVertical = 3;
                WorkerContainer.AddChild(label);
                WorkerList.Text = "Add Worker";
                EmitSignal("WorkerSelected", s);
                break;
            }
        }
    }

    public void AddWorkerToList(string worker)
    {
        this.WorkerList.AddItem(worker);
    }



}
