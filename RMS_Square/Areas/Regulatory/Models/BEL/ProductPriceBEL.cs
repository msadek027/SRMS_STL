using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ProductPriceBEL
    {
        public long ID { get; set; }
        public string SlNo { get; set; }
        public string RevisionNo { get; set; }
        public long AnnexId { get; set; }
        public string AnnexureNo { get; set; }
        public string ProductCode { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string LicenseNo { get; set; }
        public string SAPProductCode { get; set; }
        public string GenAndStrength { get; set; }
        public string DosageFormName { get; set; }
        public string PackSizeName { get; set; }
        public string BrandName { get; set; }
        public string DarNo { get; set; }
        public string PriceProposalDate { get; set; }
        public string ReceivedDate { get; set; }
        public string PriceSubmissionDate { get; set; }
        public string ApprovalDate { get; set; }
        public string PriceType { get; set; }
        public string PriceCategory { get; set; }
        public string PriceChangeType { get; set; }
        public string UnitPrice { get; set; }
        public string HighestPrice { get; set; }
        public string PriceRemarks { get; set; }
        public string PriceProposedBy { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public string UpdateBy { get; set; }
        public string UpdatedDate { get; set; }
        public virtual ICollection<DocumentFileInfoBEL> FileDetail { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ChooseOption { get; set; }
        public string LaunchingDate { get; set; }
    }
}