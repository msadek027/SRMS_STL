using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.Regulatory.Models.DAO;
using RMS_Square.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Systems.Controllers;
using Systems.Models;


namespace RMS_Square.Areas.Regulatory.Controllers
{
    public class GeneralController : ControllerController
    {
        GeneralDAO primaryDAO = new GeneralDAO();

        private FileDetailModel _fileModel = null;
        private static string _serverFilePath = string.Empty;
        public GeneralController()
        {
            _serverFilePath = Utility.GetServerPath();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetPriceType()
        {
            // var data = primaryDAO.GetPriceTypeList();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllCompany()
        {
            var data = new CompanyInfoDAO().GetCompanyList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllProduct(string companyCode)
        {
            var data = new ProductInfoDAO().GetAllProduct(companyCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
          [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllActiveProduct(string companyCode)
        {
            var data = new ProductInfoDAO().GetAllActiveProduct(companyCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
          public ActionResult GetNewComming(string pageName)
          {
              var data = new GeneralDAO().GetNewComming(pageName);
              return Json(data, JsonRequestBehavior.AllowGet);
          }

          public ActionResult frmNewComming(string pageName)
          {
              if (Session["UserID"] != null)
              {
                  Session["PageName"] = pageName;
                  return View();
              }
              return Redirect(string.Format("~/Home/frmHome"));
          }
    }
}