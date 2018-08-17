using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predilatation
{
   public abstract class AbstractResObj:ResObj
    {
        //抽象方法由选择的扩容标准重写
        public abstract void GetResObj(ResObj res, string strStdTyp, string strTyp, SqlConnection sqlconn,
            string strTableName, DataTable dtSense);
    }
}
