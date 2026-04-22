using Game.Types;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Game;

public class Program
{
	public readonly static string ResourcesFolder = Path.GetFullPath("../../../../Game/Resources/");
	static int RenderWidth { get; set; } = (int)(1920 / 1.5);
	static int RenderHeight { get; set; } = (int)(1080 / 1.5);
	static int HorizontPos => RenderHeight / 2;

	static readonly int StepSize = RenderHeight / 80;

	static int DisplayedMapSize => RenderHeight / 5;
	static int TargetFPS => 60;
	readonly static Raycaster _raycaster = new();

	static class Themes
	{
		public readonly static Theme MonoLight = new Theme(
			WallNear: new Color(255, 255, 255),
			WallFar: new Color(127, 127, 127),
			FloorNear: new Color(55, 55, 55),
			FloorFar: new Color(15, 15, 15),
			Sky: new Color(0, 0, 0)
		);

		public readonly static Theme ColoredNight = new Theme(
			WallNear: MonoLight.WallNear,
			WallFar: MonoLight.WallFar,
			FloorNear: MonoLight.FloorNear,
			FloorFar: MonoLight.FloorFar,
			Sky: MonoLight.Sky,
			WallTint: static dir => ColorTint(dir switch
			{
				Direction.South => Color.Beige,
				Direction.North => Color.SkyBlue,
				Direction.East => Color.Pink,
				Direction.West or _ => Color.Violet // (Direction)228 - is valid Direction (enum) in C# 😡
			}, Color.White)
		);

		public readonly static Theme ColoredDay = new Theme(
			WallNear: ColoredNight.WallNear,
			WallFar: ColoredNight.WallFar,
			FloorNear: ColoredNight.FloorNear,
			FloorFar: ColoredNight.FloorFar,
			Sky: ColorTint(Color.SkyBlue, Color.White),
			WallTint: ColoredNight.WallTint
		);

		public readonly static Theme MonoDark = new Theme(
			WallNear: new Color(16, 16, 16),
			WallFar: new Color(0, 0, 0),
			FloorNear: new Color(16, 16, 16),
			FloorFar: new Color(0, 0, 0),
			Sky: new Color(0, 0, 0)
		);

		public readonly static Theme Lavaland = new Theme(
			WallNear: new Color(0x60, 0x48, 0x48),
			WallFar: new Color(60, 47, 47),
			FloorNear: new Color(56, 46, 46),
			FloorFar: new Color(35, 30, 30),
			Sky: new Color(100, 11, 11)
		);

		public readonly static Theme Backroums = new Theme(
			WallNear: new Color(172, 165, 53),
			WallFar: new Color(68, 59, 0),
			FloorNear: new Color(114, 92, 17),
			FloorFar: new Color(46, 32, 0),
			Sky: new Color(154, 143, 64)
		);
	}

	readonly record struct Theme(
		Color WallNear,
		Color WallFar,
		Color FloorNear,
		Color FloorFar,
		Color Sky,
		Func<Direction, Color>? WallTint = null
	);

