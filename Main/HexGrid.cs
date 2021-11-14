using Godot;
using System;

public class HexGrid : Spatial
{
    public int width = 6;
	public int height = 6;

	public HexCell hexCell;

    HexCell[] cells;

    public override void _Ready()
    {
        cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				CreateCell(x, z, i++);
			}
		}
    }
	
	public void CreateCell (int x, int z, int i) {

		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Loader.LoadScene<HexCell>("res://Cells/HexCell.tscn");
		//cell.Tran .SetParent(transform, false);
		cell.Translation = position;
        this.AddChild(cell);
        //cell.SetText(x.ToString() + "\n" + z.ToString());
        GD.Print(position);
	}
    

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
