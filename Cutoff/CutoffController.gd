@tool
extends MeshInstance3D


@onready var plane_anchor = $"../PlaneAnchor"



func _process(delta):
	material_override.set_shader_parameter("cutplane", plane_anchor.global_transform);
	material_override.next_pass.set_shader_parameter("cutplane", plane_anchor.global_transform);
