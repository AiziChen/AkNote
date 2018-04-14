using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace AkNote
{
    /*
     * 一个笔记项对象
     */
    public struct Note
    {
        public int id;
        public string title;
        public bool encrypted;
        public int parentId;
    }
    /* 
     * 提供给TreeNode视图组件的tag标签所需要保存的内容
     */
    public struct Tags
    {
        public int id;
        public int parentId;
    }


    /**
     * 笔记列表的操作类
     * @author QuanyeChen
     */
    public class ListHelper
    {
        public static readonly string NEW_HTML = "<ul><li> &nbsp;</li></ul>";
        // 以下定义了两个为临时list，目的是加快笔记的读取速度
        public static List<Note> totalNotes { get; set; }
        public static List<TreeNode> allNodes { get; set; }

        private ListHelper() { }

        // 初始化笔记列表
        public static void InitList(Form1 form)
        {
            if (totalNotes == null)
            {
                totalNotes = new List<Note>();
            }
            if (allNodes == null)
            {
                allNodes = new List<TreeNode>();
            }
            Update(form);
        }

        // 更新笔记列表
        public static void Update(Form1 form)
        {
            totalNotes.Clear();
            allNodes.Clear();
            form.noteList.Nodes.Clear();

            foreach (var note in DBHelper.GetAllNotes())
            {
                // 保存所有的笔记项到totalNotes中
                totalNotes.Add(note);
                // 保存所有的笔记项到allNotes，但是是以TreeNode组件的形式保存的，并且它只保存了Tags(id、parentId)和title
                TreeNode treeNode = new TreeNode();
                Tags tags = new Tags
                {
                    id = note.id,
                    parentId = note.parentId
                };
                treeNode.Tag = tags;
                treeNode.Text = note.title;
                allNodes.Add(treeNode);
            }

            AddNodes(form);
        }

        // 在视图组件中添加并显示视图组件
        public static void AddNodes(Form1 form)
        {
            foreach (var node in allNodes)
            {
                Tags tags = (Tags)node.Tag;
                if (tags.parentId == DBHelper.NO_BELONG)
                {
                    // 直接添加作为根笔记项
                    form.noteList.Nodes.Add(node);
                } else
                {
                    AddSonNodes(node);
                }

            }

        }

        // 添加父项的子项
        public static void AddSonNodes(TreeNode treeNode)
        {
            Tags tags = (Tags)treeNode.Tag;
            for (int i = 0; i < totalNotes.Count; ++i)
            {
                if (totalNotes[i].id == tags.parentId)
                {
                    allNodes[i].Nodes.Add(treeNode);
                }
            }
        }

        // 添加一个笔记项（添加至数据库、临时list和视图）
        public static void AddItem(Form1 form1, string newTitle)
        {
            // 添加至临时list
            Note note = new Note
            {
                title = newTitle,
                id = GetBiggestId() + 1,
                encrypted = false,
                parentId = DBHelper.NO_BELONG
            };
            totalNotes.Add(note);

            TreeNode treeNode = new TreeNode();
            Tags tags = new Tags
            {
                id = note.id,
                parentId = note.parentId
            };
            treeNode.Tag = tags;
            treeNode.Text = note.title;
            allNodes.Add(treeNode);

            // 添加到视图
            form1.noteList.Nodes.Add(treeNode);
            // 添加至数据库
            // TODO: 添加Note的方法还未实现加密功能
            DBHelper.AddNote(newTitle, NEW_HTML, DBHelper.NO_BELONG);
        }

        // 对当前选中的Node添加新的子Node
        public static void AddNewNodeItem(Form1 form1, string newTitle)
        {
            TreeNode node = form1.noteList.SelectedNode;
            if (node != null)
            {
                Tags tags1 = (Tags)node.Tag;
                Note note = new Note();
                note.title = newTitle;
                note.id = GetBiggestId() + 1;
                note.encrypted = false;
                note.parentId = tags1.id;
                totalNotes.Add(note);

                TreeNode treeNode = new TreeNode();
                Tags tags2 = new Tags
                {
                    id = note.id,
                    parentId = note.parentId
                };
                treeNode.Tag = tags2;
                treeNode.Text = note.title;
                allNodes.Add(treeNode);

                // 添加至视图
                node.Nodes.Add(treeNode);
                // 添加至数据库
                // TODO: 添加Note的方法还未实现加密功能
                DBHelper.AddNote(newTitle, NEW_HTML, note.parentId);
            }
        }
        //
        // 删除该节点及其所有的子节点
        //
        public static void RemoveNode(TreeNode parentNode)
        {
            if (parentNode != null)
            {
                Tags parentTags = (Tags)parentNode.Tag;
                TreeNodeCollection treeNodes = parentNode.Nodes;
                for (int i = 0; i < treeNodes.Count; ++i)
                {
                    // 如果有还有子node
                    if (treeNodes.Count > 0)
                    {
                        ListHelper.RemoveNode(treeNodes[i]);
                    }
                    // 删除此节点
                    DBHelper.RemoveNote(parentTags.id);
                    ListHelper.totalNotes.Remove(ListHelper.GetNoteById(parentTags.id));
                    ListHelper.allNodes.Remove(parentNode);
                }
                // 直接删除当前节点
                DBHelper.RemoveNote(parentTags.id);
                ListHelper.totalNotes.Remove(ListHelper.GetNoteById(parentTags.id));
                ListHelper.allNodes.Remove(parentNode);
            }
        }

        // 取得最大笔记项中的最大的ID
        public static int GetBiggestId()
        {
            int result = 0;
            List<Note> currentNotes = totalNotes;
            if (totalNotes.Count <= 0)
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

        // 修改某笔记项
        public static void Modify(Tags tags, string newTitle)
        {
            for (int i = 0; i < totalNotes.Count; ++i)
            {
                if (totalNotes[i].id.ToString()
                    .Equals(tags.parentId))
                {
                    Note note = new Note
                    {
                        id = totalNotes[i].id,
                        encrypted = totalNotes[i].encrypted,
                        title = newTitle,
                        parentId = totalNotes[i].parentId
                    };

                    totalNotes[i] = note;
                }
            }
            
        }
        //
        // 工具类：通过Id来获得指定的Note
        //
        public static Note GetNoteById(int id)
        {
            foreach (var note in totalNotes)
            {
                if (note.id == id)
                {
                    return note;
                }
            }
            return new Note();
        }
        //
        // 展开该节点的所有父节点
        //
        public static void expandAllParent(TreeNode node)
        {
            // 若找不到该节点的父节点，就会返回null
            // 展开该节点的子节点
            if (node.Parent == null)
            {
                node.Expand();
            } else
            {
                expandAllParent(node.Parent);
                node.Expand();
            }
        }

        //
        // 返回找到的text为name的TreeNode的id集合
        //
        public static List<int> SearchResult(string name)
        {
            List<int> result = new List<int>();
            foreach (var node in allNodes)
            {
                if (name.Trim() != "" && node.Text.ToLower()
                    .Contains(name.ToLower()))
                {
                    Tags tags = (Tags)node.Tag;
                    result.Add(tags.id);
                }
            }
            return result;
        }

    }
}
