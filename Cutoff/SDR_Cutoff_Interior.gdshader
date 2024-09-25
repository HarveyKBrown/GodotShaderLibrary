shader_type spatial;
render_mode cull_front;

#include "CutoffFunction.gdshaderinc"

uniform	vec3 color : source_color;
uniform mat4 cutplane;

varying vec3 wvtx;


void vertex() {
	//Worldspace coordinates
	wvtx = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
	float dist = cutDistance(-cutplane[1].xyz, cutplane[3].xyz, wvtx);
	if (dist < 0.0) discard;
	ALBEDO = color;
	EMISSION = color * 7.0;
}
