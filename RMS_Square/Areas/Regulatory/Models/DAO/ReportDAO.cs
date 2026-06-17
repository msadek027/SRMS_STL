using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.DAL.Common;
using RMS_Square.DAL.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Web;
using Systems.Models;

namespace RMS_Square.Areas.Regulatory.Models.DAO
{
    public class ReportDAO
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private string qry = string.Empty;
        private static string _serverFilePath = string.Empty;
        public ReportDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
        }
        public DataTable GetProductLcInfo(ReportModel model)
        {
            DataTable dt = new DataTable();
            var query = new StringBuilder();
            try
            {
                switch (model.ReportName)
                {
                    case "MasterProductInfo":
                        query.Append(" SELECT P.PRODUCT_CODE,P.SAP_PRODUCT_CODE,P.BRAND_NAME,P.GENERIC_CODE,P.INTRODUCED_BANGLADESH,P.MANUFACTURING_TYPE,P.PRODUCT_CATEGORY,");
                        query.Append(" P.PRODUCT_SPECIFICATION,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME,TC.THERAPEUTIC_CLASS_NAME FROM  PRODUCT_INFO P ");
                        query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ");
                        query.Append(" LEFT JOIN THERAPEUTIC_CLASS_INFO TC ON TC.THERAPEUTIC_CLASS_CODE=P.THERAPEUTIC_CLASS_CODE");
                        query.Append(" WHERE 1=1 ");
                        if (!string.IsNullOrEmpty(model.ProductCodeList))
                        {
                            query.Append(" AND P.PRODUCT_CODE in(" + model.ProductCodeList + ")");
                        }
                        if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                        {
                            query.Append(" AND P.SET_ON BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
                        }
                        query.Append(" ORDER BY P.PRODUCT_CODE ASC");
                        break;
                    case "ProductLifeCycleInfoSinglePage":
                        ProductLifeCycleQuery(model, query);
                        break;
                    case "ProductLifeCycleInfo":
                        ProductLifeCycleQuery(model, query);
                        break;
                }
                dt = _dbHelper.GetDataTable(query.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        private static void ProductLifeCycleQuery(ReportModel model, StringBuilder query)
        {
            query.Append(" SELECT C.COMPANY_CODE,C.COMPANY_NAME, P.PRODUCT_CODE,P.SAP_PRODUCT_CODE,P.BRAND_NAME,P.GENERIC_CODE GENERIC_STRENGTH,P.PRODUCT_SPECIFICATION,P.PRODUCT_CATEGORY,DF.DOSAGE_FORM_NAME,");
            query.Append(" P.PACK_SIZE_NAME,R.RECIPE_SUBMISSION_TYPE,TO_CHAR(R.RECIPE_RECEIVE_DATE, 'dd/mm/yyyy')RECIPE_RECEIVE_DATE,TO_CHAR(R.RECIPE_SUBMISSION_DATE, 'dd/mm/yyyy')RECIPE_SUBMISSION_DATE,TO_CHAR(R.RECIPE_MEETING_DATE, 'dd/mm/yyyy')RECIPE_MEETING_DATE,TO_CHAR(R.RECIPE_APPROVAL_DATE, 'dd/mm/yyyy')RECIPE_APPROVAL_DATE,TO_CHAR(R.RECIPE_VALID_UPTO, 'dd/mm/yyyy')RECIPE_VALID_UPTO,TO_CHAR(PR.DTL_SUBMISSION_DATE, 'dd/mm/yyyy')DTL_SUBMISSION_DATE,");
            query.Append(" TO_CHAR(PR.DTL_APPROVAL_DATE, 'dd/mm/yyyy')DTL_APPROVAL_DATE,PR.DAR_NO,TO_CHAR(PR.ANNAX_RECEIVE_DATE, 'dd/mm/yyyy')ANNAX_RECEIVE_DATE, TO_CHAR (PR.APPROVAL_DATE, 'dd/mm/yyyy') APPROVAL_DATE, TO_CHAR (PR.INCLUSION_DATE, 'dd/mm/yyyy') INCLUSION_DATE, TO_CHAR(PR.ANNEX_SUBMISSION_DATE, 'dd/mm/yyyy')ANNEX_SUBMISSION_DATE,TO_CHAR(PR.ANNEX_VALID_UPTO, 'dd/mm/yyyy')ANNEX_VALID_UPTO,TO_CHAR(PP.PRICE_RECEIVED_DATE, 'dd/mm/yyyy')PRICE_RECEIVED_DATE,TO_CHAR(PP.PRICE_SUBMISSION_DATE, 'dd/mm/yyyy')PRICE_SUBMISSION_DATE,TO_CHAR(PP.PRICE_APPROVAL_DATE, 'dd/mm/yyyy')PRICE_APPROVAL_DATE,");
            query.Append(" PP.PRICE_PER_UNIT,PP.PRICE_CHANGE_STATUS,TO_CHAR(MA.MA_SUBMISSION_DATE, 'dd/mm/yyyy')MA_SUBMISSION_DATE,TO_CHAR(MA.MA_RECEIVE_DATE, 'dd/mm/yyyy')MA_RECEIVE_DATE,TO_CHAR(MA.MA_APPROVAL_DATE, 'dd/mm/yyyy')MA_APPROVAL_DATE,TO_CHAR(MA.MA_VALID_UPTO, 'dd/mm/yyyy')MA_VALID_UPTO ");
            query.Append(" FROM(SELECT A.ID, A.PRODUCT_CODE,A.COMPANY_CODE,A.MEETING_TYPE RECIPE_SUBMISSION_TYPE,A.RECEIVE_DATE RECIPE_RECEIVE_DATE,A.PROPOSAL_DATE RECIPE_PROPOSAL_DATE,");
            query.Append(" A.MEETING_DATE RECIPE_MEETING_DATE,A.SUBMISSION_DATE RECIPE_SUBMISSION_DATE,A.APPROVAL_DATE RECIPE_APPROVAL_DATE,A.VALID_UPTO RECIPE_VALID_UPTO FROM RECIPE_INFO A");
            query.Append(" INNER JOIN (SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM RECIPE_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) B");
            query.Append(" ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO WHERE 1=1");

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND A.COMPANY_CODE ='" + model.CompanyCode + "'");
            }
            if (!string.IsNullOrEmpty(model.ProductCodeList))
            {
                query.Append(" AND A.PRODUCT_CODE in(" + model.ProductCodeList + ")");
            }
            else
            {
                if (!string.IsNullOrEmpty(model.ProductCode))
                {
                    query.Append(" AND A.PRODUCT_CODE ='" + model.ProductCode + "'");
                }
            }
            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND A.PROPOSAL_DATE BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
            }
            query.Append(" )R LEFT JOIN(SELECT A.ANNEX_ID,A.ANNEXURE_NO,A.REVISION_NO,A.RECIPE_ID,A.RECEIVE_DATE ANNAX_RECEIVE_DATE,A.SUBMISSION_DATE ANNEX_SUBMISSION_DATE, A.RENEWAL_DATE APPROVAL_DATE, A.INCLUSION_DATE INCLUSION_DATE,");
            query.Append(" A.VALID_UPTO ANNEX_VALID_UPTO,A.DAR_NO,A.DTL_SUBMISSION_DATE,A.DTL_APPROVAL_DATE FROM PRODUCT_REGISTRATION_INFO A");
            query.Append(" INNER JOIN (SELECT RECIPE_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_REGISTRATION_INFO GROUP BY RECIPE_ID) B");
            query.Append(" ON B.RECIPE_ID=A.RECIPE_ID AND B.MaxRvNo=A.REVISION_NO )PR ON R.ID=PR.RECIPE_ID");
            query.Append(" LEFT JOIN (SELECT A.ANNEX_ID,A.RECEIVED_DATE PRICE_RECEIVED_DATE,A.SUBMISSION_DATE PRICE_SUBMISSION_DATE, A.APPROVAL_DATE PRICE_APPROVAL_DATE,");
            query.Append(" A.PRICE_PER_UNIT,A.PRICE_CHANGE_STATUS FROM PRODUCT_PRICE A");
            query.Append(" INNER JOIN (SELECT ANNEX_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_PRICE GROUP BY ANNEX_ID) B");
            query.Append(" ON B.ANNEX_ID=A.ANNEX_ID AND B.MaxRvNo=A.REVISION_NO)PP ON PP.ANNEX_ID=PR.ANNEX_ID");
            query.Append(" LEFT JOIN (SELECT A.COMPANY_CODE,A.PRODUCT_CODE,A.SUBMISSION_DATE MA_SUBMISSION_DATE,A.RECEIVE_DATE MA_RECEIVE_DATE,A.APPROVAL_DATE MA_APPROVAL_DATE,A.VALID_UPTO MA_VALID_UPTO");
            query.Append(" FROM MARKET_AUTH_CERTIFICATE A INNER JOIN (SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM MARKET_AUTH_CERTIFICATE GROUP BY COMPANY_CODE,PRODUCT_CODE) B");
            query.Append(" ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO WHERE 1=1");

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND A.COMPANY_CODE ='" + model.CompanyCode + "'");
            }
            if (!string.IsNullOrEmpty(model.ProductCodeList))
            {
                query.Append(" AND A.PRODUCT_CODE in(" + model.ProductCodeList + ")");
            }
            else
            {
                if (!string.IsNullOrEmpty(model.ProductCode))
                {
                    query.Append(" AND A.PRODUCT_CODE ='" + model.ProductCode + "'");
                }
            }
            query.Append(" )MA ON MA.COMPANY_CODE=R.COMPANY_CODE AND MA.PRODUCT_CODE=R.PRODUCT_CODE");
            query.Append(" INNER JOIN COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE INNER JOIN PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE");
            query.Append(" INNER JOIN DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE");
        }
        public DataTable GetAllNotifyInfo(ReportModel model)
        {
            DataTable dt = new DataTable();
            var query = new StringBuilder();
            try
            {
                switch (model.ReportName)
                {
                    case "CompanyLicInfo":
                        query.Append(" SELECT A.CLID, A.COMP_LICENSE_SLNO,A.REVISION_NO,A.COMPANY_CODE,A.LICENSE_NO,A.SUBMISSION_TYPE,A.SUBMISSION_DATE ,");
                        query.Append(" ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0)DateDiff,");
                        query.Append(" A.INSPECTION_DATE,TO_CHAR(A.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,A.APPROVAL_DATE,A.NOTIFICATION_DAYS,A.SET_ON, A.COMPANY_NAME,A.ADDRESS FROM");
                        query.Append(" ( SELECT CL.CLID, CL.COMP_LICENSE_SLNO,CL.REVISION_NO,CL.COMPANY_CODE,CL.LICENSE_NO,CL.SUBMISSION_TYPE,");
                        query.Append(" TO_CHAR(CL.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE , TO_CHAR(CL.INSPECTION_DATE, 'dd/mm/yyyy')INSPECTION_DATE,CL.VALID_UPTO,");
                        query.Append(" TO_CHAR(CL.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE,CL.NOTIFICATION_DAYS,TO_CHAR(CL.SET_ON, 'dd/mm/yyyy')SET_ON, C.COMPANY_NAME,C.ADDRESS");
                        query.Append(" FROM COMPANY_LICENSE CL LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=CL.COMPANY_CODE WHERE IS_DELETE <>'Y' ");
                        query.Append(" ) A INNER JOIN ( SELECT COMPANY_CODE, MAX(REVISION_NO) AS MaxRvNo ");
                        query.Append(" FROM COMPANY_LICENSE GROUP BY COMPANY_CODE) B");
                        query.Append(" ON A.COMPANY_CODE=B.COMPANY_CODE AND A.REVISION_NO=B.MaxRvNo");
                        query.Append(" WHERE 1=1 ");
                        if (!string.IsNullOrEmpty(model.CompanyCode))
                        {
                            query.Append(" AND A.COMPANY_CODE ='" + model.CompanyCode + "'");
                        }
                        if (!string.IsNullOrEmpty(model.AlarmDays))
                        {
                            query.Append(" AND ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0) <= {0}");
                        }
                        else
                        {
                            query.Append(" AND ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0)<=A.NOTIFICATION_DAYS ");
                        }
                        if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                        {
                            query.Append(" AND A.VALID_UPTO BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
                        }
                        query.Append(" ORDER BY A.CLID ASC");
                        break;
                    case "ProductLicInfo":
                        query.Append(" SELECT A.ANNEX_ID,A.ANNEXURE_NO,A.REVISION_NO,A.RECIPE_ID,A.DAR_NO,A.SUBMISSION_DATE,A.PROPOSAL_DATE,A.RECEIVE_DATE,A.ANNEXURE_STATUS,");
                        query.Append(" A.NOTIFICATION_DAYS,A.PRODUCT_CODE,A.BRAND_NAME,A.PRODUCT_CATEGORY,A.COMPANY_CODE,A.COMPANY_NAME,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME,A.LICENSE_NO,A.SLNO,");
                        query.Append(" A.DOSAGE_FORM_NAME,ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0)DateDiff,TO_CHAR(A.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO FROM");
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

                        if (!string.IsNullOrEmpty(model.CompanyCode))
                        {
                            query.Append(" AND A.COMPANY_CODE ='" + model.CompanyCode + "'");
                        }
                        if (!string.IsNullOrEmpty(model.ProductCodeList))
                        {
                            query.Append(" AND A.PRODUCT_CODE in(" + model.ProductCodeList + ")");
                        }
                        if (!string.IsNullOrEmpty(model.AlarmDays))
                        {
                            query.Append(" AND ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0) <= {0}");
                        }
                        else
                        {
                            query.Append(" AND ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ");
                        }
                        if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                        {
                            query.Append(" AND A.VALID_UPTO BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
                        }
                        query.Append(" ORDER BY A.ANNEX_ID ASC");

                        break;
                    case "GMPLicInfo":
                        break;
                    case "RecipeNotifyInfo":
                        break;
                    case "NarcoticLicInfo":
                        break;
                    case "ImportProductLicInfo":
                        break;
                    case "AdvertisementNotifyInfo":
                        break;
                }
                dt = _dbHelper.GetDataTable(string.Format(query.ToString(), model.AlarmDays));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public IList<ProductLifeCycleBEL> GetAllProductLifeCycle(ReportModel model)
        {
            DataTable dt = new DataTable();
            var query = new StringBuilder();
            ProductLifeCycleQuery(model, query);
            dt = _dbHelper.GetDataTable(query.ToString());

            var item = (from DataRow row in dt.Rows
                        select new ProductLifeCycleBEL
                        {
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenericStrength = row["GENERIC_STRENGTH"].ToString(),
                            PackSize = row["PACK_SIZE_NAME"].ToString(),
                            DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString(),

                            RecipeSubmissionType = row["RECIPE_SUBMISSION_TYPE"].ToString(),
                            RecipeReceivedDate = row["RECIPE_RECEIVE_DATE"].ToString(),
                            RecipeSubmissionDate = row["RECIPE_SUBMISSION_DATE"].ToString(),
                            RecipeMeetingDate = row["RECIPE_MEETING_DATE"].ToString(),
                            RecipeValidUptoDate = row["RECIPE_VALID_UPTO"].ToString(),
                            RecipeApprovalDate = row["RECIPE_APPROVAL_DATE"].ToString(),
                           
                            DtlSubmissionDate = row["DTL_SUBMISSION_DATE"].ToString(),
                            DtlApprovalDate = row["DTL_APPROVAL_DATE"].ToString(),
                            DarNo = row["DAR_NO"].ToString(),
                            AnnexReceivedDate = row["ANNAX_RECEIVE_DATE"].ToString(),
                            AnnexSubmissionDate = row["ANNEX_SUBMISSION_DATE"].ToString(),
                            AnnexValidUptoDate = row["ANNEX_VALID_UPTO"].ToString(),
                            AnnexApprovalDate = row["APPROVAL_DATE"].ToString(),
                            InclusionDate = row["INCLUSION_DATE"].ToString(),

                            PriceReceivedDate = row["PRICE_RECEIVED_DATE"].ToString(),
                            PriceSubmissionDate = row["PRICE_SUBMISSION_DATE"].ToString(),
                            PriceApprovalDate = row["PRICE_APPROVAL_DATE"].ToString(),
                            PricePerUnit = row["PRICE_PER_UNIT"].ToString(),

                            MacSubmissionDate = row["MA_SUBMISSION_DATE"].ToString(),
                            MacReceivedDate = row["MA_RECEIVE_DATE"].ToString(),
                            MacApprovalDate = row["MA_APPROVAL_DATE"].ToString(),
                            MacValidUptoDate = row["MA_VALID_UPTO"].ToString()
                           
                        }).ToList();
            return item;
            
        }

        // ── Helper: Query build করে ──────────────────────────────────────────────
        /*private string BuildProductDocumentQuery(ProductReportParams param)
        {
            var query = new StringBuilder();

            query.Append(" SELECT D.ANNEX_ID, D.ANNEXURE_NO, D.REVISION_NO, D.DAR_NO,");
            query.Append(" D.AUTHORITY_TYPE, D.AUTHORITY_NAME, D.AUTHORITY_LICENSE_NO, D.AUTHORITY_LICENSE_NAME,");
            query.Append(" D.STATE_STATUS, D.REMARKS,");
            query.Append(" TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy') SUBMISSION_DATE,");
            query.Append(" TO_CHAR(D.RECEIVE_DATE,    'dd/mm/yyyy') RECEIVE_DATE,");
            query.Append(" TO_CHAR(D.INCLUSION_DATE,  'dd/mm/yyyy') INCLUSION_DATE,");
            query.Append(" TO_CHAR(D.RENEWAL_DATE,    'dd/mm/yyyy') RENEWAL_DATE,");
            query.Append(" TO_CHAR(D.VALID_UPTO,      'dd/mm/yyyy') VALID_UPTO,");
            query.Append(" D.COMPANY_CODE AS COMPANY_UNIT_CODE, CU.COMPANY_UNIT_NAME,");
            query.Append(" C.COMPANY_CODE, C.COMPANY_NAME, C.LICENSE_NO,");
            query.Append(" P.PRODUCT_CODE, P.PRODUCT_NAME, P.BRAND_NAME, P.PRODUCT_CATEGORY,");
            query.Append(" P.PRODUCT_SPECIFICATION, P.GENERIC_CODE, P.PACK_SIZE_NAME,");

            // ── File columns — actual table column নাম ──────────────────────────
            query.Append(" F.FILEID, F.FILECODE, F.FILENAME, F.EXTENTION, F.FILEPATH, F.REFNO AS DOC_REF_NO,");

            // Document Status
            query.Append(" CASE");
            query.Append("   WHEN F.FILEID IS NOT NULL THEN 'Uploaded'");
            query.Append("   WHEN D.VALID_UPTO < SYSDATE THEN 'Expired'");
            query.Append("   ELSE 'Pending'");
            query.Append(" END AS DOCUMENT_STATUS");

            query.Append(" FROM PRODUCT_REGISTRATION_INFO D");
            query.Append(" LEFT JOIN COMPANY_UNIT_INFO  CU ON D.COMPANY_CODE = CU.COMPANY_UNIT_CODE");
            query.Append(" LEFT JOIN COMPANY_INFO        C ON CU.COMPANY_CODE = C.COMPANY_CODE");
            query.Append(" LEFT JOIN PRODUCT_INFO        P ON P.PRODUCT_CODE  = D.PRODUCT_CODE");

            // ── DOCUMENTFILEINFO subquery — REFLEVEL1 is NUMBER, no IS_DELETE column ──
            query.Append(" LEFT JOIN (");
            query.Append("   SELECT FILEID, REFLEVEL1, FILECODE, FILENAME, EXTENTION, FILEPATH, REFNO,");
            query.Append("          ROW_NUMBER() OVER (PARTITION BY REFLEVEL1 ORDER BY FILEID DESC) AS RN");
            query.Append("   FROM DOCUMENTFILEINFO");
            query.Append("   WHERE FILETYPE = " + (int)Enums.E_FormFileType.ProductRegistration);
            query.Append(" ) F ON F.REFLEVEL1 = D.ANNEX_ID AND F.RN = 1");  // REFLEVEL1 NUMBER — no TO_CHAR needed

            query.Append(" WHERE NVL(D.IS_DELETE,'N') = 'N'");

            // ── Filters ──────────────────────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(param.CompanyCode))
                query.Append(" AND C.COMPANY_CODE = '" + param.CompanyCode.Replace("'", "''") + "'");

            if (!string.IsNullOrWhiteSpace(param.CompanyUnitCode))
                query.Append(" AND D.COMPANY_CODE = '" + param.CompanyUnitCode.Replace("'", "''") + "'");

            if (!string.IsNullOrWhiteSpace(param.ProductCodeList))
                query.Append(" AND D.PRODUCT_CODE IN (" + param.ProductCodeList + ")");

            if (!string.IsNullOrWhiteSpace(param.AuthorityType))
                query.Append(" AND D.AUTHORITY_TYPE = '" + param.AuthorityType.Replace("'", "''") + "'");

            if (!string.IsNullOrWhiteSpace(param.DocumentType))
                query.Append(" AND D.AUTHORITY_LICENSE_NAME = '" + param.DocumentType.Replace("'", "''") + "'");

            if (!string.IsNullOrWhiteSpace(param.ValidFrom) && !string.IsNullOrWhiteSpace(param.ValidTo))
            {
                query.Append(" AND D.VALID_UPTO BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(param.ValidFrom) + "','yyyy/mm/dd')");
                query.Append(" AND TO_DATE('" + General.SetDateStrYYYYMMDD(param.ValidTo) + "','yyyy/mm/dd')");
            }

            if (!string.IsNullOrWhiteSpace(param.SubFrom) && !string.IsNullOrWhiteSpace(param.SubTo))
            {
                query.Append(" AND D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(param.SubFrom) + "','yyyy/mm/dd')");
                query.Append(" AND TO_DATE('" + General.SetDateStrYYYYMMDD(param.SubTo) + "','yyyy/mm/dd')");
            }

            // DocStatus filter — CASE alias এর জন্য subquery wrap
            string finalQuery;
            if (!string.IsNullOrWhiteSpace(param.DocStatus))
            {
                finalQuery = "SELECT * FROM (" + query.ToString() + ") WHERE DOCUMENT_STATUS = '"
                             + param.DocStatus.Replace("'", "''") + "'"
                             + " ORDER BY ANNEX_ID DESC";
            }
            else
            {
                finalQuery = query.ToString() + " ORDER BY D.ANNEX_ID DESC";
            }

            return finalQuery;
        }*/

        private string BuildProductDocumentQuery(ProductReportParams param)
        {
            var query = new StringBuilder();

            query.Append(" SELECT D.ANNEX_ID, D.ANNEXURE_NO, D.REVISION_NO, D.DAR_NO,");
            query.Append(" D.AUTHORITY_TYPE, D.AUTHORITY_NAME, D.AUTHORITY_LICENSE_NO, D.AUTHORITY_LICENSE_NAME,");
            query.Append(" D.STATE_STATUS, D.REMARKS,");
            query.Append(" TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy') SUBMISSION_DATE,");
            query.Append(" TO_CHAR(D.RECEIVE_DATE,    'dd/mm/yyyy') RECEIVE_DATE,");
            query.Append(" TO_CHAR(D.INCLUSION_DATE,  'dd/mm/yyyy') INCLUSION_DATE,");
            query.Append(" TO_CHAR(D.RENEWAL_DATE,    'dd/mm/yyyy') RENEWAL_DATE,");
            query.Append(" TO_CHAR(D.VALID_UPTO,      'dd/mm/yyyy') VALID_UPTO,");
            query.Append(" ROUND((D.VALID_UPTO - SYSDATE), 0) AS DAYS_LEFT,");  // ← new

            query.Append(" D.COMPANY_CODE AS COMPANY_UNIT_CODE, CU.COMPANY_UNIT_NAME,");
            query.Append(" C.COMPANY_CODE, C.COMPANY_NAME, C.LICENSE_NO,");
            query.Append(" P.PRODUCT_CODE, P.PRODUCT_NAME, P.BRAND_NAME, P.PRODUCT_CATEGORY,");
            query.Append(" P.PRODUCT_SPECIFICATION, P.GENERIC_CODE, P.PACK_SIZE_NAME,P.PRODUCT_VARIANT,");

            query.Append(" F.FILEID, F.FILECODE, F.FILENAME, F.EXTENTION, F.FILEPATH, F.REFNO AS DOC_REF_NO,");

            // ── Document Status — alarm days ব্যবহার করে ──────────────────
            query.Append(" CASE");
            query.Append("   WHEN D.VALID_UPTO < SYSDATE THEN 'Expired'");
            query.Append("   WHEN F.FILEID IS NOT NULL");
            query.Append("        AND ROUND((D.VALID_UPTO - SYSDATE), 0) > NVL(D.NOTIFICATION_DAYS, 0) THEN 'Uploaded'");
            query.Append("   WHEN ROUND((D.VALID_UPTO - SYSDATE), 0) <= NVL(D.NOTIFICATION_DAYS, 0) THEN 'Expiring Soon'");
            query.Append("   ELSE 'Pending'");
            query.Append(" END AS DOCUMENT_STATUS");

            query.Append(" FROM PRODUCT_REGISTRATION_INFO D");
            query.Append(" LEFT JOIN COMPANY_UNIT_INFO  CU ON CU.COMPANY_UNIT_CODE = D.COMPANY_CODE");
            query.Append(" LEFT JOIN COMPANY_INFO        C ON C.COMPANY_CODE = CU.COMPANY_CODE");
            query.Append(" LEFT JOIN PRODUCT_INFO        P ON P.PRODUCT_CODE  = D.PRODUCT_CODE");

            query.Append(" LEFT JOIN (");
            query.Append("   SELECT FILEID, REFLEVEL1, FILECODE, FILENAME, EXTENTION, FILEPATH, REFNO,");
            query.Append("          ROW_NUMBER() OVER (PARTITION BY REFLEVEL1 ORDER BY FILEID DESC) AS RN");
            query.Append("   FROM DOCUMENTFILEINFO");
            query.Append("   WHERE FILETYPE = " + (int)Enums.E_FormFileType.ProductRegistration);
            query.Append(" ) F ON F.REFLEVEL1 = D.ANNEX_ID AND F.RN = 1");

            query.Append(" WHERE NVL(D.IS_DELETE,'N') = 'N'");
            query.Append(" AND D.APPROVAL_STATUS = 'Y'");

            // ── Filters ──────────────────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(param.CompanyCode))
                query.Append(" AND C.COMPANY_CODE = '" + param.CompanyCode.Replace("'", "''") + "'");

            if (!string.IsNullOrWhiteSpace(param.CompanyUnitCode))  // ← new
                query.Append(" AND D.COMPANY_CODE = '" + param.CompanyUnitCode.Replace("'", "''") + "'");

            if (!string.IsNullOrWhiteSpace(param.ProductCodeList))
                query.Append(" AND D.PRODUCT_CODE IN (" + param.ProductCodeList + ")");

            if (!string.IsNullOrWhiteSpace(param.AuthorityType))
                query.Append(" AND D.AUTHORITY_TYPE = '" + param.AuthorityType.Replace("'", "''") + "'");

            if (!string.IsNullOrWhiteSpace(param.DocumentType))
                query.Append(" AND D.AUTHORITY_LICENSE_NAME = '" + param.DocumentType.Replace("'", "''") + "'");

            if (!string.IsNullOrWhiteSpace(param.ValidFrom) && !string.IsNullOrWhiteSpace(param.ValidTo))
            {
                query.Append(" AND D.VALID_UPTO BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(param.ValidFrom) + "','yyyy/mm/dd')");
                query.Append(" AND TO_DATE('" + General.SetDateStrYYYYMMDD(param.ValidTo) + "','yyyy/mm/dd')");
            }

            if (!string.IsNullOrWhiteSpace(param.SubFrom) && !string.IsNullOrWhiteSpace(param.SubTo))
            {
                query.Append(" AND D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(param.SubFrom) + "','yyyy/mm/dd')");
                query.Append(" AND TO_DATE('" + General.SetDateStrYYYYMMDD(param.SubTo) + "','yyyy/mm/dd')");
            }

            if (!string.IsNullOrWhiteSpace(param.AlarmDays))  // ← new
                query.Append(" AND ROUND((D.VALID_UPTO - SYSDATE), 0) <= " + param.AlarmDays.Replace("'", "''"));

            // DocStatus — subquery wrap
            string finalQuery;
            if (!string.IsNullOrWhiteSpace(param.DocStatus))
            {
                finalQuery = "SELECT * FROM (" + query.ToString() + ") WHERE DOCUMENT_STATUS = '"
                             + param.DocStatus.Replace("'", "''") + "'"
                             + " ORDER BY ANNEX_ID DESC";
            }
            else
            {
                finalQuery = query.ToString() + " ORDER BY D.ANNEX_ID DESC";
            }

            return finalQuery;
        }


        // ── Method 1: List<VM> — Grid (JSON) এর জন্য ────────────────────────────
        public IList<ProductReportResultVM> GetProductDocumentReport(ProductReportParams param)
        {
            string sql = BuildProductDocumentQuery(param);
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), sql);

            var list = (from DataRow row in dt.Rows
                        select new ProductReportResultVM
                        {
                            AnnexId = Convert.ToInt64(row["ANNEX_ID"]),
                            AnnexureNo = row["ANNEXURE_NO"].ToString(),
                            DarNo = row["DAR_NO"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            CompanyUnitCode = row["COMPANY_UNIT_CODE"].ToString(),
                            CompanyUnitName = row["COMPANY_UNIT_NAME"] != DBNull.Value ? row["COMPANY_UNIT_NAME"].ToString() : "",
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            ProductName = row["PRODUCT_NAME"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            GenericStrength = row["GENERIC_CODE"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            Variant = row["PRODUCT_VARIANT"].ToString(),
                            ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            ProductSpec = row["PRODUCT_SPECIFICATION"].ToString(),
                            AuthorityType = row["AUTHORITY_TYPE"].ToString(),
                            AuthorityName = row["AUTHORITY_NAME"].ToString(),
                            AuthorityLicenseNo = row["AUTHORITY_LICENSE_NO"].ToString(),
                            AuthorityLicenseName = row["AUTHORITY_LICENSE_NAME"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ReceiveDate = row["RECEIVE_DATE"].ToString(),
                            InclusionDate = row["INCLUSION_DATE"].ToString(),
                            ValidUptoDate = row["VALID_UPTO"].ToString(),
                            RenewalDate = row["RENEWAL_DATE"].ToString(),
                            Remarks = row["REMARKS"].ToString(),
                            DocumentStatus = row["DOCUMENT_STATUS"].ToString(),
                            DaysLeft = row["DAYS_LEFT"] != DBNull.Value ? Convert.ToDecimal(row["DAYS_LEFT"]) : 0,
                            // ── File columns — DOCUMENTFILEINFO এর actual নাম ──
                            FileID = row["FILEID"] != DBNull.Value ? Convert.ToInt64(row["FILEID"]) : 0,
                            FileCode = row["FILECODE"] != DBNull.Value ? row["FILECODE"].ToString() : null,
                            FileName = row["FILENAME"] != DBNull.Value ? row["FILENAME"].ToString() : null,
                            FileExtension = row["EXTENTION"] != DBNull.Value ? row["EXTENTION"].ToString() : null,
                            FilePath = row["FILEPATH"] != DBNull.Value ? row["FILEPATH"].ToString() : null,
                            DocRefNo = row["DOC_REF_NO"] != DBNull.Value ? row["DOC_REF_NO"].ToString() : null,
                        }).ToList();

            return list;
        }



        // ── Method 2: DataTable — Crystal Reports (Export) এর জন্য ─────────────
        public DataTable GetProductDocumentReportDT(ProductReportParams param)
        {
            string sql = BuildProductDocumentQuery(param);
            return _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), sql);
        }

        // ── Company License Report ─────────────────────────────────────────
        public List<CompanyLicenseReportResult> GetCompanyLicenseReport(
            CompanyLicenseReportParams p)
        {
            var list = new List<CompanyLicenseReportResult>();

            string sql = @"
        SELECT
    CL.CLID,
    CL.COMP_LICENSE_SLNO,
    CL.COMPANY_CODE        AS COMPANY_UNIT_CODE,
    CU.COMPANY_UNIT_NAME,
    CM.COMPANY_CODE,
    CM.COMPANY_NAME,
    CL.LICENSE_NO,
    CL.COMP_LICENSE_NAME,
    CL.REVISION_NO,
    CL.SUBMISSION_TYPE,
    TO_CHAR(CL.SUBMISSION_DATE, 'DD/MM/YYYY') AS SUBMISSION_DATE,
    TO_CHAR(CL.INSPECTION_DATE, 'DD/MM/YYYY') AS INSPECTION_DATE,
    TO_CHAR(CL.APPROVAL_DATE,   'DD/MM/YYYY') AS APPROVAL_DATE,
    TO_CHAR(CL.VALID_UPTO,      'DD/MM/YYYY') AS VALID_UPTO,
    CL.DETAILS,
    CL.RES_DEPT1,
    CL.RES_DEPT2,
    CL.NOTIFICATION_DAYS,
    ROUND((CL.VALID_UPTO - SYSDATE), 0) AS DAYS_LEFT,
    CASE
        WHEN CL.VALID_UPTO < SYSDATE
             THEN 'Expired'
        WHEN DFI.FILEID IS NOT NULL
             AND ROUND((CL.VALID_UPTO - SYSDATE), 0) > NVL(CL.NOTIFICATION_DAYS, 0)
             THEN 'Uploaded'
        WHEN ROUND((CL.VALID_UPTO - SYSDATE), 0) <= NVL(CL.NOTIFICATION_DAYS, 0)
             THEN 'Expiring Soon'
        ELSE 'Pending'
    END AS DOCUMENT_STATUS,
    DFI.FILEID,
    DFI.FILECODE,
    DFI.FILENAME,
    DFI.EXTENTION AS FILE_EXTENSION
FROM STL_SRMS.COMPANY_LICENSE CL
LEFT JOIN STL_SRMS.COMPANY_UNIT_INFO CU
       ON CU.COMPANY_UNIT_CODE = CL.COMPANY_CODE
LEFT JOIN STL_SRMS.COMPANY_INFO CM
       ON CM.COMPANY_CODE = CU.COMPANY_CODE
LEFT JOIN (
    SELECT REFLEVEL1, FILEID, FILECODE, FILENAME, EXTENTION,
           ROW_NUMBER() OVER (PARTITION BY REFLEVEL1 ORDER BY FILEID DESC) RN
    FROM STL_SRMS.DOCUMENTFILEINFO
) DFI ON DFI.REFLEVEL1 = CL.CLID AND DFI.RN = 1
WHERE 1=1
    ";

            var prms = new List<OracleParameter>();

            // Company filter — CM.COMPANY_CODE দিয়ে
            if (!string.IsNullOrEmpty(p.CompanyCode))
            {
                sql += " AND CM.COMPANY_CODE = :CompanyCode";
                prms.Add(new OracleParameter("CompanyCode", p.CompanyCode));
            }

            // Unit filter — CL.COMPANY_CODE দিয়ে (এটাই COMPANY_UNIT_CODE)
            if (!string.IsNullOrEmpty(p.CompanyUnitCode))
            {
                sql += " AND CL.COMPANY_CODE = :CompanyUnitCode";
                prms.Add(new OracleParameter("CompanyUnitCode", p.CompanyUnitCode));
            }
            if (!string.IsNullOrEmpty(p.CompLicenseName))
            {
                sql += " AND CL.COMP_LICENSE_NAME = :CompLicenseName";
                prms.Add(new OracleParameter("CompLicenseName", p.CompLicenseName));
            }
            if (!string.IsNullOrEmpty(p.SubmissionType))
            {
                sql += " AND CL.SUBMISSION_TYPE = :SubmissionType";
                prms.Add(new OracleParameter("SubmissionType", p.SubmissionType));
            }
            if (!string.IsNullOrEmpty(p.ValidFrom))
            {
                sql += " AND CL.VALID_UPTO >= TO_DATE(:ValidFrom,'DD/MM/YYYY')";
                prms.Add(new OracleParameter("ValidFrom", p.ValidFrom));
            }
            if (!string.IsNullOrEmpty(p.ValidTo))
            {
                sql += " AND CL.VALID_UPTO <= TO_DATE(:ValidTo,'DD/MM/YYYY')";
                prms.Add(new OracleParameter("ValidTo", p.ValidTo));
            }
            if (!string.IsNullOrEmpty(p.CompanyUnitCode))
            {
                sql += " AND CL.COMPANY_CODE = :CompanyUnitCode";
                prms.Add(new OracleParameter("CompanyUnitCode", p.CompanyUnitCode));
            }

            if (!string.IsNullOrEmpty(p.AlarmDays))
            {
                sql += " AND ROUND((CL.VALID_UPTO - SYSDATE), 0) <= :AlarmDays";
                prms.Add(new OracleParameter("AlarmDays", Convert.ToInt32(p.AlarmDays)));
            }
            if (!string.IsNullOrEmpty(p.SubFrom))
            {
                sql += " AND CL.SUBMISSION_DATE >= TO_DATE(:SubFrom,'DD/MM/YYYY')";
                prms.Add(new OracleParameter("SubFrom", p.SubFrom));
            }
            if (!string.IsNullOrEmpty(p.SubTo))
            {
                sql += " AND CL.SUBMISSION_DATE <= TO_DATE(:SubTo,'DD/MM/YYYY')";
                prms.Add(new OracleParameter("SubTo", p.SubTo));
            }

            // DocStatus filter — CASE alias এর জন্য subquery wrap করতে হবে
            if (!string.IsNullOrEmpty(p.DocStatus))
            {
                string statusCondition = "";
                switch (p.DocStatus)
                {
                    case "Uploaded":
                        statusCondition = " AND DOCUMENT_STATUS = 'Uploaded'"; break;
                    case "Expired":
                        statusCondition = " AND DOCUMENT_STATUS = 'Expired'"; break;
                    case "Expiring Soon":
                        statusCondition = " AND DOCUMENT_STATUS = 'Expiring Soon'"; break;
                    case "Pending":
                        statusCondition = " AND DOCUMENT_STATUS = 'Pending'"; break;
                }
                sql = "SELECT * FROM (" + sql + ") WHERE 1=1" + statusCondition;
            }

            sql += " ORDER BY COMPANY_CODE, CLID";

            using (var con = new OracleConnection(_dbConn.SAConnStrReader()))
            {
                con.Open();
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.Parameters.AddRange(prms.ToArray());
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(new CompanyLicenseReportResult
                            {
                                CLID = rdr["CLID"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["CLID"]),
                                CompLicenseSlNo = rdr["COMP_LICENSE_SLNO"] == DBNull.Value ? "" : rdr["COMP_LICENSE_SLNO"].ToString(),
                                CompanyCode = rdr["COMPANY_CODE"] == DBNull.Value ? "" : rdr["COMPANY_CODE"].ToString(),
                                CompanyName = rdr["COMPANY_NAME"] == DBNull.Value ? "" : rdr["COMPANY_NAME"].ToString(),
                                CompanyUnitCode = rdr["COMPANY_UNIT_CODE"] == DBNull.Value ? "" : rdr["COMPANY_UNIT_CODE"].ToString(),
                                CompanyUnitName = rdr["COMPANY_UNIT_NAME"] == DBNull.Value ? "" : rdr["COMPANY_UNIT_NAME"].ToString(),
                                LicenseNo = rdr["LICENSE_NO"] == DBNull.Value ? "" : rdr["LICENSE_NO"].ToString(),
                                CompLicenseName = rdr["COMP_LICENSE_NAME"] == DBNull.Value ? "" : rdr["COMP_LICENSE_NAME"].ToString(),
                                RevisionNo = rdr["REVISION_NO"] == DBNull.Value ? "" : rdr["REVISION_NO"].ToString(),
                                SubmissionType = rdr["SUBMISSION_TYPE"] == DBNull.Value ? "" : rdr["SUBMISSION_TYPE"].ToString(),
                                SubmissionDate = rdr["SUBMISSION_DATE"] == DBNull.Value ? "" : rdr["SUBMISSION_DATE"].ToString(),
                                InspectionDate = rdr["INSPECTION_DATE"] == DBNull.Value ? "" : rdr["INSPECTION_DATE"].ToString(),
                                ApprovalDate = rdr["APPROVAL_DATE"] == DBNull.Value ? "" : rdr["APPROVAL_DATE"].ToString(),
                                ValidUpto = rdr["VALID_UPTO"] == DBNull.Value ? "" : rdr["VALID_UPTO"].ToString(),
                                Details = rdr["DETAILS"] == DBNull.Value ? "" : rdr["DETAILS"].ToString(),
                                ResDept1 = rdr["RES_DEPT1"] == DBNull.Value ? "" : rdr["RES_DEPT1"].ToString(),
                                ResDept2 = rdr["RES_DEPT2"] == DBNull.Value ? "" : rdr["RES_DEPT2"].ToString(),
                                DocumentStatus = rdr["DOCUMENT_STATUS"] == DBNull.Value ? "" : rdr["DOCUMENT_STATUS"].ToString(),
                                FileID = rdr["FILEID"] == DBNull.Value ? 0 : Convert.ToInt64(rdr["FILEID"]),
                                FileCode = rdr["FILECODE"] == DBNull.Value ? "" : rdr["FILECODE"].ToString(),
                                FileName = rdr["FILENAME"] == DBNull.Value ? "" : rdr["FILENAME"].ToString(),
                                DaysLeft = rdr["DAYS_LEFT"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["DAYS_LEFT"]),
                                FileExtension = rdr["FILE_EXTENSION"] == DBNull.Value ? "" : rdr["FILE_EXTENSION"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public DataTable GetCompanyLicenseReportDT(
            CompanyLicenseReportParams p)
        {
            var dt = new DataTable();

            string sql = @"
        SELECT
            CL.COMPANY_CODE,
            CM.COMPANY_NAME,
            CL.COMP_LICENSE_NAME,
            CL.LICENSE_NO,
            CL.REVISION_NO,
            CL.SUBMISSION_TYPE,
            TO_CHAR(CL.SUBMISSION_DATE, 'DD/MM/YYYY') AS SUBMISSION_DATE,
            TO_CHAR(CL.INSPECTION_DATE, 'DD/MM/YYYY') AS INSPECTION_DATE,
            TO_CHAR(CL.APPROVAL_DATE,   'DD/MM/YYYY') AS APPROVAL_DATE,
            TO_CHAR(CL.VALID_UPTO,      'DD/MM/YYYY') AS VALID_UPTO,
            CL.DETAILS,
            CL.NOTIFICATION_DAYS,
            ROUND((CL.VALID_UPTO - SYSDATE), 0) AS DAYS_LEFT,
            CASE
                WHEN CL.VALID_UPTO < SYSDATE
                     THEN 'Expired'
                WHEN DFI.FILEID IS NOT NULL
                     AND ROUND((CL.VALID_UPTO - SYSDATE), 0) > NVL(CL.NOTIFICATION_DAYS, 0)
                     THEN 'Uploaded'
                WHEN ROUND((CL.VALID_UPTO - SYSDATE), 0) <= NVL(CL.NOTIFICATION_DAYS, 0)
                     THEN 'Expiring Soon'
                ELSE 'Pending'
            END AS DOCUMENT_STATUS
        FROM STL_SRMS.COMPANY_LICENSE CL
        LEFT JOIN STL_SRMS.COMPANY_INFO CM
               ON CM.COMPANY_CODE = CL.COMPANY_CODE
        LEFT JOIN (
            SELECT REFLEVEL1, FILEID,
                   ROW_NUMBER() OVER (PARTITION BY REFLEVEL1 ORDER BY FILEID DESC) RN
            FROM STL_SRMS.DOCUMENTFILEINFO
        ) DFI ON DFI.REFLEVEL1 = CL.CLID AND DFI.RN = 1
        WHERE 1=1
    ";

            var prms = new List<OracleParameter>();

            if (!string.IsNullOrEmpty(p.CompanyCode))
            {
                sql += " AND CL.COMPANY_CODE = :CompanyCode";
                prms.Add(new OracleParameter("CompanyCode", p.CompanyCode));
            }
            if (!string.IsNullOrEmpty(p.LicenseNo))
            {
                sql += " AND UPPER(CL.LICENSE_NO) LIKE UPPER(:LicenseNo)";
                prms.Add(new OracleParameter("LicenseNo", "%" + p.LicenseNo + "%"));
            }
            if (!string.IsNullOrEmpty(p.SubmissionType))
            {
                sql += " AND CL.SUBMISSION_TYPE = :SubmissionType";
                prms.Add(new OracleParameter("SubmissionType", p.SubmissionType));
            }
            if (!string.IsNullOrEmpty(p.ValidFrom))
            {
                sql += " AND CL.VALID_UPTO >= TO_DATE(:ValidFrom,'DD/MM/YYYY')";
                prms.Add(new OracleParameter("ValidFrom", p.ValidFrom));
            }
            if (!string.IsNullOrEmpty(p.ValidTo))
            {
                sql += " AND CL.VALID_UPTO <= TO_DATE(:ValidTo,'DD/MM/YYYY')";
                prms.Add(new OracleParameter("ValidTo", p.ValidTo));
            }
            if (!string.IsNullOrEmpty(p.SubFrom))
            {
                sql += " AND CL.SUBMISSION_DATE >= TO_DATE(:SubFrom,'DD/MM/YYYY')";
                prms.Add(new OracleParameter("SubFrom", p.SubFrom));
            }
            if (!string.IsNullOrEmpty(p.SubTo))
            {
                sql += " AND CL.SUBMISSION_DATE <= TO_DATE(:SubTo,'DD/MM/YYYY')";
                prms.Add(new OracleParameter("SubTo", p.SubTo));
            }

            if (!string.IsNullOrEmpty(p.DocStatus))
            {
                string statusCondition = "";
                switch (p.DocStatus)
                {
                    case "Uploaded": statusCondition = " AND DOCUMENT_STATUS = 'Uploaded'"; break;
                    case "Expired": statusCondition = " AND DOCUMENT_STATUS = 'Expired'"; break;
                    case "Expiring Soon": statusCondition = " AND DOCUMENT_STATUS = 'Expiring Soon'"; break;
                    case "Pending": statusCondition = " AND DOCUMENT_STATUS = 'Pending'"; break;
                }
                sql = "SELECT * FROM (" + sql + ") WHERE 1=1" + statusCondition;
            }

            sql += " ORDER BY COMPANY_CODE, CLID";

            using (var con = new OracleConnection(_dbConn.SAConnStrReader()))
            {
                con.Open();
                using (var cmd = new OracleCommand(sql, con))
                using (var adpt = new OracleDataAdapter(cmd))
                {
                    cmd.Parameters.AddRange(prms.ToArray());
                    adpt.Fill(dt);
                }
            }
            return dt;
        }
    }
}