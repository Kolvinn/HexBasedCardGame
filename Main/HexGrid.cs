using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
public class HexGrid 
{
    public int width = 6;
	public int height = 6;

	public Vector2 envBounds;
	
	public Dictionary<HexCell1,int> tiles;

    [PostLoad]
    public  AStar2D pathFinder;

	public Dictionary<Vector2, Node2D> resourcePositions = new Dictionary<Vector2, Node2D>();


	float mapX;
	float mapY;
    HexCell1[] cells;
	int index =0;
	Vector2 mapSpace;

	public ReferenceRect spawnArea;

	public static HexHorizontalTest hoveredHex;
	public Dictionary<int,HexHorizontalTest> storedHexes = new Dictionary<int, HexHorizontalTest>();
	public List<Vector2> storedVectors = new List<Vector2>();
 
    
	private YSort parent;
	public HexGrid(Vector2 mapSize, YSort parent, ReferenceRect envBounds)
	{
		this.spawnArea = envBounds;

		this.parent = parent;
		this.mapSpace =mapSize;
		pathFinder = new AStar2D();

		System.Random rand = new System.Random();
		rand.NextDouble();

		Vector2 screenSize = new Vector2(1920,1080);
		index = 0;
		

    	mapX = (screenSize.x/2) - (mapSpace.x/2);

        mapY = (screenSize.y/2) - (mapSpace.y/2);
		

		double doubleX = (double)mapX+mapSpace.x;
		double doubleY = (double)mapY+mapSpace.y;
		double randomX = doubleX*rand.NextDouble();
		double randomY = doubleY*rand.NextDouble();
		
		//float randX = rand.RandfRange(mapX, mapX+mapSpace.x);
		//float randY = rand.RandfRange(mapY, mapY+mapSpace.y);
		//Params.Print("screen space rec: {0} {1} {2} {3} ",mapX, mapX+mapSpace.x,mapY, mapY+mapSpace.y);
		////GD.Print("random start pos = ",randX, ",",randY);
		////GD.Print(mapX ," ",mapY);
		////GD.Print(mapX+((byte)mapSpace.x), " ",mapY+mapSpace.y);
		RecurseGenMapTile((float)Math.Max(mapX,randomX), (float)Math.Max(mapY,randomY),Vector2.Zero, true);
	}

	public bool PointInSpawn(Vector2 vec)
	{
		return (vec.x >= spawnArea.RectPosition.x 
		&& vec.y >= spawnArea.RectPosition.y
		&& vec.x <= spawnArea.RectPosition.x +spawnArea.RectSize.x
		&& vec.y <= spawnArea.RectPosition.y +spawnArea.RectSize.y);
	}
	public Vector2[] BoundingBox(){
		return new Vector2[]{
			new Vector2(mapX,mapY),
			new Vector2(mapX+mapSpace.x,mapY),
			new Vector2(mapX+mapSpace.x,mapY+mapSpace.y),
			new Vector2(mapX,mapY+mapSpace.y),
			};
	}
	public Vector2 HasStoredVector(float x, float y){
		return storedVectors.First(item => (item.x-2)<x && item.x+2>x && (item.y-2)<y && item.y+2>y ) ;
	}

	public int IndexOfVec(Vector2 vec){
		Vector2 found = storedVectors.FirstOrDefault(item => (item.x-2)<vec.x && item.x+2>vec.x && (item.y-2)<vec.y && item.y+2>vec.y );
		return storedVectors.IndexOf(found);
	}


