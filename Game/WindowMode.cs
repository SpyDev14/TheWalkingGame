using Raylib_cs;

namespace Game;


public enum WindowMode
{
	Fullscreen,
	Borderless,
	Resizable,
}

public static class impl_WindowMode
{
	// Previously used in several places
	public static ConfigFlags AsConfigFlag(this WindowMode self) => self switch
	{
		WindowMode.Resizable  => ConfigFlags.ResizableWindow,
		WindowMode.Borderless => ConfigFlags.BorderlessWindowMode,
		WindowMode.Fullscreen => ConfigFlags.FullscreenMode,
		_ => throw new ArgumentOutOfRangeException(nameof(WindowMode)),
	};
}
