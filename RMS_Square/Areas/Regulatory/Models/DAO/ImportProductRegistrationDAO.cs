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
    public class ImportProductRegistrationDAO : ReturnData
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;
        public ImportProductRegistrationDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }

        public bool SaveUpdate(ImportProductRegistrationBEL model, string userId)
        {
            try
            {
                var query = new StringBuilder();
                if (model.ID > 0)
                {
                    //U for update
                    ReturnMaxID = model.ID;
                    MaxID = model.SlNo;
                    RefNo = model.RevisionNo;
                    IUMode = "U";
                    query.Append(" UPDATE IMPORT_PRODUCT_REGISTRATION SET MANUFACTURER_NAME='" + model.ManufacturerName + "',SUPPLIER_NAME='" + model.SupplierName + "', PROPOSAL_DATE =(TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')),SUBMISSION_DATE=(TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')),");
                    query.Append("REGISTRATION_DATE =(TO_DATE('" + model.RegistrationDate + "','dd/MM/yyyy')),REGISTRATION_NO='" + model.RegistrationNo + "',");
                    query.Append(" VALID_UPTO =(TO_DATE('" + model.ValidUpto + "','dd/MM/yyyy')), NOTIFICATION_DAYS ='" + model.NotificationDays + "',REMARKS='" + model.Remarks + "',");
                    query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),UPDATE_BY='" + userId + "'");
                    query.Append(" WHERE ID='" + model.ID + "'");
                }
                else
                { //I for Insert  
                    ReturnMaxID = _idGenerated.getMAXSL("IMPORT_PRODUCT_REGISTRATION", "ID");
                    MaxID = _idGenerated.getMAXID("IMPORT_PRODUCT_REGISTRATION", "SLNO", "fm000000000");
                    string strCount = _dbHelper.GetValue("SELECT COUNT(1) AS RevisionNo FROM IMPORT_PRODUCT_REGISTRATION  WHERE PRODUCT_CODE='" + model.ProductCode + "' GROUP BY PRODUCT_CODE");
                    if (!string.IsNullOrEmpty(strCount))
                    {
                        RefNo = strCount;
                        //model.SubmissionType = "Renewal";
                    }
                    else
                    {
                        RefNo = "0";
                        //  model.SubmissionType = "License";
                    }

                    IUMode = "I";
                    //query.Append(" INSERT INTO IMPORT_PRODUCT_REGISTRATION(ID,SLNO,PRODUCT_CODE	,REVISION_NO,REVISION_DATE,COUNTRY_CODE,MANUFACTURER_NAME,SUPPLIER_NAME,PROPOSAL_DATE,SUBMISSION_DATE,REGISTRATION_DATE,REGISTRATION_NO,VALID_UPTO,NOTIFICATION_DAYS,REMARKS,SET_BY,SET_ON) VALUES ( ");
                    //query.Append("  '" + ReturnMaxID + "','" + MaxID + "','" + model.ProductCode + "','" + model.RevisionNo + "',(TO_DATE('" + model.RevisionDate + "','dd/MM/yyyy')),'" + model.CountryCode + "','" + model.ManufacturerName + "','" + model.SupplierName + "',(TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')),(TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.RegistrationDate + "','dd/MM/yyyy')) ");
                    //query.Append(" '" + model.RegistrationNo + "',(TO_DATE('" + model.ValidUpto + "','dd/MM/yyyy')),'" + model.NotificationDays + "','" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                    //query.Append(" )");

                    query.Append(" INSERT INTO IMPORT_PRODUCT_REGISTRATION(ID,SLNO,PRODUCT_CODE,REVISION_NO,REVISION_DATE,COUNTRY_CODE,MANUFACTURER_NAME,SUPPLIER_NAME,PROPOSAL_DATE,SUBMISSION_DATE,REGISTRATION_DATE,REGISTRATION_NO,VALID_UPTO,NOTIFICATION_DAYS,REMARKS,SET_BY,SET_ON,RECIPE_ID, COMPANY_CODE,IS_DELETE) ");
                    query.Append(" VALUES( '" + ReturnMaxID + "','" + MaxID + "','" + model.ProductCode + "','" + RefNo + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),'" + model.CountryCode + "',");
                    query.Append("'" + model.ManufacturerName + "','" + model.SupplierName + "',(TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')),(TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')),");
                    query.Append("(TO_DATE('" + model.RegistrationDate + "','dd/MM/yyyy')),'" + model.RegistrationNo + "',(TO_DATE('" + model.ValidUpto + "','dd/MM/yyyy')),");
                    query.Append(" '" + model.NotificationDays + "','" + model.Remarks + "','" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),'" + model.RecipeId + "','" + model.CompanyCode + "'");
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
        }

        public IList<ImportProductRegistrationBEL> GetAllInfo(ImportProductRegistrationBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append("SELECT A.ID, A.SLNO, A.PRODUCT_CODE,B.SAP_PRODUCT_CODE,A.RECIPE_ID,A.COMPANY_CODE,C.COMPANY_NAME,B.GENERIC_CODE,B.BRAND_NAME,B.PACK_SIZE_NAME,");
            query.Append("B.DOSAGE_FORM_CODE,D.DOSAGE_FORM_NAME,REVISION_NO,TO_CHAR(REVISION_DATE,'DD/MM/RRRR') REVISION_DATE, A.COUNTRY_CODE,COUNTRY_NAME,");
            query.Append("MANUFACTURER_NAME,SUPPLIER_NAME,TO_CHAR(PROPOSAL_DATE,'DD/MM/RRRR')PROPOSAL_DATE,TO_CHAR(SUBMISSION_DATE,'DD/MM/RRRR')SUBMISSION_DATE, ");
            query.Append("TO_CHAR(REGISTRATION_DATE,'DD/MM/RRRR') REGISTRATION_DATE,REGISTRATION_NO,TO_CHAR(VALID_UPTO,'DD/MM/RRRR')VALID_UPTO,NOTIFICATION_DAYS,A.REMARKS ");
            query.Append("FROM IMPORT_PRODUCT_REGISTRATION A, PRODUCT_INFO B, COUNTRY_INFO C,COMPANY_INFO C, DOSAGE_FORM_INFO D");
            query.Append(" WHERE A.PRODUCT_CODE=B.PRODUCT_CODE AND A.COMPANY_CODE=C.COMPANY_CODE AND  A.COUNTRY_CODE=C.COUNTRY_CODE(+)");
            query.Append(" AND B.DOSAGE_FORM_CODE=D.DOSAGE_FORM_CODE");

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND  A.COMPANY_CODE='{0}'");
            }

            if (!string.IsNullOrEmpty(model.ProductCode))
            {
                query.Append(" AND  A.PRODUCT_CODE='{1}'");
            }

            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND TO_DATE(A.SUBMISSION_DATE,'dd/mm/yyyy') BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  A.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.ProductCode));

            var item = (from DataRow row in dt.Rows
                        select new ImportProductRegistrationBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            RecipeId = row["RECIPE_ID"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            GenericCode = row["GENERIC_CODE"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            DosageFormCode = row["DOSAGE_FORM_CODE"].ToString(),
                            DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            ManufacturerName = row["MANUFACTURER_NAME"].ToString(),
                            SupplierName = row["SUPPLIER_NAME"].ToString(),
                            CountryCode = row["COUNTRY_CODE"].ToString(),
                            CountryName = row["COUNTRY_NAME"].ToString(),
                            ProposalDate = row["PROPOSAL_DATE"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            RegistrationDate = row["REGISTRATION_DATE"].ToString(),
                            RegistrationNo = row["REGISTRATION_NO"].ToString(),
                            ValidUpto = row["VALID_UPTO"].ToString(),
                            RevisionDate = row["REVISION_DATE"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString(),
                            Remarks = row["REMARKS"].ToString(),
                            NotificationDays = row["NOTIFICATION_DAYS"].ToString()
                        }).ToList();
            return item;
        }

        public List<ImportProductRegistrationBEL> GetImportProductList(string companyCode)
        {
           
            var query = new StringBuilder();
            query.Append(" SELECT R.RECIPE_ID,  P.PRODUCT_CODE,P.SAP_PRODUCT_CODE,R.COMPANY_CODE,C.COMPANY_NAME, P.GENERIC_CODE,P.DOSAGE_FORM_CODE,D.DOSAGE_FORM_NAME,P.PACK_SIZE_NAME,P.BRAND_NAME ");
            query.Append(" FROM PRODUCT_INFO P, DOSAGE_FORM_INFO D,(SELECT DISTINCT PRODUCT_CODE,SLNO RECIPE_ID, COMPANY_CODE FROM RECIPE_INFO )R , COMPANY_INFO C ");
            query.Append(" WHERE P.DOSAGE_FORM_CODE=D.DOSAGE_FORM_CODE(+) AND P.PRODUCT_CODE=R.PRODUCT_CODE AND P.MANUFACTURING_TYPE='Import' AND  R.COMPANY_CODE=C.COMPANY_CODE");
            if(!string.IsNullOrEmpty(companyCode))
            {
                query.Append(" AND C.COMPANY_CODE='" + companyCode + "'");
            }
            query.Append(" ORDER BY PRODUCT_CODE");
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), query.ToString());
            List<ImportProductRegistrationBEL> item;

            item = (from DataRow row in dt.Rows
                    select new ImportProductRegistrationBEL
                    {
                        ProductCode = row["PRODUCT_CODE"].ToString(),
                        SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                        RecipeId = row["RECIPE_ID"].ToString(),
                        CompanyCode = row["COMPANY_CODE"].ToString(),
                        CompanyName = row["COMPANY_NAME"].ToString(),
                        GenericCode = row["GENERIC_CODE"].ToString(),
                        GenAndStrength = row["GENERIC_CODE"].ToString(),
                        DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                        PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                        BrandName = row["BRAND_NAME"].ToString()
                    }).ToList();
            return item;
        }
        public IList<ImportProductRegistrationBEL> GetImportProductExpireLicense(ImportProductRegistrationBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT A.ID,A.SLNO,A.REVISION_NO,A.SUBMISSION_DATE,A.PROPOSAL_DATE,A.REGISTRATION_DATE,A.MANUFACTURER_NAME,A.SUPPLIER_NAME,A.REGISTRATION_NO,");
            query.Append(" A.NOTIFICATION_DAYS,A.PRODUCT_CODE,A.SAP_PRODUCT_CODE,A.COMPANY_CODE,A.COMPANY_NAME,A.GENERIC_CODE,A.PACK_SIZE_NAME,");
            query.Append(" A.DOSAGE_FORM_NAME,ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0)DateDiff,TO_CHAR(A.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO");
            query.Append(" FROM ( SELECT D.ID,D.SLNO,D.REVISION_NO,TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,");
            query.Append(" TO_CHAR(D.REGISTRATION_DATE, 'dd/mm/yyyy')REGISTRATION_DATE, TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,");
            query.Append(" D.VALID_UPTO,D.MANUFACTURER_NAME,D.SUPPLIER_NAME,D.REGISTRATION_NO,D.REMARKS,D.NOTIFICATION_DAYS,TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,D.COMPANY_CODE, P.PRODUCT_CODE,");
            query.Append(" P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME,C.COMPANY_NAME  FROM IMPORT_PRODUCT_REGISTRATION D ");
            query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE ");
            query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE ");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ");
            query.Append(" WHERE D.IS_DELETE <>'Y') A INNER JOIN ( SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM IMPORT_PRODUCT_REGISTRATION GROUP BY COMPANY_CODE, PRODUCT_CODE) B ");
            query.Append(" ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO  WHERE 1=1 ");

            if (!string.IsNullOrEmpty(model.ProductCode))
            {
                query.Append(" AND  A.PRODUCT_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.NotificationDays))
            {
                query.Append(" AND ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0) <= {1}");
            }
            
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND A.COMPANY_CODE='{2}'");
            }
            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND A.VALID_UPTO BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            }
            if (string.IsNullOrEmpty(model.CompanyCode) && string.IsNullOrEmpty(model.ProductCode) && string.IsNullOrEmpty(model.NotificationDays) && string.IsNullOrEmpty(model.FromDate) && string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ");
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  A.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.ProductCode, model.NotificationDays, model.CompanyCode));

            var item = (from DataRow row in dt.Rows
                        select new ImportProductRegistrationBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString(),
                            ProposalDate = row["PROPOSAL_DATE"].ToString(),
                            RegistrationDate = row["REGISTRATION_DATE"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ValidUpto = row["VALID_UPTO"].ToString(),
                            NotificationDays = row["NOTIFICATION_DAYS"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_CODE"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            ManufacturerName = row["MANUFACTURER_NAME"].ToString(),
                            SupplierName = row["SUPPLIER_NAME"].ToString(),
                            RegistrationNo = row["REGISTRATION_NO"].ToString(),
                            DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            DateDiff = Convert.ToInt32(row["DateDiff"])
                        }).ToList();
            return item;
        }

        public DataTable GetFileRefno(int p, string refLevel1)
        {
            var query = new StringBuilder();
            query.Append(" SELECT filetype,PPM,PA,PD,DA,CF FROM ");
            query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
            query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Proposed Packaging Material' PPM,'Proposed Annexure' PA,'Product Dossier' PD,'Distribution Agreement' DA,'CPP/FSC' CF))");
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), p, refLevel1));
            return dt;
        }

        public void UpdateFileRelatedInfo(ImportProductRegistrationBEL model, string userId)
        {
            var query = new StringBuilder();
            if (model.ID > 0)
            {
                //U for update
                ReturnMaxID = model.ID;
                query.Append(" UPDATE IMPORT_PRODUCT_REGISTRATION SET PROPOSAL_DATE =(TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')), ");
                query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                query.Append(" WHERE ID='" + model.ID + "'");
            }
            _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());
        }
    }
}