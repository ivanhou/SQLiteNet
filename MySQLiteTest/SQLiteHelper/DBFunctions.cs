using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;

namespace SQLiteHelper
{
    public class DBFunctions
    {
       private static string sql = "CREATE TABLE IF NOT EXISTS [O2Value] ([Id] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,"
                    + "[Channel1] INTEGER  NOT NULL, [Channel2] INTEGER  NOT NULL,"
                    + "[Channel3] INTEGER  NOT NULL,[Channel4] INTEGER  NOT NULL,[Channel5] INTEGER  NOT NULL,"
                    +"[Time] DATA  NOT NULL, [Text] VARCHAR(40)  NULL)";

        public static string ConnectionStringSQLite
        {
            get
            {
                if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Database"))
                {
                    System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\Database");
                }

                string database = AppDomain.CurrentDomain.BaseDirectory + "\\Database\\O2Monitor.db";
                string connectionString = @"Data Source=" + Path.GetFullPath(database);

                return connectionString;
            }
        }

        public static int CreateO2ValueTableStringSQLite()
        {
            int ret;
            
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionStringSQLite))//创建连接  
            {
                conn.Open();//打开数据库，若文件不存在会自动创建  

                SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
                ret = cmdCreateTable.ExecuteNonQuery();//如果表不存在，创建数据表  

            }

            return ret;
        }

        public static int GetLastId()
        {
            int id =-1;

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionStringSQLite))
                {
                    //SQLiteConnection conn = null;
                    //conn = new SQLiteConnection(ConnectionStringSQLite);//创建数据库实例，指定文件位置  
                    conn.Open();//打开数据库，若文件不存在会自动创建

                    SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
                    int ret = cmdCreateTable.ExecuteNonQuery();//如果表不存在，创建数据表  

                    string sql1 = @"SELECT * FROM O2Value ORDER BY id";
                    SQLiteCommand cmdQ = new SQLiteCommand(sql1, conn);

                    SQLiteDataReader reader = cmdQ.ExecuteReader();

                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                    }

                    conn.Close();
                }
            }
            catch (System.Exception ex)
            {
                id = -1;
            }            

            return id;
        }

        public static void Insert(ref int id, int channel1, int channel2, int channel3, int channel4, int channel5,string text)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionStringSQLite))//创建连接  
            {
                conn.Open();//打开连接  

                using (SQLiteTransaction tran = conn.BeginTransaction())//实例化一个事务  
                {
                    SQLiteCommand cmd = new SQLiteCommand(conn);//实例化SQL命令
                    cmd.Transaction = tran;
                    cmd.CommandText = "insert into O2Value values(@id, @Channel1, @Channel2, @Channel3, @Channel4, @Channel5, @Time, @Text)";//设置带参SQL语句  

                    cmd.Parameters.AddRange(new[] {//添加参数  
                           new SQLiteParameter("@id", id + 1),  
                           new SQLiteParameter("@Channel1", channel1),  
                           new SQLiteParameter("@Channel2", channel2),  
                           new SQLiteParameter("@Channel3", channel3),  
                           new SQLiteParameter("@Channel4", channel4),  
                           new SQLiteParameter("@Channel5", channel5),  
                           new SQLiteParameter("@Time", DateTime.Now),  
                           new SQLiteParameter("@Text", text)   
                       });

                    cmd.ExecuteNonQuery();//执行查询  

                    SQLiteCommand cmd_id = new SQLiteCommand(conn);//实例化SQL命令
                    cmd_id.CommandText = "SELECT last_insert_rowid() as id from O2Value";//设置带参SQL语句
                    object nieuweID = cmd_id.ExecuteScalar();
                    id = int.Parse(nieuweID.ToString());
                    tran.Commit();//提交                                       
                }
            }

        }

        public static void DelectAll(out int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionStringSQLite))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;

                    conn.Open();
                    cmd.CommandText ="DELETE FROM O2Value";
                    cmd.ExecuteNonQuery();

                    conn.Close();
                    id = -1;
                }
            }
        }

        /// <summary>
        /// 释放数据库空间
        /// </summary>
        /// <param name="sqlStr">自动释放"PRAGMA auto_vacuum = 1",手动"VACUUM"</param>
        public static void ReleaseSpace(string sqlStr = "VACUUM")
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionStringSQLite))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = sqlStr;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

    }
}
