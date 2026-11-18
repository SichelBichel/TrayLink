namespace TrayLink
{
    partial class LayoutCard
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            labelName = new Label();
            labelInfo = new Label();
            labelAuto = new Label();
            buttonRestore = new Button();
            buttonUpdate = new Button();
            buttonEdit = new Button();
            toolTip = new ToolTip(components);
            SuspendLayout();
            //
            // labelName
            //
            labelName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelName.AutoEllipsis = true;
            labelName.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            labelName.ForeColor = Color.FromArgb(20, 28, 48);
            labelName.Location = new Point(10, 10);
            labelName.Name = "labelName";
            labelName.Size = new Size(196, 24);
            labelName.TabIndex = 0;
            labelName.Text = "LAYOUT";
            labelName.UseMnemonic = false;
            //
            // labelInfo
            //
            labelInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelInfo.AutoEllipsis = true;
            labelInfo.Font = new Font("Segoe UI", 8F);
            labelInfo.ForeColor = SystemColors.GrayText;
            labelInfo.Location = new Point(11, 36);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(195, 19);
            labelInfo.TabIndex = 1;
            labelInfo.Text = "9 windows, 4 monitors";
            //
            // labelAuto
            //
            labelAuto.AutoSize = true;
            labelAuto.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            labelAuto.ForeColor = Color.FromArgb(14, 70, 170);
            labelAuto.Location = new Point(11, 64);
            labelAuto.Name = "labelAuto";
            labelAuto.Size = new Size(101, 19);
            labelAuto.TabIndex = 2;
            labelAuto.Text = "At Windows start";
            //
            // buttonRestore
            //
            buttonRestore.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonRestore.BackColor = Color.FromArgb(0, 52, 160);
            buttonRestore.Cursor = Cursors.Hand;
            buttonRestore.FlatAppearance.BorderSize = 0;
            buttonRestore.FlatAppearance.MouseOverBackColor = Color.FromArgb(24, 118, 232);
            buttonRestore.FlatStyle = FlatStyle.Flat;
            buttonRestore.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            buttonRestore.ForeColor = Color.White;
            buttonRestore.Location = new Point(212, 10);
            buttonRestore.Name = "buttonRestore";
            buttonRestore.Size = new Size(88, 34);
            buttonRestore.TabIndex = 3;
            buttonRestore.Text = "Restore";
            toolTip.SetToolTip(buttonRestore, "Start and place the windows of this layout");
            buttonRestore.UseVisualStyleBackColor = false;
            buttonRestore.Click += buttonRestore_Click;
            //
            // buttonUpdate
            //
            buttonUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonUpdate.BackColor = Color.White;
            buttonUpdate.Cursor = Cursors.Hand;
            buttonUpdate.FlatAppearance.BorderColor = Color.FromArgb(206, 213, 226);
            buttonUpdate.FlatAppearance.MouseOverBackColor = Color.FromArgb(226, 235, 250);
            buttonUpdate.FlatStyle = FlatStyle.Flat;
            buttonUpdate.Font = new Font("Segoe UI", 8F);
            buttonUpdate.ForeColor = Color.FromArgb(14, 70, 170);
            buttonUpdate.Location = new Point(162, 58);
            buttonUpdate.Name = "buttonUpdate";
            buttonUpdate.Size = new Size(66, 28);
            buttonUpdate.TabIndex = 4;
            buttonUpdate.Text = "Update";
            toolTip.SetToolTip(buttonUpdate, "Take the windows that are open now for this layout");
            buttonUpdate.UseVisualStyleBackColor = false;
            buttonUpdate.Click += buttonUpdate_Click;
            //
            // buttonEdit
            //
            buttonEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonEdit.BackColor = Color.White;
            buttonEdit.Cursor = Cursors.Hand;
            buttonEdit.FlatAppearance.BorderColor = Color.FromArgb(206, 213, 226);
            buttonEdit.FlatAppearance.MouseOverBackColor = Color.FromArgb(226, 235, 250);
            buttonEdit.FlatStyle = FlatStyle.Flat;
            buttonEdit.Font = new Font("Segoe UI", 8F);
            buttonEdit.ForeColor = Color.FromArgb(14, 70, 170);
            buttonEdit.Location = new Point(234, 58);
            buttonEdit.Name = "buttonEdit";
            buttonEdit.Size = new Size(66, 28);
            buttonEdit.TabIndex = 5;
            buttonEdit.Text = "Edit";
            toolTip.SetToolTip(buttonEdit, "Windows of this layout, start mode, arguments, delay");
            buttonEdit.UseVisualStyleBackColor = false;
            buttonEdit.Click += buttonEdit_Click;
            //
            // LayoutCard
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(buttonEdit);
            Controls.Add(buttonUpdate);
            Controls.Add(buttonRestore);
            Controls.Add(labelAuto);
            Controls.Add(labelInfo);
            Controls.Add(labelName);
            Margin = new Padding(3, 3, 3, 6);
            Name = "LayoutCard";
            Size = new Size(310, 96);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelName;
        private Label labelInfo;
        private Label labelAuto;
        private Button buttonRestore;
        private Button buttonUpdate;
        private Button buttonEdit;
        private ToolTip toolTip;
    }
}
