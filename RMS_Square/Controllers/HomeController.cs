using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RMS_Square.DAL.Gateway;
using RMS_Square.Models;
using RMS_Square.Universal.Gateway;
using Systems.Universal;
using Systems.Controllers;
using Systems.ActionFilter;
using RMS_Square.Areas.Regulatory.Models.DAO;
using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Areas.SA.Models.BEL;
using RMS_Square.Areas.SA.Models.DAL.DAO;
using System.IO;
using Systems.Models;
using RMS_Square.Universal;

namespace RMS_Square.Controllers
{
    public class HomeController : ControllerController
    {
        private FileDetailModel _fileModel = null;
        private HomePageDAO _dalObj = new HomePageDAO();
        DBHelper dbHelper = new DBHelper();

        public ActionResult LogOut()
        {
            Session["IsLogged"] = false;
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index", "LoginRegistration");
        }
        public ActionResult SessionOutMsg()
        {
            return View();
        }
        public ActionResult GetVideo()
        {
            var videoPath =
               Request.MapPath("~/Media/VideoFile.mp4");
            FileStream fs =
               new FileStream(videoPath, FileMode.Open);
            return new FileStreamResult(fs, "video/mp4");
        }
        [HttpPost]
        public ActionResult AppCloseTime()
        {
            var s = Session["UserId"];
            return View();
        }
        [HttpGet]
        public JsonResult GetDashboardData()
        {
            try
            {
                DBHelper dbHelper = new DBHelper();

                // ── 1. KPI Cards Data ──────────────────────────────────────
                string qryTotal = "SELECT COUNT(1) FROM PRODUCT_REGISTRATION_INFO WHERE NVL(IS_DELETE,'N')='N'";
                string qryPending = "SELECT COUNT(1) FROM PRODUCT_REGISTRATION_INFO WHERE NVL(IS_DELETE,'N')='N' AND NVL(APPROVAL_STATUS,'N')='N'";
                string qryExpired = "SELECT COUNT(1) FROM PRODUCT_REGISTRATION_INFO WHERE NVL(IS_DELETE,'N')='N' AND VALID_UPTO < SYSDATE";
                string qryExpiringSoon = "SELECT COUNT(1) FROM PRODUCT_REGISTRATION_INFO WHERE NVL(IS_DELETE,'N')='N' AND VALID_UPTO >= SYSDATE AND VALID_UPTO <= SYSDATE + 30";

                int totalActive = Convert.ToInt32(dbHelper.GetValue(qryTotal) ?? "0");
                int pendingApprovals = Convert.ToInt32(dbHelper.GetValue(qryPending) ?? "0");
                int expired = Convert.ToInt32(dbHelper.GetValue(qryExpired) ?? "0");
                int expiringSoon = Convert.ToInt32(dbHelper.GetValue(qryExpiringSoon) ?? "0");

                // ── 2. Chart Data: Authority Wise ──────────────────────────
                string qryBSTI = "SELECT COUNT(1) FROM PRODUCT_REGISTRATION_INFO WHERE AUTHORITY_TYPE = 'BSTI' AND NVL(IS_DELETE,'N')='N'";
                string qryDGDA = "SELECT COUNT(1) FROM PRODUCT_REGISTRATION_INFO WHERE AUTHORITY_TYPE = 'DGDA' AND NVL(IS_DELETE,'N')='N'";

                int bstiCount = Convert.ToInt32(dbHelper.GetValue(qryBSTI) ?? "0");
                int dgdaCount = Convert.ToInt32(dbHelper.GetValue(qryDGDA) ?? "0");

                var authorityData = new
                {
                    labels = new[] { "BSTI", "DGDA", "Others" },
                    values = new[] { bstiCount, dgdaCount, (totalActive - bstiCount - dgdaCount) }
                };

                var statusData = new
                {
                    labels = new[] { "Approved", "Pending", "Expired" },
                    values = new[] { (totalActive - pendingApprovals - expired), pendingApprovals, expired }
                };

                // ── 3. Table Data: Top 5 Expiring Soon ─────────────────────
                string qryTable = @"SELECT * FROM (
                              SELECT P.PRODUCT_NAME, C.COMPANY_NAME, TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy') AS VALID_UPTO, ROUND(D.VALID_UPTO - SYSDATE, 0) AS DAYS_LEFT 
                              FROM PRODUCT_REGISTRATION_INFO D
                              LEFT JOIN PRODUCT_INFO P ON D.PRODUCT_CODE = P.PRODUCT_CODE
                              LEFT JOIN COMPANY_INFO C ON D.COMPANY_CODE = C.COMPANY_CODE
                              WHERE NVL(D.IS_DELETE,'N')='N' AND D.VALID_UPTO >= SYSDATE AND D.VALID_UPTO <= SYSDATE + 60 
                              ORDER BY D.VALID_UPTO ASC
                            ) WHERE ROWNUM <= 5";

                // DBConnection বাদ দিয়ে সরাসরি GetDataTable কল করা হলো
                DataTable dtExpiring = dbHelper.GetDataTable(qryTable);

                var expiringList = new List<object>();

                if (dtExpiring != null)
                {
                    foreach (DataRow row in dtExpiring.Rows)
                    {
                        expiringList.Add(new
                        {
                            ProductName = row["PRODUCT_NAME"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            ValidUpto = row["VALID_UPTO"].ToString(),
                            DaysLeft = row["DAYS_LEFT"].ToString()
                        });
                    }
                }

                // Return everything as a single JSON object
                return Json(new
                {
                    KPI = new { Total = totalActive, Pending = pendingApprovals, Expired = expired, ExpiringSoon = expiringSoon },
                    StatusChart = statusData,
                    AuthorityChart = authorityData,
                    ExpiringTable = expiringList
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetDashboardChartData()
        {
            try
            {
                // ==========================================
                // 1. Status Chart Data (Approved, Pending, Expired)
                // ==========================================
                string qryApproved = "SELECT COUNT(1) FROM PRODUCT_REGISTRATION_INFO WHERE NVL(IS_DELETE,'N')='N' AND APPROVAL_STATUS = 'Y'";
                string qryPending = "SELECT COUNT(1) FROM PRODUCT_REGISTRATION_INFO WHERE NVL(IS_DELETE,'N')='N' AND (APPROVAL_STATUS = 'N' OR APPROVAL_STATUS IS NULL)";
                string qryExpired = "SELECT COUNT(1) FROM PRODUCT_REGISTRATION_INFO WHERE NVL(IS_DELETE,'N')='N' AND VALID_UPTO < SYSDATE";

                int approvedCount = Convert.ToInt32(dbHelper.GetValue(qryApproved));
                int pendingCount = Convert.ToInt32(dbHelper.GetValue(qryPending));
                int expiredCount = Convert.ToInt32(dbHelper.GetValue(qryExpired));

                // ==========================================
                // 2. Applications by Category Data
                // ==========================================
                // উদাহরণস্বরূপ: ক্যাটাগরি অনুযায়ী প্রোডাক্টের সংখ্যা
                string qryLocal = "SELECT COUNT(1) FROM PRODUCT_INFO WHERE MANUFACTURING_TYPE = 'Local'";
                string qryImport = "SELECT COUNT(1) FROM PRODUCT_INFO WHERE MANUFACTURING_TYPE = 'Import'";

                int localCount = Convert.ToInt32(dbHelper.GetValue(qryLocal));
                int importCount = Convert.ToInt32(dbHelper.GetValue(qryImport));

                // সব ডাটা একটি JSON অবজেক্টে প্যাক করে পাঠানো হচ্ছে
                var chartData = new
                {
                    StatusData = new
                    {
                        labels = new[] { "Approved", "Pending", "Expired" },
                        values = new[] { approvedCount, pendingCount, expiredCount }
                    },
                    CategoryData = new
                    {
                        labels = new[] { "Local Production", "Imported" },
                        values = new[] { localCount, importCount }
                    }
                };

                return Json(chartData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult frmNewIndex()
        {
            #region Dashboard Notification info

            //ViewBag.RDNProductReg = Convert.ToInt32(CountReceiveDateNotify(tableName: "PRODUCT_REGISTRATION_INFO", type: string.Empty));
            //ViewBag.RDNMA = Convert.ToInt32(CountReceiveDateNotify(tableName: "MARKET_AUTH_CERTIFICATE", type: string.Empty));
            //ViewBag.RDNRecipe = Convert.ToInt32(CountReceiveDateNotify(tableName: "RECIPE_INFO", type: string.Empty));
            //ViewBag.RDNNoc = Convert.ToInt32(CountReceiveDateNotify(tableName: "NOC_MST", type: string.Empty));

            //ViewBag.RDNDTL = Convert.ToInt32(CountReceiveDateNotify(tableName: "PRODUCT_REGISTRATION_INFO", type: "DTL"));
            //ViewBag.RDNAnnexAmd = Convert.ToInt32(CountReceiveDateNotify(tableName: "PRODUCT_REGISTRATION_INFO", type: "AnnexAmd"));
            //ViewBag.RDNPackAmd = Convert.ToInt32(CountReceiveDateNotify(tableName: "PRODUCT_REGISTRATION_INFO", type: "PackAmd"));

            //ViewBag.RDNPFC = Convert.ToInt32(CountReceiveDateNotify(tableName: "PRICE_FIXATION_INFO", type: "ChangeSMD"));
            //ViewBag.RDNPrice = Convert.ToInt32(CountReceiveDateNotify(tableName: "PRODUCT_PRICE", type: "ChangeReceiveDate"));
            //ViewBag.RDNAdvertisement = Convert.ToInt32(CountReceiveDateNotify(tableName: "ADVERTISEMENT_INFO", type: "ChangeReceiveDate"));
            //ViewBag.RDNPromotional = Convert.ToInt32(CountReceiveDateNotify(tableName: "PROMOTIONAL_INFO", type: "ChangeReceiveDate"));

            #endregion

            #region Dashboard Keyword Data

            ViewBag.ExpireComLcn = Convert.ToInt32(CountCompanyLicExpiry()); //new CompanyLicenseDAO().GetCompanyExpireLicense(new CompanyLicenseBEL(), orderBy: "DESC").Count;
            ViewBag.ExpireGmpLcn = Convert.ToInt32(CountGmpLicExpiry());//new GmpCertificateInfoDAO().GetGmpExpireLicense(new GmpCertificateInfoBEL(), orderBy: "DESC").Count;
            ViewBag.ExpireProductLcn = Convert.ToInt32(CountProductLicExpiry()); //new ProductRegistrationDAO().GetProductExpireLicense(new ProductRegistrationBEL(), orderBy: "DESC").Count;
            ViewBag.ExpireNarcoticLcn = Convert.ToInt32(CountNarcoticLicExpiry());// new NarcoticLicenseDAO().GetNarcoticExpireLicense(new NarcoticLicenseBEL(), orderBy: "DESC").Count;
            ViewBag.ExpireImpProductLcn = Convert.ToInt32(CountImpProductLicExpiry()); //new ImportProductRegistrationDAO().GetImportProductExpireLicense(new ImportProductRegistrationBEL(), orderBy: "DESC").Count;
            ViewBag.ExpireRecipeNotify = Convert.ToInt32(CountRecipeLicExpiry()); //new RecipeDAO().GetRecipeNotify(new RecipeBEL(), orderBy: "DESC").Count;
            ViewBag.ExpireAdvNotify = Convert.ToInt32(CountAdvertiseLicExpiry()); //new AdvertisementInfoDAO().GetAdvertiseNotify(new AdvertisementInfoBEL(), orderBy: "DESC").Count;
            ViewBag.ExpireMACertificate = Convert.ToInt32(CountMacLicExpiry()); //new MarketAuthCertificateDAO().GetMACertificateExpiry(new MarketAuthCertificateBEL(), orderBy: "DESC").Count;

            #endregion

            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetAllInfo(string fromDt, string toDt)
        {
            var data = _dalObj.GetAllInfo(fromDt, toDt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetAllPending(string CompanyCode, string rptType, string fromDt, string toDt)
        {
            var data = _dalObj.GetAllPending(CompanyCode, rptType, fromDt, toDt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private string CountReceiveDateNotify(string tableName, string type)
        {
            string qry = string.Empty;
            if (string.IsNullOrEmpty(type))
            {
                qry = "SELECT count(1) CountTotal FROM " + tableName + " WHERE RECEIVE_DATE is not null and SUBMISSION_DATE is null";
            }
            else if (type == "DTL")
            {
                qry = @"SELECT count(1) CountTotal FROM PRODUCT_REGISTRATION_INFO D LEFT JOIN  RECIPE_INFO R ON R.ID=D.RECIPE_ID
 LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE WHERE P.PRODUCT_SPECIFICATION ='INN' and D.RECEIVE_DATE is not null and D.SUBMISSION_DATE is null";
            }
            else if (type == "AnnexAmd")
            {
                qry = "SELECT count(1) CountTotal FROM " + tableName + " WHERE RECEIVE_DATE is not null and SUBMISSION_DATE is null AND STATE_STATUS='Annexure Amendment' ";
            }
            else if (type == "PackAmd")
            {
                qry = "SELECT count(1) CountTotal FROM " + tableName + " WHERE RECEIVE_DATE is not null and SUBMISSION_DATE is null AND STATE_STATUS='Packaging Amendment' ";
            }
            else if (type == "ChangeReceiveDate")
            {
                qry = "SELECT count(1) CountTotal FROM " + tableName + " WHERE PROPOSAL_DATE is not null and SUBMISSION_DATE is null";
            }
            else if (type == "ChangeSMD")
            {
                qry = "SELECT count(1) CountTotal FROM " + tableName + " WHERE RECEIVE_DATE is not null and PRICE_SUB_DATE is null";
            }

            string sTotal = dbHelper.GetValue(qry);
            return sTotal;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShowProductSummary(string ProductCode, string CompanyCode)
        {
       
            var data = _dalObj.ShowProductSummary(ProductCode, CompanyCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetMeetingInfo()
        {
            string user = Session["enUserID"].ToString();
            var data = _dalObj.GetMeetingInfo(); ;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult frmProductReport()
        {
            return View();
        }

        public ActionResult GetFileByRefId(string refLevel1, string refLevel2)
        {
            _fileModel = new FileDetailModel();
            _fileModel.FileType = (int)Enums.E_FormFileType.RecipeInfo;
            _fileModel.RefLevel1 = refLevel1;
            _fileModel.RefLevel2 = refLevel2;
            return Json(GetFileByParameters(_fileModel).OrderBy(o => o.FileID), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShowProductList(string fromDt, string toDt, string DType, string CompanyCode)
        {
            var data = _dalObj.ShowProductList(fromDt, toDt, DType, CompanyCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCompanyLicenseDashboardData()
        {
            try
            {
                var dao = new CompanyLicenseDAO();

                // সব company license আনো (IS_DELETE <> 'Y')
                var allLicenses = dao.GetAllInfo(new CompanyLicenseBEL(), "DESC");

                int total = allLicenses.Count;
                int expired = 0;
                int expiringSoon = 0; // 30 দিনের মধ্যে expire হবে
                int active = 0;

                var expiringTableList = new List<object>();

                foreach (var lic in allLicenses)
                {
                    if (!string.IsNullOrEmpty(lic.ValidUpto))
                    {
                        DateTime validDate;
                        if (DateTime.TryParseExact(lic.ValidUpto, "dd/MM/yyyy",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out validDate))
                        {
                            int daysLeft = (validDate - DateTime.Now.Date).Days;

                            if (daysLeft < 0)
                            {
                                expired++;
                                expiringTableList.Add(new
                                {
                                    CompanyName = lic.CompanyName ?? lic.CompanyCode,
                                    LicenseNo = lic.LicenseNo,
                                    ValidUpto = lic.ValidUpto,
                                    DaysLeft = daysLeft
                                });
                            }
                            else if (daysLeft <= 30)
                            {
                                expiringSoon++;
                                expiringTableList.Add(new
                                {
                                    CompanyName = lic.CompanyName ?? lic.CompanyCode,
                                    LicenseNo = lic.LicenseNo,
                                    ValidUpto = lic.ValidUpto,
                                    DaysLeft = daysLeft
                                });
                            }
                            else
                            {
                                active++;
                            }
                        }
                        else
                        {
                            active++; // parse না হলে active ধরে নাও
                        }
                    }
                    else
                    {
                        active++; // ValidUpto empty হলে active ধরো
                    }
                }

                // Pending = যেগুলোর ApprovalDate নেই
                int pending = allLicenses.Count(x => string.IsNullOrEmpty(x.ApprovalDate));

                // Sort expiring table: expired first (negative days), then by days ascending
                var sortedExpiringTable = expiringTableList
                    .OrderBy(x => ((dynamic)x).DaysLeft)
                    .Take(10)
                    .ToList();

                var result = new
                {
                    KPI = new
                    {
                        Total = total,
                        ExpiringSoon = expiringSoon,
                        Pending = pending,
                        Expired = expired
                    },
                    StatusChart = new
                    {
                        labels = new[] { "Active", "Expiring Soon", "Expired" },
                        values = new[] { active, expiringSoon, expired }
                    },
                    ExpiringTable = sortedExpiringTable
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        private string CountCompanyLicExpiry()
        {
            string qry = @"SELECT COUNT(1)ComLicCount FROM ( SELECT CL.CLID, CL.REVISION_NO,CL.COMPANY_CODE,CL.VALID_UPTO,CL.NOTIFICATION_DAYS
 FROM COMPANY_LICENSE CL LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=CL.COMPANY_CODE WHERE IS_DELETE <>'Y' ) A INNER JOIN ( SELECT COMPANY_CODE, MAX(REVISION_NO) AS MaxRvNo  FROM COMPANY_LICENSE GROUP BY COMPANY_CODE)
 B ON A.COMPANY_CODE=B.COMPANY_CODE AND A.REVISION_NO=B.MaxRvNo WHERE ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS  ";
            string sTotal = dbHelper.GetValue(qry);
            return sTotal;
        }
        private string CountGmpLicExpiry()
        {
            string qry = @"SELECT COUNT(1) GmpCount FROM ( SELECT D.REVISION_NO,D.COMPANY_CODE,D.VALID_UPTO,D.NOTIFICATION_DAYS FROM GMP_CERTIFICATION D LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE WHERE IS_DELETE <>'Y'  ) A 
 INNER JOIN ( SELECT COMPANY_CODE, MAX(REVISION_NO) AS MaxRvNo  FROM GMP_CERTIFICATION GROUP BY COMPANY_CODE) B ON A.COMPANY_CODE=B.COMPANY_CODE AND A.REVISION_NO=B.MaxRvNo WHERE 1=1  AND ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ";
            string sTotal = dbHelper.GetValue(qry);
            return sTotal;
        }

        private string CountProductLicExpiry()
        {
            string qry = @"SELECT COUNT(1)ProductCount FROM ( SELECT D.REVISION_NO,D.RECIPE_ID,D.VALID_UPTO,D.NOTIFICATION_DAYS FROM PRODUCT_REGISTRATION_INFO D LEFT JOIN  RECIPE_INFO R ON R.ID=D.RECIPE_ID  LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE  WHERE D.IS_DELETE <>'Y' ) A 
 INNER JOIN (SELECT RECIPE_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_REGISTRATION_INFO GROUP BY RECIPE_ID) B ON B.RECIPE_ID=A.RECIPE_ID AND B.MaxRvNo=A.REVISION_NO  WHERE  ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ";
            string sTotal = dbHelper.GetValue(qry);
            return sTotal;
        }
        private string CountNarcoticLicExpiry()
        {
            string qry = @"SELECT COUNT(1)NarcoticCount FROM ( SELECT D.REVISION_NO,D.COMPANY_CODE,D.VALID_UPTO,D.NOTIFICATION_DAYS FROM NARCOTIC_LICENSE D LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE WHERE IS_DELETE <>'Y'  ) A 
 INNER JOIN ( SELECT COMPANY_CODE, MAX(REVISION_NO) AS MaxRvNo  FROM NARCOTIC_LICENSE GROUP BY COMPANY_CODE) B ON A.COMPANY_CODE=B.COMPANY_CODE AND A.REVISION_NO=B.MaxRvNo WHERE ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ";
            string sTotal = dbHelper.GetValue(qry);
            return sTotal;
        }
        private string CountImpProductLicExpiry()
        {
            string qry = @"SELECT COUNT(1) ImpCount FROM ( SELECT D.COMPANY_CODE, D.PRODUCT_CODE ,D.REVISION_NO, D.VALID_UPTO,D.NOTIFICATION_DAYS FROM IMPORT_PRODUCT_REGISTRATION D  LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE  LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE  WHERE D.IS_DELETE <>'Y') A 
 INNER JOIN ( SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM IMPORT_PRODUCT_REGISTRATION GROUP BY COMPANY_CODE, PRODUCT_CODE) B  ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO  WHERE ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ";
            string sTotal = dbHelper.GetValue(qry);
            return sTotal;
        }
        private string CountRecipeLicExpiry()
        {
            string qry = @"SELECT COUNT(1)RecipeCount FROM ( SELECT D.ID, D.REVISION_NO,D.COMPANY_CODE, D.VALID_UPTO,D.NOTIFICATION_DAYS,P.PRODUCT_CODE FROM RECIPE_INFO D  LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE  LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ) A  
  INNER JOIN ( SELECT PRODUCT_CODE,COMPANY_CODE,MAX(REVISION_NO) AS MaxRvNo FROM RECIPE_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) B ON B.PRODUCT_CODE=A.PRODUCT_CODE AND B.COMPANY_CODE=A.COMPANY_CODE AND B.MaxRvNo=A.REVISION_NO LEFT JOIN PRODUCT_REGISTRATION_INFO PR ON PR.RECIPE_ID=A.ID WHERE PR.RECIPE_ID is null AND ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ";
            string sTotal = dbHelper.GetValue(qry);
            return sTotal;
        }
        private string CountAdvertiseLicExpiry()
        {
            string qry = @"SELECT COUNT(1)AdvCount FROM ( SELECT D.REVISION_NO,D.COMPANY_CODE,D.VALID_UPTO,D.NOTIFICATION_DAYS,P.PRODUCT_CODE FROM ADVERTISEMENT_INFO D  LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE  LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ) A 
 INNER JOIN ( SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM ADVERTISEMENT_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) B ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO  WHERE ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ";
            string sTotal = dbHelper.GetValue(qry);
            return sTotal;
        }
        private string CountMacLicExpiry()
        {
            string qry = @"SELECT COUNT(1)MACCount FROM (SELECT D.REVISION_NO,D.VALID_UPTO,D.NOTIFICATION_DAYS,P.PRODUCT_CODE,C.COMPANY_CODE FROM MARKET_AUTH_CERTIFICATE D  LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ) A 
 INNER JOIN ( SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM MARKET_AUTH_CERTIFICATE GROUP BY COMPANY_CODE,PRODUCT_CODE) B  ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO WHERE ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ";
            string sTotal = dbHelper.GetValue(qry);
            return sTotal;
        }
    }
}