using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoteBook
{
    public partial class FormModifyNoteName : Form
    {
        public FormModifyNoteName()
        {
            InitializeComponent();
        }

        public Form1 form1;
        private void buttonModifyNote_Click(object sender, EventArgs e)
        {
            string newTitle = textBoxModifyNote.Text;
            if (newTitle.Equals(""))
            {
                MessageBoxEx.Show(this, "笔记名为空，请重新输入");
                return;
            }
            foreach (var note in ListHelper.totalTitles)
            {
                if (note.title.Equals(newTitle))
                {
                    MessageBoxEx.Show(this, "含同名笔记，修改失败");
                    return;
                }
            }
            string preTitle = form1.noteList.SelectedNode.Text;
            DBHelper.ModifyTitle(preTitle, newTitle);
            ListHelper.Modify(form1, newTitle);
            this.Close();
        }

        private void textBoxModifyNote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonModifyNote_Click(sender, e);
            }
        }
        
    }
}
