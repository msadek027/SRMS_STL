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
    public class ExportInfoDAO : ReturnData
    {
        DBConnection _dbConn = new DBConnection();
        DBHelper _dbHelper = new DBHelper();
        IDGenerated _idGenerated = new IDGenerated();
        public IList<ExportInfoBEL> GetAll(ExportInfoBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT E.ID,E.SLNO,E.COMPANY_CODE,C.COMPANY_NAME,C.ADDRESS,C.LICENSE_NO,E.EXPORTING_COUNTRY,TO_CHAR(SUBMISSION_DATE, 'YYYY')YEAR,TO_CHAR(SUBMISSION_DATE, 'Month') as MONTH_NAME, ");
            query.Append(" TO_CHAR(E.RECEIVED_DATE, 'dd/mm/yyyy')RECEIVED_DATE,TO_CHAR(E.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(E.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE FROM EXPORT_MST E");
            query.Append(" LEFT JOIN COMPANY_INFO C ON C.COMPANY_CODE=E.COMPANY_CODE ");
            query.Append(" WHERE 1=1 ");


            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  E.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString()));

            var item = (from DataRow row in dt.Rows
                        select new ExportInfoBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            Address = row["ADDRESS"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            ExportCountry = row["EXPORTING_COUNTRY"].ToString(),
                            ReceivedDate = row["RECEIVED_DATE"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            ApprovalDate = row["APPROVAL_DATE"].ToString(),
                            Year = row["YEAR"].ToString(),
                            Month = row["MONTH_NAME"].ToString()
                        }).ToList();
            return item;
        }
        public IList<ExportDetailBEL> GetAllDetail(string id, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT E.ID,E.ITEM_NAME,E.BRAND_NAME,E.GENERIC_STRENGTH,E.DOSAGE_FORM,E.PACK_SIZE,E.DAR_NO, ");
            query.Append(" E.BRAND_NAME_EXPORT,E.PACK_SIZE_EXPORT,E.EXPORTING_COUNTRY,E.QUANTITY");
            query.Append(" FROM EXPORT_DTL E LEFT JOIN EXPORT_MST EM ON EM.ID=E.EXP_ID ");
            query.Append(" WHERE 1=1 ");

            if (!string.IsNullOrEmpty(id))
            {
                query.Append(" AND  E.EXP_ID ={0}");
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  E.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), id));

            var item = (from DataRow row in dt.Rows
                        select new ExportDetailBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            ItemName = row["ITEM_NAME"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            GenAndStrength = row["GENERIC_STRENGTH"].ToString(),
                            DossageForm = row["DOSAGE_FORM"].ToString(),
                            PackSize = row["PACK_SIZE"].ToString(),
                            DarNo = row["DAR_NO"].ToString(),
                            ExportBrandName = row["BRAND_NAME_EXPORT"].ToString(),
                            ExportPackSize = row["PACK_SIZE_EXPORT"].ToString(),
                            ExportCountry = row["EXPORTING_COUNTRY"].ToString(),
                            Quantity = row["QUANTITY"].ToString()
                        }).ToList();
            return item;
        }
        public bool SaveUpdate(ExportInfoBEL model, string userId)
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
                    query.Append(" UPDATE EXPORT_MST SET");
                    query.Append(" COMPANY_CODE='" + model.CompanyCode + "',");
                    query.Append(" EXPORTING_COUNTRY='" + model.ExportCountry + "',");
                    query.Append(" RECEIVED_DATE= TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy'), ");
                    query.Append(" SUBMISSION_DATE= TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy'), ");
                    query.Append(" APPROVAL_DATE=TO_DATE('" + model.ApprovalDate + "','dd/MM/yyyy'), ");
                    query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')), ");
                    query.Append(" UPDATE_BY='" + userId + "'");
                    query.Append(" WHERE ID='" + model.ID + "'");

                    
                }
                else
                { //I for Insert  
                    ReturnMaxID = _idGenerated.getMAXSL("EXPORT_MST", "ID");
                    MaxID = _idGenerated.getMAXID("EXPORT_MST", "SLNO", "fm000000000");
                    IUMode = "I";
                    query.Append(" INSERT INTO EXPORT_MST(");
                    query.Append(" ID,SLNO,COMPANY_CODE,EXPORTING_COUNTRY,RECEIVED_DATE,");
                    query.Append(" SUBMISSION_DATE,APPROVAL_DATE,IS_DELETE,SET_BY, SET_ON)");
                    query.Append(" VALUES( '" + ReturnMaxID + "','" + MaxID + "','" + model.CompanyCode + "','" + model.ExportCountry + "',");
                    query.Append(" TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy'), TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy'), TO_DATE('" + model.ApprovalDate + "','dd/MM/yyyy')," + "'N','");
                    query.Append(userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')))");
                }
                isReturn = _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());

                if (model.ExportDetail != null)
                {
                    foreach (ExportDetailBEL objItem in model.ExportDetail)
                    {
                        var qry = new StringBuilder();
                        if (objItem.ID == 0)
                        {
                            qry.Append(" INSERT INTO EXPORT_DTL(ID,EXP_ID,ITEM_NAME,BRAND_NAME,GENERIC_STRENGTH,DOSAGE_FORM,PACK_SIZE,DAR_NO,BRAND_NAME_EXPORT,PACK_SIZE_EXPORT,EXPORTING_COUNTRY, QUANTITY, IS_DELETE, SET_BY, SET_ON) ");
                            qry.Append(" VALUES( '" + _idGenerated.getMAXSL("EXPORT_DTL", "ID") + "','" + ReturnMaxID + "','" + objItem.ItemName + "','" + objItem.BrandName + "','" + objItem.GenAndStrength + "',");
                            qry.Append(" '" + objItem.DossageForm + "','" + objItem.PackSize + "','" + objItem.DarNo + "','" + objItem.ExportBrandName + "','" + objItem.ExportPackSize + "','" + objItem.ExportCountry + "','" + objItem.Quantity + "','N',");
                            qry.Append(" '" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')))");
                        }
                        else
                        {
                            qry.Append(" UPDATE EXPORT_DTL SET");
                            qry.Append(" ITEM_NAME='" + objItem.ItemName + "',");
                            qry.Append(" BRAND_NAME='" + objItem.BrandName + "',");
                            qry.Append(" GENERIC_STRENGTH='" + objItem.GenAndStrength + "',");
                            qry.Append(" DOSAGE_FORM='" + objItem.DossageForm + "',");
                            qry.Append(" PACK_SIZE='" + objItem.PackSize + "',");
                            qry.Append(" DAR_NO='" + objItem.DarNo + "',");
                            qry.Append(" BRAND_NAME_EXPORT='" + objItem.ExportBrandName + "',");
                            qry.Append(" PACK_SIZE_EXPORT='" + objItem.ExportPackSize + "',");
                            qry.Append(" EXPORTING_COUNTRY='" + objItem.ExportCountry + "',");
                            qry.Append(" QUANTITY='" + objItem.Quantity + "',");
                            qry.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')), ");
                            qry.Append(" UPDATE_BY='" + userId + "'");
                            qry.Append(" WHERE ID=" + objItem.ID);
                        }
                        isReturn = _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), qry.ToString());
                    }

                }
                return isReturn;

            }
            catch (Exception errorException)
            {
                return false;
            }
        }
        public bool DeleteDataItem(string id)
        {
            try
            {

                string deleteQuery = "DELETE FROM EXPORT_DTL WHERE ID = '" + id + "'";
                if (_dbHelper.CmdExecute(_dbConn.SAConnStrReader(), deleteQuery))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool InsertDataTable(DataTable dt, string userId)
        {
            bool retValue = false;
            var exId = 5;
            foreach (DataRow dr in dt.Rows)
            {
                var qry = new StringBuilder();
                qry.Append("INSERT INTO EXPORT_DTL(ID,EXP_ID,ITEM_NAME,BRAND_NAME,GENERIC_STRENGTH,DOSAGE_FORM,PACK_SIZE,DAR_NO,BRAND_NAME_EXPORT,PACK_SIZE_EXPORT,EXPORTING_COUNTRY, QUANTITY, IS_DELETE, SET_BY, SET_ON)");
                qry.Append(" VALUES('" + _idGenerated.getMAXSL("EXPORT_DTL", "ID") + "','" + exId + "', '" + dr["ItemName"].ToString() + "', '" + dr["BrandName"].ToString() + "','" + dr["GenAndStrength"].ToString() + "','" + dr["DossageForm"].ToString());// +"')";
                qry.Append(" ','" + dr["PackSize"].ToString() + "','" + dr["DarNo"].ToString() + "','" + dr["ExportBrandName"].ToString() + "','" + dr["ExportPackSize"].ToString() + "','" + dr["ExportCountry"].ToString() + "','" + dr["Quantity"].ToString() + "','N',");
                qry.Append(" '" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')))");
                retValue = _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), qry.ToString());
            }
            
            return retValue;
        }

        public DataTable GetAllExportWithDetailForReport(ReportModel model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT ROW_NUMBER() OVER(ORDER BY ED.ID)SN, E.ID,E.SLNO,E.COMPANY_CODE,C.COMPANY_NAME,C.ADDRESS,C.LICENSE_NO,E.EXPORTING_COUNTRY,TO_CHAR(SUBMISSION_DATE, 'YYYY')YEAR,TO_CHAR(SUBMISSION_DATE, 'Month') as MONTH_NAME, ");
            query.Append(" ED.ITEM_NAME,ED.BRAND_NAME,ED.GENERIC_STRENGTH,ED.DOSAGE_FORM,ED.PACK_SIZE,ED.DAR_NO,ED.BRAND_NAME_EXPORT,ED.PACK_SIZE_EXPORT,ED.EXPORTING_COUNTRY EXPORT_COUNTRY,ED.QUANTITY,");
            query.Append(" TO_CHAR(E.RECEIVED_DATE, 'dd/mm/yyyy')RECEIVED_DATE,TO_CHAR(E.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE,TO_CHAR(E.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE FROM EXPORT_MST E");
            query.Append(" LEFT JOIN EXPORT_DTL ED ON ED.EXP_ID=E.ID");
            query.Append(" LEFT JOIN COMPANY_INFO C ON C.COMPANY_CODE=E.COMPANY_CODE ");
            query.Append(" WHERE 1=1 ");
           
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND C.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.BrandName) )
            {
                query.Append(" AND TRIM(ED.BRAND_NAME)='{1}'");
                model.BrandName = model.BrandName.Trim();
            }
            if (!string.IsNullOrEmpty(model.ExportCounty))
            {
                query.Append(" AND ED.EXPORTING_COUNTRY='{2}'");
            }
            if (!string.IsNullOrEmpty(model.ItemName) && !model.ItemName.Equals("All"))
            {
                query.Append(" AND ED.ITEM_NAME='{3}'");
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
                    //query.Append(" OR E.RECEIVED_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR E.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                }
            }

            //if (!string.IsNullOrEmpty(orderBy))
            //{
            //    query.Append(" ORDER BY  E.ID " + orderBy);
            //}
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.BrandName, model.ExportCounty,model.ItemName));

            return dt;
        }
    }
}