using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.DAL.Gateway;
using RMS_Square.Universal.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace RMS_Square.Universal
{
    public class HomePageDAO
    {
        private DBConnection _dbConn = new DBConnection();
        private DBHelper _dbHelper = new DBHelper();
        private IDGenerated _idGenerated = new IDGenerated();

        public IList<HomePageBEL> GetAllInfo(string fromDate, string toDate)
        {
            if (string.IsNullOrEmpty(fromDate))
            {
                int yr = DateTime.Today.Year;
                fromDate = "01/01/" + yr.ToString();
            }

            if (string.IsNullOrEmpty(toDate))
            {
                DateTime crt = DateTime.Today;
                toDate = crt.ToString("dd/MM/yyyy");
            }

            var query = new StringBuilder();

            query.Append(" SELECT COMPANY_CODE, (SELECT COMPANY_NAME FROM COMPANY_INFO WHERE COMPANY_CODE=A.COMPANY_CODE)COMPANY_NAME,    (SELECT SEQ FROM COMPANY_INFO WHERE COMPANY_CODE = A.COMPANY_CODE) SEQ,SUM(NVL(RECIPE,0))RECIPE, ");
            query.Append(" SUM(NVL(DTL,0))DTL, SUM(NVL(PROD_REG,0))PROD_REG, SUM(NVL(PRICE,0))PRICE, SUM(NVL(MA,0))MA, SUM(NVL(AMENDMENT,0))AMENDMENT ");
            query.Append(" FROM ( SELECT  DISTINCT COMPANY_CODE, 0 RECIPE, 0 DTL, 0 PROD_REG, 0 PRICE, 0 MA, 0 AMENDMENT  FROM  PRODUCT_INFO ");

            query.Append(" UNION ALL SELECT COMPANY_CODE, COUNT(DISTINCT PRODUCT_CODE) RECIPE, 0 DTL,0 PROD_REG,0 PRICE, 0 MA, 0 AMENDMENT FROM RECIPE_INFO ");
            query.Append(" WHERE TO_DATE(APPROVAL_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') GROUP BY COMPANY_CODE ");

            query.Append(" UNION ALL SELECT B.COMPANY_CODE, 0 RECIPE, COUNT(A.RECIPE_ID)DTL, 0 PROD_REG, 0 PRICE, 0 MA, 0 AMENDMENT FROM PRODUCT_REGISTRATION_INFO A, RECIPE_INFO B , PRODUCT_INFO C ");
            query.Append(" WHERE  A.RECIPE_ID=B.ID AND C.PRODUCT_CODE = B.PRODUCT_CODE AND C.PRODUCT_SPECIFICATION = 'INN' AND  A.STATE_STATUS='New' AND TO_DATE(A.DTL_APPROVAL_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') GROUP BY B.COMPANY_CODE ");

            query.Append(" UNION ALL SELECT B.COMPANY_CODE, 0 RECIPE, 0 DTL, COUNT(A.RECIPE_ID)PROD_REG, 0 PRICE, 0 MA,0 AMENDMENT FROM PRODUCT_REGISTRATION_INFO A, RECIPE_INFO B WHERE A.RECIPE_ID=B.ID ");
            query.Append(" AND A.STATE_STATUS='New' AND TO_DATE(A.RENEWAL_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') GROUP BY B.COMPANY_CODE  ");

            query.Append(" UNION ALL SELECT  C.COMPANY_CODE, 0 RECIPE, 0 DTL,0 PROD_REG, COUNT(DISTINCT A.SLNO)PRICE, 0 MA,0 AMENDMENT FROM PRODUCT_PRICE A, PRODUCT_REGISTRATION_INFO B,RECIPE_INFO C ");
            query.Append(" WHERE A.ANNEX_ID=B.ANNEX_ID AND B.RECIPE_ID=C.ID AND TO_DATE(A.APPROVAL_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') GROUP BY C.COMPANY_CODE ");

            query.Append(" UNION ALL SELECT COMPANY_CODE, 0 RECIPE,  0 DTL, 0 PROD_REG, 0 PRICE, COUNT(DISTINCT PRODUCT_CODE)MA, 0 AMENDMENT FROM MARKET_AUTH_CERTIFICATE ");
            query.Append(" WHERE TO_DATE(APPROVAL_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') GROUP BY COMPANY_CODE ");

            //query.Append(" UNION ALL SELECT B.COMPANY_CODE, 0 RECIPE,  0 DTL, 0 PROD_REG, 0 PRICE, 0 MA, COUNT(A.RECIPE_ID) AMENDMENT FROM PRODUCT_REGISTRATION_INFO A, RECIPE_INFO B ");
            //query.Append(" WHERE  A.RECIPE_ID=B.ID  AND A.STATE_STATUS = 'Annexure Amendment' AND A.INCLUSION_DATE IS NULL AND TO_DATE(A.SUBMISSION_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
            //query.Append(" AND (A.STATE_STATUS='Annexure Amendment' OR A.STATE_STATUS='Packaging Amendment') GROUP BY B.COMPANY_CODE) A GROUP BY COMPANY_CODE ");

            query.Append(" UNION ALL SELECT B.COMPANY_CODE, 0 RECIPE,  0 DTL, 0 PROD_REG, 0 PRICE, 0 MA, COUNT(A.RECIPE_ID) AMENDMENT FROM PRODUCT_REGISTRATION_INFO A INNER JOIN RECIPE_INFO B ON A.RECIPE_ID = B.ID ");
            query.Append(" WHERE TO_DATE(A.RENEWAL_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
            query.Append(" AND (A.STATE_STATUS='Annexure Amendment' OR A.STATE_STATUS='Packaging Amendment') GROUP BY B.COMPANY_CODE) A GROUP BY COMPANY_CODE ORDER BY SEQ ");


            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString()));

            var item = (from DataRow row in dt.Rows
                        select new HomePageBEL
                        {
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            Recipe = row["RECIPE"].ToString(),
                            DTL = row["DTL"].ToString(),
                            Prod_Reg = row["PROD_REG"].ToString(),
                            Price = row["PRICE"].ToString(),
                            MA = row["MA"].ToString(),
                            Amendment = row["AMENDMENT"].ToString(),
                        }).ToList();
            return item;
        }

        public IList<HomePageBEL> GetAllPending(string CompanyCode, string dataType, string fromDate, string toDate)
        {
            if (string.IsNullOrEmpty(fromDate))
            {
                DateTime yr = DateTime.Today.AddYears(-1);
                fromDate = yr.ToString("dd/MM/yyyy");
            }

            if (string.IsNullOrEmpty(toDate))
            {
                DateTime crt = DateTime.Today;
                toDate = crt.ToString("dd/MM/yyyy");
            }

            var query = new StringBuilder();


            if (dataType == "Recipe")
            {
                query.Append(" SELECT B.BRAND_NAME, B.GENERIC_CODE, TO_CHAR(A.SUBMISSION_DATE, 'dd/mm/yyyy') SUBMISSION_DATE, A.REMARKS FROM RECIPE_INFO A INNER JOIN PRODUCT_INFO B ON B.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE TO_DATE(A.SUBMISSION_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
                query.Append(" AND A.APPROVAL_DATE IS NULL ");
            }

            else if (dataType == "DTL")
            {
                query.Append(" SELECT B.BRAND_NAME, B.GENERIC_CODE, TO_CHAR(C.DTL_SUBMISSION_DATE, 'dd/mm/yyyy') SUBMISSION_DATE, C.DTL_REMARKS REMARKS FROM PRODUCT_REGISTRATION_INFO C ");
                query.Append(" INNER JOIN RECIPE_INFO A ON C.RECIPE_ID = A.ID INNER JOIN PRODUCT_INFO B ON B.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE TO_DATE(C.DTL_SUBMISSION_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
                query.Append(" AND C.DTL_APPROVAL_DATE IS NULL AND C.STATE_STATUS = 'New' ");
            }

            else if (dataType == "Product")
            {
                query.Append(" SELECT B.BRAND_NAME, B.GENERIC_CODE, TO_CHAR(C.SUBMISSION_DATE, 'dd/mm/yyyy') SUBMISSION_DATE, C.REMARKS FROM PRODUCT_REGISTRATION_INFO C ");
                query.Append(" INNER JOIN RECIPE_INFO A ON C.RECIPE_ID = A.ID INNER JOIN PRODUCT_INFO B ON B.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE TO_DATE(C.SUBMISSION_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
                query.Append(" AND C.RENEWAL_DATE IS NULL AND C.STATE_STATUS = 'New' ");
            }
            else if (dataType == "Price")
            {
                query.Append(" SELECT B.BRAND_NAME, B.GENERIC_CODE, TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy') SUBMISSION_DATE, D.REMARKS FROM PRODUCT_PRICE D ");
                query.Append(" INNER JOIN PRODUCT_REGISTRATION_INFO C ON D.ANNEX_ID = C.ANNEX_ID ");
                query.Append(" INNER JOIN RECIPE_INFO A ON C.RECIPE_ID = A.ID INNER JOIN PRODUCT_INFO B ON B.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE TO_DATE(D.SUBMISSION_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
                query.Append(" AND D.APPROVAL_DATE IS NULL ");
            }
            else if (dataType == "MA")
            {
                query.Append(" SELECT B.BRAND_NAME, B.GENERIC_CODE, TO_CHAR(A.SUBMISSION_DATE, 'dd/mm/yyyy') SUBMISSION_DATE, A.REMARKS FROM MARKET_AUTH_CERTIFICATE A ");
                query.Append(" INNER JOIN PRODUCT_INFO B ON B.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE TO_DATE(A.SUBMISSION_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
                query.Append(" AND A.APPROVAL_DATE IS NULL ");
            }
            else if (dataType == "Amendment")
            {
                query.Append(" SELECT B.BRAND_NAME, B.GENERIC_CODE, TO_CHAR(C.SUBMISSION_DATE, 'dd/mm/yyyy') SUBMISSION_DATE, C.REMARKS FROM PRODUCT_REGISTRATION_INFO C ");
                query.Append(" INNER JOIN RECIPE_INFO A ON C.RECIPE_ID = A.ID INNER JOIN PRODUCT_INFO B ON B.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE TO_DATE(C.SUBMISSION_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
                query.Append(" AND (C.STATE_STATUS = 'Annexure Amendment' OR C.STATE_STATUS = 'Packaging Amendment') AND C.RENEWAL_DATE IS NULL  ");
            }

            if (!string.IsNullOrEmpty(CompanyCode))
            {
                query.Append("  AND A.COMPANY_CODE = '" + CompanyCode + "'  ");
            }

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString()));

            var item = (from DataRow row in dt.Rows
                        select new HomePageBEL
                        {
                            BrandName = row["BRAND_NAME"].ToString(),
                            GenericName = row["GENERIC_CODE"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            Remarks = row["REMARKS"].ToString(),
                        }).ToList();
            return item;
        }

        //public IList<ProductSummaryBEL> ShowProductSummary(string CompanyCode, string BrandName)
        //{
        //    var query = new StringBuilder();

        //    query.Append(" SELECT * FROM VW_RPT_PRODUCT_SUMMARY P WHERE 1=1 ");
        //    query.Append(" AND P.PRODUCT_CODE = '" + CompanyCode + "'");

        //    DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString()));

        //    var item = (from DataRow row in dt.Rows
        //                select new ProductSummaryBEL
        //                {
        //                    BrandName = row["BRAND_NAME"].ToString(),
        //                    GenericCode = row["GENERIC_CODE"].ToString(),
        //                    CompanyCode = row["COMPANY_CODE"].ToString(),
        //                    ProductCode = row["PRODUCT_CODE"].ToString(),
        //                    RCPID = row["RCP_ID"].ToString(),
        //                    RCPSubmissionDate = GetDateTime(row["RCP_SUBMISSION_DATE"].ToString()),
        //                    RCPApprovalDate = GetDateTime(row["RCP_APPROVAL_DATE"].ToString()),
        //                    RCPValidUpto = GetDateTime(row["RCP_VALID_UPTO"].ToString()),
        //                    RCPRemarks = row["RCP_REMARKS"].ToString(),
        //                    REGID = row["REG_ID"].ToString(),
        //                    DTLSubmissionDate = GetDateTime(row["DTL_SUBMISSION_DATE"].ToString()),
        //                    DTLApprovalDate = GetDateTime(row["DTL_APPROVAL_DATE"].ToString()),
        //                    DTLRemarks = row["DTL_REMARKS"].ToString(),
        //                    REGSubmissionDate = GetDateTime(row["REG_SUBMISSION_DATE"].ToString()),
        //                    REGApprovalDate = GetDateTime(row["REG_APPROVAL_DATE"].ToString()),
        //                    REGValidUptoDate = GetDateTime(row["REG_VALID_UPTO"].ToString()),
        //                    REGDarNo = row["REG_DAR_NO"].ToString(),
        //                    REGRemarks = row["REG_REMARKS"].ToString(),
        //                    PRCID = row["PRC_ID"].ToString(),
        //                    PRCSubmissionDate = GetDateTime(row["PRC_SUBMISSION_DATE"].ToString()),
        //                    PRCApprovalDate = GetDateTime(row["PRC_APPROVAL_DATE"].ToString()),
        //                    PRCRemarks = row["PRC_REMARKS"].ToString(),
        //                    MRKID = row["MRK_ID"].ToString(),
        //                    MRKSubmissionDate = GetDateTime(row["MRK_SUBMISSION_DATE"].ToString()),
        //                    MRKApprovalDate = GetDateTime(row["MRK_APPROVAL_DATE"].ToString()),
        //                    MRKValidUpto = GetDateTime(row["MRK_VALID_UPTO"].ToString()),
        //                    MRKRemarks = row["MRK_REMARKS"].ToString(),
        //                    AMDID = row["AMD_ID"].ToString(),
        //                    AMDSubmissionDate = GetDateTime(row["AMD_SUBMISSION_DATE"].ToString()),
        //                    AMDApprovalDate = GetDateTime(row["AMD_APPROVAL_DATE"].ToString()),
        //                    AMDRemarks = row["AMD_REMARKS"].ToString(), 
        //                }).ToList();
        //    return item;
        //}
        public ProductSummaryBEL ShowProductSummary(string ProductCode, string CompanyCode)
        {
            try
            {
                var query = new StringBuilder();

                query.Append(" SELECT A.BRAND_NAME, A.PRODUCT_CODE, A.PRODUCT_SPECIFICATION, A.GENERIC_CODE, A.COMPANY_CODE,  RCP.ID AS RCP_ID, RCP.SUBMISSION_DATE RCP_SUBMISSION_DATE,");
                query.Append(" RCP.APPROVAL_DATE RCP_APPROVAL_DATE, RCP.VALID_UPTO RCP_VALID_UPTO, RCP.REMARKS RCP_REMARKS, MRK.ID AS MRK_ID, ");
                query.Append(" MRK.MARKET_AUTHORIZATION_NO, MRK.SUBMISSION_DATE MRK_SUBMISSION_DATE,MRK.APPROVAL_DATE MRK_APPROVAL_DATE, MRK.VALID_UPTO MRK_VALID_UPTO, MRK.REMARKS MRK_REMARKS ");
                query.Append(" FROM PRODUCT_INFO A LEFT JOIN RECIPE_INFO RCP ON A.PRODUCT_CODE = RCP.PRODUCT_CODE AND A.COMPANY_CODE = RCP.COMPANY_CODE");
                query.Append(" LEFT JOIN MARKET_AUTH_CERTIFICATE MRK ON MRK.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE RCP.REVISION_NO IN ");
                query.Append(" (SELECT MAX (REVISION_NO) FROM RECIPE_INFO ");
                query.Append(" WHERE PRODUCT_CODE = A.PRODUCT_CODE AND COMPANY_CODE = A.COMPANY_CODE) ");
                query.Append(" AND A.PRODUCT_CODE = '" + ProductCode + "' ");

                if (!string.IsNullOrEmpty(CompanyCode))
                {
                    query.Append(" AND A.COMPANY_CODE = '" + CompanyCode + "'");
                }

                DataTable rdt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString()));
                ProductSummaryBEL item = new ProductSummaryBEL();

                if (rdt.Rows.Count > 0)
                {
                    item.BrandName = rdt.Rows[0]["BRAND_NAME"].ToString();
                    item.GenericCode = rdt.Rows[0]["GENERIC_CODE"].ToString();
                    item.CompanyCode = rdt.Rows[0]["COMPANY_CODE"].ToString();
                    item.ProductCode = rdt.Rows[0]["PRODUCT_CODE"].ToString();
                    item.ProductSpecification = rdt.Rows[0]["PRODUCT_SPECIFICATION"].ToString();
                    item.RCPID = rdt.Rows[0]["RCP_ID"].ToString();
                    item.RCPSubmissionDate = GetDateTime(rdt.Rows[0]["RCP_SUBMISSION_DATE"].ToString());
                    item.RCPApprovalDate = GetDateTime(rdt.Rows[0]["RCP_APPROVAL_DATE"].ToString());
                    item.RCPValidUpto = GetDateTime(rdt.Rows[0]["RCP_VALID_UPTO"].ToString());
                    item.RCPRemarks = rdt.Rows[0]["RCP_REMARKS"].ToString();
                    item.MRKID = rdt.Rows[0]["MRK_ID"].ToString();
                    item.MRKNumber = rdt.Rows[0]["MARKET_AUTHORIZATION_NO"].ToString();
                    item.MRKSubmissionDate = GetDateTime(rdt.Rows[0]["MRK_SUBMISSION_DATE"].ToString());
                    item.MRKApprovalDate = GetDateTime(rdt.Rows[0]["MRK_APPROVAL_DATE"].ToString());
                    item.MRKValidUpto = GetDateTime(rdt.Rows[0]["MRK_VALID_UPTO"].ToString());
                    item.MRKRemarks = rdt.Rows[0]["MRK_REMARKS"].ToString();
                }
                else
                {
                    item.BrandName = "";
                    item.GenericCode = "";
                    item.CompanyCode = "";
                    item.ProductCode = "";
                    item.RCPID = "";
                    item.ProductSpecification = "";
                    item.RCPSubmissionDate = "";
                    item.RCPApprovalDate = "";
                    item.RCPValidUpto = "";
                    item.RCPRemarks = "";
                    item.MRKID = "";
                    item.MRKNumber = "";
                    item.MRKSubmissionDate = "";
                    item.MRKApprovalDate = "";
                    item.MRKValidUpto = "";
                    item.MRKRemarks = "";
                }

                var query2 = new StringBuilder();

                query2.Append(" SELECT REG.ANNEX_ID AS REG_ID, REG.DTL_SUBMISSION_DATE DTL_SUBMISSION_DATE, REG.DTL_APPROVAL_DATE DTL_APPROVAL_DATE, REG.DTL_REMARKS DTL_REMARKS, ");
                query2.Append(" REG.SUBMISSION_DATE REG_SUBMISSION_DATE, REG.RENEWAL_DATE REG_APPROVAL_DATE, REG.VALID_UPTO REG_VALID_UPTO, REG.DAR_NO REG_DAR_NO, ");
                query2.Append(" REG.REMARKS REG_REMARKS FROM RECIPE_INFO RCP LEFT JOIN PRODUCT_REGISTRATION_INFO REG ON REG.RECIPE_ID = RCP.ID ");
                query2.Append(" WHERE REG.ANNEX_ID IN (SELECT MAX (ANNEX_ID) FROM PRODUCT_REGISTRATION_INFO WHERE RECIPE_ID = RCP.ID  AND (STATE_STATUS = 'New' OR STATE_STATUS = 'Renew')) AND RCP.PRODUCT_CODE = '" + ProductCode + "'  ");

                if (!string.IsNullOrEmpty(CompanyCode))
                {
                    query2.Append(" AND RCP.COMPANY_CODE = '" + CompanyCode + "'");
                }

                DataTable ddt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query2.ToString()));
                if (ddt.Rows.Count > 0)
                {
                    
                    item.REGID = ddt.Rows[0]["REG_ID"].ToString();
                    item.DTLSubmissionDate = GetDateTime(ddt.Rows[0]["DTL_SUBMISSION_DATE"].ToString());
                    item.DTLApprovalDate = GetDateTime(ddt.Rows[0]["DTL_APPROVAL_DATE"].ToString());
                    item.DTLRemarks = ddt.Rows[0]["DTL_REMARKS"].ToString();
                    item.REGSubmissionDate = GetDateTime(ddt.Rows[0]["REG_SUBMISSION_DATE"].ToString());
                    item.REGApprovalDate = GetDateTime(ddt.Rows[0]["REG_APPROVAL_DATE"].ToString());
                    item.REGValidUptoDate = GetDateTime(ddt.Rows[0]["REG_VALID_UPTO"].ToString());
                    item.REGDarNo = ddt.Rows[0]["REG_DAR_NO"].ToString();
                    item.REGRemarks = ddt.Rows[0]["REG_REMARKS"].ToString();
              
                }
                else
                {
                    item.REGID = "";
                    item.DTLSubmissionDate = "";
                    item.DTLApprovalDate = "";
                    item.DTLRemarks = "";
                    item.REGSubmissionDate = "";
                    item.REGApprovalDate = "";
                    item.REGValidUptoDate = "";
                    item.REGDarNo = "";
                    item.REGRemarks = "";              
                }

                var query4 = new StringBuilder();
                query4.Append(" SELECT PRC.ID AS PRC_ID,  PRC.SUBMISSION_DATE PRC_SUBMISSION_DATE, PRC.APPROVAL_DATE PRC_APPROVAL_DATE, ");
                query4.Append(" PRC.PRICE_PER_UNIT, PRC.REMARKS PRC_REMARKS FROM PRODUCT_PRICE PRC  WHERE PRC.ANNEX_ID ");
                query4.Append(" IN (SELECT ANNEX_ID FROM PRODUCT_REGISTRATION_INFO WHERE RECIPE_ID IN (SELECT ID FROM RECIPE_INFO WHERE PRODUCT_CODE = '" + ProductCode + "')) ORDER BY PRC.ID DESC ");

                DataTable prdt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query4.ToString()));

                if (prdt.Rows.Count > 0)
                {
                    item.PRCID = prdt.Rows[0]["PRC_ID"].ToString();
                    item.PRCSubmissionDate = GetDateTime(prdt.Rows[0]["PRC_SUBMISSION_DATE"].ToString());
                    item.PRCApprovalDate = GetDateTime(prdt.Rows[0]["PRC_APPROVAL_DATE"].ToString());
                    item.PRCApprovedPrice = prdt.Rows[0]["PRICE_PER_UNIT"].ToString();
                    item.PRCRemarks = prdt.Rows[0]["PRC_REMARKS"].ToString();
                }
                else
                {
                    item.PRCID = "";
                    item.PRCSubmissionDate = "";
                    item.PRCApprovalDate = "";
                    item.PRCRemarks = "";
                    item.PRCApprovedPrice = "";
                }

                var query3 = new StringBuilder();

                query3.Append(" SELECT REG.ANNEX_ID AS REG_ID, REG.SUBMISSION_DATE REG_SUBMISSION_DATE, REG.RENEWAL_DATE REG_APPROVAL_DATE, REG.REMARKS REG_REMARKS ");
                query3.Append(" FROM RECIPE_INFO RCP LEFT JOIN PRODUCT_REGISTRATION_INFO REG ON REG.RECIPE_ID = RCP.ID LEFT JOIN PRODUCT_PRICE PRC ON REG.ANNEX_ID = PRC.ANNEX_ID ");
                query3.Append(" WHERE REG.ANNEX_ID IN (SELECT MAX (ANNEX_ID) FROM PRODUCT_REGISTRATION_INFO WHERE RECIPE_ID = RCP.ID AND (STATE_STATUS = 'Annexure Amendment' OR STATE_STATUS = 'Packaging Amendment' )) ");
                query3.Append(" AND RCP.PRODUCT_CODE = '" + ProductCode + "' ");

                if (!string.IsNullOrEmpty(CompanyCode))
                {
                    query3.Append(" AND RCP.COMPANY_CODE = '" + CompanyCode + "'");
                }

                DataTable amdt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query3.ToString()));

                if (amdt.Rows.Count > 0)
                {
                    item.AMDID = amdt.Rows[0]["REG_ID"].ToString();
                    item.AMDSubmissionDate = GetDateTime(amdt.Rows[0]["REG_SUBMISSION_DATE"].ToString());
                    item.AMDApprovalDate = GetDateTime(amdt.Rows[0]["REG_APPROVAL_DATE"].ToString());
                    item.AMDRemarks = amdt.Rows[0]["REG_REMARKS"].ToString();
                }
                else
                {
                    item.AMDID = "";
                    item.AMDSubmissionDate = "";
                    item.AMDApprovalDate = "";
                    item.AMDRemarks = "";
                }

                return item;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public IList<ProductSummaryBEL> ShowProductList(string fromDate, string toDate, string DType, string CompanyCode)
        {
            if (string.IsNullOrEmpty(fromDate))
            {
                int yr = DateTime.Today.Year;
                fromDate = "01/01/" + yr.ToString();
            }

            if (string.IsNullOrEmpty(toDate))
            {
                DateTime crt = DateTime.Today;
                toDate = crt.ToString("dd/MM/yyyy");
            }

            var query = new StringBuilder();


            if (DType == "Recipe")
            {
                query.Append(" SELECT B.BRAND_NAME, B.GENERIC_CODE, TO_CHAR(A.APPROVAL_DATE, 'dd/mm/yyyy') APPROVAL_DATE FROM RECIPE_INFO A INNER JOIN PRODUCT_INFO B ON B.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE TO_DATE(A.APPROVAL_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
            }

            else if (DType == "DTL")
            {
                query.Append(" SELECT B.BRAND_NAME, B.GENERIC_CODE, TO_CHAR(C.DTL_APPROVAL_DATE, 'dd/mm/yyyy') APPROVAL_DATE FROM PRODUCT_REGISTRATION_INFO C ");
                query.Append(" INNER JOIN RECIPE_INFO A ON C.RECIPE_ID = A.ID INNER JOIN PRODUCT_INFO B ON B.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE TO_DATE(C.DTL_APPROVAL_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
                query.Append(" AND B.PRODUCT_SPECIFICATION = 'INN' AND C.STATE_STATUS = 'New' ");
            }

            else if (DType == "Product Registration")
            {
                query.Append(" SELECT B.BRAND_NAME, B.GENERIC_CODE, TO_CHAR(C.RENEWAL_DATE, 'dd/mm/yyyy') APPROVAL_DATE FROM PRODUCT_REGISTRATION_INFO C ");
                query.Append(" INNER JOIN RECIPE_INFO A ON C.RECIPE_ID = A.ID INNER JOIN PRODUCT_INFO B ON B.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE TO_DATE(C.RENEWAL_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
                query.Append(" AND C.STATE_STATUS = 'New' ");
            }
            else if (DType == "Price")
            {
                query.Append(" SELECT B.BRAND_NAME, B.GENERIC_CODE, TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy') APPROVAL_DATE FROM PRODUCT_PRICE D ");
                query.Append(" INNER JOIN PRODUCT_REGISTRATION_INFO C ON D.ANNEX_ID = C.ANNEX_ID ");
                query.Append(" INNER JOIN RECIPE_INFO A ON C.RECIPE_ID = A.ID INNER JOIN PRODUCT_INFO B ON B.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE TO_DATE(D.APPROVAL_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
            }
            else if (DType == "MA Certificate")
            {
                query.Append(" SELECT B.BRAND_NAME, B.GENERIC_CODE, TO_CHAR(A.APPROVAL_DATE, 'dd/mm/yyyy') APPROVAL_DATE FROM MARKET_AUTH_CERTIFICATE A ");
                query.Append(" INNER JOIN PRODUCT_INFO B ON B.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE TO_DATE(A.APPROVAL_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
            }
            else if (DType == "Amendment")
            {
                query.Append(" SELECT B.BRAND_NAME, B.GENERIC_CODE, TO_CHAR(C.RENEWAL_DATE, 'dd/mm/yyyy') APPROVAL_DATE FROM PRODUCT_REGISTRATION_INFO C ");
                query.Append(" INNER JOIN RECIPE_INFO A ON C.RECIPE_ID = A.ID INNER JOIN PRODUCT_INFO B ON B.PRODUCT_CODE = A.PRODUCT_CODE ");
                query.Append(" WHERE TO_DATE(C.RENEWAL_DATE,'DD/MM/RRRR') BETWEEN TO_DATE('" + fromDate + "','DD/MM/RRRR') AND TO_DATE('" + toDate + "','DD/MM/RRRR') ");
                query.Append(" AND (C.STATE_STATUS = 'Annexure Amendment' OR C.STATE_STATUS = 'Packaging Amendment') ");
            }

            if (!string.IsNullOrEmpty(CompanyCode))
            {
                query.Append("  AND A.COMPANY_CODE = '" + CompanyCode + "'  ");
            }

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString()));

            var item = (from DataRow row in dt.Rows
                        select new ProductSummaryBEL
                        {
                            BrandName = row["BRAND_NAME"].ToString(),
                            GenericCode = row["GENERIC_CODE"].ToString(),
                            PRCApprovalDate = row["APPROVAL_DATE"].ToString(),
                        }).ToList();
            return item;
        }

        private string GetDateTime(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string output = Convert.ToDateTime(input).ToString("dd-MM-yyyy");
                return output;
            }
            else
            {
                return input;
            }
        }

        public IList<MeetingInfoBEL> GetMeetingInfo()
        {
            var query = new StringBuilder();
            query.Append(" SELECT D.ID, D.MEETING_TYPE, D.REMARKS, D.MEETING_NAME,TO_CHAR(D.MEETING_DATE, 'dd/mm/yyyy')MEETING_DATE, TO_CHAR(D.MEETING_DATE,'RRRR') MEETING_YEAR,TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON, REMARKS ");
            query.Append(" FROM MEETING_INFO D");
            query.Append(" WHERE 1=1 ");
            query.Append(" AND D.MEETING_DATE >= TO_DATE('" + DateTime.Today.ToString("dd/MM/yyyy") + "','dd/MM/yyyy') ");
            query.Append(" ORDER BY  D.ID DESC ");

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString()));

            var item = (from DataRow row in dt.Rows
                        select new MeetingInfoBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            MeetingSubject = row["MEETING_NAME"].ToString(),
                            Remarks = row["REMARKS"].ToString(),
                            MeetingType = row["MEETING_TYPE"].ToString(),
                            MeetingDate = row["MEETING_DATE"].ToString(),
                            MeetingYear = row["MEETING_YEAR"].ToString(),
                            SetOn = row["SET_ON"].ToString(),
                        }).ToList();
            return item;
        }
    }
}