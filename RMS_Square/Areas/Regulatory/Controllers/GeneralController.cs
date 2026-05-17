using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.Regulatory.Models.DAO;
using RMS_Square.DAL.Common;
using RMS_Square.DAL.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
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
        public JsonResult GetCompanyUnitByCompany(string companyCode)
        {
            try
            {
                string sql = @"
            SELECT 
                CU.COMPANY_UNIT_CODE,
                CU.COMPANY_UNIT_NAME,
                CU.COMPANY_CODE
            FROM STL_SRMS.COMPANY_UNIT_INFO CU
            WHERE 1=1
        ";

                if (!string.IsNullOrEmpty(companyCode))
                    sql += " AND CU.COMPANY_CODE = '" + companyCode + "'";

                sql += " ORDER BY CU.COMPANY_UNIT_CODE";

                var dbHelper = new DBHelper();
                DataTable dt = dbHelper.GetDataTable(sql);

                var list = (from DataRow row in dt.Rows
                            select new
                            {
                                CompanyUnitCode = row["COMPANY_UNIT_CODE"].ToString(),
                                CompanyUnitName = row["COMPANY_UNIT_NAME"].ToString(),
                                CompanyCode = row["COMPANY_CODE"].ToString()
                            }).ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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