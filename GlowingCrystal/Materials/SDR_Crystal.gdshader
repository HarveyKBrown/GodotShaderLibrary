shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_lambert, specular_schlick_ggx;

group_uniforms BaseColor;
uniform vec4 BaseColor : source_color;
uniform float BaseIntensity : hint_range(0.0, 10.0, 0.1);
uniform float FresnelPower : hint_range(0, 10, 0.1);

group_uniforms Highlight;
uniform vec4 TopColor : source_color;
uniform float TopIntensity : hint_range(0.0, 10.0, 0.1);
uniform float TopLine;

uniform vec4 BottomColor : source_color;
uniform float BottomIntensity : hint_range(0.0, 10.0, 0.1);
uniform float BottomLine;


void vertex() {
}

void fragment() {

	float Top = smoothstep(UV.y, 1, TopLine);
	float Bottom = 1.0 - smoothstep(UV.y, 1, BottomLine);

	float fresnel = pow(1.0 - clamp(dot(NORMAL, VIEW), 0.0, 1.0), FresnelPower);

	vec3 TopHighlight = Top * fresnel * TopColor.xyz * TopIntensity;
	vec3 BottomHighlight = Bottom * fresnel * BottomColor.xyz * BottomIntensity;

	//ALBEDO = mix(TopHighlight, BottomHighlight, Top);

	ALBEDO = BaseColor.xyz * BaseIntensity;
	EMISSION = TopHighlight + BottomHighlight;


}
