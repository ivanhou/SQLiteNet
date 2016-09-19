using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;

namespace MySQLiteTest
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        string dbPath;
        int _id, _id1,_id2;

        private void frmMain_Load(object sender, EventArgs e)
        {

            SQLiteConnection conn = null;

            this.dbPath = "Data Source =" + Environment.CurrentDirectory + "/Contact.db";
            conn = new SQLiteConnection(dbPath);//创建数据库实例，指定文件位置  
            conn.Open();//打开数据库，若文件不存在会自动创建  

            //string sql = "CREATE TABLE IF NOT EXISTS student(id integer, name varchar(20), sex varchar(2));";//建表语句  

           string sql = "CREATE TABLE  IF NOT EXISTS [Contact] ([id] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL, [Channel1] vARCHAR(50)  NULL, [Channel2] vARCHAR(50)  NULL)";

            SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
            int ret = cmdCreateTable.ExecuteNonQuery();//如果表不存在，创建数据表  

            System.Diagnostics.Stopwatch mWatch = new System.Diagnostics.Stopwatch();
            mWatch.Start();

            sql = @"SELECT * FROM Contact ORDER BY id";

            SQLiteCommand cmdQ = new SQLiteCommand(sql, conn);

            SQLiteDataReader reader = cmdQ.ExecuteReader();
            
            while (reader.Read())
            {
                this._id = reader.GetInt32(0);
            }

            mWatch.Stop();
            Console.WriteLine("Time:{0}  id:{1}", mWatch.Elapsed, this._id);


            string sql1 = "CREATE TABLE IF NOT EXISTS [O2Value] ([Id] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,"
                + "[Channel1] INTEGER  NOT NULL, [Channel2] INTEGER  NOT NULL,"
                +"[Channel3] INTEGER  NOT NULL,[Channel4] INTEGER  NOT NULL,[Channel5] INTEGER  NOT NULL,[Time] DATA  NOT NULL)";

            cmdCreateTable = new SQLiteCommand(sql1, conn);
            int ret1 = cmdCreateTable.ExecuteNonQuery();//如果表不存在，创建数据表  

            mWatch.Restart();

            sql1 = @"SELECT * FROM O2Value ORDER BY id";
            cmdQ = new SQLiteCommand(sql1, conn);

            reader = cmdQ.ExecuteReader();

            while (reader.Read())
            {
                this._id1 = reader.GetInt32(0);
            }

            mWatch.Stop();
            Console.WriteLine("Time:{0}  id1:{1}", mWatch.Elapsed, this._id1);

            conn.Close();

           int rct = SQLiteHelper.DBFunctions.CreateO2ValueTableStringSQLite();

           this._id2 = SQLiteHelper.DBFunctions.GetLastId();
        }

        private void btnWriteData_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch mWatch = new System.Diagnostics.Stopwatch();
            mWatch.Start();

            using (SQLiteConnection conn = new SQLiteConnection(this.dbPath))//创建连接  
            {
                conn.Open();//打开连接  
                
                using (SQLiteTransaction tran = conn.BeginTransaction())//实例化一个事务  
                {
                    for (int i = 1; i < 100000; i++)
                    {
                        SQLiteCommand cmd = new SQLiteCommand(conn);//实例化SQL命令
                        cmd.Transaction = tran;
                        cmd.CommandText = "insert into Contact values(@id, @Channel1, @Channel2)";//设置带参SQL语句  
                        
                        cmd.Parameters.AddRange(new[] {//添加参数  
                           new SQLiteParameter("@id", this._id + i),  
                           new SQLiteParameter("@Channel1", "Kris1"),  
                           new SQLiteParameter("@Channel2", "Boriau1")  
                       });
                        cmd.ExecuteNonQuery();//执行查询  
                    }

                    SQLiteCommand cmd_id = new SQLiteCommand(conn);//实例化SQL命令
                    cmd_id.CommandText = "SELECT last_insert_rowid() as id from Contact";//设置带参SQL语句
                    object nieuweID = cmd_id.ExecuteScalar();
                    this._id = int.Parse(nieuweID.ToString());
                    tran.Commit();//提交                                       
                }
            }

            mWatch.Stop();

            Console.WriteLine("Time1:{0}  id:{1}", mWatch.Elapsed, this._id);

        }

        private void btnWriteTable2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch mWatch = new System.Diagnostics.Stopwatch();
            mWatch.Start();

            System.Random channel1 = new System.Random(105);
            System.Random channel2 = new System.Random(120);
            System.Random channel3 = new System.Random(140);
            System.Random channel4 = new System.Random(150);
            System.Random channel5 = new System.Random(160);


            using (SQLiteConnection conn = new SQLiteConnection(this.dbPath))//创建连接  
            {
                conn.Open();//打开连接  

                using (SQLiteTransaction tran = conn.BeginTransaction())//实例化一个事务  
                {
                    for (int i = 1; i < 10; i++)
                    {
                        SQLiteCommand cmd = new SQLiteCommand(conn);//实例化SQL命令
                        cmd.Transaction = tran;
                        cmd.CommandText = "insert into O2Value values(@id, @Channel1, @Channel2, @Channel3, @Channel4, @Channel5, @Time)";//设置带参SQL语句  

                        cmd.Parameters.AddRange(new[] {//添加参数  
                           new SQLiteParameter("@id", this._id1 + i),  
                           new SQLiteParameter("@Channel1", channel1.Next(100,200)),  
                           new SQLiteParameter("@Channel2", channel2.Next(100,200)),  
                           new SQLiteParameter("@Channel3", channel3.Next(100,200)),  
                           new SQLiteParameter("@Channel4", channel4.Next(100,200)),  
                           new SQLiteParameter("@Channel5", channel5.Next(100,200)),  
                           new SQLiteParameter("@Time", DateTime.Now)   
                       });
                        cmd.ExecuteNonQuery();//执行查询  
                    }

                    SQLiteCommand cmd_id = new SQLiteCommand(conn);//实例化SQL命令
                    cmd_id.CommandText = "SELECT last_insert_rowid() as id from O2Value";//设置带参SQL语句
                    object nieuweID = cmd_id.ExecuteScalar();
                    this._id1 = int.Parse(nieuweID.ToString());
                    tran.Commit();//提交                                       
                }
            }

            mWatch.Stop();

            Console.WriteLine("Time1:{0}  id:{1}", mWatch.Elapsed, this._id1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Random channel1 = new System.Random(105);
            System.Random channel2 = new System.Random(120);
            System.Random channel3 = new System.Random(140);
            System.Random channel4 = new System.Random(150);
            System.Random channel5 = new System.Random(160);

            SQLiteHelper.DBFunctions.Insert(ref this._id2, channel1.Next(100, 300), channel2.Next(100, 300), channel3.Next(100, 300), channel4.Next(100, 300), channel5.Next(100, 300));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SQLiteHelper.DBFunctions.DelectAll(out this._id2);
            SQLiteHelper.DBFunctions.ReleaseSpace();
        }

    }
}
