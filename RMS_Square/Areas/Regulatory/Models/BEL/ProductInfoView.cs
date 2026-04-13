using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ProductInfoView
    {
        public string ProductCode { get; set; }
        public string SAPProductCode { get; set; }
        public string GenericCode { get; set; }
        public string GenAndStrength { get; set; }
        public string GenericStrength { get; set; }
        
        public string GenericName { get; set; }
        public string StrengthCode { get; set; }
        public string StrengthName { get; set; }
        public string DosageFormCode { get; set; }
        public string DosageFormName { get; set; }
        public string DosageForm { get; set; }
        
        public string PackSize { get; set; }
        public string PackSizeName { get; set; }
        public string DarNo { get; set; }
        public string BrandName { get; set; }
        
    }
}