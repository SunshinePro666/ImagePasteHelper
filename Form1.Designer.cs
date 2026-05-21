namespace ImagePasteHelper
{
    partial class Form1
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
            convertButton = new Button();
            statusLabel = new Label();
            SuspendLayout();
            // 
            // convertButton
            // 
            convertButton.Location = new Point(20, 20);
            convertButton.Name = "convertButton";
            convertButton.Size = new Size(220, 34);
            convertButton.TabIndex = 0;
            convertButton.Text = "Convert copied image file";
            convertButton.UseVisualStyleBackColor = true;
            convertButton.Click += convertButton_Click;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(20, 70);
            statusLabel.MaximumSize = new Size(540, 0);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(123, 15);
            statusLabel.TabIndex = 1;
            statusLabel.Text = "Status: waiting for action";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 140);
            Controls.Add(statusLabel);
            Controls.Add(convertButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Image Paste Helper";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button convertButton;
        private Label statusLabel;
    }
}
