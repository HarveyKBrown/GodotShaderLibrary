using Godot;
using System;

public partial class GrassController : Node
{
	[Export]
	Node3D Player;
	[Export]
	Mesh GrassMesh;

	[Export]
	Noise Noise;

	const float RANDOMNESS = 0.8f;
	const float GRID_SIZE = 20;
	const int GRASS_PER_GRID_LENGTH = 12;
	const float SUB_GRID_SIZE = GRID_SIZE / GRASS_PER_GRID_LENGTH;
	const int MULTIMESH_GRID_SIZE = 3; //Must be odd

	MultiMeshInstance3D [] MultiMeshInstances;
	Vector2I PlayerGridPosition = Vector2I.Zero;


	//TODO LIST
	// 1. Import grass model
	// 2. Shader to modify vertex positions based wind (bonus: rotate slightly towards player)
	// 2.1. Sampling terrain height
	// 3. Randomized Color, Rotation, Scale.Y, Scale.XY
	// 3.1. Queuing system for updating (single threaded, ie.. N per frame, time the code)
	// 4. Custom instance data buffer (4, instead of 12 packed floats per instance)
	// 5. Calculating the buffer in the compute shader

	// (Possible solution) Precompute terrain height data and store it for later use
	// (Edge blending tip) Make grass shorter the further away from you

	public override void _Ready() {

		MultiMeshInstances = new MultiMeshInstance3D[MULTIMESH_GRID_SIZE*MULTIMESH_GRID_SIZE];

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
				InstanceCount = GRASS_PER_GRID_LENGTH * GRASS_PER_GRID_LENGTH,
			},
		};
		return toReturn;
	}

	void UpdateInstances() {
		int gridHalflength = MULTIMESH_GRID_SIZE / 2;

		for (int i = 0; i < MultiMeshInstances.Length; i++) {
			int column = i % MULTIMESH_GRID_SIZE - gridHalflength;
			int row = i / MULTIMESH_GRID_SIZE - gridHalflength;

			Vector2I GridPosition = PlayerGridPosition + new Vector2I(column, row);

			UpdateMultiMeshInstance(MultiMeshInstances[i], GridPosition, Noise);
		}
	}

	static void UpdateMultiMeshInstance(MultiMeshInstance3D GrassMultiMesh, Vector2I GridPosition, Noise Noise) {

		Vector3 instanceRootPosition = new Vector3(GridPosition.X, 0, GridPosition.Y) * GRID_SIZE;

		int instanceCount = GrassMultiMesh.Multimesh.InstanceCount;
		for (int i = 0; i < instanceCount; i++) {
			int column = i % GRASS_PER_GRID_LENGTH;
			int row = i / GRASS_PER_GRID_LENGTH;

			//TODO compute shader, pass in a seed derived from grid coordinates, set up instance positions in buffer
			//TOOD write my own vertex shader that uses custom data, so we only need 4 bits per grass, rather than 12 (a transform)
			Vector3 noisePositionMod = new Vector3(Noise.GetNoise2D(100 + column * 20 + GridPosition.X, row * 20 + GridPosition.Y), 0, Noise.GetNoise2D(column * 20 + GridPosition.X, row * 20 + GridPosition.Y)) * SUB_GRID_SIZE * RANDOMNESS;
			Vector3 gridPosition = new(column * SUB_GRID_SIZE, 0, row * SUB_GRID_SIZE);

			GrassMultiMesh.Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, instanceRootPosition + noisePositionMod + gridPosition));
		}
		//TODO Apply AABB to multimesh for culling
	}

	public override void _Process(double delta) {

		DisplayServer.WindowSetTitle($"{GRASS_PER_GRID_LENGTH*GRASS_PER_GRID_LENGTH*MULTIMESH_GRID_SIZE*MULTIMESH_GRID_SIZE} blades of grass @ {Engine.GetFramesPerSecond()}fps");

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
