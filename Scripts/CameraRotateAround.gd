extends Camera3D

var angle : float;
var speed : float = 0.35;
@export var distance : float = 2;
@export var target : Node3D;

func _ready():
	pass # Replace with function body.

func _process(delta):
	angle += delta * speed;
	position.x = sin(angle)*distance;
	position.z = cos(angle)*distance;
	look_at(target.global_position);
	pass
