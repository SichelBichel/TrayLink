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
            labelType = new Label();
            comboType = new ComboBox();
            labelPosition = new Label();
            numPosition = new NumericUpDown();
            labelPositionMax = new Label();
            buttonBrowse = new Button();
            buttonOk = new Button();
            buttonCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)numPosition).BeginInit();
            SuspendLayout();
            //
            // label1
            //
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlLightLight;
            label1.Location = new Point(12, 25);
            label1.Name = "label1";
            label1.Size = new Size(52, 20);
            label1.TabIndex = 10;
            label1.Text = "Name:";
            //
            // textBoxName
            //
            textBoxName.BackColor = SystemColors.ScrollBar;
            textBoxName.Location = new Point(100, 22);
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(320, 27);
            textBoxName.TabIndex = 0;
            //
            // labelType
            //
            labelType.AutoSize = true;
            labelType.ForeColor = SystemColors.ControlLightLight;
            labelType.Location = new Point(12, 63);
            labelType.Name = "labelType";
            labelType.Size = new Size(43, 20);
            labelType.TabIndex = 11;
            labelType.Text = "Type:";
            //
            // comboType
            //
            comboType.BackColor = SystemColors.ScrollBar;
            comboType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboType.FormattingEnabled = true;
            comboType.Location = new Point(100, 59);
            comboType.Name = "comboType";
            comboType.Size = new Size(120, 28);
            comboType.TabIndex = 1;
            //
            // labelPosition
            //
            labelPosition.AutoSize = true;
            labelPosition.ForeColor = SystemColors.ControlLightLight;
            labelPosition.Location = new Point(250, 63);
            labelPosition.Name = "labelPosition";
            labelPosition.Size = new Size(64, 20);
            labelPosition.TabIndex = 12;
            labelPosition.Text = "Position:";
            //
            // numPosition
            //
            numPosition.BackColor = SystemColors.ScrollBar;
            numPosition.Location = new Point(320, 60);
            numPosition.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            numPosition.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numPosition.Name = "numPosition";
            numPosition.Size = new Size(100, 27);
            numPosition.TabIndex = 2;
            numPosition.TextAlign = HorizontalAlignment.Right;
            numPosition.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // labelPositionMax
            //
            labelPositionMax.AutoSize = true;
            labelPositionMax.ForeColor = SystemColors.ControlLight;
            labelPositionMax.Location = new Point(426, 63);
            labelPositionMax.Name = "labelPositionMax";
            labelPositionMax.Size = new Size(36, 20);
            labelPositionMax.TabIndex = 13;
            labelPositionMax.Text = "/ 1";
            //
            // label2
            //
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ControlLightLight;
            label2.Location = new Point(12, 102);
            label2.Name = "label2";
            label2.Size = new Size(65, 20);
            label2.TabIndex = 14;
            label2.Text = "Path/Url:";
            //
            // textBoxPathOrUrl
            //
            textBoxPathOrUrl.BackColor = SystemColors.ScrollBar;
            textBoxPathOrUrl.Location = new Point(100, 99);
            textBoxPathOrUrl.Name = "textBoxPathOrUrl";
            textBoxPathOrUrl.Size = new Size(562, 27);
            textBoxPathOrUrl.TabIndex = 3;
            //
            // buttonBrowse
            //
            buttonBrowse.Location = new Point(668, 98);
            buttonBrowse.Name = "buttonBrowse";
            buttonBrowse.Size = new Size(44, 29);
            buttonBrowse.TabIndex = 4;
            buttonBrowse.Text = "...";
            buttonBrowse.UseVisualStyleBackColor = true;
            //
            // buttonOk
            //
            buttonOk.Location = new Point(518, 142);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(94, 40);
            buttonOk.TabIndex = 5;
            buttonOk.Text = "Save";
            buttonOk.UseVisualStyleBackColor = true;
            //
            // buttonCancel
            //
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(618, 142);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(94, 40);
            buttonCancel.TabIndex = 6;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            //
            // EditActionForm
            //
            AcceptButton = buttonOk;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            CancelButton = buttonCancel;
            ClientSize = new Size(732, 196);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOk);
            Controls.Add(buttonBrowse);
            Controls.Add(textBoxPathOrUrl);
            Controls.Add(label2);
            Controls.Add(labelPositionMax);
            Controls.Add(numPosition);
            Controls.Add(labelPosition);
            Controls.Add(comboType);
            Controls.Add(labelType);
            Controls.Add(textBoxName);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EditActionForm";
            ShowInTaskbar = false;
            Text = "Edit";
            ((System.ComponentModel.ISupportInitialize)numPosition).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxName;
        private TextBox textBoxPathOrUrl;
        private Label label1;
        private Label label2;
        private Label labelType;
        private ComboBox comboType;
        private Label labelPosition;
        private NumericUpDown numPosition;
        private Label labelPositionMax;
        private Button buttonBrowse;
        private Button buttonOk;
        private Button buttonCancel;
    }
}
