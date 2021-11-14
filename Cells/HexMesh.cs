using Godot;
using System;
using System.Collections.Generic;

public class HexMesh : Spatial
{
    MeshInstance hexMesh;
	List<Vector3> vertices;
	List<int> triangles;
    public override void _Ready()
    {
        hexMesh = GetNode<MeshInstance>("MeshInstance");
		vertices = new List<Vector3>();
		triangles = new List<int>();
       // hexMesh.Trai
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
