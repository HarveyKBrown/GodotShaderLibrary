shader_type spatial;
render_mode cull_disabled;

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
	VERTEX = VERTEX * vec3(1., 2., 0.);
	
	// Modify the size of the grass
	float height_mod = rand(MODEL_MATRIX[3].xz);
	const float y_weight = 0.8;
	const float x_weight = 0.5;
	VERTEX.y = VERTEX.y * (1. - y_weight + y_weight * height_mod);
	VERTEX.x = VERTEX.x * (1. - x_weight + x_weight * height_mod);
	
	// Taper the grass towards the top
	float grass_width_mod = 0.0 + pow(UV.y, 0.5);
	VERTEX.x *= grass_width_mod;
	
	// Billboard
	mat4 billboard_matrix = INV_VIEW_MATRIX;
	billboard_matrix[1] = MODEL_MATRIX[1];
	billboard_matrix[3] = MODEL_MATRIX[3];
	VERTEX = (billboard_matrix * VERTEX.xyzz).xyz;
	//NORMAL = (billboard_matrix * NORMAL.xyzz).xyz;
	
	//Wind
	float wind_amount = 1. - 2. * texture(WindNoise, TIME * WindScrollSpeed + MODEL_MATRIX[3].xz * 0.01f).r;
	float vert_height = VERTEX.y - MODEL_MATRIX[3].y;
	float wind_affect_height_mod = pow(vert_height, 2);
	vec2 wind_xy_mod = wind_amount * normalize(WindScroll) * WindStrength * wind_affect_height_mod;
	VERTEX.xz = VERTEX.xz + wind_xy_mod;
	//float wind_y_mod = 1. / abs(wind_amount * WindStrength * wind_affect_height_mod);
	//VERTEX.y = VERTEX.y * wind_affect_height_mod;
	
	//mat4 billboard_model_matrix = INV_VIEW_MATRIX;
	//billboard_model_matrix[3] = MODEL_MATRIX[3];
	//billboard_model_matrix[1] = MODEL_MATRIX[1];
	//MODELVIEW_MATRIX = VIEW_MATRIX * billboard_model_matrix;
}

void fragment() {
	// Gradient Albedo
	ALBEDO = texture(HeightColorGradient, UV.yy).xyz;
	
	// Randomize Albedo
	float variation_noise_value = texture(ColorVariationNoise, COLOR.xz).r * texture(ColorVariationNoise, vec2(0.31, 0.08) + COLOR.xz * 0.01).r;
	ALBEDO *= texture(ColorVariationGradient, vec2(variation_noise_value)).rgb;
	
	ROUGHNESS = 0.0;
	
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
