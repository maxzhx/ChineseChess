using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ChineseChess
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(pic1.ClientRectangle);
            Region region = new Region(gp);
            pic1.Region = region;
            gp.Dispose();
            region.Dispose();
        }

        private void pic1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            this.Hide();
        }

        private void pic2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pic_newgame_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            this.Hide();
        }

        private void pic_quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pic3_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                Form1 f = new Form1();
                f.openFileName = ofd1.FileName;
                f.isload = true;
                f.Show();
                this.Hide();    
            }
        }

        private void pic_loadgame_Click(object sender, EventArgs e)
        {
            pic3_Click(sender, e);
        }
    }
}
