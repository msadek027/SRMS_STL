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
    public class PriceFixComDAO:ReturnData
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;

        private string MeetingNO;
        public PriceFixComDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }
        public bool SaveUpdate(PriceFixComBEL model, string userId)
        {
            try
            {
                var query = new StringBuilder();
                if (model.ID > 0)
                {
                    //U for update
                    ReturnMaxID = model.ID;
                    MaxID = model.SlNo;
                    RefNo = model.ProductSlNo;
                    MeetingNO = model.MeetingNo;
                    IUMode = "U";
                    query.Append(" UPDATE PRICE_FIXATION_INFO SET PRICE_SUB_DATE=(TO_DATE('" + model.PriceSubmissionDate+ "','dd/MM/yyyy')),");
                    query.Append(" EXISTING_PRICE ='" + model.ExistingPrice + "',PROPOSED_PRICE='" + model.ProposedPrice + "',");
                    query.Append(" DGDA_MRP ='" + model.DGDAMRP + "', PFC_MRP ='" + model.PFCMRP + "',APPROVED_MRP='" + model.ApprovedMRP + "',GOVT_FIXED_HIGH_SELLING='"+model.GovtFixedHighSelling+"',REMARKS='" + model.Remarks + "',");
                    query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),UPDATE_BY='" + userId + "'");
                    query.Append(" WHERE ID='" + model.ID + "'");
                }
                else
                { //I for Insert  
                    ReturnMaxID = _idGenerated.getMAXSL("PRICE_FIXATION_INFO", "ID");                   
                    if (model.MeetingNo ==null)
                    {
                        MeetingNO = _dbHelper.GetValue("SELECT  NVL(MAX(MEETING_NO),0)+1 AS MeetingNo FROM PRICE_FIXATION_INFO");
                        MaxID = MeetingNO;
                    }
                    else
                    {
                        MaxID = model.MeetingNo;
                    }                   
                    var slno= _idGenerated.getMAXID("PRICE_FIXATION_INFO", "SLNO", "fm000000000");
                    string strCount = _dbHelper.GetValue("SELECT nvl(max(PRODUCT_SLNO),0)+1 AS ProductSlNo FROM PRICE_FIXATION_INFO  WHERE MEETING_NO='" + MaxID + "' GROUP BY MEETING_NO");
                    
                    if (!string.IsNullOrEmpty(strCount))
                    {
                        RefNo = strCount;
                        //model.SubmissionType = "Renewal";
                    }
                    else
                    {
                        RefNo = "1";
                        //  model.SubmissionType = "License";
                    }

                    IUMode = "I";

                    query.Append(" INSERT INTO PRICE_FIXATION_INFO(ID,SLNO,MEETING_DATE,MEETING_NO,COMPANY_CODE,PRODUCT_CODE,PRODUCT_SLNO,REVISION_DATE,PRICE_SUB_DATE,EXISTING_PRICE,PROPOSED_PRICE,DGDA_MRP,PFC_MRP,APPROVED_MRP,GOVT_FIXED_HIGH_SELLING,REMARKS,SET_BY,SET_ON) ");
                    query.Append(" VALUES( '" + ReturnMaxID + "','" + slno + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),'" + MaxID + "','" + model.CompanyCode + "','" + model.ProductCode + "','" + RefNo + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),");
                    query.Append(" (TO_DATE('" + model.PriceSubmissionDate + "','dd/MM/yyyy')),'" + model.ExistingPrice + "','" + model.ProposedPrice + "','" + model.DGDAMRP + "','" + model.PFCMRP + "','" + model.ApprovedMRP + "',");
                    query.Append(" '"+model.GovtFixedHighSelling+"','" +model.Remarks+ "','" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                    query.Append(" )");

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

        public List<ProductInfoBEL> GetActiveProductList()
        {
            string Qry = "SELECT P.PRODUCT_CODE,P.SAP_PRODUCT_CODE,P.GENERIC_CODE,P.DOSAGE_FORM_CODE,D.DOSAGE_FORM_NAME,P.PACK_SIZE_NAME,P.BRAND_NAME " +
                        "FROM PRODUCT_INFO P, DOSAGE_FORM_INFO D " +
                        "WHERE P.DOSAGE_FORM_CODE=D.DOSAGE_FORM_CODE AND P.STATUS='Active' ORDER BY PRODUCT_CODE";
                       
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), Qry);
            List<ProductInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new ProductInfoBEL
                    {
                        ProductCode = row["PRODUCT_CODE"].ToString(),
                        SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                        GenericCode = row["GENERIC_CODE"].ToString(),                        
                        DosageFormCode = row["DOSAGE_FORM_CODE"].ToString(),
                        DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                        PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                        BrandName = row["BRAND_NAME"].ToString()                  
                    }).ToList();
            return item;
        }


        public List<PriceFixComBEL> GetPriceFixComMeetingList()
        {
            string Qry = "select  DISTINCT  MEETING_NO, TO_CHAR(MEETING_DATE,'DD/MM/RRRR') MEETING_DATE FROM PRICE_FIXATION_INFO ";
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), Qry);
            List<PriceFixComBEL> item;

            item = (from DataRow row in dt.Rows
                    select new PriceFixComBEL
                    {
                        MeetingNo = row["MEETING_NO"].ToString(),
                        MeetingDate = row["MEETING_DATE"].ToString()                        
                    }).ToList();
            return item;
        }

        public IList<PriceFixComBEL> GetAllInfo(PriceFixComBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append("SELECT A.ID , A.SLNO, TO_CHAR(A.MEETING_DATE,'DD/MM/RRRR') MEETING_DATE,A.MEETING_NO,A.COMPANY_CODE,C.COMPANY_NAME, ");
            query.Append("A.PRODUCT_CODE,P.SAP_PRODUCT_CODE , P.GENERIC_CODE, P.DOSAGE_FORM_CODE, D.DOSAGE_FORM_NAME,P.PACK_SIZE_NAME,TO_CHAR(A.RECEIVE_DATE,'DD/MM/RRRR')RECEIVE_DATE,");
            query.Append("BRAND_NAME,A.PRODUCT_SLNO,A.REVISION_DATE,TO_CHAR(A.PRICE_SUB_DATE,'DD/MM/RRRR') PRICE_SUB_DATE, NVL(A.EXISTING_PRICE,0) EXISTING_PRICE,");
            query.Append(" NVL(A.PROPOSED_PRICE,0) PROPOSED_PRICE,NVL(A.DGDA_MRP,0) DGDA_MRP, NVL(A.PFC_MRP,0) PFC_MRP,");
            query.Append(" NVL(A.APPROVED_MRP,0) APPROVED_MRP, NVL(A.GOVT_FIXED_HIGH_SELLING,0) GOVT_FIXED_HIGH_SELLING,A.REMARKS ");
            query.Append(" FROM PRICE_FIXATION_INFO A,  COMPANY_INFO C, PRODUCT_INFO P, DOSAGE_FORM_INFO D");
            query.Append(" WHERE A.COMPANY_CODE=C.COMPANY_CODE AND A.PRODUCT_CODE=P.PRODUCT_CODE ");
            query.Append(" AND P.DOSAGE_FORM_CODE=D.DOSAGE_FORM_CODE ");

            if (!string.IsNullOrEmpty(model.MeetingNo))
            {
                query.Append(" AND  A.MEETING_NO='{0}'");
            }

            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND A.MEETING_DATE BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  A.MEETING_NO DESC, A.SLNO " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.MeetingNo));

            var item = (from DataRow row in dt.Rows
                        select new PriceFixComBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            MeetingNo = row["MEETING_NO"].ToString(),
                            MeetingDate = row["MEETING_DATE"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            ProductSlNo = row["PRODUCT_SLNO"].ToString(),
                            ProductCode = row["PRODUCT_CODE"].ToString(),
                            SAPProductCode = row["SAP_PRODUCT_CODE"].ToString(),
                            GenericCode = row["GENERIC_CODE"].ToString(),                            
                            PackSizeName = row["PACK_SIZE_NAME"].ToString(),                            
                            DosageFormName = row["DOSAGE_FORM_NAME"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            ReceivedDate = row["RECEIVE_DATE"].ToString(),
                            PriceSubmissionDate = row["PRICE_SUB_DATE"].ToString(),
                            ExistingPrice = row["EXISTING_PRICE"].ToString(),
                            ProposedPrice = row["PROPOSED_PRICE"].ToString(),
                            DGDAMRP = row["DGDA_MRP"].ToString(),
                            PFCMRP = row["PFC_MRP"].ToString(),
                            ApprovedMRP = row["APPROVED_MRP"].ToString(),
                            GovtFixedHighSelling = row["GOVT_FIXED_HIGH_SELLING"].ToString(),                           
                            Remarks = row["REMARKS"].ToString()                            
                        }).ToList();
            return item;
        }
       
        public DataTable GetFileRefno(int p, string refLevel1)
        {
            var query = new StringBuilder();
            query.Append(" SELECT filetype,CDF,I,BE FROM ");
            query.Append(" (SELECT REFNO,filetype FROM DOCUMENTFILEINFO WHERE filetype='{0}' AND reflevel1='{1}'");
            query.Append(" )PIVOT (COUNT(REFNO) FOR REFNO IN('Costing DGDA Format' CDF,'Invoice' I,'Bill of Entry' BE))");
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), p, refLevel1));
            return dt;
        }

        public void UpdateFileRelatedInfo(PriceFixComBEL model, string userId)
        {
            var query = new StringBuilder();
            if (model.ID > 0)
            {
                //U for update
                ReturnMaxID = model.ID;
                query.Append(" UPDATE PRICE_FIXATION_INFO SET RECEIVE_DATE =(TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy')), ");
                query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                query.Append(" WHERE ID='" + model.ID + "'");
            }
            _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());
        }
    }
}