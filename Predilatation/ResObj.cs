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


        private string strConn;

        public ResObj()
        {
            
            
        }

        public SqlConnection Open()
        {
            this.strConn = "data source=" + SqlServerHelper.strserver + ";Initial Catalog=Predilatation;user id=" + SqlServerHelper.struser + ";pwd=" + SqlServerHelper.strpwd + ";Pooling=true;Max Pool Size=40000;Min Pool Size=0";
            SqlConnection oConn = new SqlConnection(strConn);     
            return oConn;
        }

       /// <summary>
       /// 返回结果对象
       /// </summary>
       /// <param name="dr"></param>
       /// <returns></returns>
        public bool GetResObj(ResObj res, string strStdTyp, string strTyp, SqlConnection sqlconn, string strTableName, DataTable dtSense)
       {
           try
           {
               sqlconn.Open();
               
               //满足单日自忙时标准
               int nFreqTimes = CalcuateFreqTimes(strStdTyp, strTyp, res.CellName, sqlconn);
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

               }
               else
               {
                   //计算增长率
                   string[] Rate = GetCellGrowthRate(strTyp, strStdTyp, res.CellName, sqlconn);
                   res.AvgIsExp = strAvgIsExp;
                   res.ExpFreqTimes = nFreqTimes;
                   res.GrowthIsExp = "";
                   res.GrowthCellTyp = Rate[1];
                   res.GrowthRate = Convert.ToDouble(Rate[0]);
                   if (Convert.ToDouble(Rate[0]) > 1)
                   {
                       double dAfterGrowthRate = AfterGrowthRate(Rate[1], Convert.ToDouble(Rate[0]), strStdTyp, strTyp,
                           res.CellName, sqlconn);
                       if (!FindESTData(dAfterGrowthRate, Rate, res, sqlconn))
                       {
                           return true;
                       }

                       res.CellSense = GetSense(res.CellName, dtSense)[0];
                       res.CellSubSense = GetSense(res.CellName, dtSense)[1];
                   }
                   else
                   {
                       return true;
                   }
               }
               Insert(res, sqlconn, strTableName);
               return false;
           }
           catch (Exception e)
           {
               Console.WriteLine(e);
               throw;
           }
           finally
           {
               if(sqlconn != null)
                    sqlconn.Close();
           }
       }

        /// <summary>
        /// 写入结果
        /// </summary>
        /// <param name="res"></param>
        /// <param name="sqlconn"></param>
        /// <param name="strTableName"></param>
        public void Insert(ResObj res,SqlConnection sqlconn,string strTableName)
        {
            SqlCommand oComm = sqlconn.CreateCommand();
            oComm.CommandTimeout = 0;
            DateTime TimeStart = DateTime.Now;
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
                                    "小区归属子场景) values " +
                                    "('" + res.CellName + "'," + 
                                    res.Priority + ",'" + res.AvgIsExp + "','" + 
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
                                    res.CellSubSense + "')";
               
                oComm.ExecuteNonQuery();
                Console.WriteLine("插入耗时："+DateTime.Now.Subtract(TimeStart).TotalSeconds.ToString());
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
        /// 获取预估指标数据
        /// </summary>
        /// <param name="dAfterGrowthRate"></param>
        private bool FindESTData(double dAfterGrowthRate, string[] Rate, ResObj res, SqlConnection sqlconn)
        {
            SqlCommand oComm = sqlconn.CreateCommand();
            oComm.CommandTimeout = 0;
            SqlDataReader dr = null;
            DateTime TimeStart = DateTime.Now;
            try
            {
                string strIndexName = "";
                if (Rate[1] == "下行流量G")
                    strIndexName = "(小区用户面下行字节数/(1024.0*1024.0))";
                else if (Rate[1] == "上行流量G")
                    strIndexName = "(小区用户面下行字节数/(1024.0*1024.0))";
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
                                    "from Tempdata_After where " + dAfterGrowthRate.ToString() + " < " + strIndexName + ")" +
                                    "order by 最大利用率 desc,总流量 desc";
                dr = oComm.ExecuteReader();
                
                if (dr.Read() && dr.HasRows)
                {
                    string strCellTyp = Calculate.CheckCellType(Convert.ToDouble(dr["平均E_RAB流量"]));
                    string[] strStandard = CheckStandard(dr, strCellTyp);
                    if (strStandard[0] == "满足" || strStandard[1] == "满足")
                        res.GrowthIsExp = "满足";
                    else
                    {
                        dr.Close();
                        return false;
                    }
                    res.GrowthMaxRate = Convert.ToDouble(dr["最大利用率"]);
                    res.Priority = GetPriority(Convert.ToDouble(dr["最大利用率"]));
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
                }
                else
                {
                    dr.Close();
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
                                             "from Tempdata_After where " + dAfterGrowthRate.ToString() + " > " + strIndexName + ") " +
                                             "order by 最大利用率 desc,总流量 desc";
                    dr = oComm.ExecuteReader();
                    if (dr.Read() && dr.HasRows)
                    {
                        string strCellTyp = Calculate.CheckCellType(Convert.ToDouble(dr["平均E_RAB流量"]));
                        string[] strStandard = CheckStandard(dr, strCellTyp);
                        if (strStandard[0] == "满足" || strStandard[1] == "满足")
                            res.GrowthIsExp = "满足";
                        else
                        {
                            dr.Close();
                            return false;
                        }
                        res.GrowthMaxRate = Convert.ToDouble(dr["最大利用率"]);
                        res.Priority = GetPriority(Convert.ToDouble(dr["最大利用率"]));
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
                    }
                }
                Console.WriteLine("找最大耗时：" + DateTime.Now.Subtract(TimeStart).TotalSeconds.ToString());
                dr.Close();
                return true;
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
       private string[] CheckStandard(SqlDataReader dr, string strCellTyp)
        {
            string[] strStandard = new string[2];

            if (strCellTyp == "大包小区")
            {
                //上行标准
                if (CheckStandard(MainForm.Param.lstBu, dr))
                    strStandard[0] = "满足";
                else
                    strStandard[0] = "";

                //下行标准
                if (CheckStandard(MainForm.Param.lstBd, dr))
                    strStandard[1] = "满足";
                else
                    strStandard[1] = "";
            }
            if (strCellTyp == "中包小区")
            {
                //上行标准
                if (CheckStandard(MainForm.Param.lstMu, dr))
                    strStandard[0] = "满足";
                else
                    strStandard[0] = "";

                //下行标准
                if (CheckStandard(MainForm.Param.lstMd, dr))
                    strStandard[1] = "满足";
                else
                    strStandard[1] = "";
            }
            if (strCellTyp == "小包小区")
            {
                //上行标准
                if (CheckStandard(MainForm.Param.lstSu, dr))
                    strStandard[0] = "满足";
                else
                    strStandard[0] = "";

                //下行标准
                if (CheckStandard(MainForm.Param.lstSd, dr))
                    strStandard[1] = "满足";
                else
                    strStandard[1] = "";
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
            oComm.CommandTimeout = 0;
            try
            {
                double dAfterGrowthRate = 0;
                oComm.CommandText = "select " +
                                         strIdexTyp +
                                         " from After_Avgdata_" + strStdTyp + "_" + strTyp +
                                         " where cellname = '" + strCellName + "'";

                SqlDataReader dr = oComm.ExecuteReader();
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
                //this.oConn.Close();
            }
        }

        /// <summary>
        /// 获取最大增长率
        /// </summary>
        /// <param name="strTyp"></param>
        /// <param name="strStdTyp"></param>
        /// <param name="strCellName"></param>
        /// <returns></returns>
        private double GetGrowthMaxRate(string strTyp, string strStdTyp, string strCellName,SqlConnection sqlconn)
        {
            SqlCommand oComm = sqlconn.CreateCommand();
            oComm.CommandTimeout = 0;
            try
            {
                double dMaxRate = 0;
                
                oComm.CommandText = "select " +
                                         "(a.上行PUSCH_PRB利用率 / b.上行PUSCH_PRB利用率) as '上行PUSCH_PRB增长率'," +
                                         "(a.下行PDSCH_PRB利用率 / b.下行PDSCH_PRB利用率) as '下行PDSCH_PRB增长率'," +
                                         "(a.下行PDCCH_CCE利用率 / b.下行PDCCH_CCE利用率) as '下行PDCCH_CCE增长率' " +
                                         "from After_Avgdata_" + strStdTyp + "_" + strTyp + " a,Before_Avgdata_" +
                                         strStdTyp + "_" + strTyp + " b " +
                                         "where a.cellname = '" + strCellName + "' and b.cellname = '" + strCellName +
                                         "' " +
                                         "and b.上行PUSCH_PRB利用率 > 0 " +
                                         "and b.下行PDSCH_PRB利用率 > 0 " +
                                         "and b.下行PDCCH_CCE利用率 > 0 ";
                SqlDataReader dr = oComm.ExecuteReader();
                if (dr.Read() && dr.HasRows)
                {
                    dMaxRate = SqlServerHelper.GetMax(Convert.ToDouble(dr["上行PUSCH_PRB增长率"]),
                        Convert.ToDouble(dr["下行PDSCH_PRB增长率"]), Convert.ToDouble(dr["下行PDCCH_CCE增长率"]));
                }
                dr.Close();
               
                return dMaxRate;
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
        /// 获取场景类型
        /// </summary>
        /// <param name="strCellName"></param>
        /// <returns></returns>
        private string[] GetSense(string strCellName,DataTable dtSense)
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
       private int CalcuateFreqTimes(string strStdTyp, string strTyp, string strCellname,SqlConnection sqlconn)
       {
           int nFreqTimes = 0;
           SqlCommand oComm = sqlconn.CreateCommand();
           oComm.CommandTimeout = 0;
           try
           {
               oComm.CommandText = "(select COUNT(cellname) from After_Date_" + strStdTyp + "_" + strTyp +
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
       /// 获取小区增长率
       /// </summary>
       /// <param name="strTyp"></param>
       /// <param name="strStdTyp"></param>
       /// <param name="strCellName"></param>
       /// <returns></returns>
       private string[] GetCellGrowthRate(string strTyp, string strStdTyp, string strCellName,SqlConnection sqlconn)
       {
           double dGrowthRate = 0;
           string[] strsRate = new string[2];
           //this.oConnOpen();
           SqlCommand oComm = sqlconn.CreateCommand();
           oComm.CommandTimeout = 0;
           try
           {
               //this.oConn.ChangeDatabase("Predilatation");
               
               oComm.CommandText = "select " +
                                        "(a.平均RRC有效用户数 / b.平均RRC有效用户数) as '平均RRC有效用户数'," +
                                        "(a.上行PUSCH_PRB利用率 / b.上行PUSCH_PRB利用率) as '上行PUSCH_PRB利用率'," +
                                        "(a.下行PDSCH_PRB利用率 / b.下行PDSCH_PRB利用率) as '下行PDSCH_PRB利用率'," +
                                        "(a.下行PDCCH_CCE利用率 / b.下行PDCCH_CCE利用率) as '下行PDCCH_CCE利用率'," +
                                        "(a.上行流量G / b.上行流量G) as '上行流量G'," +
                                        "(a.下行流量G / b.下行流量G) as '下行流量G'" +
                                        "from After_Avgdata_" + strStdTyp + "_" + strTyp + " a,Before_Avgdata_" +
                                        strStdTyp + "_" + strTyp + " b " +
                                        "where a.cellname = '" + strCellName + "' and b.cellname = '" + strCellName +
                                        "' " +
                                        "and b.平均RRC有效用户数 > 0 and b.上行PUSCH_PRB利用率 > 0 " +
                                        "and b.下行PDSCH_PRB利用率 > 0 and b.下行PDCCH_CCE利用率 > 0 " +
                                        "and b.上行流量G > 0 and b.下行流量G > 0 ";
               SqlDataReader dr = oComm.ExecuteReader();
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
               }
               dr.Close();

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
           }
       }

       /// <summary>
       /// 判断均值是否满足
       /// </summary>
       /// <param name="strULStd"></param>
       /// <param name="strDLStd"></param>
       /// <returns></returns>
       private string CheckAvgIsExp(string strULStd, string strDLStd)
       {
           string strChkRes = "";
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
       private int GetPriority(string strAvgIsExp, int nFreqTimes)
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
        private int GetPriority(double dGrowthRate)
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
