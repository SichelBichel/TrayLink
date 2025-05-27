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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinkItem));
            labelType = new Label();
            labelName = new Label();
            buttonExecute = new Button();
            imageList1 = new ImageList(components);
            label1 = new Label();
            SuspendLayout();
            // 
            // labelType
            // 
            labelType.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelType.AutoSize = true;
            labelType.ForeColor = SystemColors.HotTrack;
            labelType.ImageAlign = ContentAlignment.BottomLeft;
            labelType.Location = new Point(82, 65);
            labelType.Name = "labelType";
            labelType.Size = new Size(41, 20);
            labelType.TabIndex = 0;
            labelType.Text = "TYPE";
            // 
            // labelName
            // 
            labelName.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelName.ImageAlign = ContentAlignment.TopLeft;
            labelName.Location = new Point(12, 10);
            labelName.Name = "labelName";
            labelName.Size = new Size(177, 50);
            labelName.TabIndex = 1;
            labelName.Text = "NAME";
            // 
            // buttonExecute
            // 
            buttonExecute.Anchor = AnchorStyles.Right;
            buttonExecute.ImageKey = "rocket.png";
            buttonExecute.ImageList = imageList1;
            buttonExecute.Location = new Point(195, 18);
            buttonExecute.Name = "buttonExecute";
            buttonExecute.Size = new Size(67, 67);
            buttonExecute.TabIndex = 2;
            buttonExecute.UseVisualStyleBackColor = true;
            buttonExecute.Click += inputExecute;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "rocket.png");
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.ImageAlign = ContentAlignment.BottomLeft;
            label1.Location = new Point(12, 65);
            label1.Name = "label1";
            label1.Size = new Size(73, 20);
            label1.TabIndex = 3;
            label1.Text = "Link Type:";
            // 
            // LinkItem
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(label1);
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
        private ImageList imageList1;
        private Label label1;
    }
}
