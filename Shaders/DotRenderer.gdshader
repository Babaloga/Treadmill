
//CONDEMNED

/*
shader_type spatial;
render_mode cull_disabled, unshaded, depth_draw_opaque, ambient_light_disabled, shadows_disabled, world_vertex_coords;
global uniform vec3 caster_position = vec3(0,0,0);

uniform sampler2D[64] points : filter_nearest;
uniform int hozResolution;


varying flat int vert_id;
varying vec3 readPosition;
varying flat ivec3 index_vector;

//STOP, THIS IS THE WRONG SHADER. USE THE PARTICLE ONE!

vec3 srgb_to_linear(vec3 color) {
	return color * (color * (color * 0.305306011 + 0.682171111) + 0.012522878);
}

void vertex() {
	float vertPixelRatio = float(textureSize(points[0], 0).y) / float(vertResolution);

	index_vector = ivec3(int(UV.x) / hozResolution, int(UV.y * vertPixelRatio), int(mod(UV.x, float(hozResolution))));

	readPosition = texelFetch(points[index_vector.z], index_vector.xy,0).xyz;
	//vec3 position = (vec4(readPosition - 0.5, 1) * MODEL_MATRIX).xyz;
	vec3 position = (readPosition - 0.5);

	//position.y = index_vector.y;
	//position.z = position.z + index_vector.x;
	//vec3 position = vec3(index_vector.x, 10, index_vector.y);
	//POSITION =  vec4(position, 1) * PROJECTION_MATRIX;
	VERTEX = (caster_position + position) * 1.0;
	//VERTEX = vec3(1,1,1);
	POINT_SIZE = 5.0;
}

void fragment() {
	// Called for every pixel the material is visible on.
	/*
	if(lessThan(readPosition, vec3(1.0)) == bvec3(true))
	{
		ALBEDO = vec3(1,0,0);
	}
	else
	{
		ALBEDO = vec3(1,1,1);
	}
	*/
	//ALBEDO = vec3(1);
	//ALBEDO = texture(points, index_vector).xyz;
	ALBEDO = readPosition;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
*/