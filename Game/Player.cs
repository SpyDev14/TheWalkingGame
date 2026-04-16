namespace Game;

using Raylib_cs;
using Raylib = Raylib_cs.Raylib;

internal class Player
{
	public Angle FOV
	{
		get;
		set
		{
			if (value.Radians <= 0) throw new ArgumentException($"{nameof(FOV)} cannot be <= 0");
			field = value;
		}
	} = Angle.FromDegrees(60);

	public float ViewDistance
	{
		get;
		set
		{
			if (value <= 0) throw new ArgumentException($"{nameof(ViewDistance)} cannot be <= 0");
			field = value;
		}
	} = 18.0f;

	private Angle _rotate = default;
	public Angle Rotate => _rotate;

	public Vector2 Position { get; private set; }

	/// <summary>Radians per pixel</summary>
	public float MouseSensitivity { get; set; } = 0.001f;

	// In tiles per second
	private const float MAX_WALK_SPEED = 0.8f;
	private const float MAX_SPRINT_SPEED = 1.2f;

	private const float STEP_SIZE = 0.5f;

	private bool _isSprintNow = false;

	/// <summary>
	/// Current speed for display in tiles per sec
	/// </summary>
	public float Speed { get; }

	/// <summary>
	/// Value in range -1f..1f
	/// <para>1f  : Full top</para>
	/// <para>0   : Normal</para>
	/// <para>-1f : Full down</para>
	/// </summary>
	public float StepState { get; private set; }

	private const float VELOCITY_SATURATION = 1f;
	private readonly Vector2 _saturationVelocity = new Vector2(VELOCITY_SATURATION, VELOCITY_SATURATION);

	private Vector2 _velocity;
	public static Player SpawnAt(Vector2 pos) => new Player() { Position = pos };

	public static Vector2? FindSpawnPoint(GameMap map, Vector2 pos)
	{
		int maxRadius = Math.Max(map.Size.Width, map.Size.Height);
		//int currRadius = 0;

		return pos;
	}

	public void Move(float forward, float strafe)
	{
		_velocity.X += forward;
		_velocity.Y += strafe;
	}

	public void Teleport(float x, float y)
	{
		_velocity = new();
		Position = new(x, y);
	}

	public void Teleport(float x, float y, Angle rotate)
	{
		_rotate = rotate;
		Teleport(x, y);
	}

	/// <summary>
	/// Should be called any frame
	/// </summary>
	public void Update(TimeSpan deltaTime, GameMap map)
	{
		const float STEP_ANIMATION_SPEED = 0.0f;
		const float WALK_FORWARD_SPEED_FACTOR = 0.02f;
		const float WALK_STRAFE_SPEED_FACTOR = 0.01f;

		_rotate.Radians += Raylib.GetMouseDelta().X * MouseSensitivity;

		Vector2 ProcessForMove(Vector2 delta)
		{
			//if (map.IsCollided(Position))
			//	return delta;

			if (!map.IsCollided(Position + delta))
				return delta;

			delta = new Vector2(delta.X, 0);
			if (!map.IsCollided(Position + delta))
				return delta;

			delta = new Vector2(0, delta.Y);
			if (!map.IsCollided(Position + delta))
				return delta;

			return Vector2.Zero;
		}

		if (Raylib.IsKeyDown(KeyboardKey.W))
		{
			Position += ProcessForMove(Rotate.AsDirection() * WALK_FORWARD_SPEED_FACTOR);
			StepState += STEP_ANIMATION_SPEED;
		}

		if (Raylib.IsKeyDown(KeyboardKey.A))
		{
			Position += ProcessForMove(-((Rotate + Angle.FromDegrees(90)).AsDirection() * WALK_STRAFE_SPEED_FACTOR));
			StepState -= STEP_ANIMATION_SPEED / 2;
		}

		if (Raylib.IsKeyDown(KeyboardKey.S))
		{
			Position += ProcessForMove(-(Rotate.AsDirection() * WALK_FORWARD_SPEED_FACTOR));
			StepState -= STEP_ANIMATION_SPEED;
		}

		if (Raylib.IsKeyDown(KeyboardKey.D))
		{
			Position += ProcessForMove((Rotate + Angle.FromDegrees(90)).AsDirection() * WALK_STRAFE_SPEED_FACTOR);
			StepState += STEP_ANIMATION_SPEED / 2;
		}

		if (Raylib.IsKeyPressed(KeyboardKey.KpAdd) && ViewDistance < 20)
			ViewDistance += 0.5f;

		if (Raylib.IsKeyPressed(KeyboardKey.KpSubtract) && ViewDistance > 1)
			ViewDistance -= 0.5f;
	}

	public bool CanMove(float forward, float strafe, GameMap map)
	{
		return true;
	}
}
