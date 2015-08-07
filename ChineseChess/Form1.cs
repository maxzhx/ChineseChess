using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Chess;

/* 各点坐标
 * 43,43  97,43  151,43  205,43  259,43  313,43  367,43  421,43  475,43 
 * 43,97  97,97  151,97  205,97  259,97  313,97  367,97  421,97  475,97 
 * 43,151 97,151 151,151 205,151 259,151 313,151 367,151 421,151 475,151 
 * 43,205 97,205 151,205 205,205 259,205 313,205 367,205 421,205 475,205 
 * 43,259 97,259 151,259 205,259 259,259 313,259 367,259 421,259 475,259 
 * 
 * 43,313 97,313 151,313 205,313 259,313 313,313 367,313 421,313 475,313 
 * 43,367 97,367 151,367 205,367 259,367 313,367 367,367 421,367 475,367 
 * 43,421 97,421 151,421 205,421 259,421 313,421 367,421 421,421 475,421 
 * 43,475 97,475 151,475 205,475 259,475 313,475 367,475 421,475 475,475 
 * 43,529 97,529 151,529 205,529 259,529 313,529 367,529 421,529 475,529 
 */

namespace ChineseChess
{
    public partial class Form1 : Form
    {
        #region 变量定义
        int whichside;//轮到哪方下棋,1为红方,0为黑方
        int x1, y1;
        int r_count, b_count;//纪录步数
        BZu bzu1, bzu2, bzu3, bzu4, bzu5;//实例化黑卒
        RZu rzu1, rzu2, rzu3, rzu4, rzu5;//实例化红卒
        BPao bpao1, bpao2;
        RPao rpao1, rpao2;
        BJu bju1, bju2;
        RJu rju1, rju2;
        BMa bma1, bma2;
        RMa rma1, rma2;
        BXiang bxiang1, bxiang2;
        RXiang rxiang1, rxiang2;
        BShi bshi1, bshi2;
        RShi rshi1, rshi2;
        BJiang bjiang;
        RJiang rjiang;
        System.Media.SoundPlayer s;
        public Stack<Point> stackPoint;
        //public Stack<PictureBox> stackPictureBox;
        public Stack<string> stackPictureBox;
        int r_mint;
        int b_mint;
        PictureBox[,] p_array;
        IFormatter formatter;
        Stream stream;
        public bool isload = false;
        public string openFileName = "save\\SAVA.bin";   //存储文件名
        string saveFileName = "save\\SAVA.bin";   //存储文件名
        #endregion

        void makePicRound(PictureBox pic1)//将棋子的picturebox变成圆形
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(pic1.ClientRectangle);
            Region region = new Region(gp);
            pic1.Region = region;
            gp.Dispose();
            region.Dispose();
        }

        void r_win() 
        {
            string s_show;
            int r_dead = 0, b_dead = 0;
            for (int i = 0; i != 16; i++)
            {
                if (p_array[0, i].Parent != pic_chessboard)
                {
                    b_dead++;
                }
                if (p_array[1, i].Parent != pic_chessboard)
                {
                    r_dead++;
                }
            }
            s_show = "红棋胜利！\n\n红方:\n用时:" + lab_rTime.Text + "\n步数:" + lab_rCount.Text + "\n损失棋子:" + r_dead.ToString()
                + "\n\n黑方:\n用时:" + lab_bTime.Text + "\n步数:" + lab_bCount.Text + "\n损失棋子:" + b_dead.ToString();
            tmr1.Stop();
            MessageBox.Show(s_show); 
            pic_chessboard.Enabled = false;
            btn_TakeBack.Enabled = false;
            悔棋ToolStripMenuItem.Enabled = false;
        }
        void b_win() 
        {
            string s_show;
            int r_dead = 0, b_dead = 0;
            for (int i = 0; i != 16; i++)
            {
                if (p_array[0, i].Parent != pic_chessboard)
                {
                    b_dead++;
                }
                if (p_array[1, i].Parent != pic_chessboard)
                {
                    r_dead++;
                }
            }
            s_show = "黑棋胜利！\n\n红方:\n用时:" + lab_rTime.Text + "\n步数:" + lab_rCount.Text + "\n损失棋子:" + r_dead.ToString()
                + "\n\n黑方:\n用时:" + lab_bTime.Text + "\n步数:" + lab_bCount.Text + "\n损失棋子:" + b_dead.ToString();
            tmr1.Stop();
            MessageBox.Show(s_show);
            pic_chessboard.Enabled = false;
            btn_TakeBack.Enabled = false;
            悔棋ToolStripMenuItem.Enabled = false;
        }

        void turnToRed()//转到红方下棋
        {
            whichside = 1;
            pic_whichside.Image = Image.FromFile(@"img\whichside_red.png");
            s = new System.Media.SoundPlayer(@"sound\move.WAV");
            s.Play();
            b_count++; lab_bCount.Text = b_count.ToString();
            lab_rCount.Text = r_count.ToString();
            if (rjiang.pic.Parent == pnl_RedDied) { b_win(); }
        }
        void turnToBlack()//转到黑方下棋
        {
            whichside = 0;
            pic_whichside.Image = Image.FromFile(@"img\whichside_black.png");
            s = new System.Media.SoundPlayer(@"sound\move.WAV");
            s.Play();
            r_count++; lab_rCount.Text = r_count.ToString();
            lab_bCount.Text = b_count.ToString();
            if (bjiang.pic.Parent == pnl_BlackDied) { r_win(); }
        }

        PictureBox find_pic(string s)//根据name查找对应的picturebox
        {
            for (int i = 0; i != 2; i++)
            {
                for (int j = 0; j != 16; j++)
                {
                    if (s == p_array[i, j].Name)
                    {
                        return p_array[i, j];
                    }
                }
            }
            return null;
        }

        #region 事件函数
        void mouseDown(ChessPieces cp, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                stackPoint.Push(new Point(cp.getX(), cp.getY()));
                stackPictureBox.Push(cp.pic.Name);
                if (cp.canmove == false)
                {
                    if (cp.pic.Parent == pic_chessboard)
                    {
                        cp.canmove = true;
                        x1 = MousePosition.X;
                        y1 = MousePosition.Y;
                        cp.getTemp();//纪录起始位置
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (cp.canmove)
                {
                    //MessageBox.Show("red");
                    cp.goBack();
                    pic_chessboard.Cursor = Cursors.Default;
                    cp.canmove = false;
                }
            }
        }

        void mouseMove(ChessPieces cp, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (cp.canmove)
                {
                    int dx, dy;
                    cp.pic.BringToFront();//改变其优先级使其显示在上方
                    pic_chessboard.Cursor = Cursors.Hand;
                    dx = MousePosition.X - x1;
                    dy = MousePosition.Y - y1;
                    cp.moveto(cp.x_temp + dx, cp.y_temp + dy);
                }
            }
        }

