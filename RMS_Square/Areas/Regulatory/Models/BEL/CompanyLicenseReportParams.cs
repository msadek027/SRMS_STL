using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class CompanyLicenseReportParams
    {
        public string CompanyCode { get; set; }
        public string CompanyUnitCode { get; set; }
        public string CompanyUnitName { get; set; }
        public string LicenseNo { get; set; }
        public string SubmissionType { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public string SubFrom { get; set; }
        public string SubTo { get; set; }
        public string AlarmDays { get; set; }
        public string DocStatus { get; set; }  // Uploaded / Pending / Expired
        public string ReportType { get; set; }  // PDF / EXCEL
    }

    public class CompanyLicenseReportResult
    {
        public decimal CLID { get; set; }
        public string CompLicenseSlNo { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyUnitCode { get; set; }
        public string CompanyUnitName { get; set; }

        public string LicenseNo { get; set; }
        public string CompLicenseName { get; set; }
        public string RevisionNo { get; set; }
        public string SubmissionType { get; set; }
        public string SubmissionDate { get; set; }
        public string InspectionDate { get; set; }
        public string ApprovalDate { get; set; }
        public string ValidUpto { get; set; }
        public string Details { get; set; }
        public string ResDept1 { get; set; }
        public string ResDept2 { get; set; }
        public string DocumentStatus { get; set; }  // Uploaded / Pending / Expired

        // File info
        public long FileID { get; set; }
        public string FileCode { get; set; }
        public string FileName { get; set; }
        public decimal DaysLeft { get; set; }
        public string FileExtension { get; set; }
        public string FileUrl { get; set; }
        public bool CanPreview { get; set; }
    }
}