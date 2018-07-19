using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Predilatation
{
    public class MainParam
    {
        public ListBox.ObjectCollection lstBu { get; set; }
        public ListBox.ObjectCollection lstBd { get; set; }
        public ListBox.ObjectCollection lstMu { get; set; }
        public ListBox.ObjectCollection lstMd { get; set; }
        public ListBox.ObjectCollection lstSu { get; set; }
        public ListBox.ObjectCollection lstSd { get; set; }

        public string strERAB_B { get; set; }
        public string strcmbERAB_B { get; set; }
        public string strERAB_MS { get; set; }
        public string strERAB_ME { get; set; }
        public string strcmbERAB_MS { get; set; }
        public string strcmbERAB_ME { get; set; }
        public string strERAB_S { get; set; }
        public string strcmbERAB_S { get; set; }
    }
}
