using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.Regulatory.Models.DAO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Systems.ActionFilter;

namespace RMS_Square.Areas.Regulatory.Controllers
{
    public class ProductInfoController : Controller
    {
        //
        // GET: /Regulatory/ProductInfo/
        ProductInfoDAO primaryDAO = new ProductInfoDAO();

        [ActionAuth]
        public ActionResult frmProductInfo()
        {
            if (Session["UserID"] != null)
            {
                //ViewBag.strengthList = new DalCustomer().GetAll(0, Convert.ToInt32(Session["MemberID"]), true, string.Empty, "Name").OrderBy(o => o.CustomerName);
                // ViewBag.strengthList = new StrengthInfoDAO().GetStrengthList();    
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        [HttpPost]
        public ActionResult frmProductInfo(ProductInfoBEL master)
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
        public ActionResult GetProduct()
        {
            var data = primaryDAO.GetProductList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetProductFromAnnex(string companyCode)
        {
            var data = primaryDAO.GetProductFromAnnex(companyCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetProductFromRecipe(string companyCode)
        {
            var data = primaryDAO.GetProductFromRecipe(companyCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [ActionAuth]
        public ActionResult frmProductInfoView()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult GetInfoByParams(ProductInfoBEL model)
        {
            var dMaster = primaryDAO.GetInfoByParams(model, orderBy: "DESC");
            return Json(new { dataMaster = dMaster }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetLatestProductAll()
        //{
        //    if (Session["UserID"] != null)
        //    {
        //        var data = new ProductInfoDAO().GetInfoByParams(new ProductInfoBEL(), orderBy: "DESC").ToList();
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }
        //    return Redirect(string.Format("~/Home/frmHome"));
        //}
        public ActionResult frmProductInfoRpt()
        {
            if (Session["UserID"] != null)
            {
                Session["FormNameTitle"] = "Product Info Report";
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult frmProductInfoRpt(ReportModel model)
        {
            if (Session["UserId"] != null)
            {
                var reportDocument = new ReportDocument();
                var rptPath = Server.MapPath("~/Reports");
                DataTable dt = new DataTable();

                switch (model.ReportName)
                {
                    case "ProductInfo":
                        string downFileName = string.Empty;
                        string fromTodate = "_From_" + model.FromDate + "_To_" + model.ToDate + "_";
                        dt = primaryDAO.GetProductInfoForReport(model, "ASC");
                        rptPath = rptPath + "/ProductInfoRpt.rpt";
                        string reportName = "ProductInfoRpt" + "_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        reportDocument.Refresh();
                        reportDocument.SetParameterValue("CompanyName", Session["COMPANY_NAME"]);
                        reportDocument.SetParameterValue("DevBy", Session["DEV_BY"]);
                        reportDocument.SetParameterValue("ProjectName", Session["ProjectName"]);
                        reportDocument.SetParameterValue("FromDate", model.FromDate == null ? "" : model.FromDate);
                        reportDocument.SetParameterValue("ToDate", model.ToDate == null ? "" : model.ToDate);
                        downFileName = reportName + DateTime.Now.ToString("yyyyMMdd'_'HHmmss");
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "ProductSummary":
                        dt = primaryDAO.GetProductSummaryReport(model);
                        rptPath = rptPath + "/rptProductSummary.rpt";
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        reportDocument.Refresh();
                        downFileName = "ProductSummary_" + DateTime.Now.ToString("yyyyMMdd'_'HHmmss");
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                }

                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
    }
}