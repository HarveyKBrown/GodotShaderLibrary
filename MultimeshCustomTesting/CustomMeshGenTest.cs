using Godot;
using Godot.NativeInterop;
using System;

public partial class CustomMeshGenTest : MeshInstance3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var vertices = new Vector3[]
		{
			new (0, 1, 0),
			new (1, 0, 0),
			new (0, 0, 0),

			new (0, -1, 0),
			new (-1, 0, 0),
			new (0, 0, 0),
		};

		// Initialize the ArrayMesh.
		ArrayMesh arrMesh = new();
		var arrays = new Godot.Collections.Array(); // Array of different types of arrays, some of which themselves are arrays of arrays, variant types are disgusting
		arrays.Resize((int)Mesh.ArrayType.Max);
		arrays[(int)Mesh.ArrayType.Vertex] = vertices;

		// Create the Mesh.
		arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		Mesh = arrMesh;
	}
}
