using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ProductInfoBEL
    {
        public string ProductCode { get; set; }
        public string SAPProductCode { get; set; }
        public string GenericCode { get; set; }
        public string GenAndStrength { get; set; }
        public string GenericName { get; set; }
        public string StrengthCode { get; set; }
        public string StrengthName { get; set; }
        public string DosageFormCode { get; set; }
        public string DosageFormName { get; set; }
        public string PackSizeCode { get; set; }
        public string PackSizeName { get; set; }
        public string BrandName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string LicenseNo { get; set; }
        public string ProductCategory { get; set; }
        public string TherapeuticClassCode { get; set; }
        public string TherapeuticClassName { get; set; }
        public string ProductSpecification { get; set; }
        public string IntroducedInBD { get; set; }
        public string ManufacturingType { get; set; }
        public string ProductTypeCode { get; set; }
        public string ProductTypeName { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string YearMonth { get; set; }

       // public string SapProductCode { get; set; }
        public string GenericStrength { get; set; }
        public string DosageForm { get; set; }
        public string PackSize { get; set; }
        public string DarNo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string LastDays { get; set; }
        

    }
}