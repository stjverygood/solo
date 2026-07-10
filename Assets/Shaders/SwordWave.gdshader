shader_type canvas_item;

// --- 控制参数 ---
uniform vec4  attack_color : source_color = vec4(0.3, 0.8, 1.0, 1.0); // 剑气主色
uniform vec4  glow_color   : source_color = vec4(1.0, 1.0, 1.0, 1.0); // 核心白色
uniform float progress     : hint_range(0.0, 1.0) = 0.0;             // 斩击进度
uniform float pixel_size   : hint_range(8.0, 128.0) = 64.0;          // 像素风格子大小
uniform float arc_width    : hint_range(0.01, 0.5) = 0.05;           // 弧线粗细
uniform float arc_radius   : hint_range(0.1, 1.0) = 0.45;            // 弧线半径

// 像素噪声函数（用于消散）
float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

void fragment() {
    // 1. 实现像素化坐标 (Pixel Perfect Grid)
    vec2 grid_uv = floor(UV * pixel_size) / pixel_size;
    
    // 2. 将 UV 转换为以中心点为原点的极坐标 (-0.5 到 0.5)
    vec2 centered_uv = grid_uv - 0.5;
    
    // 计算点到中心的距离 (用于生成弧半径)
    float dist = length(centered_uv);
    
    // 计算角度 (用于限制剑气只在正前方成半圆)
    float angle = atan(centered_uv.y, centered_uv.x);
    
    // 3. 数学公式生成弧形 (Base Mask)
    // 限制距离在半径附近：
    float shape_mask = smoothstep(arc_radius + arc_width, arc_radius, dist) * smoothstep(arc_radius - arc_width, arc_radius, dist);
                      
    // 限制角度只显示左侧半圆 (因为 Godot 右侧是 0度，atan 范围是 -PI 到 PI)
    shape_mask *= smoothstep(-1.6, -1.4, angle) * smoothstep(1.6, 1.4, angle);
    
    // 4. 动态消散与像素颗粒 (Dissolve)
    float noise = hash(grid_uv * 10.0 + progress); // 加入 progress 让噪声动起来
    float dissolve_value = shape_mask - (progress * 1.5);
    
    // 裁剪掉透明像素和消散像素
    if (dissolve_value < noise * 0.25 || shape_mask < 0.1) {
        discard;
    }
    
    // 5. 颜色混合
    float edge = smoothstep(0.0, 0.3, dissolve_value);
    vec4 final_color = mix(attack_color, glow_color, edge);
    
    COLOR = vec4(final_color.rgb, attack_color.a);
}