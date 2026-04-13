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
    public class GenericInfoDAO : ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();

        public List<GenericInfoBEL> GetGenericList()
        {
            string Qry = "SELECT GENERIC_CODE,GENERIC_NAME,STATUS from GENERIC_INFO";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<GenericInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new GenericInfoBEL
                    {
                        GenericCode = row["GENERIC_CODE"].ToString(),
                        GenericName = row["GENERIC_NAME"].ToString(),
                        Status      = row["STATUS"].ToString()

                    }).ToList();
            return item;
        }

        public bool SaveUpdate(GenericInfoBEL master, string userId)
        {
            try
            {

                String setBy = userId;
                string setOn = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                String updateBy = userId;
                String updateDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                string Qry = "";
                if (master.GenericCode == null || master.GenericCode == "")
                {//I for Insert  
                    MaxID = idGenerated.getMAXID("GENERIC_INFO", "GENERIC_CODE", "fm0000");
                    IUMode = "I";
                    Qry = "INSERT INTO GENERIC_INFO (GENERIC_CODE,GENERIC_NAME,STATUS,SET_BY,SET_ON) VALUES('" + MaxID + "', '" + master.GenericName + "' , '" + master.Status + "','" + setBy + "', TO_DATE('" + setOn + "','dd/MM/yyyy HH24:mi:ss'))";
                }
                else
                {//U for update
                    MaxID = master.GenericCode;
                    IUMode = "U";
                    Qry = "UPDATE GENERIC_INFO SET GENERIC_NAME = '" + master.GenericName + "',STATUS = '" + master.Status + "', UPDATE_BY='" + updateBy + "', UPDATE_DATE= TO_DATE('" + updateDate + "','dd/MM/yyyy HH24:mi:ss') WHERE GENERIC_CODE = '" + master.GenericCode + "'";
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