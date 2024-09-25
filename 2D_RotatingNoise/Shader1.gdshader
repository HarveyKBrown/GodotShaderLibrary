shader_type canvas_item;

uniform float scale_speed = 1;

void vertex() {
	vec2 position = VERTEX;

	//position *= 0.5 + 0.5 * (sin(TIME) / 2.0 + 0.5);
	//float distance = sqrt(pow(position.x, 2.0) + pow(position.y, 2.0));
	position = vec2(
	  cos(TIME) * position.x - sin(TIME) * position.y,
	  sin(TIME) * position.x + cos(TIME) * position.y
	);
	VERTEX = position;
}

//float random (vec2 uv) {
//    return fract(sin(dot(uv.xy,
//        vec2(12.9898,78.233))) * 43758.5453123);
//}

vec2 random(vec2 uv){
    uv = vec2( dot(uv, vec2(127.1,311.7) ),
               dot(uv, vec2(269.5,183.3) ) );
    return -1.0 + 2.0 * fract(sin(uv) * 43758.5453123);
}

float noise(vec2 uv) {
    vec2 uv_index = floor(uv);
    vec2 uv_fract = fract(uv);

    vec2 blur = smoothstep(0.0, 1.0, uv_fract);

    return mix( mix( dot( vec2(random(uv_index + vec2(0.0,0.0)) ), uv_fract - vec2(0.0,0.0) ),
                     dot( vec2(random(uv_index + vec2(1.0,0.0)) ), uv_fract - vec2(1.0,0.0) ), blur.x),
                mix( dot( vec2(random(uv_index + vec2(0.0,1.0)) ), uv_fract - vec2(0.0,1.0) ),
                     dot( vec2(random(uv_index + vec2(1.0,1.0)) ), uv_fract - vec2(1.0,1.0) ), blur.x), blur.y) + 0.5;
}



float worley(vec2 uv, float columns, float rows) {

	vec2 index_uv = floor(vec2(uv.x * columns, uv.y * rows));
	vec2 fract_uv = fract(vec2(uv.x * columns, uv.y * rows));

	float minimum_dist = 1.0;

	for (int y= -1; y <= 1; y++) {
		for (int x= -1; x <= 1; x++) {
			vec2 neighbor = vec2(float(x),float(y));
			vec2 point = vec2(random(index_uv + neighbor));

			vec2 diff = neighbor + point - fract_uv;
			float dist = length(diff);
			minimum_dist = min(minimum_dist, dist);
		}
	}

	return minimum_dist;
}

float fbm(vec2 uv) {
    int octaves = 6;
    float amplitude = 0.5;
    float frequency = 3.0;
	float value = 0.0;

    for(int i = 0; i < octaves; i++) {
        value += amplitude * noise(frequency * uv);
        amplitude *= 0.5;
        frequency *= 2.0;
    }
    return value;
}

void fragment() {

	vec4 color = texture(TEXTURE, UV);

	float greyscale = (color.x + color.y + color.z) / 3.0;

	//color.rgba = vec4(worley(UV, 10, 10));//vec3(greyscale);
	//color.rgba = vec4(noise(UV * 100.0));//vec3(greyscale);
	color.rgba = vec4(fbm(UV * 1.0));//vec3(greyscale);

	// Place fragment code here.
	COLOR = color;;
}
