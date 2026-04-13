using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class NocInfoBEL
    {
        public long ID { get; set; }
        public long SN { get; set; }
        public string SlNo { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string LicenseNo { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Purpose { get; set; }
        public string RevisionNo { get; set; }
        public string SubmissionDate { get; set; }
        public string ApprovalDate { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public string UpdateBy { get; set; }
        public string UpdatedDate { get; set; }
        public virtual ICollection<NocDetailInfo> NocDetail { get; set; }
        public string ItemName { get; set; }
        public string Quantity { get; set; }
        public string ReceivedDate { get; set; }
        public string ProposedBy { get; set; }
        public string ProposedDepartment { get; set; }
        
    }
}