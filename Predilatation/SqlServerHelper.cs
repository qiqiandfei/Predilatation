﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mime;
using System.Runtime.CompilerServices;

namespace Predilatation
{
    public class SqlServerHelper
    {
        public static string strserver { get; set; }
        public static string struser { get; set; }
        public static string strpwd { get; set; }
        public string strConn { get; set; }
        public SqlConnection oConn { get; set; }
        public SqlCommand oComm{ get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="user">用户名</param>
        /// <param name="pwd">密码</param>
        public SqlServerHelper()
        {
            ReadConfigFile();
            strConn = "data source=" + strserver + ";Initial Catalog=master;user id=" + struser + ";pwd=" + strpwd + ";Connect Timeout=0";
            oConn = new SqlConnection(strConn);
            oComm = oConn.CreateCommand();
            oComm.CommandTimeout = 0;

        }

        
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <returns></returns>
        public string CreatDataBase()
        {
            string strErrMsg = "";
            //建立数据库   
            //SqlCommand oComm = this.oConn.CreateCommand(); = this.oConn.CreateCommand();
            try
            {
                this.oConnOpen();
                oComm.CommandText = "select name From master.dbo.sysdatabases where name='Predilatation'";
                string res = (string)oComm.ExecuteScalar();
                if (res != "Predilatation")
                {
                    //创建数据库
                    oComm.CommandText = "CREATE DATABASE Predilatation";
                    oComm.ExecuteNonQuery();
                    
                }
                
            }
            catch(Exception ex)
            {
                strErrMsg = ex.Message;
            }
            
            oComm.Dispose();
            this.oConn.Close();
            return strErrMsg;
        }

        /// <summary>
        /// 检查数据库是否存在
        /// </summary>
        /// <returns></returns>
        public string CheckDataBase()
        {
            string strErrMsg = "";
            //建立数据库   
            //SqlCommand oComm = this.oConn.CreateCommand(); = this.oConn.CreateCommand();
            try
            {
                this.oConnOpen();
                oComm.CommandText = "select name From master.dbo.sysdatabases where name='Predilatation'";
                string res = (string)oComm.ExecuteScalar();
                if (res != "Predilatation")
                {

                    strErrMsg = "尚未创建数据库，请先创建数据库！";
                }

            }
            catch (Exception ex)
            {
                strErrMsg = ex.Message;
            }

            oComm.Dispose();
            this.oConn.Close();
            return strErrMsg;
        }

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public string CheckTable(string strTableName)
        {
            string strErrMsg = "";
            //建立数据库   
            //SqlCommand oComm = this.oConn.CreateCommand(); = this.oConn.CreateCommand();
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                oComm.CommandText = "select * from sysobjects where  type = 'U' and name = '" + strTableName + "'";
                string res = (string)oComm.ExecuteScalar();
                if (res != strTableName)
                {
                    if (strTableName.Contains("Before"))
                    {
                        strErrMsg = "前N天数据表不存在，请先创建！";
                    }
                    if (strTableName.Contains("After"))
                    {
                        strErrMsg = "后N天数据表不存在，请先创建！";
                    }
                }
            }
            catch (Exception ex)
            {
                strErrMsg = ex.Message;
            }

            oComm.Dispose();
            this.oConn.Close();
            return strErrMsg;
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="conn"></param>
        public void CreateTable(string strTableName)
        {
            this.oConnOpen();
            this.oConn.ChangeDatabase("Predilatation");       
            //SqlCommand oComm = this.oConn.CreateCommand(); = this.oConn.CreateCommand();
            try
            {
                oComm.CommandText = "if  exists(select * from sysobjects where  type = 'U' and name = '" + strTableName + "') drop table " + strTableName + " " +
                                    "create table " + strTableName + "" +
                                    "(cellname varchar(255) not null," +
                                    "time varchar(255) not null," +
                                    "E_RAB建立成功数 numeric(18,4)," +
                                    "RRC连接最大数 numeric(18,4)," +
                                    "有效RRC连接最大数 numeric(18,4)," +
                                    "有效RRC连接平均数 numeric(18,4)," +
                                    "上行PUSCH_PRB利用率 numeric(18,4)," +
                                    "下行PDSCH_PRB利用率 numeric(18,4)," +
                                    "PDCCH信道CCE占用率 numeric(18,4)," +
                                    "小区用户面上行字节数 numeric(18,4)," +
                                    "小区用户面下行字节数 numeric(18,4)," +
                                    "上行占用的PRB个数 numeric(18,4)," +
                                    "RRU_PrbUl_TotalNum numeric(18,4)," +
                                    "下行占用的PRB个数 numeric(18,4)," +
                                    "RRU_PrbDl_TotalNum numeric(18,4)," +
                                    "CCE占用量 numeric(18,4)," +
                                    "CCE可使用量 numeric(18,4)," +
                                    "总流量 numeric(18,4)," +
                                    "下行PDCCH_CCE利用率 numeric(18,4)," +
                                    "平均E_RAB流量 numeric(18,4)," +
                                    "最大利用率 numeric(18,4)," +
                                    "FlowBusy varchar(255),"+
                                    "Utilizaerate varchar(255))";
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
        /// 添加主键
        /// </summary>
        public void AddPrimary(string strTableName,string strPK_Name,string strKey)
        {
            this.oConnOpen();
            this.oConn.ChangeDatabase("Predilatation");
            try
            {
                oComm.CommandText = "alter table " + strTableName + " add constraint " + strPK_Name + " primary key (" + strKey + ")";
                oComm.ExecuteNonQuery();
                //给Tempdata_After创建索引
                //if (strTableName == "Tempdata_After")
                //{
                //    oComm.CommandText = "CREATE NONCLUSTERED INDEX FindBiger " +
                //                        "ON Tempdata_After (有效RRC连接平均数 asc,上行PUSCH_PRB利用率 asc,下行PDSCH_PRB利用率 asc,PDCCH信道CCE占用率 asc,小区用户面上行字节数 asc,小区用户面下行字节数 asc)";
                //    oComm.ExecuteNonQuery();

                //    oComm.CommandText = "CREATE NONCLUSTERED INDEX FindSmaller " +
                //                        "ON Tempdata_After (有效RRC连接平均数 desc,上行PUSCH_PRB利用率 desc,下行PDSCH_PRB利用率 desc,PDCCH信道CCE占用率 desc,小区用户面上行字节数 desc,小区用户面下行字节数 desc)";
                //    oComm.ExecuteNonQuery();
                //}
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
        /// 读取数据库配置信息
        /// </summary>
        private void ReadConfigFile()
        {
            string[] content = File.ReadAllLines(System.Windows.Forms.Application.StartupPath + "\\DBConfig.txt",Encoding.Default);
            strserver = content[0].Substring(content[0].IndexOf("：")+1);
            struser = content[1].Substring(content[1].IndexOf("：")+1);
            strpwd = content[2].Substring(content[2].IndexOf("：")+1);
        }

        /// <summary>
        /// 文件转DataTable
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public void GetTablefromFile(string[] files,object trf,string strTableName)
        {
            DataTable dt = GetTable();
            foreach (var file in files)
            {
                string strFileName = Path.GetFileName(file);
                MainForm.strCurTip = "正在将"+strFileName+"生成数据表...";
                ((TipReFresher)trf).CurTip();
                dt.Rows.Clear();
                List<string> ContentList = File.ReadAllLines(file,Encoding.Default).ToList();
                ContentList.RemoveRange(0,5);
                foreach (var line in ContentList)
                {
                    string Row = line.Replace("\"","");
                    string[] ItemRow = Row.Split(',');
                    //无效标致
                    bool Invalidflg = false;
                    DataRow dtrow = dt.NewRow();
                    dtrow[0] = ItemRow[0].Trim() + "," + ItemRow[1] + "," + ItemRow[2] + "," + ItemRow[3];
                    dtrow[1] = ItemRow[4];
                    for (int i = 5; i < 20; i++)
                    {
                        if (ItemRow[i].Contains('-') || string.IsNullOrEmpty(ItemRow[i]) || ItemRow[i].Contains("#NA") || ItemRow[i].Contains("N/A"))
                        {
                            Invalidflg = true;
                            dtrow[i - 3] = -1234567890123.1234;
                        }
                            
                        else
                        {
                            dtrow[i - 3] = Convert.ToDouble(ItemRow[i]);
                        }
                        
                    }
                    //有效计算无效置0
                    if (!Invalidflg)
                    {
                        //double dF17 = Convert.ToDouble(ItemRow[14]) / Convert.ToDouble(ItemRow[15]);//上行pusch
                        //double dF18 = Convert.ToDouble(ItemRow[16]) / Convert.ToDouble(ItemRow[17]);//下行pusch
                        double dCCE = Convert.ToDouble(ItemRow[18]) / Convert.ToDouble(ItemRow[19]) * 100;//下行cce
                        dtrow[17] = Convert.ToDouble(ItemRow[12]) + Convert.ToDouble(ItemRow[13]);//总流量
                        dtrow[18] = dCCE;
                        //dtrow[20] = dF19;
                        if (Convert.ToDouble(ItemRow[5]) == 0)
                        {
                            dtrow[19] = 0;
                        }
                        else
                        {
                            dtrow[19] = (Convert.ToDouble(ItemRow[12]) + Convert.ToDouble(ItemRow[13])) / Convert.ToDouble(ItemRow[5]);//平均ERAB
                        }
                        dtrow[20] = GetMax(Convert.ToDouble(ItemRow[9]), Convert.ToDouble(ItemRow[10]), dCCE);

                    }
                    else
                    {
                        dtrow[17] = 0;
                        dtrow[18] = 0;
                        dtrow[19] = 0;
                        dtrow[20] = 0;
                    }
                   
                    dtrow[21] = "";
                    dtrow[22] = "";
                    dt.Rows.Add(dtrow);
                }
                MainForm.strCurTip = "正在将" + strFileName + "写入数据库...";
                ((TipReFresher) trf).CurTip();
                BulkToDB(dt,strTableName);
                //计算进度条
                MainForm.nProValue += (int)Math.Ceiling(Convert.ToDouble(100 / (double)files.Length));
                ((TipReFresher)trf).CurTip();
            }
           
        }

        
        /// <summary>
        /// DataTable插入数据库
        /// </summary>
        /// <param name="dt"></param>
        public void BulkToDB(DataTable dt,string strTableName)
        {
            string strconn = "data source=" + strserver + ";Initial Catalog=Predilatation;user id=" + struser + ";pwd=" + strpwd;
            using (SqlConnection conn = new SqlConnection(strconn))
            {
                using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(strconn, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    conn.Open();
                    try
                    {
                        sqlbulkcopy.DestinationTableName = strTableName;
                        sqlbulkcopy.BatchSize = dt.Rows.Count;
                        sqlbulkcopy.BulkCopyTimeout = 0;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        sqlbulkcopy.WriteToServer(dt);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 删除临时表
        /// </summary>
        private void DelTempTable(string strTableName)
        {
            //SqlCommand oComm = this.oConn.CreateCommand(); = this.oConn.CreateCommand();
            try
            {
                oComm.CommandText = "if  exists(select * from sysobjects where  type = 'U' and name = '" + strTableName + "') drop table " + strTableName;
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
        /// 为表格添加列
        /// </summary>
        /// <param name="dt"></param>
        private DataTable GetTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]{  
            new DataColumn("cellname",typeof(string)),  
            new DataColumn("time",typeof(string)),  
            new DataColumn("E_RAB建立成功数",typeof(double)),
            new DataColumn("RRC连接最大数",typeof(double)),
            new DataColumn("有效RRC连接最大数",typeof(double)),
            new DataColumn("有效RRC连接平均数",typeof(double)),
            new DataColumn("上行PUSCH_PRB利用率",typeof(double)),
            new DataColumn("下行PDSCH_PRB利用率",typeof(double)),
            new DataColumn("PDCCH信道CCE占用率",typeof(double)),
            new DataColumn("小区用户面上行字节数",typeof(double)),
            new DataColumn("小区用户面下行字节数",typeof(double)),
            new DataColumn("上行占用的PRB个数",typeof(double)),
            new DataColumn("RRU_PrbUl_TotalNum",typeof(double)),
            new DataColumn("下行占用的PRB个数",typeof(double)),
            new DataColumn("RRU_PrbDl_TotalNum",typeof(double)),
            new DataColumn("CCE占用量",typeof(double)),
            new DataColumn("CCE可使用量",typeof(double)),
            new DataColumn("总流量",typeof(double)),
            new DataColumn("下行PDCCH_CCE利用率",typeof(double)),
            new DataColumn("平均E_RAB流量",typeof(double)),
            new DataColumn("最大利用率",typeof(double)),
            new DataColumn("FlowBusy",typeof(string)),
            new DataColumn("Utilizaerate",typeof(string))});    
            return dt;
        }

        /// <summary>
        /// 清洗无效数据
        /// </summary>
        /// <param name="files"></param>
        public void DataClean(object trf,string strTableName)
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");

                MainForm.nProValue = 40;
                MainForm.strCurTip = "正在删除无效数据...";
                ((TipReFresher)trf).CurTip();

                oComm.CommandText = "delete from  " + strTableName +
                                    " where 总流量 = 0 " +
                                    "and 下行PDCCH_CCE利用率 = 0 " +
                                    "and 平均E_RAB流量 = 0 " +
                                    "and 最大利用率 = 0";
                oComm.ExecuteNonQuery();
                MainForm.nProValue = 100;
                ((TipReFresher)trf).CurTip();

                MainForm.nProValue = 40;
                MainForm.strCurTip = "正在删除单日不满足20小时的小区...";
                ((TipReFresher)trf).CurTip();
                //删除单日中不满足20小时的小区
                oComm.CommandText = "select cellname,SUBSTRING(time,1,10) from  " + strTableName +
                                    " group by cellname,SUBSTRING(time,1,10) having(COUNT(cellname) < 20)";
                SqlDataReader dr = oComm.ExecuteReader();
                List<string>lstsql = new List<string>();
                while (dr.Read() && dr.HasRows)
                {
                    lstsql.Add("delete from " + strTableName + " where cellname = '" + Convert.ToString(dr[0]) + "' and time like '%" + Convert.ToString(dr[1]) + "%'");
                }
                dr.Close();

                for (int i = 0; i < lstsql.Count; i++)
                {
                    oComm.CommandText = lstsql[i];
                    oComm.ExecuteNonQuery();
                }

                //前N天删除7天中不满足4天的小区
                if (strTableName == "Tempdata_Before")
                {

                    MainForm.nProValue = 60;
                    MainForm.strCurTip = "正在删除7天中不满足4天的全部小区...";
                    ((TipReFresher)trf).CurTip();
                    //删除7天中不满足4天的全部小区
                    oComm.CommandText = "delete from " + strTableName + " where cellname in (select distinct(cellname) from " + strTableName + " group by cellname having(COUNT(cellname) < 20*4))";
                    oComm.ExecuteNonQuery();
                }

                MainForm.nProValue = 100;
                MainForm.strCurTip = "前N天数据清洗完成！";
                ((TipReFresher)trf).CurTip();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                this.oComm.Dispose();
                this.oConn.Close();
            }
            
        }

        /// <summary>
        /// 清洗节假日数据
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="nDays"></param>
        /// <param name="trf"></param>
        public void DataClean_Holiday(string strTableName,int nDays,object trf)
        {
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");

                MainForm.nProValue = 40;
                MainForm.strCurTip = "正在删除无效数据...";
                ((TipReFresher)trf).CurTip();

                oComm.CommandText = "delete from  " + strTableName +
                                    " where 总流量 = 0 " +
                                    "and 下行PDCCH_CCE利用率 = 0 " +
                                    "and 平均E_RAB流量 = 0 " +
                                    "and 最大利用率 = 0";
                oComm.ExecuteNonQuery();
                MainForm.nProValue = 100;
                ((TipReFresher)trf).CurTip();
                //大于三天需要继续清洗
                if (nDays > 3)
                {
                    MainForm.nProValue = 40;
                    MainForm.strCurTip = "正在删除单日不满足20小时的小区...";
                    ((TipReFresher)trf).CurTip();
                    //删除单日中不满足20小时的小区
                    oComm.CommandText = "select cellname,SUBSTRING(time,1,10) from  " + strTableName +
                                        " group by cellname,SUBSTRING(time,1,10) having(COUNT(cellname) < 20)";
                    SqlDataReader dr = oComm.ExecuteReader();
                    List<string> lstsql = new List<string>();
                    while (dr.Read() && dr.HasRows)
                    {
                        lstsql.Add("delete from " + strTableName + " where cellname = '" + Convert.ToString(dr[0]) + "' and time like '%" + Convert.ToString(dr[1]) + "%'");
                    }
                    dr.Close();

                    for (int i = 0; i < lstsql.Count; i++)
                    {
                        oComm.CommandText = lstsql[i];
                        oComm.ExecuteNonQuery();
                    }

                    if (strTableName == "Tempdata_Before")
                    {
                        MainForm.nProValue = 60;
                        MainForm.strCurTip = "正在删除7天中不满足4天的全部小区...";
                        ((TipReFresher)trf).CurTip();
                        //删除7天中不满足4天的全部小区
                        oComm.CommandText = "delete from " + strTableName + " where cellname in (select distinct(cellname) from " + strTableName + " group by cellname having(COUNT(cellname) < 20*4))";
                        oComm.ExecuteNonQuery();
                    }
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

        /// <summary>
        /// 计算流量自忙时
        /// </summary>
        public void CalculateBusy_Flow(string strTableName,string FlowTableName)
        {

            this.oConnOpen();
            this.oConn.ChangeDatabase("Predilatation");
            try
            {
                //删除临时表
                DelTempTable(FlowTableName);

                //清空流量自忙时标识
                oComm.CommandText = "update " + strTableName + " set flowbusy = ''";
                oComm.ExecuteNonQuery();

                //创建临时表
                oComm.CommandText = "select cellname,time,E_RAB建立成功数,RRC连接最大数,有效RRC连接最大数,有效RRC连接平均数,下行PDCCH_CCE利用率,小区用户面上行字节数,小区用户面下行字节数,总流量,上行PUSCH_PRB利用率,下行PDSCH_PRB利用率,最大利用率 into " + FlowTableName +
                                    " from " + strTableName + " a," +
                                    "(select b.cellname cellnamen, max(b.总流量) derived1n,SUBSTRING(time,1,10) timen " +
                                    "from " + strTableName + " b where b.总流量 > 0 " +
                                    "group by b.cellname," +
                                    "SUBSTRING(time,1,10)) c " +
                                    "where a.cellname = c.cellnamen " +
                                    "and a.总流量 = c.derived1n " +
                                    "and SUBSTRING(time,1,10)=c.timen";
                oComm.ExecuteNonQuery();

                //删除冗余数据
                DelFlowData(FlowTableName);

                //修改流量自忙时标识
                oComm.CommandText = "update " + strTableName + " set flowbusy = '流量自忙时' where exists(select 1 from " + FlowTableName + " b where " + strTableName + ".cellname = b.cellname and " + strTableName + ".time = b.time)";
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
                this.oConn.Close();
            }
        }

        /// <summary>
        /// 删除流量自忙时冗余数据
        /// </summary>
        private void DelFlowData(string strTableName)
        {
            List<string[]> lstTemp = new List<string[]>();
            //SqlCommand oComm = this.oConn.CreateCommand(); = this.oConn.CreateCommand();
            oComm.CommandText = "select cellname,time,总流量,上行PUSCH_PRB利用率,下行PDSCH_PRB利用率,下行PDCCH_CCE利用率,最大利用率 " +
                                "from " + strTableName + " a, " +
                                "(select b.cellname cellnamen,SUBSTRING(time,1,10) timen " +
                                "from " + strTableName + " b " +
                                "group by b.cellname," +
                                "SUBSTRING(time,1,10) having COUNT(SUBSTRING(time,1,10))>1) c " +
                                "where a.cellname = c.cellnamen " +
                                "and SUBSTRING(time,1,10)=c.timen order by cellname,SUBSTRING(time,1,10),最大利用率 desc";
            SqlDataReader dr = oComm.ExecuteReader();
            while (dr.Read())
            {
                string cellname = Convert.ToString(dr[0]);
                string time = Convert.ToString(dr[1]);
                string[] strsIndex = new string[2];
                strsIndex[0] = cellname;
                strsIndex[1] = time;
                lstTemp.Add(strsIndex);
            }
            dr.Close();
            //查找冗余数据
            List<string[]> DelList = new List<string[]>();
            for (int i = 1; i < lstTemp.Count; i++)
            {
                if (lstTemp[i][0] == lstTemp[i - 1][0] && lstTemp[i][1].Substring(0, 10) == lstTemp[i - 1][1].Substring(0, 10))
                {
                    DelList.Add(lstTemp[i]);
                }
                else
                {
                    continue;
                }
            }

            //删除冗余数据
            for (int i = 0; i < DelList.Count; i++)
            {
                oComm.CommandText = "delete from " + strTableName + " where cellname = '" + DelList[i][0] + "' and time = '" + DelList[i][1] + "'";
                oComm.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 计算利用率自忙时
        /// </summary>
        public void CalculateBusy_Utilizaerate(string strTableName, string strUtilizaerateName)
        {
            this.oConnOpen();
            this.oConn.ChangeDatabase("Predilatation");
            try
            {
                //删除临时表
                DelTempTable(strUtilizaerateName);

                //清空利用率自忙时标识
                oComm.CommandText = "update " + strTableName + " set Utilizaerate = ''";
                oComm.ExecuteNonQuery();

                //创建临时表
                oComm.CommandText = "select cellname,time,E_RAB建立成功数,RRC连接最大数,有效RRC连接最大数,有效RRC连接平均数,小区用户面上行字节数,小区用户面下行字节数,总流量,上行PUSCH_PRB利用率,下行PDSCH_PRB利用率,下行PDCCH_CCE利用率,最大利用率 into " + strUtilizaerateName +
                                    " from " + strTableName + " a, " +
                                    "(select b.cellname cellnamen, max(b.最大利用率) maxraten,SUBSTRING(time,1,10) timen " +
                                    "from " + strTableName + " b where b.最大利用率 > 0 " +
                                    "group by b.cellname, " +
                                    "SUBSTRING(time,1,10)) c " +
                                    "where a.cellname = c.cellnamen " +
                                    "and a.最大利用率 = c.maxraten " +
                                    "and SUBSTRING(time,1,10)=c.timen";
                oComm.ExecuteNonQuery();

                //删除利用率自忙时冗余数据
                DelUtilizaerateData(strUtilizaerateName);

                //修改利用率自忙时标识
                oComm.CommandText = "update " + strTableName + " set Utilizaerate = '利用率自忙时' where exists(select 1 from " + strUtilizaerateName + " b where " + strTableName + ".cellname = b.cellname and " + strTableName + ".time = b.time)";
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
                this.oConn.Close();
            }
        }

        /// <summary>
        /// 删除利用率自忙时冗余数据
        /// </summary>
        private void DelUtilizaerateData(string strTableName)
        {
            List<string[]> lstTemp = new List<string[]>();
            //SqlCommand oComm = this.oConn.CreateCommand(); = this.oConn.CreateCommand();
            oComm.CommandText = "select cellname,time,总流量,上行PUSCH_PRB利用率,下行PDSCH_PRB利用率,下行PDCCH_CCE利用率,最大利用率 " +
                                "from " + strTableName + " a, " +
                                "(select b.cellname cellnamen, max(b.最大利用率) maxraten,SUBSTRING(time,1,10) timen " +
                                "from " + strTableName + " b where b.最大利用率 > 0 " +
                                "group by b.cellname, " +
                                "SUBSTRING(time,1,10) having COUNT(SUBSTRING(time,1,10))>1) c " +
                                "where a.cellname = c.cellnamen " +
                                "and SUBSTRING(time,1,10)=c.timen " +
                                "order by cellname,SUBSTRING(time,1,10),总流量 desc";
            SqlDataReader dr = oComm.ExecuteReader();
            while (dr.Read())
            {
                string cellname = Convert.ToString(dr[0]);
                string time = Convert.ToString(dr[1]);
                string[] strsIndex = new string[2];
                strsIndex[0] = cellname;
                strsIndex[1] = time;
                lstTemp.Add(strsIndex);
            }
            dr.Close();
            //查找冗余数据
            List<string[]> DelList = new List<string[]>();
            for (int i = 1; i < lstTemp.Count; i++)
            {
                if (lstTemp[i][0] == lstTemp[i - 1][0] && lstTemp[i][1].Substring(0, 10) == lstTemp[i - 1][1].Substring(0, 10))
                {
                    DelList.Add(lstTemp[i]);
                }
                else
                {
                    continue;
                }
            }

            //删除冗余数据
            for (int i = 0; i < DelList.Count; i++)
            {
                oComm.CommandText = "delete from " + strTableName + " where cellname = '" + DelList[i][0] + "' and time = '" + DelList[i][1] + "'";
                oComm.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 数据同步
        /// </summary>
        public void DataSync()
        {
           
            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                oComm.CommandText = "delete from Tempdata_After where cellname not in (select distinct(cellname) from Tempdata_Before)";
                oComm.ExecuteNonQuery();
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

        public void DataSync(string strTableName)
        {

            try
            {
                this.oConnOpen();
                this.oConn.ChangeDatabase("Predilatation");
                oComm.CommandText = "delete from Tempdata_After where cellname not in (select distinct(cellname) from Tempdata_Before)";
                oComm.ExecuteNonQuery();

                oComm.CommandText = "delete from "+ strTableName +" where cellname not in (select distinct(cellname) from Tempdata_Before)";
                oComm.ExecuteNonQuery();
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
        /// 三个数取最大
        /// </summary>
        /// <param name="dValue1"></param>
        /// <param name="dValue2"></param>
        /// <param name="dValue3"></param>
        /// <returns></returns>
        public static double GetMax(double dValue1,double dValue2,double dValue3)
        {
            double Value = Math.Max(dValue1, dValue2);
            if (Value >= dValue3)
            {
                return Value;
            }
            else
            {
                return dValue3;
            }
        }

        public void oConnOpen()
        {
            if (this.oConn.State == ConnectionState.Closed)
            {
                this.oConn.Open();
            }
        }

    }
}
