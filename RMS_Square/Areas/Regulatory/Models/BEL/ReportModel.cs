using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ReportModel 
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string AlarmDays { get; set; }
        public string ReportName { get; set; }
        public string ReportType { get; set; }
        public string CompanyCode { get; set; }
        public string ProductCode { get; set; }
        public string BrandName { get; set; }
        public string GenericStrength { get; set; }
        
        public string SubmissionType { get; set; }
        public string MeetingType { get; set; }
        public string ManufacturerType { get; set; }
        public string ChooseOption { get; set; }
        public string ProductSpecification { get; set; }
        public string ReportFormat { get; set; }
        public string PriceChangeType{ get; set; }
        public string PriceType { get; set; }
        public string PriceCategory { get; set; }
        public string ProductCodeList { get; set; }
        public string ExportCounty { get; set; }
        public string ItemName { get; set; }
        public string InvoiceNo { get; set; }
        public string Department { get; set; }
        public string StateStatus { get; set; }
        
    }
}