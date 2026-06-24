using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.Regulatory.Models.DAO;
using RMS_Square.Controllers;
using RMS_Square.DAL.Common;
using RMS_Square.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Systems.ActionFilter;
using Systems.Controllers;
using Systems.Models;

namespace RMS_Square.Areas.Regulatory.Controllers
{
    public class CompanyLicenseController : ControllerController
    {
        private FileDetailModel _fileModel = null;
        private static string _serverFilePath = string.Empty;
        private CompanyLicenseDAO _dalObj = null;
        public CompanyLicenseController()
        {
            //_fileModel = new FileDetailModel();
            _serverFilePath = Utility.GetServerPath();
            _dalObj = new CompanyLicenseDAO();
        }
        //
        // GET: /Regulatory/CompanyLicense/
        [ActionAuth]
        public ActionResult frmCompanyLicense()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult frmCompanyLicense(CompanyLicenseBEL model)
        {
            try
            {
                if (_dalObj.SaveUpdate(model, userId: Session["UserID"] as string))
                {
                    return Json(new { ID = _dalObj.ReturnMaxID, SlNo = _dalObj.MaxID, Mode = _dalObj.IUMode, Status = "Yes", RevisionNo = _dalObj.RefNo });
                }

                // SaveUpdate 'false' রিটার্ন করলে View() এর বদলে Json রিটার্ন করতে হবে
                return Json(new { Status = "No", Message = "Failed to save data. Please try again." });
            }
            catch (Exception e)
            {
                string errMsg = e.Message;

                // Substring(0, 9) এর বদলে নিরাপদ চেক
                if (errMsg.StartsWith("ORA-00001"))
                    return Json(new { Status = "Error", Message = "Data already exists! (ORA-00001)" });
                else if (errMsg.StartsWith("ORA-02292"))
                    return Json(new { Status = "Error", Message = "Cannot update/delete: Child Record Found! (ORA-02292)" });
                else if (errMsg.StartsWith("ORA-12899"))
                    return Json(new { Status = "Error", Message = "Data Value Too Large! (ORA-12899)" });
                else
                    // অন্য যেকোনো এরর হলে পুরো মেসেজটি পাঠিয়ে দিন, যাতে UI তে বুঝতে সুবিধা হয়
                    return Json(new { Status = "Error", Message = errMsg });
            }
        }
        public ActionResult UploadFile(string refLevel1, string refLevel2, string fileSize, string refNo)
        {

            var obj = PutUploadFile(refLevel1, refLevel2, fileSize, _serverFilePath, (int)Enums.E_FormFileType.CompanyLicense, refNo);
            if (obj.Item1 == "S")
            {
                _fileModel = new FileDetailModel();
                // _fileModel.FileName = obj.Item2.FileName;
                _fileModel.FileName = (obj.Item2.FileName ?? "").Replace("'", "''");
                _fileModel.FileCode = obj.Item2.FileCode;
                _fileModel.FileSize = obj.Item2.FileSize;
                _fileModel.FileType = obj.Item2.FileType;
                //_fileModel.RefNo = obj.Item2.RefNo;
                _fileModel.RefNo = (obj.Item2.RefNo ?? "").Replace("'", "''");
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
            _fileModel.FileType = (int)Enums.E_FormFileType.CompanyLicense;
            _fileModel.RefLevel1 = refLevel1;
            _fileModel.RefLevel2 = refLevel2;
            return Json(GetFileByParameters(_fileModel).OrderBy(o => o.FileID), JsonRequestBehavior.AllowGet);
        }
        
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllInfo()
        {
            var data = _dalObj.GetAllInfo(new CompanyLicenseBEL(), orderBy: "DESC"); ;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [ActionAuth]
        public ActionResult frmCompanyLicenseView()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult GetInfoByParams(CompanyLicenseBEL model)
        {
            var dMaster =(string.IsNullOrEmpty(model.AlarmDays)) ? _dalObj.GetAllInfo(model, orderBy: "DESC"):_dalObj.GetCompanyExpireLicense(model, orderBy: "DESC");
            if (dMaster.Any())
            {
                _fileModel = new FileDetailModel();
                var refL1 = dMaster.FirstOrDefault().CLID;
                _fileModel.RefLevel1 = refL1.ToString();
                _fileModel.FileType = (int)Enums.E_FormFileType.CompanyLicense;
                var dLevel1 = GetFileByParameters(_fileModel).OrderBy(o => o.FileID);
                return Json(new { dataMaster = dMaster, dataLevel1 = dLevel1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { dataMaster = "", dataLevel1 = "" }, JsonRequestBehavior.AllowGet);
            }
         }
    }
}