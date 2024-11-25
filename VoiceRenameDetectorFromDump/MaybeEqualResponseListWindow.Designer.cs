namespace VoiceRenameDetectorFromDump
{
    partial class MaybeEqualResponseListWindow
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
            EqualResponseList = new ListView();
            Equivalent = new ColumnHeader();
            Distance = new ColumnHeader();
            INFOFormID = new ColumnHeader();
            DIALEditorID = new ColumnHeader();
            QUSTEditorID = new ColumnHeader();
            ResponseText = new ColumnHeader();
            Instructions = new Label();
            SaveButton = new Button();
            AutoCheck = new Button();
            SuspendLayout();
            // 
            // EqualResponseList
            // 
            EqualResponseList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            EqualResponseList.CheckBoxes = true;
            EqualResponseList.Columns.AddRange(new ColumnHeader[] { Equivalent, Distance, INFOFormID, DIALEditorID, QUSTEditorID, ResponseText });
            EqualResponseList.Location = new Point(12, 27);
            EqualResponseList.Name = "EqualResponseList";
            EqualResponseList.Size = new Size(1141, 319);
            EqualResponseList.TabIndex = 1;
            EqualResponseList.UseCompatibleStateImageBehavior = false;
            EqualResponseList.View = View.Details;
            // 
            // Equivalent
            // 
            Equivalent.Text = "Equivalent";
            Equivalent.Width = 70;
            // 
            // Distance
            // 
            Distance.Text = "Distance";
            // 
            // INFOFormID
            // 
            INFOFormID.Text = "Info Form ID";
            INFOFormID.Width = 80;
            // 
            // DIALEditorID
            // 
            DIALEditorID.Text = "DIAL Editor ID";
            DIALEditorID.Width = 200;
            // 
            // QUSTEditorID
            // 
            QUSTEditorID.Text = "QUST Editor ID";
            QUSTEditorID.Width = 200;
            // 
            // ResponseText
            // 
            ResponseText.Text = "Response Text";
            ResponseText.Width = 500;
            // 
            // Instructions
            // 
            Instructions.AutoSize = true;
            Instructions.Location = new Point(12, 9);
            Instructions.Name = "Instructions";
            Instructions.Size = new Size(242, 15);
            Instructions.TabIndex = 0;
            Instructions.Text = "Check the items below which are equivalent.";
            // 
            // SaveButton
            // 
            SaveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            SaveButton.Location = new Point(305, 352);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(75, 23);
            SaveButton.TabIndex = 2;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // AutoCheck
            // 
            AutoCheck.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            AutoCheck.Location = new Point(12, 352);
            AutoCheck.Name = "AutoCheck";
            AutoCheck.Size = new Size(287, 23);
            AutoCheck.TabIndex = 3;
            AutoCheck.Text = "Check with Distance = 1 and Equivalent Editor IDs";
            AutoCheck.UseVisualStyleBackColor = true;
            AutoCheck.Click += AutoCheck_Click;
            // 
            // MaybeEqualResponseListWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1165, 387);
            Controls.Add(AutoCheck);
            Controls.Add(SaveButton);
            Controls.Add(Instructions);
            Controls.Add(EqualResponseList);
            Name = "MaybeEqualResponseListWindow";
            Text = "Maybe Equal Responses";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView EqualResponseList;
        private ColumnHeader Equivalent;
        private Label Instructions;
        private Button SaveButton;
        private ColumnHeader INFOFormID;
        private ColumnHeader DIALEditorID;
        private ColumnHeader QUSTEditorID;
        private ColumnHeader ResponseText;
        private ColumnHeader Distance;
        private Button AutoCheck;
    }
}