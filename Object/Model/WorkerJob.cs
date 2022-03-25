using Godot;
public class WorkerJob
{
    public bool isPersistant;

    public string JobId;

    public State.WorkerJobType jobType;

    //public Node2D TargetObject;

    public HexHorizontalTest TargetHex;

    public HexHorizontalTest OriginHex;

    public StorageBuilding StorageBuilding;

    public Building AssignedBuilding;

    public HexGrid grid;


    public virtual bool IsJobStillValid(CharacterController character)
    {

        return false;
    }

    public virtual void PerformJobAction(CharacterController character)
    {

    }

    public virtual Vector2 FetchTargetVector()
    {   
        return Vector2.Zero;
    }
}