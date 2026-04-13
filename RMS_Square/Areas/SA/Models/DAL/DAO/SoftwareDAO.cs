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
    public class SoftwareDAO: ReturnData
    {
        DBConnection dbConn = new DBConnection();
        // SaHelper saHelper = new SaHelper();
        DBHelper saHelper = new DBHelper();
        public bool SaveUpdate(SoftwareBEL master)
        {
            try
            {
               
                string Qry = "Select MAX(SoftwareID) ID from SA_SOFTWARE";
                master.SoftwareID = (master.SoftwareID == "" || master.SoftwareID == null) ? "0" : master.SoftwareID; 
                var tuple = saHelper.ProcedureExecuteTFn4(dbConn.SAConnStrReader(), Qry, "Sa_Software_SSP", "p_ID", "p_Name", "p_ShortName", "p_IsActive", master.SoftwareID, master.SoftwareName, master.SoftwareShortName, master.IsActive.ToString());
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


        public List<SoftwareBEL> GetSoftwareList()
        {
            string Qry = "SELECT SoftwareID,SoftwareName,SoftwareShortName,IsActive from Sa_Software";
            DataTable dt = saHelper.DataTableFn(dbConn.SAConnStrReader(), Qry);
            List<SoftwareBEL> item;

            item = (from DataRow row in dt.Rows
                    select new SoftwareBEL
                    {
                        SoftwareID = row["SoftwareID"].ToString(),
                        SoftwareName = row["SoftwareName"].ToString(),
                        SoftwareShortName = row["SoftwareShortName"].ToString(), 
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString())

                    }).ToList();
            return item;
        }

        //public bool DeleteExecute(SoftwareBEL softwareBEL)
        //{
        //    string Qry = " Delete from Sa_Software Where SoftwareID='" + softwareBEL.ID + "'";
        //    if (dbHelper.CmdExecute(Qry))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

    }
}