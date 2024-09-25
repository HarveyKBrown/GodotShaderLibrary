shader_type canvas_item;

float rand(vec2 uv) {
	return fract(sin(dot(uv / 1000.0, vec2(65.373423,12.4673))) * 24621.2345234);
}

vec2 hash2(vec2 p) {
	mat2 m = mat2(vec2(0.434233, 0.7312), vec2(0.52368, 0.12456));
	return fract(sin(p * m) * 456234.242233);
}

vec2 hash2_norm(vec2 p) {
	return normalize(hash2(p) - 0.5);
}

float hash_float(vec2 p) {
	//return hash2(p).x;
	return rand(p * 10.0);
}

void fragment() {
	const float cells = 20.0;
	
	vec2 cell = floor(UV*cells) / cells;
	vec2 off = (UV - cell) * cells;
	vec2 cell_size = vec2(1.0) / cells;
	
	float corner00 = hash_float(cell);
	float corner10 = hash_float(cell + vec2(cell_size.x, 0.0));
	float corner01 = hash_float(cell + vec2(0.0, cell_size.y));
	float corner11 = hash_float(cell + cell_size);
	
	vec2 cube = off*off*off*(10.0+off*(-15.0 + 6.0*off));
	float a = mix(corner00, corner10, cube.x);
	float b = mix(corner01, corner11, cube.x);
	COLOR.xyz = vec3(mix(a, b, cube.y));
//	COLOR.rgb = vec3(a);
	
}

