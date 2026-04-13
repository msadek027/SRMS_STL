using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class MarketAuthCertificateBEL
    {
        public long ID { get; set; }
        public string SlNo { get; set; }
        public string RevisionNo { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string ProductCode { get; set; }
        public string SapProductCode { get; set; }
         
        public string GenericStrength { get; set; }
        public string PackSize { get; set; }
        public string DosageForm { get; set; }
        public string DarNo { get; set; }
        public string BrandName { get; set; }
        public string SubmissionDate { get; set; }
        public string ReceiveDate { get; set; }
        public string ApprovalDate { get; set; }
        public string MarketAuthorizationNo { get; set; }
        public string ValiduptoDate { get; set; }
        public string NotificationDay { get; set; }
        public int DateDiff { get; set; }
        public string Remarks { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public string UpdateBy { get; set; }
        public string UpdatedDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ChooseOption { get; set; }
        
        public string GenAndStrength { get; set; }
        public string AlarmDays { get; set; }
        public string ProductCategory { get; set; }
        

    }
}