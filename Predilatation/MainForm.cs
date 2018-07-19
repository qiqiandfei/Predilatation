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
using System.Drawing.Text;
using System.Windows.Forms.VisualStyles;

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

        public static MainParam Param = new MainParam();

        //public static Form1 GetMain()
        //{
        //    //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体
        //    if (Main == null || Main.IsDisposed)
        //    {
        //        Main = new Form1();
        //    }
        //    return Main;
        //} 

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
            radCrtDB.Checked = true;
            radFlow.Checked = true;
            progressBar.Visible = false;
        }

        //分析文件夹
        private static string strSelPath = "";
        //需要分析的文件
        private static string[] files = null;

        //当前状态
        public static string strCurTip = "";
        public static int nProValue = 0;
        private delegate void SetTipHandler();

        



        /// <summary>
        /// 选择分析文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selfiles_Click(object sender, EventArgs e)
        {
            if (this.folderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.txtPath.Text = this.folderBrowser.SelectedPath;
                strSelPath = this.txtPath.Text;
            }
        }

        /// <summary>
        /// 开始分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, EventArgs e)
        {
            string strErrMsg = Check();
            if (string.IsNullOrEmpty(strErrMsg))
            {
                try
                {
                    this.progressBar.Visible = true;
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
            SqlServerHelper sqlserver = new SqlServerHelper();
            if (this.radCrtDB.Checked)
            {
                files = Directory.GetFiles(this.txtPath.Text);
                //建库，建表
                strCurTip = "正在创建数据库...";
                nProValue = 100;
                ((TipReFresher)trf).CurTip();
                sqlserver.CreatDataBase();
                sqlserver.CreateTable();
                //添加主键
                sqlserver.AddPrimary("Tempdata","PK_Tempdata","cellname,time");
                nProValue = 100;
                ((TipReFresher)trf).CurTip();
                //文件生成表格并写入数据库
                nProValue = 0;
                strCurTip = "正在生成数据表格...";
                ((TipReFresher)trf).CurTip();
                sqlserver.GetTablefromFile(files, trf);
                
                //清洗无效数据
                nProValue = 0;
                strCurTip = "正在清洗无效数据...";
                ((TipReFresher)trf).CurTip();
                sqlserver.DataClean(files,trf);
            }

            if (this.radFlow.Checked)
            {
                strType = "FlowBusy";

                nProValue = 50;
                
                strCurTip = "正在计算流量自忙时...";
                ((TipReFresher)trf).CurTip();
                sqlserver.CalculateBusy_Flow();
                nProValue = 100;
                ((TipReFresher)trf).CurTip();
            }
            if (this.radUtilizaerate.Checked)
            {
                strType = "Utilizaerate";

                nProValue = 50;
                strCurTip = "正在计算利用率自忙时...";
                ((TipReFresher)trf).CurTip();
                sqlserver.CalculateBusy_Utilizaerate();
                nProValue = 100;
                ((TipReFresher)trf).CurTip();

            }

            if (this.radGroupSHY.Checked || this.radGroupFHY.Checked || this.radDTMobile.Checked)
            {
                Calculate Cal = new Calculate(Param);
                nProValue = 50;
                strCurTip = "正在根据标准计算平均扩容条件...";
                ((TipReFresher)trf).CurTip();

                Cal.CrtAvgData(strType, strStdTyp, trf);


                nProValue = 50;
                strCurTip = "正在根据标准计算每天扩容条件...";
                ((TipReFresher)trf).CurTip();
                Cal.CrtDataDate(strType, strStdTyp, trf);
                nProValue = 100;
                ((TipReFresher)trf).CurTip();

                double elapsedTimeInSeconds = DateTime.Now.Subtract(TimeStart).TotalSeconds;
                MessageBox.Show("总共耗时：" + elapsedTimeInSeconds.ToString());

                strCurTip = "分析结束！";
                ((TipReFresher)trf).CurTip();
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
                    if (string.IsNullOrEmpty(this.txtPath.Text))
                    {
                        strErrMsg = "请选择路径！";
                        return strErrMsg;
                    }

                    string[] files = Directory.GetFiles(this.txtPath.Text);
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
            }
            catch (Exception e)
            {
                strErrMsg = e.Message;
                return strErrMsg;
            }
            return strErrMsg;
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
            strStdTyp = "Grp2018Fir";
            //大包
            this.txtERAB_B.Text = "1000";
            this.cmbERAB_B.SelectedIndex = 0;
            lstbox_BU.Items.Clear();
            lstbox_BU.Items.Add("平均RRC有效用户数>=10");
            lstbox_BU.Items.Add("上行PUSCH PRB利用率>=0.5");
            lstbox_BU.Items.Add("上行流量(G)>=0.3");

            lstbox_BD.Items.Clear();
            lstbox_BD.Items.Add("平均RRC有效用户数>=10");
            lstbox_BD.Items.Add("下行PDSCH PRB利用率>=0.7或下行PDCCH CCE利用率>=0.5");
            lstbox_BD.Items.Add("下行流量(G)>=5");

            //中包
            this.txtERAB_MS.Text = "300";
            this.txtERAB_ME.Text = "1000";
            this.cmbERAB_MS.SelectedIndex = 0;
            this.cmbERAB_ME.SelectedIndex = 0;
            lstbox_MU.Items.Clear();
            lstbox_MU.Items.Add("平均RRC有效用户数>=20");
            lstbox_MU.Items.Add("上行PUSCH PRB利用率>=0.5");
            lstbox_MU.Items.Add("上行流量(G)>=0.3");

            lstbox_MD.Items.Clear();
            lstbox_MD.Items.Add("平均RRC有效用户数>=20");
            lstbox_MD.Items.Add("下行PDSCH PRB利用率>=0.5或下行PDCCH CCE利用率>=0.5");
            lstbox_MD.Items.Add("下行流量(G)>=3.5");

            //小包
            this.txtERAB_S.Text = "300";
            this.cmbERAB_S.SelectedIndex = 0;
            lstbox_SU.Items.Clear();
            lstbox_SU.Items.Add("平均RRC有效用户数>=50");
            lstbox_SU.Items.Add("上行PUSCH PRB利用率>=0.5");
            lstbox_SU.Items.Add("上行流量(G)>=0.3");

            lstbox_SD.Items.Clear();
            lstbox_SD.Items.Add("平均RRC有效用户数>=50");
            lstbox_SD.Items.Add("下行PDSCH PRB利用率>=0.4或下行PDCCH CCE利用率>=0.5");
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
            strStdTyp = "Grp2018Sec";
            //大包
            this.txtERAB_B.Text = "1000";
            this.cmbERAB_B.SelectedIndex = 0;
            lstbox_BU.Items.Clear();
            lstbox_BU.Items.Add("平均RRC有效用户数>=10");
            lstbox_BU.Items.Add("上行PUSCH PRB利用率>=0.5或下行PDSCH PRB利用率>=0.5");
            lstbox_BU.Items.Add("上行流量(G)>=0.3或下行流量(G)>=5");

            lstbox_BD.Items.Clear();
            lstbox_BD.Items.Add("平均RRC有效用户数>=10");
            lstbox_BD.Items.Add("下行PDCCH CCE利用率>=0.5");

            //中包
            this.txtERAB_MS.Text = "300";
            this.txtERAB_ME.Text = "1000";
            this.cmbERAB_MS.SelectedIndex = 0;
            this.cmbERAB_ME.SelectedIndex = 0;
            lstbox_MU.Items.Clear();
            lstbox_MU.Items.Add("平均RRC有效用户数>=20");
            lstbox_MU.Items.Add("上行PUSCH PRB利用率>=0.5或下行PDSCH PRB利用率>=0.5");
            lstbox_MU.Items.Add("上行流量(G)>=0.3或下行流量(G)>=3.5");

            lstbox_MD.Items.Clear();
            lstbox_MD.Items.Add("平均RRC有效用户数>=20");
            lstbox_MD.Items.Add("下行PDCCH CCE利用率>=0.5");

            //小包
            this.txtERAB_S.Text = "300";
            this.cmbERAB_S.SelectedIndex = 0;
            lstbox_SU.Items.Clear();
            lstbox_SU.Items.Add("平均RRC有效用户数>=50");
            lstbox_SU.Items.Add("上行PUSCH PRB利用率>=0.5或下行PDSCH PRB利用率>=0.4");
            lstbox_SU.Items.Add("上行流量(G)>=0.3或下行流量(G)>=2.2");

            lstbox_SD.Items.Clear();
            lstbox_SD.Items.Add("平均RRC有效用户数>=50");
            lstbox_SD.Items.Add("下行PDCCH CCE利用率>=0.5");

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
            strStdTyp = "DTMobile";
            //大包
            this.txtERAB_B.Text = "1000";
            this.cmbERAB_B.SelectedIndex = 0;
            lstbox_BU.Items.Clear();
            lstbox_BU.Items.Add("平均RRC有效用户数>=10");
            lstbox_BU.Items.Add("上行PUSCH PRB利用率>=0.2");
            lstbox_BU.Items.Add("上行流量(G)>=0.3");

            lstbox_BD.Items.Clear();
            lstbox_BD.Items.Add("平均RRC有效用户数>=10");
            lstbox_BD.Items.Add("下行PDSCH PRB利用率>=0.5或下行PDCCH CCE利用率>=0.25");
            lstbox_BD.Items.Add("下行流量(G)>=4");

            //中包
            this.txtERAB_MS.Text = "300";
            this.txtERAB_ME.Text = "1000";
            this.cmbERAB_MS.SelectedIndex = 0;
            this.cmbERAB_ME.SelectedIndex = 0;
            lstbox_MU.Items.Clear();
            lstbox_MU.Items.Add("平均RRC有效用户数>=20");
            lstbox_MU.Items.Add("上行PUSCH PRB利用率>=0.2");
            lstbox_MU.Items.Add("上行流量(G)>=0.35");

            lstbox_MD.Items.Clear();
            lstbox_MD.Items.Add("平均RRC有效用户数>=20");
            lstbox_MD.Items.Add("下行PDSCH PRB利用率>=0.3或下行PDCCH CCE利用率>=0.2");
            lstbox_MD.Items.Add("下行流量(G)>=3.4");

            //小包
            this.txtERAB_S.Text = "300";
            this.cmbERAB_S.SelectedIndex = 0;
            lstbox_SU.Items.Clear();
            lstbox_SU.Items.Add("平均RRC有效用户数>=20");
            lstbox_SU.Items.Add("上行PUSCH PRB利用率>=0.2");
            lstbox_SU.Items.Add("上行流量(G)>=0.35");

            lstbox_SD.Items.Clear();
            lstbox_SD.Items.Add("平均RRC有效用户数>=20");
            lstbox_SD.Items.Add("下行PDSCH PRB利用率>=0.25或下行PDCCH CCE利用率>=0.2");
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
                this.btnSelfiles.Enabled = true;
            }
            else
            {
                this.btnSelfiles.Enabled = false;
            }
        }
        //************************************根据模板初始化********************************************//
        
    }
}
