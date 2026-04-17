namespace Launcher;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

	#region Windows Form Designer generated code

	/// <summary>
	///  Required method for Designer support - do not modify
	///  the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		Label fullScreenLbl;
		fullScreenLbl = new Label();
		SuspendLayout();
		// 
		// fullScreenLbl
		// 
		fullScreenLbl.AutoSize = true;
		fullScreenLbl.Location = new Point(12, 33);
		fullScreenLbl.Name = "fullScreenLbl";
		fullScreenLbl.Size = new Size(117, 20);
		fullScreenLbl.TabIndex = 0;
		fullScreenLbl.Text = "Fullscreen mode";
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(8F, 20F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(422, 450);
		Controls.Add(fullScreenLbl);
		Name = "MainForm";
		Text = "The Walking Game — Launcher";
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion
}
