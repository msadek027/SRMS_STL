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
        /*public List<CompanyInfoBEL> GetCompanyList()
        {
            string Qry = "SELECT A.COMPANY_CODE,A.COMPANY_NAME,A.ADDRESS,A.LICENSE_NO,A.CONTACT_NO,A.EMAIL_ID,A.FACILITY,A.LICENSE_NO, B.COMPANY_UNIT_CODE,B.COMPANY_UNIT_NAME from COMPANY_INFO A JOIN COMPANY_UNIT_INFO B on A.COMPANY_CODE = B.COMPANY_CODE";
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
        }*/
        public List<CompanyInfoBEL> GetCompanyList()
        {
            // Use LEFT JOIN so companies without units are also returned.
            string Qry = @"
                SELECT 
                    A.COMPANY_CODE,
                    A.COMPANY_NAME,
                    A.ADDRESS AS COMPANY_ADDRESS,
                    A.LICENSE_NO,
                    A.CONTACT_NO,
                    A.EMAIL_ID,
                    A.FACILITY,
                    B.COMPANY_UNIT_CODE,
                    B.COMPANY_UNIT_NAME,
                    B.ADDRESS
                FROM COMPANY_INFO A
                LEFT JOIN COMPANY_UNIT_INFO B ON A.COMPANY_CODE = B.COMPANY_CODE
                ORDER BY A.COMPANY_NAME, B.COMPANY_UNIT_CODE";

            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);

            var companies = new Dictionary<string, CompanyInfoBEL>(StringComparer.OrdinalIgnoreCase);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string companyCode = row["COMPANY_CODE"] == DBNull.Value ? string.Empty : row["COMPANY_CODE"].ToString();
                    if (string.IsNullOrEmpty(companyCode))
                        continue; // skip malformed rows

                    CompanyInfoBEL company;
                    if (!companies.TryGetValue(companyCode, out company))
                    {
                        company = new CompanyInfoBEL
                        {
                            CompanyCode = companyCode,
                            CompanyName = row["COMPANY_NAME"] == DBNull.Value ? string.Empty : row["COMPANY_NAME"].ToString(),
                            Address = row["ADDRESS"] == DBNull.Value ? string.Empty : row["ADDRESS"].ToString(),
                            LicenseNo = row["LICENSE_NO"] == DBNull.Value ? string.Empty : row["LICENSE_NO"].ToString(),
                            ContactNo = row["CONTACT_NO"] == DBNull.Value ? string.Empty : row["CONTACT_NO"].ToString(),
                            EmailId = row["EMAIL_ID"] == DBNull.Value ? string.Empty : row["EMAIL_ID"].ToString(),
                            Facility = row["FACILITY"] == DBNull.Value ? string.Empty : row["FACILITY"].ToString(),
                            Units = new List<CompanyUnitInfoBEL>()
                        };

                        companies[companyCode] = company;
                    }

                    // Add unit if present
                    var unitCodeObj = row["COMPANY_UNIT_CODE"];
                    if (unitCodeObj != DBNull.Value)
                    {
                        string unitCode = unitCodeObj.ToString();
                        if (!string.IsNullOrWhiteSpace(unitCode))
                        {
                            var unit = new CompanyUnitInfoBEL
                            {
                                CompanyUnitCode = unitCode,
                                CompanyUnitName = row["COMPANY_UNIT_NAME"] == DBNull.Value ? string.Empty : row["COMPANY_UNIT_NAME"].ToString(),
                                Address = row["ADDRESS"] == DBNull.Value ? string.Empty : row["ADDRESS"].ToString(),
                                CompanyCode = companyCode
                            };
                            company.Units.Add(unit);
                        }
                    }
                }
            }

            return companies.Values.ToList();
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