namespace TrayLink
{
    partial class EditActionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditActionForm));
            textBoxName = new TextBox();
            textBoxPathOrUrl = new TextBox();
            label1 = new Label();
            label2 = new Label();
            buttonOk = new Button();
            SuspendLayout();
            // 
            // textBoxName
            // 
            textBoxName.Location = new Point(81, 22);
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(280, 27);
            textBoxName.TabIndex = 0;
            // 
            // textBoxPathOrUrl
            // 
            textBoxPathOrUrl.Location = new Point(81, 75);
            textBoxPathOrUrl.Name = "textBoxPathOrUrl";
            textBoxPathOrUrl.Size = new Size(625, 27);
            textBoxPathOrUrl.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 25);
            label1.Name = "label1";
            label1.Size = new Size(52, 20);
            label1.TabIndex = 2;
            label1.Text = "Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 78);
            label2.Name = "label2";
            label2.Size = new Size(65, 20);
            label2.TabIndex = 3;
            label2.Text = "Path/Url:";
            // 
            // buttonOk
            // 
            buttonOk.Location = new Point(310, 127);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(94, 44);
            buttonOk.TabIndex = 4;
            buttonOk.Text = "Save";
            buttonOk.UseVisualStyleBackColor = true;
            // 
            // EditActionForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(732, 183);
            Controls.Add(buttonOk);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBoxPathOrUrl);
            Controls.Add(textBoxName);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "EditActionForm";
            Text = "Edit";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxName;
        private TextBox textBoxPathOrUrl;
        private Label label1;
        private Label label2;
        private Button buttonOk;
    }
}