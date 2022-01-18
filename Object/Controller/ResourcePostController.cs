using Godot;
using System;
using System.Collections.Generic;
public class ResourcePostController: Node2D, ControllerBase
{

    public OptionBox optionBox;
    public List<CharacterController> AvailableWorkers;

    public List<CharacterController> AssignedWorkers;

    private AStar2D resourceMap;
    public int workerAmount =0;

    Dictionary<CharacterController, BasicResource> taskMap = new Dictionary<CharacterController, BasicResource>();
    public HexGrid grid;

    [Signal]
    public delegate void CloseDialog();

    [Signal]
    public delegate void ResourceDrop(BasicResource res, int amount);

    public Building ResourcePost;

   // private Dictionary<>
    public ResourcePostController()
    {

    }

    public ResourcePostController(OptionBox box, HexGrid grid, Building b)
    {
        this.grid = grid;
        this.MapResourcePoints();
        this.optionBox = box;
        this.ResourcePost = b;
        optionBox.UpdateType("worker");
        optionBox.Connect("WorkerAmountChange", this, nameof(On_WorkerAmountChange));
        optionBox.Connect("OnResponse",this,nameof(On_Response));
        optionBox.Visible =false;
        AvailableWorkers = new List<CharacterController>();
        AssignedWorkers = new List<CharacterController>();
    }

    public void On_WorkerAmountChange(int amount)
    {
        GD.Print("worker amount: ", this.workerAmount, " chanigng with ", amount, " worker");
        if(amount <0)
        {
            if(workerAmount+amount >= 0)
            {
                CharacterController c =  AssignedWorkers[0];
                AssignedWorkers.RemoveAt(0);
                AvailableWorkers.Add(c);
                

                
            }
            else{
                return;
            }
        }
        else{
            if(AvailableWorkers?.Count > 0)
            {
                
                CharacterController c = AvailableWorkers[0];
                c.character.Connect("TargetHexReached", c, "On_DestinationReached");
                c.Connect("TargetHexReached", this, nameof( On_DestinationReached));
                c.Connect("CapacityFull", this, nameof(On_CapacityFull));
                AvailableWorkers.RemoveAt(0);   
                AssignedWorkers.Add(c);  
                
            }  
            else {
                return;
            }
        
        }
        workerAmount += amount;
        this.optionBox.GetNode<Label>("TextureRect/MarginContainer/VBoxContainer/HBoxContainer2/Label").Text = "Workers: "+workerAmount;
        //this.optionBox.GetNode<Label>("TextureRect/MarginContainer/VBoxContainer/HBoxContainer2/Label").Text = "Workers: "+workerAmount;
    }

    public void On_DestinationReached(CharacterController c, string task)
    {
        if(task == "Gather")
        {
            c.GatherResource();
            
        }
        else if(task  == "Drop")
        {
            BasicResource b = c.resType;
            GD.Print("Emitting signala with ", c.resType, c.capacity);
            EmitSignal(nameof(ResourceDrop),b,c.capacity);
            c.capacity = 0;
            //this.AssignedWorkers.Add(c);
            c.CurrentTask = "Gather";
            QueueNextTask(c);
        }     
        
    }

    public void QueueNextTask(CharacterController c){
        int pos = resourceMap.GetClosestPoint(this.ResourcePost.buildingAsset.Position);
        
        Vector2 vec = resourceMap.GetClosestPositionInSegment(this.ResourcePost.buildingAsset.Position);
        GD.Print("closest vector to post pos: ", this.ResourcePost.buildingAsset.Position, " is ", resourceMap.GetPointPosition(pos));
        GD.Print("closest vector as ID: ", resourceMap.GetPointPosition(pos));
        GD.Print("Worker Position: ", c.character.Position);
        //GD.Print("Worker Position: ", AssignedWorkers[0].Position);
        int toIndex = grid.IndexOfVec(resourceMap.GetPointPosition(pos));
        int fromIndex = grid.IndexOfVec(c.character.currentTestTile.Position);

        HexHorizontalTest hex=  grid.storedHexes[toIndex];

        c.character.TargetHex = hex;
        c.CurrentTask = "Gather";
        //this.AssignedWorkers.Remove(contr);
        //AssignedWorkers[0].
        GD.Print("toIndex: ", toIndex, "   fromIndex: ", fromIndex);
        PopulateCharacterPath(c,fromIndex, toIndex);
    }



