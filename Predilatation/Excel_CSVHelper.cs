using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Aspose.Cells;

namespace Predilatation
{
    public class Excel_CSVHelper
    {
        /// <summary>
        /// Excel转DataTable
        /// </summary>
        /// <param name="strexcelpath">Excel文件完整路径</param>
        /// <returns>DataTable</returns>
        public static DataTable ConvertExceltoDt(string strexcelpath)
        {
            try
            {
                Workbook wb = new Workbook(strexcelpath);
                Worksheet sheet = wb.Worksheets[0];
                Cells cell = sheet.Cells;

                DataTable dt = new DataTable();
                
                //数据导入DataTable
                if (cell.Rows.Count > 0)
                {
                    dt = cell.ExportDataTable(0, 0, cell.MaxRow + 1, cell.MaxColumn + 1, true);
                }
                return dt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public static DataTable CovertCSVtoDt(string strCSVpath)
        {
            try
            {
                DataTable dt = new DataTable();
                //读取文本内容
                string[] filecontent = File.ReadAllLines(strCSVpath);
                //取列
                string[] title = filecontent[0].Split('	');
                
                //添加列
                for (int i = 0; i < title.Length; i++)
                {
                    DataColumn column = new DataColumn();
                    if (!string.IsNullOrEmpty(title[i]))
                    {
                       
                        //去掉"-"," ","("等非法字符
                        string strcolumnname = title[i];
                        if (strcolumnname.Contains("("))
                        {
                            strcolumnname = strcolumnname.Substring(0, strcolumnname.IndexOf('('));
                        }
                        if (strcolumnname.Contains("-"))
                        {
                            strcolumnname = strcolumnname.Replace('-', '_');
                        }
                        if (strcolumnname.Contains(" "))
                        {
                            strcolumnname = strcolumnname.Replace(' ', '_');
                        }
                        if (strcolumnname.Contains("["))
                        {
                            strcolumnname = strcolumnname.Substring(0, strcolumnname.IndexOf('['));
                        }
                        column.ColumnName = strcolumnname;
                        dt.Columns.Add(column);
                    }
                }
                
                //添加内容
                for (int i = 1; i < filecontent.Length; i++)
                {
                    string[] linecontent = filecontent[i].Split('	');
                    DataRow row = dt.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        row[j] = linecontent[j].ToString();
                    }
                    dt.Rows.Add(row);
                }
                return dt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// DataTable转Excel
        /// </summary>
        /// <param name="resdt"></param>
        public static string DataTableToExcel(DataTable resdt)
        {
            string strpath = System.Windows.Forms.Application.StartupPath + "\\扩容结果";
            if (!Directory.Exists(strpath))
                Directory.CreateDirectory(strpath);

            string tradeTime = DateTime.Now.ToString("yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
            string strfilename = resdt.TableName;
            string fullpath = strpath + "\\" + strfilename + tradeTime + ".xlsx";

            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];
            Cells cells = sheet.Cells;

            //标题
            CellsFactory cf = new CellsFactory();
            Style stTitle = cf.CreateStyle();
            stTitle.Font.IsBold = true;
            stTitle.Font.Size = 12;

            //写标题
            for (int i = 0; i < resdt.Columns.Count; i++)
            {
                cells[0, i].PutValue(resdt.Columns[i].ColumnName);
                cells[0, i].SetStyle(stTitle);
            }

            //写内容
            for (int c = 0; c < resdt.Columns.Count; c++)
            {
                for (int r = 0; r < resdt.Rows.Count; r++)
                {
                    cells[r + 1,c].PutValue(Convert.ToString(resdt.Rows[r][c]));
                }
            }

            wb.Save(fullpath);
            return fullpath;
        }
    }
}
