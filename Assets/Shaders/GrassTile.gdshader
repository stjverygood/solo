shader_type canvas_item;

// 确保在材质属性中，将 Texture Filter 设为 Nearest 保持像素风
uniform vec4 grass_color : source_color = vec4(0.3, 0.6, 0.2, 1.0);     // 你的基础草地颜色（如果贴图自带颜色，代码里会自动叠加）
uniform vec4 wind_color : source_color = vec4(0.4, 0.7, 0.3, 1.0);      // 风吹过时，亮部波纹的颜色

uniform float speed = 1.0;              // 风吹的速度
uniform float wind_density = 0.02;      // 风波纹的密度/粗细（值越小，波纹越宽大）
uniform float wave_threshold = 0.65;    // 波纹的清晰度/多寡（0.0~1.0，值越高，亮色波纹越少越克制）
uniform float pixel_size = 2.0;         // 像素颗粒大小（必须与你的水面 pixel_size 保持一致，画面才会协调）

// 噪声贴图：用来打破规律性，制造随机的风（建议用 FastNoiseLite，开启 Repeat 循环和 Nearest 过滤）
uniform sampler2D noise_tex : repeat_enable, filter_nearest;

// 从 vertex 传过来的世界坐标
varying vec2 world_pos;

void vertex() {
    // 依然使用世界坐标，保证多个草地瓦片拼在一起时，风能无缝跨图块吹过去
    world_pos = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment() {
    // 1. 采样瓦片自身 Alpha（如果是空白区域则裁剪掉，保留网格边缘）
    vec4 tex_color = texture(TEXTURE, UV);
    if (tex_color.a == 0.0) {
        discard;
    }
    
    // 2. 基于【世界坐标】进行像素化，防止平滑滤镜破坏像素风
    vec2 pos = floor(world_pos / pixel_size) * pixel_size;
    
    // 3. 制造风吹的方向（这里设定风向右下方吹：X轴往右，Y轴往下）
    // 通过 TIME 加上坐标，让噪声图自己滚动起来
    vec2 wind_uv = pos * wind_density - vec2(TIME * speed * 0.1, TIME * speed * 0.05);
    
    // 4. 采样两层不同缩放的噪声并叠加，让风的轨迹看起来更无序、更自然
    float noise1 = texture(noise_tex, wind_uv).r;
    float noise2 = texture(noise_tex, wind_uv * 1.8 + vec2(TIME * 0.02)).g;
    float combined_wind = (noise1 + noise2) * 0.5;
    
    // 5. 重点：像素风阶梯化处理（把连续的噪声变成一格一格的硬边缘光影）
    // 如果噪声强度超过了设定的阈值，就说明这块区域现在正“被风吹着”
    float wind_mask = step(wave_threshold, combined_wind);
    
    // 6. 混合颜色
    // 基础颜色融合：如果你的贴图本身就是绿色的，可以用乘法叠加；如果贴图是白色的，可以直接用 grass_color
    vec3 base_grass = tex_color.rgb * grass_color.rgb;
    
    // 如果有风吹过 (wind_mask = 1)，就把草地颜色平滑替换成亮色的 wind_color
    vec3 final_color = mix(base_grass, wind_color.rgb, wind_mask);
    
    // 7. 输出颜色
    COLOR = vec4(final_color, tex_color.a);
}