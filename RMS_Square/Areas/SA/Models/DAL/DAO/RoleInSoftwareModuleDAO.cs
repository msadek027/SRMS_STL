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
    public class RoleInSoftwareModuleDAO:ReturnData
    {
        DBConnection dbConn = new DBConnection();
        // SaHelper saHelper = new SaHelper();
        DBHelper saHelper = new DBHelper();
        public Boolean SaveUpdate(RoleInSoftwareModuleBEL master)
        {
            bool IsTrue = false;
            if (master != null)
            {
                if (master.detailsList != null)
                {
                    foreach (RoleInSoftwareModuleBEL details in master.detailsList)
                    {
                        IsTrue = false;
                        string Qry = "Select MAX(RoleID) ID from Sa_RoleInSM";
                        var tuple = saHelper.ProcedureExecuteTFn2(dbConn.SAConnStrReader(), Qry, "Sa_RoleIn_SM_SP", "p_RoleSMID", "p_IsActive", details.RoleID + details.SoftwareID + details.ModuleID, details.IsActive.ToString());
                        if (tuple.Item1)
                        {
                            MaxID = tuple.Item2;
                            IUMode = tuple.Item3;
                            IsTrue= true;
                        }
                        else
                        {
                            IsTrue = false;
                        }
                    }
                }
            }
            return IsTrue;
            }
    }
}