namespace TrayLink
{
    partial class TrayPopupForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrayPopupForm));
            TrayIcon = new NotifyIcon(components);
            actionPanel = new FlowLayoutPanel();
            searchBar = new TextBox();
            label1 = new Label();
            buttonReloadCFG = new Button();
            buttonOpenCFG = new Button();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            label3 = new Label();
            linkLabel1 = new LinkLabel();
            button1 = new Button();
            chkAutoHide = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // TrayIcon
            // 
            TrayIcon.Text = "TrayLink";
            TrayIcon.Visible = true;
            // 
            // actionPanel
            // 
            actionPanel.BorderStyle = BorderStyle.FixedSingle;
            actionPanel.Location = new Point(10, 197);
            actionPanel.Margin = new Padding(3, 2, 3, 2);
            actionPanel.Name = "actionPanel";
            actionPanel.Size = new Size(261, 397);
            actionPanel.TabIndex = 0;
            // 
            // searchBar
            // 
            searchBar.Location = new Point(10, 159);
            searchBar.Margin = new Padding(3, 2, 3, 2);
            searchBar.Name = "searchBar";
            searchBar.Size = new Size(261, 23);
            searchBar.TabIndex = 1;
            searchBar.TextChanged += inputSearchQuery;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 141);
            label1.Name = "label1";
            label1.Size = new Size(45, 15);
            label1.TabIndex = 2;
            label1.Text = "Search:";
            // 
            // buttonReloadCFG
            // 
            buttonReloadCFG.Location = new Point(102, 108);
            buttonReloadCFG.Margin = new Padding(3, 2, 3, 2);
            buttonReloadCFG.Name = "buttonReloadCFG";
            buttonReloadCFG.Size = new Size(88, 32);
            buttonReloadCFG.TabIndex = 3;
            buttonReloadCFG.Text = "Reload";
            buttonReloadCFG.UseVisualStyleBackColor = true;
            buttonReloadCFG.Click += inputReloadConfig;
            // 
            // buttonOpenCFG
            // 
            buttonOpenCFG.Location = new Point(10, 108);
            buttonOpenCFG.Margin = new Padding(3, 2, 3, 2);
            buttonOpenCFG.Name = "buttonOpenCFG";
            buttonOpenCFG.Size = new Size(86, 32);
            buttonOpenCFG.TabIndex = 4;
            buttonOpenCFG.Text = "Open";
            buttonOpenCFG.UseVisualStyleBackColor = true;
            buttonOpenCFG.Click += inputOpenConfig;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(9, 91);
            label2.Name = "label2";
            label2.Size = new Size(46, 15);
            label2.TabIndex = 5;
            label2.Text = "Config:";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.ReHoGaBanner3_0_Transparent;
            pictureBox1.Location = new Point(10, 3);
            pictureBox1.Margin = new Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(142, 52);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.TrayLink1;
            pictureBox2.Location = new Point(158, 7);
            pictureBox2.Margin = new Padding(3, 2, 3, 2);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(56, 42);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 7;
            pictureBox2.TabStop = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(223, 11);
            label3.Name = "label3";
            label3.Size = new Size(41, 15);
            label3.TabIndex = 8;
            label3.Text = "V 1.1.0";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(220, 34);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(45, 15);
            linkLabel1.TabIndex = 9;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Update";
            linkLabel1.LinkClicked += inputUpdate;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 8F);
            button1.Location = new Point(12, 60);
            button1.Name = "button1";
            button1.Size = new Size(258, 25);
            button1.TabIndex = 10;
            button1.Text = "Shell Integration Setup";
            button1.UseVisualStyleBackColor = true;
            button1.Click += inputShellInteg;
            // 
            // chkAutoHide
            // 
            chkAutoHide.AutoSize = true;
            chkAutoHide.Location = new Point(196, 108);
            chkAutoHide.Name = "chkAutoHide";
            chkAutoHide.Size = new Size(78, 34);
            chkAutoHide.TabIndex = 11;
            chkAutoHide.Text = "Close on \r\nfocus loss";
            chkAutoHide.UseVisualStyleBackColor = true;
            // 
            // TrayPopupForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(282, 604);
            Controls.Add(chkAutoHide);
            Controls.Add(button1);
            Controls.Add(linkLabel1);
            Controls.Add(label3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(label2);
            Controls.Add(buttonOpenCFG);
            Controls.Add(buttonReloadCFG);
            Controls.Add(label1);
            Controls.Add(searchBar);
            Controls.Add(actionPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            Name = "TrayPopupForm";
            Text = "TrayLink";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NotifyIcon TrayIcon;
        private FlowLayoutPanel actionPanel;
        private TextBox searchBar;
        private Label label1;
        private Button buttonReloadCFG;
        private Button buttonOpenCFG;
        private Label label2;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Label label3;
        private LinkLabel linkLabel1;
        private Button button1;
        private CheckBox chkAutoHide;
    }
}
