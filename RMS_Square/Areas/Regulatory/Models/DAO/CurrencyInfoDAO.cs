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
    public class CurrencyInfoDAO : ReturnData
    {

        DBConnection dbConn = new DBConnection();
        //private readonly DBHelper _dbHelper = new DBHelper();
        DBHelper dbHelper = new DBHelper();
       
        IDGenerated idGenerated = new IDGenerated();
        //private readonly DBHelper _dbHelper = new DBHelper();
        string code = string.Empty;
        long mxSl = 0;
        public List<CurrencyInfoBEL> GetCurrencyList()
        {
            string Qry = "SELECT CURRENCY_CODE,CURRENCY_NAME,SHORT_NAME,STATUS from CURRENCY_INFO";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<CurrencyInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new CurrencyInfoBEL
                    {
                        CurrencyCode = row["CURRENCY_CODE"].ToString(),
                        CurrencyName = row["CURRENCY_NAME"].ToString(),
                        ShortName = row["SHORT_NAME"].ToString(),
                        Status = row["STATUS"].ToString()

                    }).ToList();
            return item;
        }


        public bool SaveUpdate(CurrencyInfoBEL master)
        {
            try
            {
                string Qry = "";
                if (master.CurrencyCode == null || master.CurrencyCode == "")
                {//I for Insert  
                    MaxID = idGenerated.getMAXID("CURRENCY_INFO", "CURRENCY_CODE", "fm000");
                    IUMode = "I";
                     Qry = "INSERT INTO CURRENCY_INFO (CURRENCY_CODE,CURRENCY_NAME,SHORT_NAME,STATUS) VALUES('" + MaxID + "', '" + master.CurrencyName + "', '" + master.ShortName + "','" + master.Status + "')";
                }
                else
                {//U for Insert
                    MaxID = master.CurrencyCode;
                    IUMode = "U";
                    Qry = "UPDATE CURRENCY_INFO SET CURRENCY_NAME = '" + master.CurrencyName + "',SHORT_NAME = '" + master.ShortName + "', STATUS = '" + master.Status + "' WHERE CURRENCY_CODE = '" + master.CurrencyCode + "'";
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