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
    public class PromotionInfoController : ControllerController
    {



        private FileDetailModel _fileModel = null;
        private static string _serverFilePath = string.Empty;
        private PromotionInfoDAO _dalObj = null;

        public PromotionInfoController()
        {
            //_fileDetail = new FileDetailModel();
            _serverFilePath = Utility.GetServerPath();
            _dalObj = new PromotionInfoDAO();
        }


        //
        // GET: /Regulatory/PromotionInfo/
        [ActionAuth]
        public ActionResult frmPromotionInfo()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.PageAccess = GetAccessLevel(Session["ROLEID"] as string, "166");
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        [HttpPost]
        public ActionResult frmPromotionInfo(PromotionInfoBEL master)
        {
            try
            {
                string userId = Session["UserID"] as string;

                if (_dalObj.SaveUpdate(master, userId))
                {
                    return Json(new { ID = _dalObj.ReturnMaxID, RevisionNo = _dalObj.RefNo, Mode = _dalObj.IUMode, Status = "Yes" });
                }
                else
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
        }
        public ActionResult UploadFile(string refLevel1, string refLevel2, string fileSize, string refNo)
        {

            var obj = PutUploadFile(refLevel1, refLevel2, fileSize, _serverFilePath, (int)Enums.E_FormFileType.PromotionalInfo, refNo);
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
                var model = new PromotionInfoBEL();
                if (isSave)
                {
                    DataTable dt = _dalObj.GetFileRefno((int)Enums.E_FormFileType.PromotionalInfo, refLevel1);
                    bool isAll = false;
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dt.Rows[0]["PD"].ToString()) > 0)
                        {
                            isAll = true;
                        }
                    }
                    if (isAll)//
                    {
                        model.ID = Convert.ToInt64(refLevel1);
                        model.ProposalDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                        _dalObj.UpdateFileRelatedInfo(model, userId: Session["UserID"] as string);
                    }
                }

                return Json(new { msgType = "FUS", ReceiveDate = model.ProposalDate, FileList = GetFileByParameters(_fileModel).OrderByDescending(o => o.FileID) }, JsonRequestBehavior.AllowGet);
                
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
            _fileModel.FileType = (int)Enums.E_FormFileType.PromotionalInfo;
            _fileModel.RefLevel1 = refLevel1;
            _fileModel.RefLevel2 = refLevel2;
            return Json(GetFileByParameters(_fileModel).OrderBy(o => o.FileID), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllInfo()
        {
            var data = _dalObj.GetPromotionInfo(new PromotionInfoBEL(), orderBy: "DESC");
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetPromotionList()
        {
            var data = _dalObj.GetPromotionInfo(new PromotionInfoBEL(), orderBy: "DESC");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetProductList()
        {
            var data = _dalObj.GetProductInfo();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [ActionAuth]
        public ActionResult frmPromotionInfoView()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult GetInfoByParams(PromotionInfoBEL model)
        {
            var dMaster = _dalObj.GetPromotionInfo(model, orderBy: "DESC");
            if (dMaster.Any())
            {
                _fileModel = new FileDetailModel();
                var refL1 = dMaster.FirstOrDefault().ID;
                _fileModel.RefLevel1 = refL1.ToString();
                _fileModel.FileType = (int)Enums.E_FormFileType.PromotionalInfo;
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