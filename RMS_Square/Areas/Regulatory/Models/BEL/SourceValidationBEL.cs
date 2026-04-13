using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class SourceValidationBEL
    {
        public long AnnexId { get; set; }
        public long SN { get; set; }
        public string AnnexureNo { get; set; }
        public long RecipeId { get; set; }
        public string SlNo { get; set; }
        public string AnnexRevisionNo { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string LicenseNo { get; set; }
        public string StateStatus { get; set; }
        public string ProductCode { get; set; }
        public string SAPProductCode { get; set; }
        public string GenAndStrength { get; set; }
        public string DosageFormName { get; set; }
        public string PackSizeName { get; set; }
        public string ProductCategory { get; set; }
        public string ProductSpecification { get; set; }
        public string BrandName { get; set; }
        public string DarNo { get; set; }
        public string AnnexStatus { get; set; }

        public string DtlReceivedDate { get; set; }
        public string DtlSubmissionDate { get; set; }
        public string DtlApprovalDate { get; set; }
        public string DtlRemarks { get; set; }

        public string ProposalDate { get; set; }
        public string ReceivedDate { get; set; }
        public string SubmissionDate { get; set; }
        public string InclusionDate { get; set; }
        public string RenewalDate { get; set; }
        public string ValidUptoDate { get; set; }

        public string Remarks { get; set; }
        public string AlarmDays { get; set; }
        public int DateDiff { get; set; }
        public string ProposedBy { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public string UpdateBy { get; set; }
        public string UpdatedDate { get; set; }
        public virtual ICollection<DocumentFileInfoBEL> FileDetail { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ChooseOption { get; set; }
        public string CountryOfOrigin { get; internal set; }
        public string ManufacturerName { get; internal set; }
    }
}