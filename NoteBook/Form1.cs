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
        // 所有已经展开的项
        private List<TreeNode> expandedList = new List<TreeNode>();
        // 保存已经搜索出来的并处于展开状态的所有id
        private List<TreeNode> searchedExpanded = new List<TreeNode>();

        public Form1()
        {
            InitializeComponent();

            DBHelper.CreateTables();

            ListHelper.InitList(this);
            //this.Opacity -= 0.1;

            // Register the hotkey
            //注册热键Alt+Q，Id号为100。HotKey.KeyModifiers.Alt也可以直接使用数字4来表示。   
            HotKey.RegisterHotKey(Handle, 100, HotKey.KeyModifiers.Alt, Keys.Q);
            HotKey.RegisterHotKey(Handle, 200, HotKey.KeyModifiers.Ctrl, Keys.L);
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
                        case 200:   // 按下的是ctrl+L
                            if (textBoxSearch.Focused)
                            {
                                browser.Focus();
                            } else
                            {
                                textBoxSearch.Focus();
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
            // 为textBoxSearch设置hint
            Win32Utility.SetCueText(textBoxSearch, "搜索...");
        }
        
        // 实现搜索
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (buttonSearch.Text.Equals("搜索"))
            {
                // 搜索匹配的所有节点，并展开这些节点的所有父节点并用某种颜色对其进行标识
                String searchName = textBoxSearch.Text;
                List<int> ids = ListHelper.SearchResult(searchName);

                // 改回原色并关闭节点
                foreach (var node in searchedExpanded)
                {
                    node.ForeColor = Color.Magenta;
                    if (!expandedList.Contains(node))
                    {
                        node.Collapse();
                    }
                }
                // 清楚集合
                searchedExpanded.Clear();

                // 若无搜索结果，则直接返回
                if (ids.Count <= 0)
                {
                    return;
                }

                // 展开节点的所有父节点并改变其颜色，然后保存其id至一集合
                foreach (var node in ListHelper.allNodes)
                {
                    Tags tags = (Tags)node.Tag;
                    foreach (var id in ids)
                    {
                        if (id == tags.id)
                        {
                            // 展开该节点的所有父节点
                            ListHelper.expandAllParent(node);
                            // 改变其字体颜色以示区别
                            node.ForeColor = Color.Red;
                            // 保存到集合
                            searchedExpanded.Add(node);
                        }
                    }
                }
            }
            
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
                if (noteList.SelectedNode == null)
                {
                    return;
                }
                Tags tags = (Tags)noteList.SelectedNode.Tag;
                DBHelper.ModifyContent(tags.id, content);
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
                Tags tags = (Tags)noteList.SelectedNode.Tag;
                DBHelper.ModifyContent(tags.id, content);
            }
        }
        
        // 列表的右键修改菜单
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modifyNote.form1 = this;
            modifyNote.textBoxModifyNote.Text = noteList.SelectedNode.Text;
            modifyNote.ShowDialog();
        }

        // 列表的右键删除菜单
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noteList.SelectedNode != null)
            {
                TreeNode selectedNode = noteList.SelectedNode;
                // 删除此节点的所有子节点
                ListHelper.RemoveNode(selectedNode);
                // 在noteList中只需删除此节点，即可删除其所有的子节点
                noteList.SelectedNode.Remove();
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

                expandedList.Add(e.Node);
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

        // textBoxSearch的按键事件
        private void textBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Enter事件
            if (e.KeyChar == System.Convert.ToChar(13))
            {
                // 取消按下返回键时的“噔”响声
                e.Handled = true;
                // 搜索
                buttonSearch_Click(sender, e);
            }
        }
    }
}