    public void On_Response(bool accepted, OptionBox option){
        this.optionBox.Visible = false;
        EmitSignal(nameof(CloseDialog));
    }

    public void OnClick(){
        this.optionBox.Visible = true;
    }

    public void MapResourcePoints()
    {
        resourceMap = new AStar2D();
        int i =0;
        foreach(KeyValuePair<Vector2, Node2D> p in grid.resourcePositions)
        {
            if(grid.storedHexes[grid.IndexOfVec(p.Key)].isBasicResource){
                int j = 0;
                resourceMap.AddPoint(i, p.Key);
                var points  = resourceMap.GetPoints();
                foreach(var point in points)
                {
                    resourceMap.ConnectPoints(i,j++);
                }
                ++i;
            // resourceMap.AddPoint(i++,p.Key);
            
            }
        }

        //resourceMap.GetPoints

        //resourceMap.GetClosestPoint
    }

    public void On_CapacityFull(CharacterController c){
        int pos = resourceMap.GetClosestPoint(this.ResourcePost.buildingAsset.Position);

        //Vector2 vec = resourceMap.GetClosestPositionInSegment(this.ResourcePost.buildingAsset.Position);
        GD.Print("closest vector to post pos: ", this.ResourcePost.buildingAsset.Position, " is ", resourceMap.GetPointPosition(pos));
        GD.Print("closest vector as ID: ", resourceMap.GetPointPosition(pos));
        GD.Print("Worker Position: ", c.character.Position);
        //GD.Print("Worker Position: ", AssignedWorkers[0].Position);
        int toIndex = grid.IndexOfVec(this.ResourcePost.buildingAsset.Position);
        int fromIndex = grid.IndexOfVec(c.character.currentTestTile.Position);
        HexHorizontalTest hex=  grid.storedHexes[toIndex];
        c.character.TargetHex = hex;
        PopulateCharacterPath(c,fromIndex, toIndex);
        c.CurrentTask = "Drop";
    }


    public void PopulateCharacterPath(CharacterController c, int from, int to)
    {
        foreach( Vector2 v in grid.pathFinder.GetPointPath(from, to))
            {
                c.movementQueue.Enqueue(v);

            }
    }

    public override void _PhysicsProcess(float delta)
    {
        if(this.AssignedWorkers!=null && resourceMap != null && this.AssignedWorkers.Count>0){
            int pos = resourceMap.GetClosestPoint(this.ResourcePost.buildingAsset.Position);
            Vector2 vec = resourceMap.GetClosestPositionInSegment(this.ResourcePost.buildingAsset.Position);
            GD.Print("closest vector to post pos: ", this.ResourcePost.buildingAsset.Position, " is ", resourceMap.GetPointPosition(pos));
            GD.Print("closest vector as ID: ", resourceMap.GetPointPosition(pos));
            GD.Print("Worker Position: ", AssignedWorkers[0].character.Position);
            //GD.Print("Worker Position: ", AssignedWorkers[0].Position);
            int toIndex = grid.IndexOfVec(resourceMap.GetPointPosition(pos));
            int fromIndex = grid.IndexOfVec(AssignedWorkers[0].character.Position);

            HexHorizontalTest hex=  grid.storedHexes[toIndex];
            CharacterController contr = this.AssignedWorkers[0];
            contr.character.TargetHex = hex;
            contr.CurrentTask = "Gather";
            this.AssignedWorkers.Remove(contr);
            //AssignedWorkers[0].
            GD.Print("toIndex: ", toIndex, "   fromIndex: ", fromIndex);
            PopulateCharacterPath(contr,fromIndex, toIndex);

            
            //GD.Print("Index of vector ",grid.IndexOfVec(resourceMap.GetPointPosition(pos)));
            //this.AssignedWorkers[0].movementQueue
            //this.AssignedWorkers = null;
            
            //grid.pathFinder.ArePointsConnected()


        }
    }


}   