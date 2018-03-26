using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AkNote
{
    // 浮动窗口
    public partial class FloatForm : Form
    {
        private Form1 form1;
        private Point ptMouseCurrrnetPos, ptMouseNewPos, ptFormPos, ptFormNewPos;
        private bool blnMouseDown = false;

        public FloatForm()
        {
            InitializeComponent();
        }

        public FloatForm(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
        }

        private void FloatForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (form1.Visible)
            {
                form1.Hide();
            }
            else
            {
                form1.Show();
            }
        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form1.Close();
            this.Close();
            // 完全退出程序
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void FloatForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (blnMouseDown)
            {
                ptMouseNewPos = Control.MousePosition;
                ptFormNewPos.X = ptMouseNewPos.X - ptMouseCurrrnetPos.X + ptFormPos.X;
                ptFormNewPos.Y = ptMouseNewPos.Y - ptMouseCurrrnetPos.Y + ptFormPos.Y;
                Location = ptFormNewPos;
                ptFormPos = ptFormNewPos;
                ptMouseCurrrnetPos = ptMouseNewPos;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_INFO
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public uint dwTotalPhys;
            public uint dwAvailPhys;
            public uint dwTotalPageFile;
            public uint dwAvailPageFile;
            public uint dwTotalVirtual;
            public uint dwAvailVirtual;
        }
        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref MEMORY_INFO meminfo);
        private void FloatForm_Load(object sender, EventArgs e)
        {
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ShowMemoryInfo));
            thread.Start();
        }

        private void ShowMemoryInfo()
        {
            while (true)
            {
                MEMORY_INFO MemInfo;
                MemInfo = new MEMORY_INFO();
                GlobalMemoryStatus(ref MemInfo);

                long totalMb = Convert.ToInt64(MemInfo.dwTotalPhys.ToString()) / 1024 / 1024;
                long avaliableMb = Convert.ToInt64(MemInfo.dwAvailPhys.ToString()) / 1024 / 1024;
                
                label1.Invoke(new Action(() =>
                {
                    label1.Text = (totalMb - avaliableMb) * 100 / totalMb + "%";
                }));
                System.Threading.Thread.Sleep(1000);
            }
            
        }

        private void FloatForm_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(this, "双击打开或关闭编辑窗口");
        }

        private void FloatForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                blnMouseDown = true;
                ptMouseCurrrnetPos = Control.MousePosition;
                ptFormPos = Location;
            }

            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(this, new Point(e.X, e.Y));
            }
        }

        private void FloatForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                blnMouseDown = false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            List<Point> list = new List<Point>();
            int width = this.Width;
            int height = this.Height;

            //左上
            list.Add(new Point(0, 5));
            list.Add(new Point(1, 5));
            list.Add(new Point(1, 3));
            list.Add(new Point(2, 3));
            list.Add(new Point(2, 2));
            list.Add(new Point(3, 2));
            list.Add(new Point(3, 1));
            list.Add(new Point(5, 1));
            list.Add(new Point(5, 0));
            //右上
            list.Add(new Point(width - 5, 0));
            list.Add(new Point(width - 5, 1));
            list.Add(new Point(width - 3, 1));
            list.Add(new Point(width - 3, 2));
            list.Add(new Point(width - 2, 2));
            list.Add(new Point(width - 2, 3));
            list.Add(new Point(width - 1, 3));
            list.Add(new Point(width - 1, 5));
            list.Add(new Point(width - 0, 5));
            //右下
            list.Add(new Point(width - 0, height - 5));
            list.Add(new Point(width - 1, height - 5));
            list.Add(new Point(width - 1, height - 3));
            list.Add(new Point(width - 2, height - 3));
            list.Add(new Point(width - 2, height - 2));
            list.Add(new Point(width - 3, height - 2));
            list.Add(new Point(width - 3, height - 1));
            list.Add(new Point(width - 5, height - 1));
            list.Add(new Point(width - 5, height - 0));
            //左下
            list.Add(new Point(5, height - 0));
            list.Add(new Point(5, height - 1));
            list.Add(new Point(3, height - 1));
            list.Add(new Point(3, height - 2));
            list.Add(new Point(2, height - 2));
            list.Add(new Point(2, height - 3));
            list.Add(new Point(1, height - 3));
            list.Add(new Point(1, height - 5));
            list.Add(new Point(0, height - 5));

            Point[] points = list.ToArray();

            GraphicsPath shape = new GraphicsPath();
            shape.AddPolygon(points);

            //将窗体的显示区域设为GraphicsPath的实例
            Region = new System.Drawing.Region(shape);
        }
    }
}
