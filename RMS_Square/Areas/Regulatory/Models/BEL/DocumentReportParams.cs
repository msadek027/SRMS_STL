using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class DocumentReportParams
    {
        public string CompanyCode { get; set; }
        public string ProductCode { get; set; }
        public string AuthorityType { get; set; }
        public string DocumentType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class DocumentReportVM
    {
        public string FileID { get; set; }
        public string FileCode { get; set; }
        public string Extention { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string DocumentName { get; set; } // RefNo from DB

        // Registration Info
        public string CompanyName { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string AuthorityName { get; set; }
        public string UploadDate { get; set; }
    }
}