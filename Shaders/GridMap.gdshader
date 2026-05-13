shader_type canvas_item;

uniform vec4 grid_color : source_color = vec4(0.4, 0.4, 0.4, 1.0);
uniform float cell_size = 64.0;
uniform float line_thickness = 1.0;

// 这里的 varying 用于在 vertex 和 fragment 之间传递数据
varying vec2 world_pos;

void vertex() {
    // 使用内置的 MODEL_MATRIX 将局部顶点转换为世界坐标
    world_pos = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment() {
    vec2 pos = world_pos;
    
    // 计算网格
    vec2 grid = abs(fract(pos / cell_size - 0.5) - 0.5) * cell_size;
    float line = min(grid.x, grid.y);
    
    // 抗锯齿计算
    float alpha = smoothstep(line_thickness, line_thickness - 1.0, line);
    
    COLOR = vec4(grid_color.rgb, grid_color.a * alpha);
}