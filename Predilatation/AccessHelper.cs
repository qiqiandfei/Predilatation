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
    public class AccessHelper
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

            //删除垃圾数据
            sql = "delete from " + strTableName + " where F2 is null or  F2 = '时间'";
            db.Execute(sql);

            //增加主键
            sql = "alter table " + strTableName + " add PRIMARY KEY(F1,F2)";
            db.Execute(sql);
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
    }
}
