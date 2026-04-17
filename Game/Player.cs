namespace Game;

using Raylib_cs;
using System.Diagnostics;

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
	public float CurrentSpeed { get; }

	private const float MAX_SPEED = 0.8f; // tiles / second
	private const float MAX_SPRINT_SPEED = MAX_SPEED * 1.25f;

	///<summary>tiles / sec for get full speed</summary>
	private const float ACCELERATION = 8.0f;
	/// <summary>tiles / sec for full stop after lost control</summary>
	private const float FRICTION = 12.0f;
	///<summary>Inertion factor</summary> 
	private const float INERTION = 0.95f;


	// Step animation
	/// <summary>
	/// Value in range -1f..1f
	/// <para>1f  : Full top</para>
	/// <para>0   : Normal</para>
	/// <para>-1f : Full down</para>
	/// </summary>
	public float StepAnimationPhase => MathF.Sin(_stepAnimationPhase * MathF.PI * 2);
	private float _stepAnimationPhase = 0; // 0..1
	private const float STEP_SIZE = 0.5f; // tiles for full cycle (-1..1)
	///<summary>Full cycles per second on full speed</summary>
	private const float STEP_ANIMATION_SPEED = 12.0f;

	private Vector2 _velocity;
	private bool _isSprint = false;

	public static Player SpawnAt(Vector2 pos) => new Player() { Position = pos };

# warning remove and add spawn point pos to GameMap
	public static Vector2? FindSpawnPoint(GameMap map, Vector2 pos)
	{
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
# warning Сделать нормально
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
			_stepAnimationPhase += STEP_ANIMATION_SPEED;
		}

		if (Raylib.IsKeyDown(KeyboardKey.A))
		{
			Position += ProcessForMove(-((Rotate + Angle.FromDegrees(90)).AsDirection() * WALK_STRAFE_SPEED_FACTOR));
			_stepAnimationPhase -= STEP_ANIMATION_SPEED / 2;
		}

		if (Raylib.IsKeyDown(KeyboardKey.S))
		{
			Position += ProcessForMove(-(Rotate.AsDirection() * WALK_FORWARD_SPEED_FACTOR));
			_stepAnimationPhase -= STEP_ANIMATION_SPEED;
		}

		if (Raylib.IsKeyDown(KeyboardKey.D))
		{
			Position += ProcessForMove((Rotate + Angle.FromDegrees(90)).AsDirection() * WALK_STRAFE_SPEED_FACTOR);
			_stepAnimationPhase += STEP_ANIMATION_SPEED / 2;
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
