using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public class BiomeSpawner 
{
    
    Random rand = new Random();
    public BiomeSpawner()
    {
        //
    }

    public Biome SpawnNewBiome()
    {
        var next = rand.Next(1,4);

        if(next ==1)
        {
            //grassy
        }
        else if(next ==2)
        {
            //earth
        }
        else
        {
            //forest
        }
        return null;
    }

    public Biome SpawnGrassyBiome()
    {
        YSort botlayer = new YSort();
        YSort hexLayer = new YSort();
        YSort terrainLayer = new YSort();
        var riverChance = 0.5;
        
        
       // var hex = Params.LoadScene<HexHorizontalTest>("res://Test/Delete/HexHorizontalTest.tscn");
        
        HexGrid grid = new HexGrid(new Vector2(3000,3000));
        GD.Print("spawned first grid");
        foreach(KeyValuePair<int,HexHorizontalTest> entry in HexGrid.storedHexes)
        {
            var hex = entry.Value;
            hex.Position = HexGrid.storedVectors[entry.Key];
            hex.GetNode<Sprite>("suelo_sin_linea").Visible = false;           
			hex.GetNode<Sprite>("water_tile").Visible = true;                    
			hex.GetNode<Sprite>("mud_tile").Visible = false;
            
            botlayer.AddChild(hex);
            hex.GetNode<CollisionPolygon2D>("HitBox").Disabled = true;
        }
        
        grid = new HexGrid(new Vector2(1600,1200));

        foreach(KeyValuePair<int,HexHorizontalTest> entry in HexGrid.storedHexes)
        {

            var hex = entry.Value;           
            hex.Position = HexGrid.storedVectors[entry.Key];
            hex.GetNode<Sprite>("suelo_sin_linea").Visible = true;           
			hex.GetNode<Sprite>("water_tile").Visible = false;                    
			hex.GetNode<Sprite>("mud_tile").Visible = false;
            //GD.Print("Adding hex");
            hexLayer.AddChild(hex);
        } 

        //RIVERRRRR
        foreach(KeyValuePair<int,HexHorizontalTest> entry in HexGrid.storedHexes)
        {
            if(rand.NextDouble()>=0.5)
            {   
                
                List<HexHorizontalTest> visitedHexes = new List<HexHorizontalTest>();
                var hex = entry.Value;
                CreateRiver(hex,visitedHexes);
                
                var power = Params.LoadScene<AnimatedSprite>("res://Assets/Sprites/Animation Pack/WaterPowerAnimation.tscn");
                var pHex =  GetRandomWaterPowerSpawn(visitedHexes);
                power.Position = pHex.Position;
                pHex.powertilestring = "water";
                power.Position += new Vector2(0,-50);
                terrainLayer.AddChild(power);
                break;
            }
        } 
        //hexLayer.Rotate(300);
        //spawn grass and trees
        foreach(KeyValuePair<int,HexHorizontalTest> entry in HexGrid.storedHexes)
        {
            if(entry.Value.Visible && rand.NextDouble()<=0.3)
            {   
                CreateTerrain(entry.Value);
                terrainLayer.AddChild(entry.Value.HexEnv);
            }
        } 
        

        //spawn
        //  foreach(KeyValuePair<int,HexHorizontalTest> entry in HexGrid.storedHexes)
        // {
        //     if(entry.Value.Visible && rand.NextDouble()<=0.3)
        //     {   
        //         CreateTerrain(entry.Value);
        //         terrainLayer.AddChild(entry.Value.HexEnv); 
        //     }
        // } 

        // foreach(KeyValuePair<int,HexHorizontalTest> entry in HexGrid.storedHexes)
        // {
        //     if(entry.Value.Visible && rand.NextDouble()<=0.3)
        //     {   
        //         CreateTerrain(entry.Value);
        //         terrainLayer.AddChild(entry.Value.HexEnv);
        //     }
        // } 


        //create hitboxes


        
        return new Biome(botlayer, hexLayer, terrainLayer);   
    }

    public HexHorizontalTest GetRandomWaterPowerSpawn(List<HexHorizontalTest> riverTiles)
    {
        HexHorizontalTest returnHex =  null;
        while(returnHex == null)
        {
            var hex = riverTiles[rand.Next(0, Math.Max(riverTiles.Count-1,1))];
            foreach(var en in hex.connections)
            {
                if(en.hex.Visible)
                    returnHex = en.hex;
            }
            //GetRandomWaterPowerSpawn(riverTiles);
        }
        return returnHex;
        
    }
    
    
    public void CreateRiver(HexHorizontalTest hex, List<HexHorizontalTest> visitedHexes, HexHorizontalTest previous = null)
    {
        if(previous!= null )
            visitedHexes.AddRange(previous.connections.Select(item=> item.hex));

        //now a river tile
        visitedHexes.Add(hex);
        hex.Visible = false;

        foreach(var h in hex.connections.Select(item=> item.hex))
        {
            h.RemoveConnection(hex);
        }     
        foreach(var h in hex.connections.Select(item=> item.hex))
        {
            //make sure one of the connections hasn't been referenced
            if(!visitedHexes.Contains(h))
            {
                //random gen whether the tile will be a river
                if(rand.NextDouble()>=0.6 )
                {
                    CreateRiver(h, visitedHexes, hex);
                    return;
                }
            }
        }
        return;
    }

    public void CreateTerrain(HexHorizontalTest hex)
    {
        if(rand.NextDouble() <=0.7)
        {
            //grass 
            hex.HexEnv = Params.LoadScene<YSort>("res://Assets/Environment/RPGW_AncientForest_v1.0/Nodes/grasspatch.tscn");
            hex.HexEnv.Position = hex.Position;
        }
        else{
            var position = Vector2.Zero;
            var posInt = rand.Next(1,5);

            if(posInt ==1)
                position = new Vector2(48,48);
            if(posInt ==2)
                position = new Vector2(-48,48);
            if(posInt ==3)
                position = new Vector2(-48,-48);
            if(posInt ==4)
                position = new Vector2(48,-48);
            hex.HexEnv = HexGrid.LoadRandomTree();
            hex.HexEnv.Position  = hex.Position + position;
            //trrees
        }
    }
}