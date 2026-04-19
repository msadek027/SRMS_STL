using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class CompanyInfoBEL
    {
        public String CompanyCode { get; set; }
        public string CompanyName { get; set;  }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string EmailId { get; set; }
        public string Facility { get; set; }
        public string LicenseNo { get; set; }
        // New: include unit data from COMPANY_UNIT_INFO
        public List<CompanyUnitInfoBEL> Units { get; set; } = new List<CompanyUnitInfoBEL>();

    }
}