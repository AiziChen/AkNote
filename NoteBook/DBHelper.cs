using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Common;
using System.Windows.Forms;

namespace NoteBook
{
    /// <summary>
    /// DAO层组件
    /// </summary>
    public class DBHelper
    {
        private DBHelper() { }

        // 首笔记项处于笔记列表的最顶端，它没有属于任何的父笔记项
        // 没有父笔记项
        public static readonly string NO_BELONG = "-1";

        private static SQLiteConnection connection;
        private static SQLiteCommand cmd;
        //
        // Get DB connection, then return its command
        //
        public static SQLiteCommand GetCommand()
        {
            if (connection == null)
            {
                connection = new SQLiteConnection("Data Source=.\\note.db;Version=3;New=True;Compress=True;");
                connection.Open();
                cmd = connection.CreateCommand();
            }

            return cmd;
        }
        //
        // Create tables: if not exist
        //
        public static void CreateTables()
        {
            SQLiteCommand cmd = GetCommand();
            // create Notebook table
            cmd.CommandText = "CREATE TABLE if not exists Notebook(编号 integer primary key autoincrement, 日期 datetime, 访问日期 datetime" +
                ", 标题 varchar(150), 加密 boolean, 内容 TEXT, 父ID varchar(255))";
            cmd.ExecuteNonQuery();
            // create Settings table
            cmd.CommandText = "CREATE TABLE if not exists Settings(编号 integer primary key autoincrement, 修改日期 datetime" +
                ", 手势密码 varchar(100), PC密码 varchar(100))";
            cmd.ExecuteNonQuery();
        }
        //
        // Add a note
        //
        public static void AddNote(string title, string content, string parentId = "-1", DateTime date = new DateTime()
            , DateTime visitDate = new DateTime(), bool encrypted = false)
        {
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "INSERT INTO Notebook (日期, 访问日期, 加密, 标题, 内容, 父ID)" +
                " VALUES (@date, @visitDate, @encrypted, @title, @content, @parentId)";
            cmd.Parameters.Add(new SQLiteParameter("@date", date));
            cmd.Parameters.Add(new SQLiteParameter("@visitDate", visitDate));
            cmd.Parameters.Add(new SQLiteParameter("@encrypted", encrypted));
            cmd.Parameters.Add(new SQLiteParameter("@title", title));
            cmd.Parameters.Add(new SQLiteParameter("@content", content));
            cmd.Parameters.Add(new SQLiteParameter("@parentId", parentId));
            cmd.ExecuteNonQuery();
        }
        //
        // Get all titles
        //
        public static List<Note> GetAllNotes()
        {
            List<Note> result = new List<Note>();
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "select * from Notebook";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Note note = new Note();
                note.id = int.Parse(reader["编号"].ToString());
                note.encrypted = bool.Parse(reader["加密"].ToString());
                note.title = reader["标题"].ToString();
                note.parentId = reader["父ID"].ToString();

                result.Add(note);
            }
            reader.Close();

            return result;
        }
        //
        // Update a note
        //
        internal static void UpdateNote(string title, string content)
        {
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "UPDATE Notebook SET 内容=@content WHERE 标题=@title";
            cmd.Parameters.Add(new SQLiteParameter("@content", content));
            cmd.Parameters.Add(new SQLiteParameter("@title", title));
            cmd.ExecuteNonQuery();
        }

        //
        // Base title get content
        //
        public static string GetContent(string title)
        {
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "select * from Notebook";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (((string)reader["标题"]).Equals(title))
                {
                    string result = reader["内容"].ToString();
                    reader.Close();
                    return result;
                }
            }
            reader.Close();

            return "";
        }
        //
        // Base ID get content
        //
        public static string GetContent(int id)
        {
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "select * from Notebook";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (((long)reader["编号"]) == id)
                {
                    string result = reader["内容"].ToString();
                    reader.Close();
                    return result;
                }
            }
            reader.Close();

            return "";
        }
        //
        // Delete same title
        public static void RemoveNote(string title)
        {
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "DELETE FROM Notebook WHERE 标题=@title";
            cmd.Parameters.Add(new SQLiteParameter("@title", title));
            cmd.ExecuteNonQuery();
        }

        //
        // Modify title
        //
        public static void ModifyTitle(string preTitle, string newTitle)
        {
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "UPDATE Notebook SET 标题=@newTitle where 标题=@preTitle";
            cmd.Parameters.Add(new SQLiteParameter("@newTitle", newTitle));
            cmd.Parameters.Add(new SQLiteParameter("@preTitle", preTitle));
            cmd.ExecuteNonQuery();
        }
        //
        // Close the DB connection
        //
        public static void DBClose(DbDataReader reader, SQLiteConnection connection)
        {
            if (reader != null)
            {
                reader.Close();
            }
            if (connection != null && connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }
    }
}
