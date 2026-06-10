shader_type canvas_item;

// === 1. 轮廓参数 ===
uniform vec4 outline_color : source_color = vec4(0.96, 0.93, 0.73, 1.0);
uniform float outline_width : hint_range(0.0, 10.0) = 1.0;

// === 2. 闪白参数 ===
uniform float flash_modifier : hint_range(0.0, 1.0) = 0.0;
uniform vec4 flash_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
    // 获取纹理的像素间距
    vec2 size = TEXTURE_PIXEL_SIZE * outline_width;
    
    // 获取当前像素的颜色
    vec4 current_color = texture(TEXTURE, UV);
    
    // 声明一个临时变量用来存储计算好的画面颜色
    vec4 final_color = current_color;
    
    // ---- 【第一步：执行内轮廓计算】 ----
    // 只有当当前像素是不透明的（属于物体内部），才可能成为内轮廓
    if (current_color.a > 0.0) {
        // 采样上下左右四个方向的透明度
        float au = texture(TEXTURE, UV + vec2(0.0, -size.y)).a;
        float ad = texture(TEXTURE, UV + vec2(0.0, size.y)).a;
        float al = texture(TEXTURE, UV + vec2(-size.x, 0.0)).a;
        float ar = texture(TEXTURE, UV + vec2(size.x, 0.0)).a;
        
        // 如果四周有任意一个像素是透明的（a < 1.0 或 a == 0.0），
        // 说明当前像素贴着透明的背景，也就是物体的最外边缘。
        if (au * ad * al * ar < 1.0) {
            // 用轮廓颜色覆盖，但保留原本的透明度（防止半透明边缘直接变全不透明）
            final_color.rgb = outline_color.rgb;
            final_color.a = current_color.a; 
        }
    }
    // 如果当前像素本身就是透明的（current_color.a == 0.0），则保持原样，不生成向外的轮廓
    
    // ---- 【第二步：在这个基础上混合受击闪白】 ----
    // 根据 flash_modifier (0.0 到 1.0)，在算好的颜色和纯白之间插值
    final_color.rgb = mix(final_color.rgb, flash_color.rgb, flash_modifier);
    
    // ---- 【第三步：输出最终颜色】 ----
    COLOR = final_color;
}