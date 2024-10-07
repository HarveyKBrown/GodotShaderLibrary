using Godot;
using System;

public partial class BasicCharacter : CharacterBody3D
{
	[ExportCategory("ObjectLinks")]
	[Export]
	public Node3D CameraPivot;
	[Export]
	public Node3D PlayerMesh;


	[ExportCategory("Configuration")]
	[Export]
	public float Speed = 8.0f;
	[Export]
	public float JumpVelocity = 4.5f;
	[Export]
	public float PlayerRotationRate = 8.0f;
	[Export]
	public float CameraPanSpeed = 0.002f;


	Vector3 Direction;
	Vector3 InputDirection;

	bool IsMoving;

	public override void _Ready() {
		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (IsMoving) {
			PlayerMesh.Rotation = PlayerMesh.Rotation.Lerp(CameraPivot.Rotation, PlayerRotationRate * (float)delta);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("player_jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}



		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("player_left", "player_right", "player_forward", "player_back");
		Direction = (CameraPivot.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (Direction != Vector3.Zero)
		{
			velocity.X = Direction.X * Speed;
			velocity.Z = Direction.Z * Speed;
			IsMoving = true;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			IsMoving = false;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		// Mouse in viewport coordinates.
		if (@event is InputEventMouseMotion eventMouseMotion) {
			//Yaw
			CameraPivot.Rotate(CameraPivot.Basis.Y, -eventMouseMotion.Relative.X * CameraPanSpeed);
			//Pitch
			//CameraPivot.Rotate(CameraPivot.Basis.X, eventMouseMotion.Relative.Y * CameraPanSpeed);
			//CameraPivot.Rotate(Vector3.Left, eventMouseMotion.Relative.Y * CameraPanSpeed);
		}

		if (@event.IsActionPressed("exit")) GetTree().Quit();
	}
}
