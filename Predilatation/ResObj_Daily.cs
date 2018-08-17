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
    public class ResObj_Daily:AbstractResObj
    {
        

        /// <summary>
        /// 返回结果对象
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public override void GetResObj(ResObj res, string strStdTyp, string strTyp, SqlConnection sqlconn, string strTableName, DataTable dtSense)
        {
            try
            {
                if (sqlconn.State != ConnectionState.Open)
                    sqlconn.Open();
                
                //满足单日自忙时标准
                int nFreqTimes = CalcuateFreqTimes("After_Date_" + strStdTyp + "_" + strTyp, res.CellName, sqlconn);
                //判断均值是否满足扩容
                string strAvgIsExp = CheckAvgIsExp(res.UL_Std_AVG, res.DL_Std_AVG);
                if (nFreqTimes > 0)
                {
                    //计算满足规则的频次
                    res.AvgIsExp = strAvgIsExp;
                    res.Priority = GetPriority(strAvgIsExp, nFreqTimes);
                    res.ExpFreqTimes = nFreqTimes;
                    res.GrowthIsExp = "";
                    res.GrowthCellTyp = "";
                    res.GrowthRate = 0;
                    res.GrowthMaxRate = 0;
                    res.CellTyp_EST = "";
                    res.ERAB_EST = 0;
                    res.RRC_EST = 0;
                    res.UL_PUSCH_PRB_EST = 0;
                    res.UL_Flow_EST = 0;
                    res.DL_PDSCH_PRB_EST = 0;
                    res.DL_PDCCH_CCE_EST = 0;
                    res.DL_Flow_EST = 0;
                    res.UL_Std_EST = "";
                    res.DL_Std_EST = "";
                    res.CellSense = GetSense(res.CellName, dtSense)[0];
                    res.CellSubSense = GetSense(res.CellName, dtSense)[1];
                    res.Remarks = "";

                }
                else
                {
                    //计算增长率
                    string[] Rate = GetCellGrowthRate(strTyp, strStdTyp, res, sqlconn);
                    res.AvgIsExp = strAvgIsExp;
                    res.ExpFreqTimes = nFreqTimes;
                    res.GrowthIsExp = "";
                    res.GrowthCellTyp = Rate[1];
                    res.GrowthRate = Convert.ToDouble(Rate[0]);
                    res.CellSense = GetSense(res.CellName, dtSense)[0];
                    res.CellSubSense = GetSense(res.CellName, dtSense)[1];
                    if (Convert.ToDouble(Rate[0]) > 1)
                    {
                        double dAfterGrowthRate = AfterGrowthRate(Rate[1], Convert.ToDouble(Rate[0]), strStdTyp, strTyp,
                           res.CellName, sqlconn);
                        FindESTData(dAfterGrowthRate, Rate, res, sqlconn);
                    }
                    else
                    {
                        res.Remarks = "增长率小于等于1";
                    }
                }
                Insert(res, sqlconn, strTableName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (sqlconn != null)
                    sqlconn.Close();
            }
        }

        /// <summary>
        /// 获取小区增长率
        /// </summary>
        /// <param name="strTyp"></param>
        /// <param name="strStdTyp"></param>
        /// <param name="strCellName"></param>
        /// <returns></returns>
        private string[] GetCellGrowthRate(string strTyp, string strStdTyp, ResObj res, SqlConnection sqlconn)
        {
            double dGrowthRate = 0;
            string[] strsRate = new string[2];
            //this.oConnOpen();
            SqlCommand oComm = sqlconn.CreateCommand();
            SqlDataReader dr = null;
            oComm.CommandTimeout = 0;
            try
            {
                oComm.CommandText = "select " +
                                         "(a.平均RRC有效用户数 / b.平均RRC有效用户数) as '平均RRC有效用户数'," +
                                         "(a.上行PUSCH_PRB利用率 / b.上行PUSCH_PRB利用率) as '上行PUSCH_PRB利用率'," +
                                         "(a.下行PDSCH_PRB利用率 / b.下行PDSCH_PRB利用率) as '下行PDSCH_PRB利用率'," +
                                         "(a.下行PDCCH_CCE利用率 / b.下行PDCCH_CCE利用率) as '下行PDCCH_CCE利用率'," +
                                         "(a.上行流量G / b.上行流量G) as '上行流量G'," +
                                         "(a.下行流量G / b.下行流量G) as '下行流量G'" +
                                         "from After_Avgdata_" + strStdTyp + "_" + strTyp + " a,Before_Avgdata_" +
                                         strStdTyp + "_" + strTyp + " b " +
                                         "where a.cellname = '" + res.CellName + "' and b.cellname = '" + res.CellName +
                                         "' " +
                                         "and b.平均RRC有效用户数 > 0 and b.上行PUSCH_PRB利用率 > 0 " +
                                         "and b.下行PDSCH_PRB利用率 > 0 and b.下行PDCCH_CCE利用率 > 0 " +
                                         "and b.上行流量G > 0 and b.下行流量G > 0 ";
                dr = oComm.ExecuteReader();
                int nTemp = 0;
                string strTemp = "";
                if (dr.Read() && dr.HasRows)
                {
                    dGrowthRate = Convert.ToDouble(dr[0]);
                    for (int i = 1; i < 6; i++)
                    {
                        if (dGrowthRate <= Convert.ToDouble(dr[i]))
                        {
                            dGrowthRate = Convert.ToDouble(dr[i]);
                            nTemp = i;
                        }
                    }

                    switch (nTemp)
                    {
                        case 0:
                            strTemp = "平均RRC有效用户数";
                            break;
                        case 1:
                            strTemp = "上行PUSCH_PRB利用率";
                            break;
                        case 2:
                            strTemp = "下行PDSCH_PRB利用率";
                            break;
                        case 3:
                            strTemp = "下行PDCCH_CCE利用率";
                            break;
                        case 4:
                            strTemp = "上行流量G";
                            break;
                        case 5:
                            strTemp = "下行流量G";
                            break;

                    }
                    strsRate[0] = dGrowthRate.ToString();
                    strsRate[1] = strTemp;
                    res.Remarks = "";
                }
                else
                {
                    strsRate[0] = "1.2";
                    strsRate[1] = "下行流量G";
                    res.Remarks = "给定增量";
                }

                dr.Close();
                return strsRate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                oComm.Dispose();
                dr.Close();
            }
        }

        /// <summary>
        /// 获取预估指标数据
        /// </summary>
        /// <param name="dAfterGrowthRate"></param>
        private void FindESTData(double dAfterGrowthRate, string[] Rate, ResObj res, SqlConnection sqlconn)
        {
            SqlCommand oComm = sqlconn.CreateCommand();
            oComm.CommandTimeout = 0;
            SqlDataReader dr = null;

            try
            {
                string strIndexName = "";
                if (Rate[1] == "下行流量G")
                    strIndexName = "(小区用户面下行字节数/(1024.0*1024.0))";
                else if (Rate[1] == "上行流量G")
                    strIndexName = "(小区用户面上行字节数/(1024.0*1024.0))";
                else if (Rate[1] == "平均RRC有效用户数")
                    strIndexName = "有效RRC连接平均数";
                else
                    strIndexName = Rate[1];


                oComm.CommandText = "select top 1 平均E_RAB流量," +
                                    "E_RAB建立成功数," +
                                    "有效RRC连接平均数," +
                                    "上行PUSCH_PRB利用率," +
                                    "小区用户面上行字节数," +
                                    "下行PDSCH_PRB利用率," +
                                    "下行PDCCH_CCE利用率," +
                                    "小区用户面下行字节数," +
                                    "最大利用率 " +
                                    "from Tempdata_After where " + strIndexName + " = (select min(" + strIndexName + ") " +
                                    "from Tempdata_After where " + dAfterGrowthRate.ToString() + " <= " + strIndexName + ")" +
                                    "order by 最大利用率 desc,总流量 desc";
                dr = oComm.ExecuteReader();

                if (dr.Read() && dr.HasRows)
                {
                    DateTime FindBig = DateTime.Now;
                    string strCellTyp = Calculate.CheckCellType(Convert.ToDouble(dr["平均E_RAB流量"]));
                    string[] strStandard = CheckStandard(dr, strCellTyp);
                    if (strStandard[0] == "满足" || strStandard[1] == "满足")
                    {
                        res.GrowthIsExp = "满足";
                        res.Priority = GetPriority(Convert.ToDouble(dr["最大利用率"]));
                    }
                    else
                    {
                        res.GrowthIsExp = "不满足";
                        if (string.IsNullOrEmpty(res.Remarks))
                            res.Remarks = "预估不满足高负荷规则";
                    }

                    res.GrowthMaxRate = Convert.ToDouble(dr["最大利用率"]);
                    res.CellTyp_EST = strCellTyp;
                    res.ERAB_EST = Convert.ToDouble(dr["平均E_RAB流量"]);
                    res.RRC_EST = Convert.ToDouble(dr["有效RRC连接平均数"]);
                    res.UL_PUSCH_PRB_EST = Convert.ToDouble(dr["上行PUSCH_PRB利用率"]);
                    res.UL_Flow_EST = Convert.ToDouble(dr["小区用户面上行字节数"]) / (1024.0 * 1024.0);
                    res.DL_PDSCH_PRB_EST = Convert.ToDouble(dr["下行PDSCH_PRB利用率"]);
                    res.DL_PDCCH_CCE_EST = Convert.ToDouble(dr["下行PDCCH_CCE利用率"]);
                    res.DL_Flow_EST = Convert.ToDouble(dr["小区用户面下行字节数"]) / (1024.0 * 1024.0);
                    res.UL_Std_EST = strStandard[0];
                    res.DL_Std_EST = strStandard[1];
                    Console.WriteLine("找最大耗时：" + DateTime.Now.Subtract(FindBig).TotalSeconds.ToString());
                }
                else
                {
                    dr.Close();
                    DateTime FindSmall = DateTime.Now;
                    oComm.CommandText = "select top 1 平均E_RAB流量," +
                                             "E_RAB建立成功数," +
                                             "有效RRC连接平均数," +
                                             "上行PUSCH_PRB利用率," +
                                             "小区用户面上行字节数," +
                                             "下行PDSCH_PRB利用率," +
                                             "下行PDCCH_CCE利用率," +
                                             "小区用户面下行字节数," +
                                             "最大利用率 " +
                                             "from Tempdata_After where " + strIndexName + " = (select max(" + strIndexName + ") " +
                                             "from Tempdata_After where " + dAfterGrowthRate.ToString() + " >= " + strIndexName + ") " +
                                             "order by 最大利用率 desc,总流量 desc";
                    dr = oComm.ExecuteReader();
                    if (dr.Read() && dr.HasRows)
                    {
                        string strCellTyp = Calculate.CheckCellType(Convert.ToDouble(dr["平均E_RAB流量"]));
                        string[] strStandard = CheckStandard(dr, strCellTyp);
                        if (strStandard[0] == "满足" || strStandard[1] == "满足")
                        {
                            res.GrowthIsExp = "满足";
                            res.Priority = GetPriority(Convert.ToDouble(dr["最大利用率"]));
                        }
                        else
                        {
                            res.GrowthIsExp = "不满足";
                            if (string.IsNullOrEmpty(res.Remarks))
                                res.Remarks = "预估不满足高负荷规则";
                        }

                        res.GrowthMaxRate = Convert.ToDouble(dr["最大利用率"]);
                        res.CellTyp_EST = strCellTyp;
                        res.ERAB_EST = Convert.ToDouble(dr["E_RAB建立成功数"]);
                        res.RRC_EST = Convert.ToDouble(dr["有效RRC连接平均数"]);
                        res.UL_PUSCH_PRB_EST = Convert.ToDouble(dr["上行PUSCH_PRB利用率"]);
                        res.UL_Flow_EST = Convert.ToDouble(dr["小区用户面上行字节数"]) / (1024.0 * 1024.0);
                        res.DL_PDSCH_PRB_EST = Convert.ToDouble(dr["下行PDSCH_PRB利用率"]);
                        res.DL_PDCCH_CCE_EST = Convert.ToDouble(dr["下行PDCCH_CCE利用率"]);
                        res.DL_Flow_EST = Convert.ToDouble(dr["小区用户面下行字节数"]) / (1024.0 * 1024.0);
                        res.UL_Std_EST = strStandard[0];
                        res.DL_Std_EST = strStandard[1];
                        Console.WriteLine("找最小耗时：" + DateTime.Now.Subtract(FindSmall).TotalSeconds.ToString());
                    }
                    else
                    {
                        res.Remarks = "未匹配到数据";
                    }
                }

                dr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                oComm.Dispose();
                dr.Close();
            }
        }


        /// <summary>
        /// 获取增量后的数据 用于匹配原始数据
        /// </summary>
        /// <param name="strIdexTyp"></param>
        /// <param name="dValue"></param>
        /// <param name="strStdTyp"></param>
        /// <param name="strTyp"></param>
        /// <param name="strCellName"></param>
        /// <returns></returns>
        private double AfterGrowthRate(string strIdexTyp, double dValue, string strStdTyp, string strTyp, string strCellName, SqlConnection sqlconn)
        {
            SqlCommand oComm = sqlconn.CreateCommand();
            SqlDataReader dr = null;
            oComm.CommandTimeout = 0;
            try
            {
                double dAfterGrowthRate = 0;
                oComm.CommandText = "select " +
                                         strIdexTyp +
                                         " from After_Avgdata_" + strStdTyp + "_" + strTyp +
                                         " where cellname = '" + strCellName + "'";

                dr = oComm.ExecuteReader();
                if (dr.Read() && dr.HasRows)
                {
                    dAfterGrowthRate = Convert.ToDouble(dr[strIdexTyp]) * dValue;
                }
                dr.Close();
                return dAfterGrowthRate;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                oComm.Dispose();
                dr.Close();
            }
        }
       
    }
}
