using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.DAL.Gateway;
using RMS_Square.Universal.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.DAO
{
    public class CompanyInfoDAO : ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();
        public List<CompanyInfoBEL> GetCompanyList()
        {
            string Qry = "SELECT COMPANY_CODE,COMPANY_NAME,ADDRESS,LICENSE_NO,CONTACT_NO,EMAIL_ID,FACILITY,LICENSE_NO from COMPANY_INFO";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<CompanyInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new CompanyInfoBEL
                    {
                        CompanyCode = row["COMPANY_CODE"].ToString(),
                        CompanyName = row["COMPANY_NAME"].ToString(),
                        Address = row["ADDRESS"].ToString(),
                        ContactNo = row["CONTACT_NO"].ToString(),
                        EmailId = row["EMAIL_ID"].ToString(),
                        Facility = row["FACILITY"].ToString(),
                        LicenseNo = row["LICENSE_NO"].ToString()
                    }).ToList();
            return item;
        }
        public bool SaveUpdate(CompanyInfoBEL master, string userId)
        {
            try
            {
                string Qry = "";
                string setOndate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                if (master.CompanyCode == null || master.CompanyCode == "")
                {//I for Insert  
                    MaxID = idGenerated.getMAXID("COMPANY_INFO", "COMPANY_CODE", "fm0000");
                    IUMode = "I";
                    Qry = "Insert into COMPANY_INFO(COMPANY_CODE,COMPANY_NAME,ADDRESS,LICENSE_NO,CONTACT_NO,EMAIL_ID,FACILITY, SET_BY,SET_ON) Values('" + MaxID + "','" + master.CompanyName + "','" + master.Address + "','"+master.LicenseNo+"','" + master.ContactNo + "' ,'" + master.EmailId + "' ,'" + master.Facility + "' ,'" + userId + "',TO_DATE('" + setOndate + "','dd/MM/yyyy HH24:mi:ss'))";
                }
                else
                {//U for Insert
                    MaxID = master.CompanyCode;
                    IUMode = "U";
                    Qry = "Update COMPANY_INFO set COMPANY_NAME='" + master.CompanyName + "',ADDRESS='" + master.Address + "',LICENSE_NO='"+master.LicenseNo+"',CONTACT_NO='" + master.ContactNo + "',EMAIL_ID='" + master.EmailId + "' ,FACILITY='" + master.Facility + "' , UPDATE_BY ='" + userId + "', UPDATE_DATE=TO_DATE('" + setOndate + "','dd/MM/yyyy HH24:mi:ss') Where COMPANY_CODE='" + master.CompanyCode + "'";
                }
                if (dbHelper.CmdExecute(dbConn.SAConnStrReader(), Qry))
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