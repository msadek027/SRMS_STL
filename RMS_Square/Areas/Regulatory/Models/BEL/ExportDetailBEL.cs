using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ExportDetailBEL
    {
        public long ID { get; set; }
        public long ExpId { get; set; }
        public string ItemName { get; set; }
        public string BrandName { get; set; }
        public string GenAndStrength { get; set; }
        public string DossageForm { get; set; }
        public string PackSize { get; set; }
        public string DarNo { get; set; }
        public string ExportBrandName { get; set; }
        public string ExportPackSize { get; set; }
        public string ExportCountry { get; set; }
        public string Quantity { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
       
    }
}