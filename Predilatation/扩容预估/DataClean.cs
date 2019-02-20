using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOX;
using DAO = Microsoft.Office.Interop.Access.Dao;

namespace Predilatation
{
    public class DataClean
    {

        /// <summary>
        /// 写入Access
        /// </summary>
        /// <param name="path">csv文件路径</param>
        public static void InsertTable(string path, string strDbpath, string strSelPath)
        {
            string strTableName = Path.GetFileNameWithoutExtension(path);
            DAO.DBEngine dbEngine = new DAO.DBEngine();
            DAO.Database db = dbEngine.OpenDatabase(strDbpath);
            string sql = "SELECT * INTO [" + strTableName + "] FROM [Text;FMT=Delimited;DATABASE=" + strSelPath + ";HDR=No].[" + strTableName + ".csv]";
            db.Execute(sql);

            //删除多余行
            sql = "delete from " + strTableName + " where F2 is null or  F2 = '时间'";
            db.Execute(sql);

            //增加主键
            sql = "alter table " + strTableName + " add PRIMARY KEY(F1,F2)";
            db.Execute(sql);

            //增加列
            for (int i = 1; i < 7; i++)
            {
                if (i == 6)
                {
                    sql = "alter table " + strTableName + " add [Busy" + i.ToString() + "] varchar(100) NULL";
                }
                else
                {
                    sql = "alter table " + strTableName + " add [Derived" + i.ToString() + "] varchar(100) NULL";
                }
                
                db.Execute(sql);
            }
            db.Close();
        }

        /// <summary>
        /// 创建access数据库
        /// </summary>
        /// <param name="filePath">数据库文件的全路径，如 D:\\NewDb.mdb</param>
        public static bool CreateAccessDb(string filePath)
        {

            if (!File.Exists(filePath))
            {
                Catalog catalog = new Catalog();
                catalog.Create("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Jet OLEDB:Engine Type=5");
            }
            return true;
        }

        public static void DelInvalidData(string[] files, string strDbpath)
        {
            DAO.DBEngine dbEngine = new DAO.DBEngine();
            DAO.Database db = dbEngine.OpenDatabase(strDbpath);
            string strTbname_s = "";
            string strTbname_d = "";
            string sql = "";

            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetExtension(files[i]).ToLower() != ".csv")
                    continue;
                strTbname_s = Path.GetFileNameWithoutExtension(files[i]);
                
                for (int j = 0; j < files.Length; j++)
                {
                    if (Path.GetExtension(files[j]).ToLower() != ".csv")
                        continue;
                    strTbname_d = Path.GetFileNameWithoutExtension(files[j]);
                    if (strTbname_s != strTbname_d)
                    {
                        sql = "delete from " + strTbname_d + " where F1 in (SELECT distinct(F1) FROM " + strTbname_s + " where F3 = '-' or F4 = '-' or F5 = '-' or F6 = '-' or F7 = '-' or F8 = '-' or F9 = '-' or F10 = '-' or F11 = '-' or F12 = '-' or F13 = '-' or F14 = '-' or F15 = '-' or F16 = '-' or F17 = '-')";
                        db.Execute(sql);
                    }
                    
                }
                sql = "delete FROM " + strTbname_s + " where F3 = '-' or F4 = '-' or F5 = '-' or F6 = '-' or F7 = '-' or F8 = '-' or F9 = '-' or F10 = '-' or F11 = '-' or F12 = '-' or F13 = '-' or F14 = '-' or F15 = '-' or F16 = '-' or F17 = '-')";
                db.Execute(sql);
            }
            db.Close();
        }
    }
}
