using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;  
public class ResourcePostController: Node2D, ControllerBase
{

//     public OptionBox optionBox;
//     public List<CharacterController> AvailableWorkers;

//     public List<CharacterController> AssignedWorkers;

//     public Queue<CharacterController> IdleWorkers;

//     private AStar2D resourceMap;
//     public int workerAmount =0;

//     Dictionary<CharacterController, BasicResource> taskMap = new Dictionary<CharacterController, BasicResource>();
//     public HexGrid grid;

//     [Signal]
//     public delegate void CloseDialog();

//     [Signal]
//     public delegate void ResourceDrop(BasicResource res, int amount);

//     public Building ResourcePost;

//     private Dictionary<HexHorizontalTest, int> HexToInt = new Dictionary<HexHorizontalTest, int>();

    
//     public Dictionary<BasicResource, CharacterController> BusyResources = new Dictionary<BasicResource, CharacterController>();

    
//     public List<HexHorizontalTest> BusyHexes = new List<HexHorizontalTest>();

//     System.Threading.Thread WorkerThread;

//     private CharacterController NextIdleWorker;

//     public int HighestTime =int.MinValue;

//     public Queue<Node> GarbageResources = new Queue<Node>();

//     [Signal]
//     public delegate void ResourceTaskAssignmentComplete();
//    // private Dictionary<>
//     public ResourcePostController()
//     {
        
//     }
  
//     public ResourcePostController(OptionBox box, HexGrid grid, Building b)
//     {
//         //GD.Print("genning test task");
//         TestTask t = new TestTask();
//         WorkerThread = new System.Threading.Thread(new ThreadStart(ParseWorkerState));
//         this.grid = grid;
//         this.MapResourcePoints();
//         this.optionBox = box;
//         this.ResourcePost = b;
//         optionBox.UpdateType("worker");
//         optionBox.Connect("WorkerAmountChange", this, nameof(On_WorkerAmountChange));
//         optionBox.Connect("OnResponse",this,nameof(On_Response));
//         optionBox.Visible =false;
//         AvailableWorkers = new List<CharacterController>();
//         AssignedWorkers = new List<CharacterController>();
//         IdleWorkers = new Queue<CharacterController>();
//     }


//     public HexHorizontalTest FindClosestNeighbour(HexHorizontalTest hex, int fromIndex)
//     {
//         int lowest = 70000000;
//         HexHorizontalTest temp = null;
//         foreach(HexHorizontalTest h in hex.connections.Select(item=> item.hex))
//         {   

//             //get random connected neighbour. Make this better later
//             //GD.Print("testing connection for : ", hex, " aaand : ",h);
            
//             int path = 0;//grid.pathFinder.GetPointPath(HexGrid.IndexOfVec(h.Position), fromIndex).Length;
//             if(path > 0)
//             {
//                 if(path < lowest)
//                 {
//                     lowest = path;
//                     temp = h;
//                 }
//             }
//         }
//         return temp;
//     }
//     public void WorkerGoToGather(CharacterController c)
//     {
        
//         c.DestinationReached = false;

//         int pos = resourceMap.GetClosestPoint(this.ResourcePost.buildingAsset.Position);

//         if(pos == -1)
//         {
//             return;
//         }
//             int toIndex = HexGrid.IndexOfVec(resourceMap.GetPointPosition(pos));
//             int fromIndex = HexGrid.IndexOfVec(c.character.currentTestTile.Position);
//             HexHorizontalTest hex=  HexGrid.storedHexes[toIndex];
            

//             BasicResource res = hex.GetNextNonBusyResource();
//             //GD.Print("Resource is: ", res);
//             //GD.Print("hex is: ", hex);
//             //We've found a resource that isn't busy, so let's go gaather it
//             if(res != null)
//             {   
//                 //res.IsBusy = true;
//                 this.BusyResources.Add(res,c);
//                 c.currentTargetResource = res;

//             }
//             //this hex doesn't have any available resources, so let's go find another hex
//             else
//             {
//                 BusyHexes.Add(hex);
//                 //GD.Print("Disabling point at pos: ", pos);
//                 resourceMap.SetPointDisabled(HexToInt[hex], true);

//                 WorkerGoToGather(c);
//                 return;
//             }
           
            
//             HexHorizontalTest newHex =  FindClosestNeighbour(hex,fromIndex);

//             if(newHex == null)
//             {
//                 //GD.Print("no valid border");

//             }
//             else
//             {
//                 //GD.Print("border is valid, populating destination with new hex");
//                // c.character.TargetHex = hex;
//                 c.workerState = State.WorkerState.Gather;
//                 PopulateCharacterPath(c,fromIndex, HexGrid.IndexOfVec(newHex.Position));
//             }


//             // if(!grid.pathFinder.ArePointsConnected(fromIndex,toIndex))
//             // {
//             //     BusyHexes.Add(hex);
//             //     //GD.Print("POINTS ARE NOT CONENCTED MUFUSA : ", hex.connections.Count);

//             //     resourceMap.SetPointDisabled(HexToInt[hex], true);
//             //     WorkerGoToGather(c);
//             //     return;
//             // }
//             // //GD.Print("from: ", fromIndex, "  to: ",toIndex, " with c: ",c);
//             // c.character.TargetHex = hex;
//             // c.workerState = State.WorkerState.Gather;
//             // PopulateCharacterPath(c,fromIndex, toIndex);
        
//     //     while(FoundResource == null)
//     //     {
            


//     //         FoundResource = hex.GetNextNonBusyResource();

//     //         if(FoundResource != null)
//     //         {
//     //             BusyResources.Add(c, FoundResource);
//     //         }
//     //         int fromIndex = HexGrid.IndexOfVec(c.character.currentTestTile.Position);
//     //         c.character.TargetHex = hex;
//     //         c.workerState = State.WorkerState.Gather;
//     //         PopulateCharacterPath(c,fromIndex, toIndex);
//     //         //if(BusyResources[FoundResource] == )
//     //     }
//     //         //int fromIndex = HexGrid.IndexOfVec(c.character.currentTestTile.Position);
//     //    // //GD.Print("fromIdx: ", fromIndex, " toIndex: ", toIndex);
        
//     //     //if()
        
//     }


//     public void HasAccessibleBorder(){

//     }
//     public void WorkerDrop(CharacterController c)
//     {
//         BasicResource b = c.resType;
//         //GD.Print("Emitting signala with ", c.resType, c.capacity);
//         EmitSignal(nameof(ResourceDrop),b,c.capacity);
//         c.capacity = 0;
//         //this.AssignedWorkers.Add(c);
//         //c.workerState = State.WorkerState.DroppingFinish;
//         //IdleWorkers.Enqueue(c);
//     }

//     public void WorkerGoToDrop(CharacterController c)
//     {
//         c.DestinationReached = false;
//         //int pos = resourceMap.GetClosestPoint(this.ResourcePost.buildingAsset.Position);
//         ////GD.Print("Worker Position: ", AssignedWorkers[0].Position);
//         //GD.Print("Worker Go to Drop with state: ", c.workerState);
//         int toIndex = HexGrid.IndexOfVec(this.ResourcePost.buildingAsset.Position);
//         int fromIndex = HexGrid.IndexOfVec(c.character.currentTestTile.Position);
//         //GD.Print("fromIdx: ", fromIndex, " toIndex: ", toIndex);
//        // HexHorizontalTest hex=  grid.storedHexes[toIndex];
//         //c.character.TargetHex = hex;
//         c.workerState = State.WorkerState.Dropping;
//         PopulateCharacterPath(c,fromIndex, toIndex);
        
//     }

//     public void WorkerGather(CharacterController c)
//     {
//         //GD.Print("Gather Resource");
//         //c.GatherResource();
//     }


//     public void ParseWorkerState()
//     {
//         //await TestTask.Main(null);
//         switch(NextIdleWorker.workerState)
//         {
//             case State.WorkerState.Default:
//                 WorkerGoToGather(NextIdleWorker);
//                 break;
//             // case State.WorkerState.DroppingFinish:
//             //     WorkerGoToGather(NextIdleWorker);
//             //     break;
//             // case State.WorkerState.GatherFinish:
//             //     WorkerGoToDrop(NextIdleWorker);
//             //     break;
//             case State.WorkerState.Gather:
//                 WorkerGather(NextIdleWorker);
//                 break;
//             case State.WorkerState.Dropping:
//                 WorkerDrop(NextIdleWorker);
//                 break;
//         }

        

//             // int pos = resourceMap.GetClosestPoint(this.ResourcePost.buildingAsset.Position);
//             // Vector2 vec = resourceMap.GetClosestPositionInSegment(this.ResourcePost.buildingAsset.Position);
//             // //GD.Print("closest vector to post pos: ", this.ResourcePost.buildingAsset.Position, " is ", resourceMap.GetPointPosition(pos));
//             // //GD.Print("closest vector as ID: ", resourceMap.GetPointPosition(pos));
//             // //GD.Print("Worker Position: ", AssignedWorkers[0].character.Position);
//             // ////GD.Print("Worker Position: ", AssignedWorkers[0].Position);
//             // int toIndex = HexGrid.IndexOfVec(resourceMap.GetPointPosition(pos));
//             // int fromIndex = HexGrid.IndexOfVec(AssignedWorkers[0].character.Position);

//             // HexHorizontalTest hex=  grid.storedHexes[toIndex];
//             // CharacterController contr = this.AssignedWorkers[0];
//             // contr.character.TargetHex = hex;
//             // contr.CurrentTask = "Gather";
//             // this.AssignedWorkers.Remove(contr);
//             // //AssignedWorkers[0].
//             // //GD.Print("toIndex: ", toIndex, "   fromIndex: ", fromIndex);
//             // PopulateCharacterPath(contr,fromIndex, toIndex);
            
//             ////GD.Print("Index of vector ",HexGrid.IndexOfVec(resourceMap.GetPointPosition(pos)));
//             //this.AssignedWorkers[0].movementQueue
//             //this.AssignedWorkers = null;
            
//             //grid.pathFinder.ArePointsConnected()      
//     }

//     public void On_WorkerAmountChange(int amount)
//     {
//         //GD.Print("worker amount: ", this.workerAmount, " chanigng with ", amount, " worker");
//         if(amount <0)
//         {
//             if(workerAmount+amount >= 0)
//             {
//                 CharacterController c =  AssignedWorkers[0];
//                 AssignedWorkers.RemoveAt(0);
//                 AvailableWorkers.Add(c);
                
                
//             }
//             else{
//                 return;
//             }
//         }
//         else{
//             if(AvailableWorkers?.Count > 0)
//             {
//                 //GD.Print("Workers are available, setting 1 to Idle");
//                 CharacterController c = AvailableWorkers[0];
//                 c.character.Connect("TargetHexReached", c, "On_DestinationReached");
//                 c.Connect("TargetHexReached", this, nameof( On_DestinationReached));
//                 c.Connect("CapacityFull", this, nameof(On_CapacityFull));
//                 c.Connect("HexDepleted", this, "On_HexDepleted");
//                 c.Connect("ResourceDepleted", this, "On_ResourceDepleted");
//                 c.Connect("RestCompleted",this, "On_RestCompleted");
//                 AvailableWorkers.RemoveAt(0);   
//                 AssignedWorkers.Add(c);
//                 IdleWorkers.Enqueue(c);
                
//             }  
//             else {
//                 return;
//             }
        
//         }
//         workerAmount += amount;
//         if(workerAmount == 3)
//         {
//             EmitSignal("ResourceTaskAssignmentComplete");
//         }
//         this.optionBox.GetNode<Label>("TextureRect/MarginContainer/VBoxContainer/HBoxContainer2/Label").Text = "Workers: "+workerAmount;
//         //this.optionBox.GetNode<Label>("TextureRect/MarginContainer/VBoxContainer/HBoxContainer2/Label").Text = "Workers: "+workerAmount;
//     }

//     public void On_RestCompleted(CharacterController c){
//         this.IdleWorkers.Enqueue(c);
//     }

//     public void On_ResourceDepleted(CharacterController c, BasicResource res)
//     {
//         //GD.Print("on depleted res", res);
//         if(BusyResources.ContainsKey(res))
//             BusyResources.Remove(res);
//         c.currentTargetResource = null;
//         GarbageResources.Enqueue(res);        
//     }
//     public void On_HexDepleted(HexHorizontalTest hex)
//     {
        
//         int index = HexToInt[hex];
//         //GD.Print("depleted hex with index", index);
//         //resourceMap.RemovePoint(index);
//         resourceMap.SetPointDisabled(index, true);
//         BusyHexes.Remove(hex);
//         GarbageResources.Enqueue(hex.HexEnv);
//     }

//     public void On_DestinationReached(CharacterController c, string task)
//     {
//         if(c.hasEnergy)
//         {
//             if(c.workerState == State.WorkerState.Gather)
//             {
//                 //c.GatherResource();
//                 WorkerGoToDrop(c);
//             }
//             else if(c.workerState == State.WorkerState.Dropping)
//             {
//                 WorkerDrop(c);
//                 WorkerGoToGather(c);
//             }
//         }
//         else {
//             //c.Rest();
//         }
//         //IdleWorkers.Enqueue(c);
//         // if(task == "Gather")
//         // {
//         //     c.GatherResource();
            
//         // }
//         // else if(task  == "Drop")
//         // {
//         //     BasicResource b = c.resType;
//         //     //GD.Print("Emitting signala with ", c.resType, c.capacity);
//         //     EmitSignal(nameof(ResourceDrop),b,c.capacity);
//         //     c.capacity = 0;
//         //     //this.AssignedWorkers.Add(c);
//         //     c.CurrentTask = "Gather";
//         //     QueueNextTask(c);
//         // }     
        
//     }

//     public void QueueNextTask(CharacterController c){
//         int pos = resourceMap.GetClosestPoint(this.ResourcePost.buildingAsset.Position);
        
//         Vector2 vec = resourceMap.GetClosestPositionInSegment(this.ResourcePost.buildingAsset.Position);
//         //GD.Print("closest vector to post pos: ", this.ResourcePost.buildingAsset.Position, " is ", resourceMap.GetPointPosition(pos));
//         //GD.Print("closest vector as ID: ", resourceMap.GetPointPosition(pos));
//         //GD.Print("Worker Position: ", c.character.Position);
//         ////GD.Print("Worker Position: ", AssignedWorkers[0].Position);
//         int toIndex = HexGrid.IndexOfVec(resourceMap.GetPointPosition(pos));
//         int fromIndex = HexGrid.IndexOfVec(c.character.currentTestTile.Position);

//        // HexHorizontalTest hex=  grid.storedHexes[toIndex];

//         //c.character.TargetHex = hex;
//         c.CurrentTask = "Gather";
//         //this.AssignedWorkers.Remove(contr);
//         //AssignedWorkers[0].
//         //GD.Print("toIndex: ", toIndex, "   fromIndex: ", fromIndex);
//         PopulateCharacterPath(c,fromIndex, toIndex);
//     }



//     public void On_Response(bool accepted, OptionBox option){
//         this.optionBox.Visible = false;
//         EmitSignal(nameof(CloseDialog));
//     }

//     public void OnClick(){
//         this.optionBox.Visible = true;
//     }

//     public void MapResourcePoints()
//     {
//         resourceMap = new AStar2D();
//         int i =0;
//         foreach(KeyValuePair<Vector2, Node2D> p in HexGrid.resourcePositions)
//         {
//             HexHorizontalTest hex = HexGrid.storedHexes[HexGrid.IndexOfVec(p.Key)];
//             if(hex.isBasicResource){
//                 int j = 0;
//                 resourceMap.AddPoint(i, p.Key);
//                 HexToInt.Add(hex,i);
//                 var points  = resourceMap.GetPoints();
//                 foreach(var point in points)
//                 {
//                     if(i!=j)
//                         resourceMap.ConnectPoints(i,j++);
//                 }
//                 ++i;
//             // resourceMap.AddPoint(i++,p.Key);
            
//             }
//         }

//         //resourceMap.GetPoints

//         //resourceMap.GetClosestPoint
//     }

//     public void On_CapacityFull(CharacterController c){
//         //c.workerState = State.WorkerState.GatherFinish;
//         //IdleWorkers.Enqueue(c);
//         // int pos = resourceMap.GetClosestPoint(this.ResourcePost.buildingAsset.Position);

//         // //Vector2 vec = resourceMap.GetClosestPositionInSegment(this.ResourcePost.buildingAsset.Position);
//         // //GD.Print("closest vector to post pos: ", this.ResourcePost.buildingAsset.Position, " is ", resourceMap.GetPointPosition(pos));
//         // //GD.Print("closest vector as ID: ", resourceMap.GetPointPosition(pos));
//         // //GD.Print("Worker Position: ", c.character.Position);
//         // ////GD.Print("Worker Position: ", AssignedWorkers[0].Position);
//         // int toIndex = HexGrid.IndexOfVec(this.ResourcePost.buildingAsset.Position);
//         // int fromIndex = HexGrid.IndexOfVec(c.character.currentTestTile.Position);
//         // HexHorizontalTest hex=  grid.storedHexes[toIndex];
//         // c.character.TargetHex = hex;
//         // PopulateCharacterPath(c,fromIndex, toIndex);
//         // c.CurrentTask = "Drop";
//     }


//     public void PopulateCharacterPath(CharacterController c, int from, int to)
//     {

//         //GD.Print("populating charactyer path with c: ", c, " aand path length: ",grid.pathFinder.GetPointPath(from, to).Length);
//         Queue<Vector2> qT = new Queue<Vector2>();
//         // foreach( Vector2 v in grid.pathFinder.GetPointPath(from, to))
//         //     { 
                
//         //         //GD.Print("populating character path with : ",v);              
//         //         qT.Enqueue(v);                
//         //     }
//        // c.movementQueue = qT;
//     }


//     public override void _PhysicsProcess(float delta)
//     {
//          if(GarbageResources.Count > 0)
//         {
//             Node Garbage = GarbageResources.Dequeue();
//             if(IsInstanceValid(Garbage)){
//                 Garbage?.GetParent().RemoveChild(Garbage);           
//                 Garbage?.QueueFree();
//             }
//         }


//         if(this.AssignedWorkers!=null && resourceMap != null && this.AssignedWorkers.Count>0)
//         {
//             if(IdleWorkers.Count > 0 )//&& ( WorkerThread.ThreadState == ThreadState.Unstarted || WorkerThread.ThreadState == ThreadState.Stopped))
//             {
//                 GD.Print("Starting new worker thread");
//                 NextIdleWorker = IdleWorkers.Dequeue();
//                 NextIdleWorker.workerState = State.WorkerState.Default;
//                 //GD.Print("Starting worker thread for worker: ", NextIdleWorker);
//                 //GD.Print("Thread State: ", WorkerThread.ThreadState);
//                 WorkerThread = new System.Threading.Thread(new ThreadStart(ParseWorkerState));
//                 WorkerThread.Start();
//             }
//         }
//     }


}   