using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.Regulatory.Models.DAO;
using RMS_Square.DAL.Common;
using System;
using System.Web;
using System.Web.Mvc;
using Systems.ActionFilter;
using Systems.Controllers;

namespace RMS_Square.Areas.Regulatory.Controllers
{
    public class ProductRegApprovalController : ControllerController
    {
        private readonly ProductRegApprovalDAO _dal;

        public ProductRegApprovalController()
        {
            _dal = new ProductRegApprovalDAO();
        }

        // ── GET: View ──────────────────────────────────────────────
        //[ActionAuth]
        public ActionResult frmProductRegApproval()
        {
            if (Session["UserID"] == null)
                return Redirect("~/Home/frmHome");
            return View();
        }

        // ── POST: Save Approval ────────────────────────────────────
        [HttpPost]
        public ActionResult SaveApproval(ProductRegistrationBEL model)
        {
            try
            {
                bool ok = _dal.SaveApproval(model, Session["UserID"] as string);
                return Json(new { Status = ok ? "Yes" : "Save failed" });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    Status = "Error: " +
                    e.Message.Substring(0, Math.Min(60, e.Message.Length))
                });
            }
        }

        // ── GET: List ──────────────────────────────────────────────
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllInfo(string companyUnitCode, string companyCode,string productCode, string approvalStatus)
        {
            var filter = new ProductRegistrationBEL
            {
                CompanyUnitCode = companyUnitCode ?? "",
                CompanyCode = companyCode ?? "",
                ProductCode = productCode ?? "",
                ApprovalStatus = approvalStatus ?? ""
            };
            var data = _dal.GetAllInfo(filter, "DESC");
            return new JsonResult
            {
                Data = data,
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // ── GET: File Preview ──────────────────────────────────────
        public ActionResult PreviewFile(string path, string fileName, string fileId)
        {
            try
            {
                string uploadRoot = Server.MapPath(Utility.GetServerPath());
                string[] found = System.IO.Directory.GetFiles(
                    uploadRoot, path, System.IO.SearchOption.AllDirectories);

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