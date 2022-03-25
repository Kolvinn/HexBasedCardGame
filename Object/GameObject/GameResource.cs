public interface GameResource{
    public int TotalResource{
        get;set;
    }

    public string ResourceType {
        get;set;
    }

    public string RequiredAction
    {
        get;set;
    }

    public State.ToolLevel ToolLevel 
    {
        get;set;
    }

    /// <summary>
    ///  0 == locked
    ///  1 == available
    /// </summary>
    public enum ResourceState
    {
        Locked,
        Available
    }

    ResourceState resourceState
    {
        get;set;
    }

    
}