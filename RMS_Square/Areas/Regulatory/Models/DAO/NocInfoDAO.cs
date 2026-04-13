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
    public class NocInfoDAO : ReturnData
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;
        public NocInfoDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }

        public List<NocInfoBEL> GetNocInformation()
        {
            var selectQuery = new StringBuilder();

            selectQuery.Append("SELECT ID,SLNO,COMPANY_CODE,COMPANY_NAME,ADDRESS,REVISION_NO,LICENSE_NO,INVICE_NO,TO_CHAR(RECEIVE_DATE,'DD/MM/YYYY')RECEIVE_DATE,PROPOSED_BY,PROPOSED_DEPT,");
            selectQuery.Append(" TO_CHAR(INVOICE_DATE,'DD/MM/YYYY')INVOICE_DATE,COUNTRY_OF_ORIGNIN,PURPOSE,TO_CHAR(SUBMISSION_DATE,'DD/MM/YYYY') SUBMISSION_DATE,TO_CHAR(APPROVAL_DATE,'DD/MM/YYYY') APPROVAL_DATE, TO_CHAR(SUBMISSION_DATE, 'YYYY')YEAR,TO_CHAR(SUBMISSION_DATE, 'Month') as MONTH_NAME FROM VW_NOC_INFO  ORDER BY ID DESC");


            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), selectQuery.ToString());
            List<NocInfoBEL> item;
            item = (from DataRow row in dt.Rows
                    select new NocInfoBEL
                    {
                        ID = Convert.ToInt64(row["ID"]),
                        SlNo = row["SLNO"].ToString(),
                        RevisionNo = row["REVISION_NO"].ToString(),
                        CompanyCode = row["COMPANY_CODE"].ToString(),
                        CompanyName = row["COMPANY_NAME"].ToString(),
                        Address = row["ADDRESS"].ToString(),
                        LicenseNo = row["LICENSE_NO"].ToString(),
                        InvoiceNo = row["INVICE_NO"].ToString(),
                        InvoiceDate = row["INVOICE_DATE"].ToString(),
                        CountryOfOrigin = row["COUNTRY_OF_ORIGNIN"].ToString(),
                        Purpose = row["PURPOSE"].ToString(),
                        ReceivedDate = row["RECEIVE_DATE"].ToString(),
                        ProposedBy = row["PROPOSED_BY"].ToString(),
                        ProposedDepartment = row["PROPOSED_DEPT"].ToString(),
                        SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                        ApprovalDate = row["APPROVAL_DATE"].ToString(),
                        Year = row["YEAR"].ToString(),
                        Month = row["MONTH_NAME"].ToString()
                    }).ToList();
            return item;
        }

        public IList<NocDetailInfo> GetAll(string id, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT D.ID, D.ITEM_SLNO, D.ITEM_NAME, D.QUANTITY");
            query.Append(" FROM NOC_DTL D");
            query.Append(" LEFT JOIN  NOC_MST N ON N.ID=D.ND_ID");

            if (!string.IsNullOrEmpty(id))
            {
                query.Append(" WHERE  D.ND_ID={0}");
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY D.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), id));

            var item = (from DataRow row in dt.Rows
                        select new NocDetailInfo
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            ItemSlNo = row["ITEM_SLNO"].ToString(),
                            ItemName = row["ITEM_NAME"].ToString(),
                            ItemQty = row["QUANTITY"].ToString()
                        }).ToList();
            return item;
        }

        public bool SaveUpdate(NocInfoBEL model, string userId)
        {
            //long ReturnMaxId = 0;
           
            bool isReturn = false;
            try
            {
                var query = new StringBuilder();
                if (model.ID > 0)
                {
                    //U for update
                    ReturnMaxID = model.ID;
                    //ReturnMaxId = model.ID;
                    MaxID = model.SlNo;
                    RefNo = model.RevisionNo;
                    IUMode = "U";
                    query.Append(" UPDATE NOC_MST SET");
                    query.Append(" COMPANY_CODE='" + model.CompanyCode + "',");
                    query.Append(" LICENSE_NO='" + model.LicenseNo + "',");
                    query.Append(" INVICE_NO='" + model.InvoiceNo + "',");
                    query.Append(" INVOICE_DATE=TO_DATE('" + model.InvoiceDate + "','dd/MM/yyyy'), ");
                    query.Append(" COUNTRY_OF_ORIGNIN='" + model.CountryOfOrigin + "',");
                    query.Append(" PURPOSE='" + model.Purpose + "',");
                    query.Append(" RECEIVE_DATE=TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy'), ");
                    query.Append(" PROPOSED_BY='" + model.ProposedBy + "',");
                    query.Append(" PROPOSED_DEPT='" + model.ProposedDepartment + "',");
                    query.Append(" SUBMISSION_DATE= TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy'), ");
                    query.Append(" APPROVAL_DATE=TO_DATE('" + model.ApprovalDate + "','dd/MM/yyyy'), ");
                    query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')), ");
                    query.Append(" UPDATE_BY='" + userId + "'");
                    query.Append(" WHERE ID='" + model.ID + "'");
                }
                else
                { //I for Insert  
                    ReturnMaxID = _idGenerated.getMAXSL("NOC_MST", "ID");
                    //ReturnMaxId = _idGenerated.getMAXSL("NOC_MST", "ID");
                    MaxID = _idGenerated.getMAXID("NOC_MST", "SLNO", "fm000000000");
                    string strCount = _dbHelper.GetValue("SELECT COUNT(1) AS RevisionNo FROM NOC_MST  WHERE COMPANY_CODE='" + model.CompanyCode + "' GROUP BY COMPANY_CODE");
                    if (!string.IsNullOrEmpty(strCount))
                    {
                        RefNo = strCount;
                    }
                    else
                    {
                        RefNo = "0";
                    }

                    IUMode = "I";
                    query.Append(" INSERT INTO NOC_MST(");
                    query.Append(" ID, SLNO,REVISION_NO, COMPANY_CODE, LICENSE_NO,");
                    query.Append(" INVICE_NO,INVOICE_DATE,COUNTRY_OF_ORIGNIN,PURPOSE,RECEIVE_DATE,PROPOSED_BY,PROPOSED_DEPT,");
                    query.Append(" SUBMISSION_DATE,APPROVAL_DATE,IS_DELETE,");
                    query.Append(" SET_BY, SET_ON)");
                    query.Append(" VALUES( '" + ReturnMaxID + "','" + MaxID + "','" + RefNo + "','" + model.CompanyCode + "','" + model.LicenseNo + "','" + model.InvoiceNo + "',");
                    query.Append(" TO_DATE('" + model.InvoiceDate + "','dd/MM/yyyy'),'" + model.CountryOfOrigin + "','" + model.Purpose + "',");
                    query.Append(" TO_DATE('" + model.ReceivedDate + "','dd/MM/yyyy'),'" + model.ProposedBy + "','" + model.ProposedDepartment + "',");
                    query.Append(" TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy'), TO_DATE('" + model.ApprovalDate + "','dd/MM/yyyy')," + "'N','");
                    query.Append(userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')))");
                }
               isReturn= _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());

                 if (model.NocDetail != null)
                 {
                     foreach (NocDetailInfo objItem in model.NocDetail)
                     {
                         var qry = new StringBuilder();
                         if (objItem.ID == 0)
                         {
                             qry.Append(" INSERT INTO NOC_DTL(ID,ND_ID,ITEM_SLNO, ITEM_NAME, QUANTITY, IS_DELETE, SET_BY, SET_ON) ");
                             qry.Append(" VALUES( '" + _idGenerated.getMAXSL("NOC_DTL", "ID") + "','" + ReturnMaxID + "','" + objItem.ItemSlNo + "','" + objItem.ItemName + "','" + objItem.ItemQty + "','N',");                            
                             qry.Append(" '" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')))");
                         }
                         else
                         {
                             qry.Append(" UPDATE NOC_DTL SET");
                             qry.Append(" ITEM_SLNO='" + objItem.ItemSlNo + "',");
                             qry.Append(" ITEM_NAME='" + objItem.ItemName + "',");
                             qry.Append(" QUANTITY='" + objItem.ItemQty + "',");
                             qry.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')), ");
                             qry.Append(" UPDATE_BY='" + userId + "'");
                             qry.Append(" WHERE ID=" + objItem.ID );
                         }
                        isReturn= _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), qry.ToString());
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

                string deleteQuery = "DELETE FROM NOC_DTL WHERE ID = '" + id + "'";
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


        public DataTable GetAllNocWithDetailForReport(ReportModel model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT ROW_NUMBER() OVER(ORDER BY D.ID)SN, A.ID,A.SLNO,A.COMPANY_CODE, B.COMPANY_NAME,B.ADDRESS,A.LICENSE_NO,A.INVICE_NO INVOICE_NO, ");
            query.Append(" TO_CHAR(A.INVOICE_DATE, 'dd/MM/yyyy')INVOICE_DATE,A.COUNTRY_OF_ORIGNIN,A.PURPOSE,TO_CHAR(A.SUBMISSION_DATE, 'dd/MM/yyyy')SUBMISSION_DATE,TO_CHAR(A.APPROVAL_DATE, 'dd/MM/yyyy')APPROVAL_DATE,D.ITEM_SLNO, D.ITEM_NAME, D.QUANTITY");
            query.Append(" FROM NOC_MST A LEFT JOIN COMPANY_INFO B ON A.COMPANY_CODE = B.COMPANY_CODE LEFT JOIN NOC_DTL D  ON A.ID=D.ND_ID");
            query.Append(" WHERE 1=1 ");

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND B.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.InvoiceNo))
            {
                query.Append(" AND A.INVICE_NO='{1}'");
            }
            if (!string.IsNullOrEmpty(model.Department))
            {
                query.Append(" AND A.PROPOSED_DEPT='{2}'");
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
                        query.Append(" AND A.INVOICE_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                {
                    query.Append(" AND (A.SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR A.INVOICE_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                    query.Append(" OR A.APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                }
            }

            //if (!string.IsNullOrEmpty(orderBy))
            //{
            //    query.Append(" ORDER BY  E.ID " + orderBy);
            //}
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.InvoiceNo, model.Department));

            return dt;
        }
    }
}