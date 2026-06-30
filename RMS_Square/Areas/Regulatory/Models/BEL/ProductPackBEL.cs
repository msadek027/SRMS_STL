using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ProductPackBEL
    {
        public string PackCode { get; set; }
        public string VariantCode { get; set; }
        public string ProductCode { get; set; }
        public string SAPProductCode { get; set; }
        public string PackSizeName { get; set; }  // "10x10", "100ml" ইত্যাদি
        public string Status { get; set; }
        public string Remarks { get; set; }
        public bool IsDeleted { get; set; }
    }
}