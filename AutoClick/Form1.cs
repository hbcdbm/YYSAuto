using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;


namespace AutoClick
{
    public partial class Form1 : Form
    {
        public static bool bStop = false;

        private delegate void EnablebuttonDelegate(Button btn);

        public static int nMin = 0;

        private void Enablebutton(Button btn)
        {
            if (btn.InvokeRequired)
            {
                EnablebuttonDelegate handler = new EnablebuttonDelegate(Enablebutton);
                this.Invoke(handler, new object[] { btn });
            }
            else
            {
                btn.Enabled = true;
            }

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;                             //最左坐标
            public int Top;                             //最上坐标
            public int Right;                           //最右坐标
            public int Bottom;                        //最下坐标
        }

        //将枚举作为位域处理
        [Flags]
        enum MouseEventFlag : uint //设置鼠标动作的键值
        {
            Move = 0x0001,               //发生移动
            LeftDown = 0x0002,           //鼠标按下左键
            LeftUp = 0x0004,             //鼠标松开左键
            RightDown = 0x0008,          //鼠标按下右键
            RightUp = 0x0010,            //鼠标松开右键
            MiddleDown = 0x0020,         //鼠标按下中键
            MiddleUp = 0x0040,           //鼠标松开中键
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,              //鼠标轮被移动
            VirtualDesk = 0x4000,        //虚拟桌面
            Absolute = 0x8000
        }

        //设置鼠标位置
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        //设置鼠标按键和动作
        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo); //UIntPtr指针多句柄类型

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //消息发送API
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        uint WM_LBUTTONDOWN = 0x201;
        uint WM_LBUTTONUP = 0x202;

        public Form1()
        {
            InitializeComponent();
        }

        public void Job()
        {
            //刷nMin秒退出循环
            int nSecond = 1;
            while(true)
            {
                if (nSecond >= nMin)
                    break;
                nSecond++;
                SetCursorPos(144, 949);
                mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                Thread.Sleep(500);
                Bitmap jt = new Bitmap("ss.jpg");
                int iWidth = 70;
                int iHeight = 20;
                Image img = new Bitmap(iWidth, iHeight); //从一个继承自image类的对象中创建Graphics对象
                Graphics g = Graphics.FromImage(img); //抓取全屏幕
                g.CopyFromScreen(300, 720, 0, 0, new Size(iWidth, iHeight));
                Bitmap map = new Bitmap(img);
                if (ImageCompareArray(jt, map))
                {
                    SetCursorPos(598, 742);
                    mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                    Thread.Sleep(1000);
                    //判断是否进入
                    Bitmap yqfy = new Bitmap("yqfy.jpg");
                    iWidth = 80;
                    iHeight = 20;
                    Image img1 = new Bitmap(iWidth, iHeight);
                    Graphics g1 = Graphics.FromImage(img1);
                    g1.CopyFromScreen(108, 936, 0, 0, new Size(iWidth, iHeight));
                    Bitmap map1 = new Bitmap(img1);
                    if (ImageCompareArray(yqfy, map1))
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }

                }
                Thread.Sleep(800);
            }
            //播放循环结束提示音
            PlayQuit();
            Enablebutton(button6);
        }

        public void PlayTip()
        {
            string path = "tip.wav";//.wav音频文件路径
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
            player.Play();//简单播放一遍
        }

        public void PlayQuit()
        {
            string path = "quit.wav";//.wav音频文件路径
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
            player.Play();//简单播放一遍
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            Point screenPoint = Control.MousePosition;
            tb_x.Text = screenPoint.X.ToString();
            tb_y.Text = screenPoint.Y.ToString();
        }

        //判断两张图片是否一致
        public static bool ImageCompareString(Bitmap firstImage, Bitmap secondImage)
        {
            MemoryStream ms = new MemoryStream();
            firstImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String firstBitmap = Convert.ToBase64String(ms.ToArray());
            ms.Position = 0;

            secondImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String secondBitmap = Convert.ToBase64String(ms.ToArray());

            if (firstBitmap.Equals(secondBitmap))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ImageCompareArray(Bitmap firstImage, Bitmap secondImage)
        {
            bool flag = true;
            string firstPixel;
            string secondPixel;

            if (firstImage.Width == secondImage.Width
                && firstImage.Height == secondImage.Height)
            {
                for (int i = 0; i < firstImage.Width; i++)
                {
                    for (int j = 0; j < firstImage.Height; j++)
                    {
                        firstPixel = firstImage.GetPixel(i, j).ToString();
                        secondPixel = secondImage.GetPixel(i, j).ToString();
                        if (firstPixel != secondPixel)
                        {
                            flag = false;
                            break;
                        }
                    }
                }

                if (flag == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }  

        private void hook_KeyDown(object sender, KeyEventArgs e) 
        { 
            //判断按下的键（Alt + A） 
            if (e.KeyValue == (int)Keys.A && (int)Control.ModifierKeys == (int)Keys.Alt) 
            {
                MessageBox.Show("ok");
                bStop = true;
            } 
        }

        private void button6_Click(object sender, EventArgs e)
        {
            bStop = false;
            IntPtr maindHwnd = FindWindow("Qt5QWindowIcon", "ok");
            if (maindHwnd != IntPtr.Zero)
            {
                RECT rect = new RECT();
                GetWindowRect(maindHwnd, ref rect);
                MoveWindow(maindHwnd, 2, 502, rect.Right - rect.Left, rect.Bottom - rect.Top, true);
                ShowWindow(maindHwnd, 1);
                int iWidth = 80;
                int iHeight = 20;
                Image img = new Bitmap(iWidth, iHeight); //从一个继承自image类的对象中创建Graphics对象
                Graphics g = Graphics.FromImage(img); //抓取全屏幕
                g.CopyFromScreen(108, 936, 0, 0, new Size(iWidth, iHeight));
                img.Save("yqfy.jpg");
            }
            nMin = Convert.ToInt32(tb_minutes.Text) * 60;
            Thread thread1 = new Thread(Job);
            thread1.Start();
            button6.Enabled = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            IntPtr maindHwnd = FindWindow("Qt5QWindowIcon", "ok");
            if (maindHwnd != IntPtr.Zero)
            {
                RECT rect = new RECT();
                GetWindowRect(maindHwnd, ref rect);
                MoveWindow(maindHwnd, 2, 502, rect.Right - rect.Left, rect.Bottom - rect.Top, true);
                ShowWindow(maindHwnd, 1);
            }
            int iWidth = 70;
            int iHeight = 20;
            Image img = new Bitmap(iWidth, iHeight); //从一个继承自image类的对象中创建Graphics对象
            Graphics g = Graphics.FromImage(img); //抓取全屏幕
            g.CopyFromScreen(300,720,0,0, new Size(iWidth, iHeight));
            img.Save("ss.jpg");
        } 

    }

    public class GlobalMouseHandler : IMessageFilter
    {

        private const int WM_MOUSEMOVE = 0x0200;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_MOUSEMOVE)
            {
                MessageBox.Show("ok");
            }
            return false;
        }
    }
}
