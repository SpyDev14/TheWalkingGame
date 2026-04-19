namespace Game;

using Raylib = Raylib_cs.Raylib;
using Key = Raylib_cs.KeyboardKey;

using System.Diagnostics;
using Game.Audio;

internal class Player
{
	/// <summary>Radians per pixel</summary>
	public float MouseSensitivity { get; set; } = 0.001f;

	public Angle FOV {
		get; set {
			if (value.Degrees > 5) field = value;
			else Debug.Write($"WARN {nameof(FOV)}.set: cannot be < 5, ignore set.");
		}
	} = Angle.FromDegrees(85);

	public float ViewDistance {
		get; set {
			if (value > 0) field = value;
			else Debug.Write($"WARN {nameof(ViewDistance)}.set: cannot be < 5, ignore set.");
		}
	} = 18.0f;

	private Angle _rotate = default;
	public Angle Rotate => _rotate;
	public Vector2 Position { get; private set; }


	// ==== [ Movenment ] ===
	/// <summary>Current speed for display in tiles per sec</summary>
	public float CurrentSpeed => _velocity.Length();
	public float CurrentMaxSpeed => _isSprint ? MAX_SPRINT_SPEED : MAX_SPEED;

	private const float MAX_SPEED = 0.8f; // tiles / second
	private const float MAX_SPRINT_SPEED = MAX_SPEED * 1.5f;

	///<summary>tiles / sec for get full speed</summary>
	private const float ACCELERATION = 8.0f;
	/// <summary>tiles / sec for full stop after lost control</summary>
	private const float FRICTION = 2.0f;

	// Step animation
	/// <summary>
	/// Value in range -1f..1f
	/// <para>1f  : Full top</para>
	/// <para>0   : Normal</para>
	/// <para>-1f : Full down</para>
	/// </summary>
	public float StepAnimationPhase => MathF.Sin(_stepAnimationPhase * MathF.PI * 2);

	public float _stepAnimationPhase = 0f; // 0..1

	private const float STEP_SIZE = 1f; // tiles in full cycle (-1..1)
	///<summary>Full cycles per second on full speed</summary>
	private float StepAnimationSpeed => CurrentMaxSpeed / STEP_SIZE;

	public (float Vertical, float Horizontal) DisplayedStepSizeModifier => _isSprint ? (1.05f, 1.2f) : (1, 1);

	private const float COLLISION_RADIUS = 0.3f; // tiles

	private Vector2 _velocity;
	private bool _isSprint = false;

	private readonly SoundCollection _footstepSound = new(Enumerable
		.Range(1, 6)
		.Select(x => $"Audio/floor{x}.ogg")
		.Select(x => Path.Join(Program.ResourcesFolder, x))
	);

	public static Player SpawnAt(Vector2 pos) => new Player() { Position = pos };

# warning remove and add spawn point pos to GameMap
	public static Vector2? FindSpawnPoint(GameMap map, Vector2 pos)
	{
		return pos;
	}

	/// <summary>
	/// Should be called any frame
	/// </summary>
	public void Update(TimeSpan deltaTime, GameMap map)
	{
		/*
		 * map can be replaced by:
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
		const float MAX_DELTA = 0.033f;
		float dt = MathF.Min((float)deltaTime.TotalSeconds, MAX_DELTA);

		float forward = 0;
		float strafe = 0;
		
		if (Raylib.IsKeyDown(Key.W)) forward += 1f;
		if (Raylib.IsKeyDown(Key.S)) forward -= 1f;
		if (Raylib.IsKeyDown(Key.D)) strafe += 1f;
		if (Raylib.IsKeyDown(Key.A)) strafe -= 1f;

		_isSprint = Raylib.IsKeyDown(Key.LeftShift);

		Vector2 inputDirection = new(strafe, forward);
		if (!inputDirection.IsZero())
		{
			inputDirection = inputDirection.Normalized();
			Vector2 worldDirection = (
				(Rotate + Angle.Right).AsDirection() * inputDirection.X +
				Rotate.AsDirection() * inputDirection.Y
			);

			Vector2 targetVelocity = worldDirection * CurrentMaxSpeed;
			_velocity = Vector2.Lerp(_velocity, targetVelocity, ACCELERATION * dt);
		}
		else
		{
			// No input, apply friction if has velocity
			float friction = FRICTION * dt;
			if (_velocity.Length() <= friction)
				_velocity = Vector2.Zero; // memcp faster than checks

			else _velocity -= _velocity.Normalized() * friction;
		}

		// Step animation & step sound
		float prevStepAnimationPhase = StepAnimationPhase;

		float speedFactor = Math.Clamp(_velocity.Length() / CurrentMaxSpeed, 0f, 1f);
		float animDelta = StepAnimationSpeed * speedFactor * dt;
		_stepAnimationPhase += animDelta;

		if ((StepAnimationPhase > 0 && prevStepAnimationPhase < 0) ||
			(StepAnimationPhase < 0 && prevStepAnimationPhase > 0))
			_footstepSound.Play();

		// Move (apply velocity)
		Position += CalculateCollisionCorrectMoveDelta(_velocity * dt, map);

		// Mouse
		_rotate.Radians += Raylib.GetMouseDelta().X * MouseSensitivity;
	}
# warning Rename it
	public Vector2 CalculateCollisionCorrectMoveDelta(Vector2 previewDelta, GameMap map)
	{
		// If way is clear, let go
		// If position is wrong, let escape
		if (CanMove(Position + previewDelta, map) ||
			!CanMove(Position, map))
			return previewDelta;

		Vector2 stepByX = new Vector2(previewDelta.X, 0);
		Vector2 stepByY = new Vector2(0, previewDelta.Y);

		if (CanMove(Position + stepByX, map))
			return stepByX;

		if (CanMove(Position + stepByY, map))
			return stepByY;

		// Don't let moving throught walls
		return Vector2.Zero;
	}
	
	// NOTE: I think it's may be a part of GameMap
	private bool CanMove(Vector2 newPos, GameMap map)
	{
		return !map.IsCollided(newPos);
		return (
			!map.IsCollided(newPos + new Vector2(1, 0)) &&
			!map.IsCollided(newPos - new Vector2(1, 1).Normalized()) &&
			!map.IsCollided(newPos + new Vector2(0, 1)) &&
			!map.IsCollided(newPos + new Vector2(-1, 1))
		);
	}
}
