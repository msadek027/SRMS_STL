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
    public class TherapeuticClassInfoController : Controller
    {

        TherapeuticClassInfoDAO _dalObj = new TherapeuticClassInfoDAO();

        //
        // GET: /Regulatory/TherapeuticClassInfo/
        [ActionAuth]
        public ActionResult frmTherapeuticClassInfo()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        [HttpPost]
        public ActionResult frmTherapeuticClassInfo(TherapeuticClassInfoBEL master)
        {
            try
            {
                String userId;
                userId = Session["UserID"] as String;

                if (_dalObj.SaveUpdate(master, userId))
                {
                    return Json(new { ID = _dalObj.MaxID, Mode = _dalObj.IUMode, Status = "Yes" });
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
        public ActionResult GetTherapeuticClass()
        {
            var data = _dalObj.GetTherapeuticClassList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
	}
}