        void mouseUp(ChessPieces cp, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (cp.pic.Parent == pic_chessboard)
                {
                    cp.canmove = false;
                    pic_chessboard.Cursor = Cursors.Default;
                    if (cp.deadPanel.Name == "pnl_RedDied")
                    {
                        if (whichside == 0)//是否轮到黑棋
                        {
                            if (cp.move())
                            {
                                btn_TakeBack.Enabled = true;
                                悔棋ToolStripMenuItem.Enabled = true;
                                turnToRed();//调用移动规则 移动成功则轮到红棋
                            }
                        }
                        else
                        {
                            cp.goBack();//若不是轮到黑棋 则返回初始位置
                        }
                    }
                    else
                    {
                        if (whichside == 1)//是否轮到红棋
                        {
                            if (cp.move())
                            {
                                btn_TakeBack.Enabled = true;
                                悔棋ToolStripMenuItem.Enabled = true;
                                turnToBlack(); //调用移动规则 移动成功则轮到黑棋                            
                            }
                        }
                        else
                        {
                            cp.goBack();//若不是轮到红棋 则返回初始位置
                        }
                    }
                }
            }
        }
        #endregion

        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (isload)//是否为载入游戏
            {
                #region 初始化棋盘
                stackPoint = new Stack<Point>();
                stackPictureBox = new Stack<string>();
                r_count = 0; b_count = 0;
                lab_bCount.Text = b_count.ToString();
                lab_rCount.Text = r_count.ToString();
                pic_chessboard.Enabled = true;
                btn_TakeBack.Enabled = false;
                悔棋ToolStripMenuItem.Enabled = false;
                whichside = 1;
                pic_whichside.Image = Image.FromFile(@"img\whichside_red.png");
                r_mint = 0;
                b_mint = 0;
                lab_rTime.Text = "00:00:00";
                lab_bTime.Text = "00:00:00";
                p_array = new PictureBox[2, 16] { { pic_Bzu1, pic_Bzu2, pic_Bzu3, pic_Bzu4, pic_Bzu5, pic_Bpao1, pic_Bpao2, pic_Bju1, pic_Bju2, pic_Bma1, pic_Bma2, pic_Bxiang1, pic_Bxiang2, pic_Bshi1, pic_Bshi2, pic_Bjiang }, 
            { pic_Rzu1, pic_Rzu2, pic_Rzu3, pic_Rzu4, pic_Rzu5, pic_Rpao1, pic_Rpao2, pic_Rju1, pic_Rju2, pic_Rma1, pic_Rma2, pic_Rxiang1, pic_Rxiang2, pic_Rshi1, pic_Rshi2, pic_Rjiang } };
                //此数组纪录下所有棋,用于判断坐标 p_array[0,x]为黑棋 p_array[1,x]为红棋
                /*
                ChessPieces[,] cp_array = new ChessPieces[2, 16] { { bzu1, bzu2, bzu3, bzu4, bzu5, bpao1, bpao2, bju1, bju2, bma1, bma2, bxiang1, bxiang2, bshi1, bshi2, bjiang }, 
                { rzu1, rzu2, rzu3, rzu4, rzu5, rpao1, rpao2, rju1, rju2, rma1, rma2, rxiang1, rxiang2, rshi1, rshi2, rjiang } };
                //此数组纪录下所有棋,用于判断坐标 cp_array[0,x]为黑棋 cp_array[1,x]为红棋
                */
                for (int i = 0; i != 2; i++)//将所有棋子的picturebox变为圆形
                {
                    for (int j = 0; j != 16; j++)
                    { makePicRound(p_array[i, j]); }
                }
                //初始化黑棋,创建父类
                bzu1 = new BZu(pic_Bzu1, pic_chessboard, p_array, pnl_RedDied, 43, 205, stackPoint, stackPictureBox);
                bzu2 = new BZu(pic_Bzu2, pic_chessboard, p_array, pnl_RedDied, 151, 205, stackPoint, stackPictureBox);
                bzu3 = new BZu(pic_Bzu3, pic_chessboard, p_array, pnl_RedDied, 259, 205, stackPoint, stackPictureBox);
                bzu4 = new BZu(pic_Bzu4, pic_chessboard, p_array, pnl_RedDied, 367, 205, stackPoint, stackPictureBox);
                bzu5 = new BZu(pic_Bzu5, pic_chessboard, p_array, pnl_RedDied, 475, 205, stackPoint, stackPictureBox);
                bpao1 = new BPao(pic_Bpao1, pic_chessboard, p_array, pnl_RedDied, 97, 151, stackPoint, stackPictureBox);
                bpao2 = new BPao(pic_Bpao2, pic_chessboard, p_array, pnl_RedDied, 421, 151, stackPoint, stackPictureBox);
                bju1 = new BJu(pic_Bju1, pic_chessboard, p_array, pnl_RedDied, 43, 43, stackPoint, stackPictureBox);
                bju2 = new BJu(pic_Bju2, pic_chessboard, p_array, pnl_RedDied, 475, 43, stackPoint, stackPictureBox);
                bma1 = new BMa(pic_Bma1, pic_chessboard, p_array, pnl_RedDied, 97, 43, stackPoint, stackPictureBox);
                bma2 = new BMa(pic_Bma2, pic_chessboard, p_array, pnl_RedDied, 421, 43, stackPoint, stackPictureBox);
                bxiang1 = new BXiang(pic_Bxiang1, pic_chessboard, p_array, pnl_RedDied, 151, 43, stackPoint, stackPictureBox);
                bxiang2 = new BXiang(pic_Bxiang2, pic_chessboard, p_array, pnl_RedDied, 367, 43, stackPoint, stackPictureBox);
                bshi1 = new BShi(pic_Bshi1, pic_chessboard, p_array, pnl_RedDied, 205, 43, stackPoint, stackPictureBox);
                bshi2 = new BShi(pic_Bshi2, pic_chessboard, p_array, pnl_RedDied, 313, 43, stackPoint, stackPictureBox);
                bjiang = new BJiang(pic_Bjiang, pic_chessboard, p_array, pnl_RedDied, 259, 43, stackPoint, stackPictureBox);

                //初始化红棋,创建父类
                rzu1 = new RZu(pic_Rzu1, pic_chessboard, p_array, pnl_BlackDied, 43, 367, stackPoint, stackPictureBox);
                rzu2 = new RZu(pic_Rzu2, pic_chessboard, p_array, pnl_BlackDied, 151, 367, stackPoint, stackPictureBox);
                rzu3 = new RZu(pic_Rzu3, pic_chessboard, p_array, pnl_BlackDied, 259, 367, stackPoint, stackPictureBox);
                rzu4 = new RZu(pic_Rzu4, pic_chessboard, p_array, pnl_BlackDied, 367, 367, stackPoint, stackPictureBox);
                rzu5 = new RZu(pic_Rzu5, pic_chessboard, p_array, pnl_BlackDied, 475, 367, stackPoint, stackPictureBox);
                rpao1 = new RPao(pic_Rpao1, pic_chessboard, p_array, pnl_BlackDied, 97, 421, stackPoint, stackPictureBox);
                rpao2 = new RPao(pic_Rpao2, pic_chessboard, p_array, pnl_BlackDied, 421, 421, stackPoint, stackPictureBox);
                rju1 = new RJu(pic_Rju1, pic_chessboard, p_array, pnl_BlackDied, 43, 529, stackPoint, stackPictureBox);
                rju2 = new RJu(pic_Rju2, pic_chessboard, p_array, pnl_BlackDied, 475, 529, stackPoint, stackPictureBox);
                rma1 = new RMa(pic_Rma1, pic_chessboard, p_array, pnl_BlackDied, 97, 529, stackPoint, stackPictureBox);
                rma2 = new RMa(pic_Rma2, pic_chessboard, p_array, pnl_BlackDied, 421, 529, stackPoint, stackPictureBox);
                rxiang1 = new RXiang(pic_Rxiang1, pic_chessboard, p_array, pnl_BlackDied, 151, 529, stackPoint, stackPictureBox);
                rxiang2 = new RXiang(pic_Rxiang2, pic_chessboard, p_array, pnl_BlackDied, 367, 529, stackPoint, stackPictureBox);
                rshi1 = new RShi(pic_Rshi1, pic_chessboard, p_array, pnl_BlackDied, 205, 529, stackPoint, stackPictureBox);
                rshi2 = new RShi(pic_Rshi2, pic_chessboard, p_array, pnl_BlackDied, 313, 529, stackPoint, stackPictureBox);
                rjiang = new RJiang(pic_Rjiang, pic_chessboard, p_array, pnl_BlackDied, 259, 529, stackPoint, stackPictureBox);

                //s = new System.Media.SoundPlayer(@"sound\begin.WAV");
               // s.Play();
                tmr1.Start();
                #endregion
                
                #region  重构棋盘
                formatter = new BinaryFormatter();
                stream = new FileStream(this.openFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                

                bzu1 = new BZu(pic_Bzu1, pic_chessboard, p_array, pnl_RedDied, 43, 205, stackPoint, stackPictureBox);
                bzu2 = new BZu(pic_Bzu2, pic_chessboard, p_array, pnl_RedDied, 151, 205, stackPoint, stackPictureBox);
                bzu3 = new BZu(pic_Bzu3, pic_chessboard, p_array, pnl_RedDied, 259, 205, stackPoint, stackPictureBox);
                bzu4 = new BZu(pic_Bzu4, pic_chessboard, p_array, pnl_RedDied, 367, 205, stackPoint, stackPictureBox);
                bzu5 = new BZu(pic_Bzu5, pic_chessboard, p_array, pnl_RedDied, 475, 205, stackPoint, stackPictureBox);
                bpao1 = new BPao(pic_Bpao1, pic_chessboard, p_array, pnl_RedDied, 97, 151, stackPoint, stackPictureBox);
                bpao2 = new BPao(pic_Bpao2, pic_chessboard, p_array, pnl_RedDied, 421, 151, stackPoint, stackPictureBox);
                bju1 = new BJu(pic_Bju1, pic_chessboard, p_array, pnl_RedDied, 43, 43, stackPoint, stackPictureBox);
                bju2 = new BJu(pic_Bju2, pic_chessboard, p_array, pnl_RedDied, 475, 43, stackPoint, stackPictureBox);
                bma1 = new BMa(pic_Bma1, pic_chessboard, p_array, pnl_RedDied, 97, 43, stackPoint, stackPictureBox);
                bma2 = new BMa(pic_Bma2, pic_chessboard, p_array, pnl_RedDied, 421, 43, stackPoint, stackPictureBox);
                bxiang1 = new BXiang(pic_Bxiang1, pic_chessboard, p_array, pnl_RedDied, 151, 43, stackPoint, stackPictureBox);
                bxiang2 = new BXiang(pic_Bxiang2, pic_chessboard, p_array, pnl_RedDied, 367, 43, stackPoint, stackPictureBox);
                bshi1 = new BShi(pic_Bshi1, pic_chessboard, p_array, pnl_RedDied, 205, 43, stackPoint, stackPictureBox);
                bshi2 = new BShi(pic_Bshi2, pic_chessboard, p_array, pnl_RedDied, 313, 43, stackPoint, stackPictureBox);
                bjiang = new BJiang(pic_Bjiang, pic_chessboard, p_array, pnl_RedDied, 259, 43, stackPoint, stackPictureBox);

                //初始化红棋,创建父类
                rzu1 = new RZu(pic_Rzu1, pic_chessboard, p_array, pnl_BlackDied, 43, 367, stackPoint, stackPictureBox);
                rzu2 = new RZu(pic_Rzu2, pic_chessboard, p_array, pnl_BlackDied, 151, 367, stackPoint, stackPictureBox);
                rzu3 = new RZu(pic_Rzu3, pic_chessboard, p_array, pnl_BlackDied, 259, 367, stackPoint, stackPictureBox);
                rzu4 = new RZu(pic_Rzu4, pic_chessboard, p_array, pnl_BlackDied, 367, 367, stackPoint, stackPictureBox);
                rzu5 = new RZu(pic_Rzu5, pic_chessboard, p_array, pnl_BlackDied, 475, 367, stackPoint, stackPictureBox);
                rpao1 = new RPao(pic_Rpao1, pic_chessboard, p_array, pnl_BlackDied, 97, 421, stackPoint, stackPictureBox);
                rpao2 = new RPao(pic_Rpao2, pic_chessboard, p_array, pnl_BlackDied, 421, 421, stackPoint, stackPictureBox);
                rju1 = new RJu(pic_Rju1, pic_chessboard, p_array, pnl_BlackDied, 43, 529, stackPoint, stackPictureBox);
                rju2 = new RJu(pic_Rju2, pic_chessboard, p_array, pnl_BlackDied, 475, 529, stackPoint, stackPictureBox);
                rma1 = new RMa(pic_Rma1, pic_chessboard, p_array, pnl_BlackDied, 97, 529, stackPoint, stackPictureBox);
                rma2 = new RMa(pic_Rma2, pic_chessboard, p_array, pnl_BlackDied, 421, 529, stackPoint, stackPictureBox);
                rxiang1 = new RXiang(pic_Rxiang1, pic_chessboard, p_array, pnl_BlackDied, 151, 529, stackPoint, stackPictureBox);
                rxiang2 = new RXiang(pic_Rxiang2, pic_chessboard, p_array, pnl_BlackDied, 367, 529, stackPoint, stackPictureBox);
                rshi1 = new RShi(pic_Rshi1, pic_chessboard, p_array, pnl_BlackDied, 205, 529, stackPoint, stackPictureBox);
                rshi2 = new RShi(pic_Rshi2, pic_chessboard, p_array, pnl_BlackDied, 313, 529, stackPoint, stackPictureBox);
                rjiang = new RJiang(pic_Rjiang, pic_chessboard, p_array, pnl_BlackDied, 259, 529, stackPoint, stackPictureBox);

                r_count = (int)formatter.Deserialize(stream); b_count = (int)formatter.Deserialize(stream);
                lab_bCount.Text = b_count.ToString();
                lab_rCount.Text = r_count.ToString();
                pic_chessboard.Enabled = (bool)formatter.Deserialize(stream);
                whichside = (int)formatter.Deserialize(stream);
                if (whichside == 1)
                {
                    pic_whichside.Image = Image.FromFile(@"img\whichside_red.png");
                }
                else
                {
                    pic_whichside.Image = Image.FromFile(@"img\whichside_black.png");
                }
                r_mint = (int)formatter.Deserialize(stream);
                b_mint = (int)formatter.Deserialize(stream);
                //计时器重写
                int h, m, s;
                string time;
                s = r_mint % 60;
                m = (int)r_mint / 60 % 60;
                h = (int)r_mint / 60 / 60;
                time = string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
                lab_rTime.Text = time;

                s = b_mint % 60;
                m = (int)b_mint / 60 % 60;
                h = (int)b_mint / 60 / 60;
                time = string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);

                lab_bTime.Text = time;

                p_array = new PictureBox[2, 16] { { pic_Bzu1, pic_Bzu2, pic_Bzu3, pic_Bzu4, pic_Bzu5, pic_Bpao1, pic_Bpao2, pic_Bju1, pic_Bju2, pic_Bma1, pic_Bma2, pic_Bxiang1, pic_Bxiang2, pic_Bshi1, pic_Bshi2, pic_Bjiang }, 
            { pic_Rzu1, pic_Rzu2, pic_Rzu3, pic_Rzu4, pic_Rzu5, pic_Rpao1, pic_Rpao2, pic_Rju1, pic_Rju2, pic_Rma1, pic_Rma2, pic_Rxiang1, pic_Rxiang2, pic_Rshi1, pic_Rshi2, pic_Rjiang } };
                //此数组纪录下所有棋,用于判断坐标 p_array[0,x]为黑棋 p_array[1,x]为红棋
                /*
                ChessPieces[,] cp_array = new ChessPieces[2, 16] { { bzu1, bzu2, bzu3, bzu4, bzu5, bpao1, bpao2, bju1, bju2, bma1, bma2, bxiang1, bxiang2, bshi1, bshi2, bjiang }, 
                { rzu1, rzu2, rzu3, rzu4, rzu5, rpao1, rpao2, rju1, rju2, rma1, rma2, rxiang1, rxiang2, rshi1, rshi2, rjiang } };
                //此数组纪录下所有棋,用于判断坐标 cp_array[0,x]为黑棋 cp_array[1,x]为红棋
                */

                for (int i = 0; i != 2; i++)//将所有棋子的picturebox变为圆形
                {
                    for (int j = 0; j != 16; j++)
                    { makePicRound(p_array[i, j]); }
                }

                int ind;
                for (int i = 0; i != 2; i++)
                {
                    for (int j = 0; j != 16; j++)
                    {
                        ind = (int)formatter.Deserialize(stream);
                        if (ind == 1) { p_array[i, j].Parent = pic_chessboard; }
                        else
                        {
                            string sn = p_array[i, j].Name;
                            if (sn[4] == 'B')
                                p_array[i, j].Parent = pnl_BlackDied;
                            else
                                p_array[i, j].Parent = pnl_RedDied;
                        }
                        p_array[i, j].Location = (Point)formatter.Deserialize(stream);
                    }
                }
                stream.Close();
                tmr1.Start();
                stackPoint.Clear();
                stackPictureBox.Clear();
                btn_TakeBack.Enabled = false;
                悔棋ToolStripMenuItem.Enabled = false;
                #endregion
            }
            else
            {
                #region 初始化棋盘
                stackPoint = new Stack<Point>();
                stackPictureBox = new Stack<string>();
                r_count = 0; b_count = 0;
                lab_bCount.Text = b_count.ToString();
                lab_rCount.Text = r_count.ToString();
                pic_chessboard.Enabled = true;
                btn_TakeBack.Enabled = false;
                悔棋ToolStripMenuItem.Enabled = false;
                whichside = 1;
                pic_whichside.Image = Image.FromFile(@"img\whichside_red.png");
                r_mint = 0;
                b_mint = 0;
                lab_rTime.Text = "00:00:00";
                lab_bTime.Text = "00:00:00";
                p_array = new PictureBox[2, 16] { { pic_Bzu1, pic_Bzu2, pic_Bzu3, pic_Bzu4, pic_Bzu5, pic_Bpao1, pic_Bpao2, pic_Bju1, pic_Bju2, pic_Bma1, pic_Bma2, pic_Bxiang1, pic_Bxiang2, pic_Bshi1, pic_Bshi2, pic_Bjiang }, 
            { pic_Rzu1, pic_Rzu2, pic_Rzu3, pic_Rzu4, pic_Rzu5, pic_Rpao1, pic_Rpao2, pic_Rju1, pic_Rju2, pic_Rma1, pic_Rma2, pic_Rxiang1, pic_Rxiang2, pic_Rshi1, pic_Rshi2, pic_Rjiang } };
                //此数组纪录下所有棋,用于判断坐标 p_array[0,x]为黑棋 p_array[1,x]为红棋
                /*
                ChessPieces[,] cp_array = new ChessPieces[2, 16] { { bzu1, bzu2, bzu3, bzu4, bzu5, bpao1, bpao2, bju1, bju2, bma1, bma2, bxiang1, bxiang2, bshi1, bshi2, bjiang }, 
                { rzu1, rzu2, rzu3, rzu4, rzu5, rpao1, rpao2, rju1, rju2, rma1, rma2, rxiang1, rxiang2, rshi1, rshi2, rjiang } };
                //此数组纪录下所有棋,用于判断坐标 cp_array[0,x]为黑棋 cp_array[1,x]为红棋
                */
                for (int i = 0; i != 2; i++)//将所有棋子的picturebox变为圆形
                {
                    for (int j = 0; j != 16; j++)
                    { makePicRound(p_array[i, j]); }
                }
                //初始化黑棋,创建父类
                bzu1 = new BZu(pic_Bzu1, pic_chessboard, p_array, pnl_RedDied, 43, 205, stackPoint, stackPictureBox);
                bzu2 = new BZu(pic_Bzu2, pic_chessboard, p_array, pnl_RedDied, 151, 205, stackPoint, stackPictureBox);
                bzu3 = new BZu(pic_Bzu3, pic_chessboard, p_array, pnl_RedDied, 259, 205, stackPoint, stackPictureBox);
                bzu4 = new BZu(pic_Bzu4, pic_chessboard, p_array, pnl_RedDied, 367, 205, stackPoint, stackPictureBox);
                bzu5 = new BZu(pic_Bzu5, pic_chessboard, p_array, pnl_RedDied, 475, 205, stackPoint, stackPictureBox);
                bpao1 = new BPao(pic_Bpao1, pic_chessboard, p_array, pnl_RedDied, 97, 151, stackPoint, stackPictureBox);
                bpao2 = new BPao(pic_Bpao2, pic_chessboard, p_array, pnl_RedDied, 421, 151, stackPoint, stackPictureBox);
                bju1 = new BJu(pic_Bju1, pic_chessboard, p_array, pnl_RedDied, 43, 43, stackPoint, stackPictureBox);
                bju2 = new BJu(pic_Bju2, pic_chessboard, p_array, pnl_RedDied, 475, 43, stackPoint, stackPictureBox);
                bma1 = new BMa(pic_Bma1, pic_chessboard, p_array, pnl_RedDied, 97, 43, stackPoint, stackPictureBox);
                bma2 = new BMa(pic_Bma2, pic_chessboard, p_array, pnl_RedDied, 421, 43, stackPoint, stackPictureBox);
                bxiang1 = new BXiang(pic_Bxiang1, pic_chessboard, p_array, pnl_RedDied, 151, 43, stackPoint, stackPictureBox);
                bxiang2 = new BXiang(pic_Bxiang2, pic_chessboard, p_array, pnl_RedDied, 367, 43, stackPoint, stackPictureBox);
                bshi1 = new BShi(pic_Bshi1, pic_chessboard, p_array, pnl_RedDied, 205, 43, stackPoint, stackPictureBox);
                bshi2 = new BShi(pic_Bshi2, pic_chessboard, p_array, pnl_RedDied, 313, 43, stackPoint, stackPictureBox);
                bjiang = new BJiang(pic_Bjiang, pic_chessboard, p_array, pnl_RedDied, 259, 43, stackPoint, stackPictureBox);

                //初始化红棋,创建父类
                rzu1 = new RZu(pic_Rzu1, pic_chessboard, p_array, pnl_BlackDied, 43, 367, stackPoint, stackPictureBox);
                rzu2 = new RZu(pic_Rzu2, pic_chessboard, p_array, pnl_BlackDied, 151, 367, stackPoint, stackPictureBox);
                rzu3 = new RZu(pic_Rzu3, pic_chessboard, p_array, pnl_BlackDied, 259, 367, stackPoint, stackPictureBox);
                rzu4 = new RZu(pic_Rzu4, pic_chessboard, p_array, pnl_BlackDied, 367, 367, stackPoint, stackPictureBox);
                rzu5 = new RZu(pic_Rzu5, pic_chessboard, p_array, pnl_BlackDied, 475, 367, stackPoint, stackPictureBox);
                rpao1 = new RPao(pic_Rpao1, pic_chessboard, p_array, pnl_BlackDied, 97, 421, stackPoint, stackPictureBox);
                rpao2 = new RPao(pic_Rpao2, pic_chessboard, p_array, pnl_BlackDied, 421, 421, stackPoint, stackPictureBox);
                rju1 = new RJu(pic_Rju1, pic_chessboard, p_array, pnl_BlackDied, 43, 529, stackPoint, stackPictureBox);
                rju2 = new RJu(pic_Rju2, pic_chessboard, p_array, pnl_BlackDied, 475, 529, stackPoint, stackPictureBox);
                rma1 = new RMa(pic_Rma1, pic_chessboard, p_array, pnl_BlackDied, 97, 529, stackPoint, stackPictureBox);
                rma2 = new RMa(pic_Rma2, pic_chessboard, p_array, pnl_BlackDied, 421, 529, stackPoint, stackPictureBox);
                rxiang1 = new RXiang(pic_Rxiang1, pic_chessboard, p_array, pnl_BlackDied, 151, 529, stackPoint, stackPictureBox);
                rxiang2 = new RXiang(pic_Rxiang2, pic_chessboard, p_array, pnl_BlackDied, 367, 529, stackPoint, stackPictureBox);
                rshi1 = new RShi(pic_Rshi1, pic_chessboard, p_array, pnl_BlackDied, 205, 529, stackPoint, stackPictureBox);
                rshi2 = new RShi(pic_Rshi2, pic_chessboard, p_array, pnl_BlackDied, 313, 529, stackPoint, stackPictureBox);
                rjiang = new RJiang(pic_Rjiang, pic_chessboard, p_array, pnl_BlackDied, 259, 529, stackPoint, stackPictureBox);

                s = new System.Media.SoundPlayer(@"sound\begin.WAV");
                s.Play();
                tmr1.Start();
                #endregion
            }

            #region 初始化背景音乐
            wmp_background.currentPlaylist.appendItem(wmp_background.newMedia(@"sound\background\2.mp3"));
            wmp_background.currentPlaylist.appendItem(wmp_background.newMedia(@"sound\background\3.mp3"));
            wmp_background.currentPlaylist.appendItem(wmp_background.newMedia(@"sound\background\4.mp3"));
            wmp_background.currentPlaylist.appendItem(wmp_background.newMedia(@"sound\background\5.mp3"));
            wmp_background.currentPlaylist.appendItem(wmp_background.newMedia(@"sound\background\6.mp3"));
            wmp_background.settings.volume = 30;
            wmp_background.settings.playCount = 99;
            wmp_background.Ctlcontrols.play();
            #endregion
        }

        #region 棋子事件
        #region 黑卒事件
        private void pic_Bzu1_MouseDown(object sender, MouseEventArgs e)
        {

            mouseDown(bzu1, e);
        }

        private void pic_Bzu1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bzu1, e);
        }

        private void pic_Bzu1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bzu1, e);
        }

        private void pic_Bzu2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bzu2, e);
        }

        private void pic_Bzu2_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bzu2, e);
        }

        private void pic_Bzu2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bzu2, e);
        }

        private void pic_Bzu3_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bzu3, e);
        }

        private void pic_Bzu3_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bzu3, e);
        }

        private void pic_Bzu3_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bzu3, e);
        }

        private void pic_Bzu4_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bzu4, e);
        }

        private void pic_Bzu4_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bzu4, e);
        }

        private void pic_Bzu4_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bzu4, e);
        }

        private void pic_Bzu5_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bzu5, e);
        }

        private void pic_Bzu5_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bzu5, e);
        }

        private void pic_Bzu5_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bzu5, e);
        }
        #endregion

        #region 红卒事件
        private void pic_Rzu1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rzu1, e);
        }

        private void pic_Rzu1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rzu1, e);
        }

        private void pic_Rzu1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rzu1, e);
        }

        private void pic_Rzu2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rzu2, e);
        }

        private void pic_Rzu2_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rzu2, e);
        }

        private void pic_Rzu2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rzu2, e);
        }

        private void pic_Rzu3_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rzu3, e);
        }

        private void pic_Rzu3_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rzu3, e);
        }

        private void pic_Rzu3_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rzu3, e);
        }

        private void pic_Rzu4_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rzu4, e);
        }

        private void pic_Rzu4_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rzu4, e);
        }

        private void pic_Rzu4_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rzu4, e);
        }

        private void pic_Rzu5_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rzu5, e);
        }

        private void pic_Rzu5_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rzu5, e);
        }

        private void pic_Rzu5_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rzu5, e);
        }
        #endregion

        #region 黑将事件
        private void pic_Bjiang_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bjiang, e);
        }

        private void pic_Bjiang_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bjiang, e);
        }

        private void pic_Bjiang_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bjiang, e);
        }
        #endregion

        #region 红将事件
        private void pic_Rjiang_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rjiang, e);
        }

        private void pic_Rjiang_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rjiang, e);
        }

        private void pic_Rjiang_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rjiang, e);
        }
        #endregion

        #region 黑士事件
        private void pic_BShi1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bshi1, e);
        }

        private void pic_BShi1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bshi1, e);
        }
        private void pic_BShi1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bshi1, e);
        }
        private void pic_BShi2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bshi2, e);
        }

        private void pic_BShi2_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bshi2, e);
        }

        private void pic_BShi2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bshi2, e);
        }
        #endregion

        #region 红士事件
        private void pic_RShi1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rshi1, e);
        }

        private void pic_RShi1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rshi1, e);
        }
        private void pic_RShi1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rshi1, e);
        }
        private void pic_RShi2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rshi2, e);
        }

        private void pic_RShi2_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rshi2, e);
        }

        private void pic_RShi2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rshi2, e);
        }
        #endregion

        #region 黑炮事件
        private void pic_BPao1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bpao1, e);
        }

        private void pic_BPao1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bpao1, e);
        }
        private void pic_BPao1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bpao1, e);
        }
        private void pic_BPao2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bpao2, e);
        }

        private void pic_BPao2_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bpao2, e);
        }

        private void pic_BPao2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bpao2, e);
        }
        #endregion

        #region 红炮事件
        private void pic_RPao1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rpao1, e);
        }

        private void pic_RPao1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rpao1, e);
        }
        private void pic_RPao1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rpao1, e);
        }
        private void pic_RPao2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rpao2, e);
        }

        private void pic_RPao2_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rpao2, e);
        }

        private void pic_RPao2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rpao2, e);
        }
        #endregion

        #region 黑象事件
        private void pic_BXiang1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bxiang1, e);
        }

        private void pic_BXiang1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bxiang1, e);
        }
        private void pic_BXiang1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bxiang1, e);
        }
        private void pic_BXiang2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bxiang2, e);
        }

        private void pic_BXiang2_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bxiang2, e);
        }

        private void pic_BXiang2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bxiang2, e);
        }
        #endregion

        #region 红象事件
        private void pic_RXiang1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rxiang1, e);
        }

        private void pic_RXiang1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rxiang1, e);
        }
        private void pic_RXiang1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rxiang1, e);
        }
        private void pic_RXiang2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rxiang2, e);
        }

        private void pic_RXiang2_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rxiang2, e);
        }

        private void pic_RXiang2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rxiang2, e);
        }
        #endregion

        #region 黑马事件
        private void pic_Bma1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bma1, e);
        }
        private void pic_Bma1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bma1, e);
        }

        private void pic_Bma1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bma1, e);
        }
        private void pic_Bma2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bma2, e);
        }
        private void pic_Bma2_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bma2, e);
        }

        private void pic_Bma2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bma2, e);
        }
        #endregion

        #region 红马事件
        private void pic_Rma1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rma1, e);
        }

        private void pic_Rma1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rma1, e);
        }

        private void pic_Rma1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rma1, e);
        }
        private void pic_Rma2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rma2, e);
        }

        private void pic_Rma2_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rma2, e);
        }

        private void pic_Rma2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rma2, e);
        }
        #endregion

        #region 黑车事件
        private void pic_BJu1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bju1, e);
        }

        private void pic_BJu1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bju1, e);
        }
        private void pic_BJu1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bju1, e);
        }
        private void pic_BJu2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(bju2, e);
        }

        private void pic_BJu2_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(bju2, e);
        }

        private void pic_BJu2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(bju2, e);
        }
        #endregion

        #region 红车事件
        private void pic_RJu1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rju1, e);
        }

        private void pic_RJu1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rju1, e);
        }
        private void pic_RJu1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rju1, e);
        }
        private void pic_RJu2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(rju2, e);
        }

        private void pic_RJu2_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMove(rju2, e);
        }

        private void pic_RJu2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(rju2, e);
        }
        #endregion

        #endregion

        private void btn_newgame_Click(object sender, EventArgs e)
        {
            #region 初始化棋盘
            stackPoint = new Stack<Point>();
            stackPictureBox = new Stack<string>();
            r_count = 0; b_count = 0;
            lab_bCount.Text = b_count.ToString();
            lab_rCount.Text = r_count.ToString();
            pic_chessboard.Enabled = true;
            btn_TakeBack.Enabled = false;
            悔棋ToolStripMenuItem.Enabled = false;
            whichside = 1;
            pic_whichside.Image = Image.FromFile(@"img\whichside_red.png");
            r_mint = 0;
            b_mint = 0;
            lab_rTime.Text = "00:00:00";
            lab_bTime.Text = "00:00:00";
            p_array = new PictureBox[2, 16] { { pic_Bzu1, pic_Bzu2, pic_Bzu3, pic_Bzu4, pic_Bzu5, pic_Bpao1, pic_Bpao2, pic_Bju1, pic_Bju2, pic_Bma1, pic_Bma2, pic_Bxiang1, pic_Bxiang2, pic_Bshi1, pic_Bshi2, pic_Bjiang }, 
            { pic_Rzu1, pic_Rzu2, pic_Rzu3, pic_Rzu4, pic_Rzu5, pic_Rpao1, pic_Rpao2, pic_Rju1, pic_Rju2, pic_Rma1, pic_Rma2, pic_Rxiang1, pic_Rxiang2, pic_Rshi1, pic_Rshi2, pic_Rjiang } };
            //此数组纪录下所有棋,用于判断坐标 p_array[0,x]为黑棋 p_array[1,x]为红棋
            /*
            ChessPieces[,] cp_array = new ChessPieces[2, 16] { { bzu1, bzu2, bzu3, bzu4, bzu5, bpao1, bpao2, bju1, bju2, bma1, bma2, bxiang1, bxiang2, bshi1, bshi2, bjiang }, 
            { rzu1, rzu2, rzu3, rzu4, rzu5, rpao1, rpao2, rju1, rju2, rma1, rma2, rxiang1, rxiang2, rshi1, rshi2, rjiang } };
            //此数组纪录下所有棋,用于判断坐标 cp_array[0,x]为黑棋 cp_array[1,x]为红棋
            */
            for (int i = 0; i != 2; i++)//将所有棋子的picturebox变为圆形
            {
                for (int j = 0; j != 16; j++)
                { makePicRound(p_array[i, j]); }
            }
            //初始化黑棋,创建父类
            bzu1 = new BZu(pic_Bzu1, pic_chessboard, p_array, pnl_RedDied, 43, 205, stackPoint, stackPictureBox);
            bzu2 = new BZu(pic_Bzu2, pic_chessboard, p_array, pnl_RedDied, 151, 205, stackPoint, stackPictureBox);
            bzu3 = new BZu(pic_Bzu3, pic_chessboard, p_array, pnl_RedDied, 259, 205, stackPoint, stackPictureBox);
            bzu4 = new BZu(pic_Bzu4, pic_chessboard, p_array, pnl_RedDied, 367, 205, stackPoint, stackPictureBox);
            bzu5 = new BZu(pic_Bzu5, pic_chessboard, p_array, pnl_RedDied, 475, 205, stackPoint, stackPictureBox);
            bpao1 = new BPao(pic_Bpao1, pic_chessboard, p_array, pnl_RedDied, 97, 151, stackPoint, stackPictureBox);
            bpao2 = new BPao(pic_Bpao2, pic_chessboard, p_array, pnl_RedDied, 421, 151, stackPoint, stackPictureBox);
            bju1 = new BJu(pic_Bju1, pic_chessboard, p_array, pnl_RedDied, 43, 43, stackPoint, stackPictureBox);
            bju2 = new BJu(pic_Bju2, pic_chessboard, p_array, pnl_RedDied, 475, 43, stackPoint, stackPictureBox);
            bma1 = new BMa(pic_Bma1, pic_chessboard, p_array, pnl_RedDied, 97, 43, stackPoint, stackPictureBox);
            bma2 = new BMa(pic_Bma2, pic_chessboard, p_array, pnl_RedDied, 421, 43, stackPoint, stackPictureBox);
            bxiang1 = new BXiang(pic_Bxiang1, pic_chessboard, p_array, pnl_RedDied, 151, 43, stackPoint, stackPictureBox);
            bxiang2 = new BXiang(pic_Bxiang2, pic_chessboard, p_array, pnl_RedDied, 367, 43, stackPoint, stackPictureBox);
            bshi1 = new BShi(pic_Bshi1, pic_chessboard, p_array, pnl_RedDied, 205, 43, stackPoint, stackPictureBox);
            bshi2 = new BShi(pic_Bshi2, pic_chessboard, p_array, pnl_RedDied, 313, 43, stackPoint, stackPictureBox);
            bjiang = new BJiang(pic_Bjiang, pic_chessboard, p_array, pnl_RedDied, 259, 43, stackPoint, stackPictureBox);

            //初始化红棋,创建父类
            rzu1 = new RZu(pic_Rzu1, pic_chessboard, p_array, pnl_BlackDied, 43, 367, stackPoint, stackPictureBox);
            rzu2 = new RZu(pic_Rzu2, pic_chessboard, p_array, pnl_BlackDied, 151, 367, stackPoint, stackPictureBox);
            rzu3 = new RZu(pic_Rzu3, pic_chessboard, p_array, pnl_BlackDied, 259, 367, stackPoint, stackPictureBox);
            rzu4 = new RZu(pic_Rzu4, pic_chessboard, p_array, pnl_BlackDied, 367, 367, stackPoint, stackPictureBox);
            rzu5 = new RZu(pic_Rzu5, pic_chessboard, p_array, pnl_BlackDied, 475, 367, stackPoint, stackPictureBox);
            rpao1 = new RPao(pic_Rpao1, pic_chessboard, p_array, pnl_BlackDied, 97, 421, stackPoint, stackPictureBox);
            rpao2 = new RPao(pic_Rpao2, pic_chessboard, p_array, pnl_BlackDied, 421, 421, stackPoint, stackPictureBox);
            rju1 = new RJu(pic_Rju1, pic_chessboard, p_array, pnl_BlackDied, 43, 529, stackPoint, stackPictureBox);
            rju2 = new RJu(pic_Rju2, pic_chessboard, p_array, pnl_BlackDied, 475, 529, stackPoint, stackPictureBox);
            rma1 = new RMa(pic_Rma1, pic_chessboard, p_array, pnl_BlackDied, 97, 529, stackPoint, stackPictureBox);
            rma2 = new RMa(pic_Rma2, pic_chessboard, p_array, pnl_BlackDied, 421, 529, stackPoint, stackPictureBox);
            rxiang1 = new RXiang(pic_Rxiang1, pic_chessboard, p_array, pnl_BlackDied, 151, 529, stackPoint, stackPictureBox);
            rxiang2 = new RXiang(pic_Rxiang2, pic_chessboard, p_array, pnl_BlackDied, 367, 529, stackPoint, stackPictureBox);
            rshi1 = new RShi(pic_Rshi1, pic_chessboard, p_array, pnl_BlackDied, 205, 529, stackPoint, stackPictureBox);
            rshi2 = new RShi(pic_Rshi2, pic_chessboard, p_array, pnl_BlackDied, 313, 529, stackPoint, stackPictureBox);
            rjiang = new RJiang(pic_Rjiang, pic_chessboard, p_array, pnl_BlackDied, 259, 529, stackPoint, stackPictureBox);

            s = new System.Media.SoundPlayer(@"sound\begin.WAV");
            s.Play();
            tmr1.Start();
            #endregion
        }



        private void 开始新游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region 初始化棋盘
            stackPoint = new Stack<Point>();
            stackPictureBox = new Stack<string>();
            r_count = 0; b_count = 0;
            lab_bCount.Text = b_count.ToString();
            lab_rCount.Text = r_count.ToString();
            pic_chessboard.Enabled = true;
            btn_TakeBack.Enabled = false;
            悔棋ToolStripMenuItem.Enabled = false;
            whichside = 1;
            pic_whichside.Image = Image.FromFile(@"img\whichside_red.png");
            r_mint = 0;
            b_mint = 0;
            lab_rTime.Text = "00:00:00";
            lab_bTime.Text = "00:00:00";
            p_array = new PictureBox[2, 16] { { pic_Bzu1, pic_Bzu2, pic_Bzu3, pic_Bzu4, pic_Bzu5, pic_Bpao1, pic_Bpao2, pic_Bju1, pic_Bju2, pic_Bma1, pic_Bma2, pic_Bxiang1, pic_Bxiang2, pic_Bshi1, pic_Bshi2, pic_Bjiang }, 
            { pic_Rzu1, pic_Rzu2, pic_Rzu3, pic_Rzu4, pic_Rzu5, pic_Rpao1, pic_Rpao2, pic_Rju1, pic_Rju2, pic_Rma1, pic_Rma2, pic_Rxiang1, pic_Rxiang2, pic_Rshi1, pic_Rshi2, pic_Rjiang } };
            //此数组纪录下所有棋,用于判断坐标 p_array[0,x]为黑棋 p_array[1,x]为红棋
            /*
            ChessPieces[,] cp_array = new ChessPieces[2, 16] { { bzu1, bzu2, bzu3, bzu4, bzu5, bpao1, bpao2, bju1, bju2, bma1, bma2, bxiang1, bxiang2, bshi1, bshi2, bjiang }, 
            { rzu1, rzu2, rzu3, rzu4, rzu5, rpao1, rpao2, rju1, rju2, rma1, rma2, rxiang1, rxiang2, rshi1, rshi2, rjiang } };
            //此数组纪录下所有棋,用于判断坐标 cp_array[0,x]为黑棋 cp_array[1,x]为红棋
            */
            for (int i = 0; i != 2; i++)//将所有棋子的picturebox变为圆形
            {
                for (int j = 0; j != 16; j++)
                { makePicRound(p_array[i, j]); }
            }
            //初始化黑棋,创建父类
            bzu1 = new BZu(pic_Bzu1, pic_chessboard, p_array, pnl_RedDied, 43, 205, stackPoint, stackPictureBox);
            bzu2 = new BZu(pic_Bzu2, pic_chessboard, p_array, pnl_RedDied, 151, 205, stackPoint, stackPictureBox);
            bzu3 = new BZu(pic_Bzu3, pic_chessboard, p_array, pnl_RedDied, 259, 205, stackPoint, stackPictureBox);
            bzu4 = new BZu(pic_Bzu4, pic_chessboard, p_array, pnl_RedDied, 367, 205, stackPoint, stackPictureBox);
            bzu5 = new BZu(pic_Bzu5, pic_chessboard, p_array, pnl_RedDied, 475, 205, stackPoint, stackPictureBox);
            bpao1 = new BPao(pic_Bpao1, pic_chessboard, p_array, pnl_RedDied, 97, 151, stackPoint, stackPictureBox);
            bpao2 = new BPao(pic_Bpao2, pic_chessboard, p_array, pnl_RedDied, 421, 151, stackPoint, stackPictureBox);
            bju1 = new BJu(pic_Bju1, pic_chessboard, p_array, pnl_RedDied, 43, 43, stackPoint, stackPictureBox);
            bju2 = new BJu(pic_Bju2, pic_chessboard, p_array, pnl_RedDied, 475, 43, stackPoint, stackPictureBox);
            bma1 = new BMa(pic_Bma1, pic_chessboard, p_array, pnl_RedDied, 97, 43, stackPoint, stackPictureBox);
            bma2 = new BMa(pic_Bma2, pic_chessboard, p_array, pnl_RedDied, 421, 43, stackPoint, stackPictureBox);
            bxiang1 = new BXiang(pic_Bxiang1, pic_chessboard, p_array, pnl_RedDied, 151, 43, stackPoint, stackPictureBox);
            bxiang2 = new BXiang(pic_Bxiang2, pic_chessboard, p_array, pnl_RedDied, 367, 43, stackPoint, stackPictureBox);
            bshi1 = new BShi(pic_Bshi1, pic_chessboard, p_array, pnl_RedDied, 205, 43, stackPoint, stackPictureBox);
            bshi2 = new BShi(pic_Bshi2, pic_chessboard, p_array, pnl_RedDied, 313, 43, stackPoint, stackPictureBox);
            bjiang = new BJiang(pic_Bjiang, pic_chessboard, p_array, pnl_RedDied, 259, 43, stackPoint, stackPictureBox);

            //初始化红棋,创建父类
            rzu1 = new RZu(pic_Rzu1, pic_chessboard, p_array, pnl_BlackDied, 43, 367, stackPoint, stackPictureBox);
            rzu2 = new RZu(pic_Rzu2, pic_chessboard, p_array, pnl_BlackDied, 151, 367, stackPoint, stackPictureBox);
            rzu3 = new RZu(pic_Rzu3, pic_chessboard, p_array, pnl_BlackDied, 259, 367, stackPoint, stackPictureBox);
            rzu4 = new RZu(pic_Rzu4, pic_chessboard, p_array, pnl_BlackDied, 367, 367, stackPoint, stackPictureBox);
            rzu5 = new RZu(pic_Rzu5, pic_chessboard, p_array, pnl_BlackDied, 475, 367, stackPoint, stackPictureBox);
            rpao1 = new RPao(pic_Rpao1, pic_chessboard, p_array, pnl_BlackDied, 97, 421, stackPoint, stackPictureBox);
            rpao2 = new RPao(pic_Rpao2, pic_chessboard, p_array, pnl_BlackDied, 421, 421, stackPoint, stackPictureBox);
            rju1 = new RJu(pic_Rju1, pic_chessboard, p_array, pnl_BlackDied, 43, 529, stackPoint, stackPictureBox);
            rju2 = new RJu(pic_Rju2, pic_chessboard, p_array, pnl_BlackDied, 475, 529, stackPoint, stackPictureBox);
            rma1 = new RMa(pic_Rma1, pic_chessboard, p_array, pnl_BlackDied, 97, 529, stackPoint, stackPictureBox);
            rma2 = new RMa(pic_Rma2, pic_chessboard, p_array, pnl_BlackDied, 421, 529, stackPoint, stackPictureBox);
            rxiang1 = new RXiang(pic_Rxiang1, pic_chessboard, p_array, pnl_BlackDied, 151, 529, stackPoint, stackPictureBox);
            rxiang2 = new RXiang(pic_Rxiang2, pic_chessboard, p_array, pnl_BlackDied, 367, 529, stackPoint, stackPictureBox);
            rshi1 = new RShi(pic_Rshi1, pic_chessboard, p_array, pnl_BlackDied, 205, 529, stackPoint, stackPictureBox);
            rshi2 = new RShi(pic_Rshi2, pic_chessboard, p_array, pnl_BlackDied, 313, 529, stackPoint, stackPictureBox);
            rjiang = new RJiang(pic_Rjiang, pic_chessboard, p_array, pnl_BlackDied, 259, 529, stackPoint, stackPictureBox);

            s = new System.Media.SoundPlayer(@"sound\begin.WAV");
            s.Play();
            tmr1.Start();
            #endregion
        }

        private void btn_switchBackMusic_Click(object sender, EventArgs e)
        {
            if ((int)wmp_background.playState == 1)
                wmp_background.Ctlcontrols.play();
            wmp_background.Ctlcontrols.next();
        }
        private void btn_stopOrPlayBackMusic_Click(object sender, EventArgs e)
        {
            if ((int)wmp_background.playState == 3)
                wmp_background.Ctlcontrols.stop();
            else if ((int)wmp_background.playState == 1)
                wmp_background.Ctlcontrols.play();
        }
        private void 切换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((int)wmp_background.playState == 1)
                wmp_background.Ctlcontrols.play();
            wmp_background.Ctlcontrols.next();
        }
        private void 播放停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((int)wmp_background.playState == 3)
                wmp_background.Ctlcontrols.stop();
            else if ((int)wmp_background.playState == 1)
                wmp_background.Ctlcontrols.play();
        }

        private void btn_TakeBack_Click(object sender, EventArgs e)
        {
            pic_chessboard.Enabled = true;
            Point p = stackPoint.Pop();
            string s = stackPictureBox.Pop();
            PictureBox pb = find_pic(s);

            if (pb.Parent != pic_chessboard)
            {
                pb.Parent = pic_chessboard;
                pb.Location = p;
                p = stackPoint.Pop();
                s = stackPictureBox.Pop();
                pb = find_pic(s);
                pb.Location = p;
                if (whichside == 0) // 黑方
                {
                    b_count--;
                    r_count--;
                    turnToRed();
                }
                else
                {
                    b_count--;
                    r_count--;
                    turnToBlack();
                }
            }
            else
            {
                pb.Location = p;
                if (whichside == 0) // 黑方
                {
                    b_count--;
                    r_count--;
                    turnToRed();
                }
                else
                {
                    b_count--;
                    r_count--;
                    turnToBlack();
                }
            }
            if (stackPoint.Count == 0 || stackPictureBox.Count == 0)
            {
                btn_TakeBack.Enabled = false;
                悔棋ToolStripMenuItem.Enabled = false;
            }
        }
        private void 悔棋ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pic_chessboard.Enabled = true;
            Point p = stackPoint.Pop();
            string s = stackPictureBox.Pop();
            PictureBox pb = find_pic(s);
            if (pb.Parent != pic_chessboard)
            {
                pb.Parent = pic_chessboard;
                pb.Location = p;
                p = stackPoint.Pop();
                s = stackPictureBox.Pop();
                pb = find_pic(s);
                pb.Location = p;
                if (whichside == 0) // 黑方
                {
                    b_count--;
                    r_count--;
                    turnToRed();
                }
                else
                {
                    b_count--;
                    r_count--;
                    turnToBlack();
                }
            }
            else
            {
                pb.Location = p;
                if (whichside == 0) // 黑方
                {
                    b_count--;
                    r_count--;
                    turnToRed();
                }
                else
                {
                    b_count--;
                    r_count--;
                    turnToBlack();
                }
            }
            if (stackPoint.Count == 0 || stackPictureBox.Count == 0)
            {
                btn_TakeBack.Enabled = false;
                悔棋ToolStripMenuItem.Enabled = false;
            }
        }

        private void tmr1_Tick(object sender, EventArgs e)//计时
        {
            if (whichside == 1)
            {
                r_mint++;
                int h, m, s;
                string time;
                s = r_mint % 60;
                m = (int)r_mint / 60 % 60;
                h = (int)r_mint / 60 / 60;
                time = string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
                lab_rTime.Text = time;
            }
            else
            {
                b_mint++;
                int h, m, s;
                string time;
                s = b_mint % 60;
                m = (int)b_mint / 60 % 60;
                h = (int)b_mint / 60 / 60;
                time = string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
                lab_bTime.Text = time;
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {

            if (sfd1.ShowDialog() == DialogResult.OK)
            {
                saveFileName = sfd1.FileName;
                formatter = new BinaryFormatter();   //定义类,主要用此类中的两个方法来实现功能
                stream = new FileStream(this.saveFileName, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, r_count);
                formatter.Serialize(stream, b_count);
                formatter.Serialize(stream, pic_chessboard.Enabled);
                formatter.Serialize(stream, whichside);
                formatter.Serialize(stream, r_mint);
                formatter.Serialize(stream, b_mint);
                for (int i = 0; i != 2; i++)
                {
                    for (int j = 0; j != 16; j++)
                    {
                        if (p_array[i, j].Parent == pic_chessboard)
                        {
                            formatter.Serialize(stream, 1);//序列化1 表示该棋子活着
                        }
                        else
                        {
                            formatter.Serialize(stream, 0);//序列化0 表示该棋子死了 
                        }
                        formatter.Serialize(stream, p_array[i, j].Location);
                    }
                }

                stream.Close();
            }
        }
        private void btn_load_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                openFileName = ofd1.FileName;
                formatter = new BinaryFormatter();
                stream = new FileStream(this.openFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                #region  重构棋盘
                //初始化黑棋,创建父类

                bzu1 = new BZu(pic_Bzu1, pic_chessboard, p_array, pnl_RedDied, 43, 205, stackPoint, stackPictureBox);
                bzu2 = new BZu(pic_Bzu2, pic_chessboard, p_array, pnl_RedDied, 151, 205, stackPoint, stackPictureBox);
                bzu3 = new BZu(pic_Bzu3, pic_chessboard, p_array, pnl_RedDied, 259, 205, stackPoint, stackPictureBox);
                bzu4 = new BZu(pic_Bzu4, pic_chessboard, p_array, pnl_RedDied, 367, 205, stackPoint, stackPictureBox);
                bzu5 = new BZu(pic_Bzu5, pic_chessboard, p_array, pnl_RedDied, 475, 205, stackPoint, stackPictureBox);
                bpao1 = new BPao(pic_Bpao1, pic_chessboard, p_array, pnl_RedDied, 97, 151, stackPoint, stackPictureBox);
                bpao2 = new BPao(pic_Bpao2, pic_chessboard, p_array, pnl_RedDied, 421, 151, stackPoint, stackPictureBox);
                bju1 = new BJu(pic_Bju1, pic_chessboard, p_array, pnl_RedDied, 43, 43, stackPoint, stackPictureBox);
                bju2 = new BJu(pic_Bju2, pic_chessboard, p_array, pnl_RedDied, 475, 43, stackPoint, stackPictureBox);
                bma1 = new BMa(pic_Bma1, pic_chessboard, p_array, pnl_RedDied, 97, 43, stackPoint, stackPictureBox);
                bma2 = new BMa(pic_Bma2, pic_chessboard, p_array, pnl_RedDied, 421, 43, stackPoint, stackPictureBox);
                bxiang1 = new BXiang(pic_Bxiang1, pic_chessboard, p_array, pnl_RedDied, 151, 43, stackPoint, stackPictureBox);
                bxiang2 = new BXiang(pic_Bxiang2, pic_chessboard, p_array, pnl_RedDied, 367, 43, stackPoint, stackPictureBox);
                bshi1 = new BShi(pic_Bshi1, pic_chessboard, p_array, pnl_RedDied, 205, 43, stackPoint, stackPictureBox);
                bshi2 = new BShi(pic_Bshi2, pic_chessboard, p_array, pnl_RedDied, 313, 43, stackPoint, stackPictureBox);
                bjiang = new BJiang(pic_Bjiang, pic_chessboard, p_array, pnl_RedDied, 259, 43, stackPoint, stackPictureBox);

                //初始化红棋,创建父类
                rzu1 = new RZu(pic_Rzu1, pic_chessboard, p_array, pnl_BlackDied, 43, 367, stackPoint, stackPictureBox);
                rzu2 = new RZu(pic_Rzu2, pic_chessboard, p_array, pnl_BlackDied, 151, 367, stackPoint, stackPictureBox);
                rzu3 = new RZu(pic_Rzu3, pic_chessboard, p_array, pnl_BlackDied, 259, 367, stackPoint, stackPictureBox);
                rzu4 = new RZu(pic_Rzu4, pic_chessboard, p_array, pnl_BlackDied, 367, 367, stackPoint, stackPictureBox);
                rzu5 = new RZu(pic_Rzu5, pic_chessboard, p_array, pnl_BlackDied, 475, 367, stackPoint, stackPictureBox);
                rpao1 = new RPao(pic_Rpao1, pic_chessboard, p_array, pnl_BlackDied, 97, 421, stackPoint, stackPictureBox);
                rpao2 = new RPao(pic_Rpao2, pic_chessboard, p_array, pnl_BlackDied, 421, 421, stackPoint, stackPictureBox);
                rju1 = new RJu(pic_Rju1, pic_chessboard, p_array, pnl_BlackDied, 43, 529, stackPoint, stackPictureBox);
                rju2 = new RJu(pic_Rju2, pic_chessboard, p_array, pnl_BlackDied, 475, 529, stackPoint, stackPictureBox);
                rma1 = new RMa(pic_Rma1, pic_chessboard, p_array, pnl_BlackDied, 97, 529, stackPoint, stackPictureBox);
                rma2 = new RMa(pic_Rma2, pic_chessboard, p_array, pnl_BlackDied, 421, 529, stackPoint, stackPictureBox);
                rxiang1 = new RXiang(pic_Rxiang1, pic_chessboard, p_array, pnl_BlackDied, 151, 529, stackPoint, stackPictureBox);
                rxiang2 = new RXiang(pic_Rxiang2, pic_chessboard, p_array, pnl_BlackDied, 367, 529, stackPoint, stackPictureBox);
                rshi1 = new RShi(pic_Rshi1, pic_chessboard, p_array, pnl_BlackDied, 205, 529, stackPoint, stackPictureBox);
                rshi2 = new RShi(pic_Rshi2, pic_chessboard, p_array, pnl_BlackDied, 313, 529, stackPoint, stackPictureBox);
                rjiang = new RJiang(pic_Rjiang, pic_chessboard, p_array, pnl_BlackDied, 259, 529, stackPoint, stackPictureBox);

                r_count = (int)formatter.Deserialize(stream); b_count = (int)formatter.Deserialize(stream);
                lab_bCount.Text = b_count.ToString();
                lab_rCount.Text = r_count.ToString();
                pic_chessboard.Enabled = (bool)formatter.Deserialize(stream);
                whichside = (int)formatter.Deserialize(stream);
                if (whichside == 1)
                {
                    pic_whichside.Image = Image.FromFile(@"img\whichside_red.png");
                }
                else
                {
                    pic_whichside.Image = Image.FromFile(@"img\whichside_black.png");
                }
                r_mint = (int)formatter.Deserialize(stream);
                b_mint = (int)formatter.Deserialize(stream);
                //计时器重写
                int h, m, s;
                string time;
                s = r_mint % 60;
                m = (int)r_mint / 60 % 60;
                h = (int)r_mint / 60 / 60;
                time = string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
                lab_rTime.Text = time;

                s = b_mint % 60;
                m = (int)b_mint / 60 % 60;
                h = (int)b_mint / 60 / 60;
                time = string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);

                lab_bTime.Text = time;

                p_array = new PictureBox[2, 16] { { pic_Bzu1, pic_Bzu2, pic_Bzu3, pic_Bzu4, pic_Bzu5, pic_Bpao1, pic_Bpao2, pic_Bju1, pic_Bju2, pic_Bma1, pic_Bma2, pic_Bxiang1, pic_Bxiang2, pic_Bshi1, pic_Bshi2, pic_Bjiang }, 
            { pic_Rzu1, pic_Rzu2, pic_Rzu3, pic_Rzu4, pic_Rzu5, pic_Rpao1, pic_Rpao2, pic_Rju1, pic_Rju2, pic_Rma1, pic_Rma2, pic_Rxiang1, pic_Rxiang2, pic_Rshi1, pic_Rshi2, pic_Rjiang } };
                //此数组纪录下所有棋,用于判断坐标 p_array[0,x]为黑棋 p_array[1,x]为红棋
                /*
                ChessPieces[,] cp_array = new ChessPieces[2, 16] { { bzu1, bzu2, bzu3, bzu4, bzu5, bpao1, bpao2, bju1, bju2, bma1, bma2, bxiang1, bxiang2, bshi1, bshi2, bjiang }, 
                { rzu1, rzu2, rzu3, rzu4, rzu5, rpao1, rpao2, rju1, rju2, rma1, rma2, rxiang1, rxiang2, rshi1, rshi2, rjiang } };
                //此数组纪录下所有棋,用于判断坐标 cp_array[0,x]为黑棋 cp_array[1,x]为红棋
                */

                for (int i = 0; i != 2; i++)//将所有棋子的picturebox变为圆形
                {
                    for (int j = 0; j != 16; j++)
                    { makePicRound(p_array[i, j]); }
                }

                int ind;
                for (int i = 0; i != 2; i++)
                {
                    for (int j = 0; j != 16; j++)
                    {
                        ind = (int)formatter.Deserialize(stream);
                        if (ind == 1) { p_array[i, j].Parent = pic_chessboard; }
                        else
                        {
                            string sn = p_array[i, j].Name;
                            if (sn[4] == 'B')
                                p_array[i, j].Parent = pnl_BlackDied;
                            else
                                p_array[i, j].Parent = pnl_RedDied;
                        }
                        p_array[i, j].Location = (Point)formatter.Deserialize(stream);
                    }
                }
                stream.Close();
                tmr1.Start();
                stackPoint.Clear();
                stackPictureBox.Clear();
                btn_TakeBack.Enabled = false;
                悔棋ToolStripMenuItem.Enabled = false;

                #endregion
            }
        }

        private void 储存游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sfd1.ShowDialog() == DialogResult.OK)
            {
                saveFileName = sfd1.FileName;
                formatter = new BinaryFormatter();   //定义类,主要用此类中的两个方法来实现功能
                stream = new FileStream(this.saveFileName, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, r_count);
                formatter.Serialize(stream, b_count);
                formatter.Serialize(stream, pic_chessboard.Enabled);
                formatter.Serialize(stream, whichside);
                formatter.Serialize(stream, r_mint);
                formatter.Serialize(stream, b_mint);
                for (int i = 0; i != 2; i++)
                {
                    for (int j = 0; j != 16; j++)
                    {
                        if (p_array[i, j].Parent == pic_chessboard)
                        {
                            formatter.Serialize(stream, 1);//序列化1 表示该棋子活着
                        }
                        else
                        {
                            formatter.Serialize(stream, 0);//序列化0 表示该棋子死了 
                        }
                        formatter.Serialize(stream, p_array[i, j].Location);
                    }
                }

                stream.Close();
            }
        }
        private void 载入游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                openFileName = ofd1.FileName;
                formatter = new BinaryFormatter();
                stream = new FileStream(this.openFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                #region  重构棋盘
                //初始化黑棋,创建父类

                bzu1 = new BZu(pic_Bzu1, pic_chessboard, p_array, pnl_RedDied, 43, 205, stackPoint, stackPictureBox);
                bzu2 = new BZu(pic_Bzu2, pic_chessboard, p_array, pnl_RedDied, 151, 205, stackPoint, stackPictureBox);
                bzu3 = new BZu(pic_Bzu3, pic_chessboard, p_array, pnl_RedDied, 259, 205, stackPoint, stackPictureBox);
                bzu4 = new BZu(pic_Bzu4, pic_chessboard, p_array, pnl_RedDied, 367, 205, stackPoint, stackPictureBox);
                bzu5 = new BZu(pic_Bzu5, pic_chessboard, p_array, pnl_RedDied, 475, 205, stackPoint, stackPictureBox);
                bpao1 = new BPao(pic_Bpao1, pic_chessboard, p_array, pnl_RedDied, 97, 151, stackPoint, stackPictureBox);
                bpao2 = new BPao(pic_Bpao2, pic_chessboard, p_array, pnl_RedDied, 421, 151, stackPoint, stackPictureBox);
                bju1 = new BJu(pic_Bju1, pic_chessboard, p_array, pnl_RedDied, 43, 43, stackPoint, stackPictureBox);
                bju2 = new BJu(pic_Bju2, pic_chessboard, p_array, pnl_RedDied, 475, 43, stackPoint, stackPictureBox);
                bma1 = new BMa(pic_Bma1, pic_chessboard, p_array, pnl_RedDied, 97, 43, stackPoint, stackPictureBox);
                bma2 = new BMa(pic_Bma2, pic_chessboard, p_array, pnl_RedDied, 421, 43, stackPoint, stackPictureBox);
                bxiang1 = new BXiang(pic_Bxiang1, pic_chessboard, p_array, pnl_RedDied, 151, 43, stackPoint, stackPictureBox);
                bxiang2 = new BXiang(pic_Bxiang2, pic_chessboard, p_array, pnl_RedDied, 367, 43, stackPoint, stackPictureBox);
                bshi1 = new BShi(pic_Bshi1, pic_chessboard, p_array, pnl_RedDied, 205, 43, stackPoint, stackPictureBox);
                bshi2 = new BShi(pic_Bshi2, pic_chessboard, p_array, pnl_RedDied, 313, 43, stackPoint, stackPictureBox);
                bjiang = new BJiang(pic_Bjiang, pic_chessboard, p_array, pnl_RedDied, 259, 43, stackPoint, stackPictureBox);

                //初始化红棋,创建父类
                rzu1 = new RZu(pic_Rzu1, pic_chessboard, p_array, pnl_BlackDied, 43, 367, stackPoint, stackPictureBox);
                rzu2 = new RZu(pic_Rzu2, pic_chessboard, p_array, pnl_BlackDied, 151, 367, stackPoint, stackPictureBox);
                rzu3 = new RZu(pic_Rzu3, pic_chessboard, p_array, pnl_BlackDied, 259, 367, stackPoint, stackPictureBox);
                rzu4 = new RZu(pic_Rzu4, pic_chessboard, p_array, pnl_BlackDied, 367, 367, stackPoint, stackPictureBox);
                rzu5 = new RZu(pic_Rzu5, pic_chessboard, p_array, pnl_BlackDied, 475, 367, stackPoint, stackPictureBox);
                rpao1 = new RPao(pic_Rpao1, pic_chessboard, p_array, pnl_BlackDied, 97, 421, stackPoint, stackPictureBox);
                rpao2 = new RPao(pic_Rpao2, pic_chessboard, p_array, pnl_BlackDied, 421, 421, stackPoint, stackPictureBox);
                rju1 = new RJu(pic_Rju1, pic_chessboard, p_array, pnl_BlackDied, 43, 529, stackPoint, stackPictureBox);
                rju2 = new RJu(pic_Rju2, pic_chessboard, p_array, pnl_BlackDied, 475, 529, stackPoint, stackPictureBox);
                rma1 = new RMa(pic_Rma1, pic_chessboard, p_array, pnl_BlackDied, 97, 529, stackPoint, stackPictureBox);
                rma2 = new RMa(pic_Rma2, pic_chessboard, p_array, pnl_BlackDied, 421, 529, stackPoint, stackPictureBox);
                rxiang1 = new RXiang(pic_Rxiang1, pic_chessboard, p_array, pnl_BlackDied, 151, 529, stackPoint, stackPictureBox);
                rxiang2 = new RXiang(pic_Rxiang2, pic_chessboard, p_array, pnl_BlackDied, 367, 529, stackPoint, stackPictureBox);
                rshi1 = new RShi(pic_Rshi1, pic_chessboard, p_array, pnl_BlackDied, 205, 529, stackPoint, stackPictureBox);
                rshi2 = new RShi(pic_Rshi2, pic_chessboard, p_array, pnl_BlackDied, 313, 529, stackPoint, stackPictureBox);
                rjiang = new RJiang(pic_Rjiang, pic_chessboard, p_array, pnl_BlackDied, 259, 529, stackPoint, stackPictureBox);

                r_count = (int)formatter.Deserialize(stream); b_count = (int)formatter.Deserialize(stream);
                lab_bCount.Text = b_count.ToString();
                lab_rCount.Text = r_count.ToString();
                pic_chessboard.Enabled = (bool)formatter.Deserialize(stream);
                whichside = (int)formatter.Deserialize(stream);
                if (whichside == 1)
                {
                    pic_whichside.Image = Image.FromFile(@"img\whichside_red.png");
                }
                else
                {
                    pic_whichside.Image = Image.FromFile(@"img\whichside_black.png");
                }
                r_mint = (int)formatter.Deserialize(stream);
                b_mint = (int)formatter.Deserialize(stream);
                //计时器重写
                int h, m, s;
                string time;
                s = r_mint % 60;
                m = (int)r_mint / 60 % 60;
                h = (int)r_mint / 60 / 60;
                time = string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
                lab_rTime.Text = time;

                s = b_mint % 60;
                m = (int)b_mint / 60 % 60;
                h = (int)b_mint / 60 / 60;
                time = string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);

                lab_bTime.Text = time;

                p_array = new PictureBox[2, 16] { { pic_Bzu1, pic_Bzu2, pic_Bzu3, pic_Bzu4, pic_Bzu5, pic_Bpao1, pic_Bpao2, pic_Bju1, pic_Bju2, pic_Bma1, pic_Bma2, pic_Bxiang1, pic_Bxiang2, pic_Bshi1, pic_Bshi2, pic_Bjiang }, 
            { pic_Rzu1, pic_Rzu2, pic_Rzu3, pic_Rzu4, pic_Rzu5, pic_Rpao1, pic_Rpao2, pic_Rju1, pic_Rju2, pic_Rma1, pic_Rma2, pic_Rxiang1, pic_Rxiang2, pic_Rshi1, pic_Rshi2, pic_Rjiang } };
                //此数组纪录下所有棋,用于判断坐标 p_array[0,x]为黑棋 p_array[1,x]为红棋
                /*
                ChessPieces[,] cp_array = new ChessPieces[2, 16] { { bzu1, bzu2, bzu3, bzu4, bzu5, bpao1, bpao2, bju1, bju2, bma1, bma2, bxiang1, bxiang2, bshi1, bshi2, bjiang }, 
                { rzu1, rzu2, rzu3, rzu4, rzu5, rpao1, rpao2, rju1, rju2, rma1, rma2, rxiang1, rxiang2, rshi1, rshi2, rjiang } };
                //此数组纪录下所有棋,用于判断坐标 cp_array[0,x]为黑棋 cp_array[1,x]为红棋
                */

                for (int i = 0; i != 2; i++)//将所有棋子的picturebox变为圆形
                {
                    for (int j = 0; j != 16; j++)
                    { makePicRound(p_array[i, j]); }
                }

                int ind;
                for (int i = 0; i != 2; i++)
                {
                    for (int j = 0; j != 16; j++)
                    {
                        ind = (int)formatter.Deserialize(stream);
                        if (ind == 1) { p_array[i, j].Parent = pic_chessboard; }
                        else
                        {
                            string sn = p_array[i, j].Name;
                            if (sn[4] == 'B')
                                p_array[i, j].Parent = pnl_BlackDied;
                            else
                                p_array[i, j].Parent = pnl_RedDied;
                        }
                        p_array[i, j].Location = (Point)formatter.Deserialize(stream);
                    }
                }
                stream.Close();
                tmr1.Start();
                stackPoint.Clear();
                stackPictureBox.Clear();
                btn_TakeBack.Enabled = false;
                悔棋ToolStripMenuItem.Enabled = false;

                #endregion
            }
        }

        #region 音量控制
        private void 音量ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wmp_background.settings.volume += 10;
        }

        private void 音量ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            wmp_background.settings.volume -= 10;
        }

        private void btn_volumeUp_Click(object sender, EventArgs e)
        {
            wmp_background.settings.volume += 10;
        }

        private void btn_volumeDown_Click(object sender, EventArgs e)
        {
            wmp_background.settings.volume -= 10;
        }
        #endregion

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}