	public static void Run()
	{
		InitAudioDevice();

		InitWindow(RenderWidth, RenderHeight, "The Walking Game");
		SetWindowIcon(LoadImage(Path.Join(ResourcesFolder, "icon.png")));
		SetConfigFlags(ConfigFlags.ResizableWindow);
		SetConfigFlags(ConfigFlags.VSyncHint);

		SetTargetFPS(TargetFPS);

		Theme theme = Themes.ColoredNight;

		string mapPath = Path.Join(ResourcesFolder, "Map.png");
		Texture2D mapTexture = LoadTexture(mapPath);

		GameMap gameMap;
		bool usedPlugMap = false;
		if (!OperatingSystem.IsWindows())
		{
			gameMap = GameMap.PlugMap;
			usedPlugMap = true;
		}
		else
			// `Bitmap` required Windows
			gameMap = GameMap.FromImage(new Bitmap(mapPath), static px =>
			{
				if (px.IsColorEquals(Color.White))
					return GameObject.Wall;
				if (px.IsColorEquals(new Color(255, 0, 0)))
					return GameObject.SpawnPoint;
				return GameObject.None;
			});
		gameMap.IsOutsideSolid = false;

		Player player = Player.SpawnAt(gameMap.SpawnPoint);

		bool interfaceEnabled = true;
		while (!WindowShouldClose())
		{
			TimeSpan deltaTime = TimeSpan.FromSeconds(GetFrameTime());
			if (IsWindowFocused())
			{
				SetMousePosition(RenderWidth / 2, RenderHeight / 2);
				HideCursor();
			}

			if (IsKeyPressed(KeyboardKey.F2))
				ToggleFullscreen();

			if (IsKeyPressed(KeyboardKey.F7))
				interfaceEnabled = !interfaceEnabled;

			BeginDrawing();
			{
				// World
				{
					int horizontOffset = (int)(
						StepSize * (1 + Math.Abs(player.StepPhase) * player.StepVisualSizeModifier.Vertical)
					);

					// Sky & Floor
					{
						int skyY = 0;
						int skyHeight = HorizontPos + horizontOffset;
						int floorY = skyY + skyHeight;
						int floorHeight = RenderHeight + skyY - skyHeight;

						DrawRectangle(0, skyY, RenderWidth, skyHeight, theme.Sky);
						DrawRectangleGradientV(0, floorY, RenderWidth, floorHeight, theme.FloorFar, theme.FloorNear);
					}

					Angle rayStep = player.FOV / RenderWidth;
					Angle startAngle = player.Rotate - player.FOV / 2;
					Vector2 horizontalRenderingOffset = (
						(player.Rotate + Angle.Right).AsDirection()
						* player.StepPhase
						* -0.0125f
						* player.StepVisualSizeModifier.Horizontal
					);
					for (int i = 0; i < RenderWidth; i++)
					{
						Angle rayAngle = startAngle + rayStep * i;
						Angle angleFromCenter = rayAngle - player.Rotate;

						HitInfo info = _raycaster.CastRay(
							player.Position + horizontalRenderingOffset,
							rayAngle,
							gameMap.IsCollided,
							player.ViewDistance
						);

						if (info.Distance == -1)
							continue;

						float distance = info.Distance * MathF.Cos(angleFromCenter.Radians);

						float t = 1 - distance / player.ViewDistance;
						float planeDistance = (RenderWidth / 2) / MathF.Tan(player.FOV.Radians / 2);
						int wallHeight = (int)(planeDistance / distance * Constants.TILES_PER_METER);

						float wallHeightRatio = wallHeight / RenderHeight;
						int topMargin = (RenderHeight - wallHeight) / 2 + (int)(horizontOffset * (1 - wallHeightRatio));

						Color color = ColorLerp(theme.WallFar, theme.WallNear, t);
						if (theme.WallTint is not null)
							color = ColorTint(color, theme.WallTint(info.Direction));

						if (wallHeight <= 0)
							continue;

						const float SHADE_TOP_ALPHA = 0.08f;
						const float SHADE_BOTTOM_ALPHA = 0;

						Color shadeTopColor = ColorAlpha(color, SHADE_TOP_ALPHA);
						Color shadeBottomColor = ColorAlpha(color, SHADE_BOTTOM_ALPHA);

						DrawRectangle(i, topMargin, 1, wallHeight, color);
						DrawRectangleGradientV(i, topMargin + wallHeight, 1, wallHeight, shadeTopColor, shadeBottomColor);
					}
				}

				// UI
				var drawInterface = () => {
					// Cross
					int centerPos = RenderWidth / 2;
					const int CROSS_SIZE = 16;
					const int CROSS_WIDTH = 2;
					Color crossColor = ColorAlpha(Color.White, 0.5f);

					DrawRectangle(
						centerPos - CROSS_SIZE / 2,
						HorizontPos - CROSS_WIDTH / 2,
						CROSS_SIZE,
						CROSS_WIDTH,
						crossColor
					);
					DrawRectangle(
						centerPos - CROSS_WIDTH / 2,
						HorizontPos - CROSS_SIZE / 2,
						CROSS_WIDTH,
						CROSS_SIZE,
						crossColor
					);

					int leftMargin = 8;
					int topMargin = 8;

					// Map
					var drawMap = () => {
						float scale = DisplayedMapSize / mapTexture.Height;
						int width = (int)(mapTexture.Width * scale);
						int height = (int)(mapTexture.Height * scale);
						int margin = 12;

						topMargin = margin - 6;
						leftMargin = margin + width + 8;

						foreach (var x in new (int i, Color c)[] {
							(4, Color.White),
							(3, Color.Black),
							(1, Color.White)
						}) DrawRectangle(margin - x.i, margin - x.i, width + x.i * 2, height + x.i * 2, x.c);

						DrawTextureEx(mapTexture, new(margin, margin), 0, scale, Color.White);

						float playerPointRadius = 3.5f;
						float playerPointX = margin + player.Position.X * scale;
						float playerPointY = margin + player.Position.Y * scale;
						DrawCircle(
							(int)playerPointX,
							(int)playerPointY,
							playerPointRadius,
							Color.Red
						);
						DrawRectanglePro(
							new Rectangle(playerPointX, playerPointY, playerPointRadius * 2, 2),
							new(0, 0),
							player.Rotate.Degrees,
							Color.Red
						);
					};
					if (!usedPlugMap) drawMap();

					DrawFPS(leftMargin, topMargin);

					string[] labels = [
						$"Position: {player.Position:0.##}",
						$"Rotate: {player.Rotate:0.##}",
						$"Speed: {player.CurrentSpeed:0.###} / {player.CurrentMaxSpeed:0.###}",
						$"FOV: {player.FOV:0.##}",
					];

					for (int i = 0; i < labels.Length; i++)
						DrawText(labels[i], leftMargin, topMargin + (i+1) * 24, 20, Color.Gray);
				};

				if (interfaceEnabled)
					drawInterface();
			}
			EndDrawing();

			player.Update(deltaTime, gameMap);
		}

		CloseWindow();
	}

	private static void Main(string[] args)
	{
		Run();
	}
}

// Потом
public readonly record struct RunData(
	Size RenderSize,
	int TargetFps,
	bool EnableVSync,
	WindowMode WindowMode = WindowMode.Resizable
);

public enum WindowMode
{
	Fullscreen,
	Borderless,
	Resizable,
}