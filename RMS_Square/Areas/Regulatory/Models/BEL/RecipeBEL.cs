using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class RecipeBEL
    {
        public long ID { get; set; }
        public long RecipeId { get; set; }
        public string SlNo { get; set; }
        public string RevisionNo { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string LicenseNo { get; set; }
        public string ProductCode { get; set; }
        public string SAPProductCode { get; set; }
        public string ProductCategory { get; set; }
        public string ProductSpecification { get; set; }
        public string BrandName { get; set; }
        public string GenAndStrength { get; set; }
        public string DosageFormName { get; set; }
        public string PackSizeName { get; set; }
        public string IntroducedInBD { get; set; }
        public string MeetingType { get; set; }
        public string ManufacturerType { get; set; }
        public string DccNo { get; set; }
        public string ReceivedDate { get; set; }
        public string ProposalDate { get; set; }
        public string MeetingRemarks { get; set; }
        public string CountryOfOrigin { get; set; }
        public string ManufacturerName { get; set; }
        public string ImportProposalDate { get; set; }
        public string ImportSubmissionDate { get; set; }
        public string ImportRemarks { get; set; }
        public string ApvSubmissionDate { get; set; }
        public string ApvMeetingDate { get; set; }
        public string ApvApprovalDate { get; set; }
        public string ApvValidUptoDate { get; set; }
        public string ApvDateOfExtension { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApvRemarks { get; set; }
        public string InspectionDate { get; set; }
        public string InspectionRemarks { get; set; }
        public string AlarmDays { get; set; }
        public string ProposedBy { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public string UpdateBy { get; set; }
        public string UpdatedDate { get; set; }
        public virtual ICollection<DocumentFileInfoBEL> FileDetail { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ChooseOption { get; set; }
        public int DateDiff { get; set; }
    }
}