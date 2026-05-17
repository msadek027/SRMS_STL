using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.DAL.Gateway;
using RMS_Square.Universal.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.DAO
{

    public class ProductRegistrationDAO : ReturnData
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;
        public ProductRegistrationDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }
        //Old
        /*public bool SaveUpdate(ProductRegistrationBEL model, string userId)
        {
            try
            {
                var query = new StringBuilder();
                if (model.AnnexId > 0)
                {
                    //U for update
                    ReturnMaxID = model.AnnexId;
                    MaxID = model.AnnexureNo;
                    RefNo = model.AnnexRevisionNo;
                    IUMode = "U";
                    query.Append(" UPDATE PRODUCT_REGISTRATION_INFO SET RECIPE_ID='" + model.RecipeId + "', DAR_NO='" + model.DarNo + "', DTL_REMARKS='" + model.DtlRemarks + "',");
                    query.Append(" DTL_RECEIVE_DATE =(TO_DATE('" + model.DtlReceivedDate + "','dd/MM/yyyy')), DTL_SUBMISSION_DATE =(TO_DATE('" + model.DtlSubmissionDate + "','dd/MM/yyyy')),");
                    query.Append(" DTL_APPROVAL_DATE =(TO_DATE('" + model.DtlApprovalDate + "','dd/MM/yyyy')), PROPOSAL_DATE =(TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')),");
                    query.Append(" RECEIVE_DATE =(TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')), SUBMISSION_DATE =(TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')),");
                    query.Append(" INCLUSION_DATE =(TO_DATE('" + model.InclusionDate + "','dd/MM/yyyy')), VALID_UPTO =(TO_DATE('" + model.ValidUptoDate + "','dd/MM/yyyy')),");
                    query.Append(" RENEWAL_DATE =(TO_DATE('" + model.RenewalDate + "','dd/MM/yyyy')),");
                    query.Append(" ANNEXURE_STATUS='" + model.AnnexStatus + "', REMARKS='" + model.Remarks + "',");
                    query.Append(" AUTHORITY_TYPE='" + model.AuthorityType + "', AUTHORITY_NAME='" + model.AuthorityName + "',");
                    query.Append(" AUTORITY_LICENSE_NO='" + model.AuthorityLicenseNo + "', AUTHORITY_LICENSE_NAME='" + model.AuthorityLicenseName + "',");
                    query.Append(" NOTIFICATION_DAYS='" + model.AlarmDays + "', PROPOSED_BY='" + model.ProposedBy + "',");
                    query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                    query.Append(" WHERE ANNEX_ID='" + model.AnnexId + "'");
                }
                else
                { //I for Insert  
                    ReturnMaxID = _idGenerated.getMAXSL("PRODUCT_REGISTRATION_INFO", "ANNEX_ID");
                    // company & productcode wise group needed
                    string strCount = _dbHelper.GetValue("SELECT COUNT(1) AS RevisionNo FROM PRODUCT_REGISTRATION_INFO  WHERE IS_DELETE='N' AND RECIPE_ID='" + model.RecipeId + "'  GROUP BY RECIPE_ID");
                    if (!string.IsNullOrEmpty(strCount))
                    {
                        RefNo = strCount;
                        if (!string.IsNullOrEmpty(model.AnnexureNo))
                        {
                            MaxID = model.AnnexureNo;
                        }
                        else
                        {
                           MaxID= _dbHelper.GetValue("SELECT MAX(ANNEXURE_NO)ANNEXURE_NO FROM PRODUCT_REGISTRATION_INFO  WHERE IS_DELETE='N' AND RECIPE_ID='" + model.RecipeId + "'  GROUP BY RECIPE_ID");
                          // MaxID = _idGenerated.getMAXID("PRODUCT_REGISTRATION_INFO", "ANNEXURE_NO", "fm000000000");
                        }
                    }
                    else
                    {
                        RefNo = "0";
                        MaxID = _idGenerated.getMAXID("PRODUCT_REGISTRATION_INFO", "ANNEXURE_NO", "fm000000000");
                        model.StateStatus = model.StateStatus ?? "New";
                    }

                   
                    IUMode = "I";
                    query.Append(" INSERT INTO PRODUCT_REGISTRATION_INFO(ANNEX_ID,ANNEXURE_NO,REVISION_NO,RECIPE_ID,DAR_NO,DTL_REMARKS,STATE_STATUS,");
                    query.Append(" DTL_RECEIVE_DATE,DTL_SUBMISSION_DATE,DTL_APPROVAL_DATE,PROPOSAL_DATE,RECEIVE_DATE,");
                    query.Append(" SUBMISSION_DATE,INCLUSION_DATE,RENEWAL_DATE,VALID_UPTO,REMARKS,ANNEXURE_STATUS,");
                    query.Append(" PROPOSED_BY,NOTIFICATION_DAYS,SET_BY,SET_ON,IS_DELETE) ");
                    query.Append(" AUTHORITY_TYPE,AUTHORITY_NAME,AUTORITY_LICENSE_NO,AUTHORITY_LICENSE_NAME) ");
                    query.Append(" VALUES( '" + ReturnMaxID + "','" + MaxID + "','" + RefNo + "','" + model.RecipeId + "','" + model.DarNo + "','" + model.DtlRemarks + "','" + model.StateStatus + "',");
                    query.Append(" (TO_DATE('" + model.DtlReceivedDate + "','dd/MM/yyyy')),(TO_DATE('" + model.DtlSubmissionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.DtlApprovalDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')),");
                    query.Append(" (TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')),(TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.InclusionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.RenewalDate + "','dd/MM/yyyy')),");
                    query.Append(" (TO_DATE('" + model.ValidUptoDate + "','dd/MM/yyyy')),'" + model.Remarks + "','" + model.AnnexStatus + "',");
                    query.Append(" '" + model.ProposedBy + "','" + model.AlarmDays + "','" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss'))");
                    query.Append(" ,'N')");
                }
                if (_dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception errorException)
            {
                throw errorException;
            }
        }*/

        public bool SaveUpdate(ProductRegistrationBEL model, string userId)
        {
            try
            {
                var query = new StringBuilder();
                string currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                // SQL Injection এবং String Formatting এর জন্য কিছু বেসিক ক্লিনআপ
                string darNo = (model.DarNo ?? "").Replace("'", "''").Trim();
                string remarks = (model.Remarks ?? "").Replace("'", "''").Trim();
                string authorityType = (model.AuthorityType ?? "").Replace("'", "''").Trim();
                string authorityName = (model.AuthorityName ?? "").Replace("'", "''").Trim();
                string authorityLicenseNo = (model.AuthorityLicenseNo ?? "").Replace("'", "''").Trim();
                string authorityLicenseName = (model.AuthorityLicenseName ?? "").Replace("'", "''").Trim();

                // যদি AnnexId বা কোনো প্রাইমারি কি 0 এর বেশি হয়, তবে আপডেট হবে
                if (model.AnnexId > 0)
                {
                    // U for update
                    IUMode = "U";
                    ReturnMaxID = model.AnnexId;

                    query.Append(" UPDATE PRODUCT_REGISTRATION_INFO SET ");
                    query.Append(" RECIPE_ID='" + model.RecipeId + "', PRODUCT_CODE='" + model.ProductCode +"', COMPANY_CODE='" + model.CompanyCode + "', DAR_NO='" + darNo + "', DTL_REMARKS='" + model.DtlRemarks + "',");

                    query.Append(" AUTHORITY_TYPE='" + authorityType + "', AUTHORITY_NAME='" + authorityName + "', AUTHORITY_LICENSE_NO='" + authorityLicenseNo + "', AUTHORITY_LICENSE_NAME='" + authorityLicenseName + "',");

                    // ডেট কলামগুলো
                    query.Append(" DTL_RECEIVE_DATE =(TO_DATE('" + model.DtlReceivedDate + "','dd/MM/yyyy')), DTL_SUBMISSION_DATE =(TO_DATE('" + model.DtlSubmissionDate + "','dd/MM/yyyy')),");
                    query.Append(" DTL_APPROVAL_DATE =(TO_DATE('" + model.DtlApprovalDate + "','dd/MM/yyyy')), PROPOSAL_DATE =(TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')),");
                    query.Append(" RECEIVE_DATE =(TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')), SUBMISSION_DATE =(TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')),");
                    query.Append(" INCLUSION_DATE =(TO_DATE('" + model.InclusionDate + "','dd/MM/yyyy')), VALID_UPTO =(TO_DATE('" + model.ValidUptoDate + "','dd/MM/yyyy')),");
                    query.Append(" RENEWAL_DATE =(TO_DATE('" + model.RenewalDate + "','dd/MM/yyyy')),");

                    // অন্যান্য কলাম
                    query.Append(" ANNEXURE_STATUS='" + model.AnnexStatus + "', REMARKS='" + remarks + "',");
                    query.Append(" NOTIFICATION_DAYS='" + model.AlarmDays + "', PROPOSED_BY='" + model.ProposedBy + "',");
                    query.Append(" UPDATE_DATE =(TO_DATE('" + currentDate + "','dd/MM/yyyy HH24:mi:ss')), UPDATE_BY='" + userId + "'");
                    query.Append(" WHERE ANNEX_ID='" + model.AnnexId + "'");
                }
                else
                {
                    model.StateStatus = model.StateStatus ?? "New";
                    // I for Insert  
                    IUMode = "I";
                    ReturnMaxID = _idGenerated.getMAXSL("PRODUCT_REGISTRATION_INFO", "ANNEX_ID");

                    query.Append(" INSERT INTO PRODUCT_REGISTRATION_INFO (");
                    query.Append(" ANNEX_ID, ANNEXURE_NO, REVISION_NO, RECIPE_ID, PRODUCT_CODE,COMPANY_CODE, DAR_NO, DTL_REMARKS, STATE_STATUS,");
                    // নতুন ৪টি কলাম (বানান AUTHORITY_LICENSE_NO ঠিক করা হয়েছে)
                    query.Append(" AUTHORITY_TYPE, AUTHORITY_NAME, AUTHORITY_LICENSE_NO, AUTHORITY_LICENSE_NAME,");
                    query.Append(" DTL_RECEIVE_DATE, DTL_SUBMISSION_DATE, DTL_APPROVAL_DATE, PROPOSAL_DATE, RECEIVE_DATE,");
                    query.Append(" SUBMISSION_DATE, INCLUSION_DATE, RENEWAL_DATE, VALID_UPTO, REMARKS, ANNEXURE_STATUS,");
                    query.Append(" PROPOSED_BY, NOTIFICATION_DAYS, SET_BY, SET_ON, IS_DELETE) ");

                    query.Append(" VALUES( ");
                    query.Append(" '" + ReturnMaxID + "','" + model.AnnexureNo + "','" + model.AnnexRevisionNo + "','" + model.RecipeId + "','" + model.ProductCode +"','" + model.CompanyCode + "','" + darNo + "','" + model.DtlRemarks + "','" + model.StateStatus + "',");

                    // FIX: ক্লিন করা ভেরিয়েবলগুলো এখানে বসানো হয়েছে
                    query.Append(" '" + model.AuthorityType + "','" + model.AuthorityName + "','" + model.AuthorityLicenseNo + "','" + model.AuthorityLicenseName + "',");

                    query.Append(" (TO_DATE('" + model.DtlReceivedDate + "','dd/MM/yyyy')), (TO_DATE('" + model.DtlSubmissionDate + "','dd/MM/yyyy')), (TO_DATE('" + model.DtlApprovalDate + "','dd/MM/yyyy')), (TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')),");
                    query.Append(" (TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')), (TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')), (TO_DATE('" + model.InclusionDate + "','dd/MM/yyyy')), (TO_DATE('" + model.RenewalDate + "','dd/MM/yyyy')),");
                    query.Append(" (TO_DATE('" + model.ValidUptoDate + "','dd/MM/yyyy')), '" + remarks + "','" + model.AnnexStatus + "',");
                    query.Append(" '" + model.ProposedBy + "','" + model.AlarmDays + "','" + userId + "', (TO_DATE('" + currentDate + "','dd/MM/yyyy HH24:mi:ss')), 'N')");
                }

                // Debugging Tip: যদি এখনও কাজ না করে, তবে query.ToString() ভ্যালুটি কপি করে ডাটাবেসে চালিয়ে দেখুন কোথায় আটকাচ্ছে
                string finalQuery = query.ToString();

                if (_dbHelper.CmdExecute(_dbConn.SAConnStrReader(), finalQuery))
                {
                    return true;
                }
                else
                {
                    // CmdExecute ফলস রিটার্ন করছে মানে SQL এ সমস্যা আছে।
                    return false;
                }
            }
            catch (Exception errorException)
            {
                // throw errorException; এর বদলে শুধু throw; লিখলে আসল এররের লাইন নাম্বার হারায় না।
                throw;
            }
        }



        /* public IList<ProductRegistrationBEL> GetAllInfo(ProductRegistrationBEL model, string orderBy)
         {
             var query = new StringBuilder();
             query.Append(" SELECT D.ANNEX_ID,D.ANNEXURE_NO,D.REVISION_NO,D.RECIPE_ID,D.DAR_NO,D.DTL_REMARKS,D.STATE_STATUS,");
             query.Append(" SELECT D.AUTHORITY_TYPE,D.AUTHORITY_NAME,D.AUTHORITY_LICENSE_NO,D.AUTHORITY_LICENSE_NAME,D.PRODUCT_CODE,");
             query.Append(" TO_CHAR(D.DTL_RECEIVE_DATE, 'dd/mm/yyyy')DTL_RECEIVE_DATE,TO_CHAR(D.DTL_SUBMISSION_DATE, 'dd/mm/yyyy')DTL_SUBMISSION_DATE,");
             query.Append(" TO_CHAR(D.DTL_APPROVAL_DATE, 'dd/mm/yyyy')DTL_APPROVAL_DATE,TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,");
             query.Append(" TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE,TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,");
             query.Append(" TO_CHAR(D.INCLUSION_DATE, 'dd/mm/yyyy')INCLUSION_DATE,TO_CHAR(D.RENEWAL_DATE, 'dd/mm/yyyy')RENEWAL_DATE,");
             query.Append(" TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,D.REMARKS,D.PROPOSED_BY,D.ANNEXURE_STATUS,");
             query.Append(" D.NOTIFICATION_DAYS,TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,P.PRODUCT_CODE,P.PRODUCT_SPECIFICATION,");

             // ADDED: Unit Code and Unit Name selection
             query.Append(" D.COMPANY_CODE , CU.COMPANY_UNIT_NAME, ");

             // REMOVED: DF.DOSAGE_FORM_NAME
             query.Append(" C.COMPANY_CODE,C.COMPANY_NAME,C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PRODUCT_CATEGORY,P.BRAND_NAME,P.PACK_SIZE_NAME");

             query.Append(" FROM PRODUCT_REGISTRATION_INFO D");

             // ADDED: Joins for Unit and Company
             query.Append(" LEFT JOIN  COMPANY_UNIT_INFO CU ON D.COMPANY_CODE = CU.COMPANY_UNIT_CODE");
             query.Append(" LEFT JOIN  COMPANY_INFO C ON CU.COMPANY_CODE = C.COMPANY_CODE");

             query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE");

             // REMOVED: DOSAGE_FORM_INFO Join

             if (!string.IsNullOrEmpty(model.CompanyCode))
             {
                 query.Append(" AND C.COMPANY_CODE='{0}'");
             }
             if (!string.IsNullOrEmpty(model.ProductCode))
             {
                 query.Append(" AND P.PRODUCT_CODE='{1}'");
             }

             // Date Filtering Logic
             if (model.ChooseOption != "All")
             {
                 if (model.ChooseOption == "SubmissionDate")
                 {
                     if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                     {
                         query.Append(" AND D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                     }
                 }
                 else if (model.ChooseOption == "ApprovalDate")
                 {
                     if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                     {
                         query.Append(" AND D.RENEWAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                     }
                 }
                 else
                 {
                     if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                     {
                         query.Append(" AND D.RECEIVE_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                     }
                 }
             }
             else
             {
                 if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                 {
                     query.Append(" AND ( D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                     query.Append(" OR D.RECEIVE_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                     query.Append(" OR D.RENEWAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                 }
             }

             if (!string.IsNullOrEmpty(orderBy))
             {
                 query.Append(" ORDER BY D.ANNEX_ID " + orderBy);
             }

             DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.ProductCode));

             var item = (from DataRow row in dt.Rows
                         select new ProductRegistrationBEL
                         {
                             AnnexId = Convert.ToInt64(row["ANNEX_ID"]),
                             AnnexureNo = row["ANNEXURE_NO"].ToString(),
                             RecipeId = Convert.ToInt64(row["RECIPE_ID"]),
                             SlNo = row["SLNO"].ToString(),
                             AnnexRevisionNo = row["REVISION_NO"].ToString(),
                             DarNo = row["DAR_NO"].ToString(),
                             StateStatus = row["STATE_STATUS"].ToString(),
                             DtlRemarks = row["DTL_REMARKS"].ToString(),
                             DtlReceivedDate = row["DTL_RECEIVE_DATE"].ToString(),
                             DtlSubmissionDate = row["DTL_SUBMISSION_DATE"].ToString(),
                             DtlApprovalDate = row["DTL_APPROVAL_DATE"].ToString(),
                             ProposalDate = row["PROPOSAL_DATE"].ToString(),
                             ReceivedDate = row["RECEIVE_DATE"].ToString(),
                             SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                             InclusionDate = row["INCLUSION_DATE"].ToString(),
                             RenewalDate = row["RENEWAL_DATE"].ToString(),
                             ValidUptoDate = row["VALID_UPTO"].ToString(),
                             Remarks = row["REMARKS"].ToString(),
                             ProposedBy = row["PROPOSED_BY"].ToString(),
                             AnnexStatus = row["ANNEXURE_STATUS"].ToString(),
                             AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                             SetOn = row["SET_ON"].ToString(),
                             AuthorityType = row["AUTHORITY_TYPE"].ToString(),
                             AuthorityName = row["AUTHORITY_NAME"].ToString(),
                             AuthorityLicenseNo = row["AUTHORITY_LICENSE_NO"].ToString(),
                             AuthorityLicenseName = row["AUTHORITY_LICENSE_NAME"].ToString(),
                             // Added Mappings
                             CompanyUnitCode = row["COMPANY_UNIT_CODE"].ToString(),
                             CompanyUnitName = row["COMPANY_UNIT_NAME"] != DBNull.Value ? row["COMPANY_UNIT_NAME"].ToString() : "",

                             CompanyCode = row["COMPANY_CODE"].ToString(),
                             CompanyName = row["COMPANY_NAME"].ToString(),
                             LicenseNo = row["LICENSE_NO"].ToString(),
                             ProductCode = row["PRODUCT_CODE"].ToString(),
                             SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                             GenAndStrength = row["GENERIC_CODE"].ToString(),
                             PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                             ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                             BrandName = row["BRAND_NAME"].ToString(),
                             ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString()
                         }).ToList();
             return item;
         }*/
        public IList<ProductRegistrationBEL> GetAllInfo(ProductRegistrationBEL model, string orderBy)
        {
            var query = new StringBuilder();

            // ✅ FIX 1: Removed the duplicate/erroneous second "SELECT" keyword
            // ✅ FIX 2: Removed duplicate D.PRODUCT_CODE (conflicts with P.PRODUCT_CODE later)
            // ✅ FIX 3: Added D.COMPANY_CODE AS COMPANY_UNIT_CODE alias so row["COMPANY_UNIT_CODE"] mapping works
            // ✅ FIX 4: Removed duplicate C.COMPANY_CODE (D.COMPANY_CODE already selected above)
            query.Append(" SELECT D.ANNEX_ID, D.ANNEXURE_NO, D.REVISION_NO, D.RECIPE_ID, D.DAR_NO, D.DTL_REMARKS, D.STATE_STATUS,");
            query.Append(" D.AUTHORITY_TYPE, D.AUTHORITY_NAME, D.AUTHORITY_LICENSE_NO, D.AUTHORITY_LICENSE_NAME,");
            query.Append(" TO_CHAR(D.DTL_RECEIVE_DATE, 'dd/mm/yyyy') DTL_RECEIVE_DATE,");
            query.Append(" TO_CHAR(D.DTL_SUBMISSION_DATE, 'dd/mm/yyyy') DTL_SUBMISSION_DATE,");
            query.Append(" TO_CHAR(D.DTL_APPROVAL_DATE, 'dd/mm/yyyy') DTL_APPROVAL_DATE,");
            query.Append(" TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy') PROPOSAL_DATE,");
            query.Append(" TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy') RECEIVE_DATE,");
            query.Append(" TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy') SUBMISSION_DATE,");
            query.Append(" TO_CHAR(D.INCLUSION_DATE, 'dd/mm/yyyy') INCLUSION_DATE,");
            query.Append(" TO_CHAR(D.RENEWAL_DATE, 'dd/mm/yyyy') RENEWAL_DATE,");
            query.Append(" TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy') VALID_UPTO,");
            query.Append(" D.REMARKS, D.PROPOSED_BY, D.ANNEXURE_STATUS,");
            query.Append(" D.NOTIFICATION_DAYS, TO_CHAR(D.SET_ON, 'dd/mm/yyyy') SET_ON,");
            query.Append(" D.COMPANY_CODE AS COMPANY_UNIT_CODE, CU.COMPANY_UNIT_NAME,");
            query.Append(" C.COMPANY_CODE, C.COMPANY_NAME, C.LICENSE_NO,");
            query.Append(" P.PRODUCT_CODE,P.PRODUCT_NAME, P.PRODUCT_SPECIFICATION, P.SAP_PRODUCT_CODE,");
            query.Append(" P.GENERIC_CODE, P.PRODUCT_CATEGORY, P.BRAND_NAME, P.PACK_SIZE_NAME");

            query.Append(" FROM PRODUCT_REGISTRATION_INFO D");
            query.Append(" LEFT JOIN COMPANY_UNIT_INFO CU ON D.COMPANY_CODE = CU.COMPANY_UNIT_CODE");
            query.Append(" LEFT JOIN COMPANY_INFO C ON CU.COMPANY_CODE = C.COMPANY_CODE");
            query.Append(" LEFT JOIN PRODUCT_INFO P ON P.PRODUCT_CODE = D.PRODUCT_CODE");

            // ✅ FIX 5: Added WHERE 1=1 so the AND clauses below are always valid
            query.Append(" WHERE 1=1");

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND C.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.ProductCode))
            {
                query.Append(" AND P.PRODUCT_CODE='{1}'");
            }

            if (model.ChooseOption != "All")
            {
                if (model.ChooseOption == "SubmissionDate")
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd')" +
                                     " AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')");
                    }
                }
                else if (model.ChooseOption == "ApprovalDate")
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND D.RENEWAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd')" +
                                     " AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND D.RECEIVE_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd')" +
                                     " AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                {
                    query.Append(" AND (D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd')" +
                                 " AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')" +
                                 " OR D.RECEIVE_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd')" +
                                 " AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')" +
                                 " OR D.RENEWAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd')" +
                                 " AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd'))");
                }
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY D.ANNEX_ID " + orderBy);
            }

            DataTable dt = _dbHelper.GetDataTable(
                _dbConn.SAConnStrReader(),
                string.Format(query.ToString(), model.CompanyCode, model.ProductCode));

            // ✅ FIX 6: Removed row["SLNO"] — no SLNO column exists in this version of the query
            var item = (from DataRow row in dt.Rows
                        select new ProductRegistrationBEL
                        {
                            AnnexId = Convert.ToInt64(row["ANNEX_ID"]),
                            AnnexureNo = row["ANNEXURE_NO"].ToString(),
                            RecipeId = Convert.ToInt64(row["RECIPE_ID"]),
                            AnnexRevisionNo = row["REVISION_NO"].ToString(),
                            DarNo = row["DAR_NO"].ToString(),
                            StateStatus = row["STATE_STATUS"].ToString(),
                            DtlRemarks = row["DTL_REMARKS"].ToString(),
                            DtlReceivedDate = row["DTL_RECEIVE_DATE"].ToString(),
                            DtlSubmissionDate = row["DTL_SUBMISSION_DATE"].ToString(),
                            DtlApprovalDate = row["DTL_APPROVAL_DATE"].ToString(),
                            ProposalDate = row["PROPOSAL_DATE"].ToString(),
                            ReceivedDate = row["RECEIVE_DATE"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            InclusionDate = row["INCLUSION_DATE"].ToString(),
                            RenewalDate = row["RENEWAL_DATE"].ToString(),
                            ValidUptoDate = row["VALID_UPTO"].ToString(),
                            Remarks = row["REMARKS"].ToString(),
                            ProposedBy = row["PROPOSED_BY"].ToString(),
                            AnnexStatus = row["ANNEXURE_STATUS"].ToString(),
                            AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                            SetOn = row["SET_ON"].ToString(),
                            AuthorityType = row["AUTHORITY_TYPE"].ToString(),
                            AuthorityName = row["AUTHORITY_NAME"].ToString(),
                            AuthorityLicenseNo = row["AUTHORITY_LICENSE_NO"].ToString(),
                            AuthorityLicenseName = row["AUTHORITY_LICENSE_NAME"].ToString(),
                            CompanyUnitCode = row["COMPANY_UNIT_CODE"].ToString(),
                            CompanyUnitName = row["COMPANY_UNIT_NAME"] != DBNull.Value ? row["COMPANY_UNIT_NAME"].ToString() : "",
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            ProductName = row["PRODUCT_NAME"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_CODE"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString()
                        }).ToList();

            return item;
        }

        public IList<ProductRegistrationBEL> GetProductExpireLicense(ProductRegistrationBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT A.ANNEX_ID,A.ANNEXURE_NO,A.REVISION_NO,A.RECIPE_ID,A.DAR_NO,A.SUBMISSION_DATE,A.PROPOSAL_DATE,A.RECEIVE_DATE,A.ANNEXURE_STATUS,");
            query.Append(" A.NOTIFICATION_DAYS,A.PRODUCT_CODE,A.BRAND_NAME,A.PRODUCT_CATEGORY,A.COMPANY_CODE,A.COMPANY_NAME,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME,A.LICENSE_NO,A.SLNO,");
            query.Append(" A.DOSAGE_FORM_NAME,ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0)DateDiff,TO_CHAR(A.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO FROM");
            query.Append(" ( SELECT D.ANNEX_ID,D.ANNEXURE_NO,D.REVISION_NO,D.RECIPE_ID,D.DAR_NO,D.DTL_REMARKS, TO_CHAR(D.DTL_RECEIVE_DATE, 'dd/mm/yyyy')DTL_RECEIVE_DATE,TO_CHAR(D.DTL_SUBMISSION_DATE, 'dd/mm/yyyy')DTL_SUBMISSION_DATE,");
            query.Append(" TO_CHAR(D.DTL_APPROVAL_DATE, 'dd/mm/yyyy')DTL_APPROVAL_DATE,TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE, TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE,");
            query.Append(" TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE, TO_CHAR(D.INCLUSION_DATE, 'dd/mm/yyyy')INCLUSION_DATE,TO_CHAR(D.RENEWAL_DATE, 'dd/mm/yyyy')RENEWAL_DATE,");
            query.Append(" D.VALID_UPTO,D.REMARKS,D.PROPOSED_BY,D.ANNEXURE_STATUS, D.NOTIFICATION_DAYS, ");
            query.Append(" TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,P.PRODUCT_CODE,P.BRAND_NAME,P.PRODUCT_CATEGORY,R.SLNO,C.COMPANY_CODE,C.COMPANY_NAME,");
            query.Append(" C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME ");
            query.Append(" FROM PRODUCT_REGISTRATION_INFO D LEFT JOIN  RECIPE_INFO R ON R.ID=D.RECIPE_ID ");
            query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ");
            query.Append(" WHERE D.IS_DELETE <>'Y' ) A INNER JOIN ( SELECT RECIPE_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_REGISTRATION_INFO GROUP BY RECIPE_ID) B  ");
            query.Append(" ON B.RECIPE_ID=A.RECIPE_ID AND B.MaxRvNo=A.REVISION_NO ");
            query.Append(" WHERE 1=1 ");
            query.Append(" AND P.STATUS = 'Active' ");
            
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND A.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.ProductCode))
            {
                query.Append(" AND A.PRODUCT_CODE='{1}'");
            }
            if (!string.IsNullOrEmpty(model.AlarmDays))
            {
                query.Append(" AND ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0) <= {2}");
            }
            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND A.VALID_UPTO BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            }

            if (string.IsNullOrEmpty(model.CompanyCode) && string.IsNullOrEmpty(model.ProductCode) && string.IsNullOrEmpty(model.AlarmDays) && string.IsNullOrEmpty(model.FromDate) && string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ");
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  A.ANNEX_ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode,model.ProductCode, model.AlarmDays));

            var item = (from DataRow row in dt.Rows
                        select new ProductRegistrationBEL
                        {
                            AnnexId = Convert.ToInt64(row["ANNEX_ID"]),
                            AnnexureNo = row["ANNEXURE_NO"].ToString(),
                            RecipeId = Convert.ToInt64(row["RECIPE_ID"]),
                            SlNo = row["SLNO"].ToString(), //=>RecipeNo
                            AnnexRevisionNo = row["REVISION_NO"].ToString(),
                            DarNo = row["DAR_NO"].ToString(),
                            ProposalDate = row["PROPOSAL_DATE"].ToString(),
                            ReceivedDate = row["RECEIVE_DATE"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ValidUptoDate = row["VALID_UPTO"].ToString(),
                            AnnexStatus = row["ANNEXURE_STATUS"].ToString(),
                            AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_CODE"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            DateDiff = Convert.ToInt32(row["DateDiff"].ToString())
                        }).ToList();
            return item;
        }

        public DataTable GetProductRegForReport(ReportModel model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT ROW_NUMBER() OVER(ORDER BY A.ANNEX_ID)SN,A.RECIPE_ID, A.ANNEX_ID,A.ANNEXURE_NO,A.REVISION_NO,A.DAR_NO,A.PRODUCT_CODE,A.PRODUCT_CATEGORY,A.PROPOSED_BY,");
            query.Append(" A.DTL_RECEIVE_DATE,A.DTL_SUBMISSION_DATE,A.DTL_APPROVAL_DATE,A.PROPOSAL_DATE, A.DTL_REMARKS,A.RECEIVE_DATE,A.LICENSE_NO,A.RENEWAL_DATE,");
            query.Append(" A.SUBMISSION_DATE,A.INCLUSION_DATE,A.COMPANY_CODE,A.COMPANY_NAME, A.BRAND_NAME,A.PRODUCT_SPECIFICATION,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME,A.DOSAGE_FORM_NAME,A.VALID_UPTO, A.AMEND_APV_DATE, A.REMARKS");
            query.Append(" FROM (SELECT D.ANNEX_ID,D.ANNEXURE_NO,D.RECIPE_ID,P.PRODUCT_CODE,D.REVISION_NO,D.DAR_NO,D.PROPOSED_BY,C.COMPANY_CODE,C.COMPANY_NAME,C.LICENSE_NO,TO_CHAR(D.INCLUSION_DATE, 'dd/mm/yyyy')INCLUSION_DATE,TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,P.BRAND_NAME,P.PRODUCT_SPECIFICATION,P.PRODUCT_CATEGORY, D.AMEND_APV_DATE, D.REMARKS, D.DTL_REMARKS ,");
            query.Append(" TO_CHAR(D.DTL_RECEIVE_DATE, 'dd/mm/yyyy')DTL_RECEIVE_DATE,TO_CHAR(D.DTL_SUBMISSION_DATE, 'dd/mm/yyyy')DTL_SUBMISSION_DATE,TO_CHAR(D.DTL_APPROVAL_DATE, 'dd/mm/yyyy')DTL_APPROVAL_DATE,TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE, TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE,TO_CHAR(D.RENEWAL_DATE, 'dd/mm/yyyy')RENEWAL_DATE,");
            query.Append(" P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME  FROM PRODUCT_REGISTRATION_INFO D INNER JOIN  RECIPE_INFO R ON R.ID=D.RECIPE_ID ");
            query.Append(" INNER JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE  INNER JOIN  COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE ");
            query.Append(" INNER JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE  WHERE 1=1 ");
            query.Append(" AND P.STATUS = 'Active' ");
            if (!string.IsNullOrEmpty(model.StateStatus))
            {
                query.Append(" AND D.STATE_STATUS='"+model.StateStatus+"' ");
            }
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND P.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.BrandName))
            {
                query.Append(" AND TRIM(P.BRAND_NAME)='{1}'");
                model.BrandName = model.BrandName.Trim();
            }
            if (model.ProductSpecification != null && model.ProductSpecification != "All")
            {
                query.Append(" AND P.PRODUCT_SPECIFICATION='{2}'");
            }
            if (model.ChooseOption != "All")
            {
                if (model.ChooseOption == "SubmissionDate")
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
                else if (model.ChooseOption == "ApprovalDate")
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND D.RENEWAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND D.RECEIVE_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                {
                    query.Append(" AND ( D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR D.RECEIVE_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR D.RENEWAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                }
            }
            query.Append(" ) A INNER JOIN ( SELECT RECIPE_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_REGISTRATION_INFO GROUP BY RECIPE_ID) B ON B.RECIPE_ID=A.RECIPE_ID  AND B.MaxRvNo=A.REVISION_NO");

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.BrandName, model.ProductSpecification));

            return dt;
        }

        public DataTable GetFileRefno(int p, string refLevel1, string sStatus)
        {
            var query = new StringBuilder();
            if(sStatus.Equals("New"))
            {
                query.Append(" SELECT filetype,PA,PPM,SD FROM ");
                query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
                query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Proposed Annexure' PA,'Proposed Packaging Material' PPM,'Stability Data' SD))");
            } 
            else if(sStatus.Equals("Renew"))
            {
                query.Append(" SELECT filetype,PA,PPM FROM ");
                query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
                query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Proposed Annexure' PA,'Proposed Packaging Material' PPM))");
            }
            else if (sStatus.Equals("Annexure Amendment"))
            {
                query.Append(" SELECT filetype,PA,AJ,SD FROM ");
                query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
                query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Proposed Annexure' PA,'Amendment Justification' AJ,'Stability Data' SD))");
            }
            else if (sStatus.Equals("Packaging Amendment"))
            {
                query.Append(" SELECT filetype,PPM,AJ FROM ");
                query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
                query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Proposed Packaging Material' PPM,'Amendment Justification' AJ))");
            }
           
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), p, refLevel1));
            return dt;
        }

        public void UpdateFileRelatedInfo(ProductRegistrationBEL model, string userId)
        {
            var query = new StringBuilder();
            if (model.AnnexId > 0)
            {
                //U for update
                ReturnMaxID = model.AnnexId;
                MaxID = model.SlNo;
                RefNo = model.AnnexRevisionNo;
                IUMode = "U";
                query.Append(" UPDATE PRODUCT_REGISTRATION_INFO SET RECEIVE_DATE =(TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')), ");
                query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                query.Append(" WHERE ANNEX_ID='" + model.AnnexId + "'");
            }
            _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());
        }
    }
}