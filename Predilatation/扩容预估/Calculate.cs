using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Predilatation
{
    public class Calculate:SqlServerHelper
    {
        public static MainParam Param = null;

        public Calculate(MainParam mainParam)
        {
            if (Param == null)
            {
                Param = new MainParam();
                Param = mainParam;
            }
        }
        /// <summary>
        /// 基于标准计算平均的扩容条件
        /// </summary>
        /// <param name="strTyp"></param>
        /// <param name="strStdTyp"></param>
        public void CrtAvgData(string strTyp, string strStdTyp,object trf,string TableName,string AorB)
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                SqlCommand oComm = this.oConn.CreateCommand();
                oComm.CommandText = "select cellname, " +
                             "(sum(小区用户面上行字节数)+sum(小区用户面下行字节数))/sum(E_RAB建立成功数) as 'E-RAB(KB)', " +
                             "AVG(有效RRC连接平均数) as '平均RRC有效用户数', " +
                             "(SUM(上行占用的PRB个数)/SUM(RRU_PrbUl_TotalNum)) * 100 as '上行PUSCH PRB利用率', " +
                             "AVG(小区用户面上行字节数)/(1024*1024) as '上行流量(G)', " +
                             "(SUM(下行占用的PRB个数)/SUM(RRU_PrbDl_TotalNum)) * 100 as '下行PDSCH PRB利用率', " +
                             "(SUM(CCE占用量)/SUM(CCE可使用量)) * 100 as '下行PDCCH CCE利用率', " +
                             "AVG(小区用户面下行字节数)/(1024*1024) as '下行流量(G)' " +
                             "from " + TableName + " where " + strTyp + " <> '' group by cellname having(sum(E_RAB建立成功数)> 0)";
                SqlDataReader dr = oComm.ExecuteReader();
                DataTable dt = CrtAvgTable();
                while (dr.Read())
                {
                    DataRow row = dt.NewRow();
                    row[0] = Convert.ToString(dr[0]);
                    row[1] = CheckCellType(Convert.ToDouble(dr["E-RAB(KB)"]));
                    row[2] = Convert.ToDouble(dr["E-RAB(KB)"]);
                    row[3] = Convert.ToDouble(dr["平均RRC有效用户数"]);
                    row[4] = Convert.ToDouble(dr["上行PUSCH PRB利用率"]);
                    row[5] = Convert.ToDouble(dr["上行流量(G)"]);
                    row[6] = Convert.ToDouble(dr["下行PDSCH PRB利用率"]);
                    row[7] = Convert.ToDouble(dr["下行PDCCH CCE利用率"]);
                    row[8] = Convert.ToDouble(dr["下行流量(G)"]);
                    string[] strsStandard = CheckStandard(dr);
                    row[9] = strsStandard[0];
                    row[10] = strsStandard[1];
                    dt.Rows.Add(row);
                }
                dr.Close();
                this.oConn.Close();
                string strTableName = AorB + "_Avgdata_" + strStdTyp + "_" + strTyp;
                //创建平均表
                CreateAvgTable(strTableName);
                //写入平均表数据
                MainForm.strCurTip = "正在写入平均表数据...";
                ((TipReFresher)trf).CurTip();
                BulkToDB(dt, strTableName);
                MainForm.nProValue = 85;
                ((TipReFresher)trf).CurTip();
                //添加主键
                AddPrimary(strTableName, "PK_" + strTableName, "cellname");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 基于标准计算每天的扩容条件
        /// </summary>
        /// <param name="strTyp"></param>
        /// <param name="strStdTyp"></param>
        public void CrtDataDate(string strTyp, string strStdTyp,object trf,string TableName,string AorB)
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                SqlCommand oComm = this.oConn.CreateCommand();
                oComm.CommandText = "select cellname,time, " +
                             "平均E_RAB流量 as 'E-RAB(KB)', " +
                             "有效RRC连接平均数 as '平均RRC有效用户数', " +
                             "上行PUSCH_PRB利用率 as '上行PUSCH PRB利用率', " +
                             "小区用户面上行字节数/(1024*1024) as '上行流量(G)', " +
                             "下行PDSCH_PRB利用率 as '下行PDSCH PRB利用率', " +
                             "下行PDCCH_CCE利用率 as '下行PDCCH CCE利用率', " +
                             "小区用户面下行字节数/(1024*1024) as '下行流量(G)' " +
                             "from " + TableName + " where " + strTyp + " <> ''";
                SqlDataReader dr = oComm.ExecuteReader();
                DataTable dt = CrtDateTable();
                while (dr.Read())
                {
                    DataRow row = dt.NewRow();
                    row[0] = Convert.ToString(dr[0]);
                    row[1] = Convert.ToString(dr[1]);
                    row[2] = CheckCellType(Convert.ToDouble(dr["E-RAB(KB)"]));
                    row[3] = Convert.ToDouble(dr["E-RAB(KB)"]);
                    row[4] = Convert.ToDouble(dr["平均RRC有效用户数"]);
                    row[5] = Convert.ToDouble(dr["上行PUSCH PRB利用率"]);
                    row[6] = Convert.ToDouble(dr["上行流量(G)"]);
                    row[7] = Convert.ToDouble(dr["下行PDSCH PRB利用率"]);
                    row[8] = Convert.ToDouble(dr["下行PDCCH CCE利用率"]);
                    row[9] = Convert.ToDouble(dr["下行流量(G)"]);
                    string[] strsStandard = CheckStandard(dr);
                    row[10] = strsStandard[0];
                    row[11] = strsStandard[1];
                    dt.Rows.Add(row);
                }
                dr.Close();
                this.oConn.Close();
                string strTableName = AorB + "_Date_" + strStdTyp + "_" + strTyp;
                //创建每天表
                CreateDateTable(strTableName);
                //写入每天表数据
                MainForm.strCurTip = "正在写入每天表数据...";
                ((TipReFresher)trf).CurTip();
                BulkToDB(dt, strTableName);
                MainForm.nProValue = 85;
                ((TipReFresher)trf).CurTip();
                //添加主键
                AddPrimary(strTableName, "PK_" + strTableName,"cellname,time");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="conn"></param>
        private void CreateAvgTable(string strTableName)
        {
            this.oConnOpen();
            this.oConn.ChangeDatabase("Predilatation");
            SqlCommand oComm = this.oConn.CreateCommand();
            try
            {
                oComm.CommandText = "if  exists(select * from sysobjects where  type = 'U' and name = '" + strTableName + "') drop table " + strTableName +
                                    " create table " + strTableName +
                                    " (cellname varchar(255) not null," +
                                    "celltype varchar(20)," +
                                    "ERAB numeric(18,4)," +
                                    "平均RRC有效用户数 numeric(18,4)," +
                                    "上行PUSCH_PRB利用率 numeric(18,4)," +
                                    "上行流量G numeric(18,4)," +
                                    "下行PDSCH_PRB利用率 numeric(18,4)," +
                                    "下行PDCCH_CCE利用率 numeric(18,4)," +
                                    "下行流量G numeric(18,4)," +
                                    "上行扩容标准 varchar(10)," +
                                    "下行扩容标准 varchar(10))";
                oComm.ExecuteNonQuery();

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                oComm.Dispose();
                this.oConn.Close();
            }
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="conn"></param>
        private void CreateDateTable(string strTableName)
        {
            this.oConnOpen();
            this.oConn.ChangeDatabase("Predilatation");
            SqlCommand oComm = this.oConn.CreateCommand();
            try
            {
                oComm.CommandText = "if  exists(select * from sysobjects where  type = 'U' and name = '" + strTableName + "') drop table " + strTableName +
                                    " create table " + strTableName +
                                    " (cellname varchar(255) not null," +
                                    "time varchar(255) not null," +
                                    "celltype varchar(20) not null," +
                                    "ERAB numeric(18,4)," +
                                    "平均RRC有效用户数 numeric(18,4)," +
                                    "上行PUSCH_PRB利用率 numeric(18,4)," +
                                    "上行流量G numeric(18,4)," +
                                    "下行PDSCH_PRB利用率 numeric(18,4)," +
                                    "下行PDCCH_CCE利用率 numeric(18,4)," +
                                    "下行流量G numeric(18,4)," +
                                    "上行扩容标准 varchar(10)," +
                                    "下行扩容标准 varchar(10))";
                oComm.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw ;
            }
            finally
            {
                oComm.Dispose();
                this.oConn.Close();
            }
        }

        /// <summary>
        /// 判断流量准则
        /// </summary>
        /// <param name="dE_RAB"></param>
        /// <param name="dRRCNum"></param>
        /// <param name="dPUSCH_UL"></param>
        /// <param name="dFlow_UL"></param>
        /// <param name="dPUSH_DL"></param>
        /// <param name="dPDCCH_DL"></param>
        /// <param name="Flow_DL"></param>
        /// <returns></returns>
        private  string[] CheckStandard(SqlDataReader dr)
        {
            string[] strStandard = new string[2];
            
            //小区类型
            string strCellTyp = CheckCellType(Convert.ToDouble(dr["E-RAB(KB)"]));
            if (strCellTyp == "大包小区")
            {
                //上行标准
                if (CheckStandard(Param.lstBu, dr))
                    strStandard[0] = "满足";
                else
                    strStandard[0] = "不满足";

                //下行标准
                if (CheckStandard(Param.lstBd, dr))
                    strStandard[1] = "满足";
                else
                    strStandard[1] = "不满足";
            }
            if (strCellTyp == "中包小区")
            {
                //上行标准
                if (CheckStandard(Param.lstMu, dr))
                    strStandard[0] = "满足";
                else
                    strStandard[0] = "不满足";

                //下行标准
                if (CheckStandard(Param.lstMd, dr))
                    strStandard[1] = "满足";
                else
                    strStandard[1] = "不满足";
            }
            if (strCellTyp == "小包小区")
            {
                //上行标准
                if (CheckStandard(Param.lstSu, dr))
                    strStandard[0] = "满足";
                else
                    strStandard[0] = "不满足";

                //下行标准
                if (CheckStandard(Param.lstSd, dr))
                    strStandard[1] = "满足";
                else
                    strStandard[1] = "不满足";
            }

            return strStandard;
        }

        /// <summary>
        /// 判断标准
        /// </summary>
        /// <param name="Items"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        private bool CheckStandard(ListBox.ObjectCollection Items, SqlDataReader dr)
        {
            DataTable dt = new DataTable();
            string strExp = "";
            foreach (var item in Items)
            {
                //获取每个标准存到临时变量
                string strTemp = item.ToString();

                //有或的替换为逻辑运算符
                if (strTemp.Contains("或"))
                {
                    strTemp = strTemp.Replace("或", " or ");
                    strTemp = "(" + strTemp + ")";
                }
                
                //替换指标值
                if(strTemp.Contains("平均RRC有效用户数"))
                    strTemp = strTemp.Replace("平均RRC有效用户数", Convert.ToString(dr["平均RRC有效用户数"]));

                if (strTemp.Contains("上行PUSCH PRB利用率"))
                    strTemp = strTemp.Replace("上行PUSCH PRB利用率", Convert.ToString(dr["上行PUSCH PRB利用率"]));

                if (strTemp.Contains("上行流量(G)"))
                    strTemp = strTemp.Replace("上行流量(G)", Convert.ToString(dr["上行流量(G)"]));

                if (strTemp.Contains("下行PDSCH PRB利用率"))
                    strTemp = strTemp.Replace("下行PDSCH PRB利用率", Convert.ToString(dr["下行PDSCH PRB利用率"]));

                if (strTemp.Contains("下行PDCCH CCE利用率"))
                    strTemp = strTemp.Replace("下行PDCCH CCE利用率", Convert.ToString(dr["下行PDCCH CCE利用率"]));

                if (strTemp.Contains("下行流量(G)"))
                    strTemp = strTemp.Replace("下行流量(G)", Convert.ToString(dr["下行流量(G)"]));

                strExp += strTemp + " and ";
            }

            strExp = strExp.Substring(0, strExp.Length - 5);

            return (bool)dt.Compute(strExp, "");
        }

        /// <summary>
        /// 判断小区类型
        /// </summary>
        /// <param name="dERAB"></param>
        /// <returns></returns>
        public static  string CheckCellType(double dERAB)
        {
            try
            {
                DataTable dt = new DataTable();
                string strCellTyp = "";
                string strExp_B = dERAB.ToString() + MainForm.Param.strcmbERAB_B + MainForm.Param.strERAB_B;

                string strExp_M = MainForm.Param.strERAB_MS + MainForm.Param.strcmbERAB_MS +
                                  dERAB.ToString() + " and " + dERAB.ToString() + MainForm.Param.strcmbERAB_ME + MainForm.Param.strERAB_ME;

                string strExp_S = dERAB.ToString() + MainForm.Param.strcmbERAB_S + MainForm.Param.strERAB_S;

                if ((bool)dt.Compute(strExp_B, ""))
                {
                    strCellTyp = "大包小区";
                }
                if ((bool)dt.Compute(strExp_M, ""))
                {
                    strCellTyp = "中包小区";
                }
                if ((bool)dt.Compute(strExp_S, ""))
                {
                    strCellTyp = "小包小区";
                }
                return strCellTyp;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 创建平均扩容标准表
        /// </summary>
        /// <returns></returns>
        private DataTable CrtAvgTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]{  
            new DataColumn("cellname",typeof(string)), 
            new DataColumn("celltype",typeof(string)), 
            new DataColumn("ERAB",typeof(double)),
            new DataColumn("平均RRC有效用户数",typeof(double)),
            new DataColumn("上行PUSCH_PRB利用率",typeof(double)),
            new DataColumn("上行流量G",typeof(double)),
            new DataColumn("下行PDSCH_PRB利用率",typeof(double)),
            new DataColumn("下行PDCCH_CCE利用率",typeof(double)),
            new DataColumn("下行流量G",typeof(double)),
            new DataColumn("上行扩容标准",typeof(string)),
            new DataColumn("下行扩容标准",typeof(string))});
            return dt;
        }

        /// <summary>
        /// 创建每天扩容标准表
        /// </summary>
        /// <returns></returns>
        private DataTable CrtDateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]{  
            new DataColumn("cellname",typeof(string)),  
            new DataColumn("time",typeof(string)),  
            new DataColumn("celltype",typeof(string)), 
            new DataColumn("ERAB",typeof(double)),
            new DataColumn("平均RRC有效用户数",typeof(double)),
            new DataColumn("上行PUSCH_PRB利用率",typeof(double)),
            new DataColumn("上行流量G",typeof(double)),
            new DataColumn("下行PDSCH_PRB利用率",typeof(double)),
            new DataColumn("下行PDCCH_CCE利用率",typeof(double)),
            new DataColumn("下行流量G",typeof(double)),
            new DataColumn("上行扩容标准",typeof(string)),
            new DataColumn("下行扩容标准",typeof(string))});
            return dt;
        }
    }
}
