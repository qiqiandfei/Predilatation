using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aspose.Cells;


namespace Predilatation
{
    public class ResObj
    {
        public string CellName { set; get; }
        public int Priority { set; get; }
        public string AvgIsExp { set; get; }
        public string GrowthIsExp { set; get; }
        public string GrowthCellTyp { set; get; }
        public double GrowthRate { set; get; }
        public double GrowthMaxRate { set; get; }
        public int ExpFreqTimes { set; get; }
        public string CellTyp_AVG { set; get; }
        public double ERAB_AVG { set; get; }
        public double RRC_AVG { set; get; }
        public double UL_PUSCH_PRB_AVG { set; get; }
        public double UL_Flow_AVG { set; get; }
        public double DL_PDSCH_PRB_AVG { set; get; }
        public double DL_PDCCH_CCE_AVG { set; get; }
        public double DL_Flow_AVG { set; get; }
        public string UL_Std_AVG { set; get; }
        public string DL_Std_AVG { set; get; }
        public string CellTyp_EST { set; get; }
        public double ERAB_EST { set; get; }
        public double RRC_EST { set; get; }
        public double UL_PUSCH_PRB_EST { set; get; }
        public double UL_Flow_EST { set; get; }
        public double DL_PDSCH_PRB_EST { set; get; }
        public double DL_PDCCH_CCE_EST { set; get; }
        public double DL_Flow_EST { set; get; }
        public string UL_Std_EST { set; get; }
        public string DL_Std_EST { set; get; }
        public string CellSense { set; get; }
        public string CellSubSense { set; get; }
        public string Remarks { set; get; }
        //给定增长率
        public double RateC { set; get; }
        public double XRateFlow { set; get; }

        public ResObj()
        {
        }

        private string strConn;

        public SqlConnection Open()
        {
            this.strConn = "data source=" + SqlServerHelper.strserver + ";Initial Catalog=Predilatation;user id=" + SqlServerHelper.struser + ";pwd=" + SqlServerHelper.strpwd + ";Pooling=true;Max Pool Size=40000;Min Pool Size=0";
            SqlConnection oConn = new SqlConnection(strConn);
            return oConn;
        }

