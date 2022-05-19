using Godot;
public class WorkerJob
{
    protected bool isPersistant;

    public string JobId;

    public State.WorkerJobType jobType;

    //public Node2D TargetObject;

    protected HexHorizontalTest TargetHex;

    protected HexHorizontalTest OriginHex;

    protected StorageBuilding StorageBuilding;

    protected Building AssignedBuilding;


    public virtual bool IsJobStillValid(CharacterController character)
    {
        GD.Print("please tell me this isn't broken");
        return false;
    }

    public virtual bool IsJobStillValid(CharacterController character, HexGrid grid)
    {
        return false;
    }

    public virtual CharacterBaseState GetNextJobAction(CharacterController character)
    {
        return null;
    }
    public virtual CharacterBaseState HandleCharacterState(CharacterController character, CharacterBaseState state)
    {
        return null;
    }
    public virtual HexHorizontalTest FetchTargetHex()
    {
        return null;
    }
    public virtual Vector2 FetchTargetVector()
    {   
        return Vector2.Zero;
    }
}