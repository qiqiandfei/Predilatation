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



namespace Predilatation
{
    public partial class MainForm : Form
    {
        TextBox txtEdit_BU = new TextBox();
        TextBox txtEdit_BD = new TextBox();
        TextBox txtEdit_MU = new TextBox();
        TextBox txtEdit_MD = new TextBox();
        TextBox txtEdit_SU = new TextBox();
        TextBox txtEdit_SD = new TextBox();


        private static string strType = "";
        private static string strStdTyp = "";
        private static string strEstTyp = "";

        public static MainParam Param = new MainParam();


        public MainForm()
        {
            InitializeComponent();
            Init();
            txtEdit_BU.KeyDown += new KeyEventHandler(txtEdit_KeyDown);
            txtEdit_BD.KeyDown += new KeyEventHandler(txtEdit_KeyDown);
            txtEdit_MU.KeyDown += new KeyEventHandler(txtEdit_KeyDown);
            txtEdit_MD.KeyDown += new KeyEventHandler(txtEdit_KeyDown);
            txtEdit_SU.KeyDown += new KeyEventHandler(txtEdit_KeyDown);
            txtEdit_SD.KeyDown += new KeyEventHandler(txtEdit_KeyDown);
        }

        /// <summary>
        /// 界面初始化
        /// </summary>
        private void Init()
        {
            radCrtDB.Checked = false;
            radAlreadyDB.Checked = true;
            radFlow.Checked = true;
            progressBar.Visible = false;
            this.btnSelFiles_before.Enabled = false;
            this.btnSelFiles_after.Enabled = false;

            //隐藏节假日
            this.labCurNet.Visible = false;
            this.txtPath_CurNet.Visible = false;
            this.btnSelCurNet.Visible = false;
            this.radCurNet.Enabled = false;
        }

        //需要分析的文件
        private static string[] files_before = null;
        private static string[] files_after = null;
        private static string[] files_curnet = null;

        //当前状态
        public static string strCurTip = "";
        public static int nProValue = 0;
        private delegate void SetTipHandler();

        SqlServerHelper sqlserver = null;


        /// <summary>
        /// 选择分析文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelFiles_before_Click(object sender, EventArgs e)
        {
            if (this.folderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.txtPath_before.Text = this.folderBrowser.SelectedPath;
                files_before = Directory.GetFiles(this.txtPath_before.Text);
            }
        }

        /// <summary>
        /// 选择分析文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelFiles_after_Click(object sender, EventArgs e)
        {
            if (this.folderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.txtPath_after.Text = this.folderBrowser.SelectedPath;
                files_after = Directory.GetFiles(this.txtPath_after.Text);
            }
        }

        /// <summary>
        /// 选择分析文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelCurNet_Click(object sender, EventArgs e)
        {
            if (this.folderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.txtPath_CurNet.Text = this.folderBrowser.SelectedPath;
                files_curnet = Directory.GetFiles(this.txtPath_CurNet.Text);
            }
        }

        /// <summary>
        /// 开始分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, EventArgs e)
        {
            sqlserver = new SqlServerHelper();
            string strErrMsg = Check();
            if (string.IsNullOrEmpty(strErrMsg))
            {
                try
                {
                    this.progressBar.Visible = true;
                    this.Start.Enabled = false;
                    //业务开始
                    TipReFresher trf = new TipReFresher();
                    trf.CurTip = ReFreshTip;
                    Task start = new Task(BusinessStart, trf);
                    start.Start();

                }
                catch (Exception exception)
                {
                    MessageBox.Show("程序出现错误：" + exception.Message, "提示");
                }
            }
            else
            {
                MessageBox.Show("程序出现错误：" + strErrMsg, "提示");
            }

        }

        private void BusinessStart(object trf)
        {
            DateTime TimeStart = DateTime.Now;
            nProValue = 0;
            //获取选定状态
            SetType();
            if (this.radCrtDB.Checked)
            {
                if (this.radBeforeN.Checked)
                {
                    //创建前N天数据
                    DataCreate_Before(trf);
                }
                if (this.radAfterN.Checked)
                {
                    //创建后N天数据
                    DataCreate_After(trf);
                }
                if (this.radCurNet.Checked)
                {
                    //创建现网数据
                    DataCreate_CurNet(trf);
                }
                if (this.radAll.Checked)
                {
                    if (this.radHoliday.Checked)
                    {
                        //创建前N天数据
                        DataCreate_Before(trf);
                        //创建后N一天数据
                        DataCreate_After(trf);
                        //创建现网数据
                        DataCreate_CurNet(trf);
                        //同步前后数据
                        nProValue = 50;
                        strCurTip = "正在进行数据同步...";
                        ((TipReFresher)trf).CurTip();
                        sqlserver.DataSync("Tempdata_CurNet");
                        nProValue = 100;
                        ((TipReFresher)trf).CurTip();
                    }
                    else
                    {
                        //创建前N天数据
                        DataCreate_Before(trf);
                        //创建后N一天数据
                        DataCreate_After(trf);
                        //同步前后数据
                        nProValue = 50;
                        strCurTip = "正在进行数据同步...";
                        ((TipReFresher)trf).CurTip();
                        sqlserver.DataSync();
                        nProValue = 100;
                        ((TipReFresher)trf).CurTip();
                    }
                }
 
                RunByArea(trf);
            }
            else
            {
                RunByArea(trf);
            }
            strCurTip = "分析结束！";
            ((TipReFresher)trf).CurTip();

            double elapsedTimeInSeconds = DateTime.Now.Subtract(TimeStart).TotalSeconds;
            MessageBox.Show("总共耗时：" + elapsedTimeInSeconds.ToString());

            
            
        }

        /// <summary>
        /// 生成前N天数据业务
        /// </summary>
        /// <param name="trf"></param>
        private void DataCreate_Before(object trf)
        {
            //建库，建表
            strCurTip = "正在创建数据库...";
            nProValue = 100;
            ((TipReFresher)trf).CurTip();
            sqlserver.CreatDataBase();
            sqlserver.CreateTable("Tempdata_Before");
            //添加主键
            sqlserver.AddPrimary("Tempdata_Before", "PK_Tempdata_Before", "cellname,time");
            nProValue = 100;
            ((TipReFresher)trf).CurTip();
            //文件生成表格并写入数据库
            nProValue = 0;
            strCurTip = "正在生成前N天数据表格...";
            ((TipReFresher)trf).CurTip();
            sqlserver.GetTablefromFile(files_before, trf,"Tempdata_Before");

            //清洗无效数据
            nProValue = 50;
            strCurTip = "正在清洗前N天无效数据...";
            ((TipReFresher)trf).CurTip();
            sqlserver.DataClean(trf,"Tempdata_Before");

            
        }

        /// <summary>
        /// 生成后N天数据业务
        /// </summary>
        /// <param name="trf"></param>
        private void DataCreate_After(object trf)
        {
            sqlserver.CreateTable("Tempdata_After");
            //添加主键
            sqlserver.AddPrimary("Tempdata_After", "PK_Tempdata_After", "cellname,time");
            nProValue = 100;
            ((TipReFresher)trf).CurTip();
            //文件生成表格并写入数据库
            nProValue = 0;
            strCurTip = "正在生成后N天数据表格...";
            ((TipReFresher)trf).CurTip();
            sqlserver.GetTablefromFile(files_after, trf, "Tempdata_After");

            //清洗无效数据
            nProValue = 50;
            strCurTip = "正在清洗后N天无效数据...";
            ((TipReFresher)trf).CurTip();
            sqlserver.DataClean(trf, "Tempdata_After");
           
        }


        /// <summary>
        /// 生成后N天数据业务
        /// </summary>
        /// <param name="trf"></param>
        private void DataCreate_CurNet(object trf)
        {
            sqlserver.CreateTable("Tempdata_CurNet");
            //添加主键
            sqlserver.AddPrimary("Tempdata_CurNet", "PK_Tempdata_CurNet", "cellname,time");
            nProValue = 100;
            ((TipReFresher)trf).CurTip();
            //文件生成表格并写入数据库
            nProValue = 0;
            strCurTip = "正在生成现网7*24数据表格...";
            ((TipReFresher)trf).CurTip();
            sqlserver.GetTablefromFile(files_curnet, trf, "Tempdata_CurNet");

            //清洗无效数据
            nProValue = 50;
            strCurTip = "正在清洗现网7*24无效数据...";
            ((TipReFresher)trf).CurTip();
            sqlserver.DataClean(trf, "Tempdata_CurNet");

        }

       /// <summary>
       /// 前N天业务
       /// </summary>
       /// <param name="trf"></param>
        private void BS_BeforeN(object trf)
        {
            if (this.radFlow.Checked)
            {
                strType = "FlowBusy";
                nProValue = 50;
                strCurTip = "正在计算前N天流量自忙时...";
                ((TipReFresher)trf).CurTip();
                sqlserver.CalculateBusy_Flow("Tempdata_Before", "FlowBusy_Before");
                nProValue = 100;
                ((TipReFresher)trf).CurTip();
            }
            if (this.radUtilizaerate.Checked)
            {
                strType = "Utilizaerate";
                nProValue = 50;
                strCurTip = "正在计算前N天利用率自忙时...";
                ((TipReFresher)trf).CurTip();
                sqlserver.CalculateBusy_Utilizaerate("Tempdata_Before", "Utilizaerate_Before");
                nProValue = 100;
                ((TipReFresher)trf).CurTip();

            }

            //计算前N天扩容标准
            if (this.radGroupSHY.Checked || this.radGroupFHY.Checked || this.radDTMobile.Checked)
            {
                Calculate Cal = new Calculate(Param);
                nProValue = 50;
                strCurTip = "正在根据标准计算前N天平均扩容条件...";
                ((TipReFresher)trf).CurTip();

                Cal.CrtAvgData(strType, strStdTyp, trf, "Tempdata_Before", "Before");

                nProValue = 50;
                strCurTip = "正在根据标准计算前N天每天扩容条件...";
                ((TipReFresher)trf).CurTip();
                Cal.CrtDataDate(strType, strStdTyp, trf, "Tempdata_Before", "Before");
                nProValue = 100;
                ((TipReFresher)trf).CurTip();
            }

        }

        /// <summary>
        /// 后N天业务
        /// </summary>
        /// <param name="trf"></param>
        private void BS_AfterN(object trf)
        {
            if (this.radFlow.Checked)
            {
                strType = "FlowBusy";
                nProValue = 50;
                strCurTip = "正在计算后N天流量自忙时...";
                ((TipReFresher)trf).CurTip();
                sqlserver.CalculateBusy_Flow("Tempdata_After", "FlowBusy_After");
                nProValue = 100;
                ((TipReFresher)trf).CurTip();
            }
            if (this.radUtilizaerate.Checked)
            {
                strType = "Utilizaerate";
                nProValue = 50;
                strCurTip = "正在计算后N天利用率自忙时...";
                ((TipReFresher)trf).CurTip();
                sqlserver.CalculateBusy_Utilizaerate("Tempdata_After", "Utilizaerate_After");
                nProValue = 100;
                ((TipReFresher)trf).CurTip();

            }

            //计算后N天扩容标准
            if (this.radGroupSHY.Checked || this.radGroupFHY.Checked || this.radDTMobile.Checked)
            {
                Calculate Cal = new Calculate(Param);
                nProValue = 50;
                strCurTip = "正在根据标准计算后N天平均扩容条件...";
                ((TipReFresher)trf).CurTip();

                Cal.CrtAvgData(strType, strStdTyp, trf, "Tempdata_After", "After");


                nProValue = 50;
                strCurTip = "正在根据标准计算后N天每天扩容条件...";
                ((TipReFresher)trf).CurTip();
                Cal.CrtDataDate(strType, strStdTyp, trf, "Tempdata_After", "After");
                nProValue = 100;
                ((TipReFresher)trf).CurTip();
            }
        }

        /// <summary>
        /// 现网7*24
        /// </summary>
        /// <param name="trf"></param>
        private void BS_CurNet(object trf)
        {
            if (this.radFlow.Checked)
            {
                strType = "FlowBusy";
                nProValue = 50;
                strCurTip = "正在计算现网7*24流量自忙时...";
                ((TipReFresher)trf).CurTip();
                sqlserver.CalculateBusy_Flow("Tempdata_CurNet", "FlowBusy_CurNet");
                nProValue = 100;
                ((TipReFresher)trf).CurTip();
            }
            if (this.radUtilizaerate.Checked)
            {
                strType = "Utilizaerate";
                nProValue = 50;
                strCurTip = "正在计算现网7*24利用率自忙时...";
                ((TipReFresher)trf).CurTip();
                sqlserver.CalculateBusy_Utilizaerate("Tempdata_CurNet", "Utilizaerate_CurNet");
                nProValue = 100;
                ((TipReFresher)trf).CurTip();

            }

            //计算后N天扩容标准
            if (this.radGroupSHY.Checked || this.radGroupFHY.Checked || this.radDTMobile.Checked)
            {
                Calculate Cal = new Calculate(Param);
                nProValue = 50;
                strCurTip = "正在根据标准计算现网7*24平均扩容条件...";
                ((TipReFresher)trf).CurTip();

                Cal.CrtAvgData(strType, strStdTyp, trf, "Tempdata_CurNet", "CurNet");

                nProValue = 50;
                strCurTip = "正在根据标准计算现网7*24每天扩容条件...";
                ((TipReFresher)trf).CurTip();
                Cal.CrtDataDate(strType, strStdTyp, trf, "Tempdata_CurNet", "CurNet");
                nProValue = 100;
                ((TipReFresher)trf).CurTip();
            }
        }


        /// <summary>
        /// 根据范围计算
        /// </summary>
        /// <param name="trf"></param>
        private void RunByArea(object trf)
        {
            if (this.radBeforeN.Checked)
            {
                BS_BeforeN(trf);
            }
            if (this.radAfterN.Checked)
            {
                BS_AfterN(trf);
            }
            if (this.radAll.Checked)
            {
                if (this.radHoliday.Checked)
                {
                    //BS_BeforeN(trf);
                    //BS_AfterN(trf);
                    //BS_CurNet(trf);
                    MessageBox.Show("敬请期待~");
                }
                else
                {
                    //BS_BeforeN(trf);
                    //BS_AfterN(trf);
                    if (this.radDaily.Checked)
                    {
                        EstimateDaily daily = new EstimateDaily();
                        daily.CalculateEst(strType,strStdTyp,strEstTyp,trf);
                    }
                    if (this.radGivenRate.Checked)
                    {
                        MessageBox.Show("敬请期待~");
                    }
                }
               
            }
        }


