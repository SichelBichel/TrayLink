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
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();
            pnlAccent = new Panel();
            ((System.ComponentModel.ISupportInitialize)numPosition).BeginInit();
            pnlHeader.SuspendLayout();
            SuspendLayout();
            //
            // pnlHeader
            //
            pnlHeader.BackColor = Color.FromArgb(20, 28, 48);
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(732, 52);
            pnlHeader.TabIndex = 20;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.FromArgb(238, 242, 250);
            lblHeaderTitle.Location = new Point(12, 12);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Size = new Size(97, 28);
            lblHeaderTitle.TabIndex = 0;
            lblHeaderTitle.Text = "Edit entry";
            //
            // pnlAccent
            //
            pnlAccent.Dock = DockStyle.Top;
            pnlAccent.Location = new Point(0, 52);
            pnlAccent.Name = "pnlAccent";
            pnlAccent.Size = new Size(732, 3);
            pnlAccent.TabIndex = 21;
            pnlAccent.Paint += pnlAccent_Paint;
            //
            // label1
            //
            label1.AutoSize = true;
            label1.ForeColor = Color.FromArgb(20, 28, 48);
            label1.Location = new Point(12, 83);
            label1.Name = "label1";
            label1.Size = new Size(52, 20);
            label1.TabIndex = 10;
            label1.Text = "Name:";
            //
            // textBoxName
            //
            textBoxName.BackColor = Color.White;
            textBoxName.Location = new Point(100, 80);
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(320, 27);
            textBoxName.TabIndex = 0;
            //
            // labelType
            //
            labelType.AutoSize = true;
            labelType.ForeColor = Color.FromArgb(20, 28, 48);
            labelType.Location = new Point(12, 121);
            labelType.Name = "labelType";
            labelType.Size = new Size(43, 20);
            labelType.TabIndex = 11;
            labelType.Text = "Type:";
            //
            // comboType
            //
            comboType.BackColor = Color.White;
            comboType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboType.FormattingEnabled = true;
            comboType.Location = new Point(100, 117);
            comboType.Name = "comboType";
            comboType.Size = new Size(120, 28);
            comboType.TabIndex = 1;
            //
            // labelPosition
            //
            labelPosition.AutoSize = true;
            labelPosition.ForeColor = Color.FromArgb(20, 28, 48);
            labelPosition.Location = new Point(250, 121);
            labelPosition.Name = "labelPosition";
            labelPosition.Size = new Size(64, 20);
            labelPosition.TabIndex = 12;
            labelPosition.Text = "Position:";
            //
            // numPosition
            //
            numPosition.BackColor = Color.White;
            numPosition.Location = new Point(320, 118);
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
            labelPositionMax.ForeColor = SystemColors.GrayText;
            labelPositionMax.Location = new Point(426, 121);
            labelPositionMax.Name = "labelPositionMax";
            labelPositionMax.Size = new Size(36, 20);
            labelPositionMax.TabIndex = 13;
            labelPositionMax.Text = "/ 1";
            //
            // label2
            //
            label2.AutoSize = true;
            label2.ForeColor = Color.FromArgb(20, 28, 48);
            label2.Location = new Point(12, 160);
            label2.Name = "label2";
            label2.Size = new Size(65, 20);
            label2.TabIndex = 14;
            label2.Text = "Path/Url:";
            //
            // textBoxPathOrUrl
            //
            textBoxPathOrUrl.BackColor = Color.White;
            textBoxPathOrUrl.Location = new Point(100, 157);
            textBoxPathOrUrl.Name = "textBoxPathOrUrl";
            textBoxPathOrUrl.Size = new Size(562, 27);
            textBoxPathOrUrl.TabIndex = 3;
            //
            // buttonBrowse
            //
            buttonBrowse.Location = new Point(668, 156);
            buttonBrowse.Name = "buttonBrowse";
            buttonBrowse.Size = new Size(44, 29);
            buttonBrowse.TabIndex = 4;
            buttonBrowse.Text = "...";
            buttonBrowse.UseVisualStyleBackColor = true;
            //
            // buttonOk
            //
            buttonOk.Location = new Point(518, 200);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(94, 40);
            buttonOk.TabIndex = 5;
            buttonOk.Text = "Save";
            buttonOk.BackColor = Color.FromArgb(0, 52, 160);
            buttonOk.FlatAppearance.BorderSize = 0;
            buttonOk.FlatAppearance.MouseOverBackColor = Color.FromArgb(24, 118, 232);
            buttonOk.FlatStyle = FlatStyle.Flat;
            buttonOk.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            buttonOk.ForeColor = Color.White;
            buttonOk.UseVisualStyleBackColor = false;
            //
            // buttonCancel
            //
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(618, 200);
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
            BackColor = Color.FromArgb(236, 239, 244);
            CancelButton = buttonCancel;
            ClientSize = new Size(732, 254);
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
            Controls.Add(pnlAccent);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EditActionForm";
            ShowInTaskbar = false;
            Text = "Edit";
            ((System.ComponentModel.ISupportInitialize)numPosition).EndInit();
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
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
        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Panel pnlAccent;
    }
}
