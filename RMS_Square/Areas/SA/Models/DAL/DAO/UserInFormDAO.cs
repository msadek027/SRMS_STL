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
    public class UserInFormDAO:ReturnData
    {
        DBConnection dbConn=new DBConnection();
        // SaHelper saHelper = new SaHelper();
        DBHelper saHelper = new DBHelper();
        public bool SaveUpdate(UserInFormBEL master)
        {
            bool IsTrue = false;
            if (master != null)
            {
                if (master.detailsList != null)
                {
                    foreach (UserInFormBEL details in master.detailsList)
                    {
                        IsTrue = false;
                        string VSEDP = Convert.ToString(details.ViewPermission == true ? 1 : 0) + Convert.ToString(details.SavePermission == true ? 1 : 0) + Convert.ToString(details.EditPermission == true ? 1 : 0) + Convert.ToString(details.DeletePermission == true ? 1 : 0) + Convert.ToString(details.PrintPermission == true ? 1 : 0);
                                           

                        string Qry = "Select MAX(RoleID) ID from Sa_RoleInFormP";
                        var tuple = saHelper.ProcedureExecuteTFn3(dbConn.SAConnStrReader(), Qry, "Sa_RoleIn_SMP_SP", "P_RSMFormID", "p_VSEDP", "p_SetOn", details.RoleID + details.SoftwareID + details.ModuleID + details.FormID, VSEDP, master.UserID);
                        if (tuple.Item1)
                        {                       
                            MaxID = tuple.Item2;
                            IUMode = tuple.Item3;
                            DataTable dt = saHelper.DataTableRefCursorFn1(dbConn.SAConnStrReader(), "FNC_SHOW_MENU", "pUserID", master.UserID);
                            if (dt.Rows.Count>0)
                            {
                                IsTrue = true;
                            }                            
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