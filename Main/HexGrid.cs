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
    private static AStar2D pathFinder;

	public static Dictionary<Vector2, Node2D> resourcePositions = new Dictionary<Vector2, Node2D>();

    public static  Dictionary<HexHorizontalTest, int> ResourceHexToInt = new Dictionary<HexHorizontalTest, int>();


	public static AStar2D ResourceMap = new AStar2D();

	float mapX;
	float mapY;
    HexCell1[] cells;
	int index =0;
	Vector2 mapSpace;

	public ReferenceRect spawnArea;

	public static HexHorizontalTest hoveredHex;
	public static Dictionary<int,HexHorizontalTest> storedHexes = new Dictionary<int, HexHorizontalTest>();
	public static List<Vector2> storedVectors = new List<Vector2>();
 
	public static  Vector2 lastY = new Vector2(100000,100000);
	public Vector2 biggestY = new Vector2(0,0);
	public static Vector2 smallestX, biggestX; 

	public Vector2[] polygon;

	private bool isBattle = false;  

	public HexGrid(Vector2 mapSize , bool isBattle = false)
	{
		//this.spawnArea = envBounds;
		this.isBattle = isBattle;
		this.mapSpace =mapSize;
		pathFinder = new AStar2D();
		storedHexes = new Dictionary<int, HexHorizontalTest>();
		storedVectors = new List<Vector2>();

		System.Random rand = new System.Random();
		rand.NextDouble();

		Vector2 screenSize = new Vector2(1920,1080);

		index = 0;
		
		lastY = new Vector2(float.MaxValue,float.MaxValue);
		biggestY = new Vector2(float.MinValue,float.MinValue);
		smallestX = new Vector2(float.MaxValue,float.MaxValue);
		biggestX = new Vector2(float.MinValue,float.MinValue);
    	mapX = (screenSize.x/2) - (mapSpace.x/2);

        mapY = (screenSize.y/2) - (mapSpace.y/2);
		

		double doubleX = (double)mapX+mapSpace.x;
		double doubleY = (double)mapY+mapSpace.y;
		double randomX = doubleX*rand.NextDouble();
		double randomY = doubleY*rand.NextDouble();
		
		//float randX = rand.RandfRange(mapX, mapX+mapSpace.x);
		//float randY = rand.RandfRange(mapY, mapY+mapSpace.y);
		//Params.Print("screen space rec: {0} {1} {2} {3} ",mapX, mapX+mapSpace.x,mapY, mapY+mapSpace.y);
		GD.Print("random start pos = ",randomX, ",",randomY);
		GD.Print(mapX ," ",mapY);
		//////GD.Print(mapX+((byte)mapSpace.x), " ",mapY+mapSpace.y);
		RecurseGenMapTile((float)Math.Max(mapX,randomX), (float)Math.Max(mapY,randomY),Vector2.Zero, true);
		//MapResourcePoints();
	}

	
	public HexGrid(Vector2[] bounds)
	{
		//this.mapSpace =mapSize;
		pathFinder = new AStar2D();
		storedHexes = new Dictionary<int, HexHorizontalTest>();
		storedVectors = new List<Vector2>();
		this.polygon = bounds;

		//System.Random rand = new System.Random();
		//rand.NextDouble();

		//index = 0;

		//Rect2 bounding = new Rect2(polygon[1].x
		//,polygon[1].y
		//,Math.Abs(polygon[1].x) + Math.Abs(polygon[2].x)
		//,Math.Abs(polygon[1].y) + Math.Abs(polygon[2].y));

		//bou
		
		lastY = new Vector2(float.MaxValue,float.MaxValue);
		biggestY = new Vector2(float.MinValue,float.MinValue);
		smallestX = new Vector2(float.MaxValue,float.MaxValue);
		biggestX = new Vector2(float.MinValue,float.MinValue);

		Vector2 screenSize = new Vector2(1920,1080);
    	mapX = (screenSize.x/2) - (mapSpace.x/2);
        mapY = (screenSize.y/2) - (mapSpace.y/2);
		
		
		var doubleX = mapX+mapSpace.x;
		var doubleY = mapY+mapSpace.y;
		RecurseGenMapTile(doubleX,doubleY,Vector2.Zero, true,true);

		//double doubleX = (double)mapX+mapSpace.x;
		//double doubleY = (double)mapY+mapSpace.y;
		//double randomX = doubleX*rand.NextDouble();
		//double randomY = doubleY*rand.NextDouble();
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

	public static HexHorizontalTest FetchHexAtIndex(int index)
	{
		return storedHexes[index];
	}

	/// <summary>
	/// Returns the AStar int index of the given vector within a +-2 pixel error margin
	/// </summary>
	/// <param name="vec"></param>
	/// <returns></returns>
	public static int IndexOfVec(Vector2 vec){
		Vector2 found = storedVectors.FirstOrDefault(item => (item.x-2)<vec.x && item.x+2>vec.x && (item.y-2)<vec.y && item.y+2>vec.y );
		return storedVectors.IndexOf(found);
	}


	/// <summary>
	/// Recursively iterate around a starting hex position to populate map with different hex types
	/// </summary>
	/// <param name="x"> the new x coord of tile to be created</param>
	/// <param name="y">  the new y coord of tile to be created</param>
	/// <param name="connectVector"></param>
	/// <param name="startTile"></param>
	public HexHorizontalTest RecurseGenMapTile(float x, float y, Vector2 connectVector, bool startTile = false, bool polyBounds = false)
	{
		if(!polyBounds){
			if(x< mapX|| x> mapX+mapSpace.x){
			//GD.Print("Outside map bounds returning null");
			return null;
			}

			if(y< mapY|| y> mapY+mapSpace.y){
				//GD.Print("Outside map bounds returning null");
				return null;
			}
		}
		else
		{
			if (!IsPointInPolygon(new Vector2(x,y)))
				return null;
		}

		Vector2 newHexVector = new Vector2(x,y);
		int existingIndex = IndexOfVec(newHexVector);
		//
		//if we've already been here, connect to the tile that is stored at that vector
		if(existingIndex>=0)
		{	
			//GD.Print("returning existing hex");	 			
			return storedHexes[existingIndex];
			
			// storedHexes.TryGetValue(IndexOfVec(connectVector), out value2);

			// value1.connections.Add(value2);
			// value2.connections.Add(value1);

			//both tiles must be visible
			// if(value1!= null && value1.Visible && value2 !=null && value2.Visible)
			// {
			// 	value1.connections.Add(value2);
			//     value2.connections.Add(value1);
			// 	pathFinder.ConnectPoints(IndexOfVec(newHexVector),IndexOfVec(connectVector));
			// }
			
		}
	
		
				
		HexHorizontalTest tile = GenTileType(newHexVector);
		SetMaxCoords(newHexVector);

		storedVectors.Add(newHexVector);
		int indexOfVec = IndexOfVec(newHexVector);

		//add at now stored vector
		storedHexes.Add(indexOfVec,tile);
		
		// //if tile has resource/environment on it
		// if(tile.HexEnv != null)
		// 	resourcePositions.Add(newHexVector,tile.HexEnv);

		// pathFinder.AddPoint(indexOfVec,newHexVector);

		// HexHorizontalTest value = null;
		// storedHexes.TryGetValue(IndexOfVec(connectVector), out value);

		// //if both visible and is not a resource tile
		// if(!startTile  && tile.Visible && value.Visible ){

		// 	pathFinder.ConnectPoints(indexOfVec,IndexOfVec(connectVector));
			

		// }
		//if(value!=null && tile != null){
			//tile.connections.Add(value);
			//value.connections.Add(tile);
		//}
		
		//gen NE
		var NE = RecurseGenMapTile(x+(HexMetrics.outerRadius * 1.5f), y-(HexMetrics.innerRadius * 0.6f), newHexVector, polyBounds:polyBounds);
		tile.AddConnection(new HexConnection(NE, HexConnection.ConnectionCode.NE));

		//gen SE
		var SE = RecurseGenMapTile(x+(HexMetrics.outerRadius * 1.5f), y+(HexMetrics.innerRadius *  0.6f),newHexVector,polyBounds:polyBounds);
		tile.AddConnection(new HexConnection(SE, HexConnection.ConnectionCode.SE));

		//gen S
		var S = RecurseGenMapTile(x, y+(HexMetrics.innerRadius * 2 * 0.6f),newHexVector,polyBounds:polyBounds);
		tile.AddConnection(new HexConnection(S, HexConnection.ConnectionCode.S));

		//gen SW
		var SW = RecurseGenMapTile(x-(HexMetrics.outerRadius * 1.5f), y+(HexMetrics.innerRadius * 0.6f),newHexVector,polyBounds:polyBounds);
		tile.AddConnection(new HexConnection(SW, HexConnection.ConnectionCode.SW));

		//gen NW
		var NW = RecurseGenMapTile(x-(HexMetrics.outerRadius * 1.5f), y-(HexMetrics.innerRadius * 0.6f),newHexVector,polyBounds:polyBounds);
		tile.AddConnection(new HexConnection(NW, HexConnection.ConnectionCode.NW));

		//gen N
		var N = RecurseGenMapTile(x, y-(HexMetrics.innerRadius * 2 * 0.6f),newHexVector,polyBounds:polyBounds);
		tile.AddConnection(new HexConnection(N, HexConnection.ConnectionCode.N));

		// if(S == SE && S == SW && S == null)
		// {
		// 	GD.Print("found tile at bot. North tile is: ", N);
		// }

		return tile;


	}

	public void SetMaxCoords(Vector2 vec)
	{
		if(vec.y<lastY.y){
			lastY = vec;
			GD.Print("Creating last y at pos: ",lastY);
		}		
		if(vec.y> biggestY.y){
			biggestY =vec;
			//GD.Print("Creating biggest y at pos: ", biggestY);
		}
		if(vec.x<smallestX.x){
			smallestX = vec;
			//GD.Print("Creating last y at pos: ",lastY);
		}		
		if(vec.x> biggestX.x){
			biggestX =vec;
			//GD.Print("Creating biggest y at pos: ", biggestY);
		}
	}

	public bool IsPointInPolygon( Vector2 p)
	{
		//GD.Print("skjdhfkjhskdfhjskd");
		double minX = polygon[ 0 ].x;
		double maxX = polygon[ 0 ].x;
		double minY = polygon[ 0 ].y;
		double maxY = polygon[ 0 ].y;
		for ( int i = 1 ; i < polygon.Length ; i++ )
		{
			Vector2 q = polygon[ i ];
			minX = Math.Min( q.x, minX );
			maxX = Math.Max( q.x, maxX );
			minY = Math.Min( q.y, minY );
			maxY = Math.Max( q.y, maxY );
		}

		if ( p.x < minX || p.x > maxX || p.y < minY || p.y > maxY )
		{
			
			return false;
		}

		// https://wrf.ecse.rpi.edu/Research/Short_Notes/pnpoly.html
		bool inside = false;
		for ( int i = 0, j = polygon.Length - 1 ; i < polygon.Length ; j = i++ )
		{
			if ( ( polygon[ i ].y > p.y ) != ( polygon[ j ].y > p.y ) &&
				p.x < ( polygon[ j ].x - polygon[ i ].x ) * ( p.y - polygon[ i ].y ) / ( polygon[ j ].y - polygon[ i ].y ) + polygon[ i ].x )
			{
				inside = !inside;
			}
		}
		//GD.Print("tile is in bounds: ", inside, ". Vector: ", p);

		return inside;
	}
	
	public static HexHorizontalTest GetClosestResourceHex(Vector2 position)
	{
		int index = ResourceMap.GetClosestPoint(position);
		GD.Print("int is: ",index);
        return ResourceHexToInt.First(item => item.Value ==index).Key;//HexGrid.FetchHexAtIndex(index);//ResourceHexToInt.First(item => item.Value ==index).Key;
	}

	
	private void MapResourcePoints()
    {
        ResourceMap = new AStar2D();
        int i =0;
        foreach(KeyValuePair<Vector2, Node2D> p in resourcePositions)
        {
            HexHorizontalTest hex = storedHexes[IndexOfVec(p.Key)];
            if(hex.isBasicResource){
                int j = 0;
                ResourceMap.AddPoint(i, p.Key);
                ResourceHexToInt.Add(hex,i);
                var points  = ResourceMap.GetPoints();
                foreach(var point in points)
                {
                    if(i!=j)
                        ResourceMap.ConnectPoints(i,j++);
                }
                ++i;
            // resourceMap.AddPoint(i++,p.Key);
            
            }
        }

        //resourceMap.GetPoints

        //resourceMap.GetClosestPoint
    }

	public static Queue<Vector2> PopulateTravelPath(Vector2 fromVec, Vector2 targetVec)
    {
        
        int toIndex = HexGrid.IndexOfVec(targetVec);
        int fromIndex = HexGrid.IndexOfVec(fromVec);       
        Queue<Vector2> qT = new Queue<Vector2>();
        //HexHorizontalTest hex=  grid.storedHexes[toIndex];
        if(toIndex >-1 && fromIndex >-1)
        {
            foreach( Vector2 v in pathFinder.GetPointPath(fromIndex, toIndex))
            {             
                qT.Enqueue(v);                
            }
            //character.movementQueue = qT;
        }
		return qT;
    }

	public HexHorizontalTest GenTileType(Vector2 vec){
		HexHorizontalTest hex =  Params.LoadScene<HexHorizontalTest>("res://Test/Delete/HexHorizontalTest.tscn");
		return hex;
	}

	public void GenEnvironment()
	{

		var mudProb = 50;
		var waterProb = 50;
		var envProb = 20;
		var adjacentProbMultiplyer = 2;

		List<HexHorizontalTest> adjacentMud = new List<HexHorizontalTest>();
		
		List<HexHorizontalTest> adjacentWater = new List<HexHorizontalTest>();

		Random rand = new Random();
		foreach (var sm in storedHexes)
		{
			mudProb = 50;
			waterProb = 50;
			var hex = sm.Value;

			if(adjacentMud.Contains(hex)){
				mudProb = mudProb * adjacentProbMultiplyer;
				waterProb = 0;
			}

			else if(adjacentWater.Contains(hex)){
				waterProb = waterProb *adjacentProbMultiplyer;
				mudProb = 0;
			}

			if(rand.Next(1,100) <=envProb) //20% chance for tile to be active
			{
				hex.GetNode<Sprite>("suelo_sin_linea").Visible = false;
				if(mudProb == 0)
				{
					adjacentWater.AddRange(hex.connections.Select(item=> item.hex));
					hex.GetNode<Sprite>("water_tile").Visible = true;
				}
				else if (waterProb == 0)
				{			
					adjacentMud.AddRange(hex.connections.Select(item=> item.hex));		
					hex.GetNode<Sprite>("mud_tile").Visible = true;
				}
				else if(rand.Next(1,100) <=50)
				{
					adjacentMud.AddRange(hex.connections.Select(item=> item.hex));
					hex.GetNode<Sprite>("mud_tile").Visible = true;
				}
				else
				{					
					adjacentWater.AddRange(hex.connections.Select(item=> item.hex));	
					hex.GetNode<Sprite>("water_tile").Visible = true;
				}
			}
			else
			{
				
				RandomGenResource(hex);

				

			}
		}
	}

	public void RandomGenResource(HexHorizontalTest hex){
		Random rand = new Random();


		//48,48 is test coords for random gen positions
		
			if(rand.NextDouble()<=0.3)
			{
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
				

				int nextInt = rand.Next(1,4);
				//grass
				if(nextInt == 1)
				{
					var tree = LoadRandomTree();
					tree.Position = position;
					hex.AddChild(tree);
				}
					//hex.HexEnv = Params.LoadScene<YSort>("res://Assets/Environment/RPGW_AncientForest_v1.0/Nodes/grasspatch.tscn");
				else if(nextInt ==2){
					//hex.isBasicResource = true;
					var node = Params.LoadScene<BasicResource>("res://Assets/Environment/Rocks/Rock.tscn");
					node.Position = position;//hex.GetNode<s
					hex.AddChild(node);
				}
				else if(nextInt == 3){
					hex.isBasicResource = true;
					hex.HexEnv = Params.LoadScene<BasicResource>("res://Assets/Environment/Wood2.tscn");	
					hex.HexEnv.Position = position;//hex.GetNode<s
					hex.AddChild(hex.HexEnv);
					//hex.isBasicResource = true;
					//hex.HexEnv = Params.LoadScene<YSort>("res://Assets/Environment/RPGW_AncientForest_v1.0/Nodes/LeavesPatch.tscn");
				}
				else{
					//hex.isBasicResource = true;
					//hex.HexEnv = Params.LoadScene<BasicResource>("res://Assets/Environment/Wood2.tscn");					
				}
			}	
	}
	
	public static Tree LoadRandomTree(){
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
        // ////GD.Print(position);
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
			////GD.Print("y: ",tileYOffset + (tileYCount * tileHeight));
			ylist.Add(tileYOffset + (tileYCount * tileHeight));
            ++tileYCount;
        }

        float tileWidth = HexMetrics.outerRadius * 2;
        float tileXOffset = HexMetrics.outerRadius +mapX;
        float tileXCount =0 ;

        float limitX = mapX + mapSpace.x;

        while (tileXOffset + (tileXCount * tileWidth ) <=limitX){
			////GD.Print("x: ",tileXOffset + (tileXCount * tileWidth ));
			xlist.Add(tileXOffset + (tileXCount * tileWidth ));
            ++tileXCount;
        }
		////GD.Print(tileXCount,"   ",tileYCount);

    }
    
}
