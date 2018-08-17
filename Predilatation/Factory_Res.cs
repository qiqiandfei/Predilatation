using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predilatation
{
    class Factory_Res
    {
        //简单工厂 返回抽象类 用于不同算法的实例化
        public static AbstractResObj CreateResObj(string ResTyp)
        {
            AbstractResObj res = null;
            if(ResTyp == "Daily")
                res = new ResObj_Daily();
            if (ResTyp == "Holiday")
                res = new ResObj_Holiday();

            return res;
        }
    }
}
