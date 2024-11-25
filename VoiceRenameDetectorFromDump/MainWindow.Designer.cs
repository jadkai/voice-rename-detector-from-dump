namespace VoiceRenameDetectorFromDump
{
  partial class MainWindow
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
            startButton = new Button();
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader9 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            columnHeader10 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            columnHeader8 = new ColumnHeader();
            openFileDialog = new OpenFileDialog();
            SuspendLayout();
            // 
            // startButton
            // 
            startButton.Location = new Point(12, 12);
            startButton.Name = "startButton";
            startButton.Size = new Size(102, 31);
            startButton.TabIndex = 0;
            startButton.Text = "Start";
            startButton.UseVisualStyleBackColor = true;
            startButton.Click += startButton_Click;
            // 
            // listView1
            // 
            listView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader9, columnHeader3, columnHeader2, columnHeader4, columnHeader5, columnHeader10, columnHeader7, columnHeader6, columnHeader8 });
            listView1.FullRowSelect = true;
            listView1.Location = new Point(12, 49);
            listView1.Name = "listView1";
            listView1.Size = new Size(1205, 661);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Quest ID 1";
            columnHeader1.Width = 100;
            // 
            // columnHeader9
            // 
            columnHeader9.Text = "Topic name 1";
            columnHeader9.Width = 100;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "INFO Editor ID 1";
            columnHeader3.Width = 100;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Response number 1";
            columnHeader2.Width = 100;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Response text 1";
            columnHeader4.Width = 300;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Quest ID 2";
            columnHeader5.Width = 100;
            // 
            // columnHeader10
            // 
            columnHeader10.Text = "Topic name 2";
            columnHeader10.Width = 100;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "Response number 2";
            columnHeader6.Width = 100;
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "INFO Editor ID 2";
            columnHeader7.Width = 100;
            // 
            // columnHeader8
            // 
            columnHeader8.Text = "Response text 2";
            columnHeader8.Width = 300;
            // 
            // openFileDialog
            // 
            openFileDialog.Title = "Select Oblivion DIAL file";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1229, 722);
            Controls.Add(listView1);
            Controls.Add(startButton);
            Name = "MainWindow";
            Text = "Voice Rename Detector from Dump";
            ResumeLayout(false);
        }

        #endregion

        private Button startButton;
    private ListView listView1;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader3;
    private ColumnHeader columnHeader2;
    private ColumnHeader columnHeader4;
    private OpenFileDialog openFileDialog;
    private ColumnHeader columnHeader5;
    private ColumnHeader columnHeader6;
    private ColumnHeader columnHeader7;
    private ColumnHeader columnHeader8;
    private ColumnHeader columnHeader9;
    private ColumnHeader columnHeader10;
  }
}
