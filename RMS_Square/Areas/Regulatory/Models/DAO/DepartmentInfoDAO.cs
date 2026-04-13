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
    public class DepartmentInfoDAO : ReturnData
    {
        
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();
        public List<DepartmentInfoBEL> GetDeparmentList()
        {
            string Qry = "SELECT DEPARTMENT_CODE,DEPARTMENT_NAME,STATUS from DEPARTMENT_INFO";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<DepartmentInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new DepartmentInfoBEL
                    {
                        DepartmentCode = row["DEPARTMENT_CODE"].ToString(),
                        DepartmentName = row["DEPARTMENT_NAME"].ToString(),
                        Status = row["STATUS"].ToString()

                    }).ToList();
            return item;
        }
        public bool SaveUpdate(DepartmentInfoBEL master, string userId)
        {
            try
            {
                string Qry = "";
                //string setON = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string setOndate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
               // string setON = DateTime.Now.ToString("dd/MM/yyyy");
               // DateTime setONDT=Convert.ToDateTime(setON);

                if (master.DepartmentCode == null || master.DepartmentCode == "")
                {//I for Insert  
                    MaxID = idGenerated.getMAXID("DEPARTMENT_INFO", "DEPARTMENT_CODE", "fm0000");
                    IUMode = "I";

                    Qry = "Insert into DEPARTMENT_INFO(DEPARTMENT_CODE,DEPARTMENT_NAME, STATUS, SET_BY,SET_ON) Values('" + MaxID + "','" + master.DepartmentName + "','" + master.Status + "','" + userId + "',TO_DATE('" + setOndate + "','dd/MM/yyyy HH24:mi:ss'))";
                }
                else
                {//U for Insert
                    MaxID = master.DepartmentCode;
                    IUMode = "U";
                    Qry = "Update DEPARTMENT_INFO set DEPARTMENT_NAME='" + master.DepartmentName + "',STATUS='" + master.Status + "' , UPDATE_BY ='" + userId + "', UPDATE_DATE=TO_DATE('" + setOndate + "','dd/MM/yyyy HH24:mi:ss') Where DEPARTMENT_CODE='" + master.DepartmentCode + "'";
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