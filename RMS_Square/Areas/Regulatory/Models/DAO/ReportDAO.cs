using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.DAL.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Text;

namespace RMS_Square.Areas.Regulatory.Models.DAO
{
    public class ReportDAO
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private string qry = string.Empty;
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
    }
}