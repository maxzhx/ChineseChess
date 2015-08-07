using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
    #region base类
    [Serializable()]
    public class ChessPieces
    {
        public bool canmove = false;
        public Stack<Point> stackP;
        public Stack<string> stackPB;
        public PictureBox pic;
        public Panel deadPanel;
        //Ptemp用于储存路径 暂时不用
        //public Point[] Ptemp = new Point[100];
        public int x_temp;
        public int y_temp;


        public PictureBox[,] p_array;//此数组纪录下所有棋,用于判断坐标 p_array[0,x]为黑棋 p_array[1,x]为红棋
        //private int counter = 1;

        public ChessPieces(PictureBox p)
        {
            pic = p;
        }

        public ChessPieces(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb)
        {
            pic = p;
            pic.Parent = pa;
            p_array = arr;
            deadPanel = dp;
            x_temp = x;
            y_temp = y;
            pic.Location = new Point(x, y);
            stackP = sp;
            stackPB = spb;
        }

        public virtual bool move() { return false; }

        public int getX() { return pic.Location.X; }//返回棋子当前的x坐标
        public int getY() { return pic.Location.Y; }//返回棋子当前的y坐标

        public void getTemp()//用于取得MouseDown时 棋子的坐标
        {
            x_temp = pic.Location.X;
            y_temp = pic.Location.Y;
        }

        public void moveto(int x1, int y1) { pic.Location = new Point(x1, y1); }//将棋子的位置移动到(x1,y1)
        public void goBack()
        {
            if (stackP.Count != 0)
            {
                stackP.Pop();
                stackPB.Pop();
            }
            pic.Location = new Point(x_temp, y_temp);
        }//使棋子返回MouseDown时的位置 用于不符合走棋规则时
        public void showPiece() { pic.Visible = true; }

        public void killRed(PictureBox pb)
        {
            stackP.Push(pb.Location);
            stackPB.Push(pb.Name);
            pb.Parent = deadPanel;
            for (int i = 0; i != 3; i++)
            {
                for (int j = 0; j != 5; j++)
                {
                    if (isRed(j * 54, i * 54) == null)
                    {
                        pb.Location = new Point(j * 54, i * 54);
                        return;
                    }
                }
            }
        }

        public void killBlack(PictureBox pb)
        {
            stackP.Push(pb.Location);
            stackPB.Push(pb.Name);
            pb.Parent = deadPanel;
            for (int i = 0; i != 3; i++)
            {
                for (int j = 0; j != 5; j++)
                {
                    if (isBlack(j * 54, i * 54) == null)
                    {
                        pb.Location = new Point(j * 54, i * 54);
                        return;
                    }
                }
            }
        }

        public PictureBox isRed(int x1, int y1)//判断此格是否有其他的红棋 若有则将在此格的红棋返回
        {
            for (int i = 0; i <= 15; i++)
            {
                if (pic.Name != p_array[1, i].Name)
                {
                    if (p_array[1, i].Location.X == x1 && p_array[1, i].Location.Y == y1)
                    { return p_array[1, i]; }
                }
            }
            return null;
        }

        public PictureBox isBlack(int x1, int y1)//判断此格是否有其他的黑棋 若有则将在此格的黑棋返回
        {
            for (int i = 0; i <= 15; i++)
            {
                if (pic.Name != p_array[0, i].Name)
                {
                    if (p_array[0, i].Location.X == x1 && p_array[0, i].Location.Y == y1)
                    { return p_array[0, i]; }
                }
            }
            return null;
        }
    }

    #endregion

    #region 黑卒类
    public class BZu : ChessPieces
    {
        public BZu(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }

        public void MoveDown() { pic.Location = new Point(x_temp, y_temp + 54); }//向下移动
        public void MoveUp() { pic.Location = new Point(x_temp, y_temp - 54); }//向上移动
        public void MoveLeft() { pic.Location = new Point(x_temp - 54, y_temp); }//向左移动
        public void MoveRight() { pic.Location = new Point(x_temp + 54, y_temp); }//向右移动

        public override bool move()//用于判断棋子的移动是否符合规则 在MouseUp中调用
        {
            if (getX() > 0 && getX() < 509 && getY() > 0 && getY() < 549)
            {
                if (y_temp >= 313)//是否过河
                {
                    if (Math.Abs(getX() - x_temp) <= 20 && Math.Abs(getY() - (y_temp + 54)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                    {
                        if (isBlack(x_temp, y_temp + 54) == null)//如果此位置没有其他黑棋
                        {
                            MoveDown();//往下走一步
                            if (isRed(x_temp, y_temp + 54) != null)
                            {
                                killRed(isRed(x_temp, y_temp + 54)); //判断此位置是否有红棋,有则吃掉

                            }
                            return true;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else if (Math.Abs(getY() - y_temp) <= 20 && Math.Abs(getX() - (x_temp + 54)) <= 20)//同上 在右边一个
                    {
                        if (isBlack(x_temp + 54, y_temp) == null)//如果此位置没有其他黑棋
                        {
                            MoveRight();//向右走一步
                            if (isRed(x_temp + 54, y_temp) != null) { killRed(isRed(x_temp + 54, y_temp)); }//判断此位置是否有红棋,有则吃掉
                            return true; ;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else if (Math.Abs(getY() - y_temp) <= 20 && Math.Abs(getX() - (x_temp - 54)) <= 20)
                    {
                        if (isBlack(x_temp - 54, y_temp) == null)//如果此位置没有其他黑棋
                        {
                            MoveLeft();//向左走一步
                            if (isRed(x_temp - 54, y_temp) != null) { killRed(isRed(x_temp - 54, y_temp)); }//判断此位置是否有红棋,有则吃掉
                            return true; ;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else
                    {
                        goBack();//不符合走棋规则,位置不变
                        return false;
                    }
                }
                else
                {
                    if (Math.Abs(getX() - x_temp) <= 20 && Math.Abs(getY() - (y_temp + 54)) <= 20)
                    {
                        if (isBlack(x_temp, y_temp + 54) == null)//如果此位置没有其他黑棋
                        {
                            MoveDown();//往下走一步
                            if (isRed(x_temp, y_temp + 54) != null) { killRed(isRed(x_temp, y_temp + 54)); }//判断此位置是否有红棋,有则吃掉
                            return true;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else
                    {
                        goBack();//不符合走棋规则,位置不变
                        return false;
                    }
                }
            }
            else { goBack(); return false; }
        }
    }
    #endregion

    #region 红卒类
    public class RZu : ChessPieces
    {
        public RZu(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }

        public void MoveDown() { pic.Location = new Point(x_temp, y_temp + 54); }//向下移动
        public void MoveUp() { pic.Location = new Point(x_temp, y_temp - 54); }//向上移动
        public void MoveLeft() { pic.Location = new Point(x_temp - 54, y_temp); }//向左移动
        public void MoveRight() { pic.Location = new Point(x_temp + 54, y_temp); }//向右移动

        public override bool move()//用于判断棋子的移动是否符合规则 在MouseUp中调用
        {
            if (getX() > 0 && getX() < 509 && getY() > 0 && getY() < 549)
            {
                if (y_temp <= 259)//是否过河
                {
                    if (Math.Abs(getX() - x_temp) <= 20 && Math.Abs(getY() - (y_temp - 54)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                    {
                        if (isRed(x_temp, y_temp - 54) == null)//如果此位置没有其他红棋
                        {
                            MoveUp();//往上走一步
                            if (isBlack(x_temp, y_temp - 54) != null) { killBlack(isBlack(x_temp, y_temp - 54)); }//判断此位置是否有黑棋,有则吃掉
                            return true;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else if (Math.Abs(getY() - y_temp) <= 20 && Math.Abs(getX() - (x_temp + 54)) <= 20)//同上 在右边一个
                    {
                        if (isRed(x_temp + 54, y_temp) == null)//如果此位置没有其他红棋
                        {
                            MoveRight();//向右走一步
                            if (isBlack(x_temp + 54, y_temp) != null) { killBlack(isBlack(x_temp + 54, y_temp)); }//判断此位置是否有黑棋,有则吃掉
                            return true; ;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else if (Math.Abs(getY() - y_temp) <= 20 && Math.Abs(getX() - (x_temp - 54)) <= 20)
                    {
                        if (isRed(x_temp - 54, y_temp) == null)//如果此位置没有其他红棋
                        {
                            MoveLeft();//向左走一步
                            if (isBlack(x_temp - 54, y_temp) != null) { killBlack(isBlack(x_temp - 54, y_temp)); }//判断此位置是否有黑棋,有则吃掉
                            return true; ;//走棋成功
                        }
                        else { goBack(); return false; }

                    }
                    else
                    {
                        goBack();//不符合走棋规则,位置不变
                        return false;
                    }
                }
                else
                {
                    if (Math.Abs(getX() - x_temp) <= 20 && Math.Abs(getY() - (y_temp - 54)) <= 20)
                    {
                        if (isRed(x_temp, y_temp - 54) == null)//如果此位置没有其他红棋
                        {
                            MoveUp();//往上走一步
                            if (isBlack(x_temp, y_temp - 54) != null) { killBlack(isBlack(x_temp, y_temp - 54)); }//判断此位置是否有黑棋,有则吃掉
                            return true;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else
                    {
                        goBack();//不符合走棋规则,位置不变
                        return false;
                    }
                }
            }
            else { goBack(); return false; }
        }
    }
    #endregion

    #region 黑炮类
    public class BPao : ChessPieces
    {
        int xGrid = 0;  // 水平方向移动的格子数
        int yGrid = 0;  // 垂直方向移动的格子数
        int cntChess = 0;   // 初始位置与落子位置之间的棋子数
        public BPao(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }

        public void MoveDown() { pic.Location = new Point(x_temp, y_temp + 54 * yGrid); }//向下移动
        public void MoveUp() { pic.Location = new Point(x_temp, y_temp + 54 * yGrid); }//向上移动
        public void MoveLeft() { pic.Location = new Point(x_temp + 54 * xGrid, y_temp); }//向左移动
        public void MoveRight() { pic.Location = new Point(x_temp + 54 * xGrid, y_temp); }//向右移动

        public override bool move()//用于判断棋子的移动是否符合规则 在MouseUp中调用
        {
            xGrid = 0;
            yGrid = 0;
            cntChess = 0;
            if (getX() > 0 && getX() < 509 && getY() > 0 && getY() < 549)
            {
                xGrid = (int)(Math.Round((getX() - (x_temp)) / 54.0, 0));
                yGrid = (int)(Math.Round((getY() - (y_temp)) / 54.0, 0));
                if (xGrid == 0 && yGrid > 0)//如果往下走移动
                {
                    if (isBlack(x_temp, y_temp + 54 * yGrid) == null)//如果此位置没有其他黑棋
                    {
                        for (int i = 1; i < Math.Abs(yGrid); i++)
                        {
                            if (isBlack(x_temp, y_temp + 54 * i) != null ||
                                isRed(x_temp, y_temp + 54 * i) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isRed(x_temp, y_temp + 54 * yGrid) == null)
                            {
                                MoveDown();//往下走移动
                                return true;//走棋成功
                            }
                        }
                        else if (cntChess == 1)
                        {
                            if (isRed(x_temp, y_temp + 54 * yGrid) != null) //判断此位置是否有红棋,有则吃掉
                            {
                                MoveDown();//往下走移动
                                killRed(isRed(x_temp, y_temp + 54 * yGrid));
                                return true;//走棋成功
                            }
                        }
                        else { goBack(); return false; }
                        goBack(); return false;
                    }
                    else { goBack(); return false; }
                }
                else if (yGrid == 0 && xGrid > 0)//如果向右移动
                {
                    if (isBlack(x_temp + 54 * xGrid, y_temp) == null)//如果此位置没有其他黑棋
                    {
                        for (int i = 1; i < Math.Abs(xGrid); i++)
                        {
                            if (isBlack(x_temp + 54 * i, y_temp) != null ||
                                isRed(x_temp + 54 * i, y_temp) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isRed(x_temp + 54 * xGrid, y_temp) == null)
                            {
                                MoveRight();//向右移动
                                return true;//走棋成功
                            }
                        }
                        else if (cntChess == 1)
                        {
                            if (isRed(x_temp + 54 * xGrid, y_temp) != null) //判断此位置是否有红棋,有则吃掉
                            {
                                MoveRight();//向右移动
                                killRed(isRed(x_temp + 54 * xGrid, y_temp));
                                return true;//走棋成功
                            }
                        }
                        else { goBack(); return false; }
                        goBack(); return false;
                    }
                    else { goBack(); return false; }
                }
                else if (yGrid == 0 && xGrid < 0)//向左移动
                {
                    //xGrid--;
                    if (isBlack(x_temp + 54 * xGrid, y_temp) == null)//如果此位置没有其他黑棋
                    {
                        for (int i = 1; i < Math.Abs(xGrid); i++)
                        {
                            if (isBlack(x_temp - 54 * i, y_temp) != null ||
                                isRed(x_temp - 54 * i, y_temp) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isRed(x_temp + 54 * xGrid, y_temp) == null)
                            {
                                MoveLeft();//向右移动
                                return true;//走棋成功
                            }
                        }
                        else if (cntChess == 1)
                        {
                            if (isRed(x_temp + 54 * xGrid, y_temp) != null) //判断此位置是否有红棋,有则吃掉
                            {
                                MoveLeft();//向右移动
                                killRed(isRed(x_temp + 54 * xGrid, y_temp));
                                return true;//走棋成功
                            }
                        }
                        else { goBack(); return false; }
                        goBack(); return false;
                    }
                    else { goBack(); return false; }
                }
                else if (xGrid == 0 && yGrid < 0) //向上移动
                {
                    //yGrid--;
                    if (isBlack(x_temp, y_temp + 54 * yGrid) == null)//如果此位置没有其他黑棋
                    {
                        for (int i = 1; i < Math.Abs(yGrid); i++)
                        {
                            if (isBlack(x_temp, y_temp - 54 * i) != null ||
                                isRed(x_temp, y_temp - 54 * i) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isRed(x_temp, y_temp + 54 * yGrid) == null)
                            {
                                MoveUp();//向上移动
                                return true;//走棋成功
                            }
                        }
                        else if (cntChess == 1)
                        {
                            if (isRed(x_temp, y_temp + 54 * yGrid) != null) //判断此位置是否有红棋,有则吃掉
                            {
                                MoveUp();//向上移动
                                killRed(isRed(x_temp, y_temp + 54 * yGrid));
                                return true;//走棋成功
                            }
                        }
                        else { goBack(); return false; }
                        goBack(); return false;
                    }
                    else { goBack(); return false; }
                }
                else
                {
                    goBack();//不符合走棋规则,位置不变
                    return false;
                }
            }
            else
            {
                goBack(); return false;
            }
        }
    }
    #endregion

    #region 红炮类
    public class RPao : ChessPieces
    {
        int xGrid = 0;  // 水平方向移动的格子数
        int yGrid = 0;  // 垂直方向移动的格子数
        int cntChess = 0;   // 初始位置与落子位置之间的棋子数

        public RPao(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }
        public void MoveDown() { pic.Location = new Point(x_temp, y_temp + 54 * yGrid); }//向下移动
        public void MoveUp() { pic.Location = new Point(x_temp, y_temp + 54 * yGrid); }//向上移动
        public void MoveLeft() { pic.Location = new Point(x_temp + 54 * xGrid, y_temp); }//向左移动
        public void MoveRight() { pic.Location = new Point(x_temp + 54 * xGrid, y_temp); }//向右移动

        public override bool move()//用于判断棋子的移动是否符合规则 在MouseUp中调用
        {
            xGrid = 0;
            yGrid = 0;
            cntChess = 0;
            if (getX() > 0 && getX() < 509 && getY() > 0 && getY() < 549)
            {
                xGrid = (int)(Math.Round((getX() - (x_temp)) / 54.0, 0));
                yGrid = (int)(Math.Round((getY() - (y_temp)) / 54.0, 0));
                if (xGrid == 0 && yGrid > 0)//如果往下走移动
                {
                    if (isRed(x_temp, y_temp + 54 * yGrid) == null)//如果此位置没有其他红棋
                    {
                        for (int i = 1; i < Math.Abs(yGrid); i++)
                        {
                            if (isBlack(x_temp, y_temp + 54 * i) != null ||
                                isRed(x_temp, y_temp + 54 * i) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isBlack(x_temp, y_temp + 54 * yGrid) == null)
                            {
                                MoveDown();//往下走移动
                                return true;//走棋成功
                            }
                        }
                        else if (cntChess == 1)
                        {
                            if (isBlack(x_temp, y_temp + 54 * yGrid) != null) //判断此位置是否有黑棋,有则吃掉
                            {
                                MoveDown();//往下走移动
                                killBlack(isBlack(x_temp, y_temp + 54 * yGrid));
                                return true;//走棋成功
                            }
                        }
                        else { goBack(); return false; }
                        goBack(); return false;
                    }
                    else { goBack(); return false; }
                }
                else if (yGrid == 0 && xGrid > 0)//如果向右移动
                {
                    if (isRed(x_temp + 54 * xGrid, y_temp) == null)//如果此位置没有其他红棋
                    {
                        for (int i = 1; i < Math.Abs(xGrid); i++)
                        {
                            if (isBlack(x_temp + 54 * i, y_temp) != null ||
                                isRed(x_temp + 54 * i, y_temp) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isBlack(x_temp + 54 * xGrid, y_temp) == null)
                            {
                                MoveRight();//向右移动
                                return true;//走棋成功
                            }
                        }
                        else if (cntChess == 1)
                        {
                            if (isBlack(x_temp + 54 * xGrid, y_temp) != null) //判断此位置是否有黑棋,有则吃掉
                            {
                                MoveRight();//向右移动
                                killBlack(isBlack(x_temp + 54 * xGrid, y_temp));
                                return true;//走棋成功
                            }
                        }
                        else { goBack(); return false; }
                        goBack(); return false;
                    }
                    else { goBack(); return false; }
                }
                else if (yGrid == 0 && xGrid < 0)//向左移动
                {
                    //xGrid--;
                    if (isRed(x_temp + 54 * xGrid, y_temp) == null)//如果此位置没有其他红棋
                    {
                        for (int i = 1; i < Math.Abs(xGrid); i++)
                        {
                            if (isBlack(x_temp - 54 * i, y_temp) != null ||
                                isRed(x_temp - 54 * i, y_temp) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isBlack(x_temp + 54 * xGrid, y_temp) == null)
                            {
                                MoveLeft();//向右移动
                                return true;//走棋成功
                            }
                        }
                        else if (cntChess == 1)
                        {
                            if (isBlack(x_temp + 54 * xGrid, y_temp) != null) //判断此位置是否有黑棋,有则吃掉
                            {
                                MoveLeft();//向右移动
                                killBlack(isBlack(x_temp + 54 * xGrid, y_temp));
                                return true;//走棋成功
                            }
                        }
                        else { goBack(); return false; }
                        goBack(); return false;
                    }
                    else { goBack(); return false; }
                }
                else if (xGrid == 0 && yGrid < 0) //向上移动
                {
                    //yGrid--;
                    if (isRed(x_temp, y_temp + 54 * yGrid) == null)//如果此位置没有其他红棋
                    {
                        for (int i = 1; i < Math.Abs(yGrid); i++)
                        {
                            if (isBlack(x_temp, y_temp - 54 * i) != null ||
                                isRed(x_temp, y_temp - 54 * i) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isBlack(x_temp, y_temp + 54 * yGrid) == null)
                            {
                                MoveUp();//向上移动
                                return true;//走棋成功
                            }
                        }
                        else if (cntChess == 1)
                        {
                            if (isBlack(x_temp, y_temp + 54 * yGrid) != null) //判断此位置是否有黑棋,有则吃掉
                            {
                                MoveUp();//向上移动
                                killBlack(isBlack(x_temp, y_temp + 54 * yGrid));
                                return true;//走棋成功
                            }
                        }
                        else { goBack(); return false; }
                        goBack(); return false;
                    }
                    else { goBack(); return false; }
                }
                else
                {
                    goBack();//不符合走棋规则,位置不变
                    return false;
                }
            }
            else
            {
                goBack(); return false;
            }
        }
    }
    #endregion

    #region 黑车类
    public class BJu : ChessPieces
    {
        int xGrid = 0;  // 水平方向移动的格子数
        int yGrid = 0;  // 垂直方向移动的格子数
        int cntChess = 0;   // 初始位置与落子位置之间的棋子数

        public BJu(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }
        public void MoveDown() { pic.Location = new Point(x_temp, y_temp + 54 * yGrid); }//向下移动
        public void MoveUp() { pic.Location = new Point(x_temp, y_temp + 54 * yGrid); }//向上移动
        public void MoveLeft() { pic.Location = new Point(x_temp + 54 * xGrid, y_temp); }//向左移动
        public void MoveRight() { pic.Location = new Point(x_temp + 54 * xGrid, y_temp); }//向右移动

        public override bool move()//用于判断棋子的移动是否符合规则 在MouseUp中调用
        {
            xGrid = 0;
            yGrid = 0;
            cntChess = 0;
            if (getX() > 0 && getX() < 509 && getY() > 0 && getY() < 549)
            {
                xGrid = (int)(Math.Round((getX() - (x_temp)) / 54.0, 0));
                yGrid = (int)(Math.Round((getY() - (y_temp)) / 54.0, 0));
                if (xGrid == 0 && yGrid > 0)//如果往下走移动
                {
                    if (isBlack(x_temp, y_temp + 54 * yGrid) == null)//如果此位置没有其他黑棋
                    {
                        for (int i = 1; i < Math.Abs(yGrid); i++)
                        {
                            if (isBlack(x_temp, y_temp + 54 * i) != null ||
                                isRed(x_temp, y_temp + 54 * i) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isRed(x_temp, y_temp + 54 * yGrid) != null)
                            {
                                killRed(isRed(x_temp, y_temp + 54 * yGrid));
                            }
                            MoveDown();//往下走移动
                            return true;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }


                }
                else if (yGrid == 0 && xGrid > 0)//如果向右移动
                {
                    if (isBlack(x_temp + 54 * xGrid, y_temp) == null)//如果此位置没有其他黑棋
                    {
                        for (int i = 1; i < Math.Abs(xGrid); i++)
                        {
                            if (isBlack(x_temp + 54 * i, y_temp) != null ||
                                isRed(x_temp + 54 * i, y_temp) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isRed(x_temp + 54 * xGrid, y_temp) != null)
                            {
                                killRed(isRed(x_temp + 54 * xGrid, y_temp));
                            }
                            MoveRight();//向右移动
                            return true;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (yGrid == 0 && xGrid < 0)//向左移动
                {
                    if (isBlack(x_temp + 54 * xGrid, y_temp) == null)//如果此位置没有其他黑棋
                    {
                        for (int i = 1; i < Math.Abs(xGrid); i++)
                        {
                            if (isBlack(x_temp - 54 * i, y_temp) != null ||
                                isRed(x_temp - 54 * i, y_temp) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isRed(x_temp + 54 * xGrid, y_temp) != null)
                            {
                                killRed(isRed(x_temp + 54 * xGrid, y_temp));
                            }
                            MoveLeft();//向右移动
                            return true;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (xGrid == 0 && yGrid < 0) //向上移动
                {
                    if (isBlack(x_temp, y_temp + 54 * yGrid) == null)//如果此位置没有其他黑棋
                    {
                        for (int i = 1; i < Math.Abs(yGrid); i++)
                        {
                            if (isBlack(x_temp, y_temp - 54 * i) != null ||
                                isRed(x_temp, y_temp - 54 * i) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isRed(x_temp, y_temp + 54 * yGrid) != null)
                            {
                                killRed(isRed(x_temp, y_temp + 54 * yGrid));
                            }
                            MoveUp();//向上移动
                            return true;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else
                {
                    goBack();//不符合走棋规则,位置不变
                    return false;
                }
            }
            else
            {
                goBack();
                return false;
            }
        }
    }
    #endregion

    #region 红车类
    public class RJu : ChessPieces
    {
        int xGrid = 0;  // 水平方向移动的格子数
        int yGrid = 0;  // 垂直方向移动的格子数
        int cntChess = 0;   // 初始位置与落子位置之间的棋子数
        public RJu(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }

        public void MoveDown() { pic.Location = new Point(x_temp, y_temp + 54 * yGrid); }//向下移动
        public void MoveUp() { pic.Location = new Point(x_temp, y_temp + 54 * yGrid); }//向上移动
        public void MoveLeft() { pic.Location = new Point(x_temp + 54 * xGrid, y_temp); }//向左移动
        public void MoveRight() { pic.Location = new Point(x_temp + 54 * xGrid, y_temp); }//向右移动

        public override bool move()//用于判断棋子的移动是否符合规则 在MouseUp中调用
        {
            xGrid = 0;
            yGrid = 0;
            cntChess = 0;
            if (getX() > 0 && getX() < 509 && getY() > 0 && getY() < 549)
            {
                xGrid = (int)(Math.Round((getX() - (x_temp)) / 54.0, 0));
                yGrid = (int)(Math.Round((getY() - (y_temp)) / 54.0, 0));
                if (xGrid == 0 && yGrid > 0)//如果往下走移动
                {
                    if (isRed(x_temp, y_temp + 54 * yGrid) == null)//如果此位置没有其他红棋
                    {
                        for (int i = 1; i < Math.Abs(yGrid); i++)
                        {
                            if (isBlack(x_temp, y_temp + 54 * i) != null ||
                                isRed(x_temp, y_temp + 54 * i) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isBlack(x_temp, y_temp + 54 * yGrid) != null)
                            {
                                killBlack(isBlack(x_temp, y_temp + 54 * yGrid));
                            }
                            MoveDown();//往下走移动
                            return true;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (yGrid == 0 && xGrid > 0)//如果向右移动
                {
                    if (isRed(x_temp + 54 * xGrid, y_temp) == null)//如果此位置没有其他红棋
                    {
                        for (int i = 1; i < Math.Abs(xGrid); i++)
                        {
                            if (isBlack(x_temp + 54 * i, y_temp) != null ||
                                isRed(x_temp + 54 * i, y_temp) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isBlack(x_temp + 54 * xGrid, y_temp) != null)
                            {
                                killBlack(isBlack(x_temp + 54 * xGrid, y_temp));
                            }
                            MoveRight();//向右移动
                            return true;//走棋成功
                        }

                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (yGrid == 0 && xGrid < 0)//向左移动
                {
                    if (isRed(x_temp + 54 * xGrid, y_temp) == null)//如果此位置没有其他红棋
                    {
                        for (int i = 1; i < Math.Abs(xGrid); i++)
                        {
                            if (isBlack(x_temp - 54 * i, y_temp) != null ||
                                isRed(x_temp - 54 * i, y_temp) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isBlack(x_temp + 54 * xGrid, y_temp) != null)
                            {
                                killBlack(isBlack(x_temp + 54 * xGrid, y_temp));
                            }
                            MoveLeft();//向右移动
                            return true;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (xGrid == 0 && yGrid < 0) //向上移动
                {
                    if (isRed(x_temp, y_temp + 54 * yGrid) == null)//如果此位置没有其他红棋
                    {
                        for (int i = 1; i < Math.Abs(yGrid); i++)
                        {
                            if (isBlack(x_temp, y_temp - 54 * i) != null ||
                                isRed(x_temp, y_temp - 54 * i) != null)
                            {
                                cntChess++;
                            }
                        }
                        if (cntChess == 0)
                        {
                            if (isBlack(x_temp, y_temp + 54 * yGrid) != null)
                            {
                                killBlack(isBlack(x_temp, y_temp + 54 * yGrid));
                            }
                            MoveUp();//向上移动
                            return true;//走棋成功
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else
                {
                    goBack();//不符合走棋规则,位置不变
                    return false;
                }
            }
            else
            {
                goBack();
                return false;
            }
        }
    }
    #endregion

    #region 黑马类
    public class BMa : ChessPieces
    {
        public BMa(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }
        public void movelu1() { pic.Location = new Point(x_temp - 54, y_temp - 108); }//用于走 左上 竖日字
        public void movelu2() { pic.Location = new Point(x_temp - 108, y_temp - 54); }//用于走 左上 横日字
        public void moveld1() { pic.Location = new Point(x_temp - 54, y_temp + 108); }//左下 竖日字
        public void moveld2() { pic.Location = new Point(x_temp - 108, y_temp + 54); }//左下 横日字
        public void moveru1() { pic.Location = new Point(x_temp + 54, y_temp - 108); }//右上 竖日字
        public void moveru2() { pic.Location = new Point(x_temp + 108, y_temp - 54); }//右上 横日字
        public void moverd1() { pic.Location = new Point(x_temp + 54, y_temp + 108); }//右下 竖日字
        public void moverd2() { pic.Location = new Point(x_temp + 108, y_temp + 54); }//右下 横日字
        public override bool move()
        {
            if (getX() > 0 && getX() < 509 && getY() > 0 && getY() < 549)
            {
                if (Math.Abs(getX() - (x_temp - 54)) <= 20 && Math.Abs(getY() - (y_temp - 108)) <= 20)
                {
                    if (isBlack(x_temp, y_temp - 54) == null && isRed(x_temp, y_temp - 54) == null)//判断上方是否有障碍
                    {
                        if (isBlack(x_temp - 54, y_temp - 108) == null)//判断是否有黑棋，有则退回 左上 竖日
                        {
                            movelu1();
                            if (isRed(x_temp - 54, y_temp - 108) != null)
                                killRed(isRed(x_temp - 54, y_temp - 108));
                            return true;
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp - 108)) <= 20 && Math.Abs(getY() - (y_temp - 54)) <= 20)//左上 横日
                {
                    if (isBlack(x_temp - 54, y_temp) == null && isRed(x_temp - 54, y_temp) == null)
                    {
                        if (isBlack(x_temp - 108, y_temp - 54) == null)
                        {
                            movelu2();
                            if (isRed(x_temp - 108, y_temp - 54) != null)
                                killRed(isRed(x_temp - 108, y_temp - 54));
                            return true;
                        }

                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp - 54)) <= 20 && Math.Abs(getY() - (y_temp + 108)) <= 20)//左下 竖日
                {
                    if (isBlack(x_temp, y_temp + 54) == null && isRed(x_temp, y_temp + 54) == null)
                    {
                        if (isBlack(x_temp - 54, y_temp + 108) == null)
                        {
                            moveld1();
                            if (isRed(x_temp - 54, y_temp + 108) != null)
                                killRed(isRed(x_temp - 54, y_temp + 108));
                            return true;
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp - 108)) <= 20 && Math.Abs(getY() - (y_temp + 54)) <= 20)//左下 横日
                {
                    if (isBlack(x_temp - 54, y_temp) == null && isRed(x_temp - 54, y_temp) == null)
                    {
                        if (isBlack(x_temp - 108, y_temp + 54) == null)
                        {
                            moveld2();
                            if (isRed(x_temp - 108, y_temp + 54) != null)
                                killRed(isRed(x_temp - 108, y_temp + 54));
                            return true;
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp + 54)) <= 20 && Math.Abs(getY() - (y_temp - 108)) <= 20)//右上 竖日
                {
                    if (isBlack(x_temp, y_temp - 54) == null && isRed(x_temp, y_temp - 54) == null)
                    {
                        if (isBlack(x_temp + 54, y_temp - 108) == null)
                        {
                            moveru1();
                            if (isRed(x_temp + 54, y_temp - 108) != null)
                                killRed(isRed(x_temp + 54, y_temp - 108));
                            return true;

                        }
                        else { goBack(); return false; }

                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp + 108)) <= 20 && Math.Abs(getY() - (y_temp - 54)) <= 20)//右上 横日
                {
                    if (isBlack(x_temp + 54, y_temp) == null && isRed(x_temp + 54, y_temp) == null)//马脚判断
                    {
                        if (isBlack(x_temp + 108, y_temp - 54) == null)
                        {
                            moveru2();
                            if (isRed(x_temp + 108, y_temp - 54) != null)
                                killRed(isRed(x_temp + 108, y_temp - 54));
                            return true;
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp + 54)) <= 20 && Math.Abs(getY() - (y_temp + 108)) <= 20)//右下 竖日
                {
                    if (isBlack(x_temp, y_temp + 54) == null && isRed(x_temp, y_temp + 54) == null)
                    {
                        if (isBlack(x_temp + 54, y_temp + 108) == null)
                        {
                            moverd1();
                            if (isRed(x_temp + 54, y_temp + 108) != null)
                                killRed(isRed(x_temp + 54, y_temp + 108));
                            return true;
                        }
                        else { goBack(); return false; }

                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp + 108)) <= 20 && Math.Abs(getY() - (y_temp + 54)) <= 20)//右下 横日
                {
                    if (isBlack(x_temp + 54, y_temp) == null && isRed(x_temp + 54, y_temp) == null)
                    {
                        if (isBlack(x_temp + 108, y_temp + 54) == null)
                        {
                            moverd2();
                            if (isRed(x_temp + 108, y_temp + 54) != null)
                                killRed(isRed(x_temp + 108, y_temp + 54));
                            return true;
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else { goBack(); return false; }
            }
            else { goBack(); return false; }
        }
    }

    #endregion

    #region 红马类
    public class RMa : ChessPieces
    {
        public RMa(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }
        public void movelu1() { pic.Location = new Point(x_temp - 54, y_temp - 108); }//用于走 左上 竖日字
        public void movelu2() { pic.Location = new Point(x_temp - 108, y_temp - 54); }//用于走 左上 横日字
        public void moveld1() { pic.Location = new Point(x_temp - 54, y_temp + 108); }//左下 竖日字
        public void moveld2() { pic.Location = new Point(x_temp - 108, y_temp + 54); }//左下 横日字
        public void moveru1() { pic.Location = new Point(x_temp + 54, y_temp - 108); }//右上 竖日字
        public void moveru2() { pic.Location = new Point(x_temp + 108, y_temp - 54); }//右上 横日字
        public void moverd1() { pic.Location = new Point(x_temp + 54, y_temp + 108); }//右下 竖日字
        public void moverd2() { pic.Location = new Point(x_temp + 108, y_temp + 54); }//右下 横日字
        public override bool move()
        {
            if (getX() > 0 && getX() < 509 && getY() > 0 && getY() < 549)
            {
                if (Math.Abs(getX() - (x_temp - 54)) <= 20 && Math.Abs(getY() - (y_temp - 108)) <= 20)
                {
                    if (isBlack(x_temp, y_temp - 54) == null && isRed(x_temp, y_temp - 54) == null)//判断上方是否有障碍
                    {
                        if (isRed(x_temp - 54, y_temp - 108) == null)//判断是否有黑棋，有则退回 左上 竖日
                        {
                            movelu1();
                            if (isBlack(x_temp - 54, y_temp - 108) != null)
                                killBlack(isBlack(x_temp - 54, y_temp - 108));
                            return true;
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp - 108)) <= 20 && Math.Abs(getY() - (y_temp - 54)) <= 20)//左上 横日
                {
                    if (isBlack(x_temp - 54, y_temp) == null && isRed(x_temp - 54, y_temp) == null)
                    {
                        if (isRed(x_temp - 108, y_temp - 54) == null)
                        {
                            movelu2();
                            if (isBlack(x_temp - 108, y_temp - 54) != null)
                                killBlack(isBlack(x_temp - 108, y_temp - 54));
                            return true;
                        }

                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp - 54)) <= 20 && Math.Abs(getY() - (y_temp + 108)) <= 20)//左下 竖日
                {
                    if (isBlack(x_temp, y_temp + 54) == null && isRed(x_temp, y_temp + 54) == null)
                    {
                        if (isRed(x_temp - 54, y_temp + 108) == null)
                        {
                            moveld1();
                            if (isBlack(x_temp - 54, y_temp + 108) != null)
                                killBlack(isBlack(x_temp - 54, y_temp + 108));
                            return true;
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp - 108)) <= 20 && Math.Abs(getY() - (y_temp + 54)) <= 20)//左下 横日
                {
                    if (isBlack(x_temp - 54, y_temp) == null && isRed(x_temp - 54, y_temp) == null)
                    {
                        if (isRed(x_temp - 108, y_temp + 54) == null)
                        {
                            moveld2();
                            if (isBlack(x_temp - 108, y_temp + 54) != null)
                                killBlack(isBlack(x_temp - 108, y_temp + 54));
                            return true;
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp + 54)) <= 20 && Math.Abs(getY() - (y_temp - 108)) <= 20)//右上 竖日
                {
                    if (isBlack(x_temp, y_temp - 54) == null && isRed(x_temp, y_temp - 54) == null)
                    {
                        if (isRed(x_temp + 54, y_temp - 108) == null)
                        {
                            moveru1();
                            if (isBlack(x_temp + 54, y_temp - 108) != null)
                                killBlack(isBlack(x_temp + 54, y_temp - 108));
                            return true;

                        }
                        else { goBack(); return false; }

                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp + 108)) <= 20 && Math.Abs(getY() - (y_temp - 54)) <= 20)//右上 横日
                {
                    if (isBlack(x_temp + 54, y_temp) == null && isRed(x_temp + 54, y_temp) == null)//马脚判断
                    {
                        if (isRed(x_temp + 108, y_temp - 54) == null)
                        {
                            moveru2();
                            if (isBlack(x_temp + 108, y_temp - 54) != null)
                                killBlack(isBlack(x_temp + 108, y_temp - 54));
                            return true;
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp + 54)) <= 20 && Math.Abs(getY() - (y_temp + 108)) <= 20)//右下 竖日
                {
                    if (isBlack(x_temp, y_temp + 54) == null && isRed(x_temp, y_temp + 54) == null)
                    {
                        if (isRed(x_temp + 54, y_temp + 108) == null)
                        {
                            moverd1();
                            if (isBlack(x_temp + 54, y_temp + 108) != null)
                                killBlack(isBlack(x_temp + 54, y_temp + 108));
                            return true;
                        }
                        else { goBack(); return false; }

                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp + 108)) <= 20 && Math.Abs(getY() - (y_temp + 54)) <= 20)//右下 横日
                {
                    if (isBlack(x_temp + 54, y_temp) == null && isRed(x_temp + 54, y_temp) == null)
                    {
                        if (isRed(x_temp + 108, y_temp + 54) == null)
                        {
                            moverd2();
                            if (isBlack(x_temp + 108, y_temp + 54) != null)
                                killBlack(isBlack(x_temp + 108, y_temp + 54));
                            return true;
                        }
                        else { goBack(); return false; }
                    }
                    else { goBack(); return false; }
                }
                else { goBack(); return false; }
            }
            else { goBack(); return false; }
        }
    }
    #endregion

    #region 黑象类
    public class BXiang : ChessPieces
    {
        public BXiang(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }
        public void MoveLeftUp() { pic.Location = new Point(x_temp - 108, y_temp - 108); }//左上移动
        public void MoveLeftDown() { pic.Location = new Point(x_temp - 108, y_temp + 108); }//左下移动
        public void MoveRightUp() { pic.Location = new Point(x_temp + 108, y_temp - 108); }//右上移动
        public void MoveRightDown() { pic.Location = new Point(x_temp + 108, y_temp + 108); }//右下移动

        public override bool move()//用于判断棋子的移动是否符合规则 在MouseUp中调用
        {
            if (getX() > 0 && getX() < 509 && getY() > 0 && getY() < 293)
            {
                if (Math.Abs(getX() - (x_temp - 108)) <= 20 && Math.Abs(getY() - (y_temp - 108)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                {
                    if (isBlack(x_temp - 108, y_temp - 108) == null &&//如果此位置没有其他黑棋
                        isBlack(x_temp - 54, y_temp - 54) == null &&  //移动路径没有被挡
                        isRed(x_temp - 54, y_temp - 54) == null)
                    {
                        MoveLeftUp();//左上移动
                        if (isRed(x_temp - 108, y_temp - 108) != null) { killRed(isRed(x_temp - 108, y_temp - 108)); }//判断此位置是否有红棋,有则吃掉
                        return true;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getY() - (y_temp + 108)) <= 20 && Math.Abs(getX() - (x_temp - 108)) <= 20)//同上 在右边一个
                {
                    if (isBlack(x_temp - 108, y_temp + 108) == null &&//如果此位置没有其他黑棋
                        isBlack(x_temp - 54, y_temp + 54) == null &&//移动路径没有被挡
                        isRed(x_temp - 54, y_temp + 54) == null)
                    {
                        MoveLeftDown();//左下移动
                        if (isRed(x_temp - 108, y_temp + 108) != null) { killRed(isRed(x_temp - 108, y_temp + 108)); }//判断此位置是否有红棋,有则吃掉
                        return true; ;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getY() - (y_temp - 108)) <= 20 && Math.Abs(getX() - (x_temp + 108)) <= 20)
                {
                    if (isBlack(x_temp + 108, y_temp - 108) == null &&//如果此位置没有其他黑棋
                        isBlack(x_temp + 54, y_temp - 54) == null &&//移动路径没有被挡
                        isRed(x_temp + 54, y_temp - 54) == null)
                    {
                        MoveRightUp();//右上移动
                        if (isRed(x_temp + 108, y_temp - 108) != null) { killRed(isRed(x_temp + 108, y_temp - 108)); }//判断此位置是否有红棋,有则吃掉
                        return true; ;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp + 108)) <= 20 && Math.Abs(getY() - (y_temp + 108)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                {
                    if (isBlack(x_temp + 108, y_temp + 108) == null &&//如果此位置没有其他黑棋
                        isBlack(x_temp + 54, y_temp + 54) == null &&//移动路径没有被挡
                        isRed(x_temp + 54, y_temp + 54) == null)
                    {
                        MoveRightDown();//右下移动
                        if (isRed(x_temp + 108, y_temp + 108) != null) { killRed(isRed(x_temp + 108, y_temp + 108)); }//判断此位置是否有红棋,有则吃掉
                        return true;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else { goBack(); return false; }
            }

            else { goBack(); return false; }
        }
    }
    #endregion

    #region 红象类
    public class RXiang : ChessPieces
    {
        public RXiang(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }
        public void MoveLeftUp() { pic.Location = new Point(x_temp - 108, y_temp - 108); }//左上移动
        public void MoveLeftDown() { pic.Location = new Point(x_temp - 108, y_temp + 108); }//左下移动
        public void MoveRightUp() { pic.Location = new Point(x_temp + 108, y_temp - 108); }//右上移动
        public void MoveRightDown() { pic.Location = new Point(x_temp + 108, y_temp + 108); }//右下移动

        public override bool move()//用于判断棋子的移动是否符合规则 在MouseUp中调用
        {
            if (getX() > 0 && getX() < 509 && getY() > 273 && getY() < 549)
            {
                if (Math.Abs(getX() - (x_temp - 108)) <= 20 && Math.Abs(getY() - (y_temp - 108)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                {
                    if (isRed(x_temp - 108, y_temp - 108) == null &&//如果此位置没有其他红棋
                        isBlack(x_temp - 54, y_temp - 54) == null &&  //移动路径没有被挡
                        isRed(x_temp - 54, y_temp - 54) == null)
                    {
                        MoveLeftUp();//左上移动
                        if (isBlack(x_temp - 108, y_temp - 108) != null) { killBlack(isBlack(x_temp - 108, y_temp - 108)); }//判断此位置是否有黑棋,有则吃掉
                        return true;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getY() - (y_temp + 108)) <= 20 && Math.Abs(getX() - (x_temp - 108)) <= 20)//同上 在右边一个
                {
                    if (isRed(x_temp - 108, y_temp + 108) == null &&//如果此位置没有其他红棋
                        isBlack(x_temp - 54, y_temp + 54) == null &&//移动路径没有被挡
                        isRed(x_temp - 54, y_temp + 54) == null)
                    {
                        MoveLeftDown();//左下移动
                        if (isBlack(x_temp - 108, y_temp + 108) != null) { killBlack(isBlack(x_temp - 108, y_temp + 108)); }//判断此位置是否有黑棋,有则吃掉
                        return true; ;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getY() - (y_temp - 108)) <= 20 && Math.Abs(getX() - (x_temp + 108)) <= 20)
                {
                    if (isRed(x_temp + 108, y_temp - 108) == null &&//如果此位置没有其他红棋
                        isBlack(x_temp + 54, y_temp - 54) == null &&//移动路径没有被挡
                        isRed(x_temp + 54, y_temp - 54) == null)
                    {
                        MoveRightUp();//右上移动
                        if (isBlack(x_temp + 108, y_temp - 108) != null) { killBlack(isBlack(x_temp + 108, y_temp - 108)); }//判断此位置是否有黑棋,有则吃掉
                        return true; ;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp + 108)) <= 20 && Math.Abs(getY() - (y_temp + 108)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                {
                    if (isRed(x_temp + 108, y_temp + 108) == null &&//如果此位置没有其他红棋
                        isBlack(x_temp + 54, y_temp + 54) == null &&//移动路径没有被挡
                        isRed(x_temp + 54, y_temp + 54) == null)
                    {
                        MoveRightDown();//右下移动
                        if (isBlack(x_temp + 108, y_temp + 108) != null) { killBlack(isBlack(x_temp + 108, y_temp + 108)); }//判断此位置是否有黑棋,有则吃掉
                        return true;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else { goBack(); return false; }
            }

            else { goBack(); return false; }
        }
    }
    #endregion

    #region 黑士类
    public class BShi : ChessPieces
    {
        public BShi(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }
        public void MoveLeftUp() { pic.Location = new Point(x_temp - 54, y_temp - 54); }//左上移动
        public void MoveLeftDown() { pic.Location = new Point(x_temp - 54, y_temp + 54); }//左下移动
        public void MoveRightUp() { pic.Location = new Point(x_temp + 54, y_temp - 54); }//右上移动
        public void MoveRightDown() { pic.Location = new Point(x_temp + 54, y_temp + 54); }//右下移动

        public override bool move()//用于判断棋子的移动是否符合规则 在MouseUp中调用
        {
            if (getX() > 171 && getX() < 347 && getY() > 0 && getY() < 185)
            {
                if (Math.Abs(getX() - (x_temp - 54)) <= 20 && Math.Abs(getY() - (y_temp - 54)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                {
                    if (isBlack(x_temp - 54, y_temp - 54) == null)//如果此位置没有其他黑棋
                    {
                        MoveLeftUp();//左上移动
                        if (isRed(x_temp - 54, y_temp - 54) != null) { killRed(isRed(x_temp - 54, y_temp - 54)); }//判断此位置是否有红棋,有则吃掉
                        return true;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getY() - (y_temp + 54)) <= 20 && Math.Abs(getX() - (x_temp - 54)) <= 20)//同上 在右边一个
                {
                    if (isBlack(x_temp - 54, y_temp + 54) == null)//如果此位置没有其他黑棋
                    {
                        MoveLeftDown();//左下移动
                        if (isRed(x_temp - 54, y_temp + 54) != null) { killRed(isRed(x_temp - 54, y_temp + 54)); }//判断此位置是否有红棋,有则吃掉
                        return true; ;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getY() - (y_temp - 54)) <= 20 && Math.Abs(getX() - (x_temp + 54)) <= 20)
                {
                    if (isBlack(x_temp + 54, y_temp - 54) == null)//如果此位置没有其他黑棋
                    {
                        MoveRightUp();//右上移动
                        if (isRed(x_temp + 54, y_temp - 54) != null) { killRed(isRed(x_temp + 54, y_temp - 54)); }//判断此位置是否有红棋,有则吃掉
                        return true; ;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp + 54)) <= 20 && Math.Abs(getY() - (y_temp + 54)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                {
                    if (isBlack(x_temp + 54, y_temp + 54) == null)//如果此位置没有其他黑棋
                    {
                        MoveRightDown();//右下移动
                        if (isRed(x_temp + 54, y_temp + 54) != null) { killRed(isRed(x_temp + 54, y_temp + 54)); }//判断此位置是否有红棋,有则吃掉
                        return true;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else { goBack(); return false; }
            }

            else { goBack(); return false; }
        }
    }
    #endregion

    #region 红士类
    public class RShi : ChessPieces
    {
        public RShi(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }
        public void MoveLeftUp() { pic.Location = new Point(x_temp - 54, y_temp - 54); }//左上移动
        public void MoveLeftDown() { pic.Location = new Point(x_temp - 54, y_temp + 54); }//左下移动
        public void MoveRightUp() { pic.Location = new Point(x_temp + 54, y_temp - 54); }//右上移动
        public void MoveRightDown() { pic.Location = new Point(x_temp + 54, y_temp + 54); }//右下移动

        public override bool move()//用于判断棋子的移动是否符合规则 在MouseUp中调用
        {
            if (getX() > 171 && getX() < 347 && getY() > 387 && getY() < 543)
            {
                if (Math.Abs(getX() - (x_temp - 54)) <= 20 && Math.Abs(getY() - (y_temp - 54)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                {
                    if (isRed(x_temp - 54, y_temp - 54) == null)//如果此位置没有其他红棋
                    {
                        MoveLeftUp();//左上移动
                        if (isBlack(x_temp - 54, y_temp - 54) != null) { killBlack(isBlack(x_temp - 54, y_temp - 54)); }//判断此位置是否有黑棋,有则吃掉
                        return true;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getY() - (y_temp + 54)) <= 20 && Math.Abs(getX() - (x_temp - 54)) <= 20)//同上 在右边一个
                {
                    if (isRed(x_temp - 54, y_temp + 54) == null)//如果此位置没有其他红棋
                    {
                        MoveLeftDown();//左下移动
                        if (isBlack(x_temp - 54, y_temp + 54) != null) { killBlack(isBlack(x_temp - 54, y_temp + 54)); }//判断此位置是否有黑棋,有则吃掉
                        return true; ;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getY() - (y_temp - 54)) <= 20 && Math.Abs(getX() - (x_temp + 54)) <= 20)
                {
                    if (isRed(x_temp + 54, y_temp - 54) == null)//如果此位置没有其他红棋
                    {
                        MoveRightUp();//右上移动
                        if (isBlack(x_temp + 54, y_temp - 54) != null) { killBlack(isBlack(x_temp + 54, y_temp - 54)); }//判断此位置是否有黑棋,有则吃掉
                        return true; ;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - (x_temp + 54)) <= 20 && Math.Abs(getY() - (y_temp + 54)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                {
                    if (isRed(x_temp + 54, y_temp + 54) == null)//如果此位置没有其他红棋
                    {
                        MoveRightDown();//右下移动
                        if (isBlack(x_temp + 54, y_temp + 54) != null) { killBlack(isBlack(x_temp + 54, y_temp + 54)); }//判断此位置是否有黑棋,有则吃掉
                        return true;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else { goBack(); return false; }
            }

            else { goBack(); return false; }
        }
    }
    #endregion

    #region 黑将类
    public class BJiang : ChessPieces
    {
        public BJiang(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }
        public void MoveDown() { pic.Location = new Point(x_temp, y_temp + 54); }//向下移动
        public void MoveUp() { pic.Location = new Point(x_temp, y_temp - 54); }//向上移动
        public void MoveLeft() { pic.Location = new Point(x_temp - 54, y_temp); }//向左移动
        public void MoveRight() { pic.Location = new Point(x_temp + 54, y_temp); }//向右移动

        public override bool move()//用于判断棋子的移动是否符合规则 在MouseUp中调用
        {
            if (getX() > 171 && getX() < 347 && getY() > 0 && getY() < 185)
            {
                if (Math.Abs(getX() - x_temp) <= 20 && Math.Abs(getY() - (y_temp + 54)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                {
                    if (isBlack(x_temp, y_temp + 54) == null)//如果此位置没有其他黑棋
                    {
                        MoveDown();//往下走一步
                        if (isRed(x_temp, y_temp + 54) != null) { killRed(isRed(x_temp, y_temp + 54)); }//判断此位置是否有红棋,有则吃掉
                        return true;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getY() - y_temp) <= 20 && Math.Abs(getX() - (x_temp + 54)) <= 20)//同上 在右边一个
                {
                    if (isBlack(x_temp + 54, y_temp) == null)//如果此位置没有其他黑棋
                    {
                        MoveRight();//向右走一步
                        if (isRed(x_temp + 54, y_temp) != null) { killRed(isRed(x_temp + 54, y_temp)); }//判断此位置是否有红棋,有则吃掉
                        return true; ;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getY() - y_temp) <= 20 && Math.Abs(getX() - (x_temp - 54)) <= 20)
                {
                    if (isBlack(x_temp - 54, y_temp) == null)//如果此位置没有其他黑棋
                    {
                        MoveLeft();//向左走一步
                        if (isRed(x_temp - 54, y_temp) != null) { killRed(isRed(x_temp - 54, y_temp)); }//判断此位置是否有红棋,有则吃掉
                        return true; ;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - x_temp) <= 20 && Math.Abs(getY() - (y_temp - 54)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                {
                    if (isBlack(x_temp, y_temp - 54) == null)//如果此位置没有其他黑棋
                    {
                        MoveUp();//往上走一步
                        if (isRed(x_temp, y_temp - 54) != null) { killRed(isRed(x_temp, y_temp - 54)); }//判断此位置是否有红棋,有则吃掉
                        return true;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else { goBack(); return false; }
            }
            else if (Math.Abs(getX() - p_array[1, 15].Location.X) <= 20 && Math.Abs(getY() - p_array[1, 15].Location.Y) <= 20)
            {
                if (x_temp == p_array[1, 15].Location.X)
                {
                    int n = (p_array[1, 15].Location.Y - y_temp) / 54;
                    for (int i = 1; i != n; i++)
                    {
                        if (isRed(x_temp, y_temp + i * 54) != null || isBlack(x_temp, y_temp + i * 54) != null)
                        { goBack(); return false; }
                    }
                    moveto(p_array[1, 15].Location.X, p_array[1, 15].Location.Y);
                    killRed(p_array[1, 15]);
                    return true;
                }
                else { goBack(); return false; }
            }
            else { goBack(); return false; }
        }
    }
    #endregion

    #region 红将类
    public class RJiang : ChessPieces
    {
        public RJiang(PictureBox p, PictureBox pa, PictureBox[,] arr, Panel dp, int x, int y, Stack<Point> sp, Stack<string> spb) : base(p, pa, arr, dp, x, y, sp, spb) { }
        public void MoveDown() { pic.Location = new Point(x_temp, y_temp + 54); }//向下移动
        public void MoveUp() { pic.Location = new Point(x_temp, y_temp - 54); }//向上移动
        public void MoveLeft() { pic.Location = new Point(x_temp - 54, y_temp); }//向左移动
        public void MoveRight() { pic.Location = new Point(x_temp + 54, y_temp); }//向右移动

        public override bool move()//用于判断棋子的移动是否符合规则 在MouseUp中调用
        {
            if (getX() > 171 && getX() < 347 && getY() > 387 && getY() < 543)
            {
                if (Math.Abs(getX() - x_temp) <= 20 && Math.Abs(getY() - (y_temp + 54)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                {
                    if (isRed(x_temp, y_temp + 54) == null)//如果此位置没有其他红棋
                    {
                        MoveDown();//往下走一步
                        if (isBlack(x_temp, y_temp + 54) != null) { killBlack(isBlack(x_temp, y_temp + 54)); }//判断此位置是否有黑棋,有则吃掉
                        return true;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getY() - y_temp) <= 20 && Math.Abs(getX() - (x_temp + 54)) <= 20)//同上 在右边一个
                {
                    if (isRed(x_temp + 54, y_temp) == null)//如果此位置没有其他红棋
                    {
                        MoveRight();//向右走一步
                        if (isBlack(x_temp + 54, y_temp) != null) { killBlack(isBlack(x_temp + 54, y_temp)); }//判断此位置是否有黑棋,有则吃掉
                        return true; ;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getY() - y_temp) <= 20 && Math.Abs(getX() - (x_temp - 54)) <= 20)
                {
                    if (isRed(x_temp - 54, y_temp) == null)//如果此位置没有其他红棋
                    {
                        MoveLeft();//向左走一步
                        if (isBlack(x_temp - 54, y_temp) != null) { killBlack(isBlack(x_temp - 54, y_temp)); }//判断此位置是否有黑棋,有则吃掉
                        return true; ;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else if (Math.Abs(getX() - x_temp) <= 20 && Math.Abs(getY() - (y_temp - 54)) <= 20)//如果棋子当前坐标在初始坐标下方一格(误差20)
                {
                    if (isRed(x_temp, y_temp - 54) == null)//如果此位置没有其他红棋
                    {
                        MoveUp();//往上走一步
                        if (isBlack(x_temp, y_temp - 54) != null) { killBlack(isBlack(x_temp, y_temp - 54)); }//判断此位置是否有黑棋,有则吃掉
                        return true;//走棋成功
                    }
                    else { goBack(); return false; }
                }
                else { goBack(); return false; }
            }
            else if (Math.Abs(getX() - p_array[0, 15].Location.X) <= 20 && Math.Abs(getY() - p_array[0, 15].Location.Y) <= 20)
            {
                if (x_temp == p_array[0, 15].Location.X)
                {
                    int n = (y_temp - p_array[1, 15].Location.Y) / 54;
                    for (int i = 1; i != n; i++)
                    {
                        if (isRed(x_temp, y_temp - i * 54) != null || isBlack(x_temp, y_temp - i * 54) != null)
                        { goBack(); return false; }
                    }
                    moveto(p_array[0, 15].Location.X, p_array[0, 15].Location.Y);
                    killBlack(p_array[0, 15]);
                    return true;
                }
                else { goBack(); return false; }
            }
            else { goBack(); return false; }
        }
    }
    #endregion
}