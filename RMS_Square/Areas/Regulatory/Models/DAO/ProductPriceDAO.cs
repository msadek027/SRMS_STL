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

    public class ProductPriceDAO : ReturnData
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;
        public ProductPriceDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }
        public bool SaveUpdate(ProductPriceBEL model, string userId)
        {
            try
            {
                var query = new StringBuilder();
                if (!string.IsNullOrEmpty(model.UnitPrice))
                {
                    model.UnitPrice = model.UnitPrice.Replace("'", "''").Trim();
                }
                if (model.ID > 0)
                {
                    //U for update
                    ReturnMaxID = model.ID;
                    MaxID = model.SlNo;
                    RefNo = model.RevisionNo;
                    IUMode = "U";
                    query.Append(" UPDATE PRODUCT_PRICE SET ANNEX_ID='" + model.AnnexId + "', REMARKS='" + model.PriceRemarks + "',PRICE_CATEGORY='" + model.PriceCategory + "',PRICE_CHANGE_STATUS='" + model.PriceChangeType + "',PRICE_PER_UNIT='" + model.UnitPrice + "',HIGHEST_PRICE='" + model.HighestPrice + "',");
                    query.Append(" PROPOSAL_DATE =(TO_DATE('" + model.PriceProposalDate + "','dd/MM/yyyy')), SUBMISSION_DATE =(TO_DATE('" + model.PriceSubmissionDate + "','dd/MM/yyyy')), LAUNCHING_DATE =(TO_DATE('" + model.LaunchingDate + "','dd/MM/yyyy')),");
                    query.Append(" RECEIVED_DATE =(TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')), APPROVAL_DATE =(TO_DATE('" + model.ApprovalDate + "','dd/MM/yyyy')), PROPOSED_BY ='" + model.PriceProposedBy + "',");
                    query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                    query.Append(" WHERE ID='" + model.ID + "'");
                }
                else
                { //I for Insert  
                    ReturnMaxID = _idGenerated.getMAXSL("PRODUCT_PRICE", "ID");
                    MaxID = _idGenerated.getMAXID("PRODUCT_PRICE", "SLNO", "fm000000000");
                    string strCount = _dbHelper.GetValue("SELECT COUNT(1) AS RevisionNo FROM PRODUCT_PRICE  WHERE IS_DELETE='N' AND ANNEX_ID='" + model.AnnexId + "' GROUP BY ANNEX_ID");
                    if (!string.IsNullOrEmpty(strCount))
                    {
                        RefNo = strCount;
                        model.PriceType = "Revised";
                    }
                    else
                    {
                        RefNo = "0";
                        model.PriceType = "New";
                    }
                    
                    IUMode = "I";
                    query.Append(" INSERT INTO PRODUCT_PRICE(ID,SLNO,ANNEX_ID,REMARKS,REVISION_NO,PRICE_TYPE,PRICE_CATEGORY,PRICE_CHANGE_STATUS,HIGHEST_PRICE,");
                    query.Append(" PROPOSAL_DATE,SUBMISSION_DATE,RECEIVED_DATE,APPROVAL_DATE,LAUNCHING_DATE, PROPOSED_BY,PRICE_PER_UNIT,SET_BY,SET_ON,IS_DELETE)");
                    query.Append(" VALUES( '" + ReturnMaxID + "','" + MaxID + "','" + model.AnnexId + "','" + model.PriceRemarks + "','" + RefNo + "','" + model.PriceType + "','" + model.PriceCategory + "','" + model.PriceChangeType + "','" + model.HighestPrice  +"',");
                    query.Append(" (TO_DATE('" + model.PriceProposalDate + "','dd/MM/yyyy')),(TO_DATE('" + model.PriceSubmissionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ApprovalDate + "','dd/MM/yyyy')), (TO_DATE('" + model.LaunchingDate + "','dd/MM/yyyy')),");
                    query.Append(" '" + model.PriceProposedBy + "','" + model.UnitPrice + "','" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss'))");
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

        public IList<ProductPriceBEL> GetAllInfo(ProductPriceBEL model, string orderBy)
        {
            var item = new List<ProductPriceBEL>();
            try
            {
                var query = new StringBuilder();
                query.Append(" SELECT D.ID,D.ANNEX_ID,PR.ANNEXURE_NO,D.REVISION_NO,D.REMARKS,D.PRICE_TYPE,D.PRICE_CATEGORY,D.PRICE_CHANGE_STATUS,D.PROPOSED_BY,D.PRICE_PER_UNIT,D.HIGHEST_PRICE,");
                query.Append(" TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(D.LAUNCHING_DATE, 'dd/mm/yyyy')LAUNCHING_DATE, ");
                query.Append(" TO_CHAR(D.RECEIVED_DATE, 'dd/mm/yyyy')RECEIVED_DATE,TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE,");
                query.Append(" TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,P.PRODUCT_CODE,PR.DAR_NO,C.COMPANY_CODE,C.COMPANY_NAME,C.LICENSE_NO,");
                query.Append(" P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.BRAND_NAME,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME");
                query.Append(" FROM PRODUCT_PRICE D");
                query.Append(" LEFT JOIN  PRODUCT_REGISTRATION_INFO PR ON PR.ANNEX_ID=D.ANNEX_ID");
                query.Append(" LEFT JOIN  RECIPE_INFO R ON R.ID=PR.RECIPE_ID");
                query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE");
                query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE");
                query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE");
                query.Append(" WHERE D.IS_DELETE <>'Y' ");
                query.Append(" AND P.STATUS = 'Active' ");

                if (!string.IsNullOrEmpty(model.CompanyCode))
                {
                    query.Append(" AND C.COMPANY_CODE='{0}'");
                }
                if (!string.IsNullOrEmpty(model.ProductCode))
                {
                    query.Append(" AND  P.PRODUCT_CODE='{1}'");
                }
                if (model.AnnexId > 0)
                {
                    query.Append(" AND D.ANNEX_ID='{2}'");
                }
                if (!string.IsNullOrEmpty(model.PriceType) && !model.PriceType.Equals("All"))
                {
                    query.Append(" AND D.PRICE_TYPE='{3}'");
                }
                if (!string.IsNullOrEmpty(model.PriceChangeType) && !model.PriceChangeType.Equals("All"))
                {
                    query.Append(" AND D.PRICE_CHANGE_STATUS='{4}'");
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
                            query.Append(" AND D.RECEIVED_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                    {
                        query.Append(" AND ( D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                        query.Append(" OR D.RECEIVED_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                        query.Append(" OR D.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                    }
                }
           
                //if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                //{
                //    query.Append(" AND TO_DATE(D.SUBMISSION_DATE,'dd/mm/yyyy') BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
                //}
                
                if (!string.IsNullOrEmpty(orderBy))
                {
                    query.Append(" ORDER BY  D.ID " + orderBy);
                }
                DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.ProductCode, model.AnnexId,model.PriceType,model.PriceChangeType));

                item = (from DataRow row in dt.Rows
                        select new ProductPriceBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            AnnexId = Convert.ToInt64(row["ANNEX_ID"]),
                            AnnexureNo = row["ANNEXURE_NO"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString(),
                            DarNo = row["DAR_NO"].ToString(),
                            PriceType = row["PRICE_TYPE"].ToString(),
                            HighestPrice = row["HIGHEST_PRICE"].ToString(),
                            PriceCategory = row["PRICE_CATEGORY"].ToString(),
                            PriceChangeType = row["PRICE_CHANGE_STATUS"].ToString(),
                            UnitPrice = row["PRICE_PER_UNIT"].ToString(),
                            PriceRemarks = row["REMARKS"].ToString(),
                            PriceProposalDate = row["PROPOSAL_DATE"].ToString(),
                            PriceSubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ReceivedDate = row["RECEIVED_DATE"].ToString(),
                            ApprovalDate = row["APPROVAL_DATE"].ToString(),
                            PriceProposedBy = row["PROPOSED_BY"].ToString(),
                            SetOn = row["SET_ON"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            LaunchingDate = row["LAUNCHING_DATE"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_CODE"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString()
                        }).ToList();
                return item;
            }
            catch 
            {
                return item;
            }

        }
        public IList<ProductRegistrationBEL> GetAllProductFromAnnex(string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT A.* FROM (");
            query.Append(" SELECT D.ANNEX_ID,D.ANNEXURE_NO,D.REVISION_NO,D.RECIPE_ID,D.DAR_NO,D.DTL_REMARKS,");
            query.Append(" TO_CHAR(D.DTL_RECEIVE_DATE, 'dd/mm/yyyy')DTL_RECEIVE_DATE,TO_CHAR(D.DTL_SUBMISSION_DATE, 'dd/mm/yyyy')DTL_SUBMISSION_DATE,");
            query.Append(" TO_CHAR(D.DTL_APPROVAL_DATE, 'dd/mm/yyyy')DTL_APPROVAL_DATE,TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,");
            query.Append(" TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE,TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,");
            query.Append(" TO_CHAR(D.INCLUSION_DATE, 'dd/mm/yyyy')INCLUSION_DATE,TO_CHAR(D.RENEWAL_DATE, 'dd/mm/yyyy')RENEWAL_DATE,");
            query.Append(" TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,D.REMARKS,D.PROPOSED_BY,D.ANNEXURE_STATUS,");
            query.Append(" D.NOTIFICATION_DAYS,TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,P.PRODUCT_CODE,P.PRODUCT_SPECIFICATION,");
            query.Append(" R.SLNO,C.COMPANY_CODE,C.COMPANY_NAME,C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PRODUCT_CATEGORY,P.BRAND_NAME,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME");
            query.Append(" FROM PRODUCT_REGISTRATION_INFO D");
            query.Append(" LEFT JOIN  RECIPE_INFO R ON R.ID=D.RECIPE_ID");
            query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE");
            query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE");
            query.Append(" WHERE D.IS_DELETE <>'Y' ");
            query.Append(" AND P.STATUS = 'Active' ");
            query.Append(" ) A INNER JOIN ( SELECT ANNEXURE_NO,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_REGISTRATION_INFO GROUP BY ANNEXURE_NO) B ");
            query.Append(" ON B.ANNEXURE_NO=A.ANNEXURE_NO AND B.MaxRvNo=A.REVISION_NO");
            
            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  A.ANNEX_ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString()));

            var item = (from DataRow row in dt.Rows
                        select new ProductRegistrationBEL
                        {
                            AnnexId = Convert.ToInt64(row["ANNEX_ID"]),
                            AnnexureNo = row["ANNEXURE_NO"].ToString(),
                            RecipeId = Convert.ToInt64(row["RECIPE_ID"]),
                            SlNo = row["SLNO"].ToString(), //=>RecipeNo
                            AnnexRevisionNo = row["REVISION_NO"].ToString(),
                            DarNo = row["DAR_NO"].ToString(),
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
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_CODE"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString()
                        }).ToList();
            return item;
        }
        public DataTable GetProductPriceForReport(ReportModel model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT ROW_NUMBER() OVER(ORDER BY A.ID)SN,A.ID, A.ANNEX_ID,A.ANNEXURE_NO,A.REVISION_NO,A.DAR_NO,A.PRICE_PER_UNIT,A.PRODUCT_CODE,A.PROPOSED_BY,A.REMARKS,");
            query.Append(" A.SUBMISSION_DATE,A.APPROVAL_DATE,A.COMPANY_CODE,A.COMPANY_NAME, A.BRAND_NAME,A.PRODUCT_CATEGORY,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME,A.DOSAGE_FORM_NAME ");
            query.Append(" FROM (SELECT D.ID,D.ANNEX_ID,PR.ANNEXURE_NO,D.PRICE_TYPE,D.PRICE_CHANGE_STATUS,D.PRICE_CATEGORY,D.PRICE_PER_UNIT,P.PRODUCT_CODE,D.REVISION_NO,D.REMARKS,PR.DAR_NO,D.PROPOSED_BY,C.COMPANY_CODE,C.COMPANY_NAME,C.LICENSE_NO,D.APPROVAL_DATE,D.SUBMISSION_DATE,P.BRAND_NAME,P.PRODUCT_CATEGORY,");
            query.Append(" P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME  FROM PRODUCT_PRICE D INNER JOIN  PRODUCT_REGISTRATION_INFO PR ON PR.ANNEX_ID=D.ANNEX_ID ");
            query.Append(" INNER JOIN  RECIPE_INFO R ON R.ID=PR.RECIPE_ID INNER JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE  INNER JOIN  COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE ");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE  WHERE 1=1 ");
            query.Append(" AND P.STATUS = 'Active' ");
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND P.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.BrandName))
            {
                query.Append(" AND TRIM(P.BRAND_NAME)='{1}'");
                model.BrandName = model.BrandName.Trim();
            }
            if (!string.IsNullOrEmpty(model.PriceChangeType) && !model.PriceChangeType.Equals("All"))
            {
                query.Append(" AND D.PRICE_CHANGE_STATUS='{2}'");
            }
            if (!string.IsNullOrEmpty(model.PriceType) && !model.PriceType.Equals("All"))
            {
                query.Append(" AND D.PRICE_TYPE='{3}'");
            }
            if (!string.IsNullOrEmpty(model.PriceCategory) && !model.PriceCategory.Equals("All"))
            {
                query.Append(" AND D.PRICE_CATEGORY='{4}'");
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
                        query.Append(" AND D.RECEIVED_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                {
                    query.Append(" AND ( D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR D.RECEIVED_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR D.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                }
            }
           
            query.Append(" ) A INNER JOIN ( SELECT ANNEX_ID,MAX(REVISION_NO) AS MaxRvNo FROM PRODUCT_PRICE GROUP BY ANNEX_ID) B ON B.ANNEX_ID=A.ANNEX_ID  AND B.MaxRvNo=A.REVISION_NO");
            
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.BrandName,model.PriceChangeType,model.PriceType,model.PriceCategory));

            return dt;
        }

        public DataTable GetFileRefno(int p, string refLevel1)
        {
            var query = new StringBuilder();
            query.Append(" SELECT filetype,BL,I,IHA,J FROM ");
            query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
            query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Block List' BL,'Invoice' I,'In House Approval' IHA, 'Justification' J))");

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), p, refLevel1));
            return dt;
        }

        public void UpdateFileRelatedInfo(ProductPriceBEL model, string userId)
        {
            var query = new StringBuilder();
            if (model.ID > 0)
            {
                //U for update
                ReturnMaxID = model.ID;
                query.Append(" UPDATE PRODUCT_PRICE SET RECEIVE_DATE =(TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')), ");
                query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                query.Append(" WHERE ID='" + model.ID + "'");
            }
            _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());
        }
    }
}