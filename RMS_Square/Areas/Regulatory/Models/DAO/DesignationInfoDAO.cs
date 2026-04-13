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
    public class DesignationInfoDAO : ReturnData
    {

        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();
        public List<DesignationInfoBEL> GetDesignationList()
        {
            string Qry = "SELECT DESIGNATION_CODE,DESIGNATION_NAME,STATUS from DESIGNATION_INFO";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<DesignationInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new DesignationInfoBEL
                    {
                        DesignationCode = row["DESIGNATION_CODE"].ToString(),
                        DesignationName = row["DESIGNATION_NAME"].ToString(),
                        Status          = row["STATUS"].ToString()

                    }).ToList();
            return item;
        }
        public bool SaveUpdate(DesignationInfoBEL master,string userId)
        {
            try
            {
                String setBy = userId;
                 string setOn = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                String updateBy = userId;
                String updateDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
 
                string Qry = "";

                if (master.DesignationCode == null || master.DesignationCode == "")
                {
                    //I for Insert  
                    MaxID = idGenerated.getMAXID("DESIGNATION_INFO", "DESIGNATION_CODE", "fm0000");
                    IUMode = "I";

                    Qry = "Insert into DESIGNATION_INFO(DESIGNATION_CODE,DESIGNATION_NAME,STATUS,SET_BY,SET_ON) Values('" + MaxID + "','" + master.DesignationName + "','" + master.Status + "','" + setBy + "', TO_DATE('" + setOn + "','dd/MM/yyyy HH24:mi:ss'))";
                }
                else
                {
                    //U for Insert
                    MaxID = master.DesignationCode;
                    IUMode = "U";

                    Qry = "Update DESIGNATION_INFO set DESIGNATION_NAME='" + master.DesignationName + "', STATUS='" + master.Status + "', UPDATE_BY='" + updateBy + "', UPDATE_DATE= TO_DATE('" + updateDate + "','dd/MM/yyyy HH24:mi:ss')  Where DESIGNATION_CODE='" + master.DesignationCode + "'";
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