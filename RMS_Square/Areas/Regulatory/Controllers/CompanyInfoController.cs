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
    public class CompanyInfoController : Controller
    {
        CompanyInfoDAO primaryDAO = new CompanyInfoDAO();
        //
        // GET: /Regulatory/CompanyInfo/
        [ActionAuth]
        public ActionResult frmCompanyInfo()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        [HttpPost]
        public ActionResult frmCompanyInfo(CompanyInfoBEL master)
        {
            try
            {
                string userId = Session["UserID"] as string;
                if (primaryDAO.SaveUpdate(master, userId))
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

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCompany()
        {
            var data = primaryDAO.GetCompanyList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


	}
}