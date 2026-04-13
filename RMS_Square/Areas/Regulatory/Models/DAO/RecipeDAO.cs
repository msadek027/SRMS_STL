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

    public class RecipeDAO : ReturnData
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;
        public RecipeDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }
        public void UpdateFileRelatedInfo(RecipeBEL model, string userId)
        {
            var query = new StringBuilder();
            if (model.ID > 0)
            {
                //U for update
                ReturnMaxID = model.ID;
                MaxID = model.SlNo;
                RefNo = model.RevisionNo;
                IUMode = "U";
                query.Append(" UPDATE RECIPE_INFO SET RECEIVE_DATE =(TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')), ");
                query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                query.Append(" WHERE ID='" + model.ID + "'");
            }
            _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());
        }
        public bool SaveUpdate(RecipeBEL model, string userId)
        {
            try
            {
                var query = new StringBuilder();
                if (!string.IsNullOrEmpty(model.ApvApprovalDate))
                {
                    model.ApprovalStatus = "Approved";
                }
                else
                {
                    model.ApprovalStatus = "Not Approved";
                }
                if (model.ID > 0)
                {
                    //U for update
                    ReturnMaxID = model.ID;
                    //MaxID = _dbHelper.GetValue("SELECT MAX(SLNO)SLNO FROM RECIPE_INFO  WHERE IS_DELETE='N'  AND COMPANY_CODE='" + model.CompanyCode + "' AND PRODUCT_CODE='" + model.ProductCode + "'  GROUP BY COMPANY_CODE,PRODUCT_CODE");
                    MaxID = model.SlNo;
                    RefNo = model.RevisionNo;
                    IUMode = "U";
                   
                    query.Append(" UPDATE RECIPE_INFO SET COMPANY_CODE='" + model.CompanyCode + "', PRODUCT_CODE='" + model.ProductCode + "', MEETING_TYPE='" + model.MeetingType + "',");
                    query.Append(" DCC_NO='" + model.DccNo + "', MANUFACTURING_TYPE='" + model.ManufacturerType + "',MEETING_REMARKS='" + model.MeetingRemarks + "',");
                    query.Append(" RECEIVE_DATE =(TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')), PROPOSAL_DATE =(TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')),");
                    query.Append(" IMPORT_PROPOSAL_DATE =(TO_DATE('" + model.ImportProposalDate + "','dd/MM/yyyy')), IMPORT_SUBMISSION_DATE =(TO_DATE('" + model.ImportSubmissionDate + "','dd/MM/yyyy')),");
                    query.Append(" SUBMISSION_DATE =(TO_DATE('" + model.ApvSubmissionDate + "','dd/MM/yyyy')), MEETING_DATE =(TO_DATE('" + model.ApvMeetingDate + "','dd/MM/yyyy')),");
                    query.Append(" APPROVAL_DATE =(TO_DATE('" + model.ApvApprovalDate + "','dd/MM/yyyy')), VALID_UPTO =(TO_DATE('" + model.ApvValidUptoDate + "','dd/MM/yyyy')),");
                    query.Append(" DATE_OF_EXTENSION =(TO_DATE('" + model.ApvDateOfExtension + "','dd/MM/yyyy')), INSPECTION_DATE =(TO_DATE('" + model.InspectionDate + "','dd/MM/yyyy')),");
                    query.Append(" COUNTRY_CODE='" + model.CountryOfOrigin + "', MANUFACTURER_NAME='" + model.ManufacturerName + "',IMPORT_REMARKS='" + model.MeetingRemarks + "',");
                    query.Append(" APPROVAL_STATUS='" + model.ApprovalStatus + "', REMARKS='" + model.ApvRemarks + "',INSPECTION_REMARKS='" + model.InspectionRemarks + "',");
                    query.Append(" NOTIFICATION_DAYS='" + model.AlarmDays + "', PROPOSED_BY='" + model.ProposedBy + "',");
                    query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                    query.Append(" WHERE ID='" + model.ID + "'");
                }
                else
                { //I for Insert  
                    ReturnMaxID = _idGenerated.getMAXSL("RECIPE_INFO", "ID");
                    string strCount = _dbHelper.GetValue("SELECT COUNT(1) AS RevisionNo FROM RECIPE_INFO  WHERE IS_DELETE='N' AND COMPANY_CODE='" + model.CompanyCode + "' AND PRODUCT_CODE='" + model.ProductCode + "'  GROUP BY COMPANY_CODE,PRODUCT_CODE");
                    if (!string.IsNullOrEmpty(strCount))
                    {
                        RefNo = strCount;
                        if (!string.IsNullOrEmpty(model.SlNo))
                        {
                            MaxID = model.SlNo;
                        }
                        else
                        {
                            MaxID = _dbHelper.GetValue("SELECT MAX(SLNO)SLNO FROM RECIPE_INFO  WHERE IS_DELETE='N' AND COMPANY_CODE='" + model.CompanyCode + "' AND PRODUCT_CODE='" + model.ProductCode + "'  GROUP BY COMPANY_CODE,PRODUCT_CODE");
                        }
                    }
                    else
                    {
                        RefNo = "0";
                        MaxID = _idGenerated.getMAXID("RECIPE_INFO", "SLNO", "fm000000000");
                    }
                    IUMode = "I";
                    query.Append(" INSERT INTO RECIPE_INFO(ID,SLNO,REVISION_NO,COMPANY_CODE,PRODUCT_CODE,MEETING_TYPE,MANUFACTURING_TYPE,DCC_NO,RECEIVE_DATE,PROPOSAL_DATE,IMPORT_PROPOSAL_DATE,IMPORT_SUBMISSION_DATE, ");
                    query.Append(" SUBMISSION_DATE,MEETING_DATE,APPROVAL_DATE,VALID_UPTO,DATE_OF_EXTENSION,INSPECTION_DATE,MEETING_REMARKS,COUNTRY_CODE,MANUFACTURER_NAME,IMPORT_REMARKS,APPROVAL_STATUS,REMARKS, ");
                    query.Append(" INSPECTION_REMARKS,PROPOSED_BY,NOTIFICATION_DAYS,SET_BY,SET_ON,IS_DELETE) ");
                    query.Append(" VALUES( '" + ReturnMaxID + "','" + MaxID + "','" + RefNo + "','" + model.CompanyCode + "','" + model.ProductCode + "','" + model.MeetingType + "','" + model.ManufacturerType + "','" + model.DccNo + "',");
                    query.Append(" (TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ProposalDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ImportProposalDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ImportSubmissionDate + "','dd/MM/yyyy')),");
                    query.Append(" (TO_DATE('" + model.ApvSubmissionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ApvMeetingDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ApvApprovalDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ApvValidUptoDate + "','dd/MM/yyyy')),");
                    query.Append(" (TO_DATE('" + model.ApvDateOfExtension + "','dd/MM/yyyy')),(TO_DATE('" + model.InspectionDate + "','dd/MM/yyyy')),");
                    query.Append(" '" + model.MeetingRemarks + "','" + model.CountryOfOrigin + "','" + model.ManufacturerName + "','" +model.ImportRemarks + "','" + model.ApprovalStatus + "','" + model.ApvRemarks + "',");
                    query.Append(" '" + model.InspectionRemarks + "','" + model.ProposedBy + "','" + model.AlarmDays + "','" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss'))");
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

        public IList<RecipeBEL> GetAllInfo(RecipeBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT D.ID, D.SLNO,D.REVISION_NO,D.COMPANY_CODE,D.PRODUCT_CODE,D.MEETING_TYPE,D.MANUFACTURING_TYPE,D.DCC_NO,TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE,");
            query.Append(" TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,TO_CHAR(D.IMPORT_PROPOSAL_DATE, 'dd/mm/yyyy')IMPORT_PROPOSAL_DATE,TO_CHAR(D.IMPORT_SUBMISSION_DATE, 'dd/mm/yyyy')IMPORT_SUBMISSION_DATE,");
            query.Append(" TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(D.MEETING_DATE, 'dd/mm/yyyy')MEETING_DATE,TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE,");
            query.Append(" TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,TO_CHAR(D.DATE_OF_EXTENSION, 'dd/mm/yyyy')DATE_OF_EXTENSION,TO_CHAR(D.INSPECTION_DATE, 'dd/mm/yyyy')INSPECTION_DATE,");
            query.Append(" D.MEETING_REMARKS,D.COUNTRY_CODE,D.MANUFACTURER_NAME,D.IMPORT_REMARKS,D.APPROVAL_STATUS,D.REMARKS,D.INSPECTION_REMARKS,D.INSPECTION_STATUS,D.PROPOSED_BY,");
            query.Append(" D.NOTIFICATION_DAYS,TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON, P.PRODUCT_CATEGORY,P.BRAND_NAME,P.PRODUCT_SPECIFICATION,");
            query.Append(" C.COMPANY_NAME,C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.INTRODUCED_BANGLADESH,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME");
            query.Append(" FROM RECIPE_INFO D");
            query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE");
            query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE");
            query.Append(" WHERE D.IS_DELETE <>'Y' " );
            query.Append(" AND P.STATUS = 'Active' " );

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND  D.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.ProductCode))
            {
                query.Append(" AND  D.PRODUCT_CODE='{1}'");
            }
            if (!string.IsNullOrEmpty(model.MeetingType) && !model.MeetingType.Equals("All"))
            {
                query.Append(" AND  D.MEETING_TYPE='{2}'");
            }
            if (!string.IsNullOrEmpty(model.ManufacturerType) && !model.ManufacturerType.Equals("All"))
            {
                query.Append(" AND  D.MANUFACTURING_TYPE='{3}'");
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
                        query.Append(" AND D.PROPOSAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                {
                    query.Append(" AND ( D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR D.PROPOSAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
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
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.ProductCode, model.MeetingType, model.ManufacturerType));

            var item = (from DataRow row in dt.Rows
                        select new RecipeBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            RecipeId = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_CODE"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            IntroducedInBD = row["INTRODUCED_BANGLADESH"].ToString(),
                            MeetingType = row["MEETING_TYPE"].ToString(),
                            ManufacturerType = row["MANUFACTURING_TYPE"].ToString(),
                            DccNo = row["DCC_NO"].ToString(),
                            ReceivedDate = row["RECEIVE_DATE"].ToString(),
                            ProposalDate = row["PROPOSAL_DATE"].ToString(),
                            ImportProposalDate = row["IMPORT_PROPOSAL_DATE"].ToString(),
                            ImportSubmissionDate = row["IMPORT_SUBMISSION_DATE"].ToString(),
                            ApvSubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ApvMeetingDate = row["MEETING_DATE"].ToString(),
                            ApvApprovalDate = row["APPROVAL_DATE"].ToString(),
                            ApvValidUptoDate = row["VALID_UPTO"].ToString(),
                            ApvDateOfExtension = row["DATE_OF_EXTENSION"].ToString(),
                            InspectionDate = row["INSPECTION_DATE"].ToString(),
                            MeetingRemarks = row["MEETING_REMARKS"].ToString(),
                            CountryOfOrigin = row["COUNTRY_CODE"].ToString(),
                            ManufacturerName = row["MANUFACTURER_NAME"].ToString(),
                            ImportRemarks = row["IMPORT_REMARKS"].ToString(),
                            ApprovalStatus = row["APPROVAL_STATUS"].ToString(),
                            ApvRemarks = row["REMARKS"].ToString(),
                            InspectionRemarks = row["INSPECTION_REMARKS"].ToString(),
                            ProposedBy = row["PROPOSED_BY"].ToString(),
                           // Ins = row["INSPECTION_STATUS"].ToString(),
                            AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                            SetOn = row["SET_ON"].ToString(),
                            ProductCategory=row["PRODUCT_CATEGORY"].ToString(),
                            BrandName=row["BRAND_NAME"].ToString(),
                            ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString()
                        }).ToList();
            return item;
        }

        public IList<RecipeBEL> GetAllRecipeForAnnex(RecipeBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT A.* FROM (");
            query.Append(" SELECT D.ID, D.SLNO,D.REVISION_NO,D.COMPANY_CODE,D.PRODUCT_CODE,D.MEETING_TYPE,D.MANUFACTURING_TYPE,D.DCC_NO,TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE,");
            query.Append(" TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,TO_CHAR(D.IMPORT_PROPOSAL_DATE, 'dd/mm/yyyy')IMPORT_PROPOSAL_DATE,TO_CHAR(D.IMPORT_SUBMISSION_DATE, 'dd/mm/yyyy')IMPORT_SUBMISSION_DATE,");
            query.Append(" TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(D.MEETING_DATE, 'dd/mm/yyyy')MEETING_DATE,TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE,");
            query.Append(" TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,TO_CHAR(D.DATE_OF_EXTENSION, 'dd/mm/yyyy')DATE_OF_EXTENSION,TO_CHAR(D.INSPECTION_DATE, 'dd/mm/yyyy')INSPECTION_DATE,");
            query.Append(" D.MEETING_REMARKS,D.COUNTRY_CODE,D.MANUFACTURER_NAME,D.IMPORT_REMARKS,D.APPROVAL_STATUS,D.REMARKS,D.INSPECTION_REMARKS,D.INSPECTION_STATUS,D.PROPOSED_BY,");
            query.Append(" D.NOTIFICATION_DAYS,TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON, P.PRODUCT_CATEGORY,P.BRAND_NAME,P.PRODUCT_SPECIFICATION,");
            query.Append(" C.COMPANY_NAME,C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.INTRODUCED_BANGLADESH,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME");
            query.Append(" FROM RECIPE_INFO D");
            query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE");
            query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE");
            query.Append(" WHERE D.IS_DELETE <>'Y' ");
            query.Append(" AND P.STATUS = 'Active' ");

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND D.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.ProductCode))
            {
                query.Append(" AND D.PRODUCT_CODE='{1}'");
            }
            if (!string.IsNullOrEmpty(model.MeetingType) && !model.MeetingType.Equals("All"))
            {
                query.Append(" AND D.MEETING_TYPE='{2}'");
            }
            if (!string.IsNullOrEmpty(model.ManufacturerType) && !model.ManufacturerType.Equals("All"))
            {
                query.Append(" AND D.MANUFACTURING_TYPE='{3}'");
            }
            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND TO_DATE(D.SUBMISSION_DATE,'dd/mm/yyyy') BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
            }
            query.Append("  )A INNER JOIN ( SELECT PRODUCT_CODE,COMPANY_CODE,MAX(REVISION_NO) AS MaxRvNo FROM RECIPE_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) B ON B.PRODUCT_CODE=A.PRODUCT_CODE AND B.COMPANY_CODE=A.COMPANY_CODE AND B.MaxRvNo=A.REVISION_NO");
            query.Append(" LEFT JOIN PRODUCT_REGISTRATION_INFO PR ON PR.RECIPE_ID=A.ID WHERE PR.RECIPE_ID is null");
            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  A.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.ProductCode, model.MeetingType, model.ManufacturerType));

            var item = (from DataRow row in dt.Rows
                        select new RecipeBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            RecipeId = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_CODE"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            IntroducedInBD = row["INTRODUCED_BANGLADESH"].ToString(),
                            MeetingType = row["MEETING_TYPE"].ToString(),
                            ManufacturerType = row["MANUFACTURING_TYPE"].ToString(),
                            DccNo = row["DCC_NO"].ToString(),
                            ReceivedDate = row["RECEIVE_DATE"].ToString(),
                            ProposalDate = row["PROPOSAL_DATE"].ToString(),
                            ImportProposalDate = row["IMPORT_PROPOSAL_DATE"].ToString(),
                            ImportSubmissionDate = row["IMPORT_SUBMISSION_DATE"].ToString(),
                            ApvSubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ApvMeetingDate = row["MEETING_DATE"].ToString(),
                            ApvApprovalDate = row["APPROVAL_DATE"].ToString(),
                            ApvValidUptoDate = row["VALID_UPTO"].ToString(),
                            ApvDateOfExtension = row["DATE_OF_EXTENSION"].ToString(),
                            InspectionDate = row["INSPECTION_DATE"].ToString(),
                            MeetingRemarks = row["MEETING_REMARKS"].ToString(),
                            CountryOfOrigin = row["COUNTRY_CODE"].ToString(),
                            ManufacturerName = row["MANUFACTURER_NAME"].ToString(),
                            ImportRemarks = row["IMPORT_REMARKS"].ToString(),
                            ApprovalStatus = row["APPROVAL_STATUS"].ToString(),
                            ApvRemarks = row["REMARKS"].ToString(),
                            InspectionRemarks = row["INSPECTION_REMARKS"].ToString(),
                            ProposedBy = row["PROPOSED_BY"].ToString(),
                            // Ins = row["INSPECTION_STATUS"].ToString(),
                            AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                            SetOn = row["SET_ON"].ToString(),
                            ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString()
                        }).ToList();
            return item;
        }
        public IList<RecipeBEL> GetRecipeNotify(RecipeBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT A.ID,A.SLNO,A.REVISION_NO,A.SUBMISSION_DATE,A.PROPOSAL_DATE,A.APPROVAL_DATE,A.COMPANY_CODE,A.COMPANY_NAME,A.LICENSE_NO,");
            query.Append(" A.NOTIFICATION_DAYS,A.PRODUCT_CODE,A.BRAND_NAME,A.PRODUCT_CATEGORY,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME,A.COUNTRY_CODE,A.MANUFACTURER_NAME,");
            query.Append(" A.DOSAGE_FORM_NAME,ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0)DateDiff,TO_CHAR(A.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO");
            query.Append(" FROM ( SELECT D.ID,D.SLNO,D.REVISION_NO,TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,D.COMPANY_CODE,C.COMPANY_NAME,C.LICENSE_NO,");
            query.Append(" TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE, TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,");
            query.Append(" D.VALID_UPTO,D.NOTIFICATION_DAYS,  TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,D.COUNTRY_CODE,D.MANUFACTURER_NAME,P.PRODUCT_CODE,P.BRAND_NAME,P.PRODUCT_CATEGORY,");
            query.Append(" P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME  FROM RECIPE_INFO D ");
            query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE  LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE ");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ) A ");
            query.Append(" INNER JOIN ( SELECT PRODUCT_CODE,COMPANY_CODE,MAX(REVISION_NO) AS MaxRvNo FROM RECIPE_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) B");
            query.Append(" ON B.PRODUCT_CODE=A.PRODUCT_CODE AND B.COMPANY_CODE=A.COMPANY_CODE AND B.MaxRvNo=A.REVISION_NO ");
            query.Append(" LEFT JOIN PRODUCT_REGISTRATION_INFO PR ON PR.RECIPE_ID=A.ID WHERE PR.RECIPE_ID is null ");
            query.Append(" AND P.STATUS = 'Active' ");

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND A.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.ProductCode))
            {
                query.Append(" AND A.PRODUCT_CODE='{1}'");
            }
            if (!string.IsNullOrEmpty(model.AlarmDays))
            {
                query.Append(" AND ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0) <= {2}");
            }
            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND A.VALID_UPTO BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            }
            if (string.IsNullOrEmpty(model.CompanyCode) && string.IsNullOrEmpty(model.ProductCode) && string.IsNullOrEmpty(model.AlarmDays) && string.IsNullOrEmpty(model.FromDate) && string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ");
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  A.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.ProductCode, model.AlarmDays));

            var item = (from DataRow row in dt.Rows
                        select new RecipeBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_CODE"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            ProposalDate = row["PROPOSAL_DATE"].ToString(),
                            ApvSubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ApvValidUptoDate = row["VALID_UPTO"].ToString(),
                            CountryOfOrigin = row["COUNTRY_CODE"].ToString(),
                            ManufacturerName = row["MANUFACTURER_NAME"].ToString(),
                            AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                            DateDiff = Convert.ToInt32(row["DateDiff"])
                        }).ToList();
            return item;
        }



        public DataTable GetFileRefno(int p, string refLevel1)
        {
            var query = new StringBuilder();
            query.Append(" SELECT filetype,LD,DCCD,IMD FROM ");
            query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
            query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Local Document' LD,'DCC Document' DCCD,'Import Document' IMD))");

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), p, refLevel1));
            return dt;
        }

        public DataTable GetRecipeForReport(ReportModel model,string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT ROW_NUMBER() OVER(ORDER BY A.ID)SN, A.ID,A.PRODUCT_CODE,A.MEETING_TYPE,A.REMARKS,A.MANUFACTURING_TYPE,A.MEETING_DATE,A.PROPOSED_BY,A.SUBMISSION_DATE,A.APPROVAL_DATE,A.COMPANY_CODE,A.COMPANY_NAME,");
            query.Append(" A.BRAND_NAME,A.PRODUCT_CATEGORY,A.SAP_PRODUCT_CODE,A.GENERIC_CODE,A.PACK_SIZE_NAME,A.DOSAGE_FORM_NAME,A.VALID_UPTO");
            query.Append(" FROM ( SELECT D.ID,D.SLNO,P.PRODUCT_CODE,D.MEETING_TYPE,D.REMARKS,D.MANUFACTURING_TYPE,D.REVISION_NO,D.MEETING_DATE,D.PROPOSED_BY,D.COMPANY_CODE,C.COMPANY_NAME,C.LICENSE_NO,D.APPROVAL_DATE,");
            query.Append(" D.SUBMISSION_DATE, D.VALID_UPTO,P.BRAND_NAME,P.PRODUCT_CATEGORY, P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME  FROM RECIPE_INFO D");
            query.Append(" INNER JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE  INNER JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE  INNER JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE ");
            query.Append(" WHERE 1=1 ");
            query.Append(" AND P.STATUS = 'Active' ");
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND  P.COMPANY_CODE='{0}'");
            }
            //if (!string.IsNullOrEmpty(model.ProductCode))
            //{
            //    query.Append(" AND  D.PRODUCT_CODE='{1}'");
            //}
            if (!string.IsNullOrEmpty(model.BrandName))
            {
                query.Append(" AND TRIM(P.BRAND_NAME)='{1}'");
                model.BrandName = model.BrandName.Trim();
            }
            else if (!string.IsNullOrEmpty(model.GenericStrength))
            {
                query.Append(" AND P.GENERIC_CODE='{4}'");
            }
            if (!string.IsNullOrEmpty(model.SubmissionType) && !model.SubmissionType.Equals("All"))
            {
                query.Append(" AND  D.MEETING_TYPE='{2}'");
            }
            if (!string.IsNullOrEmpty(model.ManufacturerType) && !model.ManufacturerType.Equals("All"))
            {
                query.Append(" AND  D.MANUFACTURING_TYPE='{3}'");
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
                        query.Append(" AND D.PROPOSAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                {
                    query.Append(" AND ( D.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR D.PROPOSAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR D.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                }
            }
           
            
            query.Append(" ) A INNER JOIN ( SELECT PRODUCT_CODE,COMPANY_CODE,MAX(REVISION_NO) AS MaxRvNo FROM RECIPE_INFO GROUP BY COMPANY_CODE,PRODUCT_CODE) B ON B.PRODUCT_CODE=A.PRODUCT_CODE AND B.COMPANY_CODE=A.COMPANY_CODE AND B.MaxRvNo=A.REVISION_NO");
           // query.Append(" WHERE 1=1 ");
           
            //if (!string.IsNullOrEmpty(orderBy))
            //{
            //    query.Append(" ORDER BY  A.ID " + orderBy);
            //}
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.BrandName, model.SubmissionType, model.ManufacturerType, model.GenericStrength));

            return dt;
        }

        public IList<RecipeBEL> GetNewCommingRecipe()
        {
            var query = new StringBuilder();
            query.Append(" SELECT D.ID, D.SLNO,D.REVISION_NO,D.COMPANY_CODE,D.PRODUCT_CODE,D.MEETING_TYPE,D.MANUFACTURING_TYPE,D.DCC_NO,TO_CHAR(D.RECEIVE_DATE, 'dd/mm/yyyy')RECEIVE_DATE,");
            query.Append(" TO_CHAR(D.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,TO_CHAR(D.IMPORT_PROPOSAL_DATE, 'dd/mm/yyyy')IMPORT_PROPOSAL_DATE,TO_CHAR(D.IMPORT_SUBMISSION_DATE, 'dd/mm/yyyy')IMPORT_SUBMISSION_DATE,");
            query.Append(" TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(D.MEETING_DATE, 'dd/mm/yyyy')MEETING_DATE,TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE,");
            query.Append(" TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,TO_CHAR(D.DATE_OF_EXTENSION, 'dd/mm/yyyy')DATE_OF_EXTENSION,TO_CHAR(D.INSPECTION_DATE, 'dd/mm/yyyy')INSPECTION_DATE,");
            query.Append(" D.MEETING_REMARKS,D.COUNTRY_CODE,D.MANUFACTURER_NAME,D.IMPORT_REMARKS,D.APPROVAL_STATUS,D.REMARKS,D.INSPECTION_REMARKS,D.INSPECTION_STATUS,D.PROPOSED_BY,");
            query.Append(" D.NOTIFICATION_DAYS,TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON, P.PRODUCT_CATEGORY,P.BRAND_NAME,P.PRODUCT_SPECIFICATION,");
            query.Append(" C.COMPANY_NAME,C.LICENSE_NO,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.INTRODUCED_BANGLADESH,P.PACK_SIZE_NAME,DF.DOSAGE_FORM_NAME");
            query.Append(" FROM RECIPE_INFO D");
            query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE");
            query.Append(" LEFT JOIN  PRODUCT_INFO P ON P.PRODUCT_CODE=D.PRODUCT_CODE");
            query.Append(" LEFT JOIN  DOSAGE_FORM_INFO DF ON DF.DOSAGE_FORM_CODE=P.DOSAGE_FORM_CODE");
            query.Append(" WHERE D.IS_DELETE <>'Y' AND D.RECEIVE_DATE is not null and D.SUBMISSION_DATE is null");
            query.Append(" AND P.STATUS = 'Active' ");

            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString()));

            var item = (from DataRow row in dt.Rows
                        select new RecipeBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            RecipeId = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenAndStrength = row["GENERIC_CODE"].ToString(),
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                            DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            IntroducedInBD = row["INTRODUCED_BANGLADESH"].ToString(),
                            MeetingType = row["MEETING_TYPE"].ToString(),
                            ManufacturerType = row["MANUFACTURING_TYPE"].ToString(),
                            DccNo = row["DCC_NO"].ToString(),
                            ReceivedDate = row["RECEIVE_DATE"].ToString(),
                            ProposalDate = row["PROPOSAL_DATE"].ToString(),
                            ImportProposalDate = row["IMPORT_PROPOSAL_DATE"].ToString(),
                            ImportSubmissionDate = row["IMPORT_SUBMISSION_DATE"].ToString(),
                            ApvSubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ApvMeetingDate = row["MEETING_DATE"].ToString(),
                            ApvApprovalDate = row["APPROVAL_DATE"].ToString(),
                            ApvValidUptoDate = row["VALID_UPTO"].ToString(),
                            ApvDateOfExtension = row["DATE_OF_EXTENSION"].ToString(),
                            InspectionDate = row["INSPECTION_DATE"].ToString(),
                            MeetingRemarks = row["MEETING_REMARKS"].ToString(),
                            CountryOfOrigin = row["COUNTRY_CODE"].ToString(),
                            ManufacturerName = row["MANUFACTURER_NAME"].ToString(),
                            ImportRemarks = row["IMPORT_REMARKS"].ToString(),
                            ApprovalStatus = row["APPROVAL_STATUS"].ToString(),
                            ApvRemarks = row["REMARKS"].ToString(),
                            InspectionRemarks = row["INSPECTION_REMARKS"].ToString(),
                            ProposedBy = row["PROPOSED_BY"].ToString(),
                            // Ins = row["INSPECTION_STATUS"].ToString(),
                            AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                            SetOn = row["SET_ON"].ToString(),
                            ProductCategory = row["PRODUCT_CATEGORY"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            ProductSpecification = row["PRODUCT_SPECIFICATION"].ToString()
                        }).ToList();
            return item;
        }
    }
}