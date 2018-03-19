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
    public partial class FormNewNote : Form
    {
        public FormNewNote()
        {
            InitializeComponent();
        }

        public Form1 form1;
        public bool isNewNodeItem;
        private void buttonNewNote_Click(object sender, EventArgs e)
        {
            // 保存数据到数据库中
            string noteTitle = textBoxNewNote.Text;
            if (noteTitle.Equals(""))
            {
                MessageBoxEx.Show(this, "笔记名为空，无法新建");
                return;
            }
            foreach (var note in ListHelper.totalTitles)
            {
                if (noteTitle.Equals(note.title))
                {
                    MessageBoxEx.Show(this, "含同名笔记，新建失败");
                    return;
                }
            }

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

        private void textBoxNewNote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonNewNote_Click(sender, e);
            }
        }
    }
}
