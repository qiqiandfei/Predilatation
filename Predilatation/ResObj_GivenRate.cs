using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Predilatation
{
    public class ResObj_GivenRate:ResObj
    {
        private DataTable dtAvg_Before { set; get; }
        private double dfactor_cap { set; get; }
        private double dfactor_hybrid { set; get; }
        private double dfactor_cover { set; get; }
        private ConcurrentBag<TempProcess> lstBag { set; get; }
        //容量类
        private double dfactor_ERAB_cap { set; get; }
        private double dfactor_UL_PUSCH_PRB_cap { set; get; }
        private double dfactor_DL_PUSCH_PRB_cap { set; get; }
        private double dfactor_DL_PDCCH_CCE_cap { set; get; }
        private double dfactor_AVG_RRC_cap { set; get; }
        private double dfactor_UL_FLOW_cap { set; get; }
        private double dfactor_DL_FLOW_cap { set; get; }
        //混合类
        private double dfactor_ERAB_hybrid { set; get; }
        private double dfactor_UL_PUSCH_PRB_hybrid { set; get; }
        private double dfactor_DL_PUSCH_PRB_hybrid { set; get; }
        private double dfactor_DL_PDCCH_CCE_hybrid { set; get; }
        private double dfactor_AVG_RRC_hybrid { set; get; }
        private double dfactor_UL_FLOW_hybrid { set; get; }
        private double dfactor_DL_FLOW_hybrid { set; get; }
        //覆盖类类
        private double dfactor_ERAB_cover { set; get; }
        private double dfactor_UL_PUSCH_PRB_cover { set; get; }
        private double dfactor_DL_PUSCH_PRB_cover { set; get; }
        private double dfactor_DL_PDCCH_CCE_cover { set; get; }
        private double dfactor_AVG_RRC_cover { set; get; }
        private double dfactor_UL_FLOW_cover { set; get; }
        private double dfactor_DL_FLOW_cover { set; get; }



        public void GetResObj(ResObj res, string strStdTyp, string strTyp, SqlConnection sqlconn,
            string strTableName, DataTable dtSense)
        {
            try
            {
               
                //获取类型和增长因子
                GetType(res);
                //获取单小区增量X倍后的总流量
                res.XRateFlow = GetXRateFlow(res);
                string [] standards = GetStandard(res);
                res.UL_Std_EST = standards[0];
                res.DL_Std_EST = standards[1];
                res.CellSense = GetSense(res.CellName, dtSense)[0];
                res.CellSubSense = GetSense(res.CellName, dtSense)[1];

                InsertRes(res, sqlconn, strTableName);
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
               
            }
        }

        /// <summary>
        /// 写入结果
        /// </summary>
        /// <param name="res"></param>
        /// <param name="sqlconn"></param>
        /// <param name="strTableName"></param>
        public void InsertRes(ResObj res, SqlConnection sqlconn, string strTableName)
        {
            if(sqlconn.State != ConnectionState.Open)
                sqlconn.Open();
            SqlCommand oComm = sqlconn.CreateCommand();
            oComm.CommandTimeout = 0;
            try
            {
                oComm.CommandText = "Insert Into " + strTableName + " (cellname," +
                                    "小区流量分类," +
                                    "单小区流量增长因子C," +
                                    "单小区增长X倍后的总流量," +
                                    "ERAB_avg," +
                                    "平均RRC有效用户数_avg," +
                                    "上行PUSCH_PRB利用率_avg," +
                                    "上行流量G_avg," +
                                    "下行PDSCH_PRB利用率_avg," +
                                    "下行PDCCH_CCE利用率_avg," +
                                    "下行流量G_avg," +
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
                                    res.CellName + "','" +
                                    res.CellTyp_AVG + "'," +
                                    res.RateC + "," +
                                    res.XRateFlow + "," +
                                    res.ERAB_AVG + "," +
                                    res.RRC_AVG + "," +
                                    res.UL_PUSCH_PRB_AVG + "," +
                                    res.UL_Flow_AVG + "," +
                                    res.DL_PDSCH_PRB_AVG + "," +
                                    res.DL_PDCCH_CCE_AVG + "," +
                                    res.DL_Flow_AVG + ",'" +
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
                if(sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }
        }

        /// <summary>
        /// 判断是否满足高负荷规则
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private string[] GetStandard(ResObj res)
        {
            string[] standards = new string[2];
            double dERAB = 0d;
            double dUL_PUSCH_PRB = 0d;
            double dDL_PUSCH_PRB = 0d;
            double dDL_PDCCH_CCE = 0d;
            double dAVG_RRC = 0d;
            double dUL_FLOW = 0d;
            double dDL_FLOW = 0d;

            try
            {
                if (res.CellTyp_AVG == "容量类")
                {
                    dERAB = res.ERAB_AVG * dfactor_ERAB_cap;
                    dUL_PUSCH_PRB = res.UL_PUSCH_PRB_AVG * dfactor_UL_PUSCH_PRB_cap;
                    dDL_PUSCH_PRB = res.DL_PDSCH_PRB_AVG * dfactor_DL_PUSCH_PRB_cap;
                    dDL_PDCCH_CCE = res.DL_PDCCH_CCE_AVG * dfactor_DL_PDCCH_CCE_cap;
                    dAVG_RRC = res.RRC_AVG * dfactor_AVG_RRC_cap;
                    dUL_FLOW = res.UL_Flow_AVG * dfactor_UL_FLOW_cap;
                    dDL_FLOW = res.DL_Flow_AVG  * dfactor_DL_FLOW_cap;

                }
                else if (res.CellTyp_AVG == "混合类")
                {
                    dERAB = res.ERAB_AVG * dfactor_ERAB_hybrid;
                    dUL_PUSCH_PRB = res.UL_PUSCH_PRB_AVG * dfactor_UL_PUSCH_PRB_hybrid;
                    dDL_PUSCH_PRB = res.DL_PDSCH_PRB_AVG * dfactor_DL_PUSCH_PRB_hybrid;
                    dDL_PDCCH_CCE = res.DL_PDCCH_CCE_AVG * dfactor_DL_PDCCH_CCE_hybrid;
                    dAVG_RRC = res.RRC_AVG * dfactor_AVG_RRC_hybrid;
                    dUL_FLOW = res.UL_Flow_AVG * dfactor_UL_FLOW_hybrid;
                    dDL_FLOW = res.DL_Flow_AVG * dfactor_DL_FLOW_hybrid;
                }
                else if (res.CellTyp_AVG == "覆盖类")
                {
                    dERAB = res.ERAB_AVG * dfactor_ERAB_cover;
                    dUL_PUSCH_PRB = res.UL_PUSCH_PRB_AVG * dfactor_UL_PUSCH_PRB_cover;
                    dDL_PUSCH_PRB = res.DL_PDSCH_PRB_AVG * dfactor_DL_PUSCH_PRB_cover;
                    dDL_PDCCH_CCE = res.DL_PDCCH_CCE_AVG * dfactor_DL_PDCCH_CCE_cover;
                    dAVG_RRC = res.RRC_AVG * dfactor_AVG_RRC_cover;
                    dUL_FLOW = res.UL_Flow_AVG * dfactor_UL_FLOW_cover;
                    dDL_FLOW = res.DL_Flow_AVG * dfactor_DL_FLOW_cover;
                }

                if (dUL_PUSCH_PRB > 100)
                    dUL_PUSCH_PRB = 100;
                if (dDL_PUSCH_PRB > 100)
                    dDL_PUSCH_PRB = 100;
                if (dDL_PDCCH_CCE > 100)
                    dDL_PDCCH_CCE = 100;

                res.ERAB_EST = dERAB;
                res.RRC_EST = dAVG_RRC;
                res.UL_PUSCH_PRB_EST = dUL_PUSCH_PRB;
                res.DL_PDSCH_PRB_EST = dDL_PUSCH_PRB;
                res.DL_PDCCH_CCE_EST = dDL_PDCCH_CCE;
                res.UL_Flow_EST = dUL_FLOW;
                res.DL_Flow_EST = dDL_FLOW;
                res.CellTyp_EST = Calculate.CheckCellType(dERAB);

                standards = CheckStandard(res.CellTyp_EST, dUL_PUSCH_PRB, dDL_PUSCH_PRB, dDL_PDCCH_CCE, dAVG_RRC, dUL_FLOW, dDL_FLOW);
                return standards;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// 获取增长因子A
        /// </summary>
        /// <param name="sqlconn"></param>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public double GetRateA(string strSdtType, string strType)
        {
            double dFlow_before = 0d;
            double dFlow_after = 0d;
            SqlConnection sqlconn = this.Open();
            if(sqlconn.State != ConnectionState.Open)
                sqlconn.Open();
            SqlCommand oComm = sqlconn.CreateCommand();
            oComm.CommandTimeout = 0;
            try
            {
                oComm.CommandText = "select SUM(上行流量G + 下行流量G) from Before_Date_" + strSdtType + "_" + strType;
                dFlow_before = Convert.ToDouble(oComm.ExecuteScalar());

                oComm.CommandText = "select SUM(上行流量G + 下行流量G) from After_Date_" + strSdtType + "_" + strType;
                dFlow_after = Convert.ToDouble(oComm.ExecuteScalar());

                return dFlow_after / dFlow_before;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                oComm.Dispose();
                if (sqlconn != null)
                    sqlconn.Close();
            }
        }


        /// <summary>
        /// 获取自忙时均值小区表 按总流量从低到高排序
        /// </summary>
        /// <param name="sqlconn"></param>
        /// <param name="strSdtType"></param>
        /// <param name="strType"></param>
        public void GetAvgTable_Before(string strSdtType, string strType, double dRateB, ConcurrentBag<ResObj> lstRes,object trf)
        {
            SqlConnection sqlconn = this.Open();
            int nCount = 0;
            try
            {
                string strTBName_Before = "Before_AvgData_" + strSdtType + "_" + strType;
                string strTBName_After = "After_AvgData_" + strSdtType + "_" + strType;
                string sql = "select ROW_NUMBER() over(order by (c.上行流量G + c.下行流量G) desc) as 'no',cellname," +
                             "(c.上行流量G + c.下行流量G) as '总流量'," +
                             "(select (a.上行流量G + a.下行流量G) / (b.上行流量G + b.下行流量G) " +
                             "from " + strTBName_After + " a," + strTBName_Before + " b " +
                             "where a.cellname = c.cellname and b.cellname = c.cellname and b.上行流量G > 0 and b.下行流量G > 0) * " + dRateB.ToString() + " as 'C'," +
                             "'' as 'type'," +
                             "0 as '单小区X倍后的总流量'," +
                             "0 as 'ERAB建立成功次数增长因子'," +
                             "0 as '上行PUSH_PRB利用率增长因子'," +
                             "0 as '下行PUSH_PRB利用率增长因子'," +
                             "0 as '下行PDCCH_CCE利用率增长因子'," +
                             "0 as '平均有效用户数增长因子'," +
                             "0 as '上行流量增长因子'," +
                             "0 as '下行流量增长因子' " +
                             "from " + strTBName_Before + " c order by no ";
                
                SqlDataAdapter sqlData = new SqlDataAdapter(sql, sqlconn);
                DataSet ds = new DataSet();
                sqlData.Fill(ds, "table1");
                dtAvg_Before = ds.Tables[0];

                int nNo;
                string condition = "";
                MainForm.strCurTip = "正在计算小区流量分类...";
                ((TipReFresher)trf).CurTip();
                foreach (DataRow row in dtAvg_Before.Rows)
                {
                    nNo = Convert.ToInt32(row[0]);
                    //前20%容量类
                    if (nNo > 0 && nNo <= Convert.ToDouble(dtAvg_Before.Rows.Count * 0.2))
                    {
                        row[4] = "容量类";
                    }
                    //20%-70%混合类
                    else if (nNo > Convert.ToDouble(dtAvg_Before.Rows.Count * 0.2) &&
                             nNo <= Convert.ToDouble(dtAvg_Before.Rows.Count * 0.7))
                    {
                        row[4] = "混合类";
                    }
                    //70%-100%覆盖类
                    else if (nNo > Convert.ToDouble(dtAvg_Before.Rows.Count * 0.7) && nNo <= dtAvg_Before.Rows.Count)
                    {
                        row[4] = "覆盖类";
                    }
                }

                lstBag = GetlstTemp(dtAvg_Before);


                //计算增长因子C的平均值
                condition = dtAvg_Before.Columns[4].ColumnName + " = '容量类'";
                dfactor_cap = Convert.ToDouble(dtAvg_Before.Compute("avg(C)", condition));
                
                condition = dtAvg_Before.Columns[4].ColumnName + " = '混合类'";
                dfactor_hybrid = Convert.ToDouble(dtAvg_Before.Compute("avg(C)", condition));
                
                condition = dtAvg_Before.Columns[4].ColumnName + " = '覆盖类'";
                dfactor_cover = Convert.ToDouble(dtAvg_Before.Compute("avg(C)", condition));


                Parallel.ForEach(lstRes, item =>
                {
                    CalcuateData(item, lstBag);
                    Interlocked.Increment(ref nCount);
                    MainForm.nProValue = (int)((double)nCount * 100 / (double)lstRes.Count);
                    MainForm.strCurTip = "正在生成结果，当前第：" + nCount.ToString() + "条，共：" + lstRes.Count + "条...";
                    ((TipReFresher)trf).CurTip();
                });

                //foreach (ResObj item in lstRes)
                //{
                //    CalcuateData(item, lstBag);
                //    MainForm.nProValue = (int)((double)nCount * 100 / (double)lstRes.Count);
                //    MainForm.strCurTip = "正在生成结果，当前第：" + nCount.ToString() + "条，共：" + lstRes.Count + "条...";
                //    ((TipReFresher)trf).CurTip();
                //}

                dfactor_ERAB_cap = 0d;
                dfactor_UL_PUSCH_PRB_cap = 0d;
                dfactor_DL_PUSCH_PRB_cap = 0d;
                dfactor_DL_PDCCH_CCE_cap = 0d;
                dfactor_AVG_RRC_cap = 0d;
                dfactor_UL_FLOW_cap = 0d;
                dfactor_DL_FLOW_cap = 0d;
                dfactor_ERAB_hybrid = 0d;
                dfactor_UL_PUSCH_PRB_hybrid = 0d;
                dfactor_DL_PUSCH_PRB_hybrid = 0d;
                dfactor_DL_PDCCH_CCE_hybrid = 0d;
                dfactor_AVG_RRC_hybrid = 0d;
                dfactor_UL_FLOW_hybrid = 0d;
                dfactor_DL_FLOW_hybrid = 0d;
                dfactor_ERAB_cover = 0d;
                dfactor_UL_PUSCH_PRB_cover = 0d;
                dfactor_DL_PUSCH_PRB_cover = 0d;
                dfactor_DL_PDCCH_CCE_cover = 0d;
                dfactor_AVG_RRC_cover = 0d;
                dfactor_UL_FLOW_cover = 0d;
                dfactor_DL_FLOW_cover = 0d;

                //容量类
                IEnumerable<TempProcess> tp_cap = lstBag.Where(a => a.Type == "容量类");
                foreach (var item in tp_cap)
                {
                    TempProcess tp = (TempProcess) item;
                    dfactor_ERAB_cap += Convert.ToDouble(tp.ERAB);
                    dfactor_UL_PUSCH_PRB_cap += Convert.ToDouble(tp.UL_PUSCH_PRB);
                    dfactor_DL_PUSCH_PRB_cap += Convert.ToDouble(tp.DL_PUSCH_PRB);
                    dfactor_DL_PDCCH_CCE_cap += Convert.ToDouble(tp.DL_PDCCH_CCE);
                    dfactor_AVG_RRC_cap += Convert.ToDouble(tp.AVG_RRC);
                    dfactor_UL_FLOW_cap += Convert.ToDouble(tp.UL_FLOW);
                    dfactor_DL_FLOW_cap += Convert.ToDouble(tp.DL_FLOW);
                }
                dfactor_ERAB_cap = dfactor_ERAB_cap / tp_cap.Count();
                dfactor_UL_PUSCH_PRB_cap = dfactor_UL_PUSCH_PRB_cap / tp_cap.Count();
                dfactor_DL_PUSCH_PRB_cap = dfactor_DL_PUSCH_PRB_cap / tp_cap.Count();
                dfactor_DL_PDCCH_CCE_cap = dfactor_DL_PDCCH_CCE_cap / tp_cap.Count();
                dfactor_AVG_RRC_cap = dfactor_AVG_RRC_cap / tp_cap.Count();
                dfactor_UL_FLOW_cap = dfactor_UL_FLOW_cap / tp_cap.Count();
                dfactor_DL_FLOW_cap = dfactor_DL_FLOW_cap / tp_cap.Count();

                IEnumerable<TempProcess> tp_hybrid = lstBag.Where(a => a.Type == "混合类");
                foreach (var item in tp_hybrid)
                {
                    TempProcess tp = (TempProcess)item;
                    dfactor_ERAB_hybrid += Convert.ToDouble(tp.ERAB);
                    dfactor_UL_PUSCH_PRB_hybrid += Convert.ToDouble(tp.UL_PUSCH_PRB);
                    dfactor_DL_PUSCH_PRB_hybrid += Convert.ToDouble(tp.DL_PUSCH_PRB);
                    dfactor_DL_PDCCH_CCE_hybrid += Convert.ToDouble(tp.DL_PDCCH_CCE);
                    dfactor_AVG_RRC_hybrid += Convert.ToDouble(tp.AVG_RRC);
                    dfactor_UL_FLOW_hybrid += Convert.ToDouble(tp.UL_FLOW);
                    dfactor_DL_FLOW_hybrid += Convert.ToDouble(tp.DL_FLOW);
                }
                dfactor_ERAB_hybrid = dfactor_ERAB_hybrid / tp_hybrid.Count();
                dfactor_UL_PUSCH_PRB_hybrid = dfactor_UL_PUSCH_PRB_hybrid / tp_hybrid.Count();
                dfactor_DL_PUSCH_PRB_hybrid = dfactor_DL_PUSCH_PRB_hybrid / tp_hybrid.Count();
                dfactor_DL_PDCCH_CCE_hybrid = dfactor_DL_PDCCH_CCE_hybrid / tp_hybrid.Count();
                dfactor_AVG_RRC_hybrid = dfactor_AVG_RRC_hybrid / tp_hybrid.Count();
                dfactor_UL_FLOW_hybrid = dfactor_UL_FLOW_hybrid / tp_hybrid.Count();
                dfactor_DL_FLOW_hybrid = dfactor_DL_FLOW_hybrid / tp_hybrid.Count();

                IEnumerable<TempProcess> tp_cover = lstBag.Where(a => a.Type == "覆盖类");
                foreach (var item in tp_cover)
                {
                    TempProcess tp = (TempProcess)item;
                    dfactor_ERAB_cover += Convert.ToDouble(tp.ERAB);
                    dfactor_UL_PUSCH_PRB_cover += Convert.ToDouble(tp.UL_PUSCH_PRB);
                    dfactor_DL_PUSCH_PRB_cover += Convert.ToDouble(tp.DL_PUSCH_PRB);
                    dfactor_DL_PDCCH_CCE_cover += Convert.ToDouble(tp.DL_PDCCH_CCE);
                    dfactor_AVG_RRC_cover += Convert.ToDouble(tp.AVG_RRC);
                    dfactor_UL_FLOW_cover += Convert.ToDouble(tp.UL_FLOW);
                    dfactor_DL_FLOW_cover += Convert.ToDouble(tp.DL_FLOW);
                }
                dfactor_ERAB_cover = dfactor_ERAB_cover / tp_cover.Count();
                dfactor_UL_PUSCH_PRB_cover = dfactor_UL_PUSCH_PRB_cover / tp_cover.Count();
                dfactor_DL_PUSCH_PRB_cover = dfactor_DL_PUSCH_PRB_cover / tp_cover.Count();
                dfactor_DL_PDCCH_CCE_cover = dfactor_DL_PDCCH_CCE_cover / tp_cover.Count();
                dfactor_AVG_RRC_cover = dfactor_AVG_RRC_cover / tp_cover.Count();
                dfactor_UL_FLOW_cover = dfactor_UL_FLOW_cover / tp_cover.Count();
                dfactor_DL_FLOW_cover = dfactor_DL_FLOW_cover / tp_cover.Count();

              
               
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

        private ConcurrentBag<TempProcess> GetlstTemp(DataTable dt)
        {
            ConcurrentBag<TempProcess> lstTemp = new ConcurrentBag<TempProcess>();
            foreach (DataRow row in dt.Rows)
            {
                TempProcess tmp = new TempProcess();
                tmp.No = Convert.ToInt32(row[0]);
                tmp.CellName = Convert.ToString(row[1]);
                tmp.TotalFlow = Convert.ToString(row[2]);
                tmp.RateC = Convert.ToString(row[3]);
                tmp.Type = Convert.ToString(row[4]);
                tmp.XRateFlow = Convert.ToString(row[5]);
                tmp.ERAB = Convert.ToString(row[6]);
                tmp.UL_PUSCH_PRB = Convert.ToString(row[7]);
                tmp.DL_PUSCH_PRB = Convert.ToString(row[8]);
                tmp.DL_PDCCH_CCE = Convert.ToString(row[9]);
                tmp.AVG_RRC = Convert.ToString(row[10]);
                tmp.UL_FLOW = Convert.ToString(row[11]);
                tmp.DL_FLOW = Convert.ToString(row[12]);
                lstTemp.Add(tmp);
            }
            return lstTemp;
        }

        private void CalcuateData(ResObj item, ConcurrentBag<TempProcess> lstTmp)
        {
            try
            {
                IEnumerable<TempProcess> tp_enum = lstTmp.Where(a => a.CellName == item.CellName);
                foreach (var obj in tp_enum)
                {
                    TempProcess tp = (TempProcess)obj;
                    if (string.IsNullOrEmpty(Convert.ToString(tp.RateC)))
                    {
                        return;
                    }
                    else
                    {
                        double dXRate = 0d;
                        if (Convert.ToString(tp.Type) == "容量类")
                            dXRate = Convert.ToDouble(tp.TotalFlow) * dfactor_cap;
                        if (Convert.ToString(tp.Type) == "混合类")
                            dXRate = Convert.ToDouble(tp.TotalFlow) * dfactor_hybrid;
                        if (Convert.ToString((tp.Type)) == "覆盖类")
                            dXRate = Convert.ToDouble(tp.TotalFlow) * dfactor_cover;


                        //行赋值
                        tp.XRateFlow = dXRate.ToString();
                        decimal[] dValues = new decimal[7];
                        dValues = GetValues(item, dXRate);
                        tp.ERAB = dValues[0].ToString();
                        tp.UL_PUSCH_PRB = dValues[1].ToString();
                        tp.DL_PUSCH_PRB = dValues[2].ToString();
                        tp.DL_PDCCH_CCE = dValues[3].ToString();
                        tp.AVG_RRC = dValues[4].ToString();
                        tp.UL_FLOW = dValues[5].ToString();
                        tp.DL_FLOW = dValues[6].ToString();

                    }
                    break;
                }

           
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                
            }
        }

        private decimal[] GetValues(ResObj item, double dXRate)
        {
            double[] dTempValues = new double[7];
            decimal[] dValues = new decimal[] { 0M, 0M, 0M, 0M, 0M, 0M, 0M };
            SqlConnection conn = this.Open();
            if(conn.State != ConnectionState.Open)
                conn.Open();
            SqlCommand oComm = conn.CreateCommand();
            SqlDataReader dr = null;
            oComm.CommandTimeout = 0;
            try
            {
                DateTime FindBig = DateTime.Now;
                oComm.CommandText = "select top 1 " +
                                    "E_RAB建立成功数," +
                                    "上行PUSCH_PRB利用率," +
                                    "下行PDSCH_PRB利用率," +
                                    "下行PDCCH_CCE利用率," +
                                    "有效RRC连接平均数," +
                                    "小区用户面上行字节数," +
                                    "小区用户面下行字节数 " +
                                    "from Tempdata_Before where 总流量 = (select min(总流量) " +
                                    "from Tempdata_Before where " + dXRate.ToString() + " <= 总流量/(1024.0*1024.0)) ";
                dr = oComm.ExecuteReader();
                if (dr.Read() && dr.HasRows)
                {
                    
                    for (int i = 0; i < dTempValues.Length; i++)
                    {
                        dTempValues[i] = Convert.ToDouble(dr[i]);
                    }
                    Console.WriteLine("找最大耗时：" + DateTime.Now.Subtract(FindBig).TotalSeconds.ToString());
                }
                else
                {
                    DateTime FindSmall = DateTime.Now;
                    dr.Close();
                    oComm.CommandText = "select top 1 " +
                                        "E_RAB建立成功数," +
                                        "上行PUSCH_PRB利用率," +
                                        "下行PDSCH_PRB利用率," +
                                        "下行PDCCH_CCE利用率," +
                                        "有效RRC连接平均数," +
                                        "小区用户面上行字节数," +
                                        "小区用户面下行字节数 " +
                                        "from Tempdata_Before where 总流量 = (select max(总流量) " +
                                        "from Tempdata_Before where " + dXRate.ToString() + " >= 总流量/(1024.0*1024.0)) ";
                    dr = oComm.ExecuteReader();
                    if (dr.Read() && dr.HasRows)
                    {
                        for (int i = 0; i < dTempValues.Length; i++)
                        {
                            dTempValues[i] = Convert.ToDouble(dr[i]);
                        }
                        Console.WriteLine("找最小耗时：" + DateTime.Now.Subtract(FindSmall).TotalSeconds.ToString());
                    }
                }
                dr.Close();
                DateTime JS = DateTime.Now;
                oComm.CommandText = "select " +
                                    "Convert(decimal(18,4)," + dTempValues[0].ToString() + " / ERAB)," +
                                    "Convert(decimal(18,4)," + dTempValues[1].ToString() + " / 上行PUSCH_PRB利用率)," +
                                    "Convert(decimal(18,4)," + dTempValues[2].ToString() + " / 下行PDSCH_PRB利用率)," +
                                    "Convert(decimal(18,4)," + dTempValues[3].ToString() + " / 下行PDCCH_CCE利用率)," +
                                    "Convert(decimal(18,4)," + dTempValues[4].ToString() + " / 平均RRC有效用户数)," +
                                    "Convert(decimal(18,4)," + (dTempValues[5] / 1024 / 1024).ToString() + " / 上行流量G)," +
                                    "Convert(decimal(18,4)," + (dTempValues[6] / 1024 / 1024).ToString() + " / 下行流量G) " +
                                    "from Before_AvgData_" + MainForm.strStdTyp + "_" + MainForm.strType +
                                    " where ERAB > 0 and 上行PUSCH_PRB利用率 > 0 and 下行PDSCH_PRB利用率 > 0 " +
                                    "and 下行PDCCH_CCE利用率 > 0 and 平均RRC有效用户数 > 0 " +
                                    "and 上行流量G > 0 and 下行流量G > 0 " +
                                    "and cellname = '" + item.CellName + "'";
                dr = oComm.ExecuteReader();
                if (dr.Read() && dr.HasRows)
                {
                    for (int i = 0; i < dValues.Length; i++)
                    {
                        dValues[i] = Convert.ToDecimal(dr[i]);
                    }
                    Console.WriteLine("计算耗时：" + DateTime.Now.Subtract(JS).TotalSeconds.ToString());
                }
                return dValues;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                dr.Close();
                if(conn != null)
                    conn.Close();
            }
        }

        

        /// <summary>
        /// 获取场景类型
        /// </summary>
        /// <param name="strCellName"></param>
        /// <returns></returns>
        public void GetType(ResObj res)
        {
            try
            {
                string condition = dtAvg_Before.Columns[1].ColumnName + " = '" + res.CellName + "'";
                DataRow[] drs = dtAvg_Before.Select(condition);
                if (drs.Length > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(drs[0][3])))
                    {
                        res.Remarks = "无流量";
                    }
                    else
                    {
                        res.RateC = Convert.ToDouble(drs[0][3]);
                        res.CellTyp_AVG = Convert.ToString(drs[0][4]);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 获取单小区增量X倍后的总流量
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private double GetXRateFlow(ResObj res)
        {
            try
            {
                double dXRate = 0d;
                IEnumerable<TempProcess> tp_enum = lstBag.Where(a => a.CellName == res.CellName);
                foreach (var item in tp_enum)
                {
                    TempProcess tp = (TempProcess)item;
                    dXRate = Convert.ToDouble(tp.XRateFlow);
                    break;
                }
                return dXRate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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
        private string[] CheckStandard(string strCellTyp, double dUL_PUSCH_PRB, double dDL_PUSCH_PRB, double dDL_PDCCH_CCE, double dAVG_RRC, double dUL_FLOW, double dDL_FLOW)
        {
            string[] strStandard = new string[2];
            if (strCellTyp == "大包小区")
            {
                //上行标准
                if (CheckStandard(MainForm.Param.lstBu, dUL_PUSCH_PRB, dDL_PUSCH_PRB, dDL_PDCCH_CCE, dAVG_RRC, dUL_FLOW, dDL_FLOW))
                    strStandard[0] = "满足";
                else
                    strStandard[0] = "不满足";

                //下行标准
                if (CheckStandard(MainForm.Param.lstBd, dUL_PUSCH_PRB, dDL_PUSCH_PRB, dDL_PDCCH_CCE, dAVG_RRC, dUL_FLOW, dDL_FLOW))
                    strStandard[1] = "满足";
                else
                    strStandard[1] = "不满足";
            }
            if (strCellTyp == "中包小区")
            {
                //上行标准
                if (CheckStandard(MainForm.Param.lstMu, dUL_PUSCH_PRB, dDL_PUSCH_PRB, dDL_PDCCH_CCE, dAVG_RRC, dUL_FLOW, dDL_FLOW))
                    strStandard[0] = "满足";
                else
                    strStandard[0] = "不满足";

                //下行标准
                if (CheckStandard(MainForm.Param.lstMd, dUL_PUSCH_PRB, dDL_PUSCH_PRB, dDL_PDCCH_CCE, dAVG_RRC, dUL_FLOW, dDL_FLOW))
                    strStandard[1] = "满足";
                else
                    strStandard[1] = "不满足";
            }
            if (strCellTyp == "小包小区")
            {
                //上行标准
                if (CheckStandard(MainForm.Param.lstSu, dUL_PUSCH_PRB, dDL_PUSCH_PRB, dDL_PDCCH_CCE, dAVG_RRC, dUL_FLOW, dDL_FLOW))
                    strStandard[0] = "满足";
                else
                    strStandard[0] = "不满足";

                //下行标准
                if (CheckStandard(MainForm.Param.lstSd, dUL_PUSCH_PRB, dDL_PUSCH_PRB, dDL_PDCCH_CCE, dAVG_RRC, dUL_FLOW, dDL_FLOW))
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
        private bool CheckStandard(ListBox.ObjectCollection Items, double dUL_PUSCH_PRB, double dDL_PUSCH_PRB, double dDL_PDCCH_CCE, double dAVG_RRC, double dUL_FLOW, double dDL_FLOW)
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
                    strTemp = strTemp.Replace("平均RRC有效用户数", Convert.ToString(dAVG_RRC));

                if (strTemp.Contains("上行PUSCH PRB利用率"))
                    strTemp = strTemp.Replace("上行PUSCH PRB利用率", Convert.ToString(dUL_PUSCH_PRB));

                if (strTemp.Contains("上行流量(G)"))
                    strTemp = strTemp.Replace("上行流量(G)", Convert.ToString(Convert.ToDouble(dUL_FLOW)));

                if (strTemp.Contains("下行PDSCH PRB利用率"))
                    strTemp = strTemp.Replace("下行PDSCH PRB利用率", Convert.ToString(dDL_PUSCH_PRB));

                if (strTemp.Contains("下行PDCCH CCE利用率"))
                    strTemp = strTemp.Replace("下行PDCCH CCE利用率", Convert.ToString(dDL_PDCCH_CCE));

                if (strTemp.Contains("下行流量(G)"))
                    strTemp = strTemp.Replace("下行流量(G)", Convert.ToString(Convert.ToDouble(dDL_FLOW)));

                strExp += strTemp + " and ";
            }

            strExp = strExp.Substring(0, strExp.Length - 5);

            return (bool)dt.Compute(strExp, "");
        }

    }
}
