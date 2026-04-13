using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class GmpCertificateInfoBEL
    {
        public long ID { get; set; }
        public string SlNo { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string LicenseNo { get; set; }
        public string RevisionNo { get; set; }
        public string SubmissionType { get; set; }
        public string SubmissionDate { get; set; }
        public string InspectionDate { get; set; }
        public string ApprovalDate { get; set; }
        public string ValidUpto { get; set; }
        public string RevisionDate { get; set; }
        public string AlarmDays { get; set; }
        public int DateDiff { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public string UpdateBy { get; set; }
        public string UpdatedDate { get; set; }
        public virtual ICollection<DocumentFileInfoBEL> FileDetail { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}