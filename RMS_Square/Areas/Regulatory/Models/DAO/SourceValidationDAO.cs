using RMS_Square.DAL.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.Universal.Gateway;

namespace RMS_Square.Areas.Regulatory.Models.DAO
{
    public class SourceValidationDAO : ReturnData
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;
        public SourceValidationDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }
        public bool SaveUpdate(SourceValidationBEL model, string userId)
        {
            try
            {
                var query = new StringBuilder();
                if (model.AnnexId > 0)
                {
                    //U for update
                    ReturnMaxID = model.AnnexId;
                    MaxID = model.AnnexureNo;
                    RefNo = model.AnnexRevisionNo;
                    IUMode = "U";
                    query.Append(" UPDATE SOURCE_VALIDATION_INFO SET RECIPE_ID='" + model.RecipeId + "', DAR_NO='" + model.DarNo + "',");
                    query.Append(" RECEIVE_DATE =(TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')), SUBMISSION_DATE =(TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')),");
                    query.Append(" RENEWAL_DATE =(TO_DATE('" + model.RenewalDate + "','dd/MM/yyyy')), VALID_UPTO =(TO_DATE('" + model.ValidUptoDate + "','dd/MM/yyyy')),");
                    //query.Append(" RECEIVE_DATE =(TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')), SUBMISSION_DATE =(TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')),");
                    //query.Append(" INCLUSION_DATE =(TO_DATE('" + model.InclusionDate + "','dd/MM/yyyy')), VALID_UPTO =(TO_DATE('" + model.ValidUptoDate + "','dd/MM/yyyy')),");
                    query.Append(" STATE_STATUS ='" + model.StateStatus + "', GENERIC_NAME= '" + model.GenAndStrength + "', PRODUCT_SPECIFICATION= '"+model.ProductSpecification+"',");
                    query.Append(" ANNEXURE_STATUS='" + model.AnnexStatus + "', REMARKS='" + model.Remarks + "',");
                    query.Append(" COUNTRY='" + model.CountryOfOrigin + "', SOURCE_ADDRESS='" + model.ManufacturerName + "',");
                    query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                    query.Append(" WHERE ANNEX_ID='" + model.AnnexId + "'");
                }
                else
                { //I for Insert  
                    ReturnMaxID = _idGenerated.getMAXSL("SOURCE_VALIDATION_INFO", "ANNEX_ID");
                    // company & productcode wise group needed
                    string strCount = _dbHelper.GetValue("SELECT COUNT(1) AS RevisionNo FROM SOURCE_VALIDATION_INFO  WHERE IS_DELETE='N' AND RECIPE_ID='" + model.RecipeId + "'  GROUP BY RECIPE_ID");
                    if (!string.IsNullOrEmpty(strCount))
                    {
                        RefNo = strCount;
                        if (!string.IsNullOrEmpty(model.AnnexRevisionNo))
                        {
                            MaxID = model.AnnexRevisionNo;
                        }
                        else
                        {
                            MaxID = _dbHelper.GetValue("SELECT MAX(ANNEXURE_NO)ANNEXURE_NO FROM SOURCE_VALIDATION_INFO  WHERE IS_DELETE='N' AND RECIPE_ID='" + model.RecipeId + "'  GROUP BY RECIPE_ID");
                            // MaxID = _idGenerated.getMAXID("PRODUCT_REGISTRATION_INFO", "ANNEXURE_NO", "fm000000000");
                        }
                    }
                    else
                    {
                        RefNo = "0";
                        //MaxID = _idGenerated.getMAXID("SOURCE_VALIDATION_INFO", "ANNEXURE_NO", "fm000000000");
                        MaxID = _idGenerated.getMAXID("SOURCE_VALIDATION_INFO", "REVISION_NO", "fm000000000");
                        model.StateStatus = model.StateStatus ?? "New";
                    }


                    //IUMode = "I";
                    //query.Append(" INSERT INTO PRODUCT_REGISTRATION_INFO(ANNEX_ID,ANNEXURE_NO,REVISION_NO,RECIPE_ID,DAR_NO,DTL_REMARKS,STATE_STATUS,");
                    //query.Append(" DTL_RECEIVE_DATE,DTL_SUBMISSION_DATE,DTL_APPROVAL_DATE,PROPOSAL_DATE,RECEIVE_DATE,");
                    //query.Append(" SUBMISSION_DATE,INCLUSION_DATE,RENEWAL_DATE,VALID_UPTO,REMARKS,ANNEXURE_STATUS,");
                    //query.Append(" PROPOSED_BY,NOTIFICATION_DAYS,SET_BY,SET_ON,IS_DELETE) ");
                    //query.Append(" VALUES( '" + ReturnMaxID + "','" + MaxID + "','" + RefNo + "','" + model.RecipeId + "','" + model.DarNo + "','" + model.DtlRemarks + "','" + model.StateStatus + "',");
                    //query.Append(" (TO_DATE('" + model.DtlReceivedDate + "','dd/MM/yyyy')),(TO_DATE('" + model.DtlSubmissionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.DtlApprovalDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')),");
                    //query.Append(" (TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')),(TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.InclusionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.RenewalDate + "','dd/MM/yyyy')),");
                    //query.Append(" (TO_DATE('" + model.ValidUptoDate + "','dd/MM/yyyy')),'" + model.Remarks + "','" + model.AnnexStatus + "',");
                    //query.Append(" '" + model.ProposedBy + "','" + model.AlarmDays + "','" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss'))");
                    //query.Append(" ,'N')");
                    IUMode = "I";
                    //query.Append(" INSERT INTO SOURCE_VALIDATION_INFO(ANNEX_ID,REVISION_NO,RECIPE_ID,STATE_STATUS,");
                    query.Append(" INSERT INTO SOURCE_VALIDATION_INFO(ANNEX_ID,ANNEXURE_NO,REVISION_NO,RECIPE_ID,STATE_STATUS,");
                    query.Append(" GENERIC_NAME,PRODUCT_SPECIFICATION,COUNTRY,SOURCE_ADDRESS,COMPANY_NAME,");
                    query.Append(" RECEIVE_DATE,SUBMISSION_DATE,RENEWAL_DATE,VALID_UPTO,REMARKS,ANNEXURE_STATUS,");
                    query.Append(" SET_BY,SET_ON,IS_DELETE) ");
                    //query.Append(" VALUES( '" + ReturnMaxID + "','" + MaxID + "','" + RefNo + "','" + model.RecipeId + "','" + model.StateStatus + "',");
                    query.Append(" VALUES( '" + ReturnMaxID + "','" + MaxID + "','" + MaxID + "','" + model.RecipeId + "','" + model.StateStatus + "',");
                    query.Append("  '" + model.GenAndStrength + "','" + model.ProductSpecification + "','" + model.CountryOfOrigin + "','" + model.ManufacturerName + "','" + model.CompanyName + "',");
                    //query.Append(" (TO_DATE('" + model.DtlReceivedDate + "','dd/MM/yyyy')),(TO_DATE('" + model.DtlSubmissionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.DtlApprovalDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')),");
                    query.Append(" (TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')),(TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.RenewalDate + "','dd/MM/yyyy')),");
                    query.Append(" (TO_DATE('" + model.ValidUptoDate + "','dd/MM/yyyy')),'" + model.Remarks + "','" + model.AnnexStatus + "',");
                    query.Append(" '" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss'))");
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

        public DataTable GetFileRefno(int p, string refLevel1, string sStatus)
        {
            var query = new StringBuilder();
            //if (sStatus.Equals("New"))
            //{
                query.Append(" SELECT filetype,PA,PPM,SD FROM ");
                query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
                query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Proposed Annexure' PA,'Proposed Packaging Material' PPM,'Stability Data' SD))");
            //}
            //else if (sStatus.Equals("Renew"))
            //{
            //    query.Append(" SELECT filetype,PA,PPM FROM ");
            //    query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
            //    query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Proposed Annexure' PA,'Proposed Packaging Material' PPM))");
            //}
            //else if (sStatus.Equals("Annexure Amendment"))
            //{
            //    query.Append(" SELECT filetype,PA,AJ,SD FROM ");
            //    query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
            //    query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Proposed Annexure' PA,'Amendment Justification' AJ,'Stability Data' SD))");
            //}
            //else if (sStatus.Equals("Packaging Amendment"))
            //{
            //    query.Append(" SELECT filetype,PPM,AJ FROM ");
            //    query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
            //    query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Proposed Packaging Material' PPM,'Amendment Justification' AJ))");
            //}

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), p, refLevel1));
            return dt;
        }


        public void UpdateFileRelatedInfo(SourceValidationBEL model, string userId)
        {
            var query = new StringBuilder();
            if (model.AnnexId > 0)
            {
                //U for update
                ReturnMaxID = model.AnnexId;
                MaxID = model.SlNo;
                RefNo = model.AnnexRevisionNo;
                IUMode = "U";
                query.Append(" UPDATE SOURCE_VALIDATION_INFO SET RECEIVE_DATE =(TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')), ");
                query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                query.Append(" WHERE ANNEX_ID='" + model.AnnexId + "'");
            }
            _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());
        }

        public IList<SourceValidationBEL> GetAllInfo(SourceValidationBEL model, string orderBy)
        {
            var query = new StringBuilder();
            //query.Append(" SELECT D.ANNEX_ID,D.ANNEXURE_NO,D.REVISION_NO,D.RECIPE_ID,D.STATE_STATUS,");
            //// query.Append(" TO_CHAR(D.DTL_RECEIVE_DATE, 'dd/mm/yyyy')DTL_RECEIVE_DATE,TO_CHAR(D.DTL_SUBMISSION_DATE, 'dd/mm/yyyy')DTL_SUBMISSION_DATE,");
            //// query.Append(" TO_CHAR(D.DTL_APPROVAL_DATE, 'dd/mm/yyyy')DTL_APPROVAL_DATE,TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,");
            //query.Append(" TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE,TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,");
            //query.Append(" TO_CHAR(D.RENEWAL_DATE, 'dd/mm/yyyy')RENEWAL_DATE,");
            //query.Append(" TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,D.REMARKS,D.ANNEXURE_STATUS,");
            //query.Append(" TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,P.PRODUCT_CODE,P.PRODUCT_SPECIFICATION,R.COUNTRY_CODE,R.MANUFACTURER_NAME, ");
            //query.Append(" R.SLNO,C.COMPANY_CODE,C.COMPANY_NAME,C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PRODUCT_CATEGORY,P.BRAND_NAME,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME");
            //query.Append(" FROM SOURCE_VALIDATION_INFO D");
            //query.Append(" LEFT JOIN  RECIPE_INFO R ON R.ID=D.RECIPE_ID");
            //query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE");
            //query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE");
            //query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE");
            //query.Append(" WHERE D.IS_DELETE <>'Y'");

            query.Append(" Select ANNEX_ID,ANNEXURE_NO,RECIPE_ID,TO_CHAR(SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(RENEWAL_DATE, 'dd/mm/yyyy')RENEWAL_DATE,");
            query.Append(" TO_CHAR(VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO, TO_CHAR(RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE,REMARKS,SET_BY,SET_ON,IS_DELETE,ANNEXURE_STATUS,");
            query.Append(" STATE_STATUS,GENERIC_NAME,PRODUCT_SPECIFICATION,COUNTRY,SOURCE_ADDRESS,COMPANY_NAME");
            query.Append(" From SOURCE_VALIDATION_INFO WHERE IS_DELETE <>'Y'");

            //if (!string.IsNullOrEmpty(model.CompanyCode))
            //{
            //    query.Append(" AND  C.COMPANY='{0}'");
            //}
            //if (!string.IsNullOrEmpty(model.ProductCode))
            //{
            //    query.Append(" AND  P.PRODUCT_CODE='{1}'");
            //}
            //if (model.ChooseOption != "All")
            //{
            //    if (model.ChooseOption == "SubmissionDate")
            //    {
            //        if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            //        {
            //            query.Append(" AND D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            //        }
            //    }
            //    else if (model.ChooseOption == "ApprovalDate")
            //    {
            //        if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            //        {
            //            query.Append(" AND D.RENEWAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            //        }
            //    }
            //    else
            //    {
            //        if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            //        {
            //            query.Append(" AND D.RECEIVE_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            //        }
            //    }
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            //    {
            //        query.Append(" AND ( D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            //        query.Append(" OR D.RECEIVE_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            //        query.Append(" OR D.RENEWAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
            //    }
            //}
            //if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            //{
            //    query.Append(" AND TO_DATE(D.SUBMISSION_DATE,'dd/mm/yyyy') BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
            //}
            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  ANNEX_ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.ProductCode));

            var item = (from DataRow row in dt.Rows
                        select new SourceValidationBEL
                        {
                            AnnexId = Convert.ToInt64(row["ANNEX_ID"]),
                            AnnexureNo = row["ANNEXURE_NO"].ToString(),
                            RecipeId = Convert.ToInt64(row["RECIPE_ID"]),
                            //SlNo = row["SLNO"].ToString(), //=>RecipeNo
                            AnnexRevisionNo = row["ANNEXURE_NO"].ToString(),
                            //DarNo = row["DAR_NO"].ToString(),
                            StateStatus = row["STATE_STATUS"].ToString(),
                            //DtlRemarks = row["DTL_REMARKS"].ToString(),
                            //DtlReceivedDate = row["DTL_RECEIVE_DATE"].ToString(),
                            //DtlSubmissionDate = row["DTL_SUBMISSION_DATE"].ToString(),
                            //DtlApprovalDate = row["DTL_APPROVAL_DATE"].ToString(),
                            //ProposalDate = row["PROPOSAL_DATE"].ToString(),
                            ReceivedDate = row["RECEIVE_DATE"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            //InclusionDate = row["INCLUSION_DATE"].ToString(),
                            RenewalDate = row["RENEWAL_DATE"].ToString(),
                            ValidUptoDate = row["VALID_UPTO"].ToString(),
                            Remarks = row["REMARKS"].ToString(),
                            //ProposedBy = row["PROPOSED_BY"].ToString(),
                            AnnexStatus = row["ANNEXURE_STATUS"].ToString(),
                            //AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                            SetOn = row["SET_ON"].ToString(),
                            //CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            //LicenseNo = row["LICENSE_NO"].ToString(),
                            //ProductCode = row["PRODUCT_CODE"].ToString(),
                            //SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_NAME"].ToString(),
                            //PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            //DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            //ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            //BrandName = row["BRAND_NAME"].ToString(),
                            ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString(),
                            CountryOfOrigin = row["COUNTRY"].ToString(),
                            ManufacturerName = row["SOURCE_ADDRESS"].ToString(),

                        }).ToList();
            return item;
        }

        public DataTable GetSourceValidationForReport(ReportModel model, string asc)
        {
            //var query = new StringBuilder();
            //query.Append(" SELECT ROW_NUMBER() OVER(ORDER BY A.ANNEX_ID)SN,A.RECIPE_ID, A.ANNEX_ID,A.ANNEXURE_NO,A.REVISION_NO,A.DAR_NO,A.PRODUCT_CODE,A.PRODUCT_CATEGORY,A.PROPOSED_BY,");
            //query.Append(" A.DTL_RECEIVE_DATE,A.DTL_SUBMISSION_DATE,A.DTL_APPROVAL_DATE,A.PROPOSAL_DATE, A.DTL_REMARKS,A.RECEIVE_DATE,A.LICENSE_NO,A.RENEWAL_DATE,");
            //query.Append(" A.SUBMISSION_DATE,A.INCLUSION_DATE,A.COMPANY_CODE,A.COMPANY_NAME, A.BRAND_NAME,A.PRODUCT_SPECIFICATION,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME,A.DOSAGE_FORM_NAME,A.VALID_UPTO, A.AMEND_APV_DATE, A.REMARKS");
            //query.Append(" FROM (SELECT D.ANNEX_ID,D.ANNEXURE_NO,D.RECIPE_ID,P.PRODUCT_CODE,D.REVISION_NO,D.DAR_NO,D.PROPOSED_BY,C.COMPANY_CODE,C.COMPANY_NAME,C.LICENSE_NO,TO_CHAR(D.INCLUSION_DATE, 'dd/mm/yyyy')INCLUSION_DATE,TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,P.BRAND_NAME,P.PRODUCT_SPECIFICATION,P.PRODUCT_CATEGORY, D.AMEND_APV_DATE, D.REMARKS, D.DTL_REMARKS ,");
            //query.Append(" TO_CHAR(D.DTL_RECEIVE_DATE, 'dd/mm/yyyy')DTL_RECEIVE_DATE,TO_CHAR(D.DTL_SUBMISSION_DATE, 'dd/mm/yyyy')DTL_SUBMISSION_DATE,TO_CHAR(D.DTL_APPROVAL_DATE, 'dd/mm/yyyy')DTL_APPROVAL_DATE,TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE, TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE,TO_CHAR(D.RENEWAL_DATE, 'dd/mm/yyyy')RENEWAL_DATE,");
            //query.Append(" P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME  FROM PRODUCT_REGISTRATION_INFO D INNER JOIN  RECIPE_INFO R ON R.ID=D.RECIPE_ID ");
            //query.Append(" INNER JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE  INNER JOIN  COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE ");
            //query.Append(" INNER JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE  WHERE 1=1 ");
            var query = new StringBuilder();
            query.Append(" SELECT ROW_NUMBER() OVER(ORDER BY A.ANNEX_ID)SN,A.RECIPE_ID, A.ANNEX_ID,A.ANNEXURE_NO,A.REVISION_NO,A.DAR_NO,A.PRODUCT_CODE,A.PRODUCT_CATEGORY,");
            query.Append(" A.PROPOSAL_DATE,A.RECEIVE_DATE,A.LICENSE_NO,A.RENEWAL_DATE,");
            query.Append(" A.SUBMISSION_DATE,A.COMPANY_CODE,A.COMPANY_NAME, A.BRAND_NAME,A.PRODUCT_SPECIFICATION,A.COUNTRY_CODE,A.MANUFACTURER_NAME,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME,A.DOSAGE_FORM_NAME,A.VALID_UPTO, A.AMEND_APV_DATE, A.REMARKS");
            query.Append(" FROM (SELECT D.ANNEX_ID,D.ANNEXURE_NO,D.RECIPE_ID,P.PRODUCT_CODE,D.REVISION_NO,D.DAR_NO,C.COMPANY_CODE,C.COMPANY_NAME,R.COUNTRY_CODE,R.MANUFACTURER_NAME,C.LICENSE_NO,TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,P.BRAND_NAME,P.PRODUCT_SPECIFICATION,P.PRODUCT_CATEGORY, D.AMEND_APV_DATE, D.REMARKS ,");
            query.Append(" TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE, TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE,TO_CHAR(D.RENEWAL_DATE, 'dd/mm/yyyy')RENEWAL_DATE,");
            query.Append(" P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME  FROM SOURCE_VALIDATION_INFO D INNER JOIN  RECIPE_INFO R ON R.ID=D.RECIPE_ID ");
            query.Append(" INNER JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=R.PRODUCT_CODE  INNER JOIN  COMPANY_INFO C ON C.COMPANY_CODE=R.COMPANY_CODE ");
            query.Append(" INNER JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE  WHERE 1=1 ");
            if (!string.IsNullOrEmpty(model.StateStatus))
            {
                query.Append(" AND D.STATE_STATUS='" + model.StateStatus + "' ");
            }
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND P.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.BrandName))
            {
                query.Append(" AND TRIM(P.BRAND_NAME)='{1}'");
                model.BrandName = model.BrandName.Trim();
            }
            if (model.ProductSpecification != null && model.ProductSpecification != "All")
            {
                query.Append(" AND P.PRODUCT_SPECIFICATION='{2}'");
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
                        query.Append(" AND D.RENEWAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
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
                    query.Append(" OR D.RENEWAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                }
            }
            query.Append(" ) A INNER JOIN ( SELECT RECIPE_ID,MAX(REVISION_NO) AS MaxRvNo FROM SOURCE_VALIDATION_INFO GROUP BY RECIPE_ID) B ON B.RECIPE_ID=A.RECIPE_ID  AND B.MaxRvNo=A.REVISION_NO");

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.BrandName, model.ProductSpecification));

            return dt;
        }

        public IList<RecipeBEL> GetCompanyInfo(RecipeBEL recipeBEL, string orderBy)
        {
            var query = new StringBuilder();
            //query.Append(" SELECT A.* FROM (");
            //query.Append(" SELECT D.ID, D.SLNO,D.REVISION_NO,D.COMPANY_CODE,D.PRODUCT_CODE,D.MEETING_TYPE,D.MANUFACTURING_TYPE,D.DCC_NO,TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE,");
            //query.Append(" TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,TO_CHAR(D.IMPORT_PROPOSAL_DATE, 'dd/mm/yyyy')IMPORT_PROPOSAL_DATE,TO_CHAR(D.IMPORT_SUBMISSION_DATE, 'dd/mm/yyyy')IMPORT_SUBMISSION_DATE,");
            //query.Append(" TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(D.MEETING_DATE, 'dd/mm/yyyy')MEETING_DATE,TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE,");
            //query.Append(" TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,TO_CHAR(D.DATE_OF_EXTENSION, 'dd/mm/yyyy')DATE_OF_EXTENSION,TO_CHAR(D.INSPECTION_DATE, 'dd/mm/yyyy')INSPECTION_DATE,");
            //query.Append(" D.MEETING_REMARKS,D.COUNTRY_CODE,D.MANUFACTURER_NAME,D.IMPORT_REMARKS,D.APPROVAL_STATUS,D.REMARKS,D.INSPECTION_REMARKS,D.INSPECTION_STATUS,D.PROPOSED_BY,");
            //query.Append(" D.NOTIFICATION_DAYS,TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON, P.PRODUCT_CATEGORY,P.BRAND_NAME,P.PRODUCT_SPECIFICATION,");
            //query.Append(" C.COMPANY_NAME,C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.INTRODUCED_BANGLADESH,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME");
            //query.Append(" FROM RECIPE_INFO D");
            //query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE");
            //query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE");
            //query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE");
            //query.Append(" WHERE D.IS_DELETE <>'Y'");
            //query.Append("SELECT * FROM COMPANY_INFO");

            //if (!string.IsNullOrEmpty(model.CompanyCode))
            //{
            //    query.Append(" AND D.COMPANY_CODE='{0}'");
            //}
            //if (!string.IsNullOrEmpty(model.ProductCode))
            //{
            //    query.Append(" AND D.PRODUCT_CODE='{1}'");
            //}
            //if (!string.IsNullOrEmpty(model.MeetingType) && !model.MeetingType.Equals("All"))
            //{
            //    query.Append(" AND D.MEETING_TYPE='{2}'");
            //}
            //if (!string.IsNullOrEmpty(model.ManufacturerType) && !model.ManufacturerType.Equals("All"))
            //{
            //    query.Append(" AND D.MANUFACTURING_TYPE='{3}'");
            //}
            //if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            //{
            //    query.Append(" AND TO_DATE(D.SUBMISSION_DATE,'dd/mm/yyyy') BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
            //}
            //query.Append("  )A INNER JOIN ( SELECT PRODUCT_CODE,COMPANY_CODE,MAX(REVISION_NO) AS MaxRvNo FROM RECIPE_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) B ON B.PRODUCT_CODE=A.PRODUCT_CODE AND B.COMPANY_CODE=A.COMPANY_CODE AND B.MaxRvNo=A.REVISION_NO");
            //query.Append(" LEFT JOIN PRODUCT_REGISTRATION_INFO PR ON PR.RECIPE_ID=A.ID WHERE PR.RECIPE_ID is null");
            //if (!string.IsNullOrEmpty(orderBy))
            //{
            //    query.Append(" ORDER BY  ID " + orderBy);
            //}
             DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString()));

            var item = (from DataRow row in dt.Rows
                        select new RecipeBEL
                        {
                            //ID = Convert.ToInt64(row["ID"]),
                            //RecipeId = Convert.ToInt64(row["ID"]),
                            //SlNo = row["SLNO"].ToString(),
                            //RevisionNo = row["REVISION_NO"].ToString(),
                            //CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = "Square Astra Ltd.",
                            //LicenseNo = row["LICENSE_NO"].ToString(),
                            //ProductCode = row["PRODUCT_CODE"].ToString(),
                            //SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            //GenAndStrength = row["GENERIC_CODE"].ToString(),
                            //PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            //DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            //IntroducedInBD = row["INTRODUCED_BANGLADESH"].ToString(),
                            //MeetingType = row["MEETING_TYPE"].ToString(),
                            //ManufacturerType = row["MANUFACTURING_TYPE"].ToString(),
                            //DccNo = row["DCC_NO"].ToString(),
                            //ReceivedDate = row["RECEIVE_DATE"].ToString(),
                            //ProposalDate = row["PROPOSAL_DATE"].ToString(),
                            //ImportProposalDate = row["IMPORT_PROPOSAL_DATE"].ToString(),
                            //ImportSubmissionDate = row["IMPORT_SUBMISSION_DATE"].ToString(),
                            //ApvSubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            //ApvMeetingDate = row["MEETING_DATE"].ToString(),
                            //ApvApprovalDate = row["APPROVAL_DATE"].ToString(),
                            //ApvValidUptoDate = row["VALID_UPTO"].ToString(),
                            //ApvDateOfExtension = row["DATE_OF_EXTENSION"].ToString(),
                            //InspectionDate = row["INSPECTION_DATE"].ToString(),
                            //MeetingRemarks = row["MEETING_REMARKS"].ToString(),
                            //CountryOfOrigin = row["COUNTRY_CODE"].ToString(),
                            //ManufacturerName = row["MANUFACTURER_NAME"].ToString(),
                            //ImportRemarks = row["IMPORT_REMARKS"].ToString(),
                            //ApprovalStatus = row["APPROVAL_STATUS"].ToString(),
                            //ApvRemarks = row["REMARKS"].ToString(),
                            //InspectionRemarks = row["INSPECTION_REMARKS"].ToString(),
                            //ProposedBy = row["PROPOSED_BY"].ToString(),
                            //// Ins = row["INSPECTION_STATUS"].ToString(),
                            //AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                            //SetOn = row["SET_ON"].ToString(),
                            //ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            //BrandName = row["BRAND_NAME"].ToString(),
                            //ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString()
                        }).ToList();
            return item;
        }
    }
}