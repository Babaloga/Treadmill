shader_type spatial;
render_mode depth_prepass_alpha, cull_front;
global uniform vec3 caster_position = vec3(0,0,0);
global uniform float fov;
global uniform float hfov;

uniform sampler2D DEPTH_TEXTURE : hint_depth_texture, filter_nearest;


varying mat4 CAMERA;
varying mat4 INV_CUSTOM_PROJECTION;

//FROM GODOT360:
/*
vec3 latlon_to_ray(vec2 latlon) {
	float lat = latlon.x;
	float lon = latlon.y;
	return vec3(sin(lon) * cos(lat), sin(lat), -cos(lon) * cos(lat));
}

vec3 cylindrical_inverse(vec2 p) {
	if (abs(p.x) > PI) {
		return vec3(0.0, 0.0, 0.0);
	} else {
		float lon = p.x;
		float lat = atan(p.y);
		return latlon_to_ray(vec2(lat, lon));
	}
}
vec2 cylindrical_forward(vec2 latlon) {
	return(vec2(latlon.y, tan(latlon.x)));
}
vec3 cylindrical_ray(vec2 p) {
	float scale = cylindrical_forward(vec2(0.0, radians(fovx) / 2.0)).x;
	return cylindrical_inverse(p * scale);
}
*/



    mat4 create_frustum (float left, float right, float bottom, float top, float near, float far) 
	{

		mat4 M;
/*
		// Make sure there is no division by zero
		if (left === right || bottom === top || near === far) 
		{
			console.log("Invalid createFrustum parameters");
			self.setIdentity(M);
		}

		if (near <= 0 || far <= 0) 
		{
			console.log('For a perspective projection, the near and far distances must be positive');
			self.setIdentity(M);
		} 
		else 
		{
*/
			float sx = 2.0 * near / (right - left);
			float sy = 2.0 * near / (top - bottom);

			float c2 = - (far + near) / (far - near);
			float c1 = 2.0 * near * far / (near - far);

			float tx = -near * (left + right) / (right - left);
			float ty = -near * (bottom + top) / (top - bottom);

		M[0].x = sx;  M[1].x = 0.0;  M[2].x = 0.0;  M[3].x = tx;
    	M[0].y = 0.0;  M[1].y = sy;  M[2].y = 0.0;  M[3].y = ty;
    	M[0].z = 0.0;  M[1].z = 0.0;  M[2].z = c2;  M[3].z = c1;
    	M[0].w = 0.0;  M[1].w = 0.0;  M[2].w = -1.0;  M[3].w = 0.0;
		//}

		return M;
	}

	mat4 create_perspective (float fovy, float aspect, float near, float far)
	{
		//mat4 to_return;
/*
		if (fovy <= 0 || fovy >= 180 || aspect <= 0 || near >= far || near <= 0) 
		{
			to_return = identity_matrix();
		} 
		else 
		{
			*/
			float half_fovy = (fovy * (3.14159265358979323846 / 180.0)) / 2.0;

			float top = near * tan(half_fovy);
			float bottom = -top;
			float right = top * aspect;
			float left = -right;

			return create_frustum(left, right, bottom, top, near, far);
		//}

		//return M;
	}

void vertex() {
    CAMERA = INV_VIEW_MATRIX;

	float aspect_ratio = hfov/fov;

	mat4 custom_projection = create_perspective(fov, aspect_ratio * 0.95, 0.1, 100.0);
	POSITION = custom_projection * MODELVIEW_MATRIX * vec4(VERTEX, 1);
	INV_CUSTOM_PROJECTION = inverse(custom_projection);
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
    float depth = texture(DEPTH_TEXTURE, SCREEN_UV).x;
	//float depth = 0.0;
    vec3 ndc = vec3(SCREEN_UV, depth) * 2.0 - 1.0;
    vec4 view = INV_CUSTOM_PROJECTION * vec4(ndc, 1.0);
  	view.xyz /= view.w;
  	float linear_depth = -view.z;
	vec4 world = CAMERA * INV_CUSTOM_PROJECTION * vec4(ndc, 1.0);
	vec3 world_position = world.xyz / world.w;
	world_position -= caster_position;
    //ALBEDO = ((world_position - caster_position) / 5.0) + 0.5;
	ALBEDO = (world_position) + 0.5;
	//ALBEDO = vec3(pow(linear_depth, 1));
}

mat4 identity_matrix()
{
	mat4 M;

	M[0].x = 1.0;  M[1].x = 0.0;  M[2].x = 0.0;  M[3].x = 0.0;
	M[0].y = 0.0;  M[1].y = 1.0;  M[2].y = 0.0;  M[3].y = 0.0;
	M[0].z = 0.0;  M[1].z = 0.0;  M[2].z = 1.0;  M[3].z = 0.0;
	M[0].w = 0.0;  M[1].w = 0.0;  M[2].w = 0.0;  M[3].w = 1.0;

	return M;
}



