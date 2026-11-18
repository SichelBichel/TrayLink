namespace TrayLink
{
    partial class LayoutManagerForm
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
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            labelLayouts = new Label();
            listLayouts = new ListBox();
            buttonSaveNew = new Button();
            buttonRename = new Button();
            buttonDelete = new Button();
            labelInfo = new Label();
            grid = new DataGridView();
            colInclude = new DataGridViewCheckBoxColumn();
            colApp = new DataGridViewTextBoxColumn();
            colTitle = new DataGridViewTextBoxColumn();
            colMonitor = new DataGridViewTextBoxColumn();
            colState = new DataGridViewTextBoxColumn();
            colStart = new DataGridViewComboBoxColumn();
            colStartWith = new DataGridViewTextBoxColumn();
            colDelay = new DataGridViewTextBoxColumn();
            chkAutoRestore = new CheckBox();
            labelHint = new Label();
            buttonUpdate = new Button();
            buttonRestore = new Button();
            buttonClose = new Button();
            toolTip = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            SuspendLayout();
            //
            // labelLayouts
            //
            labelLayouts.AutoSize = true;
            labelLayouts.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelLayouts.Location = new Point(12, 12);
            labelLayouts.Name = "labelLayouts";
            labelLayouts.Size = new Size(66, 20);
            labelLayouts.TabIndex = 0;
            labelLayouts.Text = "Layouts";
            //
            // listLayouts
            //
            listLayouts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listLayouts.FormattingEnabled = true;
            listLayouts.IntegralHeight = false;
            listLayouts.Location = new Point(12, 36);
            listLayouts.Name = "listLayouts";
            listLayouts.Size = new Size(220, 420);
            listLayouts.TabIndex = 1;
            //
            // buttonSaveNew
            //
            buttonSaveNew.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonSaveNew.Location = new Point(12, 464);
            buttonSaveNew.Name = "buttonSaveNew";
            buttonSaveNew.Size = new Size(220, 32);
            buttonSaveNew.TabIndex = 2;
            buttonSaveNew.Text = "Save current desktop...";
            buttonSaveNew.UseVisualStyleBackColor = true;
            toolTip.SetToolTip(buttonSaveNew, "Saves every open window with its monitor, position and how to start it");
            //
            // buttonRename
            //
            buttonRename.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonRename.Location = new Point(12, 502);
            buttonRename.Name = "buttonRename";
            buttonRename.Size = new Size(106, 32);
            buttonRename.TabIndex = 3;
            buttonRename.Text = "Rename...";
            buttonRename.UseVisualStyleBackColor = true;
            //
            // buttonDelete
            //
            buttonDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonDelete.Location = new Point(126, 502);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Size = new Size(106, 32);
            buttonDelete.TabIndex = 4;
            buttonDelete.Text = "Delete";
            buttonDelete.UseVisualStyleBackColor = true;
            //
            // labelInfo
            //
            labelInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelInfo.AutoEllipsis = true;
            labelInfo.Location = new Point(248, 12);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(740, 20);
            labelInfo.TabIndex = 5;
            labelInfo.Text = "No layout selected";
            //
            // grid
            //
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = SystemColors.Window;
            headerStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            headerStyle.WrapMode = DataGridViewTriState.False;
            grid.ColumnHeadersDefaultCellStyle = headerStyle;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.Columns.AddRange(new DataGridViewColumn[] { colInclude, colApp, colTitle, colMonitor, colState, colStart, colStartWith, colDelay });
            grid.EditMode = DataGridViewEditMode.EditOnEnter;
            grid.Location = new Point(248, 36);
            grid.MultiSelect = false;
            grid.Name = "grid";
            grid.RowHeadersVisible = false;
            grid.RowHeadersWidth = 51;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Size = new Size(740, 420);
            grid.TabIndex = 6;
            //
            // colInclude
            //
            colInclude.FillWeight = 30F;
            colInclude.HeaderText = "Use";
            colInclude.MinimumWidth = 40;
            colInclude.Name = "colInclude";
            colInclude.ToolTipText = "Restore this window";
            //
            // colApp
            //
            colApp.FillWeight = 80F;
            colApp.HeaderText = "App";
            colApp.MinimumWidth = 60;
            colApp.Name = "colApp";
            colApp.ReadOnly = true;
            //
            // colTitle
            //
            colTitle.FillWeight = 140F;
            colTitle.HeaderText = "Window";
            colTitle.MinimumWidth = 80;
            colTitle.Name = "colTitle";
            colTitle.ReadOnly = true;
            //
            // colMonitor
            //
            colMonitor.FillWeight = 60F;
            colMonitor.HeaderText = "Monitor";
            colMonitor.MinimumWidth = 60;
            colMonitor.Name = "colMonitor";
            colMonitor.ReadOnly = true;
            //
            // colState
            //
            colState.FillWeight = 55F;
            colState.HeaderText = "State";
            colState.MinimumWidth = 55;
            colState.Name = "colState";
            colState.ReadOnly = true;
            //
            // colStart
            //
            colStart.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            colStart.FillWeight = 70F;
            colStart.HeaderText = "Start";
            colStart.Items.AddRange(new object[] { "Auto", "Position only" });
            colStart.MinimumWidth = 90;
            colStart.Name = "colStart";
            colStart.ToolTipText = "Auto: start the app when it is not open. Position only: never start, only place it.";
            //
            // colStartWith
            //
            colStartWith.FillWeight = 130F;
            colStartWith.HeaderText = "Start with";
            colStartWith.MinimumWidth = 80;
            colStartWith.Name = "colStartWith";
            colStartWith.ToolTipText = "Arguments of the program, folder of an Explorer window";
            //
            // colDelay
            //
            colDelay.FillWeight = 40F;
            colDelay.HeaderText = "Delay s";
            colDelay.MinimumWidth = 50;
            colDelay.Name = "colDelay";
            colDelay.ToolTipText = "Seconds after the restore started before this app is started";
            //
            // chkAutoRestore
            //
            chkAutoRestore.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkAutoRestore.AutoSize = true;
            chkAutoRestore.Location = new Point(248, 469);
            chkAutoRestore.Name = "chkAutoRestore";
            chkAutoRestore.Size = new Size(300, 24);
            chkAutoRestore.TabIndex = 7;
            chkAutoRestore.Text = "Restore this layout when Windows starts";
            chkAutoRestore.UseVisualStyleBackColor = true;
            toolTip.SetToolTip(chkAutoRestore, "Needs \"Start with Windows\" (tray menu). Runs once per Windows sign in.");
            //
            // labelHint
            //
            labelHint.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelHint.AutoEllipsis = true;
            labelHint.ForeColor = SystemColors.GrayText;
            labelHint.Location = new Point(248, 508);
            labelHint.Name = "labelHint";
            labelHint.Size = new Size(440, 20);
            labelHint.TabIndex = 8;
            labelHint.Text = "Grey rows can not be started: they are only placed when they are open.";
            //
            // buttonUpdate
            //
            buttonUpdate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonUpdate.Location = new Point(694, 464);
            buttonUpdate.Name = "buttonUpdate";
            buttonUpdate.Size = new Size(166, 32);
            buttonUpdate.TabIndex = 9;
            buttonUpdate.Text = "Update from desktop";
            buttonUpdate.UseVisualStyleBackColor = true;
            toolTip.SetToolTip(buttonUpdate, "Takes the open windows again and keeps Use, Start and Delay of the known ones");
            //
            // buttonRestore
            //
            buttonRestore.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonRestore.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            buttonRestore.Location = new Point(868, 464);
            buttonRestore.Name = "buttonRestore";
            buttonRestore.Size = new Size(120, 32);
            buttonRestore.TabIndex = 10;
            buttonRestore.Text = "Restore now";
            buttonRestore.UseVisualStyleBackColor = true;
            //
            // buttonClose
            //
            buttonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonClose.DialogResult = DialogResult.Cancel;
            buttonClose.Location = new Point(868, 502);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(120, 32);
            buttonClose.TabIndex = 11;
            buttonClose.Text = "Close";
            buttonClose.UseVisualStyleBackColor = true;
            //
            // LayoutManagerForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonClose;
            ClientSize = new Size(1000, 546);
            Controls.Add(buttonClose);
            Controls.Add(buttonRestore);
            Controls.Add(buttonUpdate);
            Controls.Add(labelHint);
            Controls.Add(chkAutoRestore);
            Controls.Add(grid);
            Controls.Add(labelInfo);
            Controls.Add(buttonDelete);
            Controls.Add(buttonRename);
            Controls.Add(buttonSaveNew);
            Controls.Add(listLayouts);
            Controls.Add(labelLayouts);
            MinimumSize = new Size(860, 420);
            Name = "LayoutManagerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TrayLink: Desktop layouts";
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelLayouts;
        private ListBox listLayouts;
        private Button buttonSaveNew;
        private Button buttonRename;
        private Button buttonDelete;
        private Label labelInfo;
        private DataGridView grid;
        private DataGridViewCheckBoxColumn colInclude;
        private DataGridViewTextBoxColumn colApp;
        private DataGridViewTextBoxColumn colTitle;
        private DataGridViewTextBoxColumn colMonitor;
        private DataGridViewTextBoxColumn colState;
        private DataGridViewComboBoxColumn colStart;
        private DataGridViewTextBoxColumn colStartWith;
        private DataGridViewTextBoxColumn colDelay;
        private CheckBox chkAutoRestore;
        private Label labelHint;
        private Button buttonUpdate;
        private Button buttonRestore;
        private Button buttonClose;
        private ToolTip toolTip;
    }
}
