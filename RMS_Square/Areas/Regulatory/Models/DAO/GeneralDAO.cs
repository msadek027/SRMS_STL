using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.DAL.Gateway;
using RMS_Square.Universal.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Systems.Models;

namespace RMS_Square.Areas.Regulatory.Models.DAO
{
    public class GeneralDAO
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;
        public GeneralDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }
        public IList<RecipeBEL> GetNewComming(string pageName)
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