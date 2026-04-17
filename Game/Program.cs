using Raylib_cs;
using static Raylib_cs.Raylib;
using Game;

internal static class Program
{
	readonly static string ResourcesFolder = Path.GetFullPath("../../../Resources/");
	static int RenderWidth { get; set; } = (int)(1920 / 1.5);
	static int RenderHeight { get; set; } = (int)(1080 / 1.5);
	static int Horizont => RenderHeight / 2;
	static readonly int StepSize = (int)(RenderHeight * 0.9f);

	static int DisplayedMapSize => RenderHeight / 5;
	static int TargetFPS => 60;
	readonly static Raycaster _raycaster = new();

	static class Themes
	{
		public readonly static Palette MonoLight = new Palette(
			WallMax: new Color(255, 255, 255),
			WallMin: new Color(127, 127, 127),
			FloorMax: new Color(55, 55, 55),
			FloorMin: new Color(15, 15, 15),
			Sky: new Color(0, 0, 0)
		);

		public readonly static Palette ColoredNight = new Palette(
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

		public readonly static Palette ColoredDay = new Palette(
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

		public readonly static Palette MonoDark = new Palette(
			WallMax: new Color(16, 16, 16),
			WallMin: new Color(0, 0, 0),
			FloorMax: new Color(16, 16, 16),
			FloorMin: new Color(0, 0, 0),
			Sky: new Color(0, 0, 0)
		);

		public readonly static Palette Lavaland = new Palette(
			WallMax: new Color(160, 10, 10),
			WallMin: new Color(140, 11, 11),
			FloorMax: new Color(120, 11, 11),
			FloorMin: new Color(100, 15, 15),
			Sky: new Color(240, 26, 22)
		);
	}

	readonly record struct Palette(
		Color WallMax,
		Color WallMin,
		Color FloorMax,
		Color FloorMin,
		Color Sky,
		Func<Direction, Color>? WallTint = null
	);

	private static void Main(string[] args)
	{
		InitWindow(RenderWidth, RenderHeight, "The Walking Game");
		SetWindowIcon(LoadImage(Path.Join(ResourcesFolder, "icon.png")));
		SetConfigFlags(ConfigFlags.ResizableWindow);
		SetConfigFlags(ConfigFlags.VSyncHint);

		SetTargetFPS(TargetFPS);

		Palette colors = Themes.ColoredNight;

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
		while (!WindowShouldClose())
		{
			TimeSpan deltaTime = TimeSpan.FromSeconds(GetFrameTime());
			if (IsWindowFocused())
			{
				SetMousePosition(RenderWidth / 2, RenderHeight / 2);
				HideCursor();
			}

			if (IsKeyDown(KeyboardKey.F))
				planeFactor -= 0.01f;
			if (IsKeyDown(KeyboardKey.V))
				planeFactor += 0.01f;

			if (IsKeyPressed(KeyboardKey.Z))
				ToggleFullscreen();

			if (IsKeyDown(KeyboardKey.C))
				player.FOV -= Angle.FromDegrees(0.2f);
			if (IsKeyDown(KeyboardKey.X))
				player.FOV += Angle.FromDegrees(0.2f);

			BeginDrawing();
			{
				// World
				{
					int displayedHorizont = (int)(Horizont + StepSize * player.StepState);
					DrawRectangle(0, 0, RenderWidth, RenderHeight - displayedHorizont, colors.Sky);
					DrawRectangleGradientV(0, displayedHorizont, RenderWidth, RenderHeight - displayedHorizont, colors.FloorMin, colors.FloorMax);

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

						int topMargin = (RenderHeight - wallHeight) / 2 + (int)(StepSize * player.StepState);

						Color color = Color.Lerp(colors.WallMin, colors.WallMax, t);
						if (colors.WallTint is not null)
							color = ColorTint(color, colors.WallTint(info.Direction));

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

				int infoBlockLeftMargin = 8;
				int infoBlockTopMargin = 8;

				// Map
				var drawMap = () =>
				{
					float scale = DisplayedMapSize / mapTexture.Height;
					int width = (int)(mapTexture.Width * scale);
					int height = (int)(mapTexture.Height * scale);
					int margin = 12;

					infoBlockTopMargin = margin - 6;
					infoBlockLeftMargin = margin + width + 8;

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

				DrawFPS(infoBlockLeftMargin, infoBlockTopMargin);
				DrawText($"View distance: {player.ViewDistance}", infoBlockLeftMargin, infoBlockTopMargin + 24, 20, Color.Gray);
				DrawText($"Position: {player.Position:0.##}", infoBlockLeftMargin, infoBlockTopMargin + 24 * 2, 20, Color.Gray);
				DrawText($"Rotate: {player.Rotate:0.##}", infoBlockLeftMargin, infoBlockTopMargin + 24 * 3, 20, Color.Gray);
				DrawText($"Plane factor: {planeFactor}", infoBlockLeftMargin, infoBlockTopMargin + 24 * 4, 20, Color.Gray);
				DrawText($"FOV: {player.FOV}", infoBlockLeftMargin, infoBlockTopMargin + 24 * 5, 20, Color.Gray);
			}
			EndDrawing();

			player.Update(deltaTime, gameMap);
		}

		CloseWindow();
	}
}