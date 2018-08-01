﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Predilatation
{
    public class EstimateDaily:SqlServerHelper
    {
        private readonly object CaculateMutex = new object();
        /// <summary>
        /// 计算预扩容
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="strSdtType"></param>
        /// <param name="strEstType"></param>
        public void CalculateEst(string strType, string strSdtType, string strEstType,object trf)
        {
            try
            {
                MainForm.strCurTip = "正在创建结果表...";
                ((TipReFresher)trf).CurTip();
                if (strType != "Utilizaerate")
                    strType = strType + "Busy";
                //创建结果表
                string strTableName = CreateResTable(strType, strSdtType, strEstType);
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                string sql = "select cellname," +
                                      "0 as '扩容优先级'," +
                                      "'' as '均值是否满足扩容'," +
                                      "'' as '增量是否满足扩容'," +
                                      "'' as '增量小区类型'," +
                                      "0 as '增量小区增长率'," +
                                      "0 as '增量Max利用率'," +
                                      "0 as '满足扩容频次'," +
                                      "celltype as '小区包类型_avg'," +
                                      "ERAB as 'ERAB_avg'," +
                                      "平均RRC有效用户数 as '平均RRC有效用户数_avg'," +
                                      "上行PUSCH_PRB利用率 as '上行PUSCH_PRB利用率_avg'," +
                                      "上行流量G as '上行流量G_avg'," +
                                      "下行PDSCH_PRB利用率 as '下行PDSCH_PRB利用率_avg'," +
                                      "下行PDCCH_CCE利用率 as '下行PDCCH_CCE利用率_avg'," +
                                      "下行流量G as '下行流量G_avg'," +
                                      "上行扩容标准 as '上行扩容标准_avg'," +
                                      "下行扩容标准 as '下行扩容标准_avg'," +
                                      "'' as '小区包类型_est'," +
                                      "0 as 'ERAB_est'," +
                                      "0 as '平均RRC有效用户数_est'," +
                                      "0 as '上行PUSCH_PRB利用率_est'," +
                                      "0 as '上行流量G_est'," +
                                      "0 as '下行PDSCH_PRB利用率_est'," +
                                      "0 as '下行PDCCH_CCE利用率_est'," +
                                      "0 as '下行流量G_est'," +
                                      "'' as '上行扩容标准_est'," +
                                      "'' as '下行扩容标准_est'," +
                                      "'' as '小区归属场景'," +
                                      "'' as '小区归属子场景'" +
                                      "from After_Avgdata_" + strSdtType + "_" + strType;
                SqlDataAdapter sqlData = new SqlDataAdapter(sql, this.oConn);
                DataSet ds = new DataSet();
                sqlData.Fill(ds, "table1");
                this.oConn.Close();

                //结果放入线程安全集合
                ConcurrentBag<ResObj> lstRes = new ConcurrentBag<ResObj>();
                lstRes = SetResObj(ds.Tables[0], lstRes);

                //创建结果对象
                ResObj resobj = new ResObj();

                //获取场景对照表
                MainForm.strCurTip = "正在生成场景对照表...";
                ((TipReFresher)trf).CurTip();
                DataTable dtSense = resobj.GetSenseTable();
                if (dtSense == null)
                {
                    MainForm.strCurTip = "在工具运行目录找不到‘小区场景归属信息.xlsx’，请检查后重新运行工具...";
                    ((TipReFresher)trf).CurTip();
                    System.Threading.Thread.Sleep(5000);
                    return;
                }

                //创建计数器
                int nCount = 0;
                int nDiscard = 0;
                Parallel.ForEach(lstRes, item =>
                {
                    try
                    {
                        SqlConnection sqlconn = resobj.Open();
                        if (resobj.GetResObj(item, strSdtType, strType, sqlconn, strTableName, dtSense))
                        {
                            //线程安全计数器
                            Interlocked.Increment(ref nDiscard); 
                        }

                        //线程安全计数器
                        Interlocked.Increment(ref nCount); 

                        MainForm.nProValue = (int)((double)nCount * 100 / (double)ds.Tables[0].Rows.Count);
                        MainForm.strCurTip = "正在计算预估值，当前第：" + nCount.ToString() + "条，共：" + ds.Tables[0].Rows.Count + "条，已丢弃：" + nDiscard.ToString() + "条...";
                        ((TipReFresher)trf).CurTip();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                });
                //foreach (var item in lstRes)
                //{

                //    nCount++;
                //    SqlConnection sqlconn = resobj.Open();
                //    if (resobj.GetResObj(item, strSdtType, strType, sqlconn, strTableName, dtSense))
                //    {
                //        nDiscard++;
                //    }

                //    MainForm.nProValue = (int)((double)nCount * 100 / (double)ds.Tables[0].Rows.Count);
                //    MainForm.strCurTip = "正在计算预估值，当前第：" + nCount.ToString() + "条，共：" + ds.Tables[0].Rows.Count + "条，已丢弃：" + nDiscard.ToString() + "条...";
                //    ((TipReFresher)trf).CurTip();

                //}

                MainForm.nProValue = 100;
                ((TipReFresher)trf).CurTip();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                this.oConn.Close();
            }
        }

        /// <summary>
        /// 将DataTable存放到线程安全集合
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="lstResObj"></param>
        /// <returns></returns>
        private ConcurrentBag<ResObj> SetResObj(DataTable dt, ConcurrentBag<ResObj> lstResObj)
        {
            
            foreach (DataRow row in dt.Rows)
            {
                ResObj res = new ResObj();
                res.CellName = Convert.ToString(row["cellname"]);
                res.Priority = Convert.ToInt32(row["扩容优先级"]);
                res.AvgIsExp = Convert.ToString(row["均值是否满足扩容"]);
                res.GrowthIsExp = Convert.ToString(row["增量是否满足扩容"]);
                res.GrowthCellTyp = Convert.ToString(row["增量小区类型"]);
                res.GrowthRate = Convert.ToDouble(row["增量小区增长率"]);
                res.GrowthMaxRate = Convert.ToDouble(row["增量Max利用率"]);
                res.ExpFreqTimes = Convert.ToInt32(row["满足扩容频次"]);
                res.CellTyp_AVG = Convert.ToString(row["小区包类型_avg"]);
                res.ERAB_AVG = Convert.ToDouble(row["ERAB_avg"]);
                res.RRC_AVG = Convert.ToDouble(row["平均RRC有效用户数_avg"]);
                res.UL_PUSCH_PRB_AVG = Convert.ToDouble(row["上行PUSCH_PRB利用率_avg"]);
                res.UL_Flow_AVG = Convert.ToDouble(row["上行流量G_avg"]);
                res.DL_PDSCH_PRB_AVG = Convert.ToDouble(row["下行PDSCH_PRB利用率_avg"]);
                res.DL_PDCCH_CCE_AVG = Convert.ToDouble(row["下行PDCCH_CCE利用率_avg"]);
                res.DL_Flow_AVG = Convert.ToDouble(row["下行流量G_avg"]);
                res.UL_Std_AVG = Convert.ToString(row["上行扩容标准_avg"]);
                res.DL_Std_AVG = Convert.ToString(row["下行扩容标准_avg"]);
                res.CellTyp_EST = Convert.ToString(row["小区包类型_est"]);
                res.ERAB_EST = Convert.ToDouble(row["ERAB_est"]);
                res.RRC_EST = Convert.ToDouble(row["平均RRC有效用户数_est"]);
                res.UL_PUSCH_PRB_EST = Convert.ToDouble(row["上行PUSCH_PRB利用率_est"]);
                res.UL_Flow_EST = Convert.ToDouble(row["上行流量G_est"]);
                res.DL_PDSCH_PRB_EST = Convert.ToDouble(row["下行PDSCH_PRB利用率_est"]);
                res.DL_PDCCH_CCE_EST = Convert.ToDouble(row["下行PDCCH_CCE利用率_est"]);
                res.DL_Flow_EST = Convert.ToDouble(row["下行流量G_est"]);
                res.UL_Std_EST = Convert.ToString(row["上行扩容标准_est"]);
                res.DL_Std_EST = Convert.ToString(row["下行扩容标准_est"]);
                res.CellSense = Convert.ToString(row["小区归属场景"]);
                res.CellSubSense = Convert.ToString(row["小区归属子场景"]); 
                lstResObj.Add(res);
            }
            return lstResObj;
        }

        
        /// <summary>
        /// 创建结果表
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="strSdtType"></param>
        /// <param name="strEstType"></param>
        public string CreateResTable(string strType,string strSdtType,string strEstType)
        {
            string strTableName = "Res_" + strEstType + "_" + strSdtType + "_" + strType;
            this.oConnOpen();
            this.oConn.ChangeDatabase("Predilatation");
            try
            {
                oComm.CommandText = "if  exists(select * from sysobjects where  type = 'U' and name = '" + strTableName +
                                    "') drop table " + strTableName + " " +
                                    "create table " + strTableName + "" +
                                    "(cellname varchar(255) not null," +
                                    "扩容优先级 int," +
                                    "满足扩容频次 int," +
                                    "均值是否满足扩容 varchar(50)," +
                                    "增量小区类型 varchar(50)," +
                                    "增量小区增长率 numeric(18,4)," +
                                    "增量是否满足扩容 varchar(50)," +
                                    "增量Max利用率 numeric(18,4)," +
                                    "小区包类型_avg varchar(50)," +
                                    "ERAB_avg numeric(18,4)," +
                                    "平均RRC有效用户数_avg numeric(18,4)," +
                                    "上行PUSCH_PRB利用率_avg numeric(18,4)," +
                                    "上行流量G_avg numeric(18,4)," +
                                    "下行PDSCH_PRB利用率_avg numeric(18,4)," +
                                    "下行PDCCH_CCE利用率_avg numeric(18,4)," +
                                    "下行流量G_avg numeric(18,4)," +
                                    "上行扩容标准_avg varchar(50)," +
                                    "下行扩容标准_avg varchar(50)," +
                                    "小区包类型_est varchar(50)," +
                                    "ERAB_est numeric(18,4)," +
                                    "平均RRC有效用户数_est numeric(18,4)," +
                                    "上行PUSCH_PRB利用率_est numeric(18,4)," +
                                    "上行流量G_est numeric(18,4)," +
                                    "下行PDSCH_PRB利用率_est numeric(18,4)," +
                                    "下行PDCCH_CCE利用率_est numeric(18,4)," +
                                    "下行流量G_est numeric(18,4)," +
                                    "上行扩容标准_est varchar(50)," +
                                    "下行扩容标准_est varchar(50)," +
                                    "小区归属场景 varchar(50)," +
                                    "小区归属子场景 varchar(50))";
                oComm.ExecuteNonQuery();
                return strTableName;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                this.oConn.Close();
            }
        }

        /// <summary>
        /// 获取结果表结构
        /// </summary>
        /// <returns></returns>
        private DataTable GetResTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("cellname", typeof(string)),
                new DataColumn("扩容优先级", typeof(int)),
                new DataColumn("满足扩容频次", typeof(int)),
                new DataColumn("均值是否满足扩容", typeof(string)),
                new DataColumn("增量小区类型", typeof(string)),
                new DataColumn("增量小区增长率", typeof(double)),
                new DataColumn("增量是否满足扩容", typeof(string)),
                new DataColumn("增量Max利用率", typeof(double)),
                new DataColumn("小区包类型_avg", typeof(string)),
                new DataColumn("ERAB_avg", typeof(double)),
                new DataColumn("平均RRC有效用户数_avg", typeof(double)),
                new DataColumn("上行PUSCH_PRB利用率_avg", typeof(double)),
                new DataColumn("上行流量G_avg", typeof(double)),
                new DataColumn("下行PDSCH_PRB利用率_avg", typeof(double)),
                new DataColumn("下行PDCCH_CCE利用率_avg", typeof(double)),
                new DataColumn("下行流量G_avg", typeof(double)),
                new DataColumn("上行扩容标准_avg", typeof(string)),
                new DataColumn("下行扩容标准_avg", typeof(string)),
                new DataColumn("小区包类型_est", typeof(string)),
                new DataColumn("ERAB_est", typeof(double)),
                new DataColumn("平均RRC有效用户数_est", typeof(double)),
                new DataColumn("上行PUSCH_PRB利用率_est", typeof(double)),
                new DataColumn("上行流量G_est", typeof(double)),
                new DataColumn("下行PDSCH_PRB利用率_est", typeof(double)),
                new DataColumn("下行PDCCH_CCE利用率_est", typeof(double)),
                new DataColumn("下行流量G_est", typeof(double)),
                new DataColumn("上行扩容标准_est", typeof(string)),
                new DataColumn("下行扩容标准_est", typeof(string)),
                new DataColumn("小区归属场景", typeof(string)),
                new DataColumn("小区归属子场景", typeof(string))});
            return dt;
        }
    }
}