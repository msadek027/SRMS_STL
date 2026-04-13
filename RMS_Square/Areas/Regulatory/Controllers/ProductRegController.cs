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
    public class ProductRegController : ControllerController
    {
        private FileDetailModel _fileModel = null;
        private static string _serverFilePath = string.Empty;
        private ProductRegistrationDAO _dalObj = null;

        public ProductRegController()
        {
            _serverFilePath = Utility.GetServerPath();
            _dalObj = new ProductRegistrationDAO();
        }
        //
        // GET: /Regulatory/ProductReg/
        [ActionAuth]
        public ActionResult frmProductReg()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.PageAccess = GetAccessLevel(Session["ROLEID"] as string, "155");
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult frmProductReg(ProductRegistrationBEL model)
        {
            try
            {
                if (_dalObj.SaveUpdate(model, userId: Session["UserID"] as string))
                {
                    return Json(new { AnnexId = _dalObj.ReturnMaxID, AnnexureNo = _dalObj.MaxID, Mode = _dalObj.IUMode, Status = "Yes", AnnexRevisionNo = _dalObj.RefNo });
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
        }
        public ActionResult UploadFile(string refLevel1, string refLevel2, string fileSize, string refNo, string sStatus)
        {

            var obj = PutUploadFile(refLevel1, refLevel2, fileSize, _serverFilePath, (int)Enums.E_FormFileType.ProductRegistration, refNo);
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
                var model = new ProductRegistrationBEL();
                if (isSave)
                {
                    DataTable dt = _dalObj.GetFileRefno((int)Enums.E_FormFileType.ProductRegistration, refLevel1, sStatus);
                    bool isAll = false;
                    if (dt.Rows.Count > 0)
                    {
                        if (sStatus.Equals("New"))
                        {
                            if (Convert.ToInt32(dt.Rows[0]["PA"].ToString()) > 0 && Convert.ToInt32(dt.Rows[0]["PPM"].ToString()) > 0 && Convert.ToInt32(dt.Rows[0]["SD"].ToString()) > 0)
                            {
                                isAll = true;
                            }
                        }
                        else if (sStatus.Equals("Renew"))
                        {
                            if (Convert.ToInt32(dt.Rows[0]["PA"].ToString()) > 0 && Convert.ToInt32(dt.Rows[0]["PPM"].ToString()) > 0)
                            {
                                isAll = true;
                            }
                        }
                        else if (sStatus.Equals("Annexure Amendment"))
                        {
                            if (Convert.ToInt32(dt.Rows[0]["PA"].ToString()) > 0 && Convert.ToInt32(dt.Rows[0]["AJ"].ToString()) > 0 && Convert.ToInt32(dt.Rows[0]["SD"].ToString()) > 0)
                            {
                                isAll = true;
                            }
                        }
                        else if (sStatus.Equals("Packaging Amendment"))
                        {
                            if (Convert.ToInt32(dt.Rows[0]["AJ"].ToString()) > 0 && Convert.ToInt32(dt.Rows[0]["PPM"].ToString()) > 0)
                            {
                                isAll = true;
                            }
                        }
                       
                    }
                    if (isAll)//

                    {
                        model.AnnexId = Convert.ToInt64(refLevel1);
                        model.ReceivedDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                        _dalObj.UpdateFileRelatedInfo(model, userId: Session["UserID"] as string);
                    }
                }

                return Json(new { msgType = "FUS", ReceiveDate = model.ReceivedDate, FileList = GetFileByParameters(_fileModel).OrderByDescending(o => o.FileID) }, JsonRequestBehavior.AllowGet);
               
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
            _fileModel.FileType = (int)Enums.E_FormFileType.ProductRegistration;
            _fileModel.RefLevel1 = refLevel1;
            _fileModel.RefLevel2 = refLevel2;
            return Json(GetFileByParameters(_fileModel).OrderBy(o => o.FileID), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllInfo()
        {
            var data = _dalObj.GetAllInfo(new ProductRegistrationBEL(), orderBy: "DESC");
            //return Json(data, JsonRequestBehavior.AllowGet);
            return new JsonResult()
            {
                Data = data,
                MaxJsonLength = Int32.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllInfoByCompany(string companyCode)
        {
            var model = new ProductRegistrationBEL();
            model.CompanyCode = companyCode;
            var data = _dalObj.GetAllInfo(model, orderBy: "DESC");
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [ActionAuth]
        public ActionResult frmProductRegView()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
        [HttpPost]
        public ActionResult GetInfoByParams(ProductRegistrationBEL model)
        {
            var dMaster = _dalObj.GetAllInfo(model, orderBy: "DESC");
            if (dMaster.Any())
            {
                _fileModel = new FileDetailModel();
                var refL1 = dMaster.FirstOrDefault().AnnexId;
                _fileModel.RefLevel1 = refL1.ToString();
                _fileModel.FileType = (int)Enums.E_FormFileType.ProductRegistration;
                var dLevel1 = GetFileByParameters(_fileModel).OrderBy(o => o.FileID);
                return Json(new { dataMaster = dMaster, dataLevel1 = dLevel1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { dataMaster = "", dataLevel1 = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult frmProductRegRpt()
        {
            if (Session["UserID"] != null)
            {
                Session["FormNameTitle"] = "Product Registration Report";
                return View();
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }

        [HttpPost]
        public ActionResult frmProductRegRpt(ReportModel model)
        {
            if (Session["UserId"] != null)
            {
                var reportDocument = new ReportDocument();
                var rptPath = Server.MapPath("~/Reports");
                DataTable dt = new DataTable();

                string downFileName = string.Empty;
                string fromTodate = "_From_" + model.FromDate + "_To_" + model.ToDate + "_";
                dt = _dalObj.GetProductRegForReport(model, "ASC");
                if(model.ReportType!="HTML")
                {
                    string reportName = "";
                    if(model.ReportFormat =="General")
                    {
                        if (model.StateStatus == "Annexure Amendment")
                        {
                            rptPath = rptPath + "/ProductRegRpt.rpt";
                            reportName = "ProductRegRpt" + "_" + fromTodate;
                            model.ReportFormat = "General Report";
                        }
                        else
                        {
                            rptPath = rptPath + "/ProductRegMainRpt.rpt";
                            reportName = "ProductRegRpt" + "_" + fromTodate;
                            model.ReportFormat = "General Report";
                        }     
                    }
                    else
                    {
                        rptPath = rptPath + "/INNProductRegRpt.rpt";
                        reportName = "INNProductRegRpt" + "_" + fromTodate;
                    }
                                  
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dt);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("CompanyName", Session["COMPANY_NAME"]);
                    reportDocument.SetParameterValue("DevBy", Session["DEV_BY"]);
                    reportDocument.SetParameterValue("ProjectName", Session["ProjectName"]);
                    reportDocument.SetParameterValue("FromDate", model.FromDate ?? "");
                    reportDocument.SetParameterValue("ToDate", model.ToDate ?? "");
                    reportDocument.SetParameterValue("StateStatus", model.StateStatus ?? "All");
                    reportDocument.SetParameterValue("ChooseOption", model.ChooseOption ?? "All");
                    reportDocument.SetParameterValue("ReportFormat", model.ReportFormat ?? "All");
                        
                    downFileName = reportName + DateTime.Now.ToString("yyyyMMdd'_'HHmmss");
                    reportDocument.ExportToHttpResponse(model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, downFileName);
                    reportDocument.Close();
                    reportDocument.Dispose();
                    return View();
                }
                else
                {
                    var item = (from DataRow row in dt.Rows
                                select new ProductRegistrationBEL
                                {
                                    SN=Convert.ToInt64(row["SN"]),
                                    AnnexId = Convert.ToInt64(row["ANNEX_ID"]),
                                    AnnexureNo = row["ANNEXURE_NO"].ToString(),
                                    RecipeId = Convert.ToInt64(row["RECIPE_ID"]),
                                    AnnexRevisionNo = row["REVISION_NO"].ToString(),
                                    DarNo = row["DAR_NO"].ToString(),
                                    DtlReceivedDate = row["DTL_RECEIVE_DATE"].ToString(),
                                    DtlSubmissionDate = row["DTL_SUBMISSION_DATE"].ToString(),
                                    DtlApprovalDate = row["DTL_APPROVAL_DATE"].ToString(),
                                    ProposalDate = row["PROPOSAL_DATE"].ToString(),
                                    ReceivedDate = row["RECEIVE_DATE"].ToString(),
                                    SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                                    InclusionDate = row["INCLUSION_DATE"].ToString(),
                                    ValidUptoDate = row["VALID_UPTO"].ToString(),
                                    ProposedBy = row["PROPOSED_BY"].ToString(),
                                    CompanyCode = row["COMPANY_CODE"].ToString(),
                                    CompanyName = row["COMPANY_NAME"].ToString(),
                                    LicenseNo = row["LICENSE_NO"].ToString(),
                                    ProductCode = row["PRODUCT_CODE"].ToString(),
                                    SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                                    GenAndStrength = row["GENERIC_CODE"].ToString(),
                                    PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                                    DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                                    ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                                    BrandName = row["BRAND_NAME"].ToString(),
                                    ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString()
                                }).ToList();

                    var data = item.OrderByDescending(s=>s.AnnexId);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            return Redirect(string.Format("~/Home/frmHome"));
        }
    }
}