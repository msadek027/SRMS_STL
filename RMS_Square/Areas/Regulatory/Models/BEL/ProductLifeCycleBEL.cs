using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ProductLifeCycleBEL
    {

        public string ProductCode { get; set; }
        public string SAPProductCode { get; set; }
        public string BrandName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string GenericStrength { get; set; }
        public string StrengthName { get; set; }
        public string DosageFormName { get; set; }
        public string PackSize { get; set; }
        public string ProductCategory { get; set; }
        public string TherapeuticClassName { get; set; }
        public string ProductSpecification { get; set; }
        public string IntroducedInBD { get; set; }

        public string RecipeSubmissionType { get; set; }
        public string RecipeSubmissionDate { get; set; }
        public string RecipeReceivedDate { get; set; }
        public string RecipeProposalDate { get; set; }
        public string RecipeMeetingDate { get; set; }
        public string RecipeValidUptoDate { get; set; }
        public string RecipeApprovalDate { get; set; }

        public string DarNo { get; set; }
        public string DtlSubmissionDate { get; set; }
        public string DtlApprovalDate { get; set; }
        public string AnnexReceivedDate { get; set; }
        public string AnnexSubmissionDate { get; set; }
        public string AnnexValidUptoDate { get; set; }
        public string InclusionDate { get; set; }
        public string AnnexApprovalDate { get; set; }

        public string PriceReceivedDate { get; set; }
        public string PriceSubmissionDate { get; set; }
        public string PriceApprovalDate { get; set; }
        public string PricePerUnit { get; set; }

        public string MacSubmissionDate { get; set; }
        public string MacReceivedDate { get; set; }
        public string MacApprovalDate { get; set; }
        public string MacValidUptoDate { get; set; }
        public string LastDays { get; set; }
        
    }
}