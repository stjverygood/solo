shader_type canvas_item;

// --- 控制参数 ---
uniform vec4 aura_color : source_color = vec4(0.0, 0.8, 1.0, 1.0); // 光环颜色
uniform float aura_radius : hint_range(0.0, 1.0) = 0.4;             // 光环基础半径
uniform float pixel_size : hint_range(1.0, 200.0) = 32.0;           // 像素颗粒度
uniform float bands : hint_range(1.0, 10.0) = 3.0;                  // 光环渐变层数
uniform float speed : hint_range(0.0, 10.0) = 2.0;                  // 呼吸动画速度
uniform float pulse_strength : hint_range(0.0, 0.5) = 0.05;         // 呼吸动画幅度

// --- 边界参数 ---
uniform vec4 border_color : source_color = vec4(1.0, 1.0, 1.0, 1.0); // 边界颜色
uniform float border_width : hint_range(0.0, 0.1) = 0.02;            // 边界宽度

void fragment() {
    // 1. 将 UV 中心点移到 (0,0)
    vec2 uv = UV - vec2(0.5);
    
    // 2. 获取当前 Control 节点的实际像素尺寸，防止拉伸变形
    vec2 size = 1.0 / TEXTURE_PIXEL_SIZE;
    uv.x *= size.x / size.y; 
    
    // 3. 实现像素化坐标
    if (pixel_size > 0.0) {
        uv = floor(uv * pixel_size) / pixel_size;
    }
    
    // 4. 计算像素点到中心的距离
    float dist = length(uv);
    
    // 5. 计算静止的总半径（边界是固定的）
    float total_radius = aura_radius + border_width;
    
    // 6. 渲染逻辑
    if (dist < total_radius) {
        if (dist >= aura_radius) {
            // 处于固定光环半径与总半径之间 -> 绘制静止的边界
            COLOR = border_color;
        } else {
            // --- 优化后的动画计算 ---
            // sin 的范围是 [-1.0, 1.0]
            // 通过 * 0.5 + 0.5，将范围映射到 [0.0, 1.0]
            float wave = sin(TIME * speed) * 0.5 + 0.5;
            
            // 让动态半径在 (aura_radius - pulse_strength) 和 aura_radius 之间完美平滑循环
            // 这样它既能保持连续的呼吸，又绝对不会触发 clamp 削顶
            float current_radius = aura_radius - (wave * pulse_strength);
            
            // 确保半径是个有效正数
            current_radius = max(current_radius, 0.01);
            
            if (dist < current_radius) {
                // 计算相对距离比例
                float pct = dist / current_radius;
                float band_pct = floor(pct * bands) / bands;
                float alpha = 1.0 - band_pct;
                
                COLOR = vec4(aura_color.rgb, alpha * aura_color.a);
            } else {
                // 收缩留下的空白区域填补为透明
                COLOR = vec4(0.0);
            }
        }
    } else {
        COLOR = vec4(0.0);
    }
}