shader_type spatial;
render_mode depth_prepass_alpha;
global uniform vec3 caster_position = vec3(0,0,0);

uniform sampler2D DEPTH_TEXTURE : hint_depth_texture, filter_nearest;

uniform float strength = 1.0;           // s: 0 = perspective, 1 = stereographic          
uniform float vertFOV = 1.6;
//uniform float aspectRatio;        // a: screenWidth / screenHeight
uniform float cylindricalRatio = 1.0;   // c: cylindrical distortion ratio. 1 = spherical

varying mat4 CAMERA;
varying vec3 vUV;
varying vec2 vUVDot;

void vertex() {
	//VERTEX = projectionMatrix * (modelViewMatrix * vec4(position, 1.0));
	float height = tan(vertFOV / 2.0); // h: tan(verticalFOVInRadians / 2)
	float aspectRatio = VIEWPORT_SIZE.x / VIEWPORT_SIZE.y;

    float scaledHeight = strength * height;
    float cylAspectRatio = aspectRatio * cylindricalRatio;
    float aspectDiagSq = aspectRatio * aspectRatio + 1.0;
    float diagSq = scaledHeight * scaledHeight * aspectDiagSq;
    vec2 signedUV = (2.0 * UV + vec2(-1.0, -1.0));

    float z = 0.5 * sqrt(diagSq + 1.0) + 0.5;
    float ny = (z - 1.0) / (cylAspectRatio * cylAspectRatio + 1.0);

    vUVDot = sqrt(ny) * vec2(cylAspectRatio, 1.0) * signedUV;
    vUV = vec3(0.5, 0.5, 1.0) * z + vec3(-0.5, -0.5, 0.0);
    vUV.xy += UV;
    CAMERA = INV_VIEW_MATRIX;
	//POSITION = vec4(VERTEX, 1.0);
}

void fragment() {
	/*
	float view_ratio = VIEWPORT_SIZE.x / VIEWPORT_SIZE.y;
	vec2 uv = FRAGCOORD.xy / min(VIEWPORT_SIZE.x, VIEWPORT_SIZE.y);
	vec3 pos = vec3(0.0, 0.0, 0.0);
	if (view_ratio > 1.0) {
		pos = cylindrical_ray(vec2(uv.x - 0.5 * view_ratio, uv.y - 0.5) * 1.0);
	} else {
		pos = cylindrical_ray(vec2(uv.x - 0.5 * view_ratio, uv.y - 0.5) * 1.0);
	}
	*/

	vec3 distorted_screen_uv = dot(vUVDot, vUVDot) * vec3(-0.5, -0.5, -1.0) + vUV;

    float depth = texture(DEPTH_TEXTURE, distorted_screen_uv.xy).x;
	//float depth = 0.0;
    vec3 ndc = vec3(distorted_screen_uv.xy, depth) * 2.0 - 1.0;
    vec4 view = INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
  	view.xyz /= view.w;
  	float linear_depth = -view.z;
	vec4 world = CAMERA * INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
	vec3 world_position = world.xyz / world.w;
	world_position -= caster_position;
    //ALBEDO = ((world_position - caster_position) / 5.0) + 0.5;
	ALBEDO = (world_position) + 0.5;
	//ALBEDO = vec3(pow(linear_depth, 1));
	}

