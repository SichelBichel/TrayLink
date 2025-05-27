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
            TrayIcon = new NotifyIcon(components);
            actionPanel = new FlowLayoutPanel();
            textBox1 = new TextBox();
            label1 = new Label();
            buttonReloadCFG = new Button();
            buttonOpenCFG = new Button();
            label2 = new Label();
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
            actionPanel.Location = new Point(12, 141);
            actionPanel.Name = "actionPanel";
            actionPanel.Size = new Size(298, 501);
            actionPanel.TabIndex = 0;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 108);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(298, 27);
            textBox1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 85);
            label1.Name = "label1";
            label1.Size = new Size(56, 20);
            label1.TabIndex = 2;
            label1.Text = "Search:";
            // 
            // buttonReloadCFG
            // 
            buttonReloadCFG.Location = new Point(172, 40);
            buttonReloadCFG.Name = "buttonReloadCFG";
            buttonReloadCFG.Size = new Size(138, 42);
            buttonReloadCFG.TabIndex = 3;
            buttonReloadCFG.Text = "Reload";
            buttonReloadCFG.UseVisualStyleBackColor = true;
            buttonReloadCFG.Click += inputReloadConfig;
            // 
            // buttonOpenCFG
            // 
            buttonOpenCFG.Location = new Point(12, 40);
            buttonOpenCFG.Name = "buttonOpenCFG";
            buttonOpenCFG.Size = new Size(138, 42);
            buttonOpenCFG.TabIndex = 4;
            buttonOpenCFG.Text = "Open";
            buttonOpenCFG.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 17);
            label2.Name = "label2";
            label2.Size = new Size(56, 20);
            label2.TabIndex = 5;
            label2.Text = "Config:";
            // 
            // TrayPopupForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(322, 654);
            Controls.Add(label2);
            Controls.Add(buttonOpenCFG);
            Controls.Add(buttonReloadCFG);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(actionPanel);
            Name = "TrayPopupForm";
            Text = "TrayLink";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NotifyIcon TrayIcon;
        private FlowLayoutPanel actionPanel;
        private TextBox textBox1;
        private Label label1;
        private Button buttonReloadCFG;
        private Button buttonOpenCFG;
        private Label label2;
    }
}
