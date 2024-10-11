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
	public float JumpHeight = 1;
	[Export]
	public float PlayerRotationRate = 8.0f;
	[Export]
	public float CameraPanSpeed = 0.002f;

	//Calculated Values
	Vector3 HorizontalVelocity;
	Vector3 MovementInput;
	public float JumpForce; //4.5
	float CameraFacingAngle;
	bool MouseLocked = true;

	public override void _Ready() {
		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Captured;
		JumpForce = Mathf.Sqrt(2 * 9.8f * JumpHeight);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		//Rotate player to camera direction
		if (MovementInput.LengthSquared() > 0) {
			Vector3 newRotation = PlayerMesh.Rotation;
			newRotation.Y = Mathf.Lerp(PlayerMesh.Rotation.Y, CameraFacingAngle, PlayerRotationRate * (float)delta);
			PlayerMesh.Rotation = newRotation;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity
		if (!IsOnFloor()) {
			velocity.Y += GetGravity().Y * (float)delta;
		}

		// Handle jump
		if (Input.IsActionJustPressed("player_jump") && IsOnFloor()) {
			velocity.Y = JumpForce;
		}

		//Get input direction
		Vector2 rawMovementInput = Input.GetVector("player_left", "player_right", "player_forward", "player_back");
		MovementInput = new Vector3(rawMovementInput.X, 0, rawMovementInput.Y);

		CameraFacingAngle = CameraPivot.Rotation.Y;
		HorizontalVelocity = MovementInput.Rotated(Vector3.Up, CameraPivot.Rotation.Y).Normalized() * Speed;

		velocity.X = HorizontalVelocity.X;
		velocity.Z = HorizontalVelocity.Z;

		Velocity = velocity;
		//GD.Print($"MovementInput: {MovementInput}, HorizontalVelocity: {HorizontalVelocity}, Velocity: {Velocity}");
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		// Mouse in viewport coordinates.
		if (@event is InputEventMouseMotion eventMouseMotion) {
			if (!MouseLocked) return;
			//Yaw
			CameraPivot.Rotate(Vector3.Up, -eventMouseMotion.Relative.X * CameraPanSpeed);
			//Pitch
			CameraPivot.Rotate(CameraPivot.Basis.X, -eventMouseMotion.Relative.Y * CameraPanSpeed);
		}

		if (@event.IsActionPressed("click")) {
			Input.MouseMode = Input.MouseModeEnum.Captured;
			MouseLocked = true;
		}

		//if (@event.IsActionPressed("exit")) GetTree().Quit();
		if (@event.IsActionPressed("exit")) {
			Input.MouseMode = Input.MouseModeEnum.Visible;
			MouseLocked = false;
		}
	}
}
