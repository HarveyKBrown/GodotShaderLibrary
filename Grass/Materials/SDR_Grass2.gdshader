shader_type spatial;
render_mode cull_disabled, diffuse_toon;

uniform sampler2D HeightColorGradient : source_color, repeat_disable;
uniform sampler2D ColorVariationGradient : source_color, repeat_disable;
uniform sampler2D ColorVariationNoise;
uniform sampler2D WindNoise;
uniform vec2 WindScroll;
uniform float WindScrollSpeed;
uniform float WindStrength;

// Return random float between 0 and 1
float rand(vec2 co){
    return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}

void vertex() {
	// Load the grass origin into color, so it can be used by the vertex shader
	COLOR = MODEL_MATRIX[3];
	
	// Scale Model
	VERTEX = VERTEX * vec3(0.8, 1.0, 0.);
	
	// Modify the size of the grass
	float height_mod = 0.5 + 1.0 * rand(MODEL_MATRIX[3].xz);
	//float height_mod = 1.5 * texture(HeightVariatioNoise, MODEL_MATRIX[3].xz * 0.01).r;
	//float height_mod = (0.5 + 0.5 * rand(MODEL_MATRIX[3].xz)) * texture(HeightVariatioNoise, MODEL_MATRIX[3].xz * 0.01).r;
	const float y_weight = 0.8;
	const float x_weight = 0.5;
	VERTEX.y = VERTEX.y * (1. - y_weight + y_weight * height_mod);
	VERTEX.x = VERTEX.x * (1. - x_weight + x_weight * height_mod);
	
	// Taper the grass towards the top
	float grass_width_mod = 0.0 + pow(UV.y, 0.5);
	VERTEX.x *= grass_width_mod;
	
	// Billboard
	mat4 billboard_matrix = INV_VIEW_MATRIX;
	billboard_matrix[1] = vec4(0.0, 1.0, 0.0, 0.0);
	VERTEX = (billboard_matrix * VERTEX.xyzz).xyz;
	
	//Wind
	vec2 normalized_wind_scroll = normalize(WindScroll);
	float wind_amount = - 0.3 + 1.3 * texture(WindNoise, normalized_wind_scroll * TIME * WindScrollSpeed + MODEL_MATRIX[3].xz * 0.01f).r;
	float vert_height = VERTEX.y - MODEL_MATRIX[3].y;
	float wind_affect_height_mod = pow(vert_height, 2);
	vec2 wind_xy_mod = wind_amount * -normalized_wind_scroll * WindStrength * wind_affect_height_mod;
	VERTEX.xz = VERTEX.xz + wind_xy_mod;
	float wind_height_mod_perc = 0.2;
	VERTEX.y = VERTEX.y * 1.0/(1.0 - wind_height_mod_perc + wind_height_mod_perc * abs(wind_amount) * 2.0);
}

void fragment() {
	// Gradient Albedo
	ALBEDO = texture(HeightColorGradient, UV.yy).xyz;
	
	// Randomize Albedo
	float variation_noise_value = texture(ColorVariationNoise, COLOR.xz).r * texture(ColorVariationNoise, vec2(0.31, 0.08) + COLOR.xz * 0.01).r;
	ALBEDO *= texture(ColorVariationGradient, vec2(variation_noise_value)).rgb;
	
	if (!FRONT_FACING) NORMAL = -NORMAL;
	
}

//void light() {
	//
    //DIFFUSE_LIGHT += clamp(dot(NORMAL, LIGHT), 0.0, 1.0) * ATTENUATION * LIGHT_COLOR;
//}
