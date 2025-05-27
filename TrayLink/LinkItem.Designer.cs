namespace TrayLink
{
    partial class LinkItem
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            labelType = new Label();
            labelName = new Label();
            buttonExecute = new Button();
            SuspendLayout();
            // 
            // labelType
            // 
            labelType.AutoSize = true;
            labelType.Location = new Point(38, 58);
            labelType.Name = "labelType";
            labelType.Size = new Size(41, 20);
            labelType.TabIndex = 0;
            labelType.Text = "TYPE";
            // 
            // labelName
            // 
            labelName.AutoSize = true;
            labelName.Location = new Point(28, 18);
            labelName.Name = "labelName";
            labelName.Size = new Size(51, 20);
            labelName.TabIndex = 1;
            labelName.Text = "NAME";
            // 
            // buttonExecute
            // 
            buttonExecute.Location = new Point(136, 18);
            buttonExecute.Name = "buttonExecute";
            buttonExecute.Size = new Size(114, 67);
            buttonExecute.TabIndex = 2;
            buttonExecute.Text = "EXECUTE";
            buttonExecute.UseVisualStyleBackColor = true;
            buttonExecute.Click += inputExecute;
            // 
            // LinkItem
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(buttonExecute);
            Controls.Add(labelName);
            Controls.Add(labelType);
            Name = "LinkItem";
            Size = new Size(270, 105);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelType;
        private Label labelName;
        private Button buttonExecute;
    }
}
