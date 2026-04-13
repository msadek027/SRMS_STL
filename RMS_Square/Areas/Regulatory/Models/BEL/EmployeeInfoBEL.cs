using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.BEL
{
    public class EmployeeInfoBEL
    {
        public long ID { get; set; }
        public string SlNo { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string LastQualification { get; set; }
        public string DateOfJoining { get; set; }
        public string TotalExperienceYr { get; set; }
        public string ContactNo { get; set; }
        public string EmailId { get; set; }
        public string Status { get; set; }
        public string SetBy { get; set; }
        public string SetOn { get; set; }
        public string UpdateBy { get; set; }
        public string UpdatedDate { get; set; }

    }

}