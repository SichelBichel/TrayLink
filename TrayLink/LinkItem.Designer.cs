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
            buttonDelete = new Button();
            buttonEdit = new Button();
            imageListIcons = new ImageList(components);
            buttonUp = new Button();
            buttonDown = new Button();
            SuspendLayout();
            // 
            // labelType
            // 
            labelType.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelType.AutoSize = true;
            labelType.ForeColor = SystemColors.HotTrack;
            labelType.ImageAlign = ContentAlignment.BottomLeft;
            labelType.Location = new Point(11, 71);
            labelType.Name = "labelType";
            labelType.Size = new Size(41, 20);
            labelType.TabIndex = 0;
            labelType.Text = "TYPE";
            // 
            // labelName
            // 
            labelName.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelName.ImageAlign = ContentAlignment.TopLeft;
            labelName.Location = new Point(11, 11);
            labelName.Name = "labelName";
            labelName.Size = new Size(177, 51);
            labelName.TabIndex = 1;
            labelName.Text = "NAME";
            // 
            // buttonExecute
            // 
            buttonExecute.Anchor = AnchorStyles.Right;
            buttonExecute.ImageKey = "rocket.png";
            buttonExecute.ImageList = imageList1;
            buttonExecute.Location = new Point(195, 17);
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
            imageList1.Images.SetKeyName(1, "icon_Uncheck.png");
            imageList1.Images.SetKeyName(2, "icon_stopall.png");
            // 
            // buttonDelete
            // 
            buttonDelete.ImageKey = "icon_Uncheck.png";
            buttonDelete.ImageList = imageList1;
            buttonDelete.Location = new Point(58, 65);
            buttonDelete.Margin = new Padding(3, 4, 3, 4);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Size = new Size(27, 32);
            buttonDelete.TabIndex = 4;
            buttonDelete.UseVisualStyleBackColor = true;
            buttonDelete.Click += inputDeleteEntry;
            // 
            // buttonEdit
            // 
            buttonEdit.ImageKey = "icon_Edit.png";
            buttonEdit.ImageList = imageListIcons;
            buttonEdit.Location = new Point(91, 65);
            buttonEdit.Margin = new Padding(3, 4, 3, 4);
            buttonEdit.Name = "buttonEdit";
            buttonEdit.Size = new Size(27, 32);
            buttonEdit.TabIndex = 5;
            buttonEdit.UseVisualStyleBackColor = true;
            buttonEdit.Click += inputEditEntry;
            // 
            // imageListIcons
            // 
            imageListIcons.ColorDepth = ColorDepth.Depth32Bit;
            imageListIcons.ImageStream = (ImageListStreamer)resources.GetObject("imageListIcons.ImageStream");
            imageListIcons.TransparentColor = Color.Transparent;
            imageListIcons.Images.SetKeyName(0, "icon_blitz.png");
            imageListIcons.Images.SetKeyName(1, "icon_Check.png");
            imageListIcons.Images.SetKeyName(2, "icon_create.png");
            imageListIcons.Images.SetKeyName(3, "icon_delete.png");
            imageListIcons.Images.SetKeyName(4, "icon_disable.png");
            imageListIcons.Images.SetKeyName(5, "icon_download.png");
            imageListIcons.Images.SetKeyName(6, "icon_Edit.png");
            imageListIcons.Images.SetKeyName(7, "icon_enable.png");
            imageListIcons.Images.SetKeyName(8, "icon_export.png");
            imageListIcons.Images.SetKeyName(9, "icon_help.png");
            imageListIcons.Images.SetKeyName(10, "icon_import.png");
            imageListIcons.Images.SetKeyName(11, "icon_information.png");
            imageListIcons.Images.SetKeyName(12, "icon_inject.png");
            imageListIcons.Images.SetKeyName(13, "icon_Load.png");
            imageListIcons.Images.SetKeyName(14, "icon_reload.png");
            imageListIcons.Images.SetKeyName(15, "icon_Save.png");
            imageListIcons.Images.SetKeyName(16, "icon_settings.png");
            imageListIcons.Images.SetKeyName(17, "icon_Start.png");
            imageListIcons.Images.SetKeyName(18, "icon_stopall.png");
            imageListIcons.Images.SetKeyName(19, "icon_Uncheck.png");
            imageListIcons.Images.SetKeyName(20, "icon_upload.png");
            imageListIcons.Images.SetKeyName(21, "icon_website.png");
            imageListIcons.Images.SetKeyName(22, "icon_clear.png");
            imageListIcons.Images.SetKeyName(23, "icon_console.png");
            imageListIcons.Images.SetKeyName(24, "icon_remove.png");
            imageListIcons.Images.SetKeyName(25, "icon_clearconsole.png");
            // 
            // buttonUp
            // 
            buttonUp.ImageKey = "icon_upload.png";
            buttonUp.ImageList = imageListIcons;
            buttonUp.Location = new Point(124, 65);
            buttonUp.Margin = new Padding(3, 4, 3, 4);
            buttonUp.Name = "buttonUp";
            buttonUp.Size = new Size(27, 32);
            buttonUp.TabIndex = 6;
            buttonUp.UseVisualStyleBackColor = true;
            buttonUp.Click += ClickMoveUp;
            // 
            // buttonDown
            // 
            buttonDown.ImageKey = "icon_download.png";
            buttonDown.ImageList = imageListIcons;
            buttonDown.Location = new Point(157, 65);
            buttonDown.Margin = new Padding(3, 4, 3, 4);
            buttonDown.Name = "buttonDown";
            buttonDown.Size = new Size(27, 32);
            buttonDown.TabIndex = 7;
            buttonDown.UseVisualStyleBackColor = true;
            buttonDown.Click += clickMoveDown;
            // 
            // LinkItem
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(buttonDown);
            Controls.Add(buttonUp);
            Controls.Add(buttonEdit);
            Controls.Add(buttonDelete);
            Controls.Add(buttonExecute);
            Controls.Add(labelName);
            Controls.Add(labelType);
            Name = "LinkItem";
            Size = new Size(270, 101);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelType;
        private Label labelName;
        private Button buttonExecute;
        private ImageList imageList1;
        private Button buttonDelete;
        private Button buttonEdit;
        private ImageList imageListIcons;
        private Button buttonUp;
        private Button buttonDown;
    }
}
