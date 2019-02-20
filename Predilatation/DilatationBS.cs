using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Predilatation
{
    public class DilatationBS:SqlServerHelper
    {
        //网元标识
        private List<string> lstnetmark { set; get; }
        private List<string> lstcellname { set; get; }


        private DilatationForm dilatation = null;
        private object trf = null;
        public DilatationBS(DilatationForm dilform,object objtrf)
        {
            dilatation = dilform;
            trf = objtrf;
            lstnetmark = new List<string>();
            lstcellname = new List<string>();
        }
        /// <summary>
        /// 导入扩容小区数据
        /// </summary>
        public void ImportEstCells()
        {

            string strnewtablename = Path.GetFileNameWithoutExtension(dilatation.strEstCellsPath);

            dilatation.strCurTip = "正在导入" + strnewtablename + "表，请稍后...";
            dilatation.nProValue = 50;
           
            ((TipReFresher)trf).CurTip();

            //扩容小区路径为空则跟据已有预估结果执行
            if (string.IsNullOrEmpty(dilatation.strEstCellsPath))
            {
                strnewtablename = "扩容小区清单";
                string stroldtablename = "Res_" + dilatation.strEstTyp + "_" + dilatation.strRuleTyp + "_" + dilatation.strStdTyp;

                //判断老表是否存在
                if (!dilatation.sqlhelper.CheckTableExist(stroldtablename))
                {
                    dilatation.strerrmsg = "原始数据表：" + stroldtablename + "不存在！";
                    return;
                }

                //检查表如果存在就删除
                dilatation.sqlhelper.ChkTableAndDel(strnewtablename);
                //从原表copy数据到新表
                dilatation.sqlhelper.CopyDataFromTable(stroldtablename, strnewtablename);
            }
            else
            {
                if (strnewtablename != "扩容小区清单")
                {
                    dilatation.strerrmsg = "导入扩容小区的文件名必须为‘扩容小区清单’！";
                    return;
                }
                //导入扩容小区到数据库
                DataTable dt = Excel_CSVHelper.ConvertExceltoDt(dilatation.strEstCellsPath);

                if (dt.Rows.Count == 0)
                {
                    dilatation.strerrmsg = "扩容小区数据不能为空！";
                    return;
                }

                //根据DataTable创建新表
                dilatation.sqlhelper.CreatTableByDataTable(dt, strnewtablename);
                //导入数据到新表
                dilatation.sqlhelper.BulkToDB(dt, strnewtablename);
            }

            dilatation.nProValue = 100;
            ((TipReFresher)trf).CurTip();
        }

        /// <summary>
        /// 导入数据源文件
        /// </summary>
        public void ImportResource()
        {
            //获取数据源文件
            string[] strresourcefiles = Directory.GetFiles(dilatation.strDataResourcePath);
            string strnewtablename = "";
            int ncount = 0;
            dilatation.nProValue = 0;
            foreach (string file in strresourcefiles)
            {
                strnewtablename = Path.GetFileNameWithoutExtension(file);
                dilatation.strCurTip = "正在导入" + strnewtablename + "表，请稍后...";
                ((TipReFresher)trf).CurTip();

                DataTable dt = new DataTable();
                if (Path.GetExtension(file) == ".xls" || Path.GetExtension(file) == ".xlsx")
                {
                    dt = Excel_CSVHelper.ConvertExceltoDt(file);

                }
                else if (Path.GetExtension(file) == ".CSV")
                {
                    dt = Excel_CSVHelper.CovertCSVtoDt(file);

                }
                else if (Path.GetExtension(file) == ".csv")
                {
                    dilatation.strerrmsg = "导入" + strnewtablename + ".csv出现错误！暂不支持.csv格式的文件导入！";
                    return;
                }
                else
                {
                    continue;
                }

                //根据DataTable创建新表
                dilatation.sqlhelper.CreatTableByDataTable(dt, strnewtablename);

                //导入数据到新表
                dilatation.sqlhelper.BulkToDB(dt, strnewtablename);

                ncount++;
                dilatation.nProValue = (ncount * 100) / strresourcefiles.Length;
                ((TipReFresher)trf).CurTip();
            }
        }

        public void CreateResTable()
        {
            dilatation.strCurTip = "正在创建扩容结果表，请稍后...";
            ((TipReFresher)trf).CurTip();
            dilatation.sqlhelper.CreateResTable("扩容结果");
        }

        /// <summary>
        /// 根据选择类型返回表名前缀
        /// </summary>
        /// <param name="strtype"></param>
        /// <returns></returns>
        public string GetResType(string strtype)
        {
            if (strtype == "日常")
                return "Daily";
            if (strtype == "节假日")
                return "Holiday";
            if (strtype == "给定增量")
                return "Given";
            if (strtype == "流量自忙时")
                return "FlowBusy";
            if (strtype == "利用率自忙时")
                return "Utilizaerate";
            if (strtype == "集团高负荷（2018上）")
                return "Grp2018Fir";
            if (strtype == "集团高负荷（2018下）")
                return "Grp2018Sec";
            if (strtype == "我司高负荷")
                return "DTMobile";
            return "";
        }

        /// <summary>
        /// 更新结果表信息
        /// </summary>
        public void UpdateResTable()
        {
            dilatation.nProValue = 0;
            ((TipReFresher)trf).CurTip();

            //设置网元标识
            dilatation.strCurTip = "正在设置网元标识......";
            ((TipReFresher)trf).CurTip();
            SetNetFriendName();
            dilatation.nProValue = 6;
            ((TipReFresher)trf).CurTip();
            
            
            //设置SectorID
            dilatation.strCurTip = "正在设置SectorID......";
            ((TipReFresher)trf).CurTip();
            SetSectorID();
            //删除多余信息
            DelSameSectorID();
            dilatation.nProValue = 12;
            ((TipReFresher)trf).CurTip();
            

            //计算站点配置小区数
            dilatation.strCurTip = "正在计算站点配置小区数......";
            ((TipReFresher)trf).CurTip();
            SetENBConfCellCount();
            dilatation.nProValue = 18;
            ((TipReFresher)trf).CurTip();
            

            //计算基站需扩容小区数
            dilatation.strCurTip = "正在计算基站需扩容小区数......";
            ((TipReFresher)trf).CurTip();
            SetENBNeedEstCellCount();
            dilatation.nProValue = 24;
            ((TipReFresher)trf).CurTip();
            

            //获取主控板类型
            dilatation.strCurTip = "正在获取主控板类型......";
            ((TipReFresher)trf).CurTip();
            SetMainBordType();
            dilatation.nProValue = 30;
            ((TipReFresher)trf).CurTip();

            //计算基带支持小区数
            dilatation.strCurTip = "正在计算基带支持小区数......";
            ((TipReFresher)trf).CurTip();
            SetBaseBordSupCells();
            dilatation.nProValue = 36;
            ((TipReFresher)trf).CurTip();

            //计算基带槽位余量
            dilatation.strCurTip = "正在计算基带槽位余量......";
            dilatation.nProValue = 42;
            ((TipReFresher)trf).CurTip();
            SetBaseSlotSurplus();
            ((TipReFresher)trf).CurTip();

            //计算RRU支持小区数
            dilatation.strCurTip = "正在计算RRU支持小区数......";
            ((TipReFresher)trf).CurTip();
            SetRRUSupCellsCount();
            dilatation.nProValue = 48;
            ((TipReFresher)trf).CurTip();
           

            //计算实际可扩小区数
            dilatation.strCurTip = "正在计算实际可扩小区数......";
            ((TipReFresher)trf).CurTip();
            SetInFactESTCellNum();
            dilatation.nProValue = 54;
            ((TipReFresher)trf).CurTip();

            //计算当前Sector配置小区数
            dilatation.strCurTip = "正在计算当前Sector配置小区数......";
            ((TipReFresher)trf).CurTip();
            SetCurSectorCells();
            dilatation.nProValue = 60;
            ((TipReFresher)trf).CurTip();

            //获取RRU类型
            dilatation.strCurTip = "正在获取当前小区Sector频段的RRU类型......";
            ((TipReFresher)trf).CurTip();
            SetRRUType();
            dilatation.nProValue = 66;
            ((TipReFresher)trf).CurTip();

            //计算当前SectoreRRU支持小区数
            dilatation.strCurTip = "正在计算当前Sectore_RRU支持小区数......";
            ((TipReFresher)trf).CurTip();
            SetRRUSuptCells();
            dilatation.nProValue = 72;
            ((TipReFresher)trf).CurTip();

            //计算当前小区Sector频段配置
            dilatation.strCurTip = "正在获取当前小区Sector频段配置......";
            ((TipReFresher)trf).CurTip();
            SetSectorType();
            dilatation.nProValue = 78;
            ((TipReFresher)trf).CurTip();

            //计算当前小区Sector频段配置
            dilatation.strCurTip = "正在计算扩容方式......";
            ((TipReFresher)trf).CurTip();
            SetESTType();
            dilatation.nProValue = 84;
            ((TipReFresher)trf).CurTip();

            //计算共站信息
            dilatation.strCurTip = "正在计算共覆盖站点......";
            ((TipReFresher)trf).CurTip();
            SetCommonCoverInfo();
            dilatation.nProValue = 100;
            ((TipReFresher)trf).CurTip();
           
            
        }

       

        /// <summary>
        /// 添加网元信息
        /// </summary>
        /// <returns></returns>
        public void SetNetFriendName()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                SqlCommand oComm = this.oConn.CreateCommand();
                oComm.CommandText = "select cellname,网元标识 from 扩容结果";
                SqlDataReader dr = oComm.ExecuteReader();
                List<string> lstupdsql = new List<string>();
                
                while (dr.Read())
                {
                    string strfriendname = dr[0].ToString().Substring(0, dr[0].ToString().IndexOf("("));
                    string[] netinfo = dr[0].ToString().Split(',');
                    string strnetmark = netinfo[2].Substring(netinfo[2].IndexOf('=') + 1);
                    lstupdsql.Add("update 扩容结果 set 网元友好名 = '" + strfriendname + "',网元标识 = " + strnetmark + " where cellname = '" + dr[0].ToString() +"'");                  
                }
                dr.Close();

                foreach (string updsql in lstupdsql)
                {
                    oComm.CommandText = updsql;
                    oComm.ExecuteNonQuery();
                }
                oComm.Dispose();
                this.oConn.Close();


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetNetFriendName");
                throw;
            }
        }

        /// <summary>
        /// 设置SectorID
        /// </summary>
        public void SetSectorID()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                SqlCommand oComm = this.oConn.CreateCommand();
                oComm.CommandText = "select cellname from 扩容结果";
                SqlDataReader dr = oComm.ExecuteReader();
                List<string> lstupdsql = new List<string>();

                while (dr.Read())
                {
                    lstupdsql.Add("update 扩容结果 set 扩容SectorID = cast((select SectorID from LTE工参信息 where 对象标识 = '" + dr[0].ToString() + "') as int) where cellname = '" + dr[0].ToString() + "'");
                }
                dr.Close();

                foreach (var updsql in lstupdsql)
                {
                    oComm.CommandText = updsql;
                    oComm.ExecuteNonQuery();
                }
               
                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetSectorID");
                throw;
            }
        }

        /// <summary>
        /// 相同的网元，相同的SectorID只保留一个，其余的删掉
        /// </summary>
        public void DelSameSectorID()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                SqlCommand oComm = this.oConn.CreateCommand();
                oComm.CommandText = "SELECT 网元标识,扩容SectorID FROM 扩容结果 GROUP BY 网元标识,扩容SectorID having count(str(网元标识) + str(扩容SectorID)) >= 1";
                SqlDataReader dr = oComm.ExecuteReader();
                List<int[]> netmarklist = new List<int[]>();
                string strcellname = "";
                string strnetmark = "";
                while (dr.Read())
                {
                    int[] netmark = new int[2];
                    netmark[0] = Convert.ToInt32(dr[0].ToString());
                    netmark[1] = Convert.ToInt32(dr[1].ToString());
                    netmarklist.Add(netmark);
                }
                dr.Close();

                string strcondition = "";
                //找到唯一网元标识+扩容SectorID小区
                foreach (var mark in netmarklist)
                {
                    oComm.CommandText = "SELECT top 1 cellname FROM 扩容结果 where 网元标识 = " + mark[0] + " and 扩容SectorID = " + mark[1];
                    strcellname = Convert.ToString(oComm.ExecuteScalar());
                    string[] netinfo = strcellname.Split(',');
                    strnetmark = netinfo[2].Substring(netinfo[2].IndexOf('=') + 1);
                    lstcellname.Add(strcellname);
                    lstnetmark.Add(strnetmark);
                    strcondition += "'" + strcellname + "',";
                }

                if (!string.IsNullOrEmpty(strcondition))
                {
                    strcondition = strcondition.Substring(0, strcondition.Length - 1);
                    //删除重复小区
                    oComm.CommandText = "delete from 扩容结果 where cellname not in (" + strcondition + ")";
                    oComm.ExecuteNonQuery();
                }
                

              

                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "DelSameSectorID");
                throw;
            }
        }

        /// <summary>
        /// 计算站点配置小区数
        /// </summary>
        private void SetENBConfCellCount()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");

                for (int i=0;i<lstnetmark.Count;i++)
                {
                    //到TD_LTE小区计算基站需扩容小区数
                    oComm.CommandText = "select count(*) from TD_LTE小区 where 网元标识 = '" + lstnetmark[i] + "'";
                    int nnetmarkcount = Convert.ToInt32(oComm.ExecuteScalar());
                    //修改结果表
                    oComm.CommandText = "update 扩容结果 set 站点配置小区数 = " + nnetmarkcount + " where cellname ='" + lstcellname[i] + "'";
                    oComm.ExecuteNonQuery();
                }

                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetENBConfCellCount");
                throw;
            }
        }

        /// <summary>
        /// 计算基站需扩容小区数
        /// </summary>
        private void SetENBNeedEstCellCount()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");

                for (int i = 0; i < lstnetmark.Count; i++)
                {
                    //到TD_LTE小区计算基站需扩容小区数
                    oComm.CommandText = "select count(*) from 扩容结果 where 网元标识 = '" + lstnetmark[i] + "'";
                    int nnetmarkcount = Convert.ToInt32(oComm.ExecuteScalar());
                    //修改结果表
                    oComm.CommandText = "update 扩容结果 set 基站需扩容小区数 = " + nnetmarkcount + " where cellname ='" + lstcellname[i] + "'";
                    oComm.ExecuteNonQuery();
                }

                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetENBNeedEstCellCount");
                throw;
            }
        }

        /// <summary>
        /// 获取主控板类型，和最大支持小区数SCTE支持9小区，SCTF支持12小区
        /// </summary>
        private void SetMainBordType()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");

                for (int i = 0; i < lstnetmark.Count; i++)
                {
                    //到TD_LTE小区计算基站需扩容小区数
                    oComm.CommandText = "select 板类型 from 板卡规划 where 网元标识 = '" + lstnetmark[i] + "' and (插槽号 = '1.0' or 插槽号 = '1')";
                    string strmainbordtype = Convert.ToString(oComm.ExecuteScalar());
                    
                    //修改结果表
                    if (strmainbordtype.ToUpper().Contains("SCTF"))
                        oComm.CommandText = "update 扩容结果 set 主控板类型 = '" + strmainbordtype + "',主控板支持小区数 = 12 where cellname ='" + lstcellname[i] + "'";
                    
                    if (strmainbordtype.ToUpper().Contains("SCTE"))
                        oComm.CommandText = "update 扩容结果 set 主控板类型 = '" + strmainbordtype + "',主控板支持小区数 = 9 where cellname ='" + lstcellname[i] + "'";
                    oComm.ExecuteNonQuery();
                }

                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetMainBordType");
                throw;
            }
        }


        /// <summary>
        /// 计算基带板支持小区数
        /// </summary>
        private void SetBaseBordSupCells()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");

                for (int i = 0; i < lstnetmark.Count; i++)
                {
                    //计算不同基带板个数
                    oComm.CommandText = "select count(*) from 板卡规划 where 网元标识 = '" + lstnetmark[i] + "' and 板类型 like '%BPOI%'";
                    int nbpoicount = Convert.ToInt32(oComm.ExecuteScalar());

                    oComm.CommandText = "select count(*) from 板卡规划 where 网元标识 = '" + lstnetmark[i] + "' and 板类型 like '%BPOG%'";
                    int nbpogcount = Convert.ToInt32(oComm.ExecuteScalar());

                    oComm.CommandText = "select count(*) from 板卡规划 where 网元标识 = '" + lstnetmark[i] + "' and 板类型 like '%BPOH%'";
                    int nbpohcount = Convert.ToInt32(oComm.ExecuteScalar());

                    //基带板支持小区个数
                    int ntotalcount = nbpoicount * 6 + nbpogcount * 3 + nbpohcount * 3;

                    //修改结果表
                    oComm.CommandText = "update 扩容结果 set 基带支持小区数 = " + ntotalcount + " where cellname ='" + lstcellname[i] + "'";
                    oComm.ExecuteNonQuery();
                }

                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetBaseBordSupCells");
                throw;
            }
        }

        /// <summary>
        /// 计算基带槽位余量
        /// </summary>
        private void SetBaseSlotSurplus()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                int ncountsurplus = 0;
                for (int i = 0; i < lstnetmark.Count; i++)
                {
                    ncountsurplus = 0;
                    //查看4、5、6、7槽位的占用情况，统计未占用数量
                    for (int j = 4; j < 8; j++)
                    {
                        //计算基带槽位余量
                        oComm.CommandText = "select count(*) from 板卡规划 where 网元标识 = '" + lstnetmark[i] + "' and 插槽号 = '" + j.ToString() +".0'";
                        if (Convert.ToInt32(oComm.ExecuteScalar()) == 0)
                            ncountsurplus++;
                    }
                    //修改结果表
                    oComm.CommandText = "update 扩容结果 set 基带槽位余量 = " + ncountsurplus + " where cellname ='" + lstcellname[i] + "'";
                    oComm.ExecuteNonQuery();
                }

                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetCommonCoverInfo");
                throw;
            }
        }

        /// <summary>
        /// 计算RRU支持小区数
        /// </summary>
        private void SetRRUSupCellsCount()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                int nsupcellscount = 0;
                List<string> lstrfcellname = new List<string>();
                for (int i = 0; i < lstnetmark.Count; i++)
                {
                    nsupcellscount = 0;
                    lstrfcellname.Clear();
                    //查找射频单元名称
                    oComm.CommandText = "select 射频单元类型名称 from 射频单元拓扑 where 网元标识='" + lstnetmark[i] + "'";
                    SqlDataReader dr = oComm.ExecuteReader();
                    while (dr.Read())
                    {
                        lstrfcellname.Add(dr[0].ToString());
                    }
                    dr.Close();

                    //根据RRU频段及支持能力表累加每个RRU类型的支持小区数
                    foreach (var rfcellname in lstrfcellname)
                    {
                        oComm.CommandText = "select 支持小区数 from RRU频段及支持能力 where 射频单元类型名称 = '" + rfcellname + "'";
                        nsupcellscount += Convert.ToInt32(oComm.ExecuteScalar());
                    }
                    //修改结果表
                    oComm.CommandText = "update 扩容结果 set RRU支持小区数 = " + nsupcellscount + " where cellname ='" + lstcellname[i] + "'";
                    oComm.ExecuteNonQuery();
                }

                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetRRUSupCellsCount");
                throw;
            }
        }

        /// <summary>
        /// 计算实际可扩小区数
        /// </summary>
        private void SetInFactESTCellNum()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                for (int i = 0; i < lstcellname.Count; i++)
                {
                    oComm.CommandText = "select 主控板支持小区数 from 扩容结果 where cellname = '" + lstcellname[i] + "'";
                    if (string.IsNullOrEmpty(Convert.ToString(oComm.ExecuteScalar())))
                        continue;
                    int nvalue1 = Convert.ToInt32(oComm.ExecuteScalar());
                    oComm.CommandText = "select RRU支持小区数 from 扩容结果 where cellname = '" + lstcellname[i] + "'";
                    if (string.IsNullOrEmpty(Convert.ToString(oComm.ExecuteScalar())))
                        continue;
                    int nvalue2 = Convert.ToInt32(oComm.ExecuteScalar());
                    oComm.CommandText = "select 基带支持小区数 from 扩容结果 where cellname = '" + lstcellname[i] + "'";
                    if (string.IsNullOrEmpty(Convert.ToString(oComm.ExecuteScalar())))
                        continue;
                    int nvalue3 = Convert.ToInt32(oComm.ExecuteScalar());

                    int nmin = GetMin(nvalue1, nvalue2, nvalue3);
                    oComm.CommandText = "select 站点配置小区数 from 扩容结果 where cellname = '" + lstcellname[i] + "'";
                    if (string.IsNullOrEmpty(Convert.ToString(oComm.ExecuteScalar())))
                        continue;
                    int nconfcellcount = Convert.ToInt32(oComm.ExecuteScalar());
                    int ninfacttstnum = nmin - nconfcellcount;

                    oComm.CommandText = "update 扩容结果 set 实际可扩小区数 = " + ninfacttstnum + " where cellname = '" + lstcellname[i] + "'";
                    oComm.ExecuteNonQuery();
                }

                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetInFactESTCellNum");
                throw;
            }
        }

        /// <summary>
        /// 三个数取最小
        /// </summary>
        /// <param name="nvalue1"></param>
        /// <param name="nvalue2"></param>
        /// <param name="nvalue3"></param>
        /// <returns></returns>
        private int GetMin(int nvalue1,int nvalue2,int nvalue3)
        {
            int nmin = Math.Min(nvalue1, nvalue2);
            if (nmin <= nvalue3)
            {
                return nmin;
            }
            else
            {
                return nvalue3;
            }
        }

        /// <summary>
        /// 当前Sector配置小区数
        /// </summary>
        private void SetCurSectorCells()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                int ncursectorcellnum = 0;
                for (int i = 0; i < lstcellname.Count; i++)
                {
                    oComm.CommandText = "select 网元标识,扩容SectorID from 扩容结果 where cellname = '" + lstcellname[i] + "'";
                    SqlDataReader dr = oComm.ExecuteReader();
                    while (dr.Read())
                    {
                        if (!string.IsNullOrEmpty(dr[1].ToString()))
                            oComm.CommandText = "select count(*) from LTE工参信息 where eNodeBID='" + dr[0].ToString() +
                                                "' and SectorID = '" + dr[1].ToString() + "'";
                        else
                            oComm.CommandText = "";
                    }
                    dr.Close();

                    if (!string.IsNullOrEmpty(oComm.CommandText))
                    {
                        ncursectorcellnum = Convert.ToInt32(oComm.ExecuteScalar());
                        oComm.CommandText = "update 扩容结果 set 当前Sector配置小区数 = " + ncursectorcellnum + " where cellname = '" + lstcellname[i] + "'";
                        oComm.ExecuteNonQuery();
                    }
                }

                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetCurSectorCells");
                throw;
            }
        }

        /// <summary>
        /// 配置RRU类型
        /// </summary>
        private void SetRRUType()
        {
            try
            {
                List<string[]> lstcellinfo = GetRRUType();
                foreach (var cellinfo in lstcellinfo)
                {
                    //排除找不到RRU型号的情况
                    if (!string.IsNullOrEmpty(cellinfo[5]))
                    {
                        //是否包含多款RRU的情况
                        if (cellinfo[5].Contains(","))
                        {
                            string[] strsrruname = cellinfo[5].Split(',');
                            UpdateRRUInfo(cellinfo[0], strsrruname[0]);
                        }
                        else
                        {
                            UpdateRRUInfo(cellinfo[0], cellinfo[5]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetRRUType");
                throw;
            }
        }


        /// <summary>
        /// 计算当前RRU支持小区数
        /// </summary>
        private void SetRRUSuptCells()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                string strRRU_F = "";
                string strRRU_D = "";
                string strRRU_E = "";
                foreach (var cellname in lstcellname)
                {
                    oComm.CommandText = "select 当前Sector的F频段RRU型号,当前Sector的D频段RRU型号,当前Sector的E频段RRU型号 from 扩容结果 where cellname = '" + cellname + "'";
                    SqlDataReader dr = oComm.ExecuteReader();
                    if (dr.Read())
                    {
                        strRRU_F = Convert.ToString(dr[0]);
                        strRRU_D = Convert.ToString(dr[1]);
                        strRRU_E = Convert.ToString(dr[2]);
                    }
                    
                    dr.Close();
                    int nsuptrrunum = 0;
                    if (!string.IsNullOrEmpty(strRRU_F))
                    {
                        oComm.CommandText = "select 支持小区数 from RRU频段及支持能力 where 射频单元类型名称 = '" + strRRU_F + "'";
                        nsuptrrunum += Convert.ToInt32(oComm.ExecuteScalar());
                    }
                    if (!string.IsNullOrEmpty(strRRU_D))
                    {
                        oComm.CommandText = "select 支持小区数 from RRU频段及支持能力 where 射频单元类型名称 = '" + strRRU_D + "'";
                        nsuptrrunum += Convert.ToInt32(oComm.ExecuteScalar());
                    }
                    if (!string.IsNullOrEmpty(strRRU_E))
                    {
                        oComm.CommandText = "select 支持小区数 from RRU频段及支持能力 where 射频单元类型名称 = '" + strRRU_E + "'";
                        nsuptrrunum += Convert.ToInt32(oComm.ExecuteScalar());
                    }

                    oComm.CommandText = "update 扩容结果 set 当前Sector_RRU支持小区数 = " + nsuptrrunum + " where cellname = '" + cellname + "'";
                    oComm.ExecuteNonQuery();
                }

                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetRRUSuptCells");
                throw;
            }
        }
        /// <summary>
        /// 获取RRU信息
        /// </summary>
        /// <param name="strcellname"></param>
        /// <param name="strrruname"></param>
        private void UpdateRRUInfo(string strcellname,string strrruname)
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                string strrrufreq = "";
                string strrrusupfreq = "";
                string strrrusupcells = "";
                string freq_a = "";
                string freq_e = "";
                string freq_f = "";
                oComm.CommandText = "select RRU频段,RRU支持频率,支持小区数 from RRU频段及支持能力 where 射频单元类型名称 = '" + strrruname + "'";
                SqlDataReader dr = oComm.ExecuteReader();
                if (dr.Read())
                {
                    strrrufreq = dr[0].ToString();
                    strrrusupfreq = dr[1].ToString();
                    strrrusupcells = dr[2].ToString();
                    dr.Close();
                    if (strrrufreq.Contains(","))
                    {
                        if (strrrusupfreq.Contains(","))
                        {
                            string[] rrusupfreq = strrrusupfreq.Split(',');
                            foreach (var supfreq in rrusupfreq)
                            {
                                if (supfreq.Contains("F"))
                                {
                                    freq_f += supfreq + ",";
                                }
                                if (supfreq.Contains("A"))
                                {
                                    freq_a += supfreq + ",";
                                }
                                if (supfreq.Contains("E"))
                                {
                                    freq_e += supfreq + ",";
                                }
                            }

                            oComm.CommandText = "update 扩容结果 set 当前Sector的F频段RRU型号 = '" + strrruname + "' where cellname = '" + strcellname + "'";
                            oComm.ExecuteNonQuery();

                            if (!string.IsNullOrEmpty(freq_a))
                            {
                                freq_a = freq_a.Substring(0, freq_a.Length - 1);
                                oComm.CommandText = "update 扩容结果 set 当前Sector_A频RRU支持频率 = '" + freq_a + "' where cellname = '" + strcellname + "'";
                                oComm.ExecuteNonQuery();
                            }
                            if (!string.IsNullOrEmpty(freq_e))
                            {
                                freq_e = freq_e.Substring(0, freq_e.Length - 1);
                                oComm.CommandText = "update 扩容结果 set 当前Sector_E频RRU支持频率 = '" + freq_e + "' where cellname = '" + strcellname + "'";
                                oComm.ExecuteNonQuery();
                            }
                            if (!string.IsNullOrEmpty(freq_f))
                            {
                                freq_f = freq_f.Substring(0, freq_f.Length - 1);
                                oComm.CommandText = "update 扩容结果 set 当前Sector_F频RRU支持频率 = '" + freq_f + "' where cellname = '" + strcellname + "'";
                                oComm.ExecuteNonQuery();
                            }

                        }
                    }
                    else
                    {
                        oComm.CommandText = "update 扩容结果 set 当前Sector的" + strrrufreq + "频段RRU型号 = '" + strrruname + "',当前Sector_" + strrrufreq + "频RRU支持频率='" + strrrusupfreq + "' where cellname = '" + strcellname + "'";
                        oComm.ExecuteNonQuery();
                    }
                }
                
                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "UpdateRRUInfo");
                throw;
            }
        }

        /// <summary>
        /// 获取RRU类型
        /// </summary>
        private List<string[]> GetRRUType()
        {
            try
            {
                List<string[]> lstcellinfo = GetRRUMark();
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                string strrruname = "";
                for (int i=0;i<lstcellinfo.Count;i++)
                {
                    strrruname = "";
                    if (string.IsNullOrEmpty(lstcellinfo[i][4]))
                    {
                        continue;
                    }
                    //包含多个RRU类型
                    if (lstcellinfo[i][4].Contains(','))
                    {
                        string[] rrutypenum = lstcellinfo[i][4].Split(',');
                        foreach (var rrunum in rrutypenum)
                        {
                            oComm.CommandText = "select 射频单元类型名称 from 射频单元拓扑 where 网元标识='" + lstcellinfo[i][1] + "' and (射频单元编号='" + rrunum + ".0' or 射频单元编号='" + rrunum + "')";
                            SqlDataReader dr = oComm.ExecuteReader();
                            if (dr.Read())
                            {
                                strrruname += dr[0].ToString() + ",";
                            }
                            dr.Close();
                        }
                        strrruname = strrruname.Substring(0, strrruname.Length - 1);
                        
                    }
                    else
                    {
                        oComm.CommandText = "select 射频单元类型名称 from 射频单元拓扑 where 网元标识='" + lstcellinfo[i][1] + "' and (射频单元编号='" + lstcellinfo[i][4] + ".0' or 射频单元编号='" + lstcellinfo[i][4] + "')";
                        SqlDataReader dr = oComm.ExecuteReader();
                        if (dr.Read())
                        {
                            strrruname = dr[0].ToString();
                        }
                        dr.Close();
                    }
                    lstcellinfo[i][5] = strrruname;
                }
                oComm.Dispose();
                this.oConn.Close();
                return lstcellinfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "GetRRUType");
                throw;
            }
        }

        /// <summary>
        /// 获取RRU标识
        /// </summary>
        /// <returns></returns>
        private List<string[]> GetRRUMark()
        {
            try
            {
                List<string[]> lstcellinfo = GetLocalcellID();
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                for (int i = 0; i < lstcellinfo.Count; i++)
                {
                    oComm.CommandText = "select RRU标识 from 子小区规划 where 网元标识='" + lstcellinfo[i][1] + "' and 本地小区标识='" + lstcellinfo[i][3] + "'";
                    string rrulogo = Convert.ToString(oComm.ExecuteScalar());
                    //去掉引号
                    if (rrulogo.Contains("\""))
                        rrulogo = rrulogo.Replace("\"", string.Empty);
                    //去掉大括号
                    if (rrulogo.Contains("{") && rrulogo.Contains("}"))
                    {
                        rrulogo = rrulogo.Replace("{", string.Empty);
                        rrulogo = rrulogo.Replace("}", string.Empty);
                        lstcellinfo[i][4] = rrulogo;
                    }
                }
              
                oComm.Dispose();
                this.oConn.Close();
                return lstcellinfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "GetRRUMark");
                throw;
            }
        }

        /// <summary>
        /// 获取现网LocalcellID
        /// </summary>
        /// <returns></returns>
        private List<string[]> GetLocalcellID()
        {
            try
            {
                List<string[]> lstcellinfo = new List<string[]>();
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                SqlDataReader dr = null;
                string strnetmark = "";
                string strsectorid = "";
                foreach (var cellname in lstcellname)
                {
                    oComm.CommandText = "select 网元标识,扩容SectorID from 扩容结果 where cellname='" + cellname + "'";
                    dr = oComm.ExecuteReader();
                    if (dr.Read())
                    {
                        strnetmark = dr[0].ToString();
                        strsectorid = dr[1].ToString();
                    }
                    dr.Close();

                    //获取现网LocalcellID
                    oComm.CommandText = "select 现网LocalcelID from LTE工参信息 where eNodeBID = '" + strnetmark + "' and SectorID ='" + strsectorid + "'";
                    dr = oComm.ExecuteReader();
                    while (dr.Read())
                    {
                        string[] info = new string[6];
                        info[0] = cellname;
                        info[1] = strnetmark;
                        info[2] = strsectorid;
                        info[3] = dr[0].ToString();
                        lstcellinfo.Add(info);
                    }
                    dr.Close();
                }
                oComm.Dispose();
                this.oConn.Close();
                return lstcellinfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "GetLocalcellID");
                throw;
            }
        }

        /// <summary>
        /// 计算当前小区Sector频段配置
        /// </summary>
        private void SetSectorType()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                string strnetmark = "";
                string strsectorid = "";
                string strsectorconf = "";
                string strtemp = "";
                List<string> lstearfcn = new List<string>();
                foreach (var cellname in lstcellname)
                {
                    strtemp = "";
                    strsectorconf = "";
                    oComm.CommandText = "select 网元标识,扩容SectorID from 扩容结果 where cellname ='" + cellname + "'";
                    SqlDataReader dr = oComm.ExecuteReader();
                    if(dr.Read())
                    {
                        strnetmark = dr[0].ToString();
                        strsectorid = dr[1].ToString();
                        dr.Close();

                        lstearfcn.Clear();
                        oComm.CommandText = "select EARFCN from LTE工参信息 where eNodeBID='" + strnetmark + "' and SectorID = '" + strsectorid + "'";
                        dr = oComm.ExecuteReader();
                        while (dr.Read())
                        {
                            lstearfcn.Add(dr[0].ToString());
                        }
                        dr.Close();
                       
                        foreach (var strearfcn in lstearfcn)
                        {
                            oComm.CommandText = "select 频点 from LTE频率频点归属 where 小区绝对信道号100千赫='" + strearfcn + "'";
                            string strfreqpoint = Convert.ToString(oComm.ExecuteScalar());
                            string strmark = strfreqpoint.Substring(0, 1);

                            //找到上次配置累加
                            oComm.CommandText = "select 当前Sector_" + strmark + "频段配置 from 扩容结果 where cellname='" + cellname + "'";
                            strtemp = Convert.ToString(oComm.ExecuteScalar());

                            if (!string.IsNullOrEmpty(strtemp) && !strtemp.Contains(strfreqpoint))
                                strfreqpoint = strtemp + "," + strfreqpoint;

                            oComm.CommandText = "update 扩容结果 set 当前Sector_" + strmark + "频段配置='" + strfreqpoint + "' where cellname='" + cellname + "'";
                            oComm.ExecuteNonQuery();
                        }

                        oComm.CommandText = "select 当前Sector_F频段配置,当前Sector_D频段配置,当前Sector_E频段配置,当前Sector_A频段配置 from 扩容结果 where cellname='" + cellname + "'";
                        dr = oComm.ExecuteReader();
                        if (dr.Read())
                        {
                            if (!string.IsNullOrEmpty(dr[0].ToString()))
                                strsectorconf += dr[0].ToString() + ",";
                            if (!string.IsNullOrEmpty(dr[1].ToString()))
                                strsectorconf += dr[1].ToString() + ",";
                            if (!string.IsNullOrEmpty(dr[2].ToString()))
                                strsectorconf += dr[2].ToString() + ",";
                            if (!string.IsNullOrEmpty(dr[3].ToString()))
                                strsectorconf += dr[3].ToString() + ",";
                        }
                        dr.Close();

                        if (!string.IsNullOrEmpty(strsectorconf))
                        {
                            strsectorconf = strsectorconf.Substring(0, strsectorconf.Length - 1);
                            oComm.CommandText = "update 扩容结果 set 当前Sector频点配置='" + strsectorconf + "' where cellname='" + cellname + "'";
                            oComm.ExecuteNonQuery();
                        }
                    }
                }

                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetSectorType");
                throw;
            }
        }

        /// <summary>
        /// 计算扩容方式
        /// </summary>
        private void SetESTType()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                foreach (var cellname in lstcellname)
                {
                    oComm.CommandText = "select (实际可扩小区数 - 基站需扩容小区数) from 扩容结果 where cellname = '" + cellname + "' and 实际可扩小区数 is not null";
                    if (string.IsNullOrEmpty(Convert.ToString(oComm.ExecuteScalar())))
                        continue;
                    int ndiff = Convert.ToInt32(oComm.ExecuteScalar());
                    if (ndiff > 0)
                    {
                        CalculateEst(cellname);
                    }
                    else
                    {
                        int nrrusupcells = 0;
                        int nstationconfcells = 0;
                        int nafterupdbord = 0;
                        int nenbneedest = 0;
                        //更新扩容频点
                        oComm.CommandText = "update 扩容结果 set 备注='更换SCTF+BPOI' where cellname ='" + cellname + "'";
                        oComm.ExecuteNonQuery();

                        oComm.CommandText = "select RRU支持小区数,站点配置小区数,基站需扩容小区数 from 扩容结果 where cellname ='" + cellname + "'";
                        SqlDataReader dr = oComm.ExecuteReader();
                        if (dr.Read())
                        {
                            nrrusupcells = Convert.ToInt32(dr[0].ToString());
                            nstationconfcells = Convert.ToInt32(dr[1].ToString());
                            nenbneedest = Convert.ToInt32(dr[2].ToString());
                        }
                        dr.Close();
                        if (nrrusupcells < 12)
                        {
                            nafterupdbord = nrrusupcells - nstationconfcells;
                        }
                        else
                        {
                            nafterupdbord = 12 - nstationconfcells;
                        }

                        oComm.CommandText = "update 扩容结果 set 更换板卡后可扩容个数=" + nafterupdbord + " where cellname ='" + cellname + "'";
                        oComm.ExecuteNonQuery();

                        if (nafterupdbord - nenbneedest > 0)
                        {
                            CalculateEst(cellname);
                        }
                        else
                        {
                            //更新扩容频点
                            oComm.CommandText = "update 扩容结果 set 扩容方式='新建BBU' where cellname ='" + cellname + "'";
                            oComm.ExecuteNonQuery();
                        }
                    }
                }
                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetESTType");
                throw;
            }
        }

        private void CalculateEst(string cellname)
        {
            try
            {
                //取当前sector频点
                oComm.CommandText = "select 当前Sector频点配置 from 扩容结果 where cellname = '" + cellname + "'";
                string strcurfreqconf = Convert.ToString(oComm.ExecuteScalar());
                if (string.IsNullOrEmpty(strcurfreqconf))
                    return;
                //多个频点拼接查询条件
                string strcondition = GetCondition(strcurfreqconf);
                oComm.CommandText = "select 扩容频点 from 扩容频点规则 where 当前Sector频点配置 = '" + strcondition + "'";
                string strestfreqpoint = Convert.ToString(oComm.ExecuteScalar());
                if (string.IsNullOrEmpty(strestfreqpoint))
                    return;

                //更新扩容频点
                oComm.CommandText = "update 扩容结果 set 建议扩容频点='" + strestfreqpoint + "' where cellname ='" + cellname + "'";
                oComm.ExecuteNonQuery();

                if (strestfreqpoint.Contains("3DMIMO"))
                {
                    //更新扩容频点
                    oComm.CommandText = "update 扩容结果 set 扩容方式='新建BBU' where cellname ='" + cellname + "'";
                    oComm.ExecuteNonQuery();
                }
                else
                {
                    string strmark = strestfreqpoint.Substring(0, 1);
                    oComm.CommandText = "select 当前Sector的" + strmark + "频段RRU型号 from 扩容结果 where cellname = '" + cellname + "'";
                    string strrruname = Convert.ToString(oComm.ExecuteScalar());
                    //查看当前是否支持该频点
                    if (!string.IsNullOrEmpty(strrruname))
                    {
                        oComm.CommandText = "select 扩容方式 from RRU频点扩容方式 where 射频单元类型名称='" + strrruname + "' and 扩容频点='" + strestfreqpoint + "'";
                        string stresttype = Convert.ToString(oComm.ExecuteScalar());
                        //更新扩容频点
                        oComm.CommandText = "update 扩容结果 set 扩容方式='" + stresttype + "' where cellname ='" + cellname + "'";
                        oComm.ExecuteNonQuery();
                    }
                    else
                    {
                        //更新扩容频点
                        oComm.CommandText = "update 扩容结果 set 扩容方式='硬扩RRU' where cellname ='" + cellname + "'";
                        oComm.ExecuteNonQuery();
                    }
                }
              
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "CalculateEst");
                throw;
            }
        }

        /// <summary>
        /// 分解当前频点，作为查询条件
        /// </summary>
        /// <param name="strcurfreqconf"></param>
        /// <returns></returns>
        private string GetCondition(string strcurfreqconf)
        {
            string strcondition = "";
            List<string> lstF = new List<string>();
            List<string> lstD = new List<string>();
            List<string> lstE = new List<string>();
            if (strcurfreqconf.Contains(","))
            {
                string[] strtemp = strcurfreqconf.Split(',');
                foreach (var str in strtemp)
                {
                    if (str.Contains("F"))
                        lstF.Add(str);
                    if (str.Contains("D"))
                        lstD.Add(str);
                    if (str.Contains("E"))
                        lstE.Add(str);
                }
                lstF.Distinct();
                lstE.Distinct();
                lstD.Distinct();
                lstF.Sort();
                lstD.Sort();
                lstE.Sort();
                if (lstF.Count > 0 && lstD.Count > 0)
                {
                    foreach (var F in lstF)
                    {
                        strcondition += F + ",";
                    }
                    foreach (var D in lstD)
                    {
                        strcondition += D + ",";
                    }
                }
                if (lstF.Count > 0 && lstD.Count == 0)
                {
                    foreach (var F in lstF)
                    {
                        strcondition += F + ",";
                    }
                }
                if (lstD.Count > 0 && lstF.Count == 0)
                {
                    foreach (var D in lstD)
                    {
                        strcondition += D + ",";
                    }
                }
                if (lstE.Count > 0)
                {
                    foreach (var E in lstE)
                    {
                        strcondition += E + ",";
                    }
                }
                strcondition = strcondition.Substring(0, strcondition.Length - 1);
                return strcondition;
            }
            else
            {
                return strcurfreqconf;
            }
        }

        /// <summary>
        /// 计算共站信息
        /// </summary>
        private void SetCommonCoverInfo()
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                int nnetmark = 0;
                int nsectorid = 0;
                string strnetfriendname = "";
                string stresttype = "";
                string strnetmark2 = "";
                string strcellname2 = "";
                string strsector = "";
                SqlDataReader dr = null;
                foreach (var cellname in lstcellname)
                {
                    oComm.CommandText = "select 网元标识,扩容SectorID,网元友好名,扩容方式 from 扩容结果 where cellname = '" + cellname + "'";
                    dr = oComm.ExecuteReader();
                    if (dr.Read())
                    {
                        nnetmark = Convert.ToInt32(dr[0].ToString());
                        nsectorid = Convert.ToInt32(dr[1].ToString());
                        strnetfriendname = dr[2].ToString();
                        stresttype = dr[3].ToString();
                    }
                    dr.Close();

                    oComm.CommandText = "select 小区名2,网元标识2,SectorID from 共站信息 where 网元标识1='" + nnetmark.ToString() + "' and SectorID = '" + nsectorid + "'";
                    dr = oComm.ExecuteReader();
                    if (dr.Read())
                    {
                        strcellname2 = dr[0].ToString();
                        strnetmark2 = dr[1].ToString();
                        strsector = dr[2].ToString();
                    }
                    dr.Close();

                    if (!string.IsNullOrEmpty(strnetmark2) && !string.IsNullOrEmpty(strcellname2) && !string.IsNullOrEmpty(strsector))
                    {
                        oComm.CommandText = "select 扩容方式 from 扩容结果 where 网元标识 ='" + strnetmark2 + "' and 扩容SectorID =" + strsector;
                        string strtype = Convert.ToString(oComm.ExecuteScalar());
                        string chkres = ChkDel(stresttype,strtype);
                        if (chkres == "delcell2")
                        {
                            oComm.CommandText = "delete from 扩容结果 where 网元标识 ='" + strnetmark2 + "' and 扩容SectorID =" + strsector;
                            oComm.ExecuteNonQuery();

                            oComm.CommandText = "update 扩容结果 set 共覆盖站点='" + nnetmark + ";" + strnetmark2 + "',网元友好名='" + strnetfriendname + "&" + strcellname2 + "' where cellname ='" + cellname + "'";
                            oComm.ExecuteNonQuery();
                            

                        }
                        if (chkres == "delcell1")
                        {
                            oComm.CommandText = "delete from 扩容结果 where cellname ='" + cellname + "'";
                            oComm.ExecuteNonQuery();

                            oComm.CommandText = "update 扩容结果 set 共覆盖站点='" + nnetmark + ";" + strnetmark2 + "',网元友好名='" + strnetfriendname + "&" + strcellname2 + "' where 网元标识 ='" + strnetmark2 + "' and 扩容SectorID =" + strsector;
                            oComm.ExecuteNonQuery();
                        }
                    }
                }

                oComm.Dispose();
                this.oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrTip(e.Message, "SetCommonCoverInfo");
                throw;
            }
        }

        /// <summary>
        /// 判断删除共站优先级
        /// </summary>
        /// <param name="strcell1"></param>
        /// <param name="strcell2"></param>
        /// <returns></returns>
        private string ChkDel(string strcell1, string strcell2)
        {
            if (strcell1 == "软扩" && strcell2 == "软扩")
            {
                return "delcell2";
            }
            if(strcell1 == "软扩" && strcell2 == "硬扩")
            {
                return "delcell2";
            }
            if(strcell1 == "软扩" && strcell2 == "新建BBU")
            {
                return "delcell2";
            }
            if(strcell1 == "硬扩" && strcell2 == "软扩")
            {
                return "delcell1";
            }
            if (strcell1 == "硬扩" && strcell2 == "硬扩")
            {
                return "delcell2";
            }
            if (strcell1 == "硬扩" && strcell2 == "新建BBU")
            {
                return "delcell2";
            }
            if(strcell1 == "新建BBU" && strcell2 == "软扩")
            {
                return "delcell1";
            }
            if(strcell1 == "新建BBU" && strcell2 == "硬扩")
            {
                return "delcell1";
            }
            if(strcell1 == "新建BBU" && strcell2 == "新建BBU")
            {
                return "delcell2";
            }
            return "";
        }

        private void ErrTip(string strerrmsg,string strfunname)
        {
            dilatation.strCurTip = "程序运行出现错误：在函数：（" + strfunname + "）\r\n详细信息：" + strerrmsg;
            ((TipReFresher)trf).CurTip();
        }
    
    }
}
