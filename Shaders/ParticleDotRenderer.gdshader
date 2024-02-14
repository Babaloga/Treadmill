shader_type particles;
render_mode disable_force, disable_velocity;
global uniform vec3 caster_position = vec3(0,0,0);

uniform vec4 color0 : source_color;
uniform vec4 color1 : source_color;
uniform vec4 color2 : source_color;
uniform vec4 color3 : source_color;

uniform sampler2D[64] points : source_color, filter_nearest;
uniform int vertResolution;
uniform vec4 colorTest;

void start(){
	TRANSFORM[3].xyz = vec3(1);
}

void process() {
	float vertPixelRatio = float(textureSize(points[0], 0).y) / float(vertResolution);

	int vertPixelCount = vertResolution;
	ivec3 index_vector;
	int indexWithinSlice = int(mod(float(INDEX), float(textureSize(points[0], 0).x * vertPixelCount)));
	int sliceIndex = int(floor(float(INDEX)/ float(textureSize(points[0], 0).x * vertPixelCount)));

	index_vector.x = int(floor(float(indexWithinSlice) / float(vertPixelCount)));
	index_vector.y = int(mod(float(indexWithinSlice), float(vertPixelCount)) * vertPixelRatio);
	index_vector.z = sliceIndex;
	
	vec4 readPosition = texelFetch(points[index_vector.z], index_vector.xy, 0);
	//vec3 position = (vec4(readPosition - 0.5, 1) * MODEL_MATRIX).xyz;
	vec3 position = (readPosition.xyz - 0.5);
	
	//vec3 position = vec3(0,0,0);

	//position.y = index_vector.y;
	//position.x = position.x + (float(index_vector.x) / 100.0);
	//position.z = position.z + (float(index_vector.y) / 100.0);
	//vec3 position = vec3(index_vector.x, 10, index_vector.y);
	//POSITION =  vec4(position, 1) * PROJECTION_MATRIX;

	TRANSFORM[3].xyz = (position) * 2.0;
	//TRANSFORM[3].xyz = vec3(sin(TIME), index_vector.y, index_vector.x);
	COLOR = vec4(readPosition.xyz, 1);

	float distance = length(position);
	//distance *= distance;

	if(distance < 2.0)
	{
		COLOR = mix(color0, color1, distance / 2.0);
	}
	else if (distance < 10.0)
	{
		COLOR = mix(color1, color2, (distance - 2.0) / 8.0);
	}
	else if (distance < 20.0)
	{
		COLOR = mix(color2, color3, (distance - 10.0) / 10.0);
	}
	else
	{
		COLOR = color3;
	}

	//COLOR = vec4(float(hozResolution) / 60.0,0,0,1);
	//COLOR = colorTest;
	//TRANSFORM[3].xyz = vec3(1,sin(TIME),1);

	//VERTEX = vec3(1,1,1);

}


