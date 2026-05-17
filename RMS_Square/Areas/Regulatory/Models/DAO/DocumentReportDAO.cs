using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.DAL.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RMS_Square.Areas.Regulatory.Models.DAO
{
    public class DocumentReportDAO
    {
        private DBConnection _dbConn = new DBConnection();
        private DBHelper _dbHelper = new DBHelper();

        public IList<DocumentReportVM> GetUploadedDocuments(DocumentReportParams param)
        {
            var query = new StringBuilder();

            // Joining DocumentFileInfo with Product_Registration_Info and Product/Company Info
            query.Append(" SELECT F.FILEID, F.FILECODE, F.EXTENTION, F.FILENAME, F.FILESIZE, F.REFNO AS DOCUMENT_NAME, ");
            query.Append(" TO_CHAR(F.SET_ON, 'dd/mm/yyyy') AS UPLOAD_DATE, ");
            query.Append(" C.COMPANY_NAME, P.PRODUCT_NAME, P.BRAND_NAME, D.AUTHORITY_NAME ");
            query.Append(" FROM DOCUMENTFILEINFO F ");
            query.Append(" INNER JOIN PRODUCT_REGISTRATION_INFO D ON F.REFLEVEL1 = TO_CHAR(D.ANNEX_ID) ");
            query.Append(" LEFT JOIN COMPANY_INFO C ON D.COMPANY_CODE = C.COMPANY_CODE ");
            query.Append(" LEFT JOIN PRODUCT_INFO P ON D.PRODUCT_CODE = P.PRODUCT_CODE ");
            query.Append(" WHERE F.FILETYPE = '155' "); // 155 is the FileType enum for Product Registration based on your code

            if (!string.IsNullOrEmpty(param.CompanyCode))
                query.AppendFormat(" AND D.COMPANY_CODE = '{0}' ", param.CompanyCode);

            if (!string.IsNullOrEmpty(param.ProductCode))
                query.AppendFormat(" AND D.PRODUCT_CODE = '{0}' ", param.ProductCode);

            if (!string.IsNullOrEmpty(param.AuthorityType))
                query.AppendFormat(" AND D.AUTHORITY_TYPE = '{0}' ", param.AuthorityType);

            if (!string.IsNullOrEmpty(param.DocumentType))
                query.AppendFormat(" AND D.DOCUMENT_TYPE = '{0}' ", param.DocumentType);

            if (!string.IsNullOrEmpty(param.FromDate) && !string.IsNullOrEmpty(param.ToDate))
            {
                query.Append(" AND F.SET_ON BETWEEN TO_DATE('" + param.FromDate + " 00:00:00','dd/mm/yyyy hh24:mi:ss') ");
                query.Append(" AND TO_DATE('" + param.ToDate + " 23:59:59','dd/mm/yyyy hh24:mi:ss') ");
            }

            query.Append(" ORDER BY F.FILEID DESC ");

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), query.ToString());

            var list = (from DataRow row in dt.Rows
                        select new DocumentReportVM
                        {
                            FileID = row["FILEID"].ToString(),
                            FileCode = row["FILECODE"].ToString(),
                            Extention = row["EXTENTION"].ToString(),
                            FileName = row["FILENAME"].ToString(),
                            FileSize = row["FILESIZE"].ToString(),
                            DocumentName = row["DOCUMENT_NAME"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            ProductName = row["PRODUCT_NAME"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            AuthorityName = row["AUTHORITY_NAME"].ToString(),
                            UploadDate = row["UPLOAD_DATE"].ToString()
                        }).ToList();

            return list;
        }
    }
}