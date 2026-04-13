using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class PriceFixComBEL
    {
        public long ID { get; set; }
        public string SlNo { get; set; }
        public string MeetingDate { get; set; }
        public string MeetingNo { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string ProductCode { get; set; }
        public string SAPProductCode { get; set; }
        public string ProductSlNo { get; set; }
        public string RevisionDate { get; set; }
        public string ReceivedDate { get; set; }
        public string GenericCode { get; set; }
        public string PackSizeName { get; set; }
        public string DosageFormName { get; set; }
        public string BrandName { get; set; }       
        public string PriceSubmissionDate { get; set; }
        public string ExistingPrice { get; set; }
        public string ProposedPrice { get; set; }       
        public string DGDAMRP { get; set; }
        public string PFCMRP { get; set; }
        public string ApprovedMRP { get; set; }
        public string GovtFixedHighSelling { get; set; }
        public string Remarks { get; set; }
        public virtual ICollection<DocumentFileInfoBEL> FileDetail { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}