shader_type spatial;
render_mode world_vertex_coords, cull_disabled, unshaded;
global uniform vec3 caster_position = vec3(0,0,0);

varying vec3 world_position;

void vertex() {
	// Called for every vertex the material is visible on.
	world_position = vec4(VERTEX, 1).xyz;
}

void fragment() {
	// Called for every pixel the material is visible on.
	//vec3 to_caster = normalize(caster_position - (INV_VIEW_MATRIX * FRAGCOORD).xyz);
	vec3 to_caster = caster_position - world_position;
	
	//ALBEDO = (vec4(NORMAL,1) * VIEW_MATRIX).xyz;

	//ALBEDO = mix(vec3(1,0,0),vec3(0,0,1),length(to_caster)/float(25));

	//ALPHA = (float(90) - acos(dot(to_caster, NORMAL)))/float(90);

	vec3 world_normal = (vec4(NORMAL,1) * VIEW_MATRIX).xyz;

	if(!FRONT_FACING)
	{
		world_normal = world_normal * float(-1);
	}

	//	MOD Striping
	//ALPHA = clamp(floor(mod(float(0.5) * degrees(acos(dot(normalize(to_caster), vec3(0,1,0)))), float(1) + (float(1)/pow(length(to_caster),2)))) * (float(90) - degrees(acos(dot(normalize(to_caster), world_normal))))/float(90), 0, 1);
	
	//	SINE Striping
	ALPHA = floor(sin(float(10) * degrees(acos(dot(normalize(to_caster), vec3(0,1,0))))) + (float(1) / pow(length(to_caster),2))) * clamp(dot(normalize(to_caster), world_normal), 0, 1);

	//ALPHA = (float(90) - degrees(acos(dot(normalize(to_caster), world_normal))))/float(90);

	//ALPHA = floor(mod(float(2) * degrees(acos(dot(normalize(to_caster), vec3(0,1,0)))), length(to_caster))) * (float(90) - degrees(acos(dot(normalize(to_caster), world_normal))))/float(90);

	ALBEDO = clamp(mix(vec3(1,0.5,1), vec3(0.1,0,0.5), length(to_caster) / float(5)),vec3(0,0,0),vec3(1,0.5,1));

	if(degrees(acos(dot(normalize(to_caster), vec3(0,1,0)))) < float(60))
	{
		discard;
	}
}

/*
float2 PostProjectionSpaceToScreenSpace(float4 pos)
{
	float2 screenPos = pos.xy / pos.w;
	return (0.5f * (float2(screenPos.x, -screenPos.y) + 1));
}

struct VertexShaderOutputMeshBased
{
	float4 Position : POSITION0;
	float4 TexCoordScreenSpace : TEXCOORD0;
};

VertexShaderOutputMeshBased PointLightMeshVS(VertexShaderInput input)
{
	VertexShaderOutputMeshBased output = (VertexShaderOutputMeshBased)0;
	output.Position = mul(input.Position, WorldViewProjection);
	//we will compute our texture coords based on pixel position further
	output.TexCoordScreenSpace = output.Position;
	return output;
}

float4 PointLightMeshPS(VertexShaderOutputMeshBased input) : COLOR0
{
	//as we are using a sphere mesh, we need to recompute each pixel position
	//into texture space coords. GBufferPixelSize is used to fetch the texel’s center
	float2 screenPos = PostProjectionSpaceToScreenSpace(input.TexCoordScreenSpace) + GBufferPixelSize;
	//read the depth value
	float depthValue = tex2D(depthSampler, screenPos).r;
	depthValue*=FarClip;
	// Reconstruct position from the depth value, the FOV, aspect and pixel position
	// Convert screenPos to [-1..1] range
	// We negate the depthValue since it goes to -FarClip in view space
	float3 pos = float3(TanAspect*(screenPos*2 – 1)*depthValue, -depthValue);
	…
}
*/
//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
