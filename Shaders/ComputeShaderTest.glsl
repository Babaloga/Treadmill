#[compute]
#version 450

const int HOZ_POINTS = 3600;
const int VERT_POINTS = 90;

const float VERT_SPREAD = 60;

layout(local_size_x = 10, local_size_y = 10, local_size_z = 1) in;

layout(set = 0, binding = 0, rgba32f) uniform image2D OUTPUT_TEXTURE;

void main()
{
    /*
    float fov = state_buffer.fov_degrees;

    float hoz_angle = (-fov / float(2)) + gl_GlobalInvocationID.x * (fov / float(HOZ_POINTS));
    float vert_angle = -VERT_SPREAD + (gl_GlobalInvocationID.y * (2 * VERT_SPREAD / float(VERT_POINTS)));

    vec3 cast_vector = state_buffer.caster_forward;

    //rotate hoz
    float cast_vector_new_x = (cast_vector.x * cos(hoz_angle)) - (cast_vector.z * sin(hoz_angle));
    float cast_vector_new_z = (cast_vector.x * sin(hoz_angle)) + (cast_vector.z * cos(hoz_angle));

    cast_vector.x = cast_vector_new_x;
    cast_vector.z = cast_vector_new_z;

    //rotate vert
    vec3 cast_vector_rotated = (rotationMatrix(vec3(cast_vector.z, 0, -cast_vector.x), vert_angle) * vec4(cast_vector, 1.0)).xyz;

    cast_vector = cast_vector_rotated;

    //March Ray
    vec4 point = ray_march(state_buffer.caster_position, cast_vector);
    */
    ivec2 texel = ivec2(gl_GlobalInvocationID.xy);
    //imageStore(OUTPUT_TEXTURE, texel, point);
    
    imageStore(OUTPUT_TEXTURE, texel, vec4(1.0,0.0,0.0,1.0));
}
