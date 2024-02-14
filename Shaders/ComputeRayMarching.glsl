#[compute]
#version 450

const int HOZ_POINTS = 3600;
const int VERT_POINTS = 90;

const float VERT_SPREAD = 60;

layout(local_size_x = 100, local_size_y = 10, local_size_z = 1) in;

layout(set = 0, binding = 0, std430) buffer StateBuffer {
    vec3 caster_position;
    vec3 caster_forward;
    float fov_degrees;
} state_buffer;

layout(set = 0, binding = 1, rgba32f) uniform image2D OUTPUT_TEXTURE;

float distance_from_sphere(in vec3 p, in vec3 c, float r)
{
    return length(p - c) - r;
}

vec4 ray_march(in vec3 ro, in vec3 rd)
{
    
    float total_distance_traveled = 0.0;
    const int NUMBER_OF_STEPS = 32;
    const float MINIMUM_HIT_DISTANCE = 0.001;
    const float MAXIMUM_TRACE_DISTANCE = 1000.0;

    for (int i = 0; i < NUMBER_OF_STEPS; ++i)
    {
        // Calculate our current position along the ray
        vec3 current_position = ro + total_distance_traveled * rd;

        // We wrote this function earlier in the tutorial -
        // assume that the sphere is centered at the origin
        // and has unit radius
        float distance_to_closest = distance_from_sphere(current_position, vec3(0.0), float(1.0));

        if (distance_to_closest < MINIMUM_HIT_DISTANCE) // hit
        {
            // We hit something! Return red for now
            return vec4(current_position, total_distance_traveled);
        }

        if (total_distance_traveled > MAXIMUM_TRACE_DISTANCE) // miss
        {
            break;
        }

        // accumulate the distance traveled thus far
        total_distance_traveled += distance_to_closest;
    }

    // If we get here, we didn't hit anything so just
    // return a background color (black)
    
    return vec4(state_buffer.caster_position, 0.0);
    

    //return vec4((ro + (100.0 * rd)), gl_GlobalInvocationID.y);
}

mat4 rotationMatrix(vec3 axis, float angle)
{
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;
    
    return mat4(oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,  0.0,
                oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,  0.0,
                oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c,           0.0,
                0.0,                                0.0,                                0.0,                                1.0);
}

void main()
{
    
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
    //vec3 cast_vector_rotated = (rotationMatrix(vec3(cast_vector.z, 0, -cast_vector.x), vert_angle) * vec4(cast_vector, 1.0)).xyz;

    //cast_vector = cast_vector_rotated;

    //March Ray
    vec4 point = ray_march(state_buffer.caster_position, cast_vector);
    
    ivec2 texel = ivec2(gl_GlobalInvocationID.xy);
    //imageStore(OUTPUT_TEXTURE, texel, point/100.0);
    
    imageStore(OUTPUT_TEXTURE, texel, point/100.0);
}