	/// <summary>
	/// Recursively iterate around a starting hex position to populate map with different hex types
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="connectVector"></param>
	/// <param name="startTile"></param>
	public void RecurseGenMapTile(float x, float y, Vector2 connectVector, bool startTile = false){
		if(x< mapX|| x> mapX+mapSpace.x)
			return ;

		if(y< mapY|| y> mapY+mapSpace.y)
			return ;

		Vector2 vec = new Vector2(x,y);
		//
		
		//if we've already been here, connect to the tile that is stored at that vector
		if(IndexOfVec(vec)>=0)
		{	
			HexHorizontalTest value1 = null, value2 =null;
			storedHexes.TryGetValue(IndexOfVec(vec), out value1);
			storedHexes.TryGetValue(IndexOfVec(connectVector), out value2);

			//both tiles must be visible
			if(value1!= null && value1.Visible && value2 !=null && value2.Visible && (!value2.isBasicResource && !value1.isBasicResource))
			{
				pathFinder.ConnectPoints(IndexOfVec(vec),IndexOfVec(connectVector));
			}
			return;
		}
	
		
				
		HexHorizontalTest tile = GenTileType(vec);
		
		storedVectors.Add(vec);
		int indexOfVec = IndexOfVec(vec);

		//add at now stored vector
		storedHexes.Add(indexOfVec,tile);
		
		//if tile has resource/environment on it
		if(tile.HexEnv != null)
			resourcePositions.Add(vec,tile.HexEnv);

		pathFinder.AddPoint(indexOfVec,vec);

		HexHorizontalTest value = null;
		storedHexes.TryGetValue(IndexOfVec(connectVector), out value);

		//if both visible and is not a resource tile
		if(!startTile  && tile.Visible && value.Visible && !value.isBasicResource){

			pathFinder.ConnectPoints(indexOfVec,IndexOfVec(connectVector));
		}
		
		

		////GD.Print("storing vector2:, ",vec, " with tile: ",tile);
		//gen NE
		RecurseGenMapTile(x+(HexMetrics.outerRadius * 1.5f), y-(HexMetrics.innerRadius * tile.Scale.y), vec);

		//gen SE
		RecurseGenMapTile(x+(HexMetrics.outerRadius * 1.5f), y+(HexMetrics.innerRadius * tile.Scale.y),vec);

		//gen S
		RecurseGenMapTile(x, y+(HexMetrics.innerRadius * 2 * tile.Scale.y),vec);

		//gen SW
		RecurseGenMapTile(x-(HexMetrics.outerRadius * 1.5f), y+(HexMetrics.innerRadius * tile.Scale.y),vec);

		//gen NW
		RecurseGenMapTile(x-(HexMetrics.outerRadius * 1.5f), y-(HexMetrics.innerRadius * tile.Scale.y),vec);

		//gen N
		RecurseGenMapTile(x, y+(HexMetrics.innerRadius * 2 * tile.Scale.y),vec);





	}

	 
	public HexHorizontalTest GenTileType(Vector2 vec){
		HexHorizontalTest hex =  Params.LoadScene<HexHorizontalTest>("res://Test/Delete/HexHorizontalTest.tscn");
		
			Random rand = new Random();

			//is now a env hex
			
			if(rand.NextDouble()>=0.5 && PointInSpawn(vec))
			{
				int nextInt = rand.Next(1,12);
				//grass
				if(nextInt == 1 || nextInt>=6)
					hex.HexEnv = Params.LoadScene<YSort>("res://Assets/Environment/RPGW_AncientForest_v1.0/Nodes/grasspatch.tscn");
				else if(nextInt ==2){
					hex.isBasicResource = true;
					hex.HexEnv = Params.LoadScene<YSort>("res://Assets/Environment/RPGW_AncientForest_v1.0/Nodes/rockpatch.tscn");
				}
				else if(nextInt == 3){
					hex.isBasicResource = true;
					hex.HexEnv = Params.LoadScene<YSort>("res://Assets/Environment/RPGW_AncientForest_v1.0/Nodes/LeavesPatch.tscn");
				}
				else if (nextInt == 4)
					hex.HexEnv = LoadRandomTree();
				else{
					hex.isBasicResource = true;
					hex.HexEnv = Params.LoadScene<BasicResource>("res://Assets/Environment/Wood2.tscn");					
				}
			}	
		
		return hex;
	}
	
	public Tree LoadRandomTree(){
		Tree node;
		Random rand = new Random();
		int next = rand.Next(1,5);
		if(next <=3)
		{
			node = Params.LoadScene<Tree>("res://Assets/Environment/RPGW_AncientForest_v1.0/Nodes/Tree"+next +".tscn");
		}
		else if(next == 5)
		{ 
			next = 1;
			node = Params.LoadScene<Tree>("res://Assets/Environment/RPGW_AncientForest_v1.0/Nodes/Tree_Green"+next +".tscn");
		}
		else
		{
			next = 2;
			node = Params.LoadScene<Tree>("res://Assets/Environment/RPGW_AncientForest_v1.0/Nodes/Tree_Green"+next +".tscn");		
		}
		node.Scale = new Vector2(2.5f,2.5f);
		return node;
	}
	public void CreateCell (int x, int z, int i) {

		// Vector3 position;
		// position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		// position.y = 0f;
		// position.z = z * (HexMetrics.outerRadius * 1.5f);

		// HexCell cell = cells[i] = Loader.LoadScene<HexCell>("res://Cells/HexCell.tscn");
		// //cell.Tran .SetParent(transform, false);
		// cell.Translation = position;
        // this.AddChild(cell);
        // //cell.SetText(x.ToString() + "\n" + z.ToString());
        // //GD.Print(position);
	}

	private void GetMapFit()
    {
        //Vector2 mapSpace = new Vector2(1000,1000);
        Vector2 screenSize = new Vector2(1920,1080);

        float mapX = (screenSize.x/2) - (mapSpace.x/2);
        float mapY = (screenSize.y/2) - (mapSpace.y/2);

        //side radius =140
        //top radius = 85.4
     

        Vector2 hexPos  = new Vector2(mapX +140, mapY +85.4f);

        //now we go column by column because our hexes are rotated!
        float tileHeight = HexMetrics.innerRadius * 2;
        float tileYOffset = HexMetrics.innerRadius + mapY;
        float tileYCount =0 ;
		int[,] array;
		List<float> ylist = new List<float>();
		List<float> xlist = new List<float>();
        float limitY = mapY + mapSpace.y;
        while (tileYOffset + (tileYCount * tileHeight ) <=limitY){
			//GD.Print("y: ",tileYOffset + (tileYCount * tileHeight));
			ylist.Add(tileYOffset + (tileYCount * tileHeight));
            ++tileYCount;
        }

        float tileWidth = HexMetrics.outerRadius * 2;
        float tileXOffset = HexMetrics.outerRadius +mapX;
        float tileXCount =0 ;

        float limitX = mapX + mapSpace.x;

        while (tileXOffset + (tileXCount * tileWidth ) <=limitX){
			//GD.Print("x: ",tileXOffset + (tileXCount * tileWidth ));
			xlist.Add(tileXOffset + (tileXCount * tileWidth ));
            ++tileXCount;
        }
		//GD.Print(tileXCount,"   ",tileYCount);

    }
    
}
