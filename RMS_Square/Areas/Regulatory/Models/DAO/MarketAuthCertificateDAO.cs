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
    public class MarketAuthCertificateDAO : ReturnData
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;

        public MarketAuthCertificateDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }

        public List<MarketAuthCertificateBEL> GetMarketAuthCertificateList()
        {
            var query = new StringBuilder();

            query.Append(" SELECT D.ID,D.SLNO,D.REVISION_NO,D.MARKET_AUTHORIZATION_NO,RP.DAR_NO,C.Address, ");
            query.Append(" TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE, TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE, TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,D.REMARKS,D.NOTIFICATION_DAYS, ");
            query.Append(" TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,P.PRODUCT_CODE,P.BRAND_NAME,P.PRODUCT_CATEGORY,C.COMPANY_CODE,C.COMPANY_NAME, C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME");
            query.Append(" FROM MARKET_AUTH_CERTIFICATE D  LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ");
            query.Append(" LEFT JOIN ( SELECT R.PRODUCT_CODE,R.COMPANY_CODE,PR.DAR_NO FROM ");
            query.Append(" (SELECT A.ID, A.PRODUCT_CODE,A.COMPANY_CODE FROM RECIPE_INFO A INNER JOIN (SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM RECIPE_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE)");
            query.Append(" B ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO)R");
            query.Append(" INNER JOIN( SELECT A.ANNEX_ID,A.ANNEXURE_NO,A.REVISION_NO,A.RECIPE_ID,A.DAR_NO FROM PRODUCT_REGISTRATION_INFO A INNER JOIN (SELECT RECIPE_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_REGISTRATION_INFO GROUP BY RECIPE_ID) B ON B.RECIPE_ID=A.RECIPE_ID AND B.MaxRvNo=A.REVISION_NO");
            query.Append(" )PR ON R.ID=PR.RECIPE_ID ) RP ON RP.PRODUCT_CODE=P.PRODUCT_CODE AND RP.COMPANY_CODE=C.COMPANY_CODE");
            query.Append(" AND P.STATUS = 'Active' ");
            query.Append(" ORDER BY D.ID DESC");

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), query.ToString());
            List<MarketAuthCertificateBEL> item;

            item = (from DataRow row in dt.Rows
                    select new MarketAuthCertificateBEL
                    {
                        ID = Convert.ToInt64(row["ID"]),
                        SlNo = row["SLNO"].ToString(),
                        RevisionNo = row["REVISION_NO"].ToString(),
                        CompanyCode = row["COMPANY_CODE"].ToString(),
                        CompanyName = row["COMPANY_NAME"].ToString(),
                        Address = row["ADDRESS"].ToString(),
                        ProductCode = row["PRODUCT_CODE"].ToString(),
                        SapProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                        GenericStrength = row["GENERIC_CODE"].ToString(),
                        PackSize = row["PACK_SIZE_NAME"].ToString(),
                        DosageForm = row["DOSAGE_FORM_NAME"].ToString(),
                        DarNo = row["DAR_NO"].ToString(),
                        BrandName = row["BRAND_NAME"].ToString(),
                        SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                        ReceiveDate = row["RECEIVE_DATE"].ToString(),
                        ApprovalDate = row["APPROVAL_DATE"].ToString(),
                        ValiduptoDate = row["VALID_UPTO"].ToString(),
                        MarketAuthorizationNo = row["MARKET_AUTHORIZATION_NO"].ToString(),
                        NotificationDay = row["NOTIFICATION_DAYS"].ToString(),
                        Remarks = row["REMARKS"].ToString()
                    }).ToList();
            return item;
        }

        public List<MarketAuthCertificateBEL> GetCompany()
        {
            var selectQuery = new StringBuilder();

            selectQuery.Append("SELECT COMPANY_CODE,COMPANY_NAME,ADDRESS FROM COMPANY_INFO ORDER BY COMPANY_CODE");

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), selectQuery.ToString());
            List<MarketAuthCertificateBEL> item;

            item = (from DataRow row in dt.Rows
                    select new MarketAuthCertificateBEL
                    {

                        CompanyCode = row["COMPANY_CODE"].ToString(),
                        CompanyName = row["COMPANY_NAME"].ToString(),
                        Address = row["ADDRESS"].ToString(),
                    }).ToList();
            return item;
        }


        public List<MarketAuthCertificateBEL> GetProduct()
        {
            var query = new StringBuilder();

            //selectQuery.Append("SELECT PRODUCT_CODE, SAP_PRODUCT_CODE,GENERIC_STRENGTH, PACK_SIZE_NAME, DOSAGE_FORM_NAME,DAR_NO,BRAND_NAME  FROM MAC_PROD_INFO ORDER BY PRODUCT_CODE");
            query.Append(" SELECT A.DAR_NO,A.PRODUCT_CODE,A.BRAND_NAME,A.PRODUCT_CATEGORY,A.COMPANY_CODE,A.COMPANY_NAME,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME,");
            query.Append(" A.LICENSE_NO,A.DOSAGE_FORM_NAME FROM ( SELECT D.ANNEX_ID,D.REVISION_NO,D.RECIPE_ID,D.DAR_NO,P.PRODUCT_CODE,P.BRAND_NAME,P.PRODUCT_CATEGORY, ");
            query.Append(" C.COMPANY_CODE,C.COMPANY_NAME, C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME  ");
            query.Append(" FROM PRODUCT_REGISTRATION_INFO D LEFT JOIN  RECIPE_INFO R ON R.ID=D.RECIPE_ID  LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE ");
            query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE ");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE  WHERE D.IS_DELETE <>'Y' AND P.STATUS = 'Active' ) A ");
            query.Append(" LEFT JOIN ( SELECT RECIPE_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_REGISTRATION_INFO GROUP BY RECIPE_ID) B ");
            query.Append(" ON B.RECIPE_ID=A.RECIPE_ID AND B.MaxRvNo=A.REVISION_NO WHERE 1=1 ");
            query.Append(" ORDER BY A.PRODUCT_CODE");

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), query.ToString());
            List<MarketAuthCertificateBEL> item;

            item = (from DataRow row in dt.Rows
                    select new MarketAuthCertificateBEL
                    {
                        ProductCode = row["PRODUCT_CODE"].ToString(),
                        SapProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                        GenericStrength = row["GENERIC_CODE"].ToString(),
                        PackSize = row["PACK_SIZE_NAME"].ToString(),
                        DosageForm = row["DOSAGE_FORM_NAME"].ToString(),
                        DarNo = row["DAR_NO"].ToString(),
                        BrandName = row["BRAND_NAME"].ToString()
                    }).ToList();
            return item;
        }
        public bool SaveUpdate(MarketAuthCertificateBEL master, string userId)
        {
            try
            {
                String setBy = userId;
                string setOn = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                String updateBy = userId;
                String updateDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                var query = new StringBuilder();
                if (master.ID > 0)
                {
                    //U for Insert
                    RefNo = master.RevisionNo;
                    IUMode = "U";
                    ReturnMaxID = master.ID;
                    query.Append(" UPDATE MARKET_AUTH_CERTIFICATE SET ");
                    query.Append(" MARKET_AUTHORIZATION_NO='" + master.MarketAuthorizationNo + "',");
                    query.Append(" COMPANY_CODE='" + master.CompanyCode + "',");
                    query.Append(" PRODUCT_CODE='" + master.ProductCode + "',");
                    query.Append(" SUBMISSION_DATE= TO_DATE('" + master.SubmissionDate + "','dd/MM/yyyy'),");
                    query.Append(" RECEIVE_DATE= TO_DATE('" + master.ReceiveDate + "','dd/MM/yyyy'),");
                    query.Append(" APPROVAL_DATE= TO_DATE('" + master.ApprovalDate + "','dd/MM/yyyy'),");
                    query.Append(" VALID_UPTO= TO_DATE('" + master.ValiduptoDate + "','dd/MM/yyyy'),");
                    query.Append(" NOTIFICATION_DAYS='" + master.NotificationDay + "',");
                    query.Append(" REMARKS='" + master.Remarks + "'");
                    query.Append(" Where ID=" + master.ID);

                }
                else
                {
                    //I for Insert 
                    ReturnMaxID = _idGenerated.getMAXSL("MARKET_AUTH_CERTIFICATE", "ID");
                    string strCount = _dbHelper.GetValue("SELECT COUNT(1) AS RevisionNo FROM MARKET_AUTH_CERTIFICATE  WHERE COMPANY_CODE='" + master.CompanyCode + "' AND PRODUCT_CODE='" + master.ProductCode + "' GROUP BY COMPANY_CODE,PRODUCT_CODE");
                    if (!string.IsNullOrEmpty(strCount))
                    {
                        RefNo = strCount;
                        if (!string.IsNullOrEmpty(master.SlNo))
                        {
                            MaxID = master.SlNo;
                        }
                        else
                        {
                            MaxID = _dbHelper.GetValue("SELECT MAX(SLNO)SLNO FROM MARKET_AUTH_CERTIFICATE  WHERE IS_DELETE='N' AND COMPANY_CODE='" + master.CompanyCode + "' AND PRODUCT_CODE='" + master.ProductCode + "'  GROUP BY COMPANY_CODE,PRODUCT_CODE");
                        }
                    }
                    else
                    {
                        RefNo = "0";
                        MaxID = _idGenerated.getMAXID("MARKET_AUTH_CERTIFICATE", "SLNO", "fm000000000");
                    }

                    IUMode = "I";
                    query.Append(" INSERT INTO MARKET_AUTH_CERTIFICATE(ID, SLNO,MARKET_AUTHORIZATION_NO, COMPANY_CODE,PRODUCT_CODE, SUBMISSION_DATE,RECEIVE_DATE, APPROVAL_DATE, VALID_UPTO, NOTIFICATION_DAYS, REMARKS, SET_BY, SET_ON,REVISION_NO) ");
                    query.Append(" VALUES(" + ReturnMaxID + ",'" + MaxID + "','" + master.MarketAuthorizationNo + "','" + master.CompanyCode + "','" + master.ProductCode + "', TO_DATE('" + master.SubmissionDate + "','dd/MM/yyyy')" + ", TO_DATE('" + master.ReceiveDate + "','dd/MM/yyyy')" + ",");
                    query.Append(" TO_DATE('" + master.ApprovalDate + "','dd/MM/yyyy')" + ", TO_DATE('" + master.ValiduptoDate + "','dd/MM/yyyy')" + ",'" + master.NotificationDay + "','" + master.Remarks + "','");
                    query.Append(setBy + "', TO_DATE('" + setOn + "','dd/MM/yyyy HH24:mi:ss')");
                    query.Append(" ,'" + RefNo + "')");
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
        public IList<MarketAuthCertificateBEL> GetMACertificateExpiry(MarketAuthCertificateBEL model, string orderBy)
        {

            var query = new StringBuilder();
            query.Append(" SELECT A.ID,A.SLNO,A.REVISION_NO,A.SUBMISSION_DATE,A.APPROVAL_DATE,A.RECEIVE_DATE,A.MARKET_AUTHORIZATION_NO, A.NOTIFICATION_DAYS, ");
            query.Append(" A.PRODUCT_CODE,A.BRAND_NAME,A.PRODUCT_CATEGORY,A.COMPANY_CODE,A.COMPANY_NAME,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME,A.LICENSE_NO,");
            query.Append(" A.DOSAGE_FORM_NAME,ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0)DateDiff,TO_CHAR(A.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO");
            query.Append(" FROM (SELECT D.ID,D.SLNO,D.REVISION_NO,D.MARKET_AUTHORIZATION_NO, ");
            query.Append(" TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE, TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE, TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,D.VALID_UPTO,D.REMARKS,D.NOTIFICATION_DAYS,");
            query.Append(" TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,P.PRODUCT_CODE,P.BRAND_NAME,P.PRODUCT_CATEGORY,C.COMPANY_CODE,C.COMPANY_NAME, C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME");
            query.Append(" FROM MARKET_AUTH_CERTIFICATE D ");
            query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE");
            query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE");
            query.Append(" ) A INNER JOIN ( SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM MARKET_AUTH_CERTIFICATE GROUP BY COMPANY_CODE,PRODUCT_CODE) B ");
            query.Append(" ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO");
            query.Append(" WHERE 1=1 ");
            query.Append(" AND P.STATUS = 'Active' ");
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND  A.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.ProductCode))
            {
                query.Append(" AND  A.PRODUCT_CODE='{1}'");
            }
            if (!string.IsNullOrEmpty(model.AlarmDays))
            {
                query.Append(" AND ROUND(((A.VALID_UPTO)-(SELECT  SYSDATE FROM DUAL)),0) <= {2}");
            }
            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND A.VALID_UPTO BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            }
            if (string.IsNullOrEmpty(model.CompanyCode) && string.IsNullOrEmpty(model.ProductCode) && string.IsNullOrEmpty(model.AlarmDays) && string.IsNullOrEmpty(model.FromDate) && string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND ROUND(((A.VALID_UPTO)-(SELECT SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ");
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  A.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.ProductCode, model.AlarmDays));

            var item = (from DataRow row in dt.Rows
                        select new MarketAuthCertificateBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SapProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_CODE"].ToString(),
                            PackSize = row["PACK_SIZE_NAME"].ToString(),
                            DosageForm = row["DOSAGE_FORM_NAME"].ToString(),
                            // DarNo = row["DAR_NO"].ToString(),
                            ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ReceiveDate = row["RECEIVE_DATE"].ToString(),
                            ApprovalDate = row["APPROVAL_DATE"].ToString(),
                            ValiduptoDate = row["VALID_UPTO"].ToString(),
                            MarketAuthorizationNo = row["MARKET_AUTHORIZATION_NO"].ToString(),
                            AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                            DateDiff = Convert.ToInt32(row["DateDiff"])
                        }).ToList();
            return item;
        }
        public IList<MarketAuthCertificateBEL> GetAll(MarketAuthCertificateBEL model, string orderBy)
        {
            var query = new StringBuilder();

            query.Append(" SELECT D.ID,D.SLNO,D.REVISION_NO,D.MARKET_AUTHORIZATION_NO, ");
            query.Append(" TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE, TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE, TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE, TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,D.REMARKS,D.NOTIFICATION_DAYS,");
            query.Append(" TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,P.PRODUCT_CODE,P.BRAND_NAME,P.PRODUCT_CATEGORY,C.COMPANY_CODE,C.COMPANY_NAME, C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME");
            query.Append(" FROM MARKET_AUTH_CERTIFICATE D ");
            query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE");
            query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE");
            query.Append(" WHERE 1=1 ");
            query.Append(" AND P.STATUS = 'Active' ");
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND  D.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.ProductCode))
            {
                query.Append(" AND  D.PRODUCT_CODE='{1}'");
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
                        query.Append(" AND D.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
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
                    query.Append(" OR D.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                }
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  D.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.ProductCode));

            var item = (from DataRow row in dt.Rows
                        select new MarketAuthCertificateBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SapProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_CODE"].ToString(),
                            PackSize = row["PACK_SIZE_NAME"].ToString(),
                            DosageForm = row["DOSAGE_FORM_NAME"].ToString(),
                            // DarNo = row["DAR_NO"].ToString(),
                            ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ReceiveDate = row["RECEIVE_DATE"].ToString(),
                            ApprovalDate = row["APPROVAL_DATE"].ToString(),
                            ValiduptoDate = row["VALID_UPTO"].ToString(),
                            MarketAuthorizationNo = row["MARKET_AUTHORIZATION_NO"].ToString(),
                            AlarmDays = row["NOTIFICATION_DAYS"].ToString()
                            //,DateDiff = Convert.ToInt32(row["DateDiff"])
                        }).ToList();
            return item;
        }

        public DataTable GetMACertificateForReport(ReportModel model, string orderBy)
        {
            var query = new StringBuilder();
            //query.Append(" SELECT ROW_NUMBER() OVER(ORDER BY MA.ID)SN, MA.ID,MA.MARKET_AUTHORIZATION_NO MANo, C.COMPANY_CODE,C.COMPANY_NAME, P.PRODUCT_CODE,P.SAP_PRODUCT_CODE,P.BRAND_NAME,P.GENERIC_CODE GENERIC_STRENGTH,P.PRODUCT_SPECIFICATION,P.PRODUCT_CATEGORY,DF.DOSAGE_FORM_NAME, P.PACK_SIZE_NAME,");
            //query.Append(" PR.DAR_NO, PP.PRICE_PER_UNIT,PP.PROPOSED_BY,MA.SUBMISSION_DATE,MA.APPROVAL_DATE FROM(SELECT A.ID, A.PRODUCT_CODE,A.COMPANY_CODE FROM RECIPE_INFO A INNER JOIN (SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM RECIPE_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) ");
            //query.Append(" B ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO WHERE 1=1");
    
            //query.Append(" )R INNER JOIN(SELECT A.ANNEX_ID,A.ANNEXURE_NO,A.REVISION_NO,A.RECIPE_ID,A.DAR_NO FROM PRODUCT_REGISTRATION_INFO A INNER JOIN (SELECT RECIPE_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_REGISTRATION_INFO GROUP BY RECIPE_ID) B ON B.RECIPE_ID=A.RECIPE_ID AND B.MaxRvNo=A.REVISION_NO ");
            //query.Append(" )PR ON R.ID=PR.RECIPE_ID INNER JOIN (SELECT A.ANNEX_ID,A.PRICE_PER_UNIT,A.PROPOSED_BY FROM PRODUCT_PRICE A INNER JOIN (SELECT ANNEX_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_PRICE GROUP BY ANNEX_ID) B ON B.ANNEX_ID=A.ANNEX_ID AND B.MaxRvNo=A.REVISION_NO)PP ");
            //query.Append(" ON PP.ANNEX_ID=PR.ANNEX_ID INNER JOIN (SELECT A.MARKET_AUTHORIZATION_NO,A.ID, A.COMPANY_CODE,A.PRODUCT_CODE,A.SUBMISSION_DATE,A.APPROVAL_DATE,A.VALID_UPTO FROM MARKET_AUTH_CERTIFICATE A INNER JOIN (SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo ");
            //query.Append(" FROM MARKET_AUTH_CERTIFICATE GROUP BY COMPANY_CODE,PRODUCT_CODE) B ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO WHERE 1=1 ");


            query.Append(" SELECT * FROM VW_RPT_MA_CERTIFICATE A WHERE 1=1 ");

            if (model.ChooseOption != "All")
            {
                if (model.ChooseOption == "SubmissionDate")
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND A.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
                else if (model.ChooseOption == "ApprovalDate")
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND A.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND A.RECEIVE_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                {
                    query.Append(" AND ( A.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR A.RECEIVE_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR A.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                }
            }

            //query.Append("  )MA ON MA.COMPANY_CODE=R.COMPANY_CODE AND MA.PRODUCT_CODE=R.PRODUCT_CODE INNER JOIN COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE INNER JOIN PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE INNER JOIN DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ");
            //query.Append(" WHERE 1=1 ");

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                //query.Append(" AND P.COMPANY_CODE ='" + model.CompanyCode + "'");
                query.Append(" AND A.COMPANY_CODE ='" + model.CompanyCode + "'");
            }
            if (!string.IsNullOrEmpty(model.BrandName))
            {
                //query.Append(" AND TRIM(P.BRAND_NAME) ='" + model.BrandName.Trim() + "'"); 
                query.Append(" AND TRIM(A.BRAND_NAME) ='" + model.BrandName.Trim() + "'");
            }

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString()));

            return dt;
        }
        public DataTable GetFileRefno(int p, string refLevel1)
        {
            var query = new StringBuilder();
            query.Append(" SELECT filetype,CP FROM ");
            query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
            query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Commercial Packaging' CP))");

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), p, refLevel1));
            return dt;
        }

        public void UpdateFileRelatedInfo(MarketAuthCertificateBEL model, string userId)
        {
            var query = new StringBuilder();
            if (model.ID > 0)
            {
                //U for update
                ReturnMaxID = model.ID;
                query.Append(" UPDATE MARKET_AUTH_CERTIFICATE SET RECEIVE_DATE =(TO_DATE('" + model.ReceiveDate + "','dd/MM/yyyy')), ");
                query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                query.Append(" WHERE ID='" + model.ID + "'");
            }
            _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());
        }
    }
}