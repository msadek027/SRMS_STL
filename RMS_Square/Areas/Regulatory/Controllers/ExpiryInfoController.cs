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
    public class ExpiryInfoController : Controller
    {
        private ReportDAO _dalReportObj = null;
        public ExpiryInfoController()
        {
            _dalReportObj = new ReportDAO();
        }
        [ActionAuth]
        public ActionResult frmExpiryInfoRpt()
        {
            if (Session["UserId"] != null)
            {
                ViewBag.formTitle = "";

                //int i = 10;
                //int x = i++ + ++i;
                //int y = ++i + i++;
                //int z = ++i + i++ + ++i; 
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult frmExpiryInfoRpt(ReportModel model)
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
                        downFileName = BindParameter(model, reportDocument, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "ProductLicInfo":
                        rptPath = rptPath + "/ProductLicInfo.rpt";
                        model.ReportName = model.ReportName + "_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                         downFileName = BindParameter(model, reportDocument, downFileName);
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
                        // downFileName = BindParameterScdc(model, reportDocument, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                }
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        
        private string BindParameter(ReportModel model, ReportDocument reportDocument, string downFileName)
        {
            reportDocument.Refresh();
            reportDocument.SetParameterValue("CompanyName", Session["COMPANY_NAME"]);
            reportDocument.SetParameterValue("DevBy", Session["DEV_BY"]);
            reportDocument.SetParameterValue("ProjectName", Session["ProjectName"]);
            reportDocument.SetParameterValue("FromDate", model.FromDate == null ? "" : model.FromDate);
            reportDocument.SetParameterValue("ToDate", model.ToDate == null ? "" : model.ToDate);
           
            reportDocument.SetParameterValue("AlarmDays", model.AlarmDays ?? "");
            downFileName = model.ReportName + DateTime.Now.ToString("yyyyMMdd'_'HHmmss");
            return downFileName;
        }
	}
	}
