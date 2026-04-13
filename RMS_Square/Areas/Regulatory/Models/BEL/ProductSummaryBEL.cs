using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class ProductSummaryBEL
    {
        public string BrandName { get; set; }
        public string GenericCode { get; set; }
        public string ProductCode { get; set; }
        public string CompanyCode { get; set; }
        public string ProductSpecification { get; set; }
        public string RCPID { get; set; }
        public string RCPSubmissionDate { get; set; }
        public string RCPApprovalDate { get; set; }
        public string RCPValidUpto { get; set; }
        public string RCPRemarks { get; set; }
        public string RCPDownload { get; set; }
        public string REGID { get; set; }
        public string DTLSubmissionDate { get; set; }
        public string DTLApprovalDate { get; set; }
        public string DTLRemarks { get; set; }
        public string DTLDownload { get; set; }
        public string REGSubmissionDate { get; set; }
        public string REGApprovalDate { get; set; }
        public string REGValidUptoDate { get; set; }
        public string REGDarNo { get; set; }
        public string REGRemarks { get; set; }
        public string REGDownload { get; set; }
        public string PRCID { get; set; }
        public string PRCSubmissionDate { get; set; }
        public string PRCApprovalDate { get; set; }
        public string PRCApprovedPrice { get; set; }
        public string PRCRemarks { get; set; }
        public string PRCDownload { get; set; }
        public string MRKID { get; set; }
        public string MRKNumber { get; set; }
        public string MRKSubmissionDate { get; set; }
        public string MRKApprovalDate { get; set; }
        public string MRKValidUpto { get; set; }
        public string MRKRemarks { get; set; }        
        public string MRKDownload { get; set; }
        public string AMDID { get; set; }
        public string AMDSubmissionDate { get; set; }        
        public string AMDApprovalDate { get; set; }
        public string AMDRemarks { get; set; }
        public string AMDDownload { get; set; }
    }
}