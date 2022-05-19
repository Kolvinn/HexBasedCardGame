using Godot;
using System.Collections.Generic;
public class Biome 
{
    public HexGrid grid;
    public List<BiomeConnection> connections = new List<BiomeConnection>();

    public YSort baseLayer;
    public YSort hexLayer, terrainLayer;
    public Biome(YSort baseLayer, YSort hexLayer, YSort terrainLayer)
    {
        this.hexLayer = hexLayer;
        this.baseLayer = baseLayer;
        this.terrainLayer = terrainLayer;
    }


}