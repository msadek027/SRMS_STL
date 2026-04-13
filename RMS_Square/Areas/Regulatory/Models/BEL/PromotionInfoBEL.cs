using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class PromotionInfoBEL
    {

        public long ID { get; set; }
        public long AnnexID { get; set; }
        public string SlNo { get; set; }
        public string RevisionNo { get; set; }
        public string ProductCode { get; set; }
        public string SapProductCode { get; set; }
        public string GenericStrength { get; set; }
        public string PackSize { get; set; }
        public string DosageForm { get; set; }
        public string DarNo { get; set; }
        public string BrandName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string AnnexureNo { get; set; }
        public string PromotionalItem { get; set; }
        public string ProposalDate { get; set; }
        public string SubmissionDate { get; set; }
        public string ApprovalDate { get; set; }
        public string Remarks { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public string UpdateBy { get; set; }
        public string UpdatedDate { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ChooseOption { get; set; }

    }
}