using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using RMS_Square.Areas.SA.Models.BEL;
using RMS_Square.DAL.Gateway;
using RMS_Square.Universal.Gateway;
using Systems.Universal;
using System.Data.OracleClient;

namespace RMS_Square.Areas.SA.Models.DAL.DAO
{
    public class RoleDAO :ReturnData
    {
        DBConnection dbConn = new DBConnection();
        // SaHelper saHelper = new SaHelper();
        DBHelper saHelper = new DBHelper();

        public Boolean SaveUpdate(RoleBEL roleBEL)
        {
            try
            {
                string Qry = "Select MAX(RoleID) ID from Sa_Role";
                roleBEL.RoleID = (roleBEL.RoleID == "" || roleBEL.RoleID == null)? "0" : roleBEL.RoleID;
                var tuple = saHelper.ProcedureExecuteTFn3(dbConn.SAConnStrReader(), Qry, "Sa_Role_SSP", "p_RoleID", "p_RoleName", "p_IsActive", roleBEL.RoleID, roleBEL.RoleName, roleBEL.IsActive.ToString());

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
        
        public bool DeleteExecute(RoleBEL roleBEL)
        {
            string Qry = " Delete from Sa_Role Where RoleID='" + roleBEL.RoleID + "'";
            if (saHelper.ExecuteFn(dbConn.SAConnStrReader(), Qry))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public List<RoleBEL> GetRoleList()
        {
            string Qry = "Select RoleID,RoleName,IsActive From Sa_Role Where RoleID >='" + HttpContext.Current.Session["RoleID"].ToString() + "'";
            DataTable dt = saHelper.DataTableFn(dbConn.SAConnStrReader(), Qry);
            List<RoleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new RoleBEL
                    {
                        RoleID = row["RoleID"].ToString(),
                        RoleName = row["RoleName"].ToString(),
                        IsActive =Convert.ToBoolean(row["IsActive"].ToString())

                    }).ToList();
            return item;
        }

        public List<RoleBEL> GetRoleInSoftwareModuleMappingList()
        {
            string Qry = "Select distinct a.RoleID,b.RoleName from Sa_RoleInSM a,Sa_Role b Where a.RoleID=b.RoleID and upper(a.IsActive)=upper('true') and a.RoleID >='" + HttpContext.Current.Session["RoleID"].ToString() + "' Order by a.RoleID";
            DataTable dt = saHelper.DataTableFn(dbConn.SAConnStrReader(), Qry);
            List<RoleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new RoleBEL
                    {
                        RoleID = row["RoleID"].ToString(),
                        RoleName = row["RoleName"].ToString(),                        

                    }).ToList();
            return item;
        }
    }
}