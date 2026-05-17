using System;
using System.Web.Mvc;
using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.Regulatory.Models.DAO;

namespace RMS_Square.Areas.Regulatory.Controllers
{
    public class ProductDocReportController : Controller
    {
        private DocumentReportDAO _dao = new DocumentReportDAO();

        public ActionResult frmDocumentReport()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return RedirectToAction("frmHome", "Home");
        }

        [HttpPost]
        public ActionResult GetDocumentData(DocumentReportParams param)
        {
            try
            {
                var data = _dao.GetUploadedDocuments(param);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}