using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Game;

public class Program
{
	readonly static string ResourcesFolder = Path.GetFullPath("../../../Resources/");
	static int RenderWidth { get; set; } = (int)(1920 / 1.5);
	static int RenderHeight { get; set; } = (int)(1080 / 1.5);
	static int Horizont => RenderHeight / 2;
	static readonly int StepSize = RenderHeight / 10;

	static int DisplayedMapSize => RenderHeight / 5;
	static int TargetFPS => 60;
	readonly static Raycaster _raycaster = new();

	static class Themes
	{
		public readonly static Theme MonoLight = new Theme(
			WallMax: new Color(255, 255, 255),
			WallMin: new Color(127, 127, 127),
			FloorMax: new Color(55, 55, 55),
			FloorMin: new Color(15, 15, 15),
			Sky: new Color(0, 0, 0)
		);

		public readonly static Theme ColoredNight = new Theme(
			WallMax: MonoLight.WallMax,
			WallMin: MonoLight.WallMin,
			FloorMax: MonoLight.FloorMax,
			FloorMin: MonoLight.FloorMin,
			Sky: MonoLight.Sky,
#pragma warning disable CS8524 // switch so stupid with enums
			WallTint: static dir => ColorTint(dir switch
			{
				Direction.South => Color.Beige,
				Direction.North => Color.SkyBlue,
				Direction.East => Color.Pink,
				Direction.West => Color.Violet
			}, Color.White)
		);
#pragma warning restore CS8524

		public readonly static Theme ColoredDay = new Theme(
			WallMax: ColoredNight.WallMax,
			WallMin: ColoredNight.WallMin,
			FloorMax: ColoredNight.FloorMax,
			FloorMin: ColoredNight.FloorMin,
			Sky: ColorTint(Color.SkyBlue, Color.White),
			WallTint: static dir => ColorTint(dir switch
			{
				Direction.South => Color.Beige,
				Direction.North => Color.SkyBlue,
				Direction.East => Color.Pink,
				Direction.West => Color.Violet
			}, Color.White)
		);

		public readonly static Theme MonoDark = new Theme(
			WallMax: new Color(16, 16, 16),
			WallMin: new Color(0, 0, 0),
			FloorMax: new Color(16, 16, 16),
			FloorMin: new Color(0, 0, 0),
			Sky: new Color(0, 0, 0)
		);

		public readonly static Theme Lavaland = new Theme(
			WallMax: new Color(160, 10, 10),
			WallMin: new Color(140, 11, 11),
			FloorMax: new Color(120, 11, 11),
			FloorMin: new Color(100, 15, 15),
			Sky: new Color(240, 26, 22)
		);
	}

	readonly record struct Theme(
		Color WallMax,
		Color WallMin,
		Color FloorMax,
		Color FloorMin,
		Color Sky,
		Func<Direction, Color>? WallTint = null
	);

	public static void Run()
	{
		InitWindow(RenderWidth, RenderHeight, "The Walking Game");
		SetWindowIcon(LoadImage(Path.Join(ResourcesFolder, "icon.png")));
		SetConfigFlags(ConfigFlags.ResizableWindow);
		SetConfigFlags(ConfigFlags.VSyncHint);

		SetTargetFPS(TargetFPS);

		Theme theme = Themes.ColoredNight;

		Texture2D mapTexture = LoadTexture(Path.Join(ResourcesFolder, "Map2.png"));

		GameMap gameMap;
		bool usedPlugMap = false;
		Vector2 spawnPos;
		if (!OperatingSystem.IsWindows())
		{
			gameMap = GameMap.PlugMap;
			spawnPos = GameMap.PlugMapSpawnPoint;
			usedPlugMap = true;
		}
		else
		{
			// `Bitmap` required Windows (anyway, it's my pet project, nobody except me run this)
			gameMap = GameMap.FromImage(Resources.Map2, static px =>
			{
				if (px.IsColorEquals(Color.White))
					return GameObject.Wall;
				return GameObject.None;
			});

			var testSpawnPos = Player.FindSpawnPoint(gameMap, gameMap.PositionInCenter);
			if (!testSpawnPos.HasValue)
			{
				gameMap = GameMap.PlugMap;
				spawnPos = GameMap.PlugMapSpawnPoint;
				usedPlugMap = true;
			}
			else spawnPos = testSpawnPos.Value;
		}

		Player player = Player.SpawnAt(spawnPos);

		float planeFactor = 0.1f;
		bool interfaceEnabled = true;
		while (!WindowShouldClose())
		{
			TimeSpan deltaTime = TimeSpan.FromSeconds(GetFrameTime());
			if (IsWindowFocused())
			{
				SetMousePosition(RenderWidth / 2, RenderHeight / 2);
				HideCursor();
			}

			if (IsKeyDown(KeyboardKey.LeftSuper))
				planeFactor -= 0.01f;
			if (IsKeyDown(KeyboardKey.RightSuper))
				planeFactor += 0.01f;

			if (IsKeyPressed(KeyboardKey.Z))
				ToggleFullscreen();

			if (IsKeyPressed(KeyboardKey.F7))
				interfaceEnabled = !interfaceEnabled;


			if (IsKeyDown(KeyboardKey.C))
				player.FOV -= Angle.FromDegrees(0.2f);
			if (IsKeyDown(KeyboardKey.X))
				player.FOV += Angle.FromDegrees(0.2f);

			BeginDrawing();
			{
				// World
				{
					int displayedHorizont = (int)(Horizont + StepSize * player.StepAnimationState);
					DrawRectangle(0, 0, RenderWidth, RenderHeight - displayedHorizont, theme.Sky);
					DrawRectangleGradientV(0, displayedHorizont, RenderWidth, RenderHeight - displayedHorizont, theme.FloorMin, theme.FloorMax);

					Angle rayStep = player.FOV / RenderWidth;
					Angle startAngle = player.Rotate - player.FOV / 2;
					for (int i = 0; i < RenderWidth; i++)
					{
						Angle rayAngle = startAngle + rayStep * i;

						HitInfo info = _raycaster.CastRay(
							player.Position,
							rayAngle,
							gameMap.IsCollided,
							player.ViewDistance
						);

						if (info.Distance == -1)
							continue;

						Angle angleFromCenter = rayAngle - player.Rotate;
						float distance = info.Distance * MathF.Cos(angleFromCenter.Radians);

						float t = 1 - distance / player.ViewDistance;
						float planeDistance = planeFactor * RenderHeight / MathF.Tan(player.FOV.Radians * planeFactor);
						int wallHeight = (int)(planeDistance / distance);

						int topMargin = (RenderHeight - wallHeight) / 2 + (int)(StepSize * player.StepAnimationState);

						Color color = Color.Lerp(theme.WallMin, theme.WallMax, t);
						if (theme.WallTint is not null)
							color = ColorTint(color, theme.WallTint(info.Direction));

						if (wallHeight <= 0)
							continue;

						const float SHADE_TOP_ALPHA = 0.08f;
						const float SHADE_BOTTOM_ALPHA = SHADE_TOP_ALPHA / 4 * 0;
						Color shadeTopColor = ColorAlpha(color, SHADE_TOP_ALPHA);
						Color shadeBottomColor = ColorAlpha(color, SHADE_BOTTOM_ALPHA);

						DrawRectangle(i, topMargin, 1, wallHeight, color);
						DrawRectangleGradientV(i, topMargin + wallHeight, 1, wallHeight, shadeTopColor, shadeBottomColor);
					}
				}

				var drawInterface = () => {
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
						DrawRectanglePro(new Rectangle(playerPointX, playerPointY, playerPointRadius * 2, 2), new(0, 0), player.Rotate.Degrees, Color.Red);
					};
					if (!usedPlugMap) drawMap();

					DrawFPS(leftMargin, topMargin);

					string[] labels = [
						$"View distance: {player.ViewDistance}",
						$"Position: {player.Position:0.##}",
						$"Rotate: {player.Rotate:0.##}",
						$"Plane factor: {planeFactor}",
						$"FOV: {player.FOV}"
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