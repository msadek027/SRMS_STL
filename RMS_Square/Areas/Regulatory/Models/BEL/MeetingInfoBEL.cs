using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class MeetingInfoBEL
    {
        public long ID { get; set; }
        public string SlNo { get; set; }
        public string MeetingYear { get; set; }
        
        public string MeetingSubject { get; set; }
        public string MeetingType { get; set; }
        public string MeetingDate { get; set; }
        public string Remarks { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public virtual ICollection<DocumentFileInfoBEL> FileDetail { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}