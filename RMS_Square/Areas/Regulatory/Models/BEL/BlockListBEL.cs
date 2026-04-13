using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class BlockListBEL
    {
        public long ID { get; set; }
        public string SlNo { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string ProposedBy { get; set; }
        public string RevisionNo { get; set; }
        public string BLNo { get; set; }
        public string ProposedDate { get; set; }
        public string BlockListDate { get; set; }
        public string ApprovalNo { get; set; }
        public string ApprovalDate { get; set; }
        public string MeetingDate { get; set; }
        public string Remarks { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public string UpdateBy { get; set; }
        public string UpdatedDate { get; set; }
        public virtual ICollection<DocumentFileInfoBEL> FileDetail { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}