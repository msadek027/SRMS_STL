using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using RMS_Square.DAL.Gateway;
using RMS_Square.Models;
using Systems.Universal;

namespace RMS_Square.DAL.DAO
{

    public class LoginRegistrationDAO
    {

        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        public List<LoginRegistrationModel> CheckUserCredential()
        {

            HttpContext.Current.Session["Conn"] = dbConn.SAConnStrReader();
            //string Qry = "SELECT UserID,NewPassword,OldPassword FROM  Sa_UserCredential";
            string Qry = "SELECT ur.UserID,ur.RoleID,r.RoleName,ur.EmpID," +
                // " --dbHR.dbo.GetName(ur.EmpID,'ER') EmpName,dbHR.dbo.GetName(ur.EmpID,'EE') SupervisorID,dbHR.dbo.GetName(dbHR.dbo.GetName(ur.EmpID,'EE'),'ER') SupervisorName,dbHR.dbo.GetName(ur.EmpID,'EE d') Designation,dbHR.dbo.GetName(ur.EmpID,'EE ed') EmploymentDate,"+
            " u.NewPassword,u.OldPassword,ur.IsActive FROM Sa_UserInRole ur, Sa_UserCredential u,Sa_Role r Where ur.UserID=u.UserID and ur.RoleID=r.RoleID and upper(ur.IsActive)=upper('true') ";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<LoginRegistrationModel> item;

            item = (from DataRow row in dt.Rows
                    select new LoginRegistrationModel
                    {
                        UserID = row["UserID"].ToString(),
                        Password = row["NewPassword"].ToString(),
                        RoleID = row["RoleID"].ToString(),
                        RoleName = row["RoleName"].ToString(),
                        EmpID =  Convert.ToInt32(row["EmpID"]),
                        //EmpName = row["EmpName"].ToString(),
                        //SupervisorID = row["SupervisorID"].ToString(),
                        //SupervisorName = row["SupervisorName"].ToString(),
                        //Designation = row["Designation"].ToString(),
                        //EmploymentDate = row["EmploymentDate"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString())

                    }).ToList();
            return item;



        }
        public List<LoginRegistrationModel> ValidUserDefaults(string UserID)
        {
            string Qry = "SELECT ur.UserID,ur.RoleID,r.RoleName,ur.EmpID,GetName(ur.EmpID,'EM') EmpName,dbHR.dbo.GetName(ur.EmpID,'EE') SupervisorID,dbHR.dbo.GetName(dbHR.dbo.GetName(ur.EmpID,'EE'),'ER') SupervisorName,u.NewPassword,u.OldPassword,ur.IsActive FROM Sa_UserInRole as ur, Sa_UserCredential as u,Sa_Role r Where ur.UserID=u.UserID and ur.RoleID=r.RoleID and ur.IsActive='1' and ur.UserID='" + UserID + "'";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<LoginRegistrationModel> item;

            item = (from DataRow row in dt.Rows
                    select new LoginRegistrationModel
                    {
                        UserID = row["UserID"].ToString(),
                        Password = row["NewPassword"].ToString(),
                        RoleID = row["RoleID"].ToString(),
                        RoleName = row["RoleName"].ToString(),
                        EmpID =  Convert.ToInt32(row["EmpID"].ToString()),
                        EmpName = row["EmpName"].ToString(),
                        SupervisorID = row["SupervisorID"].ToString(),
                        SupervisorName = row["SupervisorName"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString())

                    }).ToList();
            return item;
        }

        public bool MenuPopulate(string UserID)
        {
            if (dbHelper.ProcedureExecuteFn1(dbConn.SAConnStrReader(), "", "Sa_Menu_SP", "pUserID", UserID))
            {
                return true;
            }
            return false;
        }

        public DataTable UserAccessPermissionList(string userId)
        {
            string qry = string.Format("SELECT FormURL FROM SA_MENU WHERE UserID='{0}'", userId);
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), qry);
            return dt;
        }
        public bool MenuExist(string userId, string url)
        {
            string qry = string.Format("SELECT 1 FROM SA_MENU WHERE UserID='{0}' and FORMURL='{1}'", userId, url);
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), qry);
            return dt.Rows.Count > 0 ? true : false;
        }

    }
}
