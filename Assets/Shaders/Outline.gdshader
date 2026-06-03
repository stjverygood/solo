shader_type canvas_item;

// 暴露出轮廓颜色和粗细的变量，可以在外部面板直接调整
uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float outline_width : hint_range(0.0, 10.0) = 1.0;

void fragment() {
    // 获取纹理的像素间距
    vec2 size = TEXTURE_PIXEL_SIZE * outline_width;
    
    // 获取当前像素的颜色
    vec4 current_color = texture(TEXTURE, UV);
    
    // 如果当前像素本身不是透明的，保留原色
    if (current_color.a > 0.0) {
        COLOR = current_color;
    } else {
        // 采样上下左右四个方向的透明度
        float au = texture(TEXTURE, UV + vec2(0.0, -size.y)).a;
        float ad = texture(TEXTURE, UV + vec2(0.0, size.y)).a;
        float al = texture(TEXTURE, UV + vec2(-size.x, 0.0)).a;
        float ar = texture(TEXTURE, UV + vec2(size.x, 0.0)).a;
        
        // 如果周围有不透明的像素，说明这里是边缘，涂上轮廓颜色
        if (au + ad + al + ar > 0.0) {
            COLOR = outline_color;
        } else {
            COLOR = current_color;
        }
    }
}