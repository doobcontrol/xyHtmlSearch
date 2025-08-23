namespace Sample
{
    partial class Form1
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
            textBox1 = new TextBox();
            btnYouTube = new Button();
            btnStart = new Button();
            btnScrap = new Button();
            btnCancel = new Button();
            panel1 = new Panel();
            panel2 = new Panel();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Fill;
            textBox1.Location = new Point(0, 52);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = ScrollBars.Both;
            textBox1.Size = new Size(800, 398);
            textBox1.TabIndex = 1;
            textBox1.WordWrap = false;
            // 
            // btnYouTube
            // 
            btnYouTube.Location = new Point(112, 12);
            btnYouTube.Name = "btnYouTube";
            btnYouTube.Size = new Size(94, 29);
            btnYouTube.TabIndex = 2;
            btnYouTube.Text = "YouTube";
            btnYouTube.UseVisualStyleBackColor = true;
            btnYouTube.Click += btnYouTube_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(12, 12);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(94, 29);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnScrap
            // 
            btnScrap.Location = new Point(212, 12);
            btnScrap.Name = "btnScrap";
            btnScrap.Size = new Size(94, 29);
            btnScrap.TabIndex = 3;
            btnScrap.Text = "Scrap";
            btnScrap.UseVisualStyleBackColor = true;
            btnScrap.Click += btnScrap_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(473, 12);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(94, 29);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnScrap);
            panel1.Controls.Add(btnStart);
            panel1.Controls.Add(btnYouTube);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(458, 52);
            panel1.TabIndex = 3;
            // 
            // panel2
            // 
            panel2.Controls.Add(btnCancel);
            panel2.Controls.Add(panel1);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(800, 52);
            panel2.TabIndex = 4;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBox1);
            Controls.Add(panel2);
            Name = "Form1";
            Text = "Form1";
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox textBox1;
        private Button btnYouTube;
        private Button btnStart;
        private Button btnScrap;
        private Button btnCancel;
        private Panel panel1;
        private Panel panel2;
    }
}
