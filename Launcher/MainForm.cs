using Program = Game.Program;
using Raylib_cs;

namespace Launcher;

public partial class MainForm : Form
{
	readonly Size[] _renderSizes = [
		new (1920, 1080),
		new (1760, 990),
		new (1680, 1050),
		new (1600, 900),
		new (1440, 900),
		new (1366, 768),
		new (1280, 1024),
		new (1280, 720),
		new (1128, 634),
		new (1024, 768),
		new (960, 540),
	];

	public MainForm()
	{
		InitializeComponent();

		renderSizeSelect.Items.Clear();
		foreach (var size in _renderSizes)
			renderSizeSelect.Items.Add(size);

		renderSizeSelect.Format += (_, e) =>
		{
			if (e.ListItem is Size size)
				e.Value = $"{size.Width} x {size.Height}";
		};
		renderSizeSelect.SelectedIndex = 0;
	}

	private void closeBtn_Click(object sender, EventArgs e) => Close();

	private void runBtn_Click(object sender, EventArgs e)
	{
		Hide();
		try { Game.Program.Run(); }
		finally { Close(); }
	}
}
