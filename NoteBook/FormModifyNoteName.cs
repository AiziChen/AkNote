using System;
using System.Windows.Forms;

namespace AkNote
{
    /**
     * 修改笔记项的名字的窗口
     * @author QuanyeChen
     */
    public partial class FormModifyNoteName : Form
    {
        public FormModifyNoteName()
        {
            // 初始化组件
            InitializeComponent();
        }

        public Form1 form1;
        // 修改笔记名的方法
        private void buttonModifyNote_Click(object sender, EventArgs e)
        {
            string newTitle = textBoxModifyNote.Text;
            if (newTitle.Trim().Equals(""))
            {
                MessageBoxEx.Show(this, "笔记名为空，请重新输入");
                return;
            }
            TreeNode selectedNode = form1.noteList.SelectedNode;
            Tags tags = (Tags)selectedNode.Tag;
            DBHelper.ModifyTitle(tags.id, newTitle);
            ListHelper.Modify(tags, newTitle);
            selectedNode.Text = newTitle;
            this.Close();
        }

        // 回车即可修改笔记名
        private void textBoxModifyNote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonModifyNote_Click(sender, e);
            }
        }
        
    }
}
