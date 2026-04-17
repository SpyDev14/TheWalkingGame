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
	private void InitializeComponent() {
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
		mainPicture = new PictureBox();
		fullScreenModeCheckBox = new CheckBox();
		borderlessWindowModeCheckBox = new CheckBox();
		enableVSyncCheckBox = new CheckBox();
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
		mainPicture.Image = (Image)resources.GetObject("mainPicture.Image");
		mainPicture.Location = new Point(12, 12);
		mainPicture.Name = "mainPicture";
		mainPicture.Size = new Size(345, 136);
		mainPicture.SizeMode = PictureBoxSizeMode.StretchImage;
		mainPicture.TabIndex = 1;
		mainPicture.TabStop = false;
		// 
		// fullScreenModeCheckBox
		// 
		fullScreenModeCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		fullScreenModeCheckBox.CheckAlign = ContentAlignment.MiddleRight;
		fullScreenModeCheckBox.Location = new Point(12, 160);
		fullScreenModeCheckBox.Name = "fullScreenModeCheckBox";
		fullScreenModeCheckBox.Size = new Size(345, 16);
		fullScreenModeCheckBox.TabIndex = 2;
		fullScreenModeCheckBox.Text = "Fullscreen mode:";
		fullScreenModeCheckBox.UseVisualStyleBackColor = true;
		// 
		// borderlessWindowModeCheckBox
		// 
		borderlessWindowModeCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		borderlessWindowModeCheckBox.CheckAlign = ContentAlignment.MiddleRight;
		borderlessWindowModeCheckBox.Location = new Point(12, 184);
		borderlessWindowModeCheckBox.Name = "borderlessWindowModeCheckBox";
		borderlessWindowModeCheckBox.Size = new Size(345, 16);
		borderlessWindowModeCheckBox.TabIndex = 3;
		borderlessWindowModeCheckBox.Text = "Borderless window:";
		borderlessWindowModeCheckBox.UseVisualStyleBackColor = true;
		// 
		// enableVSyncCheckBox
		// 
		enableVSyncCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		enableVSyncCheckBox.CheckAlign = ContentAlignment.MiddleRight;
		enableVSyncCheckBox.Checked = true;
		enableVSyncCheckBox.CheckState = CheckState.Checked;
		enableVSyncCheckBox.Location = new Point(12, 235);
		enableVSyncCheckBox.Name = "enableVSyncCheckBox";
		enableVSyncCheckBox.Size = new Size(345, 16);
		enableVSyncCheckBox.TabIndex = 4;
		enableVSyncCheckBox.Text = "VSync:";
		enableVSyncCheckBox.UseVisualStyleBackColor = true;
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
		Controls.Add(enableVSyncCheckBox);
		Controls.Add(borderlessWindowModeCheckBox);
		Controls.Add(fullScreenModeCheckBox);
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
	private CheckBox fullScreenModeCheckBox;
	private CheckBox borderlessWindowModeCheckBox;
	private CheckBox enableVSyncCheckBox;
	private ComboBox renderSizeSelect;
	private Label renderSizeLbl;
	private Button closeBtn;
	private Button runBtn;
}
