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
        //数据库文件路径
        private static string strDbpath = "";
        //分析文件夹
        private static string strSelPath = "";

        /// <summary>
        /// 选择分析文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selfiles_Click(object sender, EventArgs e)
        {
            if (this.folderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = this.folderBrowser.SelectedPath;
                strSelPath = this.textBox1.Text;
                strDbpath = strSelPath + "\\DB.mdb";
            }
        }

        /// <summary>
        /// 开始分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(this.textBox1.Text);

            

            try
            {
                if (File.Exists(strDbpath))
                    File.Delete(strDbpath);

                //建库，建表
                if (AccessHelper.CreateAccessDb(strDbpath))
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
                                AccessHelper.InsertTable(path,strDbpath,strSelPath);

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

        private string Check()
        {
            string strErrMsg = "";
            if (string.IsNullOrEmpty(this.textBox1.Text))
            {
                strErrMsg = "请选择路径！";
            }

            try
            {
                string[] files = Directory.GetFiles(this.textBox1.Text);
                if (files.Length == 0)
                {
                    strErrMsg = "所选文件夹无效！";
                    return strErrMsg;
                }

                bool bflg = false;
                foreach (var file in files)
                {
                    if (Path.GetExtension(file).ToLower() == ".csv")
                    {
                        bflg = true;
                        break;
                    }
                }

                if (!bflg)
                {
                    strErrMsg = "所选文件夹中不存在有效文件！";
                    return strErrMsg;
                }
            }
            catch (Exception e)
            {
                strErrMsg = e.Message;
                return strErrMsg;
            }
            return strErrMsg;
        }
       
    }
}