        /// <summary>
        /// 输入有效性验证
        /// </summary>
        /// <returns></returns>
        private string Check()
        {
            string strErrMsg = "";
            

            try
            {
                if (this.radCrtDB.Checked)
                {
                    if (!radBeforeN.Checked && !radAfterN.Checked && !radAll.Checked && !this.radCurNet.Checked)
                    {
                        strErrMsg = "请选择计算范围！";
                        return strErrMsg;
                    }

                    if (this.radBeforeN.Checked)
                    {
                        if (!string.IsNullOrEmpty(this.txtPath_before.Text))
                        {
                            if (files_before.Length == 0)
                            {
                                strErrMsg = "前N天所选文件夹无效！";
                                return strErrMsg;
                            }

                            bool bflg = false;
                            foreach (var file in files_before)
                            {
                                if (Path.GetExtension(file).ToLower() != ".csv")
                                {
                                    bflg = true;
                                    break;
                                }
                            }

                            if (bflg)
                            {
                                strErrMsg = "前N天所选文件夹中不存在有效文件！";
                                return strErrMsg;
                            }
                        }
                        else
                        {
                            strErrMsg = "已选择的文件夹与计算范围不一致！";
                            return strErrMsg;
                        }
                    }

                    if (this.radAfterN.Checked)
                    {
                        if (!string.IsNullOrEmpty(this.txtPath_after.Text))
                        {
                            if (files_after.Length == 0)
                            {
                                strErrMsg = "后N天所选文件夹无效！";
                                return strErrMsg;
                            }

                            bool bflg = false;
                            foreach (var file in files_after)
                            {
                                if (Path.GetExtension(file).ToLower() != ".csv")
                                {
                                    bflg = true;
                                    break;
                                }
                            }

                            if (bflg)
                            {
                                strErrMsg = "后N天所选文件夹中不存在有效文件！";
                                return strErrMsg;
                            }
                        }
                        else
                        {
                            strErrMsg = "已选择的文件夹与计算范围不一致！";
                            return strErrMsg;
                        }
                    }

                    if (this.radCurNet.Checked)
                    {
                        if (!string.IsNullOrEmpty(this.txtPath_CurNet.Text))
                        {
                            if (files_curnet.Length == 0)
                            {
                                strErrMsg = "后N天所选文件夹无效！";
                                return strErrMsg;
                            }

                            bool bflg = false;
                            foreach (var file in files_curnet)
                            {
                                if (Path.GetExtension(file).ToLower() != ".csv")
                                {
                                    bflg = true;
                                    break;
                                }
                            }

                            if (bflg)
                            {
                                strErrMsg = "现网7*24所选文件夹中不存在有效文件！";
                                return strErrMsg;
                            }
                        }
                        else
                        {
                            strErrMsg = "已选择的文件夹与计算范围不一致！";
                            return strErrMsg;
                        }
                    }

                    if (this.radAll.Checked)
                    {
                        if (this.radHoliday.Checked)
                        {
                            if (string.IsNullOrEmpty(this.txtPath_before.Text) ||
                                string.IsNullOrEmpty(this.txtPath_after.Text) ||
                                string.IsNullOrEmpty(this.txtPath_CurNet.Text))
                            {
                                strErrMsg = "已选择的文件夹与计算范围不一致！";
                                return strErrMsg;
                            }

                            if (!string.IsNullOrEmpty(this.txtPath_before.Text) &&
                                !string.IsNullOrEmpty(this.txtPath_after.Text) &&
                                !string.IsNullOrEmpty(this.txtPath_CurNet.Text))
                            {
                                if (files_after.Length != files_before.Length || files_after.Length != files_curnet.Length || files_curnet.Length != files_before.Length)
                                {
                                    strErrMsg = "节日前后现网数文件数量不一致！";
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(this.txtPath_before.Text) ||
                                string.IsNullOrEmpty(this.txtPath_after.Text))
                            {
                                strErrMsg = "已选择的文件夹与计算范围不一致！";
                                return strErrMsg;
                            }

                            if (!string.IsNullOrEmpty(this.txtPath_before.Text) &&
                                !string.IsNullOrEmpty(this.txtPath_after.Text))
                            {
                                if (files_after.Length != files_before.Length)
                                {
                                    strErrMsg = "前N与后N天数文件数量不一致！";
                                }
                            }
                        }
                    }

                   
                  
                    
                }
                
                if (this.radAlreadyDB.Checked)
                {
                    strErrMsg = sqlserver.CheckDataBase();
                    //检查数据库是否存在
                    if (string.IsNullOrEmpty(strErrMsg))
                    {
                        if (!radBeforeN.Checked && !radAfterN.Checked && !radAll.Checked && !this.radCurNet.Checked)
                        {
                            strErrMsg = "请选择计算范围！";
                        }
                        //检查数据表是否存在
                        if (this.radBeforeN.Checked)
                        {
                            strErrMsg = sqlserver.CheckTable("Tempdata_Before");
                        }
                        if (this.radAfterN.Checked)
                        {
                            strErrMsg = sqlserver.CheckTable("Tempdata_After");
                        }
                        if (this.radCurNet.Checked)
                        {
                            strErrMsg = sqlserver.CheckTable("Tempdata_CurNet");
                        }
                        if (this.radAll.Checked)
                        {
                            if (this.radHoliday.Checked)
                            {
                                strErrMsg = sqlserver.CheckTable("Tempdata_Before");
                                if (string.IsNullOrEmpty(strErrMsg))
                                {
                                    strErrMsg = sqlserver.CheckTable("Tempdata_After");
                                    if (string.IsNullOrEmpty(strErrMsg))
                                    {
                                        strErrMsg = sqlserver.CheckTable("Tempdata_CurNet");
                                    }
                                }
                            }
                            else
                            {
                                strErrMsg = sqlserver.CheckTable("Tempdata_Before");
                                if (string.IsNullOrEmpty(strErrMsg))
                                {
                                    strErrMsg = sqlserver.CheckTable("Tempdata_After");
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        return strErrMsg;
                    }
                }
            }
            catch (Exception e)
            {
                strErrMsg = e.Message;
                return strErrMsg;
            }
            return strErrMsg;
        }

        /// <summary>
        /// 设置当前选定状态
        /// </summary>
        private void SetType()
        {
            if (this.radGroupFHY.Checked)
                strStdTyp = "Grp2018Fir";
            if(this.radGroupSHY.Checked)
                strStdTyp = "Grp2018Sec";
            if(this.radDTMobile.Checked)
                strStdTyp = "DTMobile";
            if (this.radDaily.Checked)
                strEstTyp = "Daily";
            if (this.radHoliday.Checked)
                strEstTyp = "Holiday";
            if (this.radGivenRate.Checked)
                strEstTyp = "GivenRate";
            if (this.radFlow.Checked)
                strType = "Flow";
            if (this.radUtilizaerate.Checked)
                strType = "Utilizaerate";
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

                if (strCurTip == "分析结束！")
                    this.Start.Enabled = true;
                
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


        //************************************ListBox***************************************************//
        private void txtEdit_KeyDown(object sender, KeyEventArgs e)
        {
            //Enter键 更新项并隐藏编辑框  
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                if (lstbox_BU.SelectedIndex != -1)
                {
                    lstbox_BU.Items[lstbox_BU.SelectedIndex] = txtEdit_BU.Text;
                    txtEdit_BU.Visible = false;
                }

                if (lstbox_BD.SelectedIndex != -1)
                {
                    lstbox_BD.Items[lstbox_BD.SelectedIndex] = txtEdit_BD.Text;
                    txtEdit_BD.Visible = false;
                }

                if (lstbox_MU.SelectedIndex != -1)
                {
                    lstbox_MU.Items[lstbox_MU.SelectedIndex] = txtEdit_MU.Text;
                    txtEdit_MU.Visible = false;
                }

                if (lstbox_MD.SelectedIndex != -1)
                {
                    lstbox_MD.Items[lstbox_MD.SelectedIndex] = txtEdit_MD.Text;
                    txtEdit_MD.Visible = false;
                }

                if (lstbox_SU.SelectedIndex != -1)
                {
                    lstbox_SU.Items[lstbox_SU.SelectedIndex] = txtEdit_SU.Text;
                    txtEdit_SU.Visible = false;
                }

                if (lstbox_SD.SelectedIndex != -1)
                {
                    lstbox_SD.Items[lstbox_SD.SelectedIndex] = txtEdit_SD.Text;
                    txtEdit_SD.Visible = false;
                }
                    
                
            }
            //Esc键 直接隐藏编辑框  
            if (e.KeyCode == System.Windows.Forms.Keys.Escape)
            {
                if (lstbox_BU.SelectedIndex != -1)
                    txtEdit_BU.Visible = false;

                if (lstbox_BD.SelectedIndex != -1)
                    txtEdit_BD.Visible = false;

                if (lstbox_MU.SelectedIndex != -1)
                    txtEdit_MU.Visible = false;

                if (lstbox_MD.SelectedIndex != -1)
                    txtEdit_MD.Visible = false;

                if (lstbox_SU.SelectedIndex != -1)
                    txtEdit_SU.Visible = false;

                if (lstbox_SD.SelectedIndex != -1)
                    txtEdit_SD.Visible = false;
            }

        }

        private void lstbox_BU_DoubleClick(object sender, EventArgs e)
        {
            int itemSelected = lstbox_BU.SelectedIndex;
            if (itemSelected == -1)
                return;
            string itemText = lstbox_BU.Items[itemSelected].ToString();

            Rectangle rect = lstbox_BU.GetItemRectangle(itemSelected);
            txtEdit_BU.Parent = lstbox_BU;
            txtEdit_BU.Bounds = rect;
            txtEdit_BU.Multiline = true;
            txtEdit_BU.Visible = true;
            txtEdit_BU.Text = itemText;
            txtEdit_BU.Height = 18;
            txtEdit_BU.Focus();
            txtEdit_BU.SelectAll();  
        }

        private void lstbox_BU_MouseClick(object sender, MouseEventArgs e)
        {
            txtEdit_BU.Visible = false;
        }

        private void lstbox_BD_DoubleClick(object sender, EventArgs e)
        {
            int itemSelected = lstbox_BD.SelectedIndex;
            if (itemSelected == -1)
                return;
            string itemText = lstbox_BD.Items[itemSelected].ToString();

            Rectangle rect = lstbox_BD.GetItemRectangle(itemSelected);
            txtEdit_BD.Parent = lstbox_BD;
            txtEdit_BD.Bounds = rect;
            txtEdit_BD.Multiline = true;
            txtEdit_BD.Visible = true;
            txtEdit_BD.Text = itemText;
            txtEdit_BD.Height = 18;
            txtEdit_BD.Focus();
            txtEdit_BD.SelectAll();  
        }

        private void lstbox_BD_MouseClick(object sender, MouseEventArgs e)
        {
            txtEdit_BD.Visible = false;
        }

        private void lstbox_MU_DoubleClick(object sender, EventArgs e)
        {
            int itemSelected = lstbox_MU.SelectedIndex;
            if (itemSelected == -1)
                return;
            string itemText = lstbox_MU.Items[itemSelected].ToString();
            Rectangle rect = lstbox_MU.GetItemRectangle(itemSelected);
            txtEdit_MU.Parent = lstbox_MU;
            txtEdit_MU.Bounds = rect;
            txtEdit_MU.Multiline = true;
            txtEdit_MU.Visible = true;
            txtEdit_MU.Text = itemText;
            txtEdit_MU.Height = 18;
            txtEdit_MU.Focus();
            txtEdit_MU.SelectAll();  
        }

        private void lstbox_MU_MouseClick(object sender, MouseEventArgs e)
        {
            txtEdit_MU.Visible = false;
        }

        private void lstbox_MD_DoubleClick(object sender, EventArgs e)
        {
            int itemSelected = lstbox_MD.SelectedIndex;
            if (itemSelected == -1)
                return;
            string itemText = lstbox_MD.Items[itemSelected].ToString();

            Rectangle rect = lstbox_MD.GetItemRectangle(itemSelected);
            txtEdit_MD.Parent = lstbox_MD;
            txtEdit_MD.Bounds = rect;
            txtEdit_MD.Multiline = true;
            txtEdit_MD.Visible = true;
            txtEdit_MD.Text = itemText;
            txtEdit_MD.Height = 18;
            txtEdit_MD.Focus();
            txtEdit_MD.SelectAll();  
        }

        private void lstbox_MD_MouseClick(object sender, MouseEventArgs e)
        {
            txtEdit_MD.Visible = false;
        }

        private void lstbox_SU_DoubleClick(object sender, EventArgs e)
        {
            int itemSelected = lstbox_SU.SelectedIndex;
            if (itemSelected == -1)
                return;
            string itemText = lstbox_SU.Items[itemSelected].ToString();

            Rectangle rect = lstbox_SU.GetItemRectangle(itemSelected);
            txtEdit_SU.Parent = lstbox_SU;
            txtEdit_SU.Bounds = rect;
            txtEdit_SU.Multiline = true;
            txtEdit_SU.Visible = true;
            txtEdit_SU.Text = itemText;
            txtEdit_SU.Height = 18;
            txtEdit_SU.Focus();
            txtEdit_SU.SelectAll();  
        }

        private void lstbox_SU_MouseClick(object sender, MouseEventArgs e)
        {
            txtEdit_SU.Visible = false;
        }

        private void lstbox_SD_DoubleClick(object sender, EventArgs e)
        {
            int itemSelected = lstbox_SD.SelectedIndex;
            if (itemSelected == -1)
                return;
            string itemText = lstbox_SD.Items[itemSelected].ToString();

            Rectangle rect = lstbox_SD.GetItemRectangle(itemSelected);
            txtEdit_SD.Parent = lstbox_SD;
            txtEdit_SD.Bounds = rect;
            txtEdit_SD.Multiline = true;
            txtEdit_SD.Visible = true;
            txtEdit_SD.Text = itemText;
            txtEdit_SD.Height = 18;
            txtEdit_SD.Focus();
            txtEdit_SD.SelectAll();
        }

        private void lstbox_SD_MouseClick(object sender, MouseEventArgs e)
        {
            txtEdit_SD.Visible = false;
        }

       
        //************************************ListBox***************************************************//

        //************************************根据模板初始化********************************************//
        private void radGroupFHY_CheckedChanged(object sender, EventArgs e)
        {
            //大包
            this.txtERAB_B.Text = "1000";
            this.cmbERAB_B.SelectedIndex = 0;
            lstbox_BU.Items.Clear();
            lstbox_BU.Items.Add("平均RRC有效用户数>=10");
            lstbox_BU.Items.Add("上行PUSCH PRB利用率>=50");
            lstbox_BU.Items.Add("上行流量(G)>=0.3");

            lstbox_BD.Items.Clear();
            lstbox_BD.Items.Add("平均RRC有效用户数>=10");
            lstbox_BD.Items.Add("下行PDSCH PRB利用率>=70或下行PDCCH CCE利用率>=50");
            lstbox_BD.Items.Add("下行流量(G)>=5");

            //中包
            this.txtERAB_MS.Text = "300";
            this.txtERAB_ME.Text = "1000";
            this.cmbERAB_MS.SelectedIndex = 0;
            this.cmbERAB_ME.SelectedIndex = 0;
            lstbox_MU.Items.Clear();
            lstbox_MU.Items.Add("平均RRC有效用户数>=20");
            lstbox_MU.Items.Add("上行PUSCH PRB利用率>=50");
            lstbox_MU.Items.Add("上行流量(G)>=0.3");

            lstbox_MD.Items.Clear();
            lstbox_MD.Items.Add("平均RRC有效用户数>=20");
            lstbox_MD.Items.Add("下行PDSCH PRB利用率>=50或下行PDCCH CCE利用率>=50");
            lstbox_MD.Items.Add("下行流量(G)>=3.5");

            //小包
            this.txtERAB_S.Text = "300";
            this.cmbERAB_S.SelectedIndex = 0;
            lstbox_SU.Items.Clear();
            lstbox_SU.Items.Add("平均RRC有效用户数>=50");
            lstbox_SU.Items.Add("上行PUSCH PRB利用率>=50");
            lstbox_SU.Items.Add("上行流量(G)>=0.3");

            lstbox_SD.Items.Clear();
            lstbox_SD.Items.Add("平均RRC有效用户数>=50");
            lstbox_SD.Items.Add("下行PDSCH PRB利用率>=40或下行PDCCH CCE利用率>=50");
            lstbox_SD.Items.Add("下行流量(G)>=2.2");

            //参数对象赋值
            Param.lstBd = this.lstbox_BD.Items;
            Param.lstBu = this.lstbox_BU.Items;
            Param.lstMd = this.lstbox_MD.Items;
            Param.lstMu = this.lstbox_MU.Items;
            Param.lstSd = this.lstbox_SD.Items;
            Param.lstSu = this.lstbox_SU.Items;

            Param.strERAB_B = this.txtERAB_B.Text.Trim();
            Param.strcmbERAB_B = (string)this.cmbERAB_B.Items[this.cmbERAB_B.SelectedIndex];

            Param.strERAB_MS = this.txtERAB_MS.Text.Trim();
            Param.strERAB_ME = this.txtERAB_ME.Text.Trim();
            Param.strcmbERAB_MS = (string)this.cmbERAB_MS.Items[this.cmbERAB_MS.SelectedIndex];
            Param.strcmbERAB_ME = (string) this.cmbERAB_ME.Items[this.cmbERAB_ME.SelectedIndex];

            Param.strERAB_S = this.txtERAB_S.Text.Trim();
            Param.strcmbERAB_S = (string) this.cmbERAB_S.Items[this.cmbERAB_S.SelectedIndex];
        }

        private void radGroupSHY_CheckedChanged(object sender, EventArgs e)
        {
            
            //大包
            this.txtERAB_B.Text = "1000";
            this.cmbERAB_B.SelectedIndex = 0;
            lstbox_BU.Items.Clear();
            lstbox_BU.Items.Add("平均RRC有效用户数>=10");
            lstbox_BU.Items.Add("上行PUSCH PRB利用率>=50或下行PDSCH PRB利用率>=50");
            lstbox_BU.Items.Add("上行流量(G)>=0.3或下行流量(G)>=5");

            lstbox_BD.Items.Clear();
            lstbox_BD.Items.Add("平均RRC有效用户数>=10");
            lstbox_BD.Items.Add("下行PDCCH CCE利用率>=50");

            //中包
            this.txtERAB_MS.Text = "300";
            this.txtERAB_ME.Text = "1000";
            this.cmbERAB_MS.SelectedIndex = 0;
            this.cmbERAB_ME.SelectedIndex = 0;
            lstbox_MU.Items.Clear();
            lstbox_MU.Items.Add("平均RRC有效用户数>=20");
            lstbox_MU.Items.Add("上行PUSCH PRB利用率>=50或下行PDSCH PRB利用率>=50");
            lstbox_MU.Items.Add("上行流量(G)>=0.3或下行流量(G)>=3.5");

            lstbox_MD.Items.Clear();
            lstbox_MD.Items.Add("平均RRC有效用户数>=20");
            lstbox_MD.Items.Add("下行PDCCH CCE利用率>=50");

            //小包
            this.txtERAB_S.Text = "300";
            this.cmbERAB_S.SelectedIndex = 0;
            lstbox_SU.Items.Clear();
            lstbox_SU.Items.Add("平均RRC有效用户数>=50");
            lstbox_SU.Items.Add("上行PUSCH PRB利用率>=50或下行PDSCH PRB利用率>=40");
            lstbox_SU.Items.Add("上行流量(G)>=0.3或下行流量(G)>=2.2");

            lstbox_SD.Items.Clear();
            lstbox_SD.Items.Add("平均RRC有效用户数>=50");
            lstbox_SD.Items.Add("下行PDCCH CCE利用率>=50");

            //参数对象赋值
            Param.lstBd = this.lstbox_BD.Items;
            Param.lstBu = this.lstbox_BU.Items;
            Param.lstMd = this.lstbox_MD.Items;
            Param.lstMu = this.lstbox_MU.Items;
            Param.lstSd = this.lstbox_SD.Items;
            Param.lstSu = this.lstbox_SU.Items;

            Param.strERAB_B = this.txtERAB_B.Text.Trim();
            Param.strcmbERAB_B = (string)this.cmbERAB_B.Items[this.cmbERAB_B.SelectedIndex];

            Param.strERAB_MS = this.txtERAB_MS.Text.Trim();
            Param.strERAB_ME = this.txtERAB_ME.Text.Trim();
            Param.strcmbERAB_MS = (string)this.cmbERAB_MS.Items[this.cmbERAB_MS.SelectedIndex];
            Param.strcmbERAB_ME = (string)this.cmbERAB_ME.Items[this.cmbERAB_ME.SelectedIndex];

            Param.strERAB_S = this.txtERAB_S.Text.Trim();
            Param.strcmbERAB_S = (string)this.cmbERAB_S.Items[this.cmbERAB_S.SelectedIndex];
            
        }

        private void radDTMobile_CheckedChanged(object sender, EventArgs e)
        {
            //大包
            this.txtERAB_B.Text = "1000";
            this.cmbERAB_B.SelectedIndex = 0;
            lstbox_BU.Items.Clear();
            lstbox_BU.Items.Add("平均RRC有效用户数>=10");
            lstbox_BU.Items.Add("上行PUSCH PRB利用率>=20");
            lstbox_BU.Items.Add("上行流量(G)>=0.3");

            lstbox_BD.Items.Clear();
            lstbox_BD.Items.Add("平均RRC有效用户数>=10");
            lstbox_BD.Items.Add("下行PDSCH PRB利用率>=50或下行PDCCH CCE利用率>=25");
            lstbox_BD.Items.Add("下行流量(G)>=4");

            //中包
            this.txtERAB_MS.Text = "300";
            this.txtERAB_ME.Text = "1000";
            this.cmbERAB_MS.SelectedIndex = 0;
            this.cmbERAB_ME.SelectedIndex = 0;
            lstbox_MU.Items.Clear();
            lstbox_MU.Items.Add("平均RRC有效用户数>=20");
            lstbox_MU.Items.Add("上行PUSCH PRB利用率>=20");
            lstbox_MU.Items.Add("上行流量(G)>=0.35");

            lstbox_MD.Items.Clear();
            lstbox_MD.Items.Add("平均RRC有效用户数>=20");
            lstbox_MD.Items.Add("下行PDSCH PRB利用率>=30或下行PDCCH CCE利用率>=20");
            lstbox_MD.Items.Add("下行流量(G)>=3.4");

            //小包
            this.txtERAB_S.Text = "300";
            this.cmbERAB_S.SelectedIndex = 0;
            lstbox_SU.Items.Clear();
            lstbox_SU.Items.Add("平均RRC有效用户数>=20");
            lstbox_SU.Items.Add("上行PUSCH PRB利用率>=20");
            lstbox_SU.Items.Add("上行流量(G)>=0.35");

            lstbox_SD.Items.Clear();
            lstbox_SD.Items.Add("平均RRC有效用户数>=20");
            lstbox_SD.Items.Add("下行PDSCH PRB利用率>=25或下行PDCCH CCE利用率>=20");
            lstbox_SD.Items.Add("下行流量(G)>=2.2");

            //参数对象赋值
            Param.lstBd = this.lstbox_BD.Items;
            Param.lstBu = this.lstbox_BU.Items;
            Param.lstMd = this.lstbox_MD.Items;
            Param.lstMu = this.lstbox_MU.Items;
            Param.lstSd = this.lstbox_SD.Items;
            Param.lstSu = this.lstbox_SU.Items;

            Param.strERAB_B = this.txtERAB_B.Text.Trim();
            Param.strcmbERAB_B = (string)this.cmbERAB_B.Items[this.cmbERAB_B.SelectedIndex];

            Param.strERAB_MS = this.txtERAB_MS.Text.Trim();
            Param.strERAB_ME = this.txtERAB_ME.Text.Trim();
            Param.strcmbERAB_MS = (string)this.cmbERAB_MS.Items[this.cmbERAB_MS.SelectedIndex];
            Param.strcmbERAB_ME = (string)this.cmbERAB_ME.Items[this.cmbERAB_ME.SelectedIndex];

            Param.strERAB_S = this.txtERAB_S.Text.Trim();
            Param.strcmbERAB_S = (string)this.cmbERAB_S.Items[this.cmbERAB_S.SelectedIndex];
        }



        private void radCrtDB_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radCrtDB.Checked)
            {
                this.btnSelFiles_before.Enabled = true;
                this.btnSelFiles_after.Enabled = true;
            }
            else
            {
                this.btnSelFiles_before.Enabled = false;
                this.btnSelFiles_after.Enabled = false;
            }
        }

        /// <summary>
        /// 节假日单选按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radHoliday_CheckedChanged(object sender, EventArgs e)
        {

            if (this.radHoliday.Checked)
            {
                this.labBefore.Text = "节前M*24：";
                this.labAfter.Text = "节日M*24：";
                this.radBeforeN.Text = "节前";
                this.radAfterN.Text = "节日";
                this.labCurNet.Visible = true;
                this.txtPath_CurNet.Visible = true;
                this.btnSelCurNet.Visible = true;
                this.radCurNet.Enabled = true;

            }
            else
            {
                this.labBefore.Text = "前N天指标：";
                this.labAfter.Text = "后N天指标：";
                this.radBeforeN.Text = "前N天";
                this.radAfterN.Text = "后N天";
                this.labCurNet.Visible = false;
                this.txtPath_CurNet.Visible = false;
                this.btnSelCurNet.Visible = false;
                this.radCurNet.Enabled = false;
            }
        }
        //************************************根据模板初始化********************************************//
        
    }
}
