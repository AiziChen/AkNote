using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoteBook
{
    public struct Note
    {
        public int id;
        public string title;
        public bool encrypted;
        public string parentId;
    }

    public class ListHelper
    {
        public static readonly string NEW_HTML = "<ul><li> &nbsp;</li></ul>";
        public static List<Note> totalTitles { get; set; }
        public static List<TreeNode> allNode { get; set; }

        public static void InitList(Form1 form)
        {
            if (totalTitles == null)
            {
                totalTitles = new List<Note>();
            }
            if (allNode == null)
            {
                allNode = new List<TreeNode>();
            }
            Update(form);
        }
        //
        // 更新笔记列表
        public static void Update(Form1 form)
        {
            totalTitles.Clear();
            allNode.Clear();
            form.noteList.Nodes.Clear();
            foreach (var note in DBHelper.GetAllNotes())
            {
                totalTitles.Add(note);
                TreeNode treeNode = new TreeNode();
                treeNode.Tag = note.parentId;
                treeNode.Text = note.title;
                allNode.Add(treeNode);
            }
            
            AddNodes(form);
        }

        public static void AddNodes(Form1 form)
        {
            foreach (var node in allNode)
            {
                if (node.Tag.Equals(DBHelper.NO_BELONG))
                {
                    form.noteList.Nodes.Add(node);
                }

                AddSonNodes(node);
            }
            
        }

        public static void AddSonNodes(TreeNode treeNode)
        {
            for (int i = 0; i < totalTitles.Count; ++i)
            {
                if (totalTitles[i].id.ToString().Equals(treeNode.Tag))
                {
                    allNode[i].Nodes.Add(treeNode);
                }
            }
        }
        //
        // 添加一个笔记项
        public static void AddItem(Form1 form1, string newTitle)
        {
            form1.noteList.Invoke(new Action(() =>
            {
                DBHelper.AddNote(newTitle, NEW_HTML);
                Note note = new Note
                {
                    title = newTitle,
                    id = GetBiggestId(),
                    encrypted = false,
                    parentId = DBHelper.NO_BELONG
                };
                totalTitles.Add(note);
                TreeNode treeNode = new TreeNode();
                treeNode.Text = note.title;
                treeNode.Tag = note.parentId;
                allNode.Add(treeNode);
                form1.noteList.Nodes.Add(newTitle, newTitle);
                form1.noteList.Nodes[newTitle].Tag = note.id.ToString();
            }));
        }
        //
        // 对当前选中的Node添加新的子Node
        public static void AddNewNodeItem(Form1 form1, string newTitle)
        {
            TreeNode node = form1.noteList.SelectedNode;
            if (node != null)
            {
                Note note = new Note();
                note.title = newTitle;
                note.id = GetBiggestId() + 1;
                note.encrypted = false;
                note.parentId = GetIdFromTitle(node.Text).ToString();
                totalTitles.Add(note);

                TreeNode treeNode = new TreeNode();
                treeNode.Text = note.title;
                treeNode.Tag = note.parentId;
                allNode.Add(treeNode);

                node.Nodes.Add(newTitle, newTitle);
                node.Nodes[newTitle].Tag = note.id.ToString();
                DBHelper.AddNote(newTitle, NEW_HTML, note.parentId);
            }
        }

        public static int GetBiggestId()
        {
            int result = 0;
            List<Note> currentNotes = totalTitles;
            if (totalTitles.Count <= 0)
            {
                currentNotes = DBHelper.GetAllNotes();
            }
            foreach (var note in currentNotes)
            {
                if (note.id > result)
                {
                    result = note.id;
                }
            }

            return result;
        }

        public static void Modify(Form1 form1, string newTitle)
        {
            TreeNode selectedNode = form1.noteList.SelectedNode;
            
            for (int i = 0; i < totalTitles.Count; ++i)
            {
                if (totalTitles[i].id.ToString()
                    .Equals(selectedNode.Tag))
                {
                    Note note = new Note
                    {
                        id = totalTitles[i].id,
                        encrypted = totalTitles[i].encrypted,
                        title = newTitle,
                        parentId = totalTitles[i].parentId
                    };

                    totalTitles[i] = note;
                    
                    selectedNode.Text = newTitle;
                }
            }
            
        }

        public static int GetIdFromTitle(string title)
        {
            foreach (var note in totalTitles)
            {
                if (note.title.Equals(title))
                {
                    return note.id;
                }
            }
            return -1;
        }

        public static List<TreeNode> result = new List<TreeNode>();
        public static List<TreeNode> GetAllNodeTag(TreeNode node)
        {
            result.Add(node);
            for(int i = 0; i < node.Nodes.Count; ++i)
            {
                if (node.Nodes[i] == null)
                {
                    return result;
                }
                GetAllNodeTag(node.Nodes[i]);
            }
            return result;
        }

        public static Note GetNoteFromTitle(string title)
        {
            foreach(var note in totalTitles)
            {
                if (note.title.Equals(title))
                {
                    return note;
                }
            }
            return new Note();
        }
    }
}
