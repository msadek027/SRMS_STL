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
using Systems.Controllers;

namespace RMS_Square.Areas.Regulatory.Controllers
{
    public class ProductRptController : ControllerController
    {
        private ReportDAO _dalReportObj = null;
        public ProductRptController()
        {
            _dalReportObj = new ReportDAO();
        }
        [ActionAuth]
        public ActionResult frmProductRptPdf()
        {
            if (Session["UserId"] != null)
            {
                ViewBag.formTitle = "Product Life Cycle Information Report";
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult frmProductRptPdf(ReportModel model)
        {
            if (Session["UserId"] != null)
            {
                var reportDocument = new ReportDocument();
                var rptPath = Server.MapPath("~/Reports");
                DataTable dt = new DataTable();

                string downFileName = string.Empty;
                string fromTodate = (model.ReportName).Replace(" ", "_") + "_From_" + model.FromDate + "_To_" + model.ToDate + "_";
                dt = _dalReportObj.GetProductLcInfo(model);

                switch (model.ReportName)
                {
                    case "MasterProductInfo":
                        rptPath = rptPath + "/MasterProductInfo.rpt";
                        model.ReportName = "MasterProductInfo_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);

                        reportDocument.Refresh();
                        reportDocument.SetParameterValue("CompanyName", Session["COMPANY_NAME"]);
                        reportDocument.SetParameterValue("DevBy", Session["DEV_BY"]);
                        reportDocument.SetParameterValue("ProjectName", Session["ProjectName"]);
                        reportDocument.SetParameterValue("FromDate", model.FromDate == null ? "" : model.FromDate);
                        reportDocument.SetParameterValue("ToDate", model.ToDate == null ? "" : model.ToDate);
                        reportDocument.SetParameterValue("P_ProductCode", model.ProductCode ?? "ALL Product");
                        downFileName = model.ReportName + DateTime.Now.ToString("yyyyMMdd'_'HHmmss");
                       
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "ProductLifeCycleInfoSinglePage":
                        rptPath = rptPath + "/ProductLifeCycleInfoSingle.rpt";
                        model.ReportName = "ProductLifeCycleInfoSingle_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        downFileName = BindParameter(model, reportDocument, downFileName);// +" From:" + model.FromDate + "To:" + model.ToDate;
                        //reportDocument.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "ProductLifeCycleInfo":
                        rptPath = rptPath + "/ProductLifeCycleInfo.rpt";
                        model.ReportName = "ProductLifeCycleInfo_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        downFileName = BindParameter(model, reportDocument, downFileName);// +" From:" + model.FromDate + "To:" + model.ToDate;
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        //if (model.ReportType == "PDF")
                        //{
                        //    rptPath = rptPath + "/ProductLifeCycleInfo.rpt";
                        //    model.ReportName = "ProductLifeCycleInfo_" + fromTodate;
                        //    reportDocument.Load(rptPath);
                        //    reportDocument.SetDataSource(dt);
                        //    downFileName = BindParameter(model, reportDocument, downFileName);// +" From:" + model.FromDate + "To:" + model.ToDate;
                        //    reportDocument.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, downFileName);
                        //    reportDocument.Close();
                        //    reportDocument.Dispose();
                        //}
                        //else
                        //{
                        //    downFileName = "ProductLifeCycleInfo_" + model.ProductCode + "_";
                        //    //It represent name of column for which you want to select records
                        //    //  string[] selectedColumns = new[] {"COMPANY_CODE","COMPANY_NAME","PRODUCT_CODE", "SAP_PRODUCT_CODE", "BRAND_NAME", "GENERIC_STRENGTH", "DOSAGE_FORM_NAME", "PACK_SIZE_NAME", "PRODUCT_CATEGORY", "RECIPE_SUBMISSION_TYPE", "RECIPE_RECEIVE_DATE", "RECIPE_PROPOSAL_DATE","RECIPE_MEETING_DATE","RECIPE_APPROVAL_DATE","RECIPE_VALID_UPTO",
                        //    //"PRODUCT_SPECIFICATION","DTL_SUBMISSION_DATE","DTL_APPROVAL_DATE","DAR_NO","ANNAX_RECEIVE_DATE","ANNEX_SUBMISSION_DATE","ANNEX_VALID_UPTO","PRICE_RECEIVED_DATE","PRICE_SUBMISSION_DATE","PRICE_APPROVAL_DATE","PRICE_PER_UNIT","PRICE_CHANGE_STATUS","MA_SUBMISSION_DATE","MA_RECEIVE_DATE","MA_APPROVAL_DATE","MA_VALID_UPTO"  };
                        //    string[] selectedColumns = new[] { "COMPANY_NAME", "PRODUCT_CODE", "SAP_PRODUCT_CODE", "BRAND_NAME", "GENERIC_STRENGTH", "DOSAGE_FORM_NAME", "PACK_SIZE_NAME", "PRODUCT_CATEGORY", "PRODUCT_SPECIFICATION", "DAR_NO" };

                        //    MadeDataForExcel(dt, downFileName, selectedColumns, "COMPANY_NAME,PRODUCT_CODE");
                        //}
                        break;
                }
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        private string BindParameter(ReportModel model, ReportDocument reportDocument, string downFileName)
        {
            reportDocument.Refresh();
            //reportDocument.SetParameterValue("CompanyName", Session["COMPANY_NAME"]);
            //reportDocument.SetParameterValue("DevBy", Session["DEV_BY"]);
            //reportDocument.SetParameterValue("ProjectName", Session["ProjectName"]);
            //reportDocument.SetParameterValue("FromDate", model.FromDate == null ? "" : model.FromDate);
            //reportDocument.SetParameterValue("ToDate", model.ToDate == null ? "" : model.ToDate);
            reportDocument.SetParameterValue("P_ProductCode", model.ProductCode ?? "ALL Product");

            downFileName = model.ReportName + DateTime.Now.ToString("yyyyMMdd'_'HHmmss");
            return downFileName;
        }

        [ActionAuth]
        public ActionResult frmProductRptView()
        {
            if (Session["UserId"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        [HttpPost]
        public ActionResult GetInfoByParams(ReportModel model)
        {
            var dMaster = _dalReportObj.GetAllProductLifeCycle(model);
            return Json(new { dataMaster = dMaster }, JsonRequestBehavior.AllowGet);
        }
    }
}