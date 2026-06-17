using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.DAL.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RMS_Square.Areas.Regulatory.Models.DAO
{
    public class ProductRegApprovalDAO
    {
        private readonly DBConnection _dbConn;
        private readonly DBHelper _dbHelper;

        public ProductRegApprovalDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
        }

        // ── Save Approval ─────────────────────────────────────────
        public bool SaveApproval(ProductRegistrationBEL model, string userId)
        {
            var query = new StringBuilder();
            string now = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            query.Append(" UPDATE PRODUCT_REGISTRATION_INFO SET");
            query.Append("   APPROVAL_STATUS  = '" + model.ApprovalStatus + "',");
            query.Append("   APPROVED_BY      = '" + userId + "',");
            query.Append("   APPROVED_DATE    = (TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),");
            query.Append("   UPDATE_DATE      = (TO_DATE('" + now + "','dd/MM/yyyy HH24:mi:ss')),");
            query.Append("   UPDATE_BY        = '" + userId + "'");
            query.Append(" WHERE ANNEX_ID = '" + model.AnnexId + "'");

            return _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());
        }

        // ── Get All — filter by CompanyUnitCode / ApprovalStatus ──
        public IList<ProductRegistrationBEL> GetAllInfo(ProductRegistrationBEL filter, string orderBy)
        {
            var query = new StringBuilder();

            query.Append(" SELECT D.ANNEX_ID, D.ANNEXURE_NO, D.REVISION_NO, D.STATE_STATUS,");
            query.Append(" D.AUTHORITY_TYPE, D.AUTHORITY_NAME,");
            query.Append(" D.REMARKS,");
            query.Append(" TO_CHAR(D.SUBMISSION_DATE,'dd/mm/yyyy')  SUBMISSION_DATE,");
            query.Append(" TO_CHAR(D.RENEWAL_DATE,'dd/mm/yyyy')     RENEWAL_DATE,");
            query.Append(" TO_CHAR(D.VALID_UPTO,'dd/mm/yyyy')       VALID_UPTO,");
            query.Append(" TO_CHAR(D.INCLUSION_DATE,'dd/mm/yyyy')   INCLUSION_DATE,");
            query.Append(" TO_CHAR(D.RECEIVE_DATE,'dd/mm/yyyy')     RECEIVE_DATE,");
            query.Append(" TO_CHAR(D.DTL_RECEIVE_DATE,'dd/mm/yyyy') DTL_RECEIVE_DATE,");
            query.Append(" TO_CHAR(D.DTL_SUBMISSION_DATE,'dd/mm/yyyy') DTL_SUBMISSION_DATE,");
            query.Append(" TO_CHAR(D.DTL_APPROVAL_DATE,'dd/mm/yyyy')  DTL_APPROVAL_DATE,");
            query.Append(" NVL(D.APPROVAL_STATUS,'N')                 APPROVAL_STATUS,");
            query.Append(" D.APPROVED_BY,");
            query.Append(" TO_CHAR(D.APPROVED_DATE,'dd/mm/yyyy')    APPROVED_DATE,");
           // query.Append(" D.APPROVAL_REMARKS,");
            query.Append(" D.COMPANY_CODE AS COMPANY_UNIT_CODE, CU.COMPANY_UNIT_NAME,");
            query.Append(" C.COMPANY_CODE, C.COMPANY_NAME,");
            query.Append(" P.PRODUCT_CODE, P.PRODUCT_NAME, P.BRAND_NAME,");
            query.Append(" P.PACK_SIZE_NAME, P.PRODUCT_CATEGORY, P.PRODUCT_SPECIFICATION");
            query.Append(" FROM PRODUCT_REGISTRATION_INFO D");
            query.Append(" LEFT JOIN COMPANY_UNIT_INFO CU ON D.COMPANY_CODE = CU.COMPANY_UNIT_CODE");
            query.Append(" LEFT JOIN COMPANY_INFO C      ON CU.COMPANY_CODE  = C.COMPANY_CODE");
            query.Append(" LEFT JOIN PRODUCT_INFO P      ON P.PRODUCT_CODE   = D.PRODUCT_CODE");
            query.Append(" WHERE NVL(D.IS_DELETE,'N') = 'N'");

            if (!string.IsNullOrEmpty(filter.CompanyUnitCode))
                query.Append(" AND D.COMPANY_CODE = '" + filter.CompanyUnitCode + "'");

            if (!string.IsNullOrEmpty(filter.CompanyCode))
                query.Append(" AND C.COMPANY_CODE = '" + filter.CompanyCode + "'");

            if (!string.IsNullOrEmpty(filter.ProductCode))
                query.Append(" AND P.PRODUCT_CODE = '" + filter.ProductCode + "'");

            if (!string.IsNullOrEmpty(filter.ApprovalStatus))
                query.Append(" AND NVL(D.APPROVAL_STATUS,'N') = '" + filter.ApprovalStatus + "'");

            if (!string.IsNullOrEmpty(orderBy))
                query.Append(" ORDER BY D.ANNEX_ID " + orderBy);

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), query.ToString());

            var list = new List<ProductRegistrationBEL>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ProductRegistrationBEL
                {
                    AnnexId = Convert.ToInt64(row["ANNEX_ID"]),
                    AnnexureNo = row["ANNEXURE_NO"].ToString(),
                    AnnexRevisionNo = row["REVISION_NO"].ToString(),
                    StateStatus = row["STATE_STATUS"].ToString(),
                    AuthorityType = row["AUTHORITY_TYPE"].ToString(),
                    AuthorityName = row["AUTHORITY_NAME"].ToString(),
                    Remarks = row["REMARKS"].ToString(),
                   // DtlRemarks = row["DtlREMARKS"].ToString(),
                    SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                    RenewalDate = row["RENEWAL_DATE"].ToString(),
                    ValidUptoDate = row["VALID_UPTO"].ToString(),
                    InclusionDate = row["INCLUSION_DATE"].ToString(),
                    ReceivedDate = row["RECEIVE_DATE"].ToString(),
                    DtlReceivedDate = row["DTL_RECEIVE_DATE"].ToString(),
                    DtlSubmissionDate = row["DTL_SUBMISSION_DATE"].ToString(),
                    DtlApprovalDate = row["DTL_APPROVAL_DATE"].ToString(),
                    ApprovalStatus = row["APPROVAL_STATUS"].ToString(),
                    ApprovedBy = row["APPROVED_BY"].ToString(),
                    ApprovedDate = row["APPROVED_DATE"].ToString(),
                    CompanyUnitCode = row["COMPANY_UNIT_CODE"].ToString(),
                    CompanyUnitName = row["COMPANY_UNIT_NAME"] != DBNull.Value
                                            ? row["COMPANY_UNIT_NAME"].ToString() : "",
                    CompanyCode = row["COMPANY_CODE"].ToString(),
                    CompanyName = row["COMPANY_NAME"].ToString(),
                    ProductCode = row["PRODUCT_CODE"].ToString(),
                    ProductName = row["PRODUCT_NAME"].ToString(),
                    BrandName = row["BRAND_NAME"].ToString(),
                    PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                    ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                    ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString()
                });
            }
            return list;
        }
    }
}