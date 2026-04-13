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
    public class RoleInFormDAO : ReturnData
    {
        DBConnection dbConn = new DBConnection();
        // SaHelper saHelper = new SaHelper();
        DBHelper saHelper = new DBHelper();
        public List<RoleInFormBEL> GetRoleInFormPermissionList(string RoleID)
        {
            RoleID = (RoleID == "" || RoleID == null) ? HttpContext.Current.Session["RoleID"].ToString() : RoleID;

            string Qry =" Select distinct  p.SoftwareID,p.SoftwareName, p.ModuleID,p.ModuleName, p.FormID,p.FormName,p.FormURL "+
                        " ,NVL(e.ViewPermission,'false')  ViewPermission,NVL(e.SavePermission,'false')  SavePermission,NVL(e.EditPermission,'false') EditPermission,NVL(e.DeletePermission,'false') DeletePermission," +
                        " NVL(e.PrintPermission,'false')  PrintPermission,e.SetOn" +
                        " from ("+
                        " (Select  a.RoleID,a.SoftwareID,d.SoftwareName, a.ModuleID,c.ModuleName, b.FormID,b.FormName,b.FormURL from Sa_RoleInSM a,Sa_Form  b,Sa_Module  c,Sa_Software d  "+
                        " Where upper(a.IsActive)=upper('true') and a.RoleID='" + RoleID + "' and a.SoftwareID||a.ModuleID||b.FormID=d.SoftwareID||c.ModuleID||b.FormID and d.SoftwareID='02'and a.ModuleID='01' and b.FormID  between '100' and '149') "+
                        " Union all " +
                         " (Select  a.RoleID,a.SoftwareID,d.SoftwareName, a.ModuleID,c.ModuleName, b.FormID,b.FormName,b.FormURL from Sa_RoleInSM a,Sa_Form  b,Sa_Module  c,Sa_Software d  " +
                        " Where upper(a.IsActive)=upper('true') and a.RoleID='" + RoleID + "' and a.SoftwareID||a.ModuleID||b.FormID=d.SoftwareID||c.ModuleID||b.FormID and d.SoftwareID='02'and a.ModuleID='02' and b.FormID  between '150' and '179') " +
                        " Union all " +
                        " (Select  a.RoleID,a.SoftwareID,d.SoftwareName, a.ModuleID,c.ModuleName, b.FormID,b.FormName,b.FormURL  from Sa_RoleInSM a,Sa_Form  b,Sa_Module  c, Sa_Software d "+
                        " Where upper(a.IsActive)=upper('true') and a.RoleID='" + RoleID + "' and a.SoftwareID||a.ModuleID||b.FormID=d.SoftwareID||c.ModuleID||b.FormID  and d.SoftwareID='01'and a.ModuleID='01' and b.FormID  between '001' and '049') " +
                        " Union all " +
                        " (Select  a.RoleID,a.SoftwareID,d.SoftwareName, a.ModuleID,c.ModuleName, b.FormID,b.FormName,b.FormURL  from Sa_RoleInSM a,Sa_Form  b,Sa_Module  c, Sa_Software d " +
                        " Where upper(a.IsActive)=upper('true') and a.RoleID='" + RoleID + "' and a.SoftwareID||a.ModuleID||b.FormID=d.SoftwareID||c.ModuleID||b.FormID  and d.SoftwareID='01'and a.ModuleID='02' and b.FormID  between '050' and '079') " +
                        " )  p " +
                        " LEFT JOIN Sa_RoleInFormP  e ON p.RoleID||p.SoftwareID||p.ModuleID||p.FormID=e.RoleID||e.SoftwareID||e.ModuleID||e.FormID   "+
                        " Order by p.SoftwareID";


            DataTable dt = saHelper.DataTableFn(dbConn.SAConnStrReader(),Qry);
            List<RoleInFormBEL> item;
            //using lamdaexpression
            item = (from DataRow row in dt.Rows
                    select new RoleInFormBEL
                    {
                        SoftwareID = row["SoftwareID"].ToString(),
                        SoftwareName = row["SoftwareName"].ToString(),
                        ModuleID = row["ModuleID"].ToString(),
                        ModuleName = row["ModuleName"].ToString(),
                        FormID = row["FormID"].ToString(),
                        FormName = row["FormName"].ToString(),
                        FormURL = row["FormURL"].ToString(),
                        ViewPermission = Convert.ToBoolean(row["ViewPermission"].ToString()),
                        SavePermission = Convert.ToBoolean(row["SavePermission"].ToString()),
                        EditPermission = Convert.ToBoolean(row["EditPermission"].ToString()),
                        DeletePermission = Convert.ToBoolean(row["DeletePermission"].ToString()),
                        PrintPermission = Convert.ToBoolean(row["PrintPermission"].ToString()),

                    }).ToList();

            return item;
        }

        public bool SaveUpdate(RoleInFormBEL master)
        {
            bool IsTrue = false;
            if (master != null)
            {
                if (master.detailsList != null)
                {
                    foreach (RoleInFormBEL details in master.detailsList)
                    {
                        IsTrue = false;
                        string VSEDP = Convert.ToString(details.ViewPermission == true ? 1 : 0) + Convert.ToString(details.SavePermission == true ? 1 : 0) + Convert.ToString(details.EditPermission == true ? 1 : 0) + Convert.ToString(details.DeletePermission == true ? 1 : 0) + Convert.ToString(details.PrintPermission == true ? 1 : 0);
                        string UserID = HttpContext.Current.Session["UserID"].ToString();
                        string Qry = "Select MAX(RoleID) ID from Sa_RoleInFormP";
                        var tuple = saHelper.ProcedureExecuteTFn3(dbConn.SAConnStrReader(), Qry, "Sa_RoleIn_SMP_SP", "P_RSMFormID", "p_VSEDP", "p_SetOn", details.RoleID + details.SoftwareID + details.ModuleID + details.FormID, VSEDP, UserID);
                        if (tuple.Item1)
                        {
                            MaxID = tuple.Item2;
                            IUMode = tuple.Item3;
                            
                            if (saHelper.ProcedureExecuteFn1(dbConn.SAConnStrReader(), "", "Sa_Menu_SP", "pUserID", UserID))
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