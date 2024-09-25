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

//shader_type spatial;
//
//varying vec3 world_position;
//
//void vertex() {
    //world_position = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz; // Use world coordinates
//}
//
//const float gridSize = 1.0; // Grid size in world space units
//
//void fragment() {
    //vec3 pos = world_position;
    //
    //// Compute the grid position in world space
    //vec3 gridPos = mod(pos, gridSize); 
//
    //// Create a checkerboard pattern based on world space coordinates
    //vec3 gridChecker = step(gridSize * 0.5, gridPos); 
//
    //// XOR-like effect to alternate between black and white
    //float checkerValue = mod(gridChecker.x + gridChecker.y + gridChecker.z, 2.0);
//
    //// Set the ALBEDO color to black or white based on the checker value
    //ALBEDO = vec3(checkerValue);
//}