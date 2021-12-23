using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class HexGrid 
{
    public int width = 6;
	public int height = 6;

	public Dictionary<HexCell1,int> tiles;

    [PostLoad]
    private AStar2D pathFinder;


	float mapX;
	float mapY;
    HexCell1[] cells;
	int index =0;
	Vector2 mapSpace;

	public Dictionary<int,HexHorizontalTest> storedHexes = new Dictionary<int, HexHorizontalTest>();
	public List<Vector2> storedVectors = new List<Vector2>();
 
    //     cells = new HexCell[height * width];

	// 	for (int z = 0, i = 0; z < height; z++) {
	// 		for (int x = 0; x < width; x++) {
	// 			CreateCell(x, z, i++);
	// 		}
	// 	}
    

	public HexGrid(Vector2 mapSize){
		this.mapSpace =mapSize;
		pathFinder = new AStar2D();
		RandomNumberGenerator rand = new RandomNumberGenerator();
		Vector2 screenSize = new Vector2(1920,1080);
		index = 0;
    	mapX = (screenSize.x/2) - (mapSpace.x/2);
        mapY = (screenSize.y/2) - (mapSpace.y/2);
		
		float randX = rand.RandfRange(mapX, mapX+mapSpace.x);
		float randY = rand.RandfRange(mapY, mapY+mapSpace.y);
		Params.Print("screen space rec: {0} {1} {2} {3} ",mapX, mapX+mapSpace.x,mapY, mapY+mapSpace.y);
		GD.Print("random start pos = ",randX, ",",randY);
		RecurseGenMapTile(randX, randY,Vector2.Zero, true);
	}

	

	public Vector2 HasStoredVector(float x, float y){
		return storedVectors.First(item => (item.x-2)<x && item.x+2>x && (item.y-2)<y && item.y+2>y ) ;
	}

	public int IndexOfVec(Vector2 vec){
		Vector2 found = storedVectors.FirstOrDefault(item => (item.x-2)<vec.x && item.x+2>vec.x && (item.y-2)<vec.y && item.y+2>vec.y );
		return storedVectors.IndexOf(found);
	}

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
			//int value = ;
			//storedHexes.TryGetValue(indexOfVec, out value);
			pathFinder.ConnectPoints(IndexOfVec(vec),IndexOfVec(connectVector));
			return;
		}
	
		
				
		HexHorizontalTest tile = GenTileType();
		
		storedVectors.Add(vec);
		int indexOfVec = IndexOfVec(vec);

		//add at now stored vector
		storedHexes.Add(indexOfVec,tile);

		pathFinder.AddPoint(indexOfVec,vec);

		if(!startTile)
			pathFinder.ConnectPoints(indexOfVec,IndexOfVec(connectVector));

		GD.Print("storing vector2:, ",vec, " with tile: ",tile);
		//gen NE
		RecurseGenMapTile(x+(HexMetrics.outerRadius * 1.5f), y-HexMetrics.innerRadius, vec);

		//gen SE
		RecurseGenMapTile(x+(HexMetrics.outerRadius * 1.5f), y+HexMetrics.innerRadius,vec);

		//gen S
		RecurseGenMapTile(x, y+HexMetrics.innerRadius*2,vec);

		//gen SW
		RecurseGenMapTile(x-(HexMetrics.outerRadius * 1.5f), y+HexMetrics.innerRadius,vec);

		//gen NW
		RecurseGenMapTile(x-(HexMetrics.outerRadius * 1.5f), y-HexMetrics.innerRadius,vec);

		//gen N
		RecurseGenMapTile(x, y+HexMetrics.innerRadius*2,vec);





	}

	public HexHorizontalTest GenTileType(){
		return Params.LoadScene<HexHorizontalTest>("res://Test/Delete/HexHorizontalTest.tscn");
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
        // GD.Print(position);
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
			GD.Print("y: ",tileYOffset + (tileYCount * tileHeight));
			ylist.Add(tileYOffset + (tileYCount * tileHeight));
            ++tileYCount;
        }

        float tileWidth = HexMetrics.outerRadius * 2;
        float tileXOffset = HexMetrics.outerRadius +mapX;
        float tileXCount =0 ;

        float limitX = mapX + mapSpace.x;

        while (tileXOffset + (tileXCount * tileWidth ) <=limitX){
			GD.Print("x: ",tileXOffset + (tileXCount * tileWidth ));
			xlist.Add(tileXOffset + (tileXCount * tileWidth ));
            ++tileXCount;
        }
		GD.Print(tileXCount,"   ",tileYCount);

    }
    
}
