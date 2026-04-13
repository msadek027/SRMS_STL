using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.DAL.Gateway;
using RMS_Square.Universal.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;


namespace RMS_Square.Areas.Regulatory.Models.DAO
{
    public class EmployeeInfoDAO : ReturnData
    {

        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();

        public List<EmployeeInfoBEL> GetEmployeeList()
        {
            var selectQuery = new StringBuilder();

            selectQuery.Append("SELECT ID,SLNO,EMPLOYEE_CODE,EMPLOYEE_NAME,DESIGNATION_CODE,FN_DESIGNATION_NAME(DESIGNATION_CODE) DESIGNATION_NAME,");
            selectQuery.Append("LTRIM(RTRIM(DEPARTMENT_CODE)) DEPARTMENT_CODE,FN_DEPARTMENT_NAME(LTRIM(RTRIM(DEPARTMENT_CODE))) DEPARTMENT_NAME,COMPANY_CODE,FN_COMPANY_NAME(COMPANY_CODE) COMPANY_NAME,");
            selectQuery.Append("LAST_QUALIFICATION,TO_CHAR(DATE_OF_JOINING,'dd/MM/yyyy') DATE_OF_JOINING,TOTAL_EXPERIENCE_YR,CONTACT_NO,EMAIL_ID,STATUS FROM EMPLOYEE_INFO");

            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), selectQuery.ToString());
            List<EmployeeInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new EmployeeInfoBEL
                    {
                        ID                = Convert.ToInt64( row["ID"]),
                        SlNo              = row["SLNO"].ToString(),
                        EmployeeCode      = row["EMPLOYEE_CODE"].ToString(),
                        EmployeeName      = row["EMPLOYEE_NAME"].ToString(),
                        DesignationCode   = row["DESIGNATION_CODE"].ToString(),
                        DesignationName   = row["DESIGNATION_NAME"].ToString(),
                        DepartmentCode    = row["DEPARTMENT_CODE"].ToString(),
                        DepartmentName    = row["DEPARTMENT_NAME"].ToString(),
                        CompanyCode       = row["COMPANY_CODE"].ToString(),
                        CompanyName       = row["COMPANY_NAME"].ToString(),
                        LastQualification = row["LAST_QUALIFICATION"].ToString(),
                        DateOfJoining     = row["DATE_OF_JOINING"].ToString(), 
                        TotalExperienceYr = row["TOTAL_EXPERIENCE_YR"].ToString(),
                        ContactNo         = row["CONTACT_NO"].ToString(),
                        EmailId           = row["EMAIL_ID"].ToString(),
                        Status            = row["STATUS"].ToString()

                    }).ToList();
            return item;
        }

        public bool SaveUpdate(EmployeeInfoBEL master, string userId)
        {
            try
            {
                String setBy = userId;
                string setOn = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                String updateBy = userId;
                String updateDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                var query = new StringBuilder();
                if (master.ID > 0)
                {
                    //U for Insert
                    // MaxID = master.EmployeeCode;
                    IUMode = "U";

                    query.Append(" UPDATE EMPLOYEE_INFO SET EMPLOYEE_NAME='" + master.EmployeeName + "', EMPLOYEE_CODE='" + master.EmployeeCode + "', DESIGNATION_CODE='" + master.DesignationCode + "', DEPARTMENT_CODE='" + master.DepartmentCode);
                    query.Append(" ', COMPANY_CODE='" + master.CompanyCode + "', LAST_QUALIFICATION='" + master.LastQualification + "', DATE_OF_JOINING=" + "TO_DATE('" + master.DateOfJoining + "','dd/MM/yyyy')");
                    query.Append(", TOTAL_EXPERIENCE_YR='" + master.TotalExperienceYr + "', CONTACT_NO='" + master.ContactNo + "', EMAIL_ID='" + master.EmailId);
                    query.Append(" ', STATUS='" + master.Status + "', UPDATE_BY='" + updateBy + "', UPDATE_DATE= TO_DATE('" + updateDate + "','dd/MM/yyyy HH24:mi:ss')  Where ID='" + master.ID + "'");
                }
                else
                {
                    ReturnMaxID = idGenerated.getMAXSL("EMPLOYEE_INFO", "ID");
                    MaxID = idGenerated.getMAXID("EMPLOYEE_INFO", "SLNO", "fm0000");
                    //I for Insert  
                    //MaxID = master.EmployeeCode;
                    IUMode = "I";
                    query.Append(" INSERT INTO EMPLOYEE_INFO(ID,SLNO,EMPLOYEE_CODE, EMPLOYEE_NAME, DESIGNATION_CODE, DEPARTMENT_CODE, COMPANY_CODE, LAST_QUALIFICATION,");
                    query.Append(" DATE_OF_JOINING,TOTAL_EXPERIENCE_YR, CONTACT_NO, EMAIL_ID, STATUS,SET_BY, SET_ON) ");
                    query.Append(" VALUES(" + ReturnMaxID + ",'" + MaxID + "','" + master.EmployeeCode + "','" + master.EmployeeName + "','" + master.DesignationCode + "','" + master.DepartmentCode + "','");
                    query.Append(master.CompanyCode + "','" + master.LastQualification + "'," + "TO_DATE('" + master.DateOfJoining + "','dd/MM/yyyy')" + ",'" + master.TotalExperienceYr + "','");
                    query.Append(master.ContactNo + "','" + master.EmailId + "','" + master.Status + "','" + setBy + "'," + "TO_DATE('" + setOn + "','dd/MM/yyyy HH24:mi:ss'))");
                 }

                if (dbHelper.CmdExecute(dbConn.SAConnStrReader(), query.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception errorException)
            {
                throw errorException;
            }
        }
    }
}