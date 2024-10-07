@tool
extends CSGMesh3D

@export var LockX : bool;

func _ready():
	pass


func _process(_delta):
	look_at(get_viewport().get_camera_3d().global_position);
	rotation.z = 0;
	if LockX : rotation.x = 0;
	rotate_object_local(Vector3.UP, PI);
	pass
