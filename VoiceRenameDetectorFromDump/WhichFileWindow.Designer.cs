namespace VoiceRenameDetectorFromDump
{
  partial class WhichFileWindow
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      oblivionButton = new Button();
      skyrimButton = new Button();
      SuspendLayout();
      // 
      // oblivionButton
      // 
      oblivionButton.Location = new Point(12, 12);
      oblivionButton.Name = "oblivionButton";
      oblivionButton.Size = new Size(251, 55);
      oblivionButton.TabIndex = 0;
      oblivionButton.Text = "Oblivion";
      oblivionButton.UseVisualStyleBackColor = true;
      // 
      // skyrimButton
      // 
      skyrimButton.Location = new Point(12, 73);
      skyrimButton.Name = "skyrimButton";
      skyrimButton.Size = new Size(251, 55);
      skyrimButton.TabIndex = 1;
      skyrimButton.Text = "Skyrim";
      skyrimButton.UseVisualStyleBackColor = true;
      // 
      // WhichFileWindow
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(275, 138);
      Controls.Add(skyrimButton);
      Controls.Add(oblivionButton);
      FormBorderStyle = FormBorderStyle.FixedDialog;
      MaximizeBox = false;
      MinimizeBox = false;
      Name = "WhichFileWindow";
      Text = "What kind of file is this?";
      ResumeLayout(false);
    }

    #endregion

    private Button oblivionButton;
    private Button skyrimButton;
  }
}