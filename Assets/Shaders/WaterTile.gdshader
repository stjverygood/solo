shader_type canvas_item;

// 确保 Texture Filter = Nearest

uniform vec4 water_color : source_color = vec4(0.0, 0.35, 0.65, 1.0);
uniform vec4 shallow_color : source_color = vec4(0.05, 0.5, 0.75, 1.0);
uniform vec4 wave_color : source_color = vec4(0.5, 0.85, 0.95, 1.0);

uniform float speed = 1.2;

uniform float wave_frequency = 0.04;
uniform float wave_width = 0.12;

uniform float pixel_size = 4.0;


// 噪声
uniform sampler2D noise_tex : repeat_enable, filter_nearest;

uniform float noise_strength = 1.0;
uniform float noise_scale = 0.015;


// 世界坐标
varying vec2 world_pos;


void vertex()
{
    world_pos = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}


void fragment()
{
    vec4 tex_color = texture(TEXTURE, UV);

    if (tex_color.a == 0.0)
    {
        discard;
    }


    // ==============================
    // 1. 像素化世界坐标
    // ==============================

    vec2 pos = floor(world_pos / pixel_size) * pixel_size;



    // ==============================
    // 2. 动态噪声扰动坐标
    // ==============================

    float distortion1 = texture(
        noise_tex,
        pos * noise_scale +
        vec2(TIME * speed * 0.05, TIME * speed * 0.02)
    ).r;


    float distortion2 = texture(
        noise_tex,
        pos * noise_scale * 1.7 +
        vec2(-TIME * speed * 0.03, TIME * speed * 0.06)
    ).g;



    vec2 distorted_pos = pos;

    distorted_pos.x += (distortion1 - 0.5) * 40.0 * noise_strength;
    distorted_pos.y += (distortion2 - 0.5) * 40.0 * noise_strength;



    // ==============================
    // 3. 再采样两个不同方向噪声
    // ==============================

    float n1 = texture(
        noise_tex,
        distorted_pos * noise_scale +
        vec2(TIME * speed * 0.03,
        TIME * speed * 0.01)
    ).r;


    float n2 = texture(
        noise_tex,
        distorted_pos * noise_scale * 1.5 +
        vec2(-TIME * speed * 0.02,
        TIME * speed * 0.05)
    ).g;



    float combined_noise = (n1 + n2) * 0.5;



    // ==============================
    // 4. 非规则交叉水纹
    // ==============================

    float wave1 = sin(
        distorted_pos.x * wave_frequency +
        distorted_pos.y * wave_frequency * 0.5 +
        TIME * speed +
        n1 * 6.0
    );


    float wave2 = cos(
        distorted_pos.y * wave_frequency * 0.8 -
        distorted_pos.x * wave_frequency * 0.3 +
        TIME * speed * 1.3 +
        n2 * 6.0
    );


    float wave_height = (wave1 + wave2) * 0.5;



    // ==============================
    // 5. 浪花计算
    // ==============================

    float foam_threshold = 1.0 - wave_width;


    float foam_value =
        wave_height +
        (combined_noise - 0.5) * noise_strength;



    float wave_mask_foam =
        step(foam_threshold + 0.12, foam_value);


    float wave_mask_shallow =
        step(foam_threshold - 0.12, foam_value);



    // 微小闪光

    float micro_sparkle =
        step(
            0.85,
            combined_noise * wave_height
        );



    // ==============================
    // 6. 颜色混合
    // ==============================

    vec3 final_color = water_color.rgb;


    final_color = mix(
        final_color,
        shallow_color.rgb,
        wave_mask_shallow
    );


    final_color = mix(
        final_color,
        wave_color.rgb,
        wave_mask_foam
    );


    if (wave_mask_shallow < 0.1)
    {
        final_color = mix(
            final_color,
            shallow_color.rgb,
            micro_sparkle * 0.5
        );
    }


    COLOR = vec4(
        final_color,
        tex_color.a * water_color.a
    );
}