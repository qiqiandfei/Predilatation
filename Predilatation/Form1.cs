using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ADOX;
using System.Data.OleDb;
using DAO = Microsoft.Office.Interop.Access.Dao;
using Application = System.Windows.Forms.Application;
using Form = System.Windows.Forms.Form;
using System.Threading;
using System.Data.Common;

namespace Predilatation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static string strDbpath = "";
        private static string strSelPath = "";

        private void Selfiles_Click(object sender, EventArgs e)
        {
            if (this.folderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = this.folderBrowser.SelectedPath;
                strSelPath = this.textBox1.Text;
                strDbpath = strSelPath + "\\DB.mdb";
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(this.textBox1.Text);

            if(string.IsNullOrEmpty(strDbpath))
            {
                MessageBox.Show("请选择路径！");
                return;
            }

            try
            {
                if (File.Exists(strDbpath))
                    File.Delete(strDbpath);

                //建库，建表
                if (CreateAccessDb(strDbpath))
                {
                    //写数据
                    DateTime start = DateTime.Now;

                    TaskScheduler ts = TaskScheduler.FromCurrentSynchronizationContext();
                    Task task = new Task(() =>
                    {
                        Parallel.For(0, files.Length, i =>
                        {
                            string path = files[i];
                            if (Path.GetExtension(path).ToLower() == ".csv")
                                InsertTable(path);

                        });
                    });

                    task.ContinueWith((t) =>
                    {
                        double elapsedTimeInSeconds = DateTime.Now.Subtract(start).TotalSeconds;
                        MessageBox.Show("总共耗时：" + elapsedTimeInSeconds.ToString());
                    }, ts);

                    task.Start();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("程序出现错误：" + exception.Message,"提示");
            }
        }

        /// <summary>
        /// 写入Access
        /// </summary>
        /// <param name="path">csv文件路径</param>
        private void InsertTable(string path)
        {
            string strTableName = Path.GetFileNameWithoutExtension(path);
            DAO.DBEngine dbEngine = new DAO.DBEngine();
            DAO.Database db = dbEngine.OpenDatabase(strDbpath);
            string sql = "SELECT * INTO [" + strTableName + "] FROM [Text;FMT=Delimited;DATABASE=" + strSelPath + ";HDR=No].[" + strTableName + ".csv]";
            db.Execute(sql);

            //删除垃圾数据
            sql = "delete from " + strTableName + " where F2 is null or  F2 = '时间'";
            db.Execute(sql);

            //增加主键
            sql = "alter table " + strTableName + " add PRIMARY KEY(F1,F2)";
            db.Execute(sql);
            db.Close();
        }

        /// <summary>
        /// 创建access数据库
        /// </summary>
        /// <param name="filePath">数据库文件的全路径，如 D:\\NewDb.mdb</param>
        public static bool CreateAccessDb(string filePath)
        {
            
            if (!File.Exists(filePath))
            {
                Catalog catalog = new Catalog();
                catalog.Create("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Jet OLEDB:Engine Type=5");
            }
            return true;
        }
    }
}
