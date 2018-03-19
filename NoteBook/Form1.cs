using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CefSharp;
using CefSharp.WinForms;
using System.Threading;
using System.Web;

namespace NoteBook
{
    public partial class Form1 : Form
    {

        private FormNewNote newNote = new FormNewNote();
        private FormModifyNoteName modifyNote = new FormModifyNoteName();
        private FloatForm floatForm;
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

        public ChromiumWebBrowser browser;

        private void Form1_Load(object sender, EventArgs e)
        {
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
            browser = new ChromiumWebBrowser(AppDomain.CurrentDomain.BaseDirectory + "\\tinymce\\editor.html");
            this.browserPanel.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;

            // Register the JSEvent
            browser.RegisterJsObject("jsEvent", this);
            // 创建并显示浮动窗口
            floatForm = new FloatForm(this);
            floatForm.Show();
        }
        
        private void buttonSearch_Click(object sender, EventArgs e)
        {
        }

        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewNote();
        }

        public void NewNote()
        {
            newNote.isNewNodeItem = false;
            newNote.form1 = this;
            newNote.ShowDialog();
        }

        public void NewSonNote()
        {
            newNote.isNewNodeItem = true;
            newNote.form1 = this;
            newNote.ShowDialog();
        }
        
        // 暴露Push方法给JS调用，来存储编辑器内容到数据库
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

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modifyNote.form1 = this;
            modifyNote.textBoxModifyNote.Text = noteList.SelectedNode.Text;
            modifyNote.ShowDialog();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noteList.SelectedNode != null)
            {
                ListHelper.result.Clear();
                List<TreeNode> sonNodes = ListHelper.GetAllNodeTag(noteList.SelectedNode);
                for (int i = 0; i < sonNodes.Count; ++i)
                {
                    DBHelper.RemoveNote(sonNodes[i].Text);
                    ListHelper.totalTitles
                        .Remove(ListHelper.GetNoteFromTitle(sonNodes[i].Text));
                    ListHelper.allNode.Remove(sonNodes[i]);
                }
                noteList.Nodes.Remove(sonNodes[0]);
            }
        }

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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 不关闭窗口
            e.Cancel = true;
            // 隐藏窗口
            this.Hide();
        }

        private void noteList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null)
            {
                string title = e.Node.Text;
                string content = DBHelper.GetContent(title);
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
