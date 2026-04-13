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
    public class StrengthInfoDAO:ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();

        public List<StrengthInfoBEL> GetStrengthList()
        {
            string Qry = "SELECT STRENGTH_CODE,STRENGTH_NAME,STATUS from STRENGTH_INFO";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<StrengthInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new StrengthInfoBEL
                    {
                        StrengthCode = row["STRENGTH_CODE"].ToString(),
                        StrengthName = row["STRENGTH_NAME"].ToString(),
                        Status = row["STATUS"].ToString()

                    }).ToList();
            return item;
        }

        public bool SaveUpdate(StrengthInfoBEL master, string userId)
        {
            try
            {
                string Qry = "";
                string setOndate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                if (master.StrengthCode == null || master.StrengthCode == "")
                {//I for Insert  
                    MaxID = idGenerated.getMAXID("STRENGTH_INFO", "STRENGTH_CODE", "fm0000");
                    IUMode = "I";

                    Qry = "Insert into STRENGTH_INFO(STRENGTH_CODE,STRENGTH_NAME, STATUS, SET_BY,SET_ON) Values('" + MaxID + "','" + master.StrengthName + "','" + master.Status + "','" + userId + "',TO_DATE('" + setOndate + "','dd/MM/yyyy HH24:mi:ss'))";
                }
                else
                {//U for Insert
                    MaxID = master.StrengthCode;
                    IUMode = "U";
                    Qry = "Update STRENGTH_INFO set STRENGTH_NAME='" + master.StrengthName + "',STATUS='" + master.Status + "' , UPDATE_BY ='" + userId + "', UPDATE_DATE=TO_DATE('" + setOndate + "','dd/MM/yyyy HH24:mi:ss') Where STRENGTH_CODE='" + master.StrengthCode + "'";
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