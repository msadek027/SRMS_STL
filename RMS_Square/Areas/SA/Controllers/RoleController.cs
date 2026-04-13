using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RMS_Square.Areas.SA.Models.BEL;
using RMS_Square.Areas.SA.Models.DAL.DAO;
using RMS_Square.DAL.DAO;
using RMS_Square.DAL.Gateway;
using Systems.ActionFilter;

namespace RMS_Square.Areas.SA.Controllers
{
    public class RoleController : Controller
    {

        RoleDAO primaryDAO = new RoleDAO();
        //
        // GET: /SA/Role/
        [ActionAuth]
        public ActionResult frmRole()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetRole()
        {
            var data = primaryDAO.GetRoleList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetRoleInSoftwareModuleMapping()
        {
            var data = primaryDAO.GetRoleInSoftwareModuleMappingList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult OperationsMode(RoleBEL master)
        {
            try
            {
                if (primaryDAO.SaveUpdate(master))
                {
                    return Json(new { ID = primaryDAO.MaxID, Mode = primaryDAO.IUMode, Status = "Yes" });
                }
                else
                    return View("frmRole");
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