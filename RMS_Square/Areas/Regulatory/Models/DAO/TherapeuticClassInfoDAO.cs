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
    public class TherapeuticClassInfoDAO :  ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();

        public List<TherapeuticClassInfoBEL> GetTherapeuticClassList()
        {
            string Qry = "SELECT THERAPEUTIC_CLASS_CODE,THERAPEUTIC_CLASS_NAME,STATUS from THERAPEUTIC_CLASS_INFO";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
           
            List<TherapeuticClassInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new TherapeuticClassInfoBEL
                    {
                        TherapeuticClassCode = row["THERAPEUTIC_CLASS_CODE"].ToString(),
                        TherapeuticClassName = row["THERAPEUTIC_CLASS_NAME"].ToString(),
                        Status = row["STATUS"].ToString()

                    }).ToList();
            return item;
        }

        public bool SaveUpdate(TherapeuticClassInfoBEL master, string userId)
        {
            try
            {
                String setBy = userId;
                string setOn = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                String updateBy = userId;
                String updateDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                string Qry = "";

                if (master.TherapeuticClassCode == null || master.TherapeuticClassCode == "")
                {
                    //I for Insert  
                    MaxID = idGenerated.getMAXID("THERAPEUTIC_CLASS_INFO", "THERAPEUTIC_CLASS_CODE", "fm0000");
                    IUMode = "I";

                    Qry = "Insert into THERAPEUTIC_CLASS_INFO(THERAPEUTIC_CLASS_CODE,THERAPEUTIC_CLASS_NAME,STATUS,SET_BY,SET_ON) Values('" + MaxID + "','" + master.TherapeuticClassName + "','" + master.Status + "','" + setBy + "', TO_DATE('" + setOn + "','dd/MM/yyyy HH24:mi:ss'))";
                }
                else
                {
                    //U for Insert
                    MaxID = master.TherapeuticClassCode;
                    IUMode = "U";

                    Qry = "Update THERAPEUTIC_CLASS_INFO set THERAPEUTIC_CLASS_NAME='" + master.TherapeuticClassName + "', STATUS='" + master.Status + "', UPDATE_BY='" + updateBy + "', UPDATE_DATE= TO_DATE('" + updateDate + "','dd/MM/yyyy HH24:mi:ss')  Where THERAPEUTIC_CLASS_CODE='" + master.TherapeuticClassCode + "'";
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