using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Game.Audio;


internal readonly struct SoundCollection
{
	private static readonly Random _rng = new Random();
	private readonly Sound[] _sounds;

	public SoundCollection(IEnumerable<Sound> sounds)
		=> _sounds = sounds.Where(s => IsSoundValid(s)).ToArray();
	public SoundCollection(IEnumerable<string> soundPaths)
		: this(soundPaths.Select(p => LoadSound(p))) { }

	public bool IsEmpty => _sounds.Length == 0;

	public Sound? Play()
	{
		if (_sounds.Length == 0)
			return null;

		var sound = _sounds.Choose(_rng);
		PlaySound(sound);
		return sound;
	}
}
