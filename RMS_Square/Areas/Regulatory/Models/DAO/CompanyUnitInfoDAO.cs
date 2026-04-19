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
    public class CompanyUnitInfoDAO : ReturnData
    {

        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();

        public List<CompanyUnitInfoBEL> GetCompanyUnitInfoList()
        {
            string Qry = "SELECT COMPANY_UNIT_CODE,COMPANY_UNIT_NAME,COMPANY_CODE from COMPANY_UNIT_INFO";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<CompanyUnitInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new CompanyUnitInfoBEL
                    {
                        CompanyCode = row["COMPANY_CODE"].ToString(),
                        CompanyUnitCode = row["COMPANY_UNIT_CODE"].ToString(),
                        CompanyUnitName = row["COMPANY_UNIT_NAME"].ToString()

                    }).ToList();
            return item;
        }

        public bool SaveUpdate(CompanyUnitInfoBEL master, string userId)
        {
            try
            {
                if (master == null)
                    throw new ArgumentNullException(nameof(master));

                // CompanyCode required for insert or when changing company on update
                if (string.IsNullOrWhiteSpace(master.CompanyCode))
                    throw new ArgumentException("CompanyCode is required.", nameof(master.CompanyCode));

                string Qry = "";

                if (string.IsNullOrWhiteSpace(master.CompanyUnitCode))
                {
                    // INSERT path: generate new COMPANY_UNIT_CODE for the selected company
                    string prefix = master.CompanyCode.Trim();

                    string qMax = "SELECT MAX(COMPANY_UNIT_CODE) AS MAXID FROM COMPANY_UNIT_INFO WHERE COMPANY_UNIT_CODE LIKE '" + prefix + "%'";
                    DataTable dtMax = dbHelper.GetDataTable(dbConn.SAConnStrReader(), qMax);

                    string nextSuffix = "01";
                    if (dtMax != null && dtMax.Rows.Count > 0 && dtMax.Rows[0]["MAXID"] != DBNull.Value)
                    {
                        string maxId = dtMax.Rows[0]["MAXID"].ToString();
                        string currentSuffix = string.Empty;
                        if (maxId.Length > prefix.Length)
                            currentSuffix = maxId.Substring(prefix.Length);

                        int num;
                        if (int.TryParse(currentSuffix, out num))
                        {
                            num++;
                        }
                        else
                        {
                            num = 1;
                        }

                        int pad = Math.Max(2, currentSuffix.Length > 0 ? currentSuffix.Length : 2);
                        nextSuffix = num.ToString().PadLeft(pad, '0');
                    }

                    MaxID = prefix + nextSuffix;
                    IUMode = "I";

                    Qry = "INSERT INTO COMPANY_UNIT_INFO (COMPANY_UNIT_CODE, COMPANY_UNIT_NAME, COMPANY_CODE) " +
                          "VALUES ('" + MaxID + "', '" + master.CompanyUnitName + "', '" + master.CompanyCode + "')";
                }
                else
                {
                    // UPDATE path
                    string oldCode = master.CompanyUnitCode.Trim();
                    MaxID = oldCode;
                    IUMode = "U";

                    // Get existing company code for the record
                    string qOld = "SELECT COMPANY_CODE FROM COMPANY_UNIT_INFO WHERE COMPANY_UNIT_CODE = '" + oldCode + "'";
                    DataTable dtOld = dbHelper.GetDataTable(dbConn.SAConnStrReader(), qOld);

                    string existingCompanyCode = null;
                    if (dtOld != null && dtOld.Rows.Count > 0 && dtOld.Rows[0]["COMPANY_CODE"] != DBNull.Value)
                        existingCompanyCode = dtOld.Rows[0]["COMPANY_CODE"].ToString().Trim();

                    // If company changed, generate a new COMPANY_UNIT_CODE under new company and update the PK value
                    if (!string.IsNullOrEmpty(existingCompanyCode) && !string.Equals(existingCompanyCode, master.CompanyCode, StringComparison.Ordinal))
                    {
                        string prefix = master.CompanyCode.Trim();

                        string qMax = "SELECT MAX(COMPANY_UNIT_CODE) AS MAXID FROM COMPANY_UNIT_INFO WHERE COMPANY_UNIT_CODE LIKE '" + prefix + "%'";
                        DataTable dtMax = dbHelper.GetDataTable(dbConn.SAConnStrReader(), qMax);

                        string nextSuffix = "01";
                        if (dtMax != null && dtMax.Rows.Count > 0 && dtMax.Rows[0]["MAXID"] != DBNull.Value)
                        {
                            string maxId = dtMax.Rows[0]["MAXID"].ToString();
                            string currentSuffix = string.Empty;
                            if (maxId.Length > prefix.Length)
                                currentSuffix = maxId.Substring(prefix.Length);

                            int num;
                            if (int.TryParse(currentSuffix, out num))
                            {
                                num++;
                            }
                            else
                            {
                                num = 1;
                            }

                            int pad = Math.Max(2, currentSuffix.Length > 0 ? currentSuffix.Length : 2);
                            nextSuffix = num.ToString().PadLeft(pad, '0');
                        }

                        string newCode = prefix + nextSuffix;

                        // Update COMPANY_UNIT_CODE to newCode and other fields
                        Qry = "UPDATE COMPANY_UNIT_INFO SET COMPANY_UNIT_CODE = '" + newCode + "', COMPANY_UNIT_NAME = '" + master.CompanyUnitName + "', COMPANY_CODE = '" + master.CompanyCode + "' " +
                              "WHERE COMPANY_UNIT_CODE = '" + oldCode + "'";

                        MaxID = newCode; // report new id to caller
                    }
                    else
                    {
                        // company not changed -> simple update
                        Qry = "UPDATE COMPANY_UNIT_INFO SET COMPANY_UNIT_NAME = '" + master.CompanyUnitName + "', COMPANY_CODE = '" + master.CompanyCode + "' " +
                              "WHERE COMPANY_UNIT_CODE = '" + oldCode + "'";
                        MaxID = oldCode;
                    }
                }

                return dbHelper.CmdExecute(dbConn.SAConnStrReader(), Qry);
            }
            catch (Exception errorException)
            {
                throw errorException;
            }
        }

        public List<CompanyInfoBEL> GetCompanyList()
        {
            string qry = "SELECT COMPANY_CODE, COMPANY_NAME FROM COMPANY_INFO ORDER BY COMPANY_NAME";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), qry);

            var list = (from DataRow row in dt.Rows
                        select new CompanyInfoBEL
                        {
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString()
                        }).ToList();

            return list;
        }
    }
}