using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Predilatation
{
    public partial class DilatationForm : Form
    {
        private ChooseFunForm chooseFunForm = null;

        //当前状态
        public string strCurTip = "";
        public int nProValue = 0;
        private delegate void SetTipHandler();
        private delegate void ChangeStateHandler();

        //错误
        public string strerrmsg = "";

        //数据库操作对象
        public SqlServerHelper sqlhelper = null;

        //业务对象
        private DilatationBS dilatationbs = null;

        //界面变量
        public string strEstTyp = "";
        public string strStdTyp = "";
        public string strRuleTyp = "";
        public string strEstCellsPath = "";
        public string strDataResourcePath = "";

        private CancellationTokenSource cts = new CancellationTokenSource();
        public DilatationForm(ChooseFunForm chooseFun)
        {
            InitializeComponent();
            chooseFunForm = chooseFun;
        }

        private void DilatationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            chooseFunForm.Show();
        }

        /// <summary>
        /// 加载界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DilatationForm_Load(object sender, EventArgs e)
        {
            this.cmbCalType.SelectedIndex = 0;
            this.cmbChkRule.SelectedIndex = 0;
            this.cmbEstType.SelectedIndex = 0;
            this.progressBar.Visible = false;
        }

        private void cmbEstType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbEstType.SelectedIndex == 0 && this.cmbCalType.SelectedIndex == 0 &&
                this.cmbChkRule.SelectedIndex == 0)
                this.btnSelEstCells.Enabled = true;
            else
            {
                this.btnSelEstCells.Enabled = false;
                this.txtEstCellsPath.Text = "";
                strEstCellsPath = "";
            }
                
        }

        private void cmbCalType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbEstType.SelectedIndex == 0 && this.cmbCalType.SelectedIndex == 0 &&
                this.cmbChkRule.SelectedIndex == 0)
                this.btnSelEstCells.Enabled = true;
            else
            {
                this.btnSelEstCells.Enabled = false;
                this.txtEstCellsPath.Text = "";
                strEstCellsPath = "";
            }
                

        }

        private void cmbChkRule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbEstType.SelectedIndex == 0 && this.cmbCalType.SelectedIndex == 0 &&
                this.cmbChkRule.SelectedIndex == 0)
                this.btnSelEstCells.Enabled = true;
            else
            {
                this.btnSelEstCells.Enabled = false;
                this.txtEstCellsPath.Text = "";
                strEstCellsPath = "";
            }
                
        }

        private void btnSelDataSource_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK) 
                this.txtDataResourcePath.Text = this.folderBrowserDialog.SelectedPath;
        }

        private void btnSelEstCells_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
                this.txtEstCellsPath.Text = this.openFileDialog.FileName;
        }

        /// <summary>
        /// 计算扩容任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalculate_Click(object sender, EventArgs e)
        {
            TipReFresher trf = new TipReFresher();
            dilatationbs = new DilatationBS(this,trf);
            strerrmsg = Check();
            if (string.IsNullOrEmpty(strerrmsg))
            {
                //界面变量赋值
                strEstTyp = dilatationbs.GetResType(this.cmbEstType.SelectedItem.ToString());
                strStdTyp = dilatationbs.GetResType(this.cmbCalType.SelectedItem.ToString());
                strRuleTyp = dilatationbs.GetResType(this.cmbChkRule.SelectedItem.ToString());
                strEstCellsPath = this.txtEstCellsPath.Text;
                strDataResourcePath = this.txtDataResourcePath.Text;

                //清空错误信息
                strerrmsg = "";

                //实例化数据库操作类
                sqlhelper = new SqlServerHelper();
                this.progressBar.Visible = true;
                this.btnCalculate.Enabled = false;
                
                //业务开始
                trf.CurTip = ReFreshTip;
                trf.ChangeState = ChangeState;
                Task start = new Task(CalculateStart,trf);
                start.Start();
            }
            else
            {
                MessageBox.Show(strerrmsg, "提示");
            }
        }


        /// <summary>
        /// 计算扩容业务
        /// </summary>
        /// <param name="trf"></param>
        private void CalculateStart(object trf)
        {
            try
            {
                DateTime timeStart = DateTime.Now;
                nProValue = 0;

                //导入计算小区
                dilatationbs.ImportEstCells();
                if (string.IsNullOrEmpty(strerrmsg))
                {
                    //导入数据源
                    dilatationbs.ImportResource();
                    if (string.IsNullOrEmpty(strerrmsg))
                    {
                        //创建结果表
                        dilatationbs.CreateResTable();
                        //写入结果表信息
                        dilatationbs.UpdateResTable();

                        //导出结果到Excel
                        ExportToExcel(trf);
                    }
                }

                if (string.IsNullOrEmpty(strerrmsg))
                {
                    strCurTip = "计算结束！";
                    ((TipReFresher)trf).CurTip();
                    double elapsedTimeInSeconds = DateTime.Now.Subtract(timeStart).TotalSeconds;
                    MessageBox.Show("总共耗时：" + elapsedTimeInSeconds.ToString());
                }
                else
                {
                    ((TipReFresher) trf).ChangeState();
                    MessageBox.Show(strerrmsg, "提示");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                strerrmsg = e.Message;
            }
        }

        /// <summary>
        /// 导出结果到Excel
        /// </summary>
        /// <param name="trf"></param>
        private void ExportToExcel(object trf)
        {
            SqlServerHelper sqlserver = new SqlServerHelper();
            strCurTip = "正在导出结果到Excel，请稍后......";
            nProValue = 50;
            ((TipReFresher)trf).CurTip();
            DataTable redt = sqlserver.GetResTable("扩容结果");
            string filename = Excel_CSVHelper.DataTableToExcel(redt);
            if (File.Exists(filename))
            {
                nProValue = 100;
                ((TipReFresher)trf).CurTip();
                DialogResult diars = MessageBox.Show("扩容结果导出完成，是否现在打开？", "提示", MessageBoxButtons.YesNo);
                if (diars == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(filename);
                }
            }
        }


       

        /// <summary>
        /// 输入有效性验证
        /// </summary>
        /// <returns></returns>
        private string Check()
        {
            if (string.IsNullOrEmpty(this.txtEstCellsPath.Text))
            {
                if (this.cmbEstType.SelectedIndex == 0 || this.cmbCalType.SelectedIndex == 0 ||
                    this.cmbChkRule.SelectedIndex == 0)
                {
                    return "预估方式，计算方式，判断规则填写不完整！";
                }
               
            }

            if (string.IsNullOrEmpty(this.txtDataResourcePath.Text))
                return "请选择数据源的路径！";

            return "";
        }

        private void ReFreshTip()
        { 
            if (this.labTip.InvokeRequired)
            {
                SetTipHandler set = new SetTipHandler(ReFreshTip);
                this.BeginInvoke(set);
            }
            else
            {
                this.labTip.Text = strCurTip;

                if (strCurTip == "计算结束！")
                    this.btnCalculate.Enabled = true;

                if (nProValue >= 100)
                {
                    this.progressBar.Value = 100;
                    this.progressBar.Visible = false;
                }
                else
                {
                    this.progressBar.Visible = true;
                    this.progressBar.Value = nProValue;
                }
            }
        }

        private void ChangeState()
        {
            if (this.btnCalculate.InvokeRequired)
            {
                ChangeStateHandler set = new ChangeStateHandler(ChangeState);
                this.BeginInvoke(set);
            }
            else
            {
                this.btnCalculate.Enabled = true;
                this.progressBar.Visible = false;
                this.labTip.Text = "计算结束！";
            }
        }
    }
}
