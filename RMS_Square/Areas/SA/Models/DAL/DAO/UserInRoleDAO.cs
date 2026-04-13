using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using RMS_Square.Areas.SA.Models.BEL;
using RMS_Square.DAL.Gateway;
using Systems.Universal;
using System.Text;

namespace RMS_Square.Areas.SA.Models.DAL.DAO
{
    public class UserInRoleDAO : ReturnData
    {
        DBConnection  dbConn=new DBConnection();
        //Encryption encryption = new Encryption();
        // SaHelper saHelper = new SaHelper();
        DBHelper saHelper = new DBHelper();
        public bool SaveUpdate(UserInRoleBEL master)
        {
            try
            {
                string Qry = "Select MAX(UserID) ID from Sa_UserInRole";
                var tuple = saHelper.ProcedureExecuteTFn5(dbConn.SAConnStrReader(), Qry, "Sa_UserInRole_SSP", "p_RoleID", "p_UserID", "p_EmpID", "p_NewPassword", "p_IsActive", master.RoleID, Encryption.Encrypt(master.UserID),Convert.ToString(master.EmpID), Encryption.Encrypt(master.Password), master.IsActive.ToString());
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

        public List<UserInRoleBEL> GetUserInRoleList()
        {
            //string Qry = "SELECT ur.UserID,ur.RoleID,r.RoleName,ur.EmpID,GetName(ur.EmpID,'EM') EmpName,u.NewPassword,u.OldPassword,ur.IsActive FROM Sa_UserInRole ur, Sa_UserCredential u,Sa_Role r Where ur.UserID=u.UserID and ur.RoleID=r.RoleID and ur.RoleID>='" + HttpContext.Current.Session["RoleID"].ToString() + "'";
            var query = new StringBuilder();
            query.Append(" SELECT ur.UserID,ur.RoleID,r.RoleName,ur.EmpID,EI.EMPLOYEE_NAME EmpName,EI.EMPLOYEE_CODE EmpCode,u.NewPassword,u.OldPassword,ur.IsActive  FROM Sa_UserCredential U");
            query.Append(" LEFT JOIN  Sa_UserInRole ur ON U.USERID=ur.USERID");
            query.Append(" LEFT JOIN Sa_Role r ON r.RoleID=ur.RoleID ");
            query.Append(" LEFT JOIN EMPLOYEE_INFO EI ON EI.ID=UR.EMPID ");
            //query.Append(" WHERE ur.RoleID IS NOT NULL ");
            query.Append(" WHERE ur.RoleID >='"+ HttpContext.Current.Session["RoleID"].ToString() + "'");


            DataTable dt = saHelper.DataTableFn(dbConn.SAConnStrReader(), query.ToString());
            List<UserInRoleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new UserInRoleBEL
                    {
                        RoleID = row["RoleID"].ToString(),
                        RoleName = row["RoleName"].ToString(),
                        UserID = Encryption.Decrypt(row["UserID"].ToString()),                     
                        EmpID =  Convert.ToInt32(row["EmpID"]),
                        EmpCode = row["EmpCode"].ToString(),
                        EmpName = row["EmpName"].ToString(),
                        Password = Encryption.Decrypt(row["NewPassword"].ToString()),                        
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString())

                    }).ToList();
            return item;
        }

        public List<UserInRoleBEL> GetEmployeeList()
        {
            string Qry = "Select EMPLOYEE_CODE,EMPLOYEE_NAME From Sa_Employee -- where Upper(STATUS)=Upper('true')";
            DataTable dt = saHelper.DataTableFn(dbConn.SAConnStrReader(), Qry);
            List<UserInRoleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new UserInRoleBEL
                    {
                        EmpID =  Convert.ToInt32(row["EmpID"].ToString()),
                        EmpName = row["EMPLOYEE_NAME"].ToString()
                       

                    }).ToList();
            return item;
        }
        public List<UserInRoleBEL> GetEmployeeNotYetAssignedList()
        {
            string Qry = "Select E.ID EmpID,E.EMPLOYEE_CODE,E.EMPLOYEE_NAME From EMPLOYEE_INFO E LEFT JOIN Sa_UserInRole UR ON UR.EmpID=E.ID where UR.EmpID is null  ORDER BY E.EMPLOYEE_NAME --and  Upper(STATUS)=Upper('true')";
            DataTable dt = saHelper.DataTableFn(dbConn.SAConnStrReader(), Qry);
            List<UserInRoleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new UserInRoleBEL
                    {
                        EmpID = Convert.ToInt32(row["EmpID"].ToString()),
                        EmpCode = row["EMPLOYEE_CODE"].ToString(),
                        EmpName = row["EMPLOYEE_NAME"].ToString()
                    }).ToList();
            return item;
        }

        public List<UserInRoleBEL> GetBuyerList()
        {
            string Qry = "Select BUYER_CODE,BUYER_NAME From BUYER_INFO ";
            DataTable dt = saHelper.DataTableFn(dbConn.SAConnStrReader(), Qry);
            List<UserInRoleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new UserInRoleBEL
                    {
                        BuyerID = row["BUYER_CODE"].ToString(),
                        BuyerName = row["BUYER_NAME"].ToString()


                    }).ToList();
            return item;
        }
       public List<UserInRoleBEL> GetUserList()
        {
            string Qry = "Select UserID,EmpID,GetName(EmpID,'EM') EmpName From Sa_UserInRole Where Upper(IsActive)=Upper('true')";
            DataTable dt = saHelper.DataTableFn(dbConn.SAConnStrReader(), Qry);
            List<UserInRoleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new UserInRoleBEL
                    {
                        UserID = Encryption.Decrypt(row["UserID"].ToString()),
                        EmpName = row["EmpName"].ToString()
                    }).ToList();
            return item;
        }
     
       public List<UserInRoleBEL> GetBuyerYetAssignedList(string EmpID)
       {
           string Qry = "Select BUYER_ID,GetName(BUYER_ID,'BR') BuyerName  From SA_EMP_BUYER_MAPPING where Emp_ID='" + EmpID + "'";
           DataTable dt = saHelper.DataTableFn(dbConn.SAConnStrReader(), Qry);
           List<UserInRoleBEL> item;

           item = (from DataRow row in dt.Rows
                   select new UserInRoleBEL
                   {
                       BuyerID = row["BUYER_ID"].ToString(),
                       BuyerName = row["BuyerName"].ToString()
                   }).ToList();
           return item;
       }
    }
}