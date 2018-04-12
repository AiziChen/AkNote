using System;
using System.Windows.Forms;

namespace AkNote
{
    public partial class FormNewNote : Form
    {
        public FormNewNote()
        {
            InitializeComponent();
        }

        public Form1 form1;
        // 是否为某个父项的子项
        public bool isNewNodeItem;
        private void buttonNewNote_Click(object sender, EventArgs e)
        {
            string noteTitle = textBoxNewNote.Text;
            if (noteTitle.Trim().Equals(""))
            {
                MessageBoxEx.Show(this, "笔记名为空，无法新建");
                return;
            }
            // 新建一个笔记项
            form1.noteList.Invoke(new Action(() =>
            {
                if (isNewNodeItem)
                {
                    ListHelper.AddNewNodeItem(form1, noteTitle);
                } else
                {
                    ListHelper.AddItem(form1, noteTitle);
                }
            }));

            this.Close();
        }

        // 回车即可新建笔记名
        private void textBoxNewNote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonNewNote_Click(sender, e);
            }
        }
    }
}
