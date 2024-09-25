@tool
extends OmniLight3D

var time : float;
var speed : float = 25;

func _process(delta):
	time += delta * speed;
	light_energy = 0.9 + 0.1 * (3 + sin(time)/2.0 + sin(time * 0.41 + 0.23)/2.0 + sin(time * 0.15 + 0.76)/2.0);
	pass
