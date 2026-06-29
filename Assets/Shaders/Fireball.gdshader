shader_type particles;

// --- 材质属性 ---
uniform vec4 color_start : source_color = vec4(1.0, 0.6, 0.0, 1.0); // 核心火球颜色（橙黄）
uniform vec4 color_end : source_color = vec4(0.8, 0.1, 0.0, 0.0);   // 尾部消散颜色（暗红带透明）
uniform float initial_size = 1.0;
uniform float final_size = 0.2;
uniform vec2 base_velocity = vec2(-100.0, 0.0); // 默认向后喷射
uniform float randomness = 30.0;                // 随机扩散程度

// 自定义一个稳定高效的伪随机函数（输入种子，输出 0.0 ~ 1.0）
float hash_rand(uint seed) {
	seed = (seed ^ 61u) ^ (seed >> 16u);
	seed *= 9u;
	seed = seed ^ (seed >> 4u);
	seed *= 0x27d4eb2du;
	seed = seed ^ (seed >> 15u);
	return float(seed) / 4294967296.0;
}

void start() {
	// 使用系统提供的 NUMBER (粒子编号) 和 RANDOM_SEED 组合出不同的独立种子
	uint seed1 = RANDOM_SEED + NUMBER;
	uint seed2 = seed1 * 2654435761u;
	uint seed3 = seed2 * 2654435761u;

	// 1. 初始化位置 (让粒子在火球核心周围轻微散开，防止变成一条死板的线)
	float angle = hash_rand(seed1) * 6.2831853; // TAU
	float radius = hash_rand(seed2) * 6.0;      // 扩散半径 6 像素
	TRANSFORM[3].xyz = vec3(cos(angle) * radius, sin(angle) * radius, 0.0);

	// 2. 初始化速度 (基础速度 + 随机扰动)
	// 引入 seed3 让 X 和 Y 的随机表现不同
	float rand_x = (hash_rand(seed2) - 0.5) * randomness;
	float rand_y = (hash_rand(seed3) - 0.5) * randomness;
	VELOCITY.xyz = vec3(base_velocity.x + rand_x, base_velocity.y + rand_y, 0.0);

	// 3. 初始化颜色
	COLOR = color_start;
	
	// 4. 初始化大小 (这里加入了随机大小，完美替代了“随机寿命”带来的视觉变化)
	float rand_size_scale = 0.8 + hash_rand(seed1) * 0.4; // 80% 到 120% 的大小随机
	float size = initial_size * rand_size_scale;
	TRANSFORM[0].x = size;
	TRANSFORM[1].y = size;
}

void process() {
	// CUSTOM.y 是 Godot 粒子系统自带的：当前粒子归一化生命周期进度 (0.0 = 出生, 1.0 = 死亡)
	float age_ratio = CUSTOM.y; 

	// 1. 随着时间推移改变尺寸 (火球尾部变小)
	float current_size = mix(initial_size, final_size, age_ratio);
	TRANSFORM[0].x = current_size;
	TRANSFORM[1].y = current_size;

	// 2. 颜色渐变 & 渐隐 (从炽热变暗淡，最后透明)
	COLOR = mix(color_start, color_end, age_ratio);

	// 3. 模拟热气流上升 (2D 中 Y轴负方向是向上)
	VELOCITY.y += -60.0 * DELTA; 
}