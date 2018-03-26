﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
        public string parentId;
    }
    /* 
     * 提供给TreeNode视图组件的tag标签所需要保存的内容
     */
    public struct Tags
    {
        public int id;
        public string parentId;
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
                if (tags.parentId.Equals(DBHelper.NO_BELONG))
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
                if (totalNotes[i].id.ToString().Equals(tags.parentId))
                {
                    allNodes[i].Nodes.Add(treeNode);
                }
            }
        }
        
        // 添加一个笔记项（添加至数据库、临时list和视图）
        public static void AddItem(Form1 form1, string newTitle)
        {
            form1.noteList.Invoke(new Action(() =>
            {
                // 添加至数据库
                // TODO: 添加Note的方法还未完善，目前只添加了标题和HTML内容
                DBHelper.AddNote(newTitle, NEW_HTML);
                // 添加至临时list
                Note note = new Note
                {
                    title = newTitle,
                    id = GetBiggestId(),
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

                // 添加到视图（淘汰）
                //form1.noteList.Nodes.Add(newTitle, newTitle);
                // 将id和parentId保存到视图中
                //form1.noteList.Nodes[newTitle].Tag = note.id.ToString();
            }));
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
                note.parentId = tags1.id.ToString();
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
                // 淘汰
                //node.Nodes.Add(newTitle, newTitle);
                //node.Nodes[newTitle].Tag = note.id.ToString();
                // 添加至数据库
                DBHelper.AddNote(newTitle, NEW_HTML, note.parentId);
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
        public static void Modify(Form1 form1, string newTitle)
        {
            TreeNode selectedNode = form1.noteList.SelectedNode;
            
            for (int i = 0; i < totalNotes.Count; ++i)
            {
                Tags tags = (Tags)selectedNode.Tag;
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
                    
                    selectedNode.Text = newTitle;
                }
            }
            
        }

        // 工具类：通过Id来获得指定的Note
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
    }
}
