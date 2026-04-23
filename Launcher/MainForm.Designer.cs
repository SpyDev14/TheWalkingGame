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
		mainPicture = new PictureBox();
		fullscreenModeChk = new CheckBox();
		borderlessWindowModeChk = new CheckBox();
		enableVSyncChk = new CheckBox();
		renderSizeSelect = new ComboBox();
		renderSizeLbl = new Label();
		closeBtn = new Button();
		runBtn = new Button();
		((System.ComponentModel.ISupportInitialize)mainPicture).BeginInit();
		SuspendLayout();
		// 
		// mainPicture
		// 
		mainPicture.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		mainPicture.BorderStyle = BorderStyle.Fixed3D;
		mainPicture.Image = Properties.Resources.main_picture_crop;
		mainPicture.Location = new Point(12, 12);
		mainPicture.Name = "mainPicture";
		mainPicture.Size = new Size(345, 136);
		mainPicture.SizeMode = PictureBoxSizeMode.StretchImage;
		mainPicture.TabIndex = 1;
		mainPicture.TabStop = false;
		// 
		// fullscreenModeChk
		// 
		fullscreenModeChk.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		fullscreenModeChk.CheckAlign = ContentAlignment.MiddleRight;
		fullscreenModeChk.Location = new Point(12, 160);
		fullscreenModeChk.Name = "fullScreenModeCheckBox";
		fullscreenModeChk.Size = new Size(345, 16);
		fullscreenModeChk.TabIndex = 2;
		fullscreenModeChk.Text = "Fullscreen mode:";
		fullscreenModeChk.UseVisualStyleBackColor = true;
		fullscreenModeChk.CheckedChanged += fullscreenModeChk_CheckedChanged;
		// 
		// borderlessWindowModeChk
		// 
		borderlessWindowModeChk.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		borderlessWindowModeChk.CheckAlign = ContentAlignment.MiddleRight;
		borderlessWindowModeChk.Location = new Point(12, 184);
		borderlessWindowModeChk.Name = "borderlessWindowModeCheckBox";
		borderlessWindowModeChk.Size = new Size(345, 16);
		borderlessWindowModeChk.TabIndex = 3;
		borderlessWindowModeChk.Text = "Borderless window:";
		borderlessWindowModeChk.UseVisualStyleBackColor = true;
		// 
		// enableVSyncChk
		// 
		enableVSyncChk.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		enableVSyncChk.CheckAlign = ContentAlignment.MiddleRight;
		enableVSyncChk.Checked = true;
		enableVSyncChk.CheckState = CheckState.Checked;
		enableVSyncChk.Location = new Point(12, 235);
		enableVSyncChk.Name = "enableVSyncCheckBox";
		enableVSyncChk.Size = new Size(345, 16);
		enableVSyncChk.TabIndex = 4;
		enableVSyncChk.Text = "VSync:";
		enableVSyncChk.UseVisualStyleBackColor = true;
		// 
		// renderSizeSelect
		// 
		renderSizeSelect.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		renderSizeSelect.DropDownStyle = ComboBoxStyle.DropDownList;
		renderSizeSelect.FlatStyle = FlatStyle.System;
		renderSizeSelect.FormattingEnabled = true;
		renderSizeSelect.Items.AddRange(new object[] { "1760 x 990", "1920 x 1080" });
		renderSizeSelect.Location = new Point(177, 206);
		renderSizeSelect.Name = "renderSizeSelect";
		renderSizeSelect.Size = new Size(180, 23);
		renderSizeSelect.TabIndex = 5;
		// 
		// renderSizeLbl
		// 
		renderSizeLbl.Location = new Point(13, 206);
		renderSizeLbl.Name = "renderSizeLbl";
		renderSizeLbl.Size = new Size(158, 23);
		renderSizeLbl.TabIndex = 6;
		renderSizeLbl.Text = "Render size";
		renderSizeLbl.TextAlign = ContentAlignment.MiddleLeft;
		// 
		// closeBtn
		// 
		closeBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		closeBtn.Location = new Point(282, 265);
		closeBtn.Name = "closeBtn";
		closeBtn.Size = new Size(75, 23);
		closeBtn.TabIndex = 7;
		closeBtn.Text = "Cancel";
		closeBtn.UseVisualStyleBackColor = true;
		closeBtn.Click += closeBtn_Click;
		// 
		// runBtn
		// 
		runBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		runBtn.Location = new Point(201, 265);
		runBtn.Name = "runBtn";
		runBtn.Size = new Size(75, 23);
		runBtn.TabIndex = 8;
		runBtn.Text = "Run";
		runBtn.UseVisualStyleBackColor = true;
		runBtn.Click += runBtn_Click;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(369, 298);
		ControlBox = false;
		Controls.Add(runBtn);
		Controls.Add(closeBtn);
		Controls.Add(renderSizeLbl);
		Controls.Add(renderSizeSelect);
		Controls.Add(enableVSyncChk);
		Controls.Add(borderlessWindowModeChk);
		Controls.Add(fullscreenModeChk);
		Controls.Add(mainPicture);
		FormBorderStyle = FormBorderStyle.FixedSingle;
		Margin = new Padding(3, 2, 3, 2);
		MaximizeBox = false;
		MinimizeBox = false;
		Name = "MainForm";
		Text = "The Walking Game — Launcher";
		((System.ComponentModel.ISupportInitialize)mainPicture).EndInit();
		ResumeLayout(false);
	}

	#endregion
	private PictureBox mainPicture;
	private CheckBox fullscreenModeChk;
	private CheckBox borderlessWindowModeChk;
	private CheckBox enableVSyncChk;
	private ComboBox renderSizeSelect;
	private Label renderSizeLbl;
	private Button closeBtn;
	private Button runBtn;
}
