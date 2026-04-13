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
    public class CountryInfoDAO : ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();
        public List<CountryInfoBEL> GetCountryList()
        {
            string Qry = "SELECT COUNTRY_CODE,COUNTRY_NAME,SHORT_NAME from COUNTRY_INFO";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<CountryInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new CountryInfoBEL
                    {
                        CountryCode = row["COUNTRY_CODE"].ToString(),
                        CountryName = row["COUNTRY_NAME"].ToString(),
                        ShortName = row["SHORT_NAME"].ToString()

                    }).ToList();
            return item;
        }
        public bool SaveUpdate(CountryInfoBEL master)
        {
            try
            {
                string Qry = "";
                if (master.CountryCode == null || master.CountryCode == "")
                {//I for Insert  
                    MaxID = idGenerated.getMAXID("COUNTRY_INFO", "COUNTRY_CODE", "fm0000");
                    IUMode = "I";
                    Qry = "Insert into COUNTRY_INFO(COUNTRY_CODE,COUNTRY_NAME, SHORT_NAME) Values('" + MaxID + "','" + master.CountryName + "','" + master.ShortName + "')";
                }
                else
                {//U for Insert
                    MaxID = master.CountryCode;
                    IUMode = "U";
                    Qry = "Update COUNTRY_INFO set COUNTRY_NAME='" + master.CountryName + "',SHORT_NAME='" + master.ShortName + "' Where COUNTRY_CODE='" + master.CountryCode + "'";
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