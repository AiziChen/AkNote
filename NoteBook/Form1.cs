using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace AkNote
{
    /**
     * AKBook主界面
     * @author QuanyeChen
     */
    public partial class Form1 : Form
    {
        // 窗口
        private FormNewNote newNote = new FormNewNote();
        private FormModifyNoteName modifyNote = new FormModifyNoteName();
        private FloatForm floatForm;
        // Chromminum浏览器实例
        public ChromiumWebBrowser browser;

        public Form1()
        {
            InitializeComponent();

            DBHelper.CreateTables();

            ListHelper.InitList(this);
            //this.Opacity -= 0.1;

            // Register the hotkey
            //注册热键Alt+Q，Id号为100。HotKey.KeyModifiers.Alt也可以直接使用数字4来表示。   
            HotKey.RegisterHotKey(Handle, 100, HotKey.KeyModifiers.Alt, Keys.Q);
        }
        //
        // 处理热键
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            //按快捷键    
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:    //按下的是Alt+Q   
                            if (this.Visible)
                            {
                                this.Hide();
                            } else
                            {
                                this.Show();
                            }
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        // 窗口加载时执行
        private void Form1_Load(object sender, EventArgs e)
        {
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
            // 发布版本的绝对路径
            browser = new ChromiumWebBrowser(AppDomain.CurrentDomain.BaseDirectory + "\\tinymce\\editor.html");
            // 测试版本的绝对路径
            //browser = new ChromiumWebBrowser(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\..\\tinymce\\editor.html");
            this.browserPanel.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;

            // Register the JSEvent
            browser.RegisterJsObject("jsEvent", this);
            // 创建并显示浮动窗口
            floatForm = new FloatForm(this);
            floatForm.Show();
        }
        
        // 实现搜索
        private void buttonSearch_Click(object sender, EventArgs e)
        {
        }

        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewNote();
        }

        // 新建一个根Note
        public void NewNote()
        {
            newNote.isNewNodeItem = false;
            newNote.form1 = this;
            newNote.ShowDialog();
        }

        // 新建一个父Note的子Note
        public void NewSonNote()
        {
            newNote.isNewNodeItem = true;
            newNote.form1 = this;
            newNote.ShowDialog();
        }
        
        // 暴露Push方法给JS脚本调用，用来存储编辑器内容到数据库
        public void Push(string content)
        {
            noteList.Invoke(new Action(() =>
            {
                if (noteList.SelectedNode == null) return;
                DBHelper.UpdateNote(noteList.SelectedNode.Text, content);
            }));
        }

        //
        // 通过调用JS的代码获取编辑器内容
        //
        private void 提交ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noteList.SelectedNode == null) return;
            Task<JavascriptResponse> response = browser.EvaluateScriptAsync("getHtml();");
            response.Wait();
            if (response.Result.Result != null)
            {
                string content = response.Result.Result.ToString();
                DBHelper.UpdateNote(noteList.SelectedNode.Text, content);
            }
        }
        
        // 列表的右键修改菜单
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modifyNote.form1 = this;
            modifyNote.textBoxModifyNote.Text = noteList.SelectedNode.Text;
            modifyNote.ShowDialog();
        }

        // 列表的右键删除改菜单
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noteList.SelectedNode != null)
            {
                List<TreeNode> sonNodes = ListHelper.allNodes;
                for (int i = 0; i < sonNodes.Count; ++i)
                {
                    DBHelper.RemoveNote(sonNodes[i].Text);
                    ListHelper.totalNotes
                        .Remove(ListHelper.GetNoteById( ((Tags)sonNodes[i].Tag).id ));
                    ListHelper.allNodes.Remove(sonNodes[i]);
                }
                noteList.Nodes.Remove(sonNodes[0]);
            }
        }

        // 当右键按下并松开后，会执行此监听方法，此方法在Form设计器中已经设定绑定到了右键松开的监听器
        private void noteList_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //int posindex = noteList.IndexFromPoint(new Point(e.X, e.Y));
                TreeNode treeNode = noteList.GetNodeAt(new Point(e.X, e.Y));
                noteList.ContextMenuStrip = null;
                if (treeNode != null)
                {
                    noteList.SelectedNode = treeNode;
                    contextMenuStrip1.Show(noteList, new Point(e.X, e.Y));
                }
            }
            noteList.Refresh();
        }

        // 窗口关闭时的监听方法
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 不关闭窗口
            e.Cancel = true;
            // 隐藏窗口
            this.Hide();
        }

        // 列表被点击时的监听方法
        private void noteList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null)
            {
                Tags tags = (Tags)e.Node.Tag;
                string content = DBHelper.GetContent(tags.id);
                content = HttpUtility.UrlEncode(content);
                browser.ExecuteScriptAsync("setHtml('" + content + "');");
            }
        }

        private void 添加子项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewSonNote();
        }

        private void 关于ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            HelpForm form = new HelpForm();
            form.ShowDialog();
        }
    }
}
