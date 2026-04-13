using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.Regulatory.Models.DAO;
using RMS_Square.DAL.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Systems.ActionFilter;
using Systems.Controllers;
using Systems.Models;

namespace RMS_Square.Areas.Regulatory.Controllers
{
    public class NocInfoController : ControllerController
    {
        private FileDetailModel _fileModel = null;
        private static string _serverFilePath = string.Empty;
        private NocInfoDAO _dalObj = null;

        public NocInfoController()
        {
            _serverFilePath = Utility.GetServerPath();
            _dalObj = new NocInfoDAO();
        }
       
        [ActionAuth]
        public ActionResult frmNocInfo()
        {
            if (Session["UserID"] != null)
            {
                Session["FormNameTitle"] = "Entry NOC Info";
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAll()
        {
            var data = _dalObj.GetNocInformation();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult frmNocInfo(NocInfoBEL model)
        {
            try
            {
                if (_dalObj.SaveUpdate(model, userId: Session["UserID"] as string))
                {
                    return Json(new { ID = _dalObj.ReturnMaxID, SlNo = _dalObj.MaxID, Mode = _dalObj.IUMode, Status = "Yes", RevisionNo = _dalObj.RefNo });
                }
                return View();
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
            return View();
        }

       
        public ActionResult GetFileByRefIdAndDetailData(string refLevel1, string refLevel2)
        {
            _fileModel = new FileDetailModel();
            _fileModel.FileType = (int)Enums.E_FormFileType.NOCInfo;
            _fileModel.RefLevel1 = refLevel1;
            _fileModel.RefLevel2 = refLevel2;
            return Json(new { dataFile = GetFileByParameters(_fileModel).OrderBy(o => o.FileID), dataDetail = _dalObj.GetAll(refLevel1, orderBy: "DESC") }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult UploadFile(string refLevel1, string refLevel2, string fileSize, string refNo)
        {
            var obj = PutUploadFile(refLevel1, refLevel2, fileSize, _serverFilePath, (int)Enums.E_FormFileType.NOCInfo, refNo);
            if (obj.Item1 == "S")
            {
                _fileModel = new FileDetailModel();
                _fileModel.FileName = obj.Item2.FileName;
                _fileModel.FileCode = obj.Item2.FileCode;
                _fileModel.FileSize = obj.Item2.FileSize;
                _fileModel.FileType = obj.Item2.FileType;
                _fileModel.RefNo = obj.Item2.RefNo;
                _fileModel.RefLevel1 = obj.Item2.RefLevel1;
                _fileModel.RefLevel2 = obj.Item2.RefLevel2;
                _fileModel.Extention = obj.Item2.Extention;

                bool isSave = SaveUploadFileInfo(_fileModel, Session["UserID"] as string);

                return Json(new { msgType = "FUS", FileList = GetFileByParameters(_fileModel).OrderByDescending(o => o.FileID) }, JsonRequestBehavior.AllowGet);
            }
            else if (obj.Item1 == "L")
            {
                return Json(new { msgType = "FLI", FileList = "" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { msgType = "FUE", FileList = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetFileByRefId(string refLevel1, string refLevel2)
        {
            _fileModel = new FileDetailModel();
            _fileModel.FileType = (int)Enums.E_FormFileType.NOCInfo;
            _fileModel.RefLevel1 = refLevel1;
            _fileModel.RefLevel2 = refLevel2;
            return Json(GetFileByParameters(_fileModel).OrderBy(o => o.FileID), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteDataItem(string id)
        {
            var isDelete = _dalObj.DeleteDataItem(id);//Convert.ToInt32(Session["UserID"])
            return Json(new { Mode = isDelete ? "D" : "E" });
        }

        public ActionResult frmNocInfoRpt()
        {
            if (Session["UserID"] != null)
            {
                Session["FormNameTitle"] = "NOC Info Report";
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult frmNocInfoRpt(ReportModel model)
        {
            if (Session["UserId"] != null)
            {
                var reportDocument = new ReportDocument();
                var rptPath = Server.MapPath("~/Reports");
                DataTable dt = new DataTable();

                string downFileName = string.Empty;
                string fromTodate = "_From_" + model.FromDate + "_To_" + model.ToDate + "_";
                dt = _dalObj.GetAllNocWithDetailForReport(model, "ASC");
                if (model.ReportType != "HTML")
                {
                    rptPath = rptPath + "/NocInfoRpt.rpt";
                    string reportName = "NocInfoRpt" + "_" + fromTodate;
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
                    return View();
                }
                else
                {
                    var item = (from DataRow row in dt.Rows
                                select new NocInfoBEL
                                {
                                    SN = Convert.ToInt64(row["SN"]),
                                    ID = Convert.ToInt64(row["ID"]),
                                    SlNo = row["SLNO"].ToString(),
                                    CompanyCode = row["COMPANY_CODE"].ToString(),
                                    CompanyName = row["COMPANY_NAME"].ToString(),
                                    Address = row["ADDRESS"].ToString(),
                                    LicenseNo = row["LICENSE_NO"].ToString(),
                                    SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                                    ApprovalDate = row["APPROVAL_DATE"].ToString(),
                                    InvoiceDate = row["INVOICE_DATE"].ToString(),
                                    InvoiceNo = row["INVOICE_NO"].ToString(),
                                    ItemName = row["ITEM_NAME"].ToString(),
                                    Quantity = row["QUANTITY"].ToString(),
                                    CountryOfOrigin = row["COUNTRY_OF_ORIGNIN"].ToString(),
                                }).ToList();

                   // var data = item.OrderBy(s => s.ID);
                    return Json(item, JsonRequestBehavior.AllowGet);
                }
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
    }
}