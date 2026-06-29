shader_type canvas_item;

// 确保在材质属性中，将 Texture Filter 设为 Nearest
uniform vec4 water_color : source_color = vec4(0.0, 0.35, 0.65, 1.0);     // 基础深水色
uniform vec4 shallow_color : source_color = vec4(0.05, 0.5, 0.75, 1.0);   // 浅水/过渡色
uniform vec4 wave_color : source_color = vec4(0.5, 0.85, 0.95, 1.0);      // 浪花颜色

uniform float speed = 1.2;               // 整体速度
uniform float wave_frequency = 0.04;     // 浪的密度
uniform float wave_width = 0.12;         // 浪尖宽度
uniform float pixel_size = 2.0;          // 像素大小

uniform sampler2D noise_tex : repeat_enable, filter_nearest;
uniform float noise_strength = 0.5;      // 扭曲强度
uniform float noise_scale = 0.01;        // 噪声缩放

// 使用 varying 将正确的世界坐标从 vertex 阶段传给 fragment 阶段
varying vec2 world_pos;

void vertex() {
    // Godot 4 标准 2D 世界坐标获取：模型矩阵 * 顶点
    world_pos = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    if (tex_color.a == 0.0) {
        discard;
    }
    
    // 1. 获取像素化的世界坐标
    vec2 pos = floor(world_pos / pixel_size) * pixel_size;
    
    // 2. 制造两个不同方向、不同速度的动态噪声，用于相互干涉
    vec2 time_offset1 = vec2(TIME * speed * 0.05, TIME * speed * 0.02);
    vec2 time_offset2 = vec2(-TIME * speed * 0.03, TIME * speed * 0.06);
    
    float n1 = texture(noise_tex, pos * noise_scale + time_offset1).r;
    float n2 = texture(noise_tex, pos * (noise_scale * 1.5) + time_offset2).g;
    
    // 混合噪声
    float combined_noise = (n1 + n2) * 0.5;
    
    // 3. 用双向干涉波（斜向交叉的波浪）
    float wave1 = sin(pos.x * wave_frequency + pos.y * wave_frequency * 0.5 + TIME * speed + n1 * 4.0);
    float wave2 = cos(pos.y * wave_frequency * 0.8 - pos.x * wave_frequency * 0.3 + TIME * speed * 1.3 + n2 * 4.0);
    
    // 双波叠加，形成乱序的动态水面
    float wave_height = (wave1 + wave2) * 0.5;
    
    // 4. 让浪花随着噪声消融断裂
    float foam_threshold = 1.0 - wave_width;
    float foam_value = wave_height + (combined_noise - 0.5) * noise_strength;
    
    // 5. 阶梯化处理（保持像素风硬边缘）
    float wave_mask_foam = step(foam_threshold + 0.15, foam_value);
    float wave_mask_shallow = step(foam_threshold - 0.15, foam_value);
    
    // 6. 额外增加一点深水区的微波闪烁
    float micro_sparkle = step(0.82, combined_noise * wave_height);
    
    // 7. 调色盘混合
    vec3 final_color = water_color.rgb;
    
    // 浅水层
    final_color = mix(final_color, shallow_color.rgb, wave_mask_shallow);
    // 浪花层
    final_color = mix(final_color, wave_color.rgb, wave_mask_foam);
    // 微波闪烁
    if (wave_mask_shallow < 0.1) {
        final_color = mix(final_color, shallow_color.rgb, micro_sparkle * 0.6);
    }
    
    COLOR = vec4(final_color, tex_color.a * water_color.a);
}