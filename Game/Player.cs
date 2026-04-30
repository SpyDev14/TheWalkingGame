using Game.Audio;
using Game.Levels;
using Game.Maths;
using Raylib_cs;
using Key = Raylib_cs.KeyboardKey;
using Raylib = Raylib_cs.Raylib;

namespace Game;

internal class Player
{
	/// <summary>Radians per pixel</summary>
	private float MouseSensitivity { get; set; } = 0.001f;
	bool _mouseCapturingEnabled = true;

	private const float ZOOM_SCALE = 0.5f;
	private bool _isZoom = false;

	private Angle _fov = Angle.FromDegrees(85);
	public Angle FOV => _fov * (_isZoom ? ZOOM_SCALE : 1);

	/// <summary>In tiles</summary>
	public float ViewDistance { get; private set; } = 12.0f.ToTiles();

	private Angle _rotate = default;
	public Angle Rotate => _rotate;
	/// <summary>Position in level coords (tiles)</summary>
	public Vector2 Position { get; private set; }


	// ==== [ Movenment ] ====
	/// <summary>Current speed in meters per sec</summary>
	public float CurrentSpeed => _velocity.Length();
	/// <summary>Current max speed in meters per sec</summary>
	public float CurrentMaxSpeed => _isSprint ? MAX_SPRINT_SPEED : MAX_SPEED;

	///<summary>m / sec</summary>
	private const float MAX_SPEED = 1.2f;
	private const float MAX_SPRINT_SPEED = MAX_SPEED * 1.5f;

	///<summary>Acceleration speed in m / sec</summary>
	private const float ACCELERATION = 8.0f;
	/// <summary>Stop speed in m / sec (sliding after lost control)</summary>
	private const float FRICTION = 2.0f;

	// Step animation
	/// <summary>meters for full cycle</summary>
	public float StepSize => _isSprint ? 1.2f : 1f;
	/// <summary>In ratio</summary>
	public (float Vertical, float Horizontal) StepVisualSizeModifier => _isSprint ? (1.05f, 1.1f) : (1, 1);

	///<summary>Full cycles per second on full speed</summary>
	private float StepAnimationSpeed => CurrentMaxSpeed / StepSize;

	public float _stepPhase = 0f; // 0..1

	/// <summary>Value in range -1f..1f where 1 - full top, -1 - full down</summary>
	public float StepPhase => MathF.Sin(_stepPhase * MathF.PI * 2);

	/// <summary>Radius of player circle-shape in meters</summary>
	private const float COLLISION_RADIUS = 0.15f;

	/// <summary>In meters</summary>
	private Vector2 _velocity;
	public Vector2 InputDirection { get; private set; }
	private bool _isSprint = false;

	/// <summary>
	/// Aka "ref GameMap". Returns current game level.
	/// In the future may be refactored to ILevelManager dependency.
	/// </summary>
	private readonly Func<GameLevel> _getCurrentLevel;

	private readonly SoundCollection _footstepSound = new(Enumerable
		.Range(1, 5)
		.Select(x => $"Audio/floor{x}.ogg")
		.Select(x => Path.Join(REWORK_IT.ResourcesFolder, x))
	);

	public Player(Vector2 pos, Func<GameLevel> getCurrentLevel)
	{
		Position = pos;
		_getCurrentLevel = getCurrentLevel;
	}

	/// <summary>
	/// Should be called any frame
	/// </summary>
	public void Update(TimeSpan deltaTime)
	{
		/*
		 * level can be replaced by:
		 *     _levelManager.GetCurrentLevel();
		 * who be get by:
		 *     [Dependency] ILevelManager _levelManager = default!;
		 * what be realized like:
		 *     dependencyContainer.Set<ILevelManager, LevelManager>(); // in Main
		 *     dependencyContainer.Get<ILevelManager>(); // In [Dependency] filling
		 * or just:
		 *    Player(ILevelManaer levelMan) {...} // Here
		 *    new Player(levelManager) // In Main
		 */
		static bool IsKeyDown(Key key) => Raylib.IsKeyDown(key);

		float dt = (float)deltaTime.TotalSeconds;
		var level = _getCurrentLevel();

		float forward = 0;
		float strafe = 0;
		if (IsKeyDown(Key.W)) forward += 1f;
		if (IsKeyDown(Key.S)) forward -= 1f;
		if (IsKeyDown(Key.D)) strafe += 1f;
		if (IsKeyDown(Key.A)) strafe -= 1f;
		if (IsKeyDown(Key.F)) _fov.Degrees += -Raylib.GetMouseWheelMove() * 1.2f;

		_isSprint = IsKeyDown(Key.LeftShift);
		_isZoom = IsKeyDown(Key.C);

		InputDirection = new(strafe, forward);
		if (!InputDirection.IsZero())
		{
			// Test DOOM-like moving
			// InputDirection = InputDirection.Normalized();
			Vector2 worldDirection = (
				(Rotate + Angle.Right).AsDirection() * InputDirection.X +
				Rotate.AsDirection() * InputDirection.Y
			);

			Vector2 targetVelocity = worldDirection * CurrentMaxSpeed;
			_velocity = Vector2.Lerp(_velocity, targetVelocity, ACCELERATION * dt);
		}
		else
		{
			// No input, apply friction if has velocity
			float friction = FRICTION * dt;
			if (_velocity.Length() <= friction)
				_velocity = Vector2.Zero;

			else _velocity -= _velocity.Normalized() * friction;

		}
		// Move (apply velocity)
		Vector2 moveDelta = GetCollisionAdjustedDelta(_velocity * dt * UnitConverations.TILES_PER_METER, level);
		Position += moveDelta;

		// Step animation & step sound
		float prevStepPhase = StepPhase;

		float speedFactor = Math.Clamp(_velocity.Length() / CurrentMaxSpeed, 0f, 1f);
		float animDelta = StepAnimationSpeed * speedFactor * dt;
		_stepPhase += animDelta;

		if ((StepPhase > 0 && prevStepPhase < 0) ||
			(StepPhase < 0 && prevStepPhase > 0))
			_footstepSound.Play();

		// Mouse
		if (Raylib.IsMouseButtonPressed(MouseButton.Left))
			_mouseCapturingEnabled = !_mouseCapturingEnabled;

		if (_mouseCapturingEnabled)
		{
			_rotate.NormalizedRadians += Raylib.GetMouseDelta().X * MouseSensitivity;
			_rotate = _rotate.Normalized();

			var centerPos = Raylib.GetScreenCenter();
			Raylib.SetMousePosition((int)centerPos.X, (int)centerPos.Y);
			Raylib.HideCursor();
		}
		else Raylib.ShowCursor();
	}

	private Vector2 GetCollisionAdjustedDelta(Vector2 delta, GameLevel level)
	{
		// If way is clear, let's go
		// If position is wrong, let escape
		if (CanMove(delta, level) || !CanMove(Vector2.Zero, level))
			return delta;

		Vector2 stepByX = new Vector2(delta.X, 0);
		Vector2 stepByY = new Vector2(0, delta.Y);

		if (CanMove(stepByX, level))
			return stepByX;

		if (CanMove(stepByY, level))
			return stepByY;

		// Don't let moving throught walls
		return Vector2.Zero;
	}

	private bool CanMove(Vector2 delta, GameLevel level) => !level.IsCircleCollided(
		Position + delta, COLLISION_RADIUS.ToTiles()
	);
}
