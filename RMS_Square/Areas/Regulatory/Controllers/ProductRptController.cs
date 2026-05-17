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

        public ActionResult PreviewFile(string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                    return HttpNotFound();

                // fileUrl থেকে সব parameter parse করুন
                var uri = new Uri("http://localhost" + fileUrl);
                var qs = System.Web.HttpUtility.ParseQueryString(uri.Query);
                string fileId = qs["fileId"];   // 9041
                string guidName = qs["path"];     // 968ce0d8-6d84-4264-a161-d521f4d45ae4.pdf  ← disk এ এই নামে আছে
                string fileName = qs["fileName"]; // 1 - Top500... ← display name শুধু

                if (string.IsNullOrEmpty(fileId))
                    return HttpNotFound("fileId missing.");

                DBHelper db = new DBHelper();
                var fileInfo = db.GetDocumentFileInfoById(fileId);

                if (fileInfo == null || string.IsNullOrEmpty(fileInfo.FilePath))
                    return HttpNotFound("File record not found. FileId: " + fileId);

                string relativePath = fileInfo.FilePath.TrimEnd('/') + "/" + guidName;
                // result: ~/App_Data/Upload/20260510/968ce0d8-6d84-4264-a161-d521f4d45ae4.pdf

                string filePath = Server.MapPath(relativePath);

                if (!System.IO.File.Exists(filePath))
                    return Content("File not found on disk: " + filePath);

                // Extension 
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

                // Display name হিসেবে original fileName দেখাবে
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


    }
}
