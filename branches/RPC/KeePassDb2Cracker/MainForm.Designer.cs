namespace KeePassDbCracker
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.startButton = new System.Windows.Forms.Button();
            this.serversButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.serversGroupBox = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rateLabeledLabel = new NLib.Windows.Forms.LabeledLabel2();
            this.lastGuessLabeledLabel = new NLib.Windows.Forms.LabeledLabel2();
            this.databasePathLabeledTextBox = new NLib.Windows.Forms.LabeledTextBox();
            this.numThreadsLabeledTextBox = new NLib.Windows.Forms.LabeledTextBox();
            this.serversGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startButton.Location = new System.Drawing.Point(312, 190);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // serversButton
            // 
            this.serversButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.serversButton.Location = new System.Drawing.Point(375, 67);
            this.serversButton.Name = "serversButton";
            this.serversButton.Size = new System.Drawing.Size(75, 23);
            this.serversButton.TabIndex = 15;
            this.serversButton.Text = "Servers...";
            this.serversButton.UseVisualStyleBackColor = true;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(393, 190);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 16;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // serversGroupBox
            // 
            this.serversGroupBox.Controls.Add(this.label5);
            this.serversGroupBox.Controls.Add(this.label4);
            this.serversGroupBox.Controls.Add(this.label3);
            this.serversGroupBox.Controls.Add(this.label2);
            this.serversGroupBox.Controls.Add(this.label1);
            this.serversGroupBox.Controls.Add(this.serversButton);
            this.serversGroupBox.Location = new System.Drawing.Point(12, 76);
            this.serversGroupBox.Name = "serversGroupBox";
            this.serversGroupBox.Size = new System.Drawing.Size(456, 96);
            this.serversGroupBox.TabIndex = 19;
            this.serversGroupBox.TabStop = false;
            this.serversGroupBox.Text = "Servers";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "localhost:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(230, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "CPU Cores";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(323, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "% Capacity";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(417, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Rate";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(118, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Status";
            // 
            // rateLabeledLabel
            // 
            this.rateLabeledLabel.AutoSize = true;
            this.rateLabeledLabel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.rateLabeledLabel.LabelText = "Rate";
            this.rateLabeledLabel.Location = new System.Drawing.Point(66, 57);
            this.rateLabeledLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.rateLabeledLabel.Name = "rateLabeledLabel";
            this.rateLabeledLabel.Size = new System.Drawing.Size(60, 13);
            this.rateLabeledLabel.TabIndex = 22;
            this.rateLabeledLabel.ValueText = "N/A";
            // 
            // lastGuessLabeledLabel
            // 
            this.lastGuessLabeledLabel.AutoSize = true;
            this.lastGuessLabeledLabel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.lastGuessLabeledLabel.LabelText = "Last Guess";
            this.lastGuessLabeledLabel.Location = new System.Drawing.Point(36, 38);
            this.lastGuessLabeledLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.lastGuessLabeledLabel.Name = "lastGuessLabeledLabel";
            this.lastGuessLabeledLabel.Size = new System.Drawing.Size(90, 13);
            this.lastGuessLabeledLabel.TabIndex = 21;
            this.lastGuessLabeledLabel.ValueText = "N/A";
            // 
            // databasePathLabeledTextBox
            // 
            this.databasePathLabeledTextBox.LabelText = "Database Path:";
            this.databasePathLabeledTextBox.Location = new System.Drawing.Point(12, 12);
            this.databasePathLabeledTextBox.Name = "databasePathLabeledTextBox";
            this.databasePathLabeledTextBox.Size = new System.Drawing.Size(450, 20);
            this.databasePathLabeledTextBox.TabIndex = 11;
            this.databasePathLabeledTextBox.TextBoxText = "C:\\Users\\Mark\\Documents\\Master - Old.kdbx";
            // 
            // numThreadsLabeledTextBox
            // 
            this.numThreadsLabeledTextBox.LabelText = "Threads:";
            this.numThreadsLabeledTextBox.Location = new System.Drawing.Point(333, 50);
            this.numThreadsLabeledTextBox.Name = "numThreadsLabeledTextBox";
            this.numThreadsLabeledTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.numThreadsLabeledTextBox.Size = new System.Drawing.Size(129, 20);
            this.numThreadsLabeledTextBox.TabIndex = 23;
            this.numThreadsLabeledTextBox.TextBoxText = "1";
            // 
            // MainForm
            // 
            this.AcceptButton = this.startButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 225);
            this.Controls.Add(this.numThreadsLabeledTextBox);
            this.Controls.Add(this.rateLabeledLabel);
            this.Controls.Add(this.lastGuessLabeledLabel);
            this.Controls.Add(this.serversGroupBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.databasePathLabeledTextBox);
            this.Controls.Add(this.startButton);
            this.Name = "MainForm";
            this.Text = "KeePass 2 Database Cracker";
            this.serversGroupBox.ResumeLayout(false);
            this.serversGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private NLib.Windows.Forms.LabeledTextBox databasePathLabeledTextBox;
        private System.Windows.Forms.Button serversButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.GroupBox serversGroupBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private NLib.Windows.Forms.LabeledLabel2 lastGuessLabeledLabel;
        private NLib.Windows.Forms.LabeledLabel2 rateLabeledLabel;
        private NLib.Windows.Forms.LabeledTextBox numThreadsLabeledTextBox;

    }
}

