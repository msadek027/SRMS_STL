using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using RMS_Square.Areas.SA.Models.BEL;
using RMS_Square.DAL.Gateway;
using RMS_Square.Universal.Gateway;
using Systems.Universal;

namespace RMS_Square.Areas.SA.Models.DAL.DAO
{
    public class ModuleDAO:ReturnData
    {
        DBConnection dbConn = new DBConnection();
        // SaHelper saHelper = new SaHelper();
        DBHelper saHelper = new DBHelper();
        public Boolean SaveUpdate(ModuleBEL moduleBEL)
        {
            try
            {
                string Qry = "Select MAX(ModuleID) ID from Sa_Module";
                moduleBEL.ModuleID = (moduleBEL.ModuleID == "" || moduleBEL.ModuleID == null) ? "0" : moduleBEL.ModuleID; 
                var tuple = saHelper.ProcedureExecuteTFn3(dbConn.SAConnStrReader(), Qry, "Sa_Module_SSP", "p_ID", "p_Name", "p_IsActive", moduleBEL.ModuleID, moduleBEL.ModuleName, moduleBEL.IsActive.ToString());
                if (tuple.Item1)
                {
                    MaxID = tuple.Item2;
                    IUMode = tuple.Item3;
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

        

        public bool DeleteExecute(ModuleBEL moduleBEL)
        {
            string Qry = " Delete from Sa_Module Where ModuleID='" + moduleBEL.ModuleID + "'";
            if (saHelper.ExecuteFn(dbConn.SAConnStrReader(), Qry))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<ModuleBEL> GetModuleList()
        {
            string Qry = "Select ModuleID,ModuleName,IsActive From Sa_Module ";
            DataTable dt = saHelper.DataTableFn(dbConn.SAConnStrReader(), Qry);
            List<ModuleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new ModuleBEL
                    {
                        ModuleID = row["ModuleID"].ToString(),
                        ModuleName = row["ModuleName"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString())

                    }).ToList();
            return item;
        }
    }
}