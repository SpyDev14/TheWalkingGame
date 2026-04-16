namespace Game.Arhive.Colors;

using Color = Raylib_cs.Color;

internal readonly struct PreProcessedGradient
{
	readonly ImmutableArray<Color> _gradientSteps;

	public PreProcessedGradient(Color from, Color to, int steps)
	{
		if (steps <= 0)
			throw new ArgumentOutOfRangeException(nameof(steps), "Should be > 0");

		var gradientSteps = new Color[steps];
		int diffA = from.A - to.A;
		int diffR = from.R - to.R;
		int diffG = from.G - to.G;
		int diffB = from.B - to.B;

		for (int i = 0; i < steps; i++)
		{
			var color = new Color(
				Math.Clamp(from.R + diffR, 0, 255),
				Math.Clamp(from.G + diffG, 0, 255),
				Math.Clamp(from.B + diffB, 0, 255),
				Math.Clamp(from.A + diffA, 0, 255)
			);
			gradientSteps[i] = color;
		}
		_gradientSteps = gradientSteps.ToImmutableArray();
	}

	public Color Get(double factor)
	{
		factor = Math.Clamp(factor, 0, 1 );
		int idx = (int)Math.Round(_gradientSteps.Length * factor);
		return _gradientSteps[idx];
	}
}
