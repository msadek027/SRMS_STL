using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.Regulatory.Models.DAO;
using RMS_Square.DAL.Common;
using RMS_Square.DAL.Gateway;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Systems.Controllers;

namespace RMS_Square.Areas.Regulatory.Controllers
{
    public class CompanyLicenseRptController : ControllerController
    {
        private readonly ReportDAO _dao = new ReportDAO();

        public ActionResult frmCompanyLicenseRpt()
        {
            if (Session["UserID"] != null) return View();
            return Redirect("~/Home/frmHome");
        }

        [HttpPost]
        public JsonResult GetCompanyLicenseReportData(CompanyLicenseReportParams param)
        {
            try
            {
                var data = _dao.GetCompanyLicenseReport(param);

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

                var dt = _dao.GetCompanyLicenseReportDT(param);
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
        public ActionResult PreviewFile(string fileUrl, string fileName, string fileId)
        {
            try
            {
                string uploadRoot = Server.MapPath(Utility.GetServerPath());
                string[] found = System.IO.Directory.GetFiles(
                    uploadRoot, fileUrl, System.IO.SearchOption.AllDirectories);

                if (found.Length == 0)
                    return HttpNotFound();

                string contentType = MimeMapping.GetMimeMapping(fileName);
                Response.AppendHeader("Content-Disposition",
                    "inline; filename=\"" + fileName + "\"");
                return File(found[0], contentType);
            }
            catch
            {
                return HttpNotFound();
            }
        }
    }
}