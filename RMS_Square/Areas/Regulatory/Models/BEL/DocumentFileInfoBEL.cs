using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class DocumentFileInfoBEL
    {
        public string FileId { get; set; }
        public string RefNo { get; set; }
        public string FileCode { get; set; }
        public string FileName { get; set; }
        public string Extention { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
        public string RefLevel1 { get; set; }
        public string RefLevel2 { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
    }
}