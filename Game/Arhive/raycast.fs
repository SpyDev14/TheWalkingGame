#version 330
in vec2 fragTexCoord;
out vec4 finalColor;

uniform sampler2D mapTexture;    // карта уровней (R=стена, G=...)
uniform sampler2D wallTexture;   // текстура для стен
uniform vec2 cameraPos;          // позиция камеры (x, y)
uniform float cameraAngle;       // угол камеры в радианах
uniform float fov = 1.0472;      // поле зрения (60°)
uniform int screenWidth;
uniform int screenHeight;

float castRay(vec2 start, vec2 dir, float maxDist) {
    vec2 step = sign(dir);
    vec2 deltaDist = vec2(
        length(vec2(dir.x, 0.0)) > 0.0001 ? 1.0 / abs(dir.x) : 1e10,
        length(vec2(0.0, dir.y)) > 0.0001 ? 1.0 / abs(dir.y) : 1e10
    );
    vec2 sideDist;
    ivec2 mapCell = ivec2(floor(start));
    ivec2 stepCell = ivec2(step);
    
    // DDA инициализация
    sideDist.x = (step.x > 0) ? (mapCell.x + 1 - start.x) * deltaDist.x : (start.x - mapCell.x) * deltaDist.x;
    sideDist.y = (step.y > 0) ? (mapCell.y + 1 - start.y) * deltaDist.y : (start.y - mapCell.y) * deltaDist.y;
    
    float dist = 0.0;
    bool hit = false;
    int side = 0; // 0=горизонталь, 1=вертикаль
    
    while (dist < maxDist && !hit) {
        if (sideDist.x < sideDist.y) {
            sideDist.x += deltaDist.x;
            mapCell.x += stepCell.x;
            side = 0;
        } else {
            sideDist.y += deltaDist.y;
            mapCell.y += stepCell.y;
            side = 1;
        }
        // Проверка, что внутри карты
        if (mapCell.x < 0 || mapCell.y < 0) break;
        float texel = texelFetch(mapTexture, mapCell, 0).r;
        if (texel > 0.1) { // стена
            hit = true;
            dist = (side == 0) ? sideDist.x - deltaDist.x : sideDist.y - deltaDist.y;
        }
    }
    return hit ? dist / 2 : maxDist / 2;
}

void main() {
    // Преобразуем координаты пикселя в нормализованные экранные координаты (-1..1 по ширине)
    float screenX = (fragTexCoord.x * 2.0 - 1.0) * (float(screenWidth)/float(screenHeight));
    float screenY = (1.0 - fragTexCoord.y * 2.0);
    
    // Направление луча: угол камеры + смещение по горизонтали
    float rayAngle = cameraAngle + (screenX * fov * 0.5);
    vec2 rayDir = vec2(cos(rayAngle), sin(rayAngle));
    
    // Пускаем луч
    float dist = castRay(cameraPos, rayDir, 10.0);
    
    // Устраняем искажения "рыбий глаз"
    dist *= cos(rayAngle - cameraAngle);
    
    // Вычисляем высоту стены
    float wallHeight = 1.0 / dist;
    // Определяем, попадает ли текущий пиксель в вертикальную полосу стены
    float halfScreen = float(screenHeight) * 0.5;
    float wallTop = halfScreen - wallHeight * halfScreen;
    float wallBottom = halfScreen + wallHeight * halfScreen;
    float y = (1.0 - fragTexCoord.y) * float(screenHeight); // инвертируем Y
    
    if (y >= wallTop && y <= wallBottom) {
        // Текстурирование (упрощённо: по X координате удара)
        // В реальном коде нужно вычислять точку удара и брать из wallTexture соответствующий цвет
        float texCoord = fract(fract(fragTexCoord.x * 10.0)); // заглушка
        vec3 color = texture(wallTexture, vec2(texCoord, 0.5)).rgb;
        // Затемнение по расстоянию
        color *= (1.0 - dist / 10.0);
        finalColor = vec4(color, 1.0);
    } else {
        // Небо и пол
        float gradient = (y < halfScreen) ? (1.0 - y/halfScreen) : (y - halfScreen)/halfScreen;
        finalColor = mix(vec4(0.2,0.2,0.4,1.0), vec4(0.5,0.3,0.1,1.0), gradient);
    }
}