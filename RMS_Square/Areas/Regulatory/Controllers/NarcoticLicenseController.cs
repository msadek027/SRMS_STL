using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.Regulatory.Models.DAO;
using RMS_Square.DAL.Common;
using RMS_Square.Models;
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
    public class NarcoticLicenseController : ControllerController
    {
        private FileDetailModel _fileModel = null;
        private static string _serverFilePath = string.Empty;
        private NarcoticLicenseDAO _dalObj = null;

        public NarcoticLicenseController()
        {
            _serverFilePath = Utility.GetServerPath();
            _dalObj = new NarcoticLicenseDAO();
        }
        //
        // GET: /Regulatory/NarcoticLicense/
        [ActionAuth]
        public ActionResult frmNarcoticLicense()
        {
            if (Session["UserID"] != null)
            {
                Session["FormNameTitle"] = "Entry Narcotic License";
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult frmNarcoticLicense(NarcoticLicenseBEL model)
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
            _fileModel.FileType = (int)Enums.E_FormFileType.NarcoticLicense;
            _fileModel.RefLevel1 = refLevel1;
            _fileModel.RefLevel2 = refLevel2;
            return Json(new { dataFile = GetFileByParameters(_fileModel).OrderBy(o => o.FileID), dataDetail = _dalObj.GetAll(refLevel1, orderBy: "DESC") }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadFile(string refLevel1, string refLevel2, string fileSize, string refNo)
        {

            var obj = PutUploadFile(refLevel1, refLevel2, fileSize, _serverFilePath, (int)Enums.E_FormFileType.NarcoticLicense, refNo);
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
       
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllInfo()
        {
            var data = _dalObj.GetAllInfo(new NarcoticLicenseBEL(), orderBy: "DESC");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetDetailGeneric(NarcoticLicenseBEL model )
        {
            var data = _dalObj.GetDetailGeneric(model,orderBy: "DESC");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetGenericBrand()
        {
            var data = _dalObj.GetGenericList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult DeleteDataItem(string id)
        {
            var isDelete = _dalObj.DeleteDataItem(id);//Convert.ToInt32(Session["UserID"])
            return Json(new { Mode = isDelete?"D":"E"});
        }
        
        [ActionAuth]
        public ActionResult frmNarcoticLicenseView()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult GetInfoByParams(NarcoticLicenseBEL model)
        {
            var dMaster = _dalObj.GetAllInfo(model, orderBy: "DESC");
            if (dMaster.Any())
            {
                _fileModel = new FileDetailModel();
                var refL1 = dMaster.FirstOrDefault().ID;
                _fileModel.RefLevel1 = refL1.ToString();
                _fileModel.FileType = (int)Enums.E_FormFileType.NarcoticLicense;
                var dLevel1 = GetFileByParameters(_fileModel).OrderBy(o => o.FileID);
                return Json(new { dataMaster = dMaster, dataLevel1 = dLevel1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { dataMaster = "", dataLevel1 = "" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult frmNarcoticLicenseRpt()
        {
            if (Session["UserID"] != null)
            {
                Session["FormNameTitle"] = "Narcotic License Report";
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        [HttpPost]
        public ActionResult frmNarcoticLicenseRpt(ReportModel model)
        {
            {
                var reportDocument = new ReportDocument();
                var rptPath = Server.MapPath("~/Reports");
                DataTable dt = new DataTable();

                string downFileName = string.Empty;
                string fromTodate = (model.ReportName).Replace(" ", "_") + "_From_" + model.FromDate + "_To_" + model.ToDate + "_";
                dt = _dalObj.GetAllNarcoticsCheckList(model);

                switch (model.ReportName)
                {
                    case "ExportNarcotics":
                        rptPath = rptPath + "/RptExportNarcoticsCheckListStatus.rpt";
                        model.ReportName = model.ReportName + "_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        downFileName = BindParameter(model, reportDocument, downFileName);
                        reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                        reportDocument.Close();
                        reportDocument.Dispose();
                        break;
                    case "ImportNarcotics":
                        rptPath = rptPath + "/RptImportNarcoticsCheckListStatus.rpt";
                        model.ReportName = model.ReportName + "_" + fromTodate;
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dt);
                        downFileName = BindParameter(model, reportDocument, downFileName);
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

            //reportDocument.SetParameterValue("AlarmDays", model.AlarmDays ?? "");
            downFileName = model.ReportName + DateTime.Now.ToString("yyyyMMdd'_'HHmmss");
            return downFileName;
        }
    }
}