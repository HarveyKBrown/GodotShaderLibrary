shader_type spatial;

varying vec3 world_position;

uniform float GridSize = 1.f;
uniform vec4 Color1 : source_color;
uniform vec4 Color2 : source_color;

void vertex() {
    world_position = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
    vec3 pos = world_position;
    vec3 gridPos = mod(pos, GridSize);
    vec3 gridChecker = step(GridSize * 0.5, gridPos);

    float value = mod(gridChecker.x + gridChecker.y + gridChecker.z, 2.0);

    ALBEDO = mix(Color1.xyz, Color2.xyz, value);
	ROUGHNESS = mix(Color1.a, Color2.a, value);
}