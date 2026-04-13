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

    public class BlockListDAO : ReturnData
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;
        public BlockListDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }

        public bool SaveUpdate(BlockListBEL model, string userId)
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
                    query.Append(" UPDATE BLOCK_LIST SET COMPANY_CODE='" + model.CompanyCode + "', PROPOSED_BY='" + model.ProposedBy + "',BLOCK_LIST_NO='" + model.BLNo + "', REMARKS='" + model.Remarks + "',");
                    query.Append(" BLOCK_LIST_DATE =(TO_DATE('" + model.BlockListDate + "','dd/MM/yyyy')), PROPOSAL_DATE =(TO_DATE('" + model.ProposedDate + "','dd/MM/yyyy')),");
                    query.Append(" MEETING_DATE =(TO_DATE('" + model.MeetingDate + "','dd/MM/yyyy')), APPROVAL_DATE =(TO_DATE('" + model.ApprovalDate + "','dd/MM/yyyy')), APPORVAL_NO ='" + model.ApprovalNo + "',");
                    query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                    query.Append(" WHERE ID='" + model.ID + "'");
                }
                else
                { //I for Insert  
                    ReturnMaxID = _idGenerated.getMAXSL("BLOCK_LIST", "ID");
                    MaxID = _idGenerated.getMAXID("BLOCK_LIST", "SLNO", "fm000000000");
                    string strCount = _dbHelper.GetValue("SELECT COUNT(1) AS RevisionNo FROM BLOCK_LIST  WHERE IS_DELETE='N' AND COMPANY_CODE='" + model.CompanyCode + "' GROUP BY COMPANY_CODE");
                    if (!string.IsNullOrEmpty(strCount))
                    {
                        RefNo = strCount;
                        //model.SubmissionType = "Renewal";
                    }
                    else
                    {
                        RefNo = "0";
                        //model.SubmissionType = "License";
                    }

                    IUMode = "I";
                    query.Append(" INSERT INTO BLOCK_LIST(ID,SLNO,COMPANY_CODE,BLOCK_LIST_NO,REVISION_NO,PROPOSED_BY,BLOCK_LIST_DATE,PROPOSAL_DATE,MEETING_DATE,APPROVAL_DATE,APPORVAL_NO,REMARKS,SET_BY,SET_ON,IS_DELETE) ");
                    query.Append(" VALUES( '" + ReturnMaxID + "','" + MaxID + "','" + model.CompanyCode + "','" + model.BLNo + "','" + RefNo + "','" + model.ProposedBy + "',(TO_DATE('" + model.BlockListDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ProposedDate + "','dd/MM/yyyy')),(TO_DATE('" + model.MeetingDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ApprovalDate + "','dd/MM/yyyy')),");
                    query.Append(" '" + model.ApprovalNo + "','" + model.Remarks + "','" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss'))");
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

        public IList<BlockListBEL> GetAllInfo(BlockListBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT CL.ID, CL.SLNO,CL.REVISION_NO,CL.COMPANY_CODE,CL.BLOCK_LIST_NO,CL.PROPOSED_BY,TO_CHAR(CL.BLOCK_LIST_DATE, 'dd/mm/yyyy')BLOCK_LIST_DATE ,");
            query.Append(" TO_CHAR(CL.PROPOSAL_DATE, 'dd/mm/yyyy')PROPOSAL_DATE,TO_CHAR(CL.MEETING_DATE, 'dd/mm/yyyy')MEETING_DATE,TO_CHAR(CL.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE,CL.APPORVAL_NO,CL.REMARKS,TO_CHAR(CL.SET_ON, 'dd/mm/yyyy')SET_ON,");
            query.Append(" C.COMPANY_NAME,C.ADDRESS");
            query.Append(" FROM BLOCK_LIST CL");
            query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=CL.COMPANY_CODE");
            query.Append(" WHERE IS_DELETE <>'Y'");

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND  CL.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.BLNo))
            {
                query.Append(" AND  CL.BLOCK_LIST_NO='{1}'");
            }
            if (!string.IsNullOrEmpty(model.ProposedBy))
            {
                query.Append(" AND  CL.PROPOSED_BY='{2}'");
            }
            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND CL.BLOCK_LIST_DATE BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  CL.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.BLNo, model.ProposedBy));

            var item = (from DataRow row in dt.Rows
                        select new BlockListBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            Address = row["ADDRESS"].ToString(),
                            BLNo = row["BLOCK_LIST_NO"].ToString(),
                            ProposedBy = row["PROPOSED_BY"].ToString(),
                            BlockListDate = row["BLOCK_LIST_DATE"].ToString(),
                            ProposedDate = row["PROPOSAL_DATE"].ToString(),
                            MeetingDate = row["MEETING_DATE"].ToString(),
                            ApprovalDate = row["APPROVAL_DATE"].ToString(),
                            ApprovalNo = row["APPORVAL_NO"].ToString(),
                            Remarks = row["REMARKS"].ToString(),
                            SetOn = row["SET_ON"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString()
                        }).ToList();
            return item;
        }
    }
}