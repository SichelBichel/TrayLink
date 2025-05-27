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
            actionPanel.Location = new Point(12, 206);
            actionPanel.Name = "actionPanel";
            actionPanel.Size = new Size(298, 457);
            actionPanel.TabIndex = 0;
            // 
            // searchBar
            // 
            searchBar.Location = new Point(12, 165);
            searchBar.Name = "searchBar";
            searchBar.Size = new Size(298, 27);
            searchBar.TabIndex = 1;
            searchBar.TextChanged += inputSearchQuery;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 142);
            label1.Name = "label1";
            label1.Size = new Size(56, 20);
            label1.TabIndex = 2;
            label1.Text = "Search:";
            // 
            // buttonReloadCFG
            // 
            buttonReloadCFG.Location = new Point(172, 97);
            buttonReloadCFG.Name = "buttonReloadCFG";
            buttonReloadCFG.Size = new Size(138, 42);
            buttonReloadCFG.TabIndex = 3;
            buttonReloadCFG.Text = "Reload";
            buttonReloadCFG.UseVisualStyleBackColor = true;
            buttonReloadCFG.Click += inputReloadConfig;
            // 
            // buttonOpenCFG
            // 
            buttonOpenCFG.Location = new Point(12, 97);
            buttonOpenCFG.Name = "buttonOpenCFG";
            buttonOpenCFG.Size = new Size(138, 42);
            buttonOpenCFG.TabIndex = 4;
            buttonOpenCFG.Text = "Open";
            buttonOpenCFG.UseVisualStyleBackColor = true;
            buttonOpenCFG.Click += inputOpenConfig;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 74);
            label2.Name = "label2";
            label2.Size = new Size(56, 20);
            label2.TabIndex = 5;
            label2.Text = "Config:";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.ReHoGaBanner3_0_Transparent;
            pictureBox1.Location = new Point(12, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(163, 67);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.TrayLink1;
            pictureBox2.Location = new Point(181, 9);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(64, 56);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 7;
            pictureBox2.TabStop = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(255, 15);
            label3.Name = "label3";
            label3.Size = new Size(52, 20);
            label3.TabIndex = 8;
            label3.Text = "V 1.0.0";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(252, 45);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(58, 20);
            linkLabel1.TabIndex = 9;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Update";
            // 
            // TrayPopupForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(322, 675);
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
    }
}
