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
using Systems.Controllers;

namespace RMS_Square.Areas.Regulatory.Controllers
{
    public class ReportsController : ControllerController
    {

        private ReportDAO _dalReportObj = null;
        public ReportsController()
        {
            _dalReportObj = new ReportDAO();
        }
       
        public ActionResult frmRptProductLcInfo()
        {
            if (Session["UserId"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult frmRptProductLcInfo(ReportModel model)
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
                    case "ScpGroupValueShare":
                        rptPath = rptPath + "/ScpGroupValueShare.rpt";
                        model.ReportName = "Special_Campaign_Group_Value_Share_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        //downFileName = BindParameterScdc(model, reportDocument, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "ScpGroupValueSharePersent":
                        rptPath = rptPath + "/ScpGroupValueSharePersent.rpt";
                        model.ReportName = "Special_Campaign_Group_Value_Share_Percentage_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        // downFileName = BindParameterScdc(model, reportDocument, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "ProductLifeCycleInfo":
                        if (model.ReportType == "PDF")
                        {
                            rptPath = rptPath + "/ProductLifeCycleInfo.rpt";
                            model.ReportName = "ProductLifeCycleInfo_" + fromTodate;
                            reportDocument.Load(rptPath);
                            reportDocument.SetDataSource(dt);
                            downFileName = BindParameter(model, reportDocument, downFileName);// +" From:" + model.FromDate + "To:" + model.ToDate;
                            reportDocument.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, downFileName);
                            reportDocument.Close();
                            reportDocument.Dispose();
                        }
                        else
                        {
                            downFileName = "ProductLifeCycleInfo_" + model.ProductCode + "_";
                            //It represent name of column for which you want to select records
                            string[] selectedColumns = new[] {"COMPANY_CODE","COMPANY_NAME","PRODUCT_CODE", "SAP_PRODUCT_CODE", "BRAND_NAME", "GENERIC_STRENGTH", "DOSAGE_FORM_NAME", "PACK_SIZE_NAME", "PRODUCT_CATEGORY", "RECIPE_SUBMISSION_TYPE", "RECIPE_RECEIVE_DATE", "RECIPE_PROPOSAL_DATE","RECIPE_MEETING_DATE","RECIPE_APPROVAL_DATE","RECIPE_VALID_UPTO",
                          "PRODUCT_SPECIFICATION","DTL_SUBMISSION_DATE","DTL_APPROVAL_DATE","DAR_NO","ANNAX_RECEIVE_DATE","ANNEX_SUBMISSION_DATE","ANNEX_VALID_UPTO","PRICE_RECEIVED_DATE","PRICE_SUBMISSION_DATE","PRICE_APPROVAL_DATE","PRICE_PER_UNIT","PRICE_CHANGE_STATUS","MA_SUBMISSION_DATE","MA_RECEIVE_DATE","MA_APPROVAL_DATE","MA_VALID_UPTO"  };
                            MadeDataForExcel(dt, downFileName, selectedColumns, "COMPANY_NAME,PRODUCT_CODE");
                        }
                        break;
                }
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        
        private string BindParameter(ReportModel model, ReportDocument reportDocument, string downFileName)
        {
            reportDocument.Refresh();
            reportDocument.SetParameterValue("P_ProductCode", model.ProductCode ?? "ALL Product");
            downFileName = model.ReportName + DateTime.Now.ToString("yyyyMMdd'_'HHmmss");
            return downFileName;
        }

        public ActionResult frmAllExpiryRpt()
        {
            if (Session["UserId"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult frmAllExpiryRpt(ReportModel model)
        {
            if (Session["UserId"] != null)
            {
                var reportDocument = new ReportDocument();
                var rptPath = Server.MapPath("~/Reports");
                DataTable dt = new DataTable();

                string downFileName = string.Empty;
                string fromTodate = (model.ReportName).Replace(" ", "_") + "_From_" + model.FromDate + "_To_" + model.ToDate + "_";
                dt = _dalReportObj.GetAllNotifyInfo(model);

                switch (model.ReportName)
                {
                    case "CompanyLicInfo":
                        rptPath = rptPath + "/CompanyLicInfo.rpt";
                        model.ReportName = model.ReportName + "_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        downFileName = BindParameterExpiry(model, reportDocument, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "ProductLicInfo":
                        rptPath = rptPath + "/ProductLicInfo.rpt";
                        model.ReportName = model.ReportName + "_" + fromTodate;

                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        downFileName = BindParameterExpiry(model, reportDocument, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "GMPLicInfo":
                        rptPath = rptPath + "/GMPLicInfo.rpt";
                        model.ReportName = model.ReportName + "_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        // downFileName = BindParameterScdc(model, reportDocument, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "RecipeNotifyInfo":
                        rptPath = rptPath + "/RecipeNotifyInfo.rpt";
                        model.ReportName = model.ReportName + "_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        // downFileName = BindParameterScdc(model, reportDocument, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "NarcoticLicInfo":
                        rptPath = rptPath + "/NarcoticLicInfo.rpt";
                        model.ReportName = model.ReportName + "_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        // downFileName = BindParameterScdc(model, reportDocument, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "ImportProductLicInfo":
                        rptPath = rptPath + "/ImportProductLicInfo.rpt";
                        model.ReportName = model.ReportName + "_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        // downFileName = BindParameterScdc(model, reportDocument, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "AdvertisementNotifyInfo":
                        rptPath = rptPath + "/AdvertisementNotifyInfo.rpt";
                        model.ReportName = model.ReportName + "_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        // downFileName = BindParameterExpiry(model, reportDocument, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                }
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        private string BindParameterExpiry(ReportModel model, ReportDocument reportDocument, string downFileName)
        {
            reportDocument.Refresh();
            reportDocument.SetParameterValue("FromDate", model.FromDate == null ? "" : model.FromDate);
            reportDocument.SetParameterValue("ToDate", model.ToDate == null ? "" : model.ToDate);

            reportDocument.SetParameterValue("AlarmDays", model.AlarmDays ?? "");

            downFileName = model.ReportName + DateTime.Now.ToString("yyyyMMdd'_'HHmmss");
            return downFileName;
        }

    }
}