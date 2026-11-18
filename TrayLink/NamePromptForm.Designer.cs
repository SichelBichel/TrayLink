namespace TrayLink
{
    partial class NamePromptForm
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
            labelPrompt = new Label();
            textName = new TextBox();
            buttonOk = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            //
            // labelPrompt
            //
            labelPrompt.AutoSize = true;
            labelPrompt.Location = new Point(12, 15);
            labelPrompt.Name = "labelPrompt";
            labelPrompt.Size = new Size(52, 20);
            labelPrompt.TabIndex = 0;
            labelPrompt.Text = "Name:";
            //
            // textName
            //
            textName.Location = new Point(12, 40);
            textName.MaxLength = 80;
            textName.Name = "textName";
            textName.Size = new Size(376, 27);
            textName.TabIndex = 1;
            //
            // buttonOk
            //
            buttonOk.Location = new Point(188, 82);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(94, 32);
            buttonOk.TabIndex = 2;
            buttonOk.Text = "OK";
            buttonOk.UseVisualStyleBackColor = true;
            //
            // buttonCancel
            //
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(294, 82);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(94, 32);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            //
            // NamePromptForm
            //
            AcceptButton = buttonOk;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(400, 126);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOk);
            Controls.Add(textName);
            Controls.Add(labelPrompt);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NamePromptForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Name";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelPrompt;
        private TextBox textName;
        private Button buttonOk;
        private Button buttonCancel;
    }
}
