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
    public class AdvertisementInfoDAO : ReturnData
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;

        public AdvertisementInfoDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }
        public void UpdateFileRelatedInfo(AdvertisementInfoBEL model, string userId)
        {
            var query = new StringBuilder();
            if (model.ID > 0)
            {
                //U for update
                ReturnMaxID = model.ID;
                MaxID = model.SlNo;
                IUMode = "U";
                query.Append(" UPDATE ADVERTISEMENT_INFO SET PROPOSAL_DATE =(TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')), ");
                query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                query.Append(" WHERE ID='" + model.ID + "'");
            }
            _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());
        }
        public DataTable GetFileRefno(int p, string refLevel1)
        {
            var query = new StringBuilder();
            query.Append(" SELECT filetype,PD FROM ");
            query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
            query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Proposed Document' PD))");

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), p, refLevel1));
            return dt;
        }
        public List<AdvertisementInfoBEL> GetAdvertisementList(AdvertisementInfoBEL model, string orderBy)
        {
            var query = new StringBuilder();
            //query.Append("SELECT * FROM VW_ADVERTISEMENT_INFO ORDER BY ID DESC");
            query.Append(" SELECT PM.ID,PM.SLNO, PM.REVISION_NO, C.COMPANY_CODE,C.COMPANY_NAME, P.PRODUCT_CODE,P.SAP_PRODUCT_CODE,P.BRAND_NAME,P.GENERIC_CODE GENERIC_STRENGTH,P.PRODUCT_SPECIFICATION,P.PRODUCT_CATEGORY,DF.DOSAGE_FORM_NAME, ");
            query.Append(" P.PACK_SIZE_NAME,PR.DAR_NO,PR.ANNEXURE_NO,TO_CHAR(PM.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,TO_CHAR(PM.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(PM.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE,TO_CHAR(PM.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO_DATE,PM.REMARKS,PM.NOTIFICATION_DAYS ");
            query.Append(" FROM(SELECT A.ID,A.SLNO, A.PRODUCT_CODE,A.COMPANY_CODE,A.REVISION_NO,A.PROPOSAL_DATE,A.SUBMISSION_DATE,A.APPROVAL_DATE,A.VALID_UPTO,A.REMARKS,A.NOTIFICATION_DAYS FROM ADVERTISEMENT_INFO A ");
            //query.Append(" INNER JOIN (SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM ADVERTISEMENT_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) B ");
            //query.Append(" ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO WHERE 1=1");
            query.Append(" WHERE 1=1");
            
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND A.COMPANY_CODE='{0}'");
            }

            if (model.ChooseOption != "All")
            {
                if (model.ChooseOption == "SubmissionDate")
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND A.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND A.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                {
                    query.Append(" AND ( A.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR A.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                }
            }

            query.Append(" )PM LEFT JOIN (SELECT A.ID, A.PRODUCT_CODE,A.COMPANY_CODE FROM RECIPE_INFO A");
            query.Append(" INNER JOIN (SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM RECIPE_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) B");
            query.Append(" ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO WHERE 1=1 ");
            query.Append(" )R ON R.COMPANY_CODE=PM.COMPANY_CODE AND R.PRODUCT_CODE=PM.PRODUCT_CODE");
            query.Append(" LEFT JOIN(SELECT A.ANNEX_ID,A.RECIPE_ID,A.DAR_NO,A.ANNEXURE_NO FROM PRODUCT_REGISTRATION_INFO A");
            query.Append(" INNER JOIN (SELECT RECIPE_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_REGISTRATION_INFO GROUP BY RECIPE_ID) B");
            query.Append(" ON B.RECIPE_ID=A.RECIPE_ID AND B.MaxRvNo=A.REVISION_NO WHERE 1=1");
            query.Append(" )PR ON PR.RECIPE_ID=R.ID INNER JOIN PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE");
            query.Append(" INNER JOIN COMPANY_INFO C ON C.COMPANY_CODE=P.COMPANY_CODE INNER JOIN DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE");
            query.Append(" WHERE 1=1 ");

            if (!string.IsNullOrEmpty(model.BrandName))
            {
                query.Append(" AND TRIM(P.BRAND_NAME)='{1}'");
                model.BrandName = model.BrandName.Trim();
            }
            if (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("DESC"))
            {
                query.Append(" ORDER BY PM.ID DESC ");
            }
            else
            {
                query.Append(" ORDER BY PM.ID ASC ");
            }
            
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.BrandName));
            List<AdvertisementInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new AdvertisementInfoBEL
                    {
                        ID = Convert.ToInt64(row["ID"]),
                        SlNo = row["SLNO"].ToString(),
                        RevisionNo = row["REVISION_NO"].ToString(),
                        ProductCode = row["PRODUCT_CODE"].ToString(),                     
                        SapProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                        GenericStrength = row["GENERIC_STRENGTH"].ToString(),
                        PackSize = row["PACK_SIZE_NAME"].ToString(),
                        DosageForm = row["DOSAGE_FORM_NAME"].ToString(),
                        DarNo = row["DAR_NO"].ToString(),
                        BrandName = row["BRAND_NAME"].ToString(),
                        CompanyCode=row["COMPANY_CODE"].ToString(),
                        CompanyName=row["COMPANY_NAME"].ToString(),
                        AnnexureNo = row["ANNEXURE_NO"].ToString(),
                        ProposalDate = row["PROPOSAL_DATE"].ToString(),
                        SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                        ApprovalDate = row["APPROVAL_DATE"].ToString(),
                        ValiduptoDate = row["VALID_UPTO_DATE"].ToString(),
                        NotificationDay = row["NOTIFICATION_DAYS"].ToString(),
                        Remarks = row["REMARKS"].ToString()

                    }).ToList();
            return item;
        }

        public List<AdvertisementInfoBEL> GetProductList()
        {
            var selectQuery = new StringBuilder();
            selectQuery.Append("SELECT * FROM VW_PRODUCT_INFO");
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), selectQuery.ToString());
            List<AdvertisementInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new AdvertisementInfoBEL
                    {
                        ProductCode = row["PRODUCT_CODE"].ToString(),
                        SapProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                        GenericStrength = row["GENERIC_STRENGTH"].ToString(),
                        PackSize = row["PACK_SIZE_NAME"].ToString(),
                        DosageForm = row["DOSAGE_FORM_NAME"].ToString(),
                        DarNo = row["DAR_NO"].ToString(),
                        BrandName = row["BRAND_NAME"].ToString(),
                        AnnexureNo = row["ANNEXURE_NO"].ToString(),
                        AnnexID = Convert.ToInt64(row["ANNEX_ID"]),
                        CompanyName = row["COMPANY_NAME"].ToString(),
                        CompanyCode = row["COMPANY_CODE"].ToString()

                    }).ToList();
            return item;
        }

        public bool SaveUpdate(AdvertisementInfoBEL master, string userId)
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
                    IUMode = "U";
                    ReturnMaxID = master.ID;
                    RefNo = master.RevisionNo;
                    query.Append(" UPDATE ADVERTISEMENT_INFO SET ");
                    query.Append(" COMPANY_CODE='" + master.CompanyCode + "',");
                    query.Append(" PROPOSAL_DATE= TO_DATE('" + master.ProposalDate + "','dd/MM/yyyy'),");
                    query.Append(" SUBMISSION_DATE= TO_DATE('" + master.SubmissionDate + "','dd/MM/yyyy'),");
                    query.Append(" APPROVAL_DATE= TO_DATE('" + master.ApprovalDate + "','dd/MM/yyyy'),");
                    query.Append(" VALID_UPTO= TO_DATE('" + master.ValiduptoDate + "','dd/MM/yyyy'),");
                    query.Append(" NOTIFICATION_DAYS='" + master.NotificationDay + "',");
                    query.Append(" REMARKS='" + master.Remarks + "'");
                    query.Append(" Where ID=" + master.ID);
                   
                }
                else
                {
                    //I for Insert 
                    ReturnMaxID = _idGenerated.getMAXSL("ADVERTISEMENT_INFO", "ID");
                    MaxID = _idGenerated.getMAXID("ADVERTISEMENT_INFO", "SLNO", "fm0000");

                    string strCount = _dbHelper.GetValue("SELECT COUNT(1) AS RevisionNo FROM ADVERTISEMENT_INFO  WHERE COMPANY_CODE='" + master.CompanyCode + "' AND PRODUCT_CODE='" + master.ProductCode + "' GROUP BY COMPANY_CODE,PRODUCT_CODE");
                    if (!string.IsNullOrEmpty(strCount))
                    {
                        RefNo = strCount;
                    }
                    else
                    {
                        RefNo = "0";
                    }
                    
                    IUMode = "I";
                    query.Append(" INSERT INTO ADVERTISEMENT_INFO(ID, SLNO, PRODUCT_CODE, COMPANY_CODE, PROPOSAL_DATE, SUBMISSION_DATE, APPROVAL_DATE, VALID_UPTO, NOTIFICATION_DAYS, REMARKS, SET_BY, SET_ON,REVISION_NO) ");
                    query.Append(" VALUES(" + ReturnMaxID + ",'" + MaxID + "','" + master.ProductCode + "','" + master.CompanyCode + "', TO_DATE('" + master.ProposalDate + "','dd/MM/yyyy')" + ", TO_DATE('" + master.SubmissionDate + "','dd/MM/yyyy')" + ",");
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
        public IList<AdvertisementInfoBEL> GetAdvertiseNotify(AdvertisementInfoBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT A.ID,A.SLNO,A.REVISION_NO,A.SUBMISSION_DATE,A.PROPOSAL_DATE,A.APPROVAL_DATE,A.COMPANY_CODE,A.COMPANY_NAME,");
            query.Append(" A.NOTIFICATION_DAYS,A.PRODUCT_CODE,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME,");
            query.Append(" A.DOSAGE_FORM_NAME,ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0)DateDiff,TO_CHAR(A.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO");
            query.Append(" FROM ( SELECT D.ID,D.SLNO,D.REVISION_NO,TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,D.COMPANY_CODE,C.COMPANY_NAME,");
            query.Append(" TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE, TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,");
            query.Append(" D.VALID_UPTO,D.NOTIFICATION_DAYS,  TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,P.PRODUCT_CODE,");
            query.Append(" P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME  FROM ADVERTISEMENT_INFO D ");
            query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE ");
            query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE ");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ) A ");
            query.Append(" INNER JOIN ( SELECT COMPANY_CODE,PRODUCT_CODE,MAX(REVISION_NO) AS MaxRvNo FROM ADVERTISEMENT_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) B");
            query.Append(" ON B.COMPANY_CODE=A.COMPANY_CODE AND B.PRODUCT_CODE=A.PRODUCT_CODE AND B.MaxRvNo=A.REVISION_NO  WHERE 1=1 ");
            
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND  A.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.ProductCode))
            {
                query.Append(" AND  A.PRODUCT_CODE='{1}'");
            }
            if (!string.IsNullOrEmpty(model.NotificationDay))
            {
                query.Append(" AND ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0) <= {2}");
            }
            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND A.VALID_UPTO BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            }
            if (string.IsNullOrEmpty(model.CompanyCode) && string.IsNullOrEmpty(model.ProductCode) && string.IsNullOrEmpty(model.NotificationDay) && string.IsNullOrEmpty(model.FromDate) && string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ");
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  A.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.ProductCode, model.NotificationDay));

            var item = (from DataRow row in dt.Rows
                        select new AdvertisementInfoBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SapProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenericStrength = row["GENERIC_CODE"].ToString(),
                            PackSize = row["PACK_SIZE_NAME"].ToString(),
                            DosageForm = row["DOSAGE_FORM_NAME"].ToString(),
                            //DarNo = row["DAR_NO"].ToString(),
                            //BrandName = row["BRAND_NAME"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            //AnnexureNo = row["ANNEXURE_NO"].ToString(),
                            ProposalDate = row["PROPOSAL_DATE"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ApprovalDate = row["APPROVAL_DATE"].ToString(),
                            ValiduptoDate = row["VALID_UPTO"].ToString(),
                            NotificationDay = row["NOTIFICATION_DAYS"].ToString(),
                            DateDiff = Convert.ToInt32(row["DateDiff"])
                        }).ToList();
            return item;
        }
    }
}