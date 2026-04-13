using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class NocDetailInfo
    {
        public long ID { get; set; }
        public long NDID { get; set; }
        public string ItemSlNo { get; set; }
        public string ItemName { get; set; }
        public string ItemQty { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public string UpdateBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}