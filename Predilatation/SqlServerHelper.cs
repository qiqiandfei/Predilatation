using System;
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
        public string strserver { get; set; }
        public string struser { get; set; }
        public string strpwd { get; set; }
        public string strConn { get; set; }
        public SqlConnection oConn { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="user">用户名</param>
        /// <param name="pwd">密码</param>
        public SqlServerHelper()
        {
            ReadConfigFile();
            strConn = "data source=" + strserver + ";Initial Catalog=master;user id=" + struser + ";pwd=" + strpwd;
            oConn = new SqlConnection(strConn);

        }

        
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <returns></returns>
        public string CreatDataBase()
        {
            string strErrMsg = "";
            //建立数据库   
            SqlCommand oComm = this.oConn.CreateCommand();
            try
            {
                this.oConn.Open();
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
        /// 创建表
        /// </summary>
        /// <param name="conn"></param>
        public void CreateTable()
        {
            this.oConn.Open();
            this.oConn.ChangeDatabase("Predilatation");       
            SqlCommand oComm = this.oConn.CreateCommand();
            try
            {
                oComm.CommandText = "if  exists(select * from sysobjects where  type = 'U' and name = 'Tempdata') drop table Tempdata " +
                                    "create table Tempdata" +
                                    "(cellname varchar(255) not null," +
                                    "time varchar(255) not null," +
                                    "F1 numeric(18,4)," +
                                    "F2 numeric(18,4)," +
                                    "F3 numeric(18,4)," +
                                    "F4 numeric(18,4)," +
                                    "F5 numeric(18,4)," +
                                    "F6 numeric(18,4)," +
                                    "F7 numeric(18,4)," +
                                    "F8 numeric(18,4)," +
                                    "F9 numeric(18,4)," +
                                    "F10 numeric(18,4)," +
                                    "F11 numeric(18,4)," +
                                    "F12 numeric(18,4)," +
                                    "F13 numeric(18,4)," +
                                    "F14 numeric(18,4)," +
                                    "F15 numeric(18,4)," +
                                    "Derived1 numeric(18,4)," +
                                    "Derived2 numeric(18,4)," +
                                    "Derived3 numeric(18,4)," +
                                    "Derived4 numeric(18,4)," +
                                    "Derived5 numeric(18,4)," +
                                    "MaxRate numeric(18,4)," +
                                    "FlowBusy varchar(255),"+
                                    "Utilizaerate varchar(255))";
                oComm.ExecuteNonQuery();
                
            }
            catch (Exception e)
            {
                throw e;
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
            this.oConn.Open();
            this.oConn.ChangeDatabase("Predilatation");
            SqlCommand oComm = this.oConn.CreateCommand();
            try
            {
                oComm.CommandText = "alter table " + strTableName + " add constraint " + strPK_Name + " primary key (" + strKey + ")";
                oComm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
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
            this.strserver = content[0].Substring(content[0].IndexOf("：")+1);
            this.struser = content[1].Substring(content[1].IndexOf("：")+1);
            this.strpwd = content[2].Substring(content[2].IndexOf("：")+1);
        }

        /// <summary>
        /// 文件转DataTable
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public void GetTablefromFile(string[] files,object trf)
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
                        if (ItemRow[i].Contains('-') || string.IsNullOrEmpty(ItemRow[i]) || ItemRow[i].Contains("#NA"))
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
                        double dF17 = Convert.ToDouble(ItemRow[14]) / Convert.ToDouble(ItemRow[15]);
                        double dF18 = Convert.ToDouble(ItemRow[16]) / Convert.ToDouble(ItemRow[17]);
                        double dF19 = Convert.ToDouble(ItemRow[18]) / Convert.ToDouble(ItemRow[19]);
                        dtrow[17] = Convert.ToDouble(ItemRow[12]) + Convert.ToDouble(ItemRow[13]);
                        dtrow[18] = dF17;
                        dtrow[19] = dF18;
                        dtrow[20] = dF19;
                        if (Convert.ToDouble(ItemRow[5]) == 0)
                        {
                            dtrow[21] = 0;
                        }
                        else
                        {
                            dtrow[21] = (Convert.ToDouble(ItemRow[12]) + Convert.ToDouble(ItemRow[13])) / Convert.ToDouble(ItemRow[5]);
                        }
                        dtrow[22] = GetMax(dF17,dF18,dF19);

                    }
                    else
                    {
                        dtrow[17] = 0;
                        dtrow[18] = 0;
                        dtrow[19] = 0;
                        dtrow[20] = 0;
                        dtrow[21] = 0;
                        dtrow[22] = 0;
                    }
                   
                    dtrow[23] = "";
                    dtrow[24] = "";
                    dt.Rows.Add(dtrow);
                }
                MainForm.strCurTip = "正在将" + strFileName + "写入数据库...";
                ((TipReFresher)trf).CurTip();
                BulkToDB(dt, "Tempdata");
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
                        sqlbulkcopy.BulkCopyTimeout = 30;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        sqlbulkcopy.WriteToServer(dt);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
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
            SqlCommand oComm = this.oConn.CreateCommand();
            try
            {
                oComm.CommandText = "if  exists(select * from sysobjects where  type = 'U' and name = '" + strTableName + "') drop table " + strTableName;
                oComm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
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
            new DataColumn("F1",typeof(double)),
            new DataColumn("F2",typeof(double)),
            new DataColumn("F3",typeof(double)),
            new DataColumn("F4",typeof(double)),
            new DataColumn("F5",typeof(double)),
            new DataColumn("F6",typeof(double)),
            new DataColumn("F7",typeof(double)),
            new DataColumn("F8",typeof(double)),
            new DataColumn("F9",typeof(double)),
            new DataColumn("F10",typeof(double)),
            new DataColumn("F11",typeof(double)),
            new DataColumn("F12",typeof(double)),
            new DataColumn("F13",typeof(double)),
            new DataColumn("F14",typeof(double)),
            new DataColumn("F15",typeof(double)),
            new DataColumn("Derived1",typeof(double)),
            new DataColumn("Derived2",typeof(double)),
            new DataColumn("Derived3",typeof(double)),
            new DataColumn("Derived4",typeof(double)),
            new DataColumn("Derived5",typeof(double)),
            new DataColumn("MaxRate",typeof(double)),
            new DataColumn("FlowBusy",typeof(string)),
            new DataColumn("Utilizaerate",typeof(string))});    
            return dt;
        }

        /// <summary>
        /// 清洗无效数据
        /// </summary>
        /// <param name="files"></param>
        public void DataClean(string[] files,object trf)
        {
            string strTime_s = "";
            string strTime_d = "";
            this.oConn.Open();
            this.oConn.ChangeDatabase("Predilatation");
            SqlCommand oComm = this.oConn.CreateCommand();
            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetExtension(files[i]).ToLower() != ".csv")
                    continue;

                strTime_s = Path.GetFileNameWithoutExtension(files[i]);
                for (int j = 0; j < files.Length; j++)
                {
                    if (Path.GetExtension(files[j]).ToLower() != ".csv")
                        continue; 

                    strTime_d = Path.GetFileNameWithoutExtension(files[j]);
                    if (strTime_s != strTime_d)
                    {
                        oComm.CommandText = "delete from Tempdata where cellname in (SELECT distinct(cellname) FROM Tempdata where (F1 = -1234567890123.1234 or F2 = -1234567890123.1234 or F3 = -1234567890123.1234 or F4 = -1234567890123.1234 or F5 = -1234567890123.1234 or F6 = -1234567890123.1234 or F7 = -1234567890123.1234 or F8 = -1234567890123.1234 or F9 = -1234567890123.1234 or F10 = -1234567890123.1234 or F11 = -1234567890123.1234 or F12 = -1234567890123.1234 or F13 = -1234567890123.1234 or F14 = -1234567890123.1234 or F15 = -1234567890123.1234) and time like '" + strTime_s + "%') and time like '" + strTime_d + "%'";
                        oComm.ExecuteNonQuery();
                        
                    }
                }
                oComm.CommandText = "delete FROM Tempdata where (F1 = -1234567890123.1234 or F2 = -1234567890123.1234 or F3 = -1234567890123.1234 or F4 = -1234567890123.1234 or F5 = -1234567890123.1234 or F6 = -1234567890123.1234 or F7 = -1234567890123.1234 or F8 = -1234567890123.1234 or F9 = -1234567890123.1234 or F10 = -1234567890123.1234 or F11 = -1234567890123.1234 or F12 = -1234567890123.1234 or F13 = -1234567890123.1234 or F14 = -1234567890123.1234 or F15 = -1234567890123.1234) and time like '" + strTime_s + "%'";
                oComm.ExecuteNonQuery();
                //计算进度条
                MainForm.nProValue += (int)Math.Ceiling(Convert.ToDouble(100 / (double)files.Length));
                ((TipReFresher)trf).CurTip();
            }
            this.oConn.Close();
        }

        /// <summary>
        /// 计算流量自忙时
        /// </summary>
        public void CalculateBusy_Flow()
        {

            this.oConn.Open();
            this.oConn.ChangeDatabase("Predilatation");
            SqlCommand oComm = this.oConn.CreateCommand();
            try
            {
                //删除临时表
                DelTempTable("Flowbusy");

                //清空流量自忙时标识
                oComm.CommandText = "update tempdata set flowbusy = ''";
                oComm.ExecuteNonQuery();

                //创建临时表
                oComm.CommandText = "select cellname,time,derived1,derived2,derived3,derived4,maxrate into Flowbusy " +
                                    "from tempdata a," +
                                    "(select b.cellname cellnamen, max(b.derived1) derived1n,SUBSTRING(time,1,10) timen " +
                                    "from tempdata b where b.derived1 > 0 " +
                                    "group by b.cellname," +
                                    "SUBSTRING(time,1,10)) c " +
                                    "where a.cellname = c.cellnamen " +
                                    "and a.derived1 = c.derived1n " +
                                    "and SUBSTRING(time,1,10)=c.timen";
                oComm.ExecuteNonQuery();

                //删除冗余数据
                DelFlowData();

                //修改流量自忙时标识
                oComm.CommandText = "update tempdata set flowbusy = '流量自忙时' where exists(select 1 from Flowbusy b where tempdata.cellname = b.cellname and tempdata.time = b.time)";
                oComm.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw e;
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
        private void DelFlowData()
        {
            List<string[]> lstTemp = new List<string[]>();
            SqlCommand oComm = this.oConn.CreateCommand();
            oComm.CommandText = "select cellname,time,derived1,derived2,derived3,derived4,maxrate " +
                                "from Flowbusy a, " +
                                "(select b.cellname cellnamen,SUBSTRING(time,1,10) timen " +
                                "from Flowbusy b " +
                                "group by b.cellname," +
                                "SUBSTRING(time,1,10) having COUNT(SUBSTRING(time,1,10))>1) c " +
                                "where a.cellname = c.cellnamen " +
                                "and SUBSTRING(time,1,10)=c.timen order by cellname,SUBSTRING(time,1,10),maxrate desc";
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
                oComm.CommandText = "delete from Flowbusy where cellname = '"+DelList[i][0]+"' and time = '"+ DelList[i][1] +"'";
                oComm.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 计算利用率自忙时
        /// </summary>
        public void CalculateBusy_Utilizaerate()
        {
            this.oConn.Open();
            this.oConn.ChangeDatabase("Predilatation");
            SqlCommand oComm = this.oConn.CreateCommand();
            try
            {
                //删除临时表
                DelTempTable("Utilizaerate");

                //清空利用率自忙时标识
                oComm.CommandText = "update tempdata set Utilizaerate = ''";
                oComm.ExecuteNonQuery();

                //创建临时表
                oComm.CommandText = "select cellname,time,derived1,derived2,derived3,derived4,MaxRate into Utilizaerate " +
                                    "from tempdata a, " +
                                    "(select b.cellname cellnamen, max(b.MaxRate) maxraten,SUBSTRING(time,1,10) timen " +
                                    "from tempdata b where b.MaxRate > 0 " +
                                    "group by b.cellname, " +
                                    "SUBSTRING(time,1,10)) c " +
                                    "where a.cellname = c.cellnamen " +
                                    "and a.maxrate = c.maxraten " +
                                    "and SUBSTRING(time,1,10)=c.timen";
                oComm.ExecuteNonQuery();

                //删除利用率自忙时冗余数据
                DelUtilizaerateData();

                //修改利用率自忙时标识
                oComm.CommandText = "update tempdata set Utilizaerate = '利用率自忙时' where exists(select 1 from Utilizaerate b where tempdata.cellname = b.cellname and tempdata.time = b.time)";
                oComm.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw e;
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
        private void DelUtilizaerateData()
        {
            List<string[]> lstTemp = new List<string[]>();
            SqlCommand oComm = this.oConn.CreateCommand();
            oComm.CommandText = "select cellname,time,derived1,derived2,derived3,derived4,MaxRate " +
                                "from Utilizaerate a, " +
                                "(select b.cellname cellnamen, max(b.MaxRate) maxraten,SUBSTRING(time,1,10) timen " +
                                "from Utilizaerate b where b.MaxRate > 0 " +
                                "group by b.cellname, " +
                                "SUBSTRING(time,1,10) having COUNT(SUBSTRING(time,1,10))>1) c " +
                                "where a.cellname = c.cellnamen " +
                                "and SUBSTRING(time,1,10)=c.timen " +
                                "order by cellname,SUBSTRING(time,1,10),derived1 desc";
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
                oComm.CommandText = "delete from Utilizaerate where cellname = '" + DelList[i][0] + "' and time = '" + DelList[i][1] + "'";
                oComm.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 三个数取最大
        /// </summary>
        /// <param name="dValue1"></param>
        /// <param name="dValue2"></param>
        /// <param name="dValue3"></param>
        /// <returns></returns>
        private double GetMax(double dValue1,double dValue2,double dValue3)
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

    }
}