        /// <summary>
        /// 写入结果
        /// </summary>
        /// <param name="res"></param>
        /// <param name="sqlconn"></param>
        /// <param name="strTableName"></param>
        public void Insert(ResObj res, SqlConnection sqlconn, string strTableName)
        {
            SqlCommand oComm = sqlconn.CreateCommand();
            oComm.CommandTimeout = 0;
            try
            {
                oComm.CommandText = "Insert Into " + strTableName + " (cellname," +
                                    "扩容优先级," +
                                    "均值是否满足扩容," +
                                    "增量是否满足扩容," +
                                    "增量小区类型," +
                                    "增量小区增长率," +
                                    "增量Max利用率," +
                                    "满足扩容频次," +
                                    "小区包类型_avg," +
                                    "ERAB_avg," +
                                    "平均RRC有效用户数_avg," +
                                    "上行PUSCH_PRB利用率_avg," +
                                    "上行流量G_avg," +
                                    "下行PDSCH_PRB利用率_avg," +
                                    "下行PDCCH_CCE利用率_avg," +
                                    "下行流量G_avg," +
                                    "上行扩容标准_avg," +
                                    "下行扩容标准_avg," +
                                    "小区包类型_est," +
                                    "ERAB_est," +
                                    "平均RRC有效用户数_est," +
                                    "上行PUSCH_PRB利用率_est," +
                                    "上行流量G_est," +
                                    "下行PDSCH_PRB利用率_est," +
                                    "下行PDCCH_CCE利用率_est," +
                                    "下行流量G_est," +
                                    "上行扩容标准_est," +
                                    "下行扩容标准_est," +
                                    "小区归属场景," +
                                    "小区归属子场景," +
                                    "备注) values " +
                                    "('" +
                                    res.CellName + "'," +
                                    res.Priority + ",'" +
                                    res.AvgIsExp + "','" +
                                    res.GrowthIsExp + "','" +
                                    res.GrowthCellTyp + "'," +
                                    res.GrowthRate + "," +
                                    res.GrowthMaxRate + "," +
                                    res.ExpFreqTimes + ",'" +
                                    res.CellTyp_AVG + "'," +
                                    res.ERAB_AVG + "," +
                                    res.RRC_AVG + "," +
                                    res.UL_PUSCH_PRB_AVG + "," +
                                    res.UL_Flow_AVG + "," +
                                    res.DL_PDSCH_PRB_AVG + "," +
                                    res.DL_PDCCH_CCE_AVG + "," +
                                    res.DL_Flow_AVG + ",'" +
                                    res.UL_Std_AVG + "','" +
                                    res.DL_Std_AVG + "','" +
                                    res.CellTyp_EST + "'," +
                                    res.ERAB_EST + "," +
                                    res.RRC_EST + "," +
                                    res.UL_PUSCH_PRB_EST + "," +
                                    res.UL_Flow_EST + "," +
                                    res.DL_PDSCH_PRB_EST + "," +
                                    res.DL_PDCCH_CCE_EST + "," +
                                    res.DL_Flow_EST + ",'" +
                                    res.UL_Std_EST + "','" +
                                    res.DL_Std_EST + "','" +
                                    res.CellSense + "','" +
                                    res.CellSubSense + "','" +
                                    res.Remarks + "')";

                oComm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                oComm.Dispose();
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
        public string[] CheckStandard(SqlDataReader dr, string strCellTyp)
        {
            string[] strStandard = new string[2];

            if (strCellTyp == "大包小区")
            {
                //上行标准
                if (CheckStandard(MainForm.Param.lstBu, dr))
                    strStandard[0] = "满足";
                else
                    strStandard[0] = "不满足";

                //下行标准
                if (CheckStandard(MainForm.Param.lstBd, dr))
                    strStandard[1] = "满足";
                else
                    strStandard[1] = "不满足";
            }
            if (strCellTyp == "中包小区")
            {
                //上行标准
                if (CheckStandard(MainForm.Param.lstMu, dr))
                    strStandard[0] = "满足";
                else
                    strStandard[0] = "不满足";

                //下行标准
                if (CheckStandard(MainForm.Param.lstMd, dr))
                    strStandard[1] = "满足";
                else
                    strStandard[1] = "不满足";
            }
            if (strCellTyp == "小包小区")
            {
                //上行标准
                if (CheckStandard(MainForm.Param.lstSu, dr))
                    strStandard[0] = "满足";
                else
                    strStandard[0] = "不满足";

                //下行标准
                if (CheckStandard(MainForm.Param.lstSd, dr))
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
                if (strTemp.Contains("平均RRC有效用户数"))
                    strTemp = strTemp.Replace("平均RRC有效用户数", Convert.ToString(dr["有效RRC连接平均数"]));

                if (strTemp.Contains("上行PUSCH PRB利用率"))
                    strTemp = strTemp.Replace("上行PUSCH PRB利用率", Convert.ToString(dr["上行PUSCH_PRB利用率"]));

                if (strTemp.Contains("上行流量(G)"))
                    strTemp = strTemp.Replace("上行流量(G)", Convert.ToString(Convert.ToDouble(dr["小区用户面上行字节数"]) / (1024.0 * 1024.0)));

                if (strTemp.Contains("下行PDSCH PRB利用率"))
                    strTemp = strTemp.Replace("下行PDSCH PRB利用率", Convert.ToString(dr["下行PDSCH_PRB利用率"]));

                if (strTemp.Contains("下行PDCCH CCE利用率"))
                    strTemp = strTemp.Replace("下行PDCCH CCE利用率", Convert.ToString(dr["下行PDCCH_CCE利用率"]));

                if (strTemp.Contains("下行流量(G)"))
                    strTemp = strTemp.Replace("下行流量(G)", Convert.ToString(Convert.ToDouble(dr["小区用户面下行字节数"]) / (1024.0 * 1024.0)));

                strExp += strTemp + " and ";
            }

            strExp = strExp.Substring(0, strExp.Length - 5);

            return (bool)dt.Compute(strExp, "");
        }

        

        /// <summary>
        /// 获取场景类型
        /// </summary>
        /// <param name="strCellName"></param>
        /// <returns></returns>
        public string[] GetSense(string strCellName, DataTable dtSense)
        {
            try
            {
                string[] sense = new string[2];
                string condition = dtSense.Columns[0].ColumnName + " = '" + strCellName + "'";
                DataRow[] drs = dtSense.Select(condition);
                if (drs.Length > 0)
                {
                    sense[0] = Convert.ToString(drs[0][1]);
                    sense[1] = Convert.ToString(drs[0][2]);
                }
                else
                {
                    sense[0] = "未匹配";
                    sense[1] = "未匹配";
                }
                return sense;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        /// <summary>
        /// 计算满足的频次
        /// </summary>
        /// <param name="strStdTyp"></param>
        /// <param name="strTyp"></param>
        /// <param name="strCellname"></param>
        /// <returns></returns>
        public int CalcuateFreqTimes(string strTableName, string strCellname, SqlConnection sqlconn)
        {
            int nFreqTimes = 0;
            SqlCommand oComm = sqlconn.CreateCommand();
            oComm.CommandTimeout = 0;
            try
            {
                oComm.CommandText = "(select COUNT(cellname) from " + strTableName +
                                         " where cellname = '" + strCellname + "' and (上行扩容标准='满足' or 下行扩容标准='满足'))";
                nFreqTimes = Convert.ToInt32(oComm.ExecuteScalar());

                return nFreqTimes;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                oComm.Dispose();
            }
        }

        

        /// <summary>
        /// 判断均值是否满足
        /// </summary>
        /// <param name="strULStd"></param>
        /// <param name="strDLStd"></param>
        /// <returns></returns>
        public string CheckAvgIsExp(string strULStd, string strDLStd)
        {
            string strChkRes = "不满足";
            if (strULStd == "满足" || strDLStd == "满足")
                strChkRes = "满足";
            return strChkRes;
        }

        /// <summary>
        /// 计算优先级
        /// </summary>
        /// <param name="strAvgIsExp"></param>
        /// <param name="nFreqTimes"></param>
        /// <returns></returns>
        public int GetPriority(string strAvgIsExp, int nFreqTimes)
        {
            int nPriority = 0;
            if (strAvgIsExp == "满足")
            {
                switch (nFreqTimes)
                {
                    case 7:
                        nPriority = 1;
                        break;
                    case 6:
                        nPriority = 2;
                        break;
                    case 5:
                        nPriority = 3;
                        break;
                    case 4:
                        nPriority = 4;
                        break;
                    case 3:
                        nPriority = 5;
                        break;
                    case 2:
                        nPriority = 6;
                        break;
                    case 1:
                        nPriority = 7;
                        break;
                }
            }
            else
            {
                switch (nFreqTimes)
                {
                    case 6:
                        nPriority = 8;
                        break;
                    case 5:
                        nPriority = 9;
                        break;
                    case 4:
                        nPriority = 10;
                        break;
                    case 3:
                        nPriority = 11;
                        break;
                    case 2:
                        nPriority = 12;
                        break;
                    case 1:
                        nPriority = 13;
                        break;
                }
            }


            return nPriority;
        }

        /// <summary>
        /// 判断优先级
        /// </summary>
        /// <param name="dGrowthRate"></param>
        /// <returns></returns>
        public int GetPriority(double dGrowthRate)
        {
            int nPriority = 0;
            if (dGrowthRate >= 90)
                nPriority = 14;
            else if (dGrowthRate >= 80 && dGrowthRate < 90)
                nPriority = 15;
            else if (dGrowthRate >= 70 && dGrowthRate < 80)
                nPriority = 16;
            else if (dGrowthRate >= 60 && dGrowthRate < 70)
                nPriority = 17;
            else if (dGrowthRate >= 50 && dGrowthRate < 60)
                nPriority = 18;
            else if (dGrowthRate < 50)
                nPriority = 19;
            return nPriority;
        }

        /// <summary>
        /// 获取场景对照表
        /// </summary>
        public DataTable GetSenseTable()
        {
            string strSenseFile = System.Windows.Forms.Application.StartupPath + "\\小区场景归属信息.xlsx";
            try
            {
                if (File.Exists(strSenseFile))
                {
                    DataTable dtSense = new DataTable();
                    Workbook wb = new Workbook(strSenseFile);
                    Worksheet sheet = wb.Worksheets[0];
                    Cells cell = sheet.Cells;
                    dtSense = cell.ExportDataTable(0, 0, cell.MaxRow + 1, cell.MaxColumn + 1);
                    return dtSense;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}