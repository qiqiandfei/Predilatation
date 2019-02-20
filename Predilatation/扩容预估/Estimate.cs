using System;
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
    public class Estimate:SqlServerHelper
    {
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
               
                //业务表
                string strTBName = "";
                //结果表
                string strTableName = "";
                
                //根据类型取表名
                if (strEstType == "Holiday")
                {
                    strTBName = "CurNet_Avgdata_" + strSdtType + "_" + strType;
                }
                else
                {
                    strTBName = "After_Avgdata_" + strSdtType + "_" + strType;
                }
                strTableName = CreateResTable(strType, strSdtType, strEstType);
                this.oConn.Open();
                this.oConn.ChangeDatabase("Predilatation");
                string sql =  " select cellname," +
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
                            "'' as '小区归属子场景'," +
                            "'' as '备注' " + 
                            " from " + strTBName;

                SqlDataAdapter sqlData = new SqlDataAdapter(sql, this.oConn);
                DataSet ds = new DataSet();
                sqlData.Fill(ds, "table1");
                this.oConn.Close();

                //结果放入线程安全集合
                ConcurrentBag<ResObj> lstRes = new ConcurrentBag<ResObj>();
                lstRes = SetResObj(ds.Tables[0], lstRes);

                //创建结果对象
                AbstractResObj resobj = Factory_Res.CreateResObj(strEstType);

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

                Parallel.ForEach(lstRes, item =>
                {
                    try
                    {
                        SqlConnection sqlconn = resobj.Open();

                        resobj.GetResObj(item, strSdtType, strType, sqlconn, strTableName, dtSense);

                        //线程安全计数器
                        Interlocked.Increment(ref nCount);

                        MainForm.nProValue = (int)((double)nCount * 100 / (double)ds.Tables[0].Rows.Count);
                        MainForm.strCurTip = "正在计算预估值，当前第：" + nCount.ToString() + "条，共：" + ds.Tables[0].Rows.Count + "条...";
                        ((TipReFresher)trf).CurTip();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                });

                //单线程逻辑测试用
                //foreach (var item in lstRes)
                //{

                //    nCount++;
                //    SqlConnection sqlconn = resobj.Open();
                //    resobj.GetResObj(item, strSdtType, strType, sqlconn, strTableName, dtSense);

                //    MainForm.nProValue = (int)((double)nCount * 100 / (double)ds.Tables[0].Rows.Count);
                //    MainForm.strCurTip = "正在计算预估值，当前第：" + nCount.ToString() + "条，共：" + ds.Tables[0].Rows.Count + "条...";
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
        /// 计算预扩容
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="strSdtType"></param>
        /// <param name="strEstType"></param>
        public void CalculateEst(double dRateGiven, string strType, string strSdtType, string strEstType, object trf)
        {
            try
            {
                string strTBName = "Before_Avgdata_" + strSdtType + "_" + strType;

                //创建结果表
                string strTableName = CreateResTable_Given(strType,strSdtType,strEstType);
                this.oConn.Open();
                this.oConn.ChangeDatabase("Predilatation");
                string sql = " select cellname," +
                            "'' as '小区流量分类'," +
                            "0 as '单小区流量增长因子C'," +
                            "0 as '单小区增长X倍后的总流量'," +
                            "ERAB as 'ERAB_avg'," +
                            "平均RRC有效用户数 as '平均RRC有效用户数_avg'," +
                            "上行PUSCH_PRB利用率 as '上行PUSCH_PRB利用率_avg'," +
                            "上行流量G as '上行流量G_avg'," +
                            "下行PDSCH_PRB利用率 as '下行PDSCH_PRB利用率_avg'," +
                            "下行PDCCH_CCE利用率 as '下行PDCCH_CCE利用率_avg'," +
                            "下行流量G as '下行流量G_avg'," +
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
                            "'' as '小区归属子场景'," +
                            "'' as '备注' " +
                            "from " + strTBName;

                SqlDataAdapter sqlData = new SqlDataAdapter(sql, this.oConn);
                DataSet ds = new DataSet();
                sqlData.Fill(ds, "table1");
                this.oConn.Close();

                //结果放入线程安全集合
                ConcurrentBag<ResObj> lstRes = new ConcurrentBag<ResObj>();
                lstRes = SetResObj_Rate(ds.Tables[0], lstRes);

                //创建结果对象
                ResObj_GivenRate resobj = new ResObj_GivenRate();

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

                //获取增长因子A
                double dRateA = resobj.GetRateA(strSdtType, strType);
                //获取增长因子B
                double dRateB = dRateGiven / dRateA;
                //创建临时Table
                resobj.GetAvgTable_Before(strSdtType, strType,dRateB,lstRes,trf);

                //创建计数器
                int nCount = 0;

                Parallel.ForEach(lstRes, item =>
                {
                    try
                    {
                        SqlConnection sqlconn = resobj.Open();

                        resobj.GetResObj(item, strSdtType, strType, sqlconn, strTableName, dtSense);

                        //线程安全计数器
                        Interlocked.Increment(ref nCount);

                        MainForm.nProValue = (int)((double)nCount * 100 / (double)ds.Tables[0].Rows.Count);
                        MainForm.strCurTip = "正在写入预估结果，当前第：" + nCount.ToString() + "条，共：" + ds.Tables[0].Rows.Count + "条...";
                        ((TipReFresher)trf).CurTip();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                });

                //单线程逻辑测试用
                //foreach (var item in lstRes)
                //{

                //    nCount++;
                //    SqlConnection sqlconn = resobj.Open();
                //    resobj.GetResObj(item, strSdtType, strType, sqlconn, strTableName, dtSense);

                //    MainForm.nProValue = (int)((double)nCount * 100 / (double)ds.Tables[0].Rows.Count);
                //    MainForm.strCurTip = "正在计算预估值，当前第：" + nCount.ToString() + "条，共：" + ds.Tables[0].Rows.Count + "条...";
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
                res.Remarks = Convert.ToString(row["备注"]); 
                lstResObj.Add(res);
            }
            return lstResObj;
        }

        /// <summary>
        /// 将DataTable存放到线程安全集合
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="lstResObj"></param>
        /// <returns></returns>
        private ConcurrentBag<ResObj> SetResObj_Rate(DataTable dt, ConcurrentBag<ResObj> lstResObj)
        {

            foreach (DataRow row in dt.Rows)
            {
                ResObj res = new ResObj();
                res.CellName = Convert.ToString(row["cellname"]);
                res.CellTyp_AVG = Convert.ToString(row["小区流量分类"]);
                res.RateC = Convert.ToDouble(row["单小区流量增长因子C"]);
                res.XRateFlow = Convert.ToDouble(row["单小区增长X倍后的总流量"]);
                res.ERAB_AVG = Convert.ToDouble(row["ERAB_avg"]);
                res.RRC_AVG = Convert.ToDouble(row["平均RRC有效用户数_avg"]);
                res.UL_PUSCH_PRB_AVG = Convert.ToDouble(row["上行PUSCH_PRB利用率_avg"]);
                res.UL_Flow_AVG = Convert.ToDouble(row["上行流量G_avg"]);
                res.DL_PDSCH_PRB_AVG = Convert.ToDouble(row["下行PDSCH_PRB利用率_avg"]);
                res.DL_PDCCH_CCE_AVG = Convert.ToDouble(row["下行PDCCH_CCE利用率_avg"]);
                res.DL_Flow_AVG = Convert.ToDouble(row["下行流量G_avg"]);
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
                res.Remarks = Convert.ToString(row["备注"]);
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
                                    "小区归属子场景 varchar(50)," +
                                    "备注 varchar(50))";
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
        /// 创建结果表
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="strSdtType"></param>
        /// <param name="strEstType"></param>
        public string CreateResTable_Given(string strType, string strSdtType, string strEstType)
        {
            string strTableName = "Res_Given_" + strSdtType  + "_" + strType;
            this.oConn.Open();
            this.oConn.ChangeDatabase("Predilatation");
            try
            {
                oComm.CommandText = "if  exists(select * from sysobjects where  type = 'U' and name = '" + strTableName +
                                    "') drop table " + strTableName + " " +
                                    "create table " + strTableName + "" +
                                    "(cellname varchar(255) not null," +
                                    "小区流量分类 varchar(50)," +
                                    "单小区流量增长因子C numeric(18,4)," +
                                    "单小区增长X倍后的总流量 numeric(18,4)," +
                                    "ERAB_avg numeric(18,4)," +
                                    "平均RRC有效用户数_avg numeric(18,4)," +
                                    "上行PUSCH_PRB利用率_avg numeric(18,4)," +
                                    "上行流量G_avg numeric(18,4)," +
                                    "下行PDSCH_PRB利用率_avg numeric(18,4)," +
                                    "下行PDCCH_CCE利用率_avg numeric(18,4)," +
                                    "下行流量G_avg numeric(18,4)," +
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
                                    "小区归属子场景 varchar(50)," +
                                    "备注 varchar(50))";
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
    }
}
