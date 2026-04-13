using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.DAL.Gateway;
using RMS_Square.Universal.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.DAO
{
    public class BrandExportDAO : ReturnData
    {
        DBConnection _dbConn = new DBConnection();
        DBHelper _dbHelper = new DBHelper();
        IDGenerated _idGenerated = new IDGenerated();
        public IList<BrandExportBEL> GetAll(BrandExportBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT E.ID,E.SLNO,E.COMPANY_CODE,C.COMPANY_NAME,C.ADDRESS,C.LICENSE_NO,E.EXPORTING_COUNTRY,TO_CHAR(E.SUBMISSION_DATE, 'YYYY')YEAR,TO_CHAR(E.SUBMISSION_DATE, 'Month') as MONTH_NAME, ");
            query.Append(" E.BRAND_NAME,E.DAR_NO,E.DOSAGE_FORM,E.GEN_STRENGTH,E.BRAND_NAME_EXPORT,E.PROPOSED_BY,");
            query.Append(" TO_CHAR(E.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(E.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE,TO_CHAR(E.RECEIVED_DATE, 'dd/mm/yyyy')RECEIVED_DATE FROM EXPORT_BRAND_INFO E");
            query.Append(" LEFT JOIN COMPANY_INFO C ON C.COMPANY_CODE=E.COMPANY_CODE ");
            query.Append(" WHERE 1=1 ");


            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  E.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString()));

            var item = (from DataRow row in dt.Rows
                        select new BrandExportBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            Address = row["ADDRESS"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            GenAndStrength = row["GEN_STRENGTH"].ToString(),
                            DarNo = row["DAR_NO"].ToString(),
                            DossageForm = row["DOSAGE_FORM"].ToString(),
                            ExportBrandName = row["BRAND_NAME_EXPORT"].ToString(),
                            ProposedBy = row["PROPOSED_BY"].ToString(),
                            ExportingCountry = row["EXPORTING_COUNTRY"].ToString(),
                            ReceivedDate = row["RECEIVED_DATE"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ApprovalDate = row["APPROVAL_DATE"].ToString(),
                            Year = row["YEAR"].ToString(),
                            Month = row["MONTH_NAME"].ToString()
                        }).ToList();
            return item;
        }

        public bool SaveUpdate(BrandExportBEL model, string userId)
        {
            bool isReturn = false;
            try
            {
                var query = new StringBuilder();
                if (model.ID > 0)
                {
                    //U for update
                    ReturnMaxID = model.ID;
                    MaxID = model.SlNo;
                    IUMode = "U";
                    query.Append(" UPDATE EXPORT_BRAND_INFO SET");
                    query.Append(" COMPANY_CODE='" + model.CompanyCode + "',");
                    query.Append(" BRAND_NAME='" + model.BrandName + "',");
                    query.Append(" DAR_NO='" + model.DarNo + "',");
                    query.Append(" GEN_STRENGTH='" + model.GenAndStrength + "',");
                    query.Append(" DOSAGE_FORM='" + model.DossageForm + "',");
                    query.Append(" BRAND_NAME_EXPORT='" + model.ExportBrandName + "',");
                    query.Append(" EXPORTING_COUNTRY='" + model.ExportingCountry + "',");
                    query.Append(" PROPOSED_BY='" + model.ProposedBy + "',");
                    query.Append(" RECEIVED_DATE= TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy'), ");
                    query.Append(" SUBMISSION_DATE= TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy'), ");
                    query.Append(" APPROVAL_DATE=TO_DATE('" + model.ApprovalDate + "','dd/MM/yyyy'), ");
                    query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')), ");
                    query.Append(" UPDATE_BY='" + userId + "'");
                    query.Append(" WHERE ID='" + model.ID + "'");
                }
                else
                { //I for Insert  
                    ReturnMaxID = _idGenerated.getMAXSL("EXPORT_BRAND_INFO", "ID");
                    MaxID = _idGenerated.getMAXID("EXPORT_BRAND_INFO", "SLNO", "fm000000000");
                    IUMode = "I";
                    query.Append(" INSERT INTO EXPORT_BRAND_INFO(");
                    query.Append(" ID,SLNO,COMPANY_CODE,EXPORTING_COUNTRY,BRAND_NAME,DAR_NO,GEN_STRENGTH,DOSAGE_FORM,BRAND_NAME_EXPORT,PROPOSED_BY,");
                    query.Append(" RECEIVED_DATE,SUBMISSION_DATE,APPROVAL_DATE,IS_DELETE,SET_BY, SET_ON)");
                    query.Append(" VALUES( '" + ReturnMaxID + "','" + MaxID + "','" + model.CompanyCode + "','" + model.ExportingCountry + "',");
                    query.Append(" '" + model.BrandName + "','" + model.DarNo + "','" + model.GenAndStrength + "','" + model.DossageForm + "','" + model.ExportBrandName + "','" + model.ProposedBy + "',");
                    query.Append(" TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy'), TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy'), TO_DATE('" + model.ApprovalDate + "','dd/MM/yyyy')," + "'N','");
                    query.Append(userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')))");
                }
                isReturn = _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());

            
                return isReturn;

            }
            catch (Exception errorException)
            {
                return false;
            }
        }

        public DataTable GetAllExportWithDetailForReport(ReportModel model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT ROW_NUMBER() OVER(ORDER BY E.ID)SN, E.ID,E.SLNO,E.COMPANY_CODE,C.COMPANY_NAME,C.ADDRESS,C.LICENSE_NO,E.EXPORTING_COUNTRY,TO_CHAR(SUBMISSION_DATE, 'YYYY')YEAR,TO_CHAR(SUBMISSION_DATE, 'Month') as MONTH_NAME, ");
            query.Append(" E.DAR_NO,E.BRAND_NAME,E.GEN_STRENGTH GENERIC_STRENGTH,E.DOSAGE_FORM,E.BRAND_NAME_EXPORT,E.EXPORTING_COUNTRY,");
            query.Append(" TO_CHAR(E.RECEIVED_DATE, 'dd/mm/yyyy')RECEIVED_DATE,TO_CHAR(E.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(E.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE FROM EXPORT_BRAND_INFO E");
            query.Append(" LEFT JOIN COMPANY_INFO C ON C.COMPANY_CODE=E.COMPANY_CODE ");
            query.Append(" WHERE 1=1 ");

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND C.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.BrandName))
            {
                query.Append(" AND TRIM(E.BRAND_NAME_EXPORT)='{1}'");
                model.BrandName = model.BrandName.Trim();
            }
            if (!string.IsNullOrEmpty(model.ExportCounty))
            {
                query.Append(" AND ED.EXPORTING_COUNTRY='{2}'");
            }
            if (model.ChooseOption != "All")
            {
                if (model.ChooseOption == "SubmissionDate")
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND E.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
                else if (model.ChooseOption == "ApprovalDate")
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND E.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND D.RECEIVED_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                {
                    query.Append(" AND ( E.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR E.RECEIVED_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR E.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                }
            }

            //if (!string.IsNullOrEmpty(orderBy))
            //{
            //    query.Append(" ORDER BY  E.ID " + orderBy);
            //}
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.BrandName, model.ExportCounty));

            return dt;
        }
    }
}