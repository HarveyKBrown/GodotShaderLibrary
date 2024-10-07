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

	const float RANDOMNESS = 0.6f;
	const float GRID_SIZE = 10;
	const int GRASS_PER_GRID_LENGTH = 4;
	const int MULTIMESH_GRID_SIZE = 3; //Must be odd

	//MultiMeshInstance3D [] MultiMeshInstances;
	MultiMeshInstance3D GrassMultiMesh1;
	Vector2I PlayerGridPosition = Vector2I.Zero;

	public override void _Ready() {
		GrassMultiMesh1 = GetNewMultimeshInstance();
		AddChild(GrassMultiMesh1);



		PlayerGridPosition = CalculateGridPosition(new Vector2(Player.Position.X, Player.Position.Z));

		UpdateMultiMeshInstance(GrassMultiMesh1, PlayerGridPosition, Noise);
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

	static void UpdateMultiMeshInstance(MultiMeshInstance3D GrassMultiMesh, Vector2I GridPosition, Noise Noise) {

		Vector3 instanceRootPosition = new Vector3(GridPosition.X, 0, GridPosition.Y) * GRID_SIZE;

		int instanceCount = GrassMultiMesh.Multimesh.InstanceCount;
		for (int i = 0; i < instanceCount; i++) {
			int column = i % GRASS_PER_GRID_LENGTH;
			int row = i / GRASS_PER_GRID_LENGTH;
			float subGridSize = GRID_SIZE / GRASS_PER_GRID_LENGTH;

			Vector3 noisePositionMod = new Vector3(1, 0, 1) * Noise.GetNoise2D(column * 20 + GridPosition.X, row * 20 + GridPosition.Y) * subGridSize * RANDOMNESS;
			Vector3 localPosition = new(column * subGridSize, 0, row * subGridSize);

			GrassMultiMesh.Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, instanceRootPosition + noisePositionMod + localPosition));
		}
		//TODO Apply AABB to multimesh for culling
	}

	public override void _Process(double delta) {
		Vector2I currentGridPosition = CalculateGridPosition(new Vector2(Player.Position.X, Player.Position.Z));

		if (!currentGridPosition.Equals(PlayerGridPosition)) {
			PlayerGridPosition = currentGridPosition;
			UpdateMultiMeshInstance(GrassMultiMesh1, PlayerGridPosition, Noise);
			//TODO Only update the multimeshes that need to be updated
		}
	}

	static Vector2I CalculateGridPosition(Vector2 Position) {
		return new Vector2I(Mathf.FloorToInt(Position.X / GRID_SIZE), Mathf.FloorToInt(Position.Y / GRID_SIZE));
	}
}
