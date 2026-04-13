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
    public class DosageFormInfoDAO : ReturnData
    {

        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();
        public List<DosageFormInfoBEL> GetDosageFormList()
        {
            string Qry = "SELECT DOSAGE_FORM_CODE,DOSAGE_FORM_NAME,STATUS from DOSAGE_FORM_INFO";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<DosageFormInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new DosageFormInfoBEL
                    {
                        DosageFormCode = row["DOSAGE_FORM_CODE"].ToString(),
                        DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                        Status          = row["STATUS"].ToString()

                    }).ToList();
            return item;
        }
        public bool SaveUpdate(DosageFormInfoBEL master,string userId)
        {
            try
            {
                String setBy = userId;
                 string setOn = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                String updateBy = userId;
                String updateDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
 
                string Qry = "";

                if (master.DosageFormCode == null || master.DosageFormCode == "")
                {
                    //I for Insert  
                    MaxID = idGenerated.getMAXID("DOSAGE_FORM_INFO", "DOSAGE_FORM_CODE", "fm0000");
                    IUMode = "I";

                    Qry = "Insert into DOSAGE_FORM_INFO(DOSAGE_FORM_CODE,DOSAGE_FORM_NAME,STATUS,SET_BY,SET_ON) Values('" + MaxID + "','" + master.DosageFormName + "','" + master.Status + "','" + setBy + "', TO_DATE('" + setOn + "','dd/MM/yyyy HH24:mi:ss'))";
                }
                else
                {
                    //U for update
                    MaxID = master.DosageFormCode;
                    IUMode = "U";

                    Qry = "Update DOSAGE_FORM_INFO set DOSAGE_FORM_NAME='" + master.DosageFormName + "', STATUS='" + master.Status + "', UPDATE_BY='" + updateBy + "', UPDATE_DATE= TO_DATE('" + updateDate + "','dd/MM/yyyy HH24:mi:ss')  Where DOSAGE_FORM_CODE='" + master.DosageFormCode + "'";
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
