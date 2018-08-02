using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Common;
using System.Windows.Forms;

namespace AkNote
{
    /// <summary>
    /// DAO层组件
    /// </summary>
    public class DBHelper
    {
        private DBHelper() { }

        // 首笔记项处于笔记列表的最顶端，它没有属于任何的父笔记项
        // 没有父笔记项
        public static readonly int NO_BELONG = -1;

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
                ", 标题 varchar(255), 加密 boolean, 内容 LONGTEXT, 父ID integer)";
            cmd.ExecuteNonQuery();
            // create Settings table
            cmd.CommandText = "CREATE TABLE if not exists Settings(编号 integer primary key autoincrement, 修改日期 datetime" +
                ", 手势密码 varchar(100), PC密码 varchar(100))";
            cmd.ExecuteNonQuery();
        }
        //
        // Add a note
        // Default is root-note
        //
        public static void AddNote(string title, string content, int parentId, bool encrypted = false)
        {
            DateTime date = DateTime.Now;
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "INSERT INTO Notebook (日期, 访问日期, 加密, 标题, 内容, 父ID)" +
                " VALUES (@date, @visitDate, @encrypted, @title, @content, @parentId)";
            cmd.Parameters.Add(new SQLiteParameter("@date", date));
            cmd.Parameters.Add(new SQLiteParameter("@visitDate", date));
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
            cmd.CommandText = "SELECT * FROM Notebook";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Note note = new Note
                {
                    id = int.Parse(reader["编号"].ToString()),
                    encrypted = bool.Parse(reader["加密"].ToString()),
                    title = reader["标题"].ToString(),
                    parentId = int.Parse(reader["父ID"].ToString())
                };

                result.Add(note);
            }
            reader.Close();

            return result;
        }
        //
        // Base ID get content
        //
        public static string GetContent(int id)
        {
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "SELECT * FROM Notebook WHERE 编号 = @ID";
            cmd.Parameters.Add(new SQLiteParameter("@ID", id));
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string result = reader["内容"].ToString();
                reader.Close();
                return result;
            }
            else
            {
                reader.Close();
                return "";
            }
        }
        //
        // Delete note by id
        //
        public static void RemoveNote(int id)
        {
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "DELETE FROM Notebook WHERE 编号=@ID";
            cmd.Parameters.Add(new SQLiteParameter("@ID", id));
            cmd.ExecuteNonQuery();
        }

        //
        // Modify title
        //
        public static void ModifyTitle(int id, string newTitle)
        {
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "UPDATE Notebook SET 标题=@newTitle WHERE 编号=@ID";
            cmd.Parameters.Add(new SQLiteParameter("@newTitle", newTitle));
            cmd.Parameters.Add(new SQLiteParameter("@ID", id));
            cmd.ExecuteNonQuery();
        }
        //
        // Modify a content
        //
        internal static void ModifyContent(int id, string content)
        {
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "UPDATE Notebook SET 内容=@content WHERE 编号=@ID";
            cmd.Parameters.Add(new SQLiteParameter("@content", content));
            cmd.Parameters.Add(new SQLiteParameter("@ID", id));
            cmd.ExecuteNonQuery();
        }

        //
        // Get The maximum ID
        //
        public static int GetMaxId()
        {
            SQLiteCommand cmd = GetCommand();
            cmd.CommandText = "SELECT MAX(编号) AS 最大编号 FROM Notebook";
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) {
                int result = Int32.Parse(reader["最大编号"].ToString());
                reader.Close();
                return result;
            }
            else
            {
                reader.Close();
                return 0;
            }
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
