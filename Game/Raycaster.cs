namespace Game;

internal enum Direction : byte
{
	South,
	North,
	East,
	West,
}

internal readonly record struct HitInfo(float Distance, Direction Direction, (int x, int y) Cell);
internal class Raycaster
{
	public HitInfo CastRay(Vector2 pos, Angle angle, Func<int, int, bool> isWall, float maxDistance)
	{
		Vector2 direction = angle.AsDirection();

		int mapX = (int)pos.X;
		int mapY = (int)pos.Y;
		int mapStepX = Math.Sign(direction.X);
		int mapStepY = Math.Sign(direction.Y);

		// Сколько нужно пройти по лучу, чтобы сменить одну клетку по вертикали
		float distanceDeltaX = direction.X != 0 ? MathF.Abs(1 / direction.X) : float.PositiveInfinity;
		// Сколько нужно пройти по лучу, чтобы сменить одну клетку по горизонтали
		float distanceDeltaY = direction.Y != 0 ? MathF.Abs(1 / direction.Y) : float.PositiveInfinity;
		
		// Расстояние до стороны следующей клетки.
		// Начинаем проверять с первой перед лучём
		// До следующей вертикальной стороны клетки
		float distanceToSideX = direction.X switch
		{
			> 0 => (mapX + 1 - pos.X) / direction.X,
			< 0 => (pos.X - mapX) / -direction.X,
			_ => float.PositiveInfinity // 0 or NaN, но NaN здесь никогда не будет
		};

		float distanceToSideY = direction.Y switch
		{
			> 0 => (mapY + 1 - pos.Y) / direction.Y,
			< 0 => (pos.Y - mapY) / -direction.Y,
			_ => float.PositiveInfinity // 0 or NaN, но NaN здесь никогда не будет
		};

		// Вместо постоянных перерасчётов
		Direction horizontalSide = mapStepX > 0 ? Direction.South : Direction.North;
		Direction verticalSide   = mapStepY > 0 ? Direction.West  : Direction.East;

		Direction side;
		float distance = 0;
		while (true)
		{
			if (distanceToSideX < distanceToSideY)
			{
				distanceToSideX += distanceDeltaX;
				mapX += mapStepX;
				side = horizontalSide;
			}
			else
			{
				distanceToSideY += distanceDeltaY;
				mapY += mapStepY;
				side = verticalSide;
			}

			distance = side == horizontalSide
				? distanceToSideX - distanceDeltaX
				: distanceToSideY - distanceDeltaY;

			if (isWall(mapX, mapY))
				break;

			if (distance > maxDistance)
				return new HitInfo(-1, side, (mapX, mapY));
		}

		return new HitInfo(distance, side, (mapX, mapY));
	}
}
