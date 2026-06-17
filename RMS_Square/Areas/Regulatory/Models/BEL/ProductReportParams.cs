using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ProductReportParams
    {
        public string ReportType { get; set; } // HTML, PDF, EXCEL
        public string ReportName { get; set; } // ProductLifeCycleInfo, ProductLifeCycleInfoSinglePage, MasterProductInfo
        public string CompanyCode { get; set; }
        public string ProductCodeList { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
       // public string CompanyCode { get; set; }   // COMPANY_INFO level
        public string CompanyUnitCode { get; set; }   // COMPANY_UNIT_INFO level (optional)
       // public string ProductCodeList { get; set; }   // "'P001','P002'" — IN clause format
        public string AuthorityType { get; set; }
        public string DocumentType { get; set; }   // AuthorityLicenseName এ map হয়
        public string ValidFrom { get; set; }   // dd/mm/yyyy
        public string ValidTo { get; set; }
        public string AlarmDays { get; set; }
        public string SubFrom { get; set; }   // Submission date from
        public string SubTo { get; set; }   // Submission date to
        public string DocStatus { get; set; }   // Uploaded | Pending | Expired
        //public string ReportType { get; set; }   // PDF | EXCEL (Export এর জন্য)
    }
    public class ProductReportResultVM
    {
        public long AnnexId { get; set; }
        public string AnnexureNo { get; set; }
        public string DarNo { get; set; }
        // Company
        public string CompanyCode { get; set; }
        public decimal DaysLeft { get; set; }  // ← new
        public string CompanyName { get; set; }
        public string CompanyUnitCode { get; set; }
        public string CompanyUnitName { get; set; }
        public string LicenseNo { get; set; }
        // Product
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string GenericStrength { get; set; }
        public string PackSizeName { get; set; }
        public string Variant { get; set; }
        public string ProductCategory { get; set; }
        public string ProductSpec { get; set; }
        // Authority
        public string AuthorityType { get; set; }
        public string AuthorityName { get; set; }
        public string AuthorityLicenseNo { get; set; }
        public string AuthorityLicenseName { get; set; }
        // Dates
        public string SubmissionDate { get; set; }
        public string ExpireDate { get; set; }
        public string ReceiveDate { get; set; }
        public string InclusionDate { get; set; }
        public string ValidUptoDate { get; set; }
        public string RenewalDate { get; set; }
        public string Remarks { get; set; }
        // Document / File
        public long FileID { get; set; }  // ← NEW: Download URL এর জন্য
        public string FileCode { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public string DocRefNo { get; set; }
        public string FileUrl { get; set; }
        public bool CanPreview { get; set; }  // ← NEW: Preview button show/hide
        public string DocumentStatus { get; set; }  // Uploaded | Pending | Expired
       // public string FileCode { get; set; }
      //  public string FileName { get; set; }
       // public string FileExtension { get; set; }
     //   public string DocRefNo { get; set; }
       // public string FileUrl { get; set; }  // Preview / Download URL
       // public string CompanyCode { get; set; }
       // public string CompanyName { get; set; }
       // public string ProductCode { get; set; }
        public string SAPProductCode { get; set; }
       // public string BrandName { get; set; }
       // public string GenericStrength { get; set; }
        //public string PackSize { get; set; }
        public string DosageForm { get; set; }
       // public string DarNo { get; set; }
        //public string DocumentStatus { get; set; }
       //public string Remarks { get; set; }
    }
}