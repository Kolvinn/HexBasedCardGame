public class HexConnection 
{
    public enum ConnectionCode 
    {
        NE,
        N,
        S,
        SE,
        SW,
        NW
    }

    public ConnectionCode code;
    public HexHorizontalTest hex;
    public HexConnection(HexHorizontalTest connection, ConnectionCode code)
    {
        this.hex = connection;
        this.code = code;
    }

    public string GetNodeConnectionString()
    {
        return this.code +"_Border";
    }
}