namespace TrayLink
{
    partial class TrayPopupForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Dispose(bool) lives in TrayPopupForm.cs (also disposes the file watcher).

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrayPopupForm));
            pnlHeader = new Panel();
            picIcon = new PictureBox();
            lblTitle = new Label();
            lblVersion = new Label();
            stripHeader = new ToolStrip();
            ddOptions = new ToolStripDropDownButton();
            menuAddEntry = new ToolStripMenuItem();
            menuOptionsSep1 = new ToolStripSeparator();
            menuOpenConfig = new ToolStripMenuItem();
            menuReloadConfig = new ToolStripMenuItem();
            menuOptionsSep2 = new ToolStripSeparator();
            menuAutoHide = new ToolStripMenuItem();
            menuShellIntegration = new ToolStripMenuItem();
            menuStartWithWindows = new ToolStripMenuItem();
            menuAppearance = new ToolStripMenuItem();
            menuThemeSystem = new ToolStripMenuItem();
            menuThemeLight = new ToolStripMenuItem();
            menuThemeDark = new ToolStripMenuItem();
            menuOptionsSep3 = new ToolStripSeparator();
            menuCheckUpdates = new ToolStripMenuItem();
            stripTabs = new ToolStrip();
            tabLinks = new ToolStripButton();
            tabDesktops = new ToolStripButton();
            pnlAccent = new Panel();
            pnlLinks = new Panel();
            actionPanel = new FlowLayoutPanel();
            pnlSearch = new Panel();
            searchBar = new TextBox();
            buttonAdd = new Button();
            pnlDesktops = new Panel();
            layoutPanel = new FlowLayoutPanel();
            lblNoLayouts = new Label();
            pnlDesktopsTop = new Panel();
            buttonSaveLayout = new Button();
            pnlDesktopsBottom = new Panel();
            buttonManageLayouts = new Button();
            statusMain = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            lblLogo = new ToolStripStatusLabel();
            toolTip = new ToolTip(components);
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picIcon).BeginInit();
            stripHeader.SuspendLayout();
            stripTabs.SuspendLayout();
            pnlLinks.SuspendLayout();
            pnlSearch.SuspendLayout();
            pnlDesktops.SuspendLayout();
            pnlDesktopsTop.SuspendLayout();
            pnlDesktopsBottom.SuspendLayout();
            statusMain.SuspendLayout();
            SuspendLayout();
            //
            // pnlHeader
            //
            pnlHeader.BackColor = Color.FromArgb(20, 28, 48);
            pnlHeader.Controls.Add(stripHeader);
            pnlHeader.Controls.Add(lblVersion);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(picIcon);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(340, 60);
            pnlHeader.TabIndex = 0;
            //
            // picIcon
            //
            picIcon.Image = Properties.Resources.TrayLink1;
            picIcon.Location = new Point(12, 12);
            picIcon.Name = "picIcon";
            picIcon.Size = new Size(36, 36);
            picIcon.SizeMode = PictureBoxSizeMode.Zoom;
            picIcon.TabIndex = 0;
            picIcon.TabStop = false;
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(238, 242, 250);
            lblTitle.Location = new Point(54, 6);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(91, 30);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "TrayLink";
            //
            // lblVersion
            //
            lblVersion.AutoSize = true;
            lblVersion.Font = new Font("Segoe UI", 8F);
            lblVersion.ForeColor = Color.FromArgb(158, 172, 198);
            lblVersion.Location = new Point(57, 36);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(46, 19);
            lblVersion.TabIndex = 2;
            lblVersion.Text = "V 1.7.2";
            //
            // stripHeader
            //
            stripHeader.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            stripHeader.BackColor = Color.FromArgb(20, 28, 48);
            stripHeader.Dock = DockStyle.None;
            stripHeader.GripStyle = ToolStripGripStyle.Hidden;
            stripHeader.ImageScalingSize = new Size(20, 20);
            stripHeader.Items.AddRange(new ToolStripItem[] { ddOptions });
            stripHeader.Location = new Point(236, 16);
            stripHeader.Name = "stripHeader";
            stripHeader.Size = new Size(96, 27);
            stripHeader.TabIndex = 3;
            //
            // ddOptions
            //
            ddOptions.DisplayStyle = ToolStripItemDisplayStyle.Text;
            ddOptions.DropDownDirection = ToolStripDropDownDirection.BelowLeft;
            ddOptions.DropDownItems.AddRange(new ToolStripItem[] { menuAddEntry, menuOptionsSep1, menuOpenConfig, menuReloadConfig, menuOptionsSep2, menuAutoHide, menuShellIntegration, menuStartWithWindows, menuAppearance, menuOptionsSep3, menuCheckUpdates });
            ddOptions.ForeColor = Color.FromArgb(238, 242, 250);
            ddOptions.Name = "ddOptions";
            ddOptions.Padding = new Padding(6, 0, 6, 0);
            ddOptions.Size = new Size(85, 24);
            ddOptions.Text = "Options";
            ddOptions.DropDownOpening += ddOptions_DropDownOpening;
            //
            // menuAddEntry
            //
            menuAddEntry.Name = "menuAddEntry";
            menuAddEntry.Size = new Size(330, 26);
            menuAddEntry.Text = "Add link...";
            menuAddEntry.Click += inputAddEntry;
            //
            // menuOptionsSep1
            //
            menuOptionsSep1.Name = "menuOptionsSep1";
            menuOptionsSep1.Size = new Size(327, 6);
            //
            // menuOpenConfig
            //
            menuOpenConfig.Name = "menuOpenConfig";
            menuOpenConfig.Size = new Size(330, 26);
            menuOpenConfig.Text = "Open actions.ini";
            menuOpenConfig.Click += inputOpenConfig;
            //
            // menuReloadConfig
            //
            menuReloadConfig.Name = "menuReloadConfig";
            menuReloadConfig.Size = new Size(330, 26);
            menuReloadConfig.Text = "Reload actions.ini";
            menuReloadConfig.Click += inputReloadConfig;
            //
            // menuOptionsSep2
            //
            menuOptionsSep2.Name = "menuOptionsSep2";
            menuOptionsSep2.Size = new Size(327, 6);
            //
            // menuAutoHide
            //
            menuAutoHide.Name = "menuAutoHide";
            menuAutoHide.Size = new Size(330, 26);
            menuAutoHide.Text = "Close on focus loss";
            menuAutoHide.Click += menuAutoHide_Click;
            //
            // menuShellIntegration
            //
            menuShellIntegration.Name = "menuShellIntegration";
            menuShellIntegration.Size = new Size(330, 26);
            menuShellIntegration.Text = "\"Add to TrayLink\" in the Explorer menu";
            menuShellIntegration.Click += inputShellInteg;
            //
            // menuStartWithWindows
            //
            menuStartWithWindows.Name = "menuStartWithWindows";
            menuStartWithWindows.Size = new Size(330, 26);
            menuStartWithWindows.Text = "Start with Windows";
            menuStartWithWindows.Click += menuStartWithWindows_Click;
            //
            // menuAppearance
            //
            menuAppearance.DropDownDirection = ToolStripDropDownDirection.Left;
            menuAppearance.DropDownItems.AddRange(new ToolStripItem[] { menuThemeSystem, menuThemeLight, menuThemeDark });
            menuAppearance.Name = "menuAppearance";
            menuAppearance.Size = new Size(330, 26);
            menuAppearance.Text = "Appearance";
            //
            // menuThemeSystem
            //
            menuThemeSystem.Name = "menuThemeSystem";
            menuThemeSystem.Size = new Size(230, 26);
            menuThemeSystem.Tag = "System";
            menuThemeSystem.Text = "Like Windows";
            menuThemeSystem.Click += menuTheme_Click;
            //
            // menuThemeLight
            //
            menuThemeLight.Name = "menuThemeLight";
            menuThemeLight.Size = new Size(230, 26);
            menuThemeLight.Tag = "Light";
            menuThemeLight.Text = "Light";
            menuThemeLight.Click += menuTheme_Click;
            //
            // menuThemeDark
            //
            menuThemeDark.Name = "menuThemeDark";
            menuThemeDark.Size = new Size(230, 26);
            menuThemeDark.Tag = "Dark";
            menuThemeDark.Text = "Dark";
            menuThemeDark.Click += menuTheme_Click;
            //
            // menuOptionsSep3
            //
            menuOptionsSep3.Name = "menuOptionsSep3";
            menuOptionsSep3.Size = new Size(327, 6);
            //
            // menuCheckUpdates
            //
            menuCheckUpdates.Name = "menuCheckUpdates";
            menuCheckUpdates.Size = new Size(330, 26);
            menuCheckUpdates.Text = "Check for updates";
            menuCheckUpdates.Click += inputUpdate;
            //
            // stripTabs
            //
            stripTabs.AutoSize = false;
            stripTabs.BackColor = Color.FromArgb(12, 16, 28);
            stripTabs.CanOverflow = false;
            stripTabs.Dock = DockStyle.Top;
            stripTabs.GripStyle = ToolStripGripStyle.Hidden;
            stripTabs.ImageScalingSize = new Size(20, 20);
            stripTabs.Items.AddRange(new ToolStripItem[] { tabLinks, tabDesktops });
            stripTabs.Location = new Point(0, 60);
            stripTabs.Name = "stripTabs";
            stripTabs.Padding = new Padding(0);
            stripTabs.Size = new Size(340, 38);
            stripTabs.TabIndex = 1;
            stripTabs.Tag = "tabs";
            stripTabs.Resize += stripTabs_Resize;
            //
            // tabLinks
            //
            tabLinks.AutoSize = false;
            tabLinks.Checked = true;
            tabLinks.CheckState = CheckState.Checked;
            tabLinks.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tabLinks.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            tabLinks.ForeColor = Color.FromArgb(238, 242, 250);
            tabLinks.Margin = new Padding(0);
            tabLinks.Name = "tabLinks";
            tabLinks.Size = new Size(170, 38);
            tabLinks.Text = "Links";
            tabLinks.Click += tabLinks_Click;
            //
            // tabDesktops
            //
            tabDesktops.AutoSize = false;
            tabDesktops.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tabDesktops.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            tabDesktops.ForeColor = Color.FromArgb(238, 242, 250);
            tabDesktops.Margin = new Padding(0);
            tabDesktops.Name = "tabDesktops";
            tabDesktops.Size = new Size(170, 38);
            tabDesktops.Text = "Desktops";
            tabDesktops.Click += tabDesktops_Click;
            //
            // pnlAccent
            //
            pnlAccent.Dock = DockStyle.Top;
            pnlAccent.Location = new Point(0, 98);
            pnlAccent.Name = "pnlAccent";
            pnlAccent.Size = new Size(340, 3);
            pnlAccent.TabIndex = 2;
            pnlAccent.Paint += pnlAccent_Paint;
            pnlAccent.Resize += pnlAccent_Resize;
            //
            // pnlLinks
            //
            pnlLinks.BackColor = Color.FromArgb(236, 239, 244);
            pnlLinks.Controls.Add(actionPanel);
            pnlLinks.Controls.Add(pnlSearch);
            pnlLinks.Dock = DockStyle.Fill;
            pnlLinks.Location = new Point(0, 101);
            pnlLinks.Name = "pnlLinks";
            pnlLinks.Padding = new Padding(6, 0, 2, 4);
            pnlLinks.Size = new Size(340, 633);
            pnlLinks.TabIndex = 3;
            //
            // actionPanel
            //
            actionPanel.AllowDrop = true;
            actionPanel.AutoScroll = true;
            actionPanel.BackColor = Color.FromArgb(236, 239, 244);
            actionPanel.Dock = DockStyle.Fill;
            actionPanel.FlowDirection = FlowDirection.TopDown;
            actionPanel.Location = new Point(6, 44);
            actionPanel.Name = "actionPanel";
            actionPanel.Size = new Size(332, 585);
            actionPanel.TabIndex = 1;
            actionPanel.WrapContents = false;
            //
            // pnlSearch
            //
            pnlSearch.Controls.Add(buttonAdd);
            pnlSearch.Controls.Add(searchBar);
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Location = new Point(6, 0);
            pnlSearch.Name = "pnlSearch";
            pnlSearch.Size = new Size(332, 44);
            pnlSearch.TabIndex = 0;
            //
            // searchBar
            //
            searchBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            searchBar.BorderStyle = BorderStyle.FixedSingle;
            searchBar.Location = new Point(3, 9);
            searchBar.Name = "searchBar";
            searchBar.Size = new Size(280, 27);
            searchBar.TabIndex = 0;
            searchBar.TextChanged += inputSearchQuery;
            //
            // buttonAdd
            //
            buttonAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonAdd.BackColor = Color.FromArgb(0, 52, 160);
            buttonAdd.Cursor = Cursors.Hand;
            buttonAdd.FlatAppearance.BorderSize = 0;
            buttonAdd.FlatAppearance.MouseOverBackColor = Color.FromArgb(24, 118, 232);
            buttonAdd.FlatStyle = FlatStyle.Flat;
            buttonAdd.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            buttonAdd.ForeColor = Color.White;
            buttonAdd.Location = new Point(289, 8);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new Size(36, 29);
            buttonAdd.TabIndex = 1;
            buttonAdd.Text = "+";
            toolTip.SetToolTip(buttonAdd, "Add link (or drop files and folders into the list)");
            buttonAdd.UseVisualStyleBackColor = false;
            buttonAdd.Click += inputAddEntry;
            //
            // pnlDesktops
            //
            pnlDesktops.BackColor = Color.FromArgb(236, 239, 244);
            pnlDesktops.Controls.Add(layoutPanel);
            pnlDesktops.Controls.Add(lblNoLayouts);
            pnlDesktops.Controls.Add(pnlDesktopsBottom);
            pnlDesktops.Controls.Add(pnlDesktopsTop);
            pnlDesktops.Dock = DockStyle.Fill;
            pnlDesktops.Location = new Point(0, 101);
            pnlDesktops.Name = "pnlDesktops";
            pnlDesktops.Padding = new Padding(6, 0, 2, 0);
            pnlDesktops.Size = new Size(340, 633);
            pnlDesktops.TabIndex = 4;
            pnlDesktops.Visible = false;
            //
            // layoutPanel
            //
            layoutPanel.AutoScroll = true;
            layoutPanel.BackColor = Color.FromArgb(236, 239, 244);
            layoutPanel.Dock = DockStyle.Fill;
            layoutPanel.FlowDirection = FlowDirection.TopDown;
            layoutPanel.Location = new Point(6, 110);
            layoutPanel.Name = "layoutPanel";
            layoutPanel.Size = new Size(332, 479);
            layoutPanel.TabIndex = 2;
            layoutPanel.WrapContents = false;
            //
            // lblNoLayouts
            //
            lblNoLayouts.Dock = DockStyle.Top;
            lblNoLayouts.ForeColor = SystemColors.GrayText;
            lblNoLayouts.Location = new Point(6, 46);
            lblNoLayouts.Name = "lblNoLayouts";
            lblNoLayouts.Padding = new Padding(4, 6, 4, 0);
            lblNoLayouts.Size = new Size(332, 64);
            lblNoLayouts.TabIndex = 1;
            lblNoLayouts.Text = "No desktop layout yet. Arrange your windows on the monitors, then \"Save current desktop\".";
            //
            // pnlDesktopsTop
            //
            pnlDesktopsTop.Controls.Add(buttonSaveLayout);
            pnlDesktopsTop.Dock = DockStyle.Top;
            pnlDesktopsTop.Location = new Point(6, 0);
            pnlDesktopsTop.Name = "pnlDesktopsTop";
            pnlDesktopsTop.Size = new Size(332, 46);
            pnlDesktopsTop.TabIndex = 0;
            //
            // buttonSaveLayout
            //
            buttonSaveLayout.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            buttonSaveLayout.BackColor = Color.FromArgb(0, 52, 160);
            buttonSaveLayout.Cursor = Cursors.Hand;
            buttonSaveLayout.FlatAppearance.BorderSize = 0;
            buttonSaveLayout.FlatAppearance.MouseOverBackColor = Color.FromArgb(24, 118, 232);
            buttonSaveLayout.FlatStyle = FlatStyle.Flat;
            buttonSaveLayout.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            buttonSaveLayout.ForeColor = Color.White;
            buttonSaveLayout.Location = new Point(3, 8);
            buttonSaveLayout.Name = "buttonSaveLayout";
            buttonSaveLayout.Size = new Size(322, 32);
            buttonSaveLayout.TabIndex = 0;
            buttonSaveLayout.Text = "Save current desktop...";
            toolTip.SetToolTip(buttonSaveLayout, "Saves every open window with its monitor, position and how to start it");
            buttonSaveLayout.UseVisualStyleBackColor = false;
            buttonSaveLayout.Click += buttonSaveLayout_Click;
            //
            // pnlDesktopsBottom
            //
            pnlDesktopsBottom.Controls.Add(buttonManageLayouts);
            pnlDesktopsBottom.Dock = DockStyle.Bottom;
            pnlDesktopsBottom.Location = new Point(6, 589);
            pnlDesktopsBottom.Name = "pnlDesktopsBottom";
            pnlDesktopsBottom.Size = new Size(332, 44);
            pnlDesktopsBottom.TabIndex = 3;
            //
            // buttonManageLayouts
            //
            buttonManageLayouts.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            buttonManageLayouts.BackColor = Color.White;
            buttonManageLayouts.Cursor = Cursors.Hand;
            buttonManageLayouts.FlatAppearance.BorderColor = Color.FromArgb(206, 213, 226);
            buttonManageLayouts.FlatAppearance.MouseOverBackColor = Color.FromArgb(226, 235, 250);
            buttonManageLayouts.FlatStyle = FlatStyle.Flat;
            buttonManageLayouts.ForeColor = Color.FromArgb(14, 70, 170);
            buttonManageLayouts.Location = new Point(3, 4);
            buttonManageLayouts.Name = "buttonManageLayouts";
            buttonManageLayouts.Size = new Size(322, 32);
            buttonManageLayouts.TabIndex = 0;
            buttonManageLayouts.Text = "Manage layouts...";
            toolTip.SetToolTip(buttonManageLayouts, "Windows of a layout: use, start mode, arguments, delay, restore at Windows start");
            buttonManageLayouts.UseVisualStyleBackColor = false;
            buttonManageLayouts.Click += inputLayouts;
            //
            // statusMain
            //
            statusMain.BackColor = Color.FromArgb(12, 16, 28);
            statusMain.ImageScalingSize = new Size(20, 20);
            statusMain.Items.AddRange(new ToolStripItem[] { lblStatus, lblLogo });
            statusMain.Location = new Point(0, 734);
            statusMain.Name = "statusMain";
            statusMain.Size = new Size(340, 26);
            statusMain.SizingGrip = false;
            statusMain.TabIndex = 5;
            //
            // lblStatus
            //
            lblStatus.ForeColor = Color.FromArgb(238, 242, 250);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(240, 20);
            lblStatus.Spring = true;
            lblStatus.Text = "0 links";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblLogo
            //
            lblLogo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            lblLogo.ImageScaling = ToolStripItemImageScaling.None;
            lblLogo.Name = "lblLogo";
            lblLogo.Size = new Size(85, 20);
            lblLogo.ToolTipText = "ReHoGa Interactive";
            //
            // TrayPopupForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(236, 239, 244);
            ClientSize = new Size(340, 760);
            Controls.Add(pnlLinks);
            Controls.Add(pnlDesktops);
            Controls.Add(pnlAccent);
            Controls.Add(stripTabs);
            Controls.Add(pnlHeader);
            Controls.Add(statusMain);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "TrayPopupForm";
            Text = "TrayLink";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picIcon).EndInit();
            stripHeader.ResumeLayout(false);
            stripHeader.PerformLayout();
            stripTabs.ResumeLayout(false);
            stripTabs.PerformLayout();
            pnlLinks.ResumeLayout(false);
            pnlSearch.ResumeLayout(false);
            pnlSearch.PerformLayout();
            pnlDesktops.ResumeLayout(false);
            pnlDesktopsTop.ResumeLayout(false);
            pnlDesktopsBottom.ResumeLayout(false);
            statusMain.ResumeLayout(false);
            statusMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlHeader;
        private PictureBox picIcon;
        private Label lblTitle;
        private Label lblVersion;
        private ToolStrip stripHeader;
        private ToolStripDropDownButton ddOptions;
        private ToolStripMenuItem menuAddEntry;
        private ToolStripSeparator menuOptionsSep1;
        private ToolStripMenuItem menuOpenConfig;
        private ToolStripMenuItem menuReloadConfig;
        private ToolStripSeparator menuOptionsSep2;
        private ToolStripMenuItem menuAutoHide;
        private ToolStripMenuItem menuShellIntegration;
        private ToolStripMenuItem menuStartWithWindows;
        private ToolStripMenuItem menuAppearance;
        private ToolStripMenuItem menuThemeSystem;
        private ToolStripMenuItem menuThemeLight;
        private ToolStripMenuItem menuThemeDark;
        private ToolStripSeparator menuOptionsSep3;
        private ToolStripMenuItem menuCheckUpdates;
        private ToolStrip stripTabs;
        private ToolStripButton tabLinks;
        private ToolStripButton tabDesktops;
        private Panel pnlAccent;
        private Panel pnlLinks;
        private FlowLayoutPanel actionPanel;
        private Panel pnlSearch;
        private TextBox searchBar;
        private Button buttonAdd;
        private Panel pnlDesktops;
        private FlowLayoutPanel layoutPanel;
        private Label lblNoLayouts;
        private Panel pnlDesktopsTop;
        private Button buttonSaveLayout;
        private Panel pnlDesktopsBottom;
        private Button buttonManageLayouts;
        private StatusStrip statusMain;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblLogo;
        private ToolTip toolTip;
    }
}
