using Godot;
using System;
using System.Data;

public partial class GrassController : Node
{
	[Export]
	Node3D Player;
	[Export]
	Mesh GrassMesh;

	[Export]
	Noise Noise;

	const float RANDOMNESS = 1f;
	const float GRID_SIZE = 20;
	const int GRASS_PER_GRID_LENGTH_MAX = 100; //100
	const int GRASS_PER_GRID_LENGTH_MID = 50;
	const int GRASS_PER_GRID_LENGTH_MIN = 25;

	/// <summary>
	/// The size of the grid, within which we render the maximum amount of grass.
	/// Must be odd.
	/// </summary>
	const int MULTIMESH_GRID_SIZE_MAX = 5;
	const int MULTIMESH_GRID_SIZE_MID = 9;
	const int MULTIMESH_GRID_SIZE_MIN = 21;

	MultiMeshInstance3D [] MultiMeshInstances;
	Vector2I PlayerGridPosition = Vector2I.Zero;

	int GrassCount = 0;


	//TODO LIST
	// 2.1. Sampling terrain height
	// 3.1. Queuing system for updating (single threaded, ie.. N per frame, time the code)
	// 4. Custom instance data buffer (4, instead of 12 packed floats per instance)
	// 5. Calculating the buffer in the compute shader
	// 6. Calculate positions AND mesh data on the GPU, passing in a copy of the target mesh

	// (Possible solution) Precompute terrain height data and store it for later use
	// (Edge blending tip) Make grass shorter the further away from you

	public override void _Ready() {

		MultiMeshInstances = new MultiMeshInstance3D[MULTIMESH_GRID_SIZE_MIN*MULTIMESH_GRID_SIZE_MIN];

		for (int i = 0; i < MultiMeshInstances.Length; i++) {
			MultiMeshInstances[i] = GetNewMultimeshInstance();
			AddChild(MultiMeshInstances[i]);
		}

		PlayerGridPosition = CalculateGridPosition(new Vector2(Player.Position.X, Player.Position.Z));

		UpdateInstances();
	}

	MultiMeshInstance3D GetNewMultimeshInstance() {
		MultiMeshInstance3D toReturn = new() {
			Multimesh = new() {
				Mesh = GrassMesh,
				TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
				InstanceCount = GRASS_PER_GRID_LENGTH_MAX * GRASS_PER_GRID_LENGTH_MAX,
			},
		};
		return toReturn;
	}

	void UpdateInstances() {
		GrassCount = 0;
		int gridHalflength = MULTIMESH_GRID_SIZE_MIN / 2;

		for (int i = 0; i < MultiMeshInstances.Length; i++) {
			int column = i % MULTIMESH_GRID_SIZE_MIN - gridHalflength;
			int row = i / MULTIMESH_GRID_SIZE_MIN - gridHalflength;

			Vector2I GridPosition = PlayerGridPosition + new Vector2I(column, row);

			int gridDistance = Mathf.Max(Mathf.Abs(column), Mathf.Abs(row));

			if (gridDistance <= (MULTIMESH_GRID_SIZE_MAX - 1)/2) {
				GrassCount += UpdateMultiMeshInstance(MultiMeshInstances[i], GridPosition, GRASS_PER_GRID_LENGTH_MAX, Noise);
			} else if (gridDistance <= (MULTIMESH_GRID_SIZE_MID - 1)/2) {
				GrassCount += UpdateMultiMeshInstance(MultiMeshInstances[i], GridPosition, GRASS_PER_GRID_LENGTH_MID, Noise);
			} else {
				GrassCount += UpdateMultiMeshInstance(MultiMeshInstances[i], GridPosition, GRASS_PER_GRID_LENGTH_MIN, Noise);
			}
		}
	}

	static int UpdateMultiMeshInstance(MultiMeshInstance3D GrassMultiMesh, Vector2I GridPosition, int GrassPerGridLength, Noise Noise) {

		Vector3 instanceRootPosition = new Vector3(GridPosition.X, 0, GridPosition.Y) * GRID_SIZE;

		GrassMultiMesh.Multimesh.InstanceCount = GrassPerGridLength * GrassPerGridLength;

		int instanceCount = GrassMultiMesh.Multimesh.InstanceCount;
		for (int i = 0; i < instanceCount; i++) {
			int column = i % GrassPerGridLength;
			int row = i / GrassPerGridLength;

			float subGridSize = (float)GRID_SIZE / GrassPerGridLength;

			//TODO compute shader that calculates the packed buffer for us
			Vector3 noisePositionMod = new Vector3(Noise.GetNoise2D(100 + column * 20 + GridPosition.X, row * 20 + GridPosition.Y), 0, Noise.GetNoise2D(column * 20 + GridPosition.X, row * 20 + GridPosition.Y)) * subGridSize * RANDOMNESS;
			Vector3 gridPosition = new(column * subGridSize, 0, row * subGridSize);

			//GrassMultiMesh.Multimesh.Buffer = *COMPUTE_SHADER_OUTPUT*
			GrassMultiMesh.Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, instanceRootPosition + noisePositionMod + gridPosition));
		}
		//TODO Apply AABB to multimesh for culling
		return instanceCount;
	}

	public override void _Process(double delta) {

		DisplayServer.WindowSetTitle($"{GrassCount} blades of grass @ {Engine.GetFramesPerSecond()}fps");

		Vector2I currentGridPosition = CalculateGridPosition(new Vector2(Player.Position.X, Player.Position.Z));
		if (!currentGridPosition.Equals(PlayerGridPosition)) {
			PlayerGridPosition = currentGridPosition;
			UpdateInstances();
		}
	}

	static Vector2I CalculateGridPosition(Vector2 Position) {
		return new Vector2I(Mathf.FloorToInt(Position.X / GRID_SIZE), Mathf.FloorToInt(Position.Y / GRID_SIZE));
	}
}
