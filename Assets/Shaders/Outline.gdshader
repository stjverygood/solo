shader_type canvas_item;

// === 1. 你原本的轮廓参数 ===
uniform vec4 outline_color : source_color = vec4(0.96, 0.93, 0.73, 1.0);
uniform float outline_width : hint_range(0.0, 10.0) = 1.0;

// === 2. 新增的闪白参数 ===
uniform float flash_modifier : hint_range(0.0, 1.0) = 0.0;
uniform vec4 flash_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
    // 获取纹理的像素间距
    vec2 size = TEXTURE_PIXEL_SIZE * outline_width;
    
    // 获取当前像素的颜色
    vec4 current_color = texture(TEXTURE, UV);
    
    // 声明一个临时变量用来存储计算好的画面颜色
    vec4 final_color;
    
    // ---- 【第一步：执行你原本的轮廓计算】 ----
    if (current_color.a > 0.0) {
        final_color = current_color;
    } else {
        // 采样上下左右四个方向的透明度
        float au = texture(TEXTURE, UV + vec2(0.0, -size.y)).a;
        float ad = texture(TEXTURE, UV + vec2(0.0, size.y)).a;
        float al = texture(TEXTURE, UV + vec2(-size.x, 0.0)).a;
        float ar = texture(TEXTURE, UV + vec2(size.x, 0.0)).a;
        
        // 如果周围有不透明的像素，说明这里是边缘，涂上轮廓颜色
        if (au + ad + al + ar > 0.0) {
            final_color = outline_color;
        } else {
            final_color = current_color;
        }
    }
    
    // ---- 【第二步：在这个基础上混合受击闪白】 ----
    // 根据 flash_modifier (0.0 到 1.0)，在算好的颜色和纯白之间插值
    final_color.rgb = mix(final_color.rgb, flash_color.rgb, flash_modifier);
    
    // ---- 【第三步：输出最终颜色】 ----
    COLOR = final_color;
}