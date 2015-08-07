namespace ChineseChess
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.pic1 = new System.Windows.Forms.PictureBox();
            this.pic2 = new System.Windows.Forms.PictureBox();
            this.pic_newgame = new System.Windows.Forms.PictureBox();
            this.pic_quit = new System.Windows.Forms.PictureBox();
            this.pic3 = new System.Windows.Forms.PictureBox();
            this.pic_loadgame = new System.Windows.Forms.PictureBox();
            this.ofd1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pic1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_newgame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_quit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_loadgame)).BeginInit();
            this.SuspendLayout();
            // 
            // pic1
            // 
            this.pic1.BackColor = System.Drawing.Color.Transparent;
            this.pic1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pic1.BackgroundImage")));
            this.pic1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic1.Location = new System.Drawing.Point(25, 21);
            this.pic1.Name = "pic1";
            this.pic1.Size = new System.Drawing.Size(149, 146);
            this.pic1.TabIndex = 0;
            this.pic1.TabStop = false;
            this.pic1.Click += new System.EventHandler(this.pic1_Click);
            // 
            // pic2
            // 
            this.pic2.BackColor = System.Drawing.Color.Transparent;
            this.pic2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pic2.BackgroundImage")));
            this.pic2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic2.Location = new System.Drawing.Point(25, 338);
            this.pic2.Name = "pic2";
            this.pic2.Size = new System.Drawing.Size(149, 146);
            this.pic2.TabIndex = 2;
            this.pic2.TabStop = false;
            this.pic2.Click += new System.EventHandler(this.pic2_Click);
            // 
            // pic_newgame
            // 
            this.pic_newgame.BackColor = System.Drawing.Color.Transparent;
            this.pic_newgame.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pic_newgame.BackgroundImage")));
            this.pic_newgame.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_newgame.Location = new System.Drawing.Point(195, 56);
            this.pic_newgame.Name = "pic_newgame";
            this.pic_newgame.Size = new System.Drawing.Size(291, 59);
            this.pic_newgame.TabIndex = 4;
            this.pic_newgame.TabStop = false;
            this.pic_newgame.Click += new System.EventHandler(this.pic_newgame_Click);
            // 
            // pic_quit
            // 
            this.pic_quit.BackColor = System.Drawing.Color.Transparent;
            this.pic_quit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pic_quit.BackgroundImage")));
            this.pic_quit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_quit.Location = new System.Drawing.Point(195, 388);
            this.pic_quit.Name = "pic_quit";
            this.pic_quit.Size = new System.Drawing.Size(105, 59);
            this.pic_quit.TabIndex = 5;
            this.pic_quit.TabStop = false;
            this.pic_quit.Click += new System.EventHandler(this.pic_quit_Click);
            // 
            // pic3
            // 
            this.pic3.BackColor = System.Drawing.Color.Transparent;
            this.pic3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pic3.BackgroundImage")));
            this.pic3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic3.Location = new System.Drawing.Point(25, 173);
            this.pic3.Name = "pic3";
            this.pic3.Size = new System.Drawing.Size(149, 146);
            this.pic3.TabIndex = 6;
            this.pic3.TabStop = false;
            this.pic3.Click += new System.EventHandler(this.pic3_Click);
            // 
            // pic_loadgame
            // 
            this.pic_loadgame.BackColor = System.Drawing.Color.Transparent;
            this.pic_loadgame.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pic_loadgame.BackgroundImage")));
            this.pic_loadgame.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_loadgame.Location = new System.Drawing.Point(195, 214);
            this.pic_loadgame.Name = "pic_loadgame";
            this.pic_loadgame.Size = new System.Drawing.Size(244, 59);
            this.pic_loadgame.TabIndex = 7;
            this.pic_loadgame.TabStop = false;
            this.pic_loadgame.Click += new System.EventHandler(this.pic_loadgame_Click);
            // 
            // ofd1
            // 
            this.ofd1.FileName = "openFileDialog1";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(900, 546);
            this.Controls.Add(this.pic_loadgame);
            this.Controls.Add(this.pic3);
            this.Controls.Add(this.pic_quit);
            this.Controls.Add(this.pic_newgame);
            this.Controls.Add(this.pic2);
            this.Controls.Add(this.pic1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(916, 584);
            this.MinimumSize = new System.Drawing.Size(916, 584);
            this.Name = "Form2";
            this.Text = "中国象棋 - 欢迎界面";
            this.Load += new System.EventHandler(this.Form2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pic1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_newgame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_quit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_loadgame)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pic1;
        private System.Windows.Forms.PictureBox pic2;
        private System.Windows.Forms.PictureBox pic_newgame;
        private System.Windows.Forms.PictureBox pic_quit;
        private System.Windows.Forms.PictureBox pic3;
        private System.Windows.Forms.PictureBox pic_loadgame;
        private System.Windows.Forms.OpenFileDialog ofd1;

    }
}