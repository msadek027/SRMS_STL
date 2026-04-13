using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ExportInfoBEL
    {
        public long ID { get; set; }
        public string SlNo { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string LicenseNo { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }

        public string ExportingCountry { get; set; }
        public string ReceivedDate { get; set; }
        public string SubmissionDate { get; set; }
        public string ApprovalDate { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public virtual ICollection<ExportDetailBEL> ExportDetail { get; set; }

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

        public string ProposedBy { get; set; }

       
    }
}