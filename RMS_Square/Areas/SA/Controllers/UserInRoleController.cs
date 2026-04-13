using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RMS_Square.Areas.SA.Models.BEL;
using RMS_Square.Areas.SA.Models.DAL.DAO;
using Systems.ActionFilter;

namespace RMS_Square.Areas.SA.Controllers
{
    public class UserInRoleController : Controller
    {
        UserInRoleDAO primaryDAO = new UserInRoleDAO();
        [ActionAuth]
        public ActionResult frmUserInRole()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        [HttpGet]
        public ActionResult GetEmployee()
        {
            var data = primaryDAO.GetEmployeeList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetEmployeeNotYetAssigned()
        {
            var data = primaryDAO.GetEmployeeNotYetAssignedList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBuyerYetAssigned(string EmpID)
        {
            var data = primaryDAO.GetBuyerYetAssignedList(EmpID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetBuyer()
        {
            var data = primaryDAO.GetBuyerList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetUser()
        {
            var data = primaryDAO.GetUserList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetUserInRole()
        {
            var data = primaryDAO.GetUserInRoleList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult OperationsMode(UserInRoleBEL master)
        {
            try
            {
               // var eu = Systems.Universal.Encryption.Encrypt(master.UserID);
                if (primaryDAO.SaveUpdate(master))//primaryDAO.SaveUpdate(master)
                {
                    return Json(new { ID = primaryDAO.MaxID, Mode = primaryDAO.IUMode, Status = "Yes" });
                }
                else
                    return View();
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 9) == "ORA-00001")
                    return Json(new { Status = "Error:ORA-00001,Data already exists!" });//Unique Identifier.
                else if (e.Message.Substring(0, 9) == "ORA-02292")
                    return Json(new { Status = "Error:ORA-02292,Data already exists!" });//Child Record Found.
                else if (e.Message.Substring(0, 9) == "ORA-12899")
                    return Json(new { Status = "Error:ORA-12899,Data Value Too Large!" });//Value Too Large.
                else
                    return Json(new { Status = "! Error : Error Code:" + e.Message.Substring(0, 9) });//Other Wise Error Found

            }
        }

	}
}