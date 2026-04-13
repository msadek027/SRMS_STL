using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class NarcoticLicenseBEL
    {
        public long ID { get; set; }
        public long DetailID { get; set; }
        public string SlNo { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string LicenseNo { get; set; }
        public string LicenseType { get; set; }
        public string RevisionNo { get; set; }
        public string SubmissionType { get; set; }
        public string SubmissionDate { get; set; }
        public string InspectionDate { get; set; }
        public string ApprovalDate { get; set; }
        public string ValidUpto { get; set; }
        public string RevisionDate { get; set; }
        public string AlarmDays { get; set; }
        public int DateDiff { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public string UpdateBy { get; set; }
        public string UpdatedDate { get; set; }

        //public long ID { get; set; }
        public long NLID { get; set; }
        public string GenericCode { get; set; }
        public string GenericName { get; set; }
        public string AnnualQuota { get; set; }
        public string ImportQtyPerYr { get; set; }
        public string ImportQtyCurYr { get; set; }
        public string ImportPurpose { get; set; }
        public string PermitApprovalDate { get; set; }
        public string ImportQty { get; set; }
        public string ImportDate { get; set; }
        //public string SetBy { get; set; }
        //public string SetOn { get; set; }
        //public string UpdateBy { get; set; }
        //public string UpdatedDate { get; set; }

        public string SubDGDA { get; set; }

        public string ApproveDGDA { get; set; }

        public string SubNercotic { get; set; }

        public string BrandName { get; set; }

        public string RecSentNarc { get; set; }

        public string InsRptRcvNarc { get; set; }

        public string SubInsRptNhq { get; set; }

        public string SubInsRptNarc { get; set; }

        public string FinalImpPermit { get; set; }

        public string RMImpQty { get; set; }

        public string SendToPPIC { get; set; }

        public string PPICLocalApp { get; set; }

        public string InsSampleCall { get; set; }

        public string SampleRec { get; set; }

        public string RPTDispatch { get; set; }

        public string PPICSent { get; set; }

        public string PPICApplyNarc { get; set; }

        public string RptForwrdRcv { get; set; }

        public string DivToDnc { get; set; }

        public string FinalPermit { get; set; }

        public string DeliverToIMD { get; set; }

        public virtual ICollection<DocumentFileInfoBEL> FileDetail { get; set; }
        public List<NarcoticLicenseDetailBEL> NLDetail { get; set; }
        
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string DtlRemarks { get; set; }
    }
}