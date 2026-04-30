using Game.Maths;
using Game.Levels;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Game;

public class Program
{
	/// <summary>In pixels</summary>
	int RenderWidth { get; set; }

	/// <summary>In pixels</summary>
	int RenderHeight { get; set; }

	/// <summary>In pixels</summary>
	int HorizontPos => RenderHeight / 2;

	/// <summary>In pixels</summary>
	int StepSize => RenderHeight / 80;

	/// <summary>In pixels</summary>
	int MinimapHeight => RenderHeight / 5;

	/// <summary>In tiles</summary>
	readonly Size MinimapSize = new(16, 16);
	readonly RunArgs Args;

	readonly Raycaster _raycaster = new();

	public Program(RunArgs args)
	{
		RenderWidth = args.RenderSize.Width;
		RenderHeight = args.RenderSize.Height;
		Args = args;
	}

	public Program() : this(new RunArgs(
		RenderSize: new Size(1280, 720),
		TargetFps: 60,
		EnableVSync: true,
		WindowMode.Resizable
	)) { }

	public void Run()
	{
		InitAudioDevice();

		InitWindow(RenderWidth, RenderHeight, "The Walking Game");
		SetWindowIcon(LoadImage(Path.Join(REWORK_IT.ResourcesFolder, "icon.png")));

		SetWindowState(Args.WindowMode.AsConfigFlag());
		SetTargetFPS(Args.TargetFps);
		if (Args.EnableVSync)
			SetWindowState(ConfigFlags.VSyncHint);

		int currMonitorId = GetCurrentMonitor();
		int currMonitorHeight = GetMonitorHeight(currMonitorId);
		int currMonitorWidth = GetMonitorWidth(currMonitorId);
		SetWindowMaxSize(currMonitorWidth, currMonitorHeight);
		SetWindowMinSize(currMonitorWidth / 6, currMonitorHeight / 6);


		Theme theme = Theme.ColoredNight;

		string mapPath = Path.Join(REWORK_IT.ResourcesFolder, "Map2.png");
		GameLevel gameMap;
		if (!OperatingSystem.IsWindowsVersionAtLeast(6, 1)) // `Bitmap` required Windows 6.1+
			gameMap = CreateLevel.PlugLevel;
		else gameMap = CreateLevel.FromImage("Default", new Bitmap(mapPath), static px => {
			if (px.IsColorEquals(Color.White))
				return GameObject.Wall;
			if (px.IsColorEquals(new Color(255, 0, 0)))
				return GameObject.SpawnPoint;
			return GameObject.None;
		});

		Player player = new Player(gameMap.SpawnPointPos, () => gameMap);
		bool interfaceEnabled = true;
		while (!WindowShouldClose())
		{
			TimeSpan deltaTime = TimeSpan.FromSeconds(GetFrameTime());

			if (IsWindowResized())
			{
				RenderHeight = GetScreenHeight();
				RenderWidth  = GetScreenWidth();
			}

			if (IsKeyPressed(KeyboardKey.F2))
				ToggleFullscreen();

			if (IsKeyPressed(KeyboardKey.F7))
				interfaceEnabled = !interfaceEnabled;

			if (IsKeyPressed(KeyboardKey.B))
				gameMap.IsOutsideSolid = !gameMap.IsOutsideSolid;

			if (IsKeyPressed(KeyboardKey.Space)) UnitConverations.TILES_PER_METER += 0.125f;
			if (IsKeyPressed(KeyboardKey.V)) UnitConverations.TILES_PER_METER -= 0.125f;

			// For unload after frame drawing
			// DON'T UNLOAD TEXTURE BEFORE EndDrawing!!!
			// Raylib use delayed drawing (draw frame at EndDrawing,
			// Draw functions just add commands to command buffer)
			// Unloading BEFORE EndDrawing() let to use-after-free
			// and texture drawed as... ASCII character sheet?...
			// I think it's undefined behavour (my first <3).
			Texture2D minimapAsTexture = default;
			BeginDrawing();
			{
				ClearBackground(Color.Black);

				// World
				{
					int horizontOffset = (int)(
						StepSize * Math.Abs(player.StepPhase)
						* player.StepVisualSizeModifier.Vertical
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

						float planeDistance = RenderWidth / 2f / MathF.Tan(player.FOV.Radians / 2f);
						int wallHeight = (int)(planeDistance / distance * UnitConverations.TILES_PER_METER);

						float wallHeightRatio = wallHeight / RenderHeight;
						int topMargin = (RenderHeight - wallHeight) / 2 + (int)(horizontOffset * (1 - wallHeightRatio));

						Color color = ColorLerp(
							theme.WallFar,
							theme.WallNear,
							1 - distance / player.ViewDistance
						);
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
					{
						Vector2 playerMarkPosOffset = new();
						{
							int tilesPerPixel = MinimapHeight / MinimapSize.Height;
							Image minimapImg = gameMap.GetMinimapImage(
								MinimapSize, player.Position, tilesPerPixel, out playerMarkPosOffset
							);
							minimapAsTexture = LoadTextureFromImage(minimapImg);
							UnloadImage(minimapImg);
						}

						float scale = MinimapHeight / (float)minimapAsTexture.Height;

						int width = (int)(minimapAsTexture.Width * scale);
						int height = (int)(minimapAsTexture.Height * scale);
						int margin = 12;

						topMargin = margin - 6;
						leftMargin = margin + width + 8;

						foreach (var x in new (int i, Color c)[] {
							(4, Color.White),
							(3, Color.Black),
							(1, Color.White)
						}) DrawRectangle(margin - x.i, margin - x.i, width + x.i * 2, height + x.i * 2, x.c);

						DrawTextureEx(minimapAsTexture, new(margin, margin), 0, scale, Color.White);
						// Unload texture after EndDrawing

						var drawPlayer = () => {
							float playerMarkRadius = 3.5f;
							float playerMarkX = margin + (minimapAsTexture.Width  / 2f + playerMarkPosOffset.X) * scale;
							float playerMarkY = margin + (minimapAsTexture.Height / 2f + playerMarkPosOffset.Y) * scale;
							DrawCircle(
								(int)playerMarkX,
								(int)playerMarkY,
								playerMarkRadius,
								Color.Red
							);
							DrawRectanglePro(
								new Rectangle(playerMarkX, playerMarkY, playerMarkRadius * 2, 2),
								new(0, 0),
								player.Rotate.Degrees,
								Color.Red
							);
						};
						if (!gameMap.IsOutsideMap(player.Position))
							drawPlayer();
					}

					DrawFPS(leftMargin, topMargin);

					string[] labels = [
						$"Position: {player.Position:0.##}",
						$"Rotate: {player.Rotate:0.##}",
						$"Speed: {player.CurrentSpeed:0.###} / {player.CurrentMaxSpeed:0.###}",
						$"FOV: {player.FOV:0.##}",
						$"Tiles per meter: {UnitConverations.TILES_PER_METER}",
					];

					for (int i = 0; i < labels.Length; i++)
						DrawText(labels[i], leftMargin, topMargin + (i+1) * 24, 20, Color.Gray);
				};

				if (interfaceEnabled)
					drawInterface();
			}
			EndDrawing();

			if (minimapAsTexture.Id != 0)
				UnloadTexture(minimapAsTexture);

			player.Update(deltaTime);
		}

		CloseWindow();
	}
	private static void Main(string[] args) => new Program().Run();
}

public readonly record struct RunArgs(
	Size RenderSize,
	int TargetFps,
	bool EnableVSync,
	WindowMode WindowMode = WindowMode.Resizable
);
