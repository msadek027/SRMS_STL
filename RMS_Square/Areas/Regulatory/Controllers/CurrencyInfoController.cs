using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.Regulatory.Models.DAO;
using RMS_Square.DAL.Gateway;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Systems.ActionFilter;

namespace RMS_Square.Areas.Regulatory.Controllers
{
    public class CurrencyInfoController : Controller
    {
       
        CurrencyInfoDAO currencyDAO = new CurrencyInfoDAO();
        ExceptionHandler exceptionHandler = new ExceptionHandler();
        //
        // GET: /Export/CurrencyInfo/
        [ActionAuth]
        public ActionResult frmCurrencyInfo()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCurrency()
        {
            var data = currencyDAO.GetCurrencyList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }       

        [HttpPost]
        public ActionResult OperationsMode(CurrencyInfoBEL master)
        {
            try
            {
                if (currencyDAO.SaveUpdate(master))
                {
                    return Json(new { ID = currencyDAO.MaxID, Mode = currencyDAO.IUMode, Status = "Yes" });
                }
                else
                    return View("frmCurrencyInfo");
            }
            catch (Exception e)
            {
                return exceptionHandler.ErrorMsg(e);
            }
        }

	}
}