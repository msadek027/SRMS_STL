using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.Regulatory.Models.DAO;
using RMS_Square.DAL.Gateway;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Systems.ActionFilter;
using Systems.Controllers;
using Systems.Models;

namespace RMS_Square.Areas.Regulatory.Controllers
{
    public class ProductRptController : ControllerController
    {
        private ReportDAO _dalReportObj = null;
        private static string _serverFilePath = string.Empty;
        public ProductRptController()
        {
            _dalReportObj = new ReportDAO();
        }

        // GET: /Regulatory/ProductRpt/frmProductRpt
       // [ActionAuth]
        public ActionResult frmProductRptPdf()
        {
            if (Session["UserID"] != null)
                return View();

            return Redirect("~/Home/frmHome");
        }

        public ActionResult frmProductRptView()
        {
            if (Session["UserID"] != null) return View();
            return Redirect("~/Home/frmHome");
        }

        [HttpPost]
        public JsonResult GetCompanyLicenseReportData(CompanyLicenseReportParams param)
        {
            try
            {
                var data = _dalReportObj.GetCompanyLicenseReport(param);

                string downloadBase = Url.Action("Download", "General");
                foreach (var r in data.Where(x => x.FileID > 0))
                {
                    r.FileUrl = downloadBase
                                + "?path=" + r.FileCode + r.FileExtension
                                + "&fileName=" + r.FileName
                                + "&fileId=" + r.FileID;

                    r.CanPreview = r.FileExtension == ".pdf"
                                || r.FileExtension == ".png"
                                || r.FileExtension == ".jpg"
                                || r.FileExtension == ".jpeg";
                }

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ExportCompanyLicenseReport(CompanyLicenseReportParams param)
        {
            try
            {
                if (Session["UserID"] == null) return Redirect("~/Home/frmHome");

                var dt = _dalReportObj.GetCompanyLicenseReportDT(param);
                string fromTo = "_" + (param.SubFrom ?? "") + "_To_" + (param.SubTo ?? "") + "_";
                string rptBase = Server.MapPath("~/Reports");

                using (var rpt = new CrystalDecisions.CrystalReports.Engine.ReportDocument())
                {
                    rpt.Load(rptBase + "/CompanyLicenseRpt.rpt");
                    rpt.SetDataSource(dt);
                    rpt.Refresh();
                    rpt.SetParameterValue("CompanyName", Session["COMPANY_NAME"]);
                    rpt.SetParameterValue("DevBy", Session["DEV_BY"]);
                    rpt.SetParameterValue("ProjectName", Session["ProjectName"]);
                    rpt.SetParameterValue("FilterCompany", param.CompanyCode ?? "All");
                    rpt.SetParameterValue("FilterStatus", param.DocStatus ?? "All");
                    rpt.SetParameterValue("FromDate", param.SubFrom ?? "");
                    rpt.SetParameterValue("ToDate", param.SubTo ?? "");

                    string fileName = "CompanyLicenseRpt" + fromTo + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var exportFmt = param.ReportType == "PDF"
                        ? CrystalDecisions.Shared.ExportFormatType.PortableDocFormat
                        : CrystalDecisions.Shared.ExportFormatType.ExcelRecord;

                    rpt.ExportToHttpResponse(
                        exportFmt,
                        System.Web.HttpContext.Current.Response,
                        false, fileName);
                }

                return View("frmCompanyLicenseRpt");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Export Error: " + ex.Message;
                return View("frmCompanyLicenseRpt");
            }
        }

        // ── File Preview (same pattern as ProductRpt) ────────────────────
    

        public ActionResult PreviewFile(string fileUrl, string fileId, string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileId) || fileId == "undefined")
                    return HttpNotFound("fileId missing.");

                if (string.IsNullOrEmpty(fileUrl))
                    return HttpNotFound("fileUrl missing.");

                DBHelper db = new DBHelper();
                var fileInfo = db.GetDocumentFileInfoById(fileId);

                if (fileInfo == null || string.IsNullOrEmpty(fileInfo.FilePath))
                    return HttpNotFound("File record not found. FileId: " + fileId);

                string guidName = fileUrl;

                string filePath = Server.MapPath(
                    fileInfo.FilePath.TrimEnd('/') + "/" + guidName);

                if (!System.IO.File.Exists(filePath))
                    return Content("File not found on disk: " + filePath);

                string ext = Path.GetExtension(guidName).ToLower();
                string contentType;
                switch (ext)
                {
                    case ".pdf": contentType = "application/pdf"; break;
                    case ".jpg":
                    case ".jpeg": contentType = "image/jpeg"; break;
                    case ".png": contentType = "image/png"; break;
                    case ".gif": contentType = "image/gif"; break;
                    default: contentType = "application/octet-stream"; break;
                }

                string displayName = !string.IsNullOrEmpty(fileName) ? fileName : guidName;
                Response.Headers["Content-Disposition"] =
                    "inline; filename=\"" + displayName + "\"";

                return File(filePath, contentType);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }



        // ── Search: Grid  ───────────────────────────────────────
        [HttpPost]
        public JsonResult GetProductReportData(ProductReportParams param)
        {
            try
            {
                var data = _dalReportObj.GetProductDocumentReport(param);

                string downloadBase = Url.Action("Download", "General");
                foreach (var r in data.Where(x => x.FileID > 0))
                {
                    // handleDownload এর exact pattern
                    r.FileUrl = downloadBase
                                + "?path=" + r.FileCode + r.FileExtension
                                + "&fileName=" + r.FileName
                                + "&fileId=" + r.FileID;

                    r.CanPreview = r.FileExtension == ".pdf"
                                || r.FileExtension == ".png"
                                || r.FileExtension == ".jpg"
                                || r.FileExtension == ".jpeg";
                }

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        [HttpGet]
        public JsonResult GetFilesByAnnexId(string annexId)
        {
            try
            {
                var fileModel = new FileDetailModel
                {
                    FileType = (int)Enums.E_FormFileType.ProductRegistration,
                    RefLevel1 = annexId,
                    RefLevel2 = ""
                };

                string downloadBase = Url.Action("Download", "General");

                var files = GetFileByParameters(fileModel)
                                .OrderBy(o => o.FileID)
                                .Select(f => new
                                {
                                    f.FileID,
                                    f.FileCode,
                                    f.FileName,
                                    f.Extention,
                                    f.FileSize,
                                    f.RefNo,
                                    // handleDownload এর exact same pattern
                                    DownloadUrl = downloadBase
                                                  + "?path=" + f.FileCode + f.Extention
                                                  + "&fileName=" + f.FileName
                                                  + "&fileId=" + f.FileID,
                                    CanPreview = f.Extention == ".pdf"
                                               || f.Extention == ".png"
                                               || f.Extention == ".jpg"
                                               || f.Extention == ".jpeg"
                                })
                                .ToList();

                return Json(files, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // ── Export: Crystal Reports দিয়ে PDF/Excel ──────────────────────────
        [HttpPost]
        public ActionResult ExportProductReport(ProductReportParams param)
        {
            try
            {
                if (Session["UserID"] == null)
                    return Redirect("~/Home/frmHome");

                // DB থেকে DataTable আনো
                var dt = _dalReportObj.GetProductDocumentReportDT(param);

                string fromTo = "_" + (param.SubFrom ?? "") + "_To_" + (param.SubTo ?? "") + "_";
                string rptBasePath = Server.MapPath("~/Reports");

                using (var reportDocument = new CrystalDecisions.CrystalReports.Engine.ReportDocument())
                {
                    reportDocument.Load(rptBasePath + "/ProductDocumentRpt.rpt");
                    reportDocument.SetDataSource(dt);
                    reportDocument.Refresh();

                    // Session parameters — existing pattern এর মতো
                    reportDocument.SetParameterValue("CompanyName", Session["COMPANY_NAME"]);
                    reportDocument.SetParameterValue("DevBy", Session["DEV_BY"]);
                    reportDocument.SetParameterValue("ProjectName", Session["ProjectName"]);
                    reportDocument.SetParameterValue("FilterCompany", param.CompanyCode ?? "All");
                    reportDocument.SetParameterValue("FilterStatus", param.DocStatus ?? "All");
                    reportDocument.SetParameterValue("FromDate", param.SubFrom ?? "");
                    reportDocument.SetParameterValue("ToDate", param.SubTo ?? "");

                    string fileName = "ProductDocumentRpt" + fromTo + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                    var exportFormat = param.ReportType == "PDF"
                        ? CrystalDecisions.Shared.ExportFormatType.PortableDocFormat
                        : CrystalDecisions.Shared.ExportFormatType.ExcelRecord;

                    reportDocument.ExportToHttpResponse(
                        exportFormat,
                        System.Web.HttpContext.Current.Response,
                        false,
                        fileName);
                }

                return View("frmProductRpt");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Export Error: " + ex.Message;
                return View("frmProductRpt");
            }
        }

      


    }
}
