shader_type spatial;
#include "CutoffFunction.gdshaderinc"

uniform	vec3 color : source_color;
uniform	vec3 borderColor : source_color;
uniform mat4 cutplane;

varying vec3 wvtx;

render_mode cull_back;

void vertex() {
	//Worldspace coordinates
	wvtx = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
	float dist = cutDistance(-cutplane[1].xyz, cutplane[3].xyz, wvtx);
	if (dist < 0.0) discard;
	
	float borderDist = smoothstep(0.0, 0.05, dist);
	ALBEDO = mix(borderColor, color, borderDist);
	EMISSION = mix(borderColor * 7.0, vec3(0.0), borderDist);
}
