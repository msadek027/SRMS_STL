using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.Regulatory.Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Systems.ActionFilter;

namespace RMS_Square.Areas.Regulatory.Controllers
{
    public class CompanyUnitInfoController : Controller
    {
        CompanyUnitInfoDAO oCompanyUnitInfoDAO = new CompanyUnitInfoDAO();


        //GET: /Regulatory/DesignationInfo/
        //[ActionAuth]
        public ActionResult frmCompanyUnitInfo()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        [HttpPost]
        public ActionResult frmCompanyUnitInfo(CompanyUnitInfoBEL master)
        {
            try
            {
                String userId;
                userId = Session["UserID"] as String;

                if (oCompanyUnitInfoDAO.SaveUpdate(master, userId))
                {
                    return Json(new { ID = oCompanyUnitInfoDAO.MaxID, Mode = oCompanyUnitInfoDAO.IUMode, Status = "Yes" });
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

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCompanyUnitInfo()
        {
            var data = oCompanyUnitInfoDAO.GetCompanyUnitInfoList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // returns company code + name list for dropdown (JSON)
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCompanyList()
        {
            var data = oCompanyUnitInfoDAO.GetCompanyList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}