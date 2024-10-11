using Godot;
using System;

public partial class MultimeshCustom : MultiMeshInstance3D
{
	
	public override void _Ready() {
		Multimesh.InstanceCount = 1;
		Multimesh.SetInstanceCustomData(0, Colors.Green);
		Multimesh.SetInstanceTransform(0, Transform3D.Identity);
	}

	public override void _Process(double delta) {
	}
}


// debugging values 
