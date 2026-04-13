using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ImportProductRegistrationBEL
    {
        public long ID { get; set; }
        public string SlNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string SAPProductCode { get; set; }
        public string RevisionNo { get; set; }
        public string RevisionDate { get; set; }
        public string GenericCode { get; set; }
        public string GenAndStrength { get; set; }
        public string PackSizeCode { get; set; }
        public string PackSizeName { get; set; }
        public string DosageFormCode { get; set; }
        public string DosageFormName { get; set; }

        public string CountryCode { get; set; }
        public string CountryName { get; set; }

        public string BrandName { get; set; }

        public string ManufacturerName { get; set; }
        public string SupplierName { get; set; }
        public string ProposalDate { get; set; }
        public string SubmissionDate { get; set; }
        public string RegistrationDate { get; set; }
        public string RegistrationNo { get; set; }
        //  public string ApprovalDate { get; set; }
        public string ValidUpto { get; set; }
        public string NotificationDays { get; set; }
        public int DateDiff { get; set; }
        public string Remarks { get; set; }
        public virtual ICollection<DocumentFileInfoBEL> FileDetail { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string RecipeId { get; set; }

    }

}