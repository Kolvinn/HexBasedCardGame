using Godot;
using System;

public class HexGrid2d : Node2D
{
    public int width = 6;
	public int height = 6;

	public Hex2d hexCell;

    Hex2d[] cells;
    public override void _Ready()
    {
        cells = new Hex2d[height * width];

		for (int y = 0, i = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				CreateCell(x, y, i++);
			}
		}
    }
	
	public void CreateCell (int x, int y, int i) {

		Vector2 position;
		//position.x = (x + y * 0.5f - y / 2) * (HexMetrics.innerRadius * 2f);
		//position.y = 0f;
		//position.y = y * (HexMetrics.outerRadius * 1.5f);

        position.x = 100 + (x + y * 0.5f - y / 2)  * (HexMetrics.innerRadius * 2f);

        float squashdist  = HexMetrics.outerRadius * 0.2f;
		position.y = 100 + y   * ((HexMetrics.outerRadius * 1.5f) -squashdist);

        Camera2D camera2D;
        

		Hex2d cell = cells[i] = Loader.LoadScene<Hex2d>("res://2d/Hex2d.tscn");
		//cell.Tran .SetParent(transform, false);
		cell.Position = position;
        this.AddChild(cell);
        cell.SetText(x.ToString() + "\n" + y.ToString());

	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
