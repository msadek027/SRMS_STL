using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RMS_Square.DAL.Gateway
{
    public class ReturnData
    {
        public string MaxID { get; set; }
        public long ReturnMaxID { get; set; }
        public string MaxCode { get; set; }
        public string IUMode { get; set; }
        public string Msg { get; set; }
        public string RefNo { get; set; }
        //public DataTable dt {get;set;}
       

    }
}