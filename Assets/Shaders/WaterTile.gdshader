shader_type canvas_item;

// 确保在材质属性中，将 Texture Filter 设为 Nearest 保持像素风
uniform vec4 water_color : source_color = vec4(0.0, 0.4, 0.7, 1.0);     // 基础深水色
uniform vec4 shallow_color : source_color = vec4(0.1, 0.5, 0.8, 1.0);   // 浅水/过渡色
uniform vec4 wave_color : source_color = vec4(0.2, 0.6, 0.9, 1.0);      // 浪花/波浪颜色

uniform float speed = 1.5;              // 浪推进的速度
uniform vec2 flow_dir = vec2(0.0, 1.0); // 浪的前进方向（0,1 表示从上往下）
uniform float wave_frequency = 0.05;    // 浪的标准频率（值越大，浪与浪之间的间距越近）
uniform float wave_width = 0.15;        // 核心浪带的粗细宽度（0.0 ~ 1.0）
uniform float pixel_size = 2.0;         // 像素颗粒大小

// 噪声贴图：用于打破规律性。在 Inspector 中开启 Repeat 循环和 Nearest 过滤
uniform sampler2D noise_tex : repeat_enable, filter_nearest;
uniform float noise_strength = 0.4;     // 噪声对波浪的干扰强度（值越大，浪越破碎歪曲）
uniform float noise_scale = 0.005;      // 基础噪声缩放大小（控制噪声颗粒的疏密）

// 定义一个从 vertex 传到 fragment 的变量：世界坐标
varying vec2 world_pos;

void vertex() {
    world_pos = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment() {
    // 1. 采样瓦片自身 Alpha（如果是空白区域则裁剪掉）
    vec4 tex_color = texture(TEXTURE, UV);
    if (tex_color.a == 0.0) {
        discard;
    }
    
    // 2. 基于【世界坐标】进行像素化
    vec2 pos = floor(world_pos / pixel_size) * pixel_size;
    vec2 dir = normalize(flow_dir);
    
    // 3. 【核心优化】采样大尺寸的低频宏观噪声，用于打破区域统一性
    // 缩放比原有噪声小 10 倍（基本看不见纹理，只提供平滑的区域过渡），且让它随时间极慢漂移
    vec2 macro_noise_uv = pos * (noise_scale * 0.1) + (dir * TIME * 0.005);
    float macro_noise = texture(noise_tex, macro_noise_uv).r;
    
    // 4. 采样原有的细节噪声贴图（负责浪花的局部锯齿破碎感）
    vec2 noise_uv = pos * noise_scale + (dir * TIME * speed * 0.02);
    float noise_val = texture(noise_tex, noise_uv).r; 
    
    // 5. 将二维位置投影到水流方向上
    float distort_dir = dot(pos, vec2(-dir.y, dir.x)); 
    
    // 【核心优化】利用大区域噪声生成一个随机的时间/相位偏移（最大偏移整个周期：2 * PI）
    // 这样不同区域的水面会处于不同的流动状态，再也不会“整齐排队”
    float phase_offset = macro_noise * 3.14159 * 2.0;
    
    float wave_progress = dot(pos, dir) * wave_frequency - (TIME * speed) + phase_offset;
    
    // 6. 融入传统的数学扰动和细节噪声扰动
    wave_progress += sin(distort_dir * 0.08) * 0.1;       // 基础微弱锯齿
    wave_progress += (noise_val - 0.5) * noise_strength;  // 细节噪声打破直线
    
    // 7. 使用 fract 将推进线转成本地 [0, 1] 的循环层浪
    float wave_chain = fract(wave_progress);
    
    // 8. 截取不同宽度的条带，形成“亮色浪尖”与“过渡浅水”
    float wave_mask_foam = step(1.0 - wave_width, wave_chain);          // 核心浪花
    float wave_mask_shallow = step(1.0 - (wave_width * 2.2), wave_chain); // 浪花后面的尾迹
    
    // 9. 三色混合（深水 -> 浅水 -> 浪花）
    vec3 final_color = water_color.rgb;
    final_color = mix(final_color, shallow_color.rgb, wave_mask_shallow);
    final_color = mix(final_color, wave_color.rgb, wave_mask_foam);
    
    // 10. 输出颜色
    COLOR = vec4(final_color, tex_color.a * water_color.a);
}