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
    public class NarcoticLicenseDAO : ReturnData
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;
        public NarcoticLicenseDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }

        public bool SaveUpdate(NarcoticLicenseBEL model, string userId)
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
                    query.Append(" UPDATE NARCOTIC_LICENSE SET COMPANY_CODE='" + model.CompanyCode + "', LICENSE_NO='" + model.LicenseNo + "',SUBMISSION_TYPE='" + model.SubmissionType + "',LICENSE_TYPE='" + model.LicenseType + "',");
                    query.Append(" SUBMISSION_DATE =(TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')), INSPECTION_DATE =(TO_DATE('" + model.InspectionDate + "','dd/MM/yyyy')),");
                    query.Append(" VALID_UPTO =(TO_DATE('" + model.ValidUpto + "','dd/MM/yyyy')), APPROVAL_DATE =(TO_DATE('" + model.ApprovalDate + "','dd/MM/yyyy')), NOTIFICATION_DAYS ='" + model.AlarmDays + "',");
                    query.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),UPDATE_BY='" + userId + "'");
                    query.Append(" WHERE ID='" + model.ID + "'");
                }
                else
                { //I for Insert  
                    ReturnMaxID = _idGenerated.getMAXSL("NARCOTIC_LICENSE", "ID");
                    MaxID = _idGenerated.getMAXID("NARCOTIC_LICENSE", "SLNO", "fm000000000");
                    string strCount = _dbHelper.GetValue("SELECT COUNT(1) AS RevisionNo FROM NARCOTIC_LICENSE  WHERE IS_DELETE='N' AND COMPANY_CODE='" + model.CompanyCode + "' GROUP BY COMPANY_CODE,LICENSE_TYPE");
                    if (!string.IsNullOrEmpty(strCount))
                    {
                        RefNo = strCount;
                        model.SubmissionType = "Renewal";
                    }
                    else
                    {
                        RefNo = "0";
                        model.SubmissionType = "License";
                    }

                    IUMode = "I";
                    query.Append(" INSERT INTO NARCOTIC_LICENSE(ID,SLNO,COMPANY_CODE,LICENSE_NO,REVISION_NO,LICENSE_TYPE,SUBMISSION_TYPE,SUBMISSION_DATE,INSPECTION_DATE,VALID_UPTO,APPROVAL_DATE,NOTIFICATION_DAYS,SET_BY,SET_ON,IS_DELETE) ");
                    query.Append(" VALUES( '" + ReturnMaxID + "','" + MaxID + "','" + model.CompanyCode + "','" + model.LicenseNo + "','" + RefNo + "','" + model.LicenseType + "','" + model.SubmissionType + "',(TO_DATE('" + model.SubmissionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.InspectionDate + "','dd/MM/yyyy')),(TO_DATE('" + model.ValidUpto + "','dd/MM/yyyy')),(TO_DATE('" + model.ApprovalDate + "','dd/MM/yyyy')),");
                    query.Append(" '" + model.AlarmDays + "','" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss'))");
                    query.Append(" ,'N')");
                }
                _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), query.ToString());

                if (model.NLDetail != null)
                {

                    foreach (var objItem in model.NLDetail)
                    {
                        var qry = new StringBuilder();
                        if (objItem.ID == 0)
                        {
                            qry.Append(" INSERT INTO NARCOTIC_LICENSE_DETAIL(ID,NL_ID,GENERIC_CODE,ANNUAL_QUOTA,IMPORT_QTY_PRE_YR,IMPORT_QTY_CUR_YR,IMPORT_PURPOSE,IMPORT_QTY,PERMIT_APPROVAL_DATE,IMPORT_DATE,SUB_DGDA,APPROVE_DGDA,SUB_NERCOTIC,SET_BY,SET_ON, ");
                            qry.Append(" BRAND_NAME,DTL_REMARKS,RECV_SENT_NARC,INST_RPT_RECV_NARC,SUB_INST_RPT_NARC,SUB_INST_RPT_NHQ,FINAL_IMP_PERMIT,RM_IMPORT_QTY,SEND_TO_PPIC,PPIC_LOCAL_APP,INST_SAMPLE_CALL,SAMPLE_RECV_NARC,RPT_DISPATCH,PPIC_SENT,PPIC_APPLY_NARC,RPT_FORWARDING_RECV,DIV_TO_DNC,FINAL_PERMIT,DELIVERED_TO_IMD) ");
                            qry.Append(" VALUES( '" + _idGenerated.getMAXSL("NARCOTIC_LICENSE_DETAIL", "ID") + "','" + ReturnMaxID + "','" + objItem.GenericCode + "','" + objItem.AnnualQuota + "','" + objItem.ImportQtyPerYr + "','" + objItem.ImportQtyCurYr + "','" + objItem.ImportPurpose + "','" + objItem.ImportQty + "',");
                            qry.Append(" (TO_DATE('" + Convert.ToDateTime(objItem.PermitApprovalDate).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.ImportDate).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),");
                            qry.Append(" (TO_DATE('" + Convert.ToDateTime(objItem.SubDGDA).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.ApproveDGDA).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),");
                            qry.Append(" '" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')) ");
                            qry.Append(" ,'" + objItem.BrandName + "', '" + objItem.DtlRemarks + "', (TO_DATE('" + Convert.ToDateTime(objItem.RecSentNarc).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.InsRptRcvNarc).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubInsRptNhq).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                            qry.Append(",(TO_DATE('" + Convert.ToDateTime(objItem.SubInsRptNarc).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.FinalImpPermit).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.RMImpQty).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                            qry.Append(",(TO_DATE('" + Convert.ToDateTime(objItem.SendToPPIC).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.PPICLocalApp).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.InsSampleCall).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                            qry.Append(",(TO_DATE('" + Convert.ToDateTime(objItem.SampleRec).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.RPTDispatch).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.PPICSent).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                            qry.Append(",(TO_DATE('" + Convert.ToDateTime(objItem.PPICApplyNarc).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.RptForwrdRcv).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.DivToDnc).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                            qry.Append(",(TO_DATE('" + Convert.ToDateTime(objItem.FinalPermit).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.DeliverToIMD).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')))");
                            //qry.Append(",(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),,'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                            //qry.Append(",(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),,'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                            //qry.Append(",(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),,'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                           // qry.Append(",(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),,'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                           // qry.Append(",(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),,'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                            //qry.Append(",(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),,'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                            //qry.Append(",'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),,'dd/MM/yyyy')),(TO_DATE('" + Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'))");
                            _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), qry.ToString());
                        }
                        else
                        {
                            objItem.PermitApprovalDate = string.IsNullOrEmpty(objItem.PermitApprovalDate) || objItem.PermitApprovalDate.Contains("Z") ? Convert.ToDateTime(objItem.PermitApprovalDate).Date.ToString("dd/MM/yyyy") : objItem.PermitApprovalDate;
                            objItem.ImportDate = string.IsNullOrEmpty(objItem.ImportDate) || objItem.ImportDate.Contains("Z") ? Convert.ToDateTime(objItem.ImportDate).Date.ToString("dd/MM/yyyy") : objItem.ImportDate;

                            objItem.SubDGDA = string.IsNullOrEmpty(objItem.SubDGDA) || objItem.SubDGDA.Contains("Z") ? Convert.ToDateTime(objItem.SubDGDA).Date.ToString("dd/MM/yyyy") : objItem.SubDGDA;
                            objItem.ApproveDGDA = string.IsNullOrEmpty(objItem.ApproveDGDA) || objItem.ApproveDGDA.Contains("Z") ? Convert.ToDateTime(objItem.ApproveDGDA).Date.ToString("dd/MM/yyyy") : objItem.ApproveDGDA;
                            objItem.SubNercotic = string.IsNullOrEmpty(objItem.SubNercotic) || objItem.SubNercotic.Contains("Z") ? Convert.ToDateTime(objItem.SubNercotic).Date.ToString("dd/MM/yyyy") : objItem.SubNercotic;
                            objItem.RecSentNarc = string.IsNullOrEmpty(objItem.RecSentNarc) || objItem.RecSentNarc.Contains("Z") ? Convert.ToDateTime(objItem.RecSentNarc).Date.ToString("dd/MM/yyyy") : objItem.RecSentNarc;
                            objItem.InsRptRcvNarc = string.IsNullOrEmpty(objItem.InsRptRcvNarc) || objItem.InsRptRcvNarc.Contains("Z") ? Convert.ToDateTime(objItem.InsRptRcvNarc).Date.ToString("dd/MM/yyyy") : objItem.InsRptRcvNarc;
                            objItem.SubInsRptNhq = string.IsNullOrEmpty(objItem.SubInsRptNhq) || objItem.SubInsRptNhq.Contains("Z") ? Convert.ToDateTime(objItem.SubInsRptNhq).Date.ToString("dd/MM/yyyy") : objItem.SubInsRptNhq;
                            objItem.SubInsRptNarc = string.IsNullOrEmpty(objItem.SubInsRptNarc) || objItem.SubInsRptNarc.Contains("Z") ? Convert.ToDateTime(objItem.SubInsRptNarc).Date.ToString("dd/MM/yyyy") : objItem.SubInsRptNarc;
                            objItem.FinalImpPermit = string.IsNullOrEmpty(objItem.FinalImpPermit) || objItem.FinalImpPermit.Contains("Z") ? Convert.ToDateTime(objItem.FinalImpPermit).Date.ToString("dd/MM/yyyy") : objItem.FinalImpPermit;
                            objItem.RMImpQty = string.IsNullOrEmpty(objItem.RMImpQty) || objItem.RMImpQty.Contains("Z") ? Convert.ToDateTime(objItem.RMImpQty).Date.ToString("dd/MM/yyyy") : objItem.RMImpQty;
                            objItem.SendToPPIC = string.IsNullOrEmpty(objItem.SendToPPIC) || objItem.SendToPPIC.Contains("Z") ? Convert.ToDateTime(objItem.SendToPPIC).Date.ToString("dd/MM/yyyy") : objItem.SendToPPIC;
                            objItem.PPICLocalApp = string.IsNullOrEmpty(objItem.PPICLocalApp) || objItem.PPICLocalApp.Contains("Z") ? Convert.ToDateTime(objItem.PPICLocalApp).Date.ToString("dd/MM/yyyy") : objItem.PPICLocalApp;
                            objItem.InsSampleCall = string.IsNullOrEmpty(objItem.InsSampleCall) || objItem.InsSampleCall.Contains("Z") ? Convert.ToDateTime(objItem.InsSampleCall).Date.ToString("dd/MM/yyyy") : objItem.InsSampleCall;
                            objItem.SampleRec = string.IsNullOrEmpty(objItem.SampleRec) || objItem.SampleRec.Contains("Z") ? Convert.ToDateTime(objItem.SampleRec).Date.ToString("dd/MM/yyyy") : objItem.SampleRec;
                            objItem.RPTDispatch = string.IsNullOrEmpty(objItem.RPTDispatch) || objItem.RPTDispatch.Contains("Z") ? Convert.ToDateTime(objItem.RPTDispatch).Date.ToString("dd/MM/yyyy") : objItem.RPTDispatch;
                            objItem.PPICSent = string.IsNullOrEmpty(objItem.PPICSent) || objItem.PPICSent.Contains("Z") ? Convert.ToDateTime(objItem.PPICSent).Date.ToString("dd/MM/yyyy") : objItem.PPICSent;
                            objItem.PPICApplyNarc = string.IsNullOrEmpty(objItem.PPICApplyNarc) || objItem.PPICApplyNarc.Contains("Z") ? Convert.ToDateTime(objItem.PPICApplyNarc).Date.ToString("dd/MM/yyyy") : objItem.PPICApplyNarc;
                            objItem.RptForwrdRcv = string.IsNullOrEmpty(objItem.RptForwrdRcv) || objItem.RptForwrdRcv.Contains("Z") ? Convert.ToDateTime(objItem.RptForwrdRcv).Date.ToString("dd/MM/yyyy") : objItem.RptForwrdRcv;
                            objItem.DivToDnc = string.IsNullOrEmpty(objItem.DivToDnc) || objItem.DivToDnc.Contains("Z") ? Convert.ToDateTime(objItem.DivToDnc).Date.ToString("dd/MM/yyyy") : objItem.DivToDnc;
                            objItem.FinalPermit = string.IsNullOrEmpty(objItem.FinalPermit) || objItem.FinalPermit.Contains("Z") ? Convert.ToDateTime(objItem.FinalPermit).Date.ToString("dd/MM/yyyy") : objItem.FinalPermit;
                            objItem.DeliverToIMD = string.IsNullOrEmpty(objItem.DeliverToIMD) || objItem.DeliverToIMD.Contains("Z") ? Convert.ToDateTime(objItem.DeliverToIMD).Date.ToString("dd/MM/yyyy") : objItem.DeliverToIMD;

                            string qry1 = "";
                            qry1 = "UPDATE NARCOTIC_LICENSE_DETAIL SET GENERIC_CODE='" + objItem.GenericCode + "',BRAND_NAME='" + objItem.BrandName + "',DTL_REMARKS='" + objItem.DtlRemarks + "', ANNUAL_QUOTA='" + objItem.AnnualQuota + "',IMPORT_QTY_PRE_YR='" + objItem.ImportQtyPerYr + "',IMPORT_QTY_CUR_YR='" + objItem.ImportQtyCurYr + "'," +
                            " IMPORT_PURPOSE='" + objItem.ImportPurpose + "',IMPORT_QTY='" + objItem.ImportQty + "',"+
                            " PERMIT_APPROVAL_DATE =(TO_DATE('" + objItem.PermitApprovalDate + "','dd/MM/yyyy')), IMPORT_DATE =(TO_DATE('" + objItem.ImportDate + "','dd/MM/yyyy')),"+
                            " SUB_DGDA =(TO_DATE('" + objItem.SubDGDA + "','dd/MM/yyyy')), APPROVE_DGDA =(TO_DATE('" + objItem.ApproveDGDA + "','dd/MM/yyyy')),SUB_NERCOTIC =(TO_DATE('" + objItem.SubNercotic + "','dd/MM/yyyy')),"+
                            " RECV_SENT_NARC =(TO_DATE('" + objItem.RecSentNarc + "','dd/MM/yyyy')), INST_RPT_RECV_NARC =(TO_DATE('" + objItem.InsRptRcvNarc + "','dd/MM/yyyy')),SUB_INST_RPT_NHQ =(TO_DATE('" + objItem.SubInsRptNhq + "','dd/MM/yyyy')),"+
                            " SUB_INST_RPT_NARC =(TO_DATE('" + objItem.SubInsRptNarc + "','dd/MM/yyyy')), FINAL_IMP_PERMIT =(TO_DATE('" + objItem.FinalImpPermit + "','dd/MM/yyyy')),RM_IMPORT_QTY =(TO_DATE('" + objItem.RMImpQty + "','dd/MM/yyyy')),"+
                            " SEND_TO_PPIC =(TO_DATE('" + objItem.SendToPPIC + "','dd/MM/yyyy')), PPIC_LOCAL_APP =(TO_DATE('" + objItem.PPICLocalApp + "','dd/MM/yyyy')),INST_SAMPLE_CALL =(TO_DATE('" + objItem.InsSampleCall + "','dd/MM/yyyy')),"+
                            " SAMPLE_RECV_NARC =(TO_DATE('" + objItem.SampleRec + "','dd/MM/yyyy')), RPT_DISPATCH =(TO_DATE('" + objItem.RPTDispatch + "','dd/MM/yyyy')),PPIC_SENT =(TO_DATE('" + objItem.PPICSent + "','dd/MM/yyyy')),"+
                            " PPIC_APPLY_NARC =(TO_DATE('" + objItem.PPICApplyNarc + "','dd/MM/yyyy')), RPT_FORWARDING_RECV =(TO_DATE('" + objItem.RptForwrdRcv + "','dd/MM/yyyy')),DIV_TO_DNC =(TO_DATE('" + objItem.DivToDnc + "','dd/MM/yyyy')),"+
                            " FINAL_PERMIT =(TO_DATE('" + objItem.FinalPermit + "','dd/MM/yyyy')), DELIVERED_TO_IMD =(TO_DATE('" + objItem.DeliverToIMD + "','dd/MM/yyyy')),"+
                            " UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),"+
                            " UPDATE_BY='" + userId + "'"+
                            " WHERE ID ="+objItem.DetailID+"";

                            _dbHelper.CmdExecute(_dbConn.SAConnStrReader(), qry1.ToString());
                        }
                        

                    }

                }
                
                return true;
            }
            catch (Exception errorException)
            {
                return false;
            }
        }

        public IList<NarcoticLicenseBEL> GetAllInfo(NarcoticLicenseBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT D.ID, D.SLNO,D.REVISION_NO,D.COMPANY_CODE,D.LICENSE_NO,D.LICENSE_TYPE,D.SUBMISSION_TYPE,TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE ,");
            query.Append(" TO_CHAR(D.INSPECTION_DATE, 'dd/mm/yyyy')INSPECTION_DATE,TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE,D.NOTIFICATION_DAYS,TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,");
            query.Append(" C.COMPANY_NAME,C.ADDRESS");
            query.Append(" FROM NARCOTIC_LICENSE D");
            query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE");
            query.Append(" WHERE D.IS_DELETE <>'Y'");
            //query.Append(" SELECT A.ID,A.SLNO,A.REVISION_NO,A.COMPANY_CODE,A.LICENSE_NO, A.LICENSE_TYPE,A.SUBMISSION_TYPE,TO_CHAR (A.SUBMISSION_DATE, 'dd/mm/yyyy') SUBMISSION_DATE,TO_CHAR (A.INSPECTION_DATE, 'dd/mm/yyyy') INSPECTION_DATE,");
            //query.Append(" TO_CHAR (A.VALID_UPTO, 'dd/mm/yyyy') VALID_UPTO,TO_CHAR (A.APPROVAL_DATE, 'dd/mm/yyyy') APPROVAL_DATE,A.NOTIFICATION_DAYS,TO_CHAR (A.SET_ON, 'dd/mm/yyyy') SET_ON,C.COMPANY_NAME,C.ADDRESS,");
            //query.Append(" B.ANNUAL_QUOTA,B.APPROVE_DGDA,B.BRAND_NAME,B.DELIVERED_TO_IMD,B.DIV_TO_DNC,B.FINAL_IMP_PERMIT,B.FINAL_PERMIT,B.GENERIC_CODE,B.IMPORT_DATE,B.IMPORT_PURPOSE,B.IMPORT_QTY,B.IMPORT_QTY_CUR_YR,");
            //query.Append(" B.IMPORT_QTY_PRE_YR,B.INST_RPT_RECV_NARC,B.INST_SAMPLE_CALL,B.IS_DELETE,B.NL_ID,B.PERMIT_APPROVAL_DATE,B.PPIC_APPLY_NARC,B.PPIC_LOCAL_APP,B.PPIC_SENT,B.RECV_SENT_NARC,B.RM_IMPORT_QTY,");
            //query.Append(" B.RPT_DISPATCH,B.RPT_FORWARDING_RECV,B.SAMPLE_RECV_NARC,B.SEND_TO_PPIC,B.SUB_DGDA,B.SUB_INST_RPT_NARC,B.SUB_INST_RPT_NHQ,B.SUB_NERCOTIC FROM NARCOTIC_LICENSE A");
            //query.Append(" LEFT JOIN COMPANY_INFO C ON C.COMPANY_CODE = A.COMPANY_CODE");
            //query.Append(" LEFT JOIN NARCOTIC_LICENSE_DETAIL B ON A.ID = B.NL_ID");
            //query.Append(" WHERE A.IS_DELETE <> 'Y'");
            //query.Append("");
                
            
            
            
            
            
            
            
            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND  D.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.LicenseNo))
            {
                query.Append(" AND  D.LICENSE_NO='{1}'");
            }
            if (!string.IsNullOrEmpty(model.SubmissionType) && !model.SubmissionType.Equals("All"))
            {
                query.Append(" AND  D.SUBMISSION_TYPE='{2}'");
            }
            if (!string.IsNullOrEmpty(model.LicenseType) && !model.LicenseType.Equals("All"))
            {
                query.Append(" AND  D.LICENSE_TYPE='{3}'");
            }
            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND TO_DATE(D.SUBMISSION_DATE,'dd/mm/yyyy') BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY D.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.LicenseNo, model.SubmissionType, model.LicenseType));

            var item = (from DataRow row in dt.Rows
                        select new NarcoticLicenseBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            Address = row["ADDRESS"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            LicenseType = row["LICENSE_TYPE"].ToString(),
                            SubmissionType = row["SUBMISSION_TYPE"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            InspectionDate = row["INSPECTION_DATE"].ToString(),
                            ValidUpto = row["VALID_UPTO"].ToString(),
                            ApprovalDate = row["APPROVAL_DATE"].ToString(),
                            AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                            RevisionDate = row["SET_ON"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString(),

                            //GenericName = row["GENERIC_CODE"].ToString(),
                            //BrandName = row["BRAND_NAME"].ToString(),
                            //AnnualQuota = row["ANNUAL_QUOTA"].ToString(),
                            //SubDGDA = row["SUB_DGDA"].ToString(),
                            //ApproveDGDA = row["APPROVE_DGDA"].ToString(),
                            //SubNercotic = row["SUB_NERCOTIC"].ToString(),
                            //ImportQtyPerYr = row["IMPORT_QTY_PRE_YR"].ToString(),
                            //ImportQtyCurYr = row["IMPORT_QTY_CUR_YR"].ToString(),
                            //ImportPurpose = row["IMPORT_PURPOSE"].ToString(),
                            //PermitApprovalDate = row["PERMIT_APPROVAL_DATE"].ToString(),
                            //ImportDate = row["IMPORT_DATE"].ToString(),
                            //ImportQty = row["IMPORT_QTY"].ToString(),
                            //RecSentNarc = row["RECV_SENT_NARC"].ToString(),
                            //InsRptRcvNarc = row["INST_RPT_RECV_NARC"].ToString(),
                            //SubInsRptNhq = row["SUB_INST_RPT_NHQ"].ToString(),
                            //SubInsRptNarc = row["SUB_INST_RPT_NARC"].ToString(),
                            //FinalImpPermit = row["FINAL_IMP_PERMIT"].ToString(),
                            //RMImpQty = row["RM_IMPORT_QTY"].ToString(),
                            //SendToPPIC = row["SEND_TO_PPIC"].ToString(),
                            //PPICLocalApp = row["PPIC_LOCAL_APP"].ToString(),
                            //InsSampleCall = row["INST_SAMPLE_CALL"].ToString(),
                            //SampleRec = row["SAMPLE_RECV_NARC"].ToString(),
                            //RPTDispatch = row["RPT_DISPATCH"].ToString(),
                            //PPICSent = row["PPIC_SENT"].ToString(),
                            //PPICApplyNarc = row["PPIC_APPLY_NARC"].ToString(),
                            //RptForwrdRcv = row["RPT_FORWARDING_RECV"].ToString(),
                            //DivToDnc = row["DIV_TO_DNC"].ToString(),
                            //FinalPermit = row["FINAL_PERMIT"].ToString(),
                            //DeliverToIMD = row["DELIVERED_TO_IMD"].ToString(),
                            //ImportQtyPerYr = row["REVISION_NO"].ToString(),
                        }).ToList();
            return item;
        }
        public IList<NarcoticLicenseBEL> GetDetailGeneric(NarcoticLicenseBEL model, string orderBy)
        {
            var query = new StringBuilder();
            //query.Append(" SELECT D.ID, D.SLNO,D.REVISION_NO,D.COMPANY_CODE,D.LICENSE_NO,D.LICENSE_TYPE,D.SUBMISSION_TYPE,TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE ,");
            //query.Append(" TO_CHAR(D.INSPECTION_DATE, 'dd/mm/yyyy')INSPECTION_DATE,TO_CHAR(D.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE,D.NOTIFICATION_DAYS,TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON,");
            //query.Append(" C.COMPANY_NAME,C.ADDRESS");
            //query.Append(" FROM NARCOTIC_LICENSE D");
            //query.Append(" LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE");
            //query.Append(" WHERE D.IS_DELETE <>'Y'");
            //query.Append(" SELECT A.ID,A.SLNO,A.REVISION_NO,A.COMPANY_CODE,A.LICENSE_NO, A.LICENSE_TYPE,A.SUBMISSION_TYPE,TO_CHAR (A.SUBMISSION_DATE, 'dd/mm/yyyy') SUBMISSION_DATE,TO_CHAR (A.INSPECTION_DATE, 'dd/mm/yyyy') INSPECTION_DATE,");
            //query.Append(" TO_CHAR (A.VALID_UPTO, 'dd/mm/yyyy') VALID_UPTO,TO_CHAR (A.APPROVAL_DATE, 'dd/mm/yyyy') APPROVAL_DATE,A.NOTIFICATION_DAYS,TO_CHAR (A.SET_ON, 'dd/mm/yyyy') SET_ON,C.COMPANY_NAME,C.ADDRESS,");
            //query.Append(" SELECT A.ID,B.NL_ID,B.ANNUAL_QUOTA,TO_CHAR (B.APPROVE_DGDA, 'dd/mm/yyyy') APPROVE_DGDA,B.BRAND_NAME,TO_CHAR (B.DELIVERED_TO_IMD, 'dd/mm/yyyy')DELIVERED_TO_IMD,B.DIV_TO_DNC,B.FINAL_IMP_PERMIT,B.FINAL_PERMIT,B.GENERIC_CODE,B.IMPORT_DATE,B.IMPORT_PURPOSE,B.IMPORT_QTY,B.IMPORT_QTY_CUR_YR,");
            //query.Append(" B.IMPORT_QTY_PRE_YR,B.INST_RPT_RECV_NARC,B.INST_SAMPLE_CALL,B.IS_DELETE,B.NL_ID,B.PERMIT_APPROVAL_DATE,B.PPIC_APPLY_NARC,B.PPIC_LOCAL_APP,B.PPIC_SENT,B.RECV_SENT_NARC,B.RM_IMPORT_QTY,");
            //query.Append(" B.RPT_DISPATCH,B.RPT_FORWARDING_RECV,B.SAMPLE_RECV_NARC,B.SEND_TO_PPIC,B.SUB_DGDA,B.SUB_INST_RPT_NARC,B.SUB_INST_RPT_NHQ,B.SUB_NERCOTIC FROM NARCOTIC_LICENSE A");
            //query.Append(" LEFT JOIN COMPANY_INFO C ON C.COMPANY_CODE = A.COMPANY_CODE");
            //query.Append(" LEFT JOIN NARCOTIC_LICENSE_DETAIL B ON A.ID = B.NL_ID");
            //query.Append(" WHERE B.NL_ID=" + model.NLID + "  AND A.IS_DELETE <> 'Y'");
            //query.Append("Select * from NARCOTIC_LICENSE_DETAIL Where NL_ID=" + model.ID + "");



            query.Append(" SELECT A.ID,B.NL_ID,B.ID As Detail_ID,B.ANNUAL_QUOTA,B.GENERIC_CODE,B.BRAND_NAME, B.DTL_REMARKS ,B.IMPORT_PURPOSE,B.IMPORT_QTY,B.IMPORT_QTY_CUR_YR,B.IMPORT_QTY_PRE_YR,B.IS_DELETE,");
            query.Append(" TO_CHAR (B.APPROVE_DGDA, 'dd/mm/yyyy') APPROVE_DGDA,TO_CHAR (B.DELIVERED_TO_IMD, 'dd/mm/yyyy') DELIVERED_TO_IMD,TO_CHAR (B.DIV_TO_DNC, 'dd/mm/yyyy') DIV_TO_DNC,");
            query.Append(" TO_CHAR (B.FINAL_IMP_PERMIT, 'dd/mm/yyyy') FINAL_IMP_PERMIT,TO_CHAR (B.FINAL_PERMIT, 'dd/mm/yyyy') FINAL_PERMIT,TO_CHAR (B.IMPORT_DATE, 'dd/mm/yyyy') IMPORT_DATE,");
            query.Append(" TO_CHAR (B.INST_RPT_RECV_NARC, 'dd/mm/yyyy') INST_RPT_RECV_NARC,TO_CHAR (B.INST_SAMPLE_CALL, 'dd/mm/yyyy') INST_SAMPLE_CALL,TO_CHAR (B.PERMIT_APPROVAL_DATE, 'dd/mm/yyyy') PERMIT_APPROVAL_DATE,");
            query.Append(" TO_CHAR (B.PPIC_APPLY_NARC, 'dd/mm/yyyy') PPIC_APPLY_NARC,TO_CHAR (B.PPIC_LOCAL_APP, 'dd/mm/yyyy') PPIC_LOCAL_APP,TO_CHAR (B.PPIC_SENT, 'dd/mm/yyyy') PPIC_SENT,");
            query.Append(" TO_CHAR (B.RECV_SENT_NARC, 'dd/mm/yyyy') RECV_SENT_NARC,TO_CHAR (B.RM_IMPORT_QTY, 'dd/mm/yyyy') RM_IMPORT_QTY,TO_CHAR (B.RPT_DISPATCH, 'dd/mm/yyyy') RPT_DISPATCH,");
            query.Append(" TO_CHAR (B.RPT_FORWARDING_RECV, 'dd/mm/yyyy') RPT_FORWARDING_RECV,TO_CHAR (B.SAMPLE_RECV_NARC, 'dd/mm/yyyy') SAMPLE_RECV_NARC,TO_CHAR (B.SEND_TO_PPIC, 'dd/mm/yyyy') SEND_TO_PPIC,");
            query.Append(" TO_CHAR (B.SUB_DGDA, 'dd/mm/yyyy') SUB_DGDA,TO_CHAR (B.SUB_INST_RPT_NARC, 'dd/mm/yyyy') SUB_INST_RPT_NARC,TO_CHAR (B.SUB_INST_RPT_NHQ, 'dd/mm/yyyy') SUB_INST_RPT_NHQ,");
            query.Append(" TO_CHAR (B.SUB_INST_RPT_NARC, 'dd/mm/yyyy') SUB_INST_RPT_NARC,TO_CHAR (B.SUB_NERCOTIC, 'dd/mm/yyyy') SUB_NERCOTIC ");
            query.Append(" FROM NARCOTIC_LICENSE A LEFT JOIN COMPANY_INFO C ON C.COMPANY_CODE = A.COMPANY_CODE LEFT JOIN NARCOTIC_LICENSE_DETAIL B ON A.ID = B.NL_ID");
            query.Append(" WHERE B.NL_ID = " + model.ID + " AND A.IS_DELETE <> 'Y'");




            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND  D.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.LicenseNo))
            {
                query.Append(" AND  D.LICENSE_NO='{1}'");
            }
            if (!string.IsNullOrEmpty(model.SubmissionType) && !model.SubmissionType.Equals("All"))
            {
                query.Append(" AND  D.SUBMISSION_TYPE='{2}'");
            }
            if (!string.IsNullOrEmpty(model.LicenseType) && !model.LicenseType.Equals("All"))
            {
                query.Append(" AND  D.LICENSE_TYPE='{3}'");
            }
            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND TO_DATE(D.SUBMISSION_DATE,'dd/mm/yyyy') BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.LicenseNo, model.SubmissionType, model.LicenseType));

            var item = (from DataRow row in dt.Rows
                        select new NarcoticLicenseBEL
                        {
                             ID = Convert.ToInt64(row["ID"]),
                             DetailID = Convert.ToInt64(row["Detail_ID"]),
                            //SlNo = row["SLNO"].ToString(),
                            //CompanyCode = row["COMPANY_CODE"].ToString(),
                            //CompanyName = row["COMPANY_NAME"].ToString(),
                            //Address = row["ADDRESS"].ToString(),
                            //LicenseNo = row["LICENSE_NO"].ToString(),
                            //LicenseType = row["LICENSE_TYPE"].ToString(),
                            //SubmissionType = row["SUBMISSION_TYPE"].ToString(),
                            //SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            //InspectionDate = row["INSPECTION_DATE"].ToString(),
                            //ValidUpto = row["VALID_UPTO"].ToString(),
                            //ApprovalDate = row["APPROVAL_DATE"].ToString(),
                            //AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                            //RevisionDate = row["SET_ON"].ToString(),
                            //RevisionNo = row["REVISION_NO"].ToString(),
                             NLID = Convert.ToInt64(row["NL_ID"]),
                            GenericCode = row["GENERIC_CODE"].ToString(),
                            BrandName = row["BRAND_NAME"].ToString(),
                            DtlRemarks = row["DTL_REMARKS"].ToString(),
                            AnnualQuota = row["ANNUAL_QUOTA"].ToString(),
                            SubDGDA = row["SUB_DGDA"].ToString(),
                            ApproveDGDA = row["APPROVE_DGDA"].ToString(),
                            SubNercotic = row["SUB_NERCOTIC"].ToString(),
                            ImportQtyPerYr = row["IMPORT_QTY_PRE_YR"].ToString(),
                            ImportQtyCurYr = row["IMPORT_QTY_CUR_YR"].ToString(),
                            ImportPurpose = row["IMPORT_PURPOSE"].ToString(),
                            PermitApprovalDate = row["PERMIT_APPROVAL_DATE"].ToString(),
                            ImportDate = row["IMPORT_DATE"].ToString(),
                            ImportQty = row["IMPORT_QTY"].ToString(),
                            RecSentNarc = row["RECV_SENT_NARC"].ToString(),
                            InsRptRcvNarc = row["INST_RPT_RECV_NARC"].ToString(),
                            SubInsRptNhq = row["SUB_INST_RPT_NHQ"].ToString(),
                            SubInsRptNarc = row["SUB_INST_RPT_NARC"].ToString(),
                            FinalImpPermit = row["FINAL_IMP_PERMIT"].ToString(),
                            RMImpQty = row["RM_IMPORT_QTY"].ToString(),
                            SendToPPIC = row["SEND_TO_PPIC"].ToString(),
                            PPICLocalApp = row["PPIC_LOCAL_APP"].ToString(),
                            InsSampleCall = row["INST_SAMPLE_CALL"].ToString(),
                            SampleRec = row["SAMPLE_RECV_NARC"].ToString(),
                            RPTDispatch = row["RPT_DISPATCH"].ToString(),
                            PPICSent = row["PPIC_SENT"].ToString(),
                            PPICApplyNarc = row["PPIC_APPLY_NARC"].ToString(),
                            RptForwrdRcv = row["RPT_FORWARDING_RECV"].ToString(),
                            DivToDnc = row["DIV_TO_DNC"].ToString(),
                            FinalPermit = row["FINAL_PERMIT"].ToString(),
                            DeliverToIMD = row["DELIVERED_TO_IMD"].ToString(),
                            //ImportQtyPerYr = row["REVISION_NO"].ToString(),
                        }).ToList();
            return item;
        }

        public IList<NarcoticLicenseDetailBEL> GetAll(string id, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT D.ID, D.GENERIC_CODE,D.ANNUAL_QUOTA,D.IMPORT_QTY_PRE_YR,D.IMPORT_QTY_CUR_YR,D.IMPORT_PURPOSE,D.IMPORT_QTY,DECODE(TO_CHAR(D.PERMIT_APPROVAL_DATE,'dd/MM/yyyy'),'01/01/0001','',TO_CHAR(D.PERMIT_APPROVAL_DATE, 'dd/MM/yyyy'))PERMIT_APPROVAL_DATE ,");
            query.Append(" DECODE(TO_CHAR(D.IMPORT_DATE,'dd/MM/yyyy'),'01/01/0001','',TO_CHAR(D.IMPORT_DATE, 'dd/MM/yyyy'))IMPORT_DATE, G.GENERIC_NAME,");
            query.Append(" DECODE(TO_CHAR(D.SUB_DGDA,'dd/MM/yyyy'),'01/01/0001','',TO_CHAR(D.SUB_DGDA, 'dd/MM/yyyy'))SUB_DGDA, DECODE(TO_CHAR(D.APPROVE_DGDA,'dd/MM/yyyy'),'01/01/0001','',TO_CHAR(D.APPROVE_DGDA, 'dd/MM/yyyy'))APPROVE_DGDA,DECODE(TO_CHAR(D.SUB_NERCOTIC,'dd/MM/yyyy'),'01/01/0001','',TO_CHAR(D.SUB_NERCOTIC, 'dd/MM/yyyy'))SUB_NERCOTIC");
            query.Append(" FROM NARCOTIC_LICENSE_DETAIL D");
            query.Append(" LEFT JOIN  GENERIC_INFO G ON G.GENERIC_CODE=D.GENERIC_CODE");
            query.Append(" LEFT JOIN  NARCOTIC_LICENSE N ON N.ID=D.NL_ID");

            if (!string.IsNullOrEmpty(id))
            {
                query.Append(" WHERE  D.NL_ID={0}");
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY D.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), id));

            var item = (from DataRow row in dt.Rows
                        select new NarcoticLicenseDetailBEL
                        {

                            ID = Convert.ToInt64(row["ID"]),
                            GenericCode = row["GENERIC_CODE"].ToString(),
                            GenericName = row["GENERIC_NAME"].ToString(),
                            AnnualQuota = row["ANNUAL_QUOTA"].ToString(),
                            ImportQtyPerYr = row["IMPORT_QTY_PRE_YR"].ToString(),
                            ImportQtyCurYr = row["IMPORT_QTY_CUR_YR"].ToString(),
                            ImportPurpose = row["IMPORT_PURPOSE"].ToString(),
                            ImportQty = row["IMPORT_QTY"].ToString(),
                            PermitApprovalDate = row["PERMIT_APPROVAL_DATE"].ToString(),
                            ImportDate = row["IMPORT_DATE"].ToString(),
                            SubDGDA = row["SUB_DGDA"].ToString(),
                            ApproveDGDA = row["APPROVE_DGDA"].ToString(),
                            SubNercotic = row["SUB_NERCOTIC"].ToString()
                        }).ToList();
            return item;
        }

        public IList<NarcoticLicenseBEL> GetNarcoticExpireLicense(NarcoticLicenseBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT A.ID, A.SLNO,A.REVISION_NO,A.COMPANY_CODE,A.LICENSE_NO,A.SUBMISSION_TYPE,A.SUBMISSION_DATE ,");
            query.Append(" ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0)DateDiff,");
            query.Append(" A.INSPECTION_DATE,TO_CHAR(A.VALID_UPTO, 'dd/mm/yyyy')VALID_UPTO,A.APPROVAL_DATE,A.NOTIFICATION_DAYS,A.SET_ON, A.COMPANY_NAME,A.ADDRESS FROM");
            query.Append(" ( SELECT D.ID, D.SLNO,D.REVISION_NO,D.COMPANY_CODE,D.LICENSE_TYPE,D.LICENSE_NO,D.SUBMISSION_TYPE,");
            query.Append(" TO_CHAR(D.SUBMISSION_DATE, 'dd/mm/yyyy')SUBMISSION_DATE , TO_CHAR(D.INSPECTION_DATE, 'dd/mm/yyyy')INSPECTION_DATE,D.VALID_UPTO,");
            query.Append(" TO_CHAR(D.APPROVAL_DATE, 'dd/mm/yyyy')APPROVAL_DATE,D.NOTIFICATION_DAYS,TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON, C.COMPANY_NAME,C.ADDRESS");
            query.Append(" FROM NARCOTIC_LICENSE D LEFT JOIN  COMPANY_INFO C ON C.COMPANY_CODE=D.COMPANY_CODE WHERE IS_DELETE <>'Y' ");
            query.Append(" ) A INNER JOIN ( SELECT COMPANY_CODE,LICENSE_TYPE,MAX(REVISION_NO) AS MaxRvNo ");
            query.Append(" FROM NARCOTIC_LICENSE GROUP BY COMPANY_CODE) B");
            query.Append(" ON A.COMPANY_CODE=B.COMPANY_CODE AND A.LICENSE_TYPE=B.LICENSE_TYPE AND A.REVISION_NO=B.MaxRvNo");
            query.Append(" WHERE 1=1 ");

            if (!string.IsNullOrEmpty(model.CompanyCode))
            {
                query.Append(" AND  A.COMPANY_CODE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.SubmissionType) && !model.SubmissionType.Equals("All"))
            {
                query.Append(" AND  A.SUBMISSION_TYPE='{1}'");
            }
            if (!string.IsNullOrEmpty(model.AlarmDays))
            {
                query.Append(" AND ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0) <= {2}");
            }
            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND A.VALID_UPTO BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
            }
            if (string.IsNullOrEmpty(model.CompanyCode) && string.IsNullOrEmpty(model.AlarmDays) && string.IsNullOrEmpty(model.FromDate) && string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND ROUND(((NVL(A.VALID_UPTO, TO_DATE('12/31/2099', 'mm/dd/yyyy')))-(SELECT  SYSDATE FROM DUAL)),0) <= A.NOTIFICATION_DAYS ");
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  A.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.SubmissionType, model.AlarmDays));

            var item = (from DataRow row in dt.Rows
                        select new NarcoticLicenseBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            SlNo = row["SLNO"].ToString(),
                            CompanyCode = row["COMPANY_CODE"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            Address = row["ADDRESS"].ToString(),
                            LicenseNo = row["LICENSE_NO"].ToString(),
                            SubmissionType = row["SUBMISSION_TYPE"].ToString(),
                            SubmissionDate = row["SUBMISSION_DATE"].ToString(),
                            InspectionDate = row["INSPECTION_DATE"].ToString(),
                            ValidUpto = row["VALID_UPTO"].ToString(),
                            ApprovalDate = row["APPROVAL_DATE"].ToString(),
                            AlarmDays = row["NOTIFICATION_DAYS"].ToString(),
                            RevisionDate = row["SET_ON"].ToString(),
                            RevisionNo = row["REVISION_NO"].ToString(),
                            DateDiff = Convert.ToInt32(row["DateDiff"])
                        }).ToList();
            return item;
        }

        public bool DeleteDataItem(string id)
        {
            try
            {
                //var qry = new StringBuilder();
                //qry.Append(" UPDATE NARCOTIC_LICENSE_DETAIL SET IS_DELETE='Y',");
                //qry.Append(" UPDATE_DATE =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')),UPDATE_BY='" + userId + "'");
                //qry.Append(" WHERE ID='" + objItem.ID + "' ");

                string deleteQuery = "DELETE FROM NARCOTIC_LICENSE_DETAIL WHERE ID = '" + id + "'";
                if (_dbHelper.CmdExecute(_dbConn.SAConnStrReader(),deleteQuery))
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

        //internal object GetGenericList()
        //{
        //    throw new NotImplementedException();
        //}

        public List<ProductInfoBEL> GetGenericList()
        {
            string Qry = "SELECT GENERIC_CODE,BRAND_NAME,STATUS from PRODUCT_INFO";
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), Qry);
            List<ProductInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new ProductInfoBEL
                    {
                        GenericCode = row["GENERIC_CODE"].ToString(),
                        GenericName = row["GENERIC_CODE"].ToString(),
                        BrandName = row["BRAND_NAME"].ToString(),
                        Status = row["STATUS"].ToString()

                    }).ToList();
            return item;
        }

        public DataTable GetAllNarcoticsCheckList(ReportModel model)
        {
            DataTable dt = new DataTable();
            var query = new StringBuilder();
            try
            {
                switch (model.ReportName)
                {
                    case "ExportNarcotics":
                        //query.Append(" SELECT A.ID,A.COMPANY_CODE,A.LICENSE_TYPE,A.SUBMISSION_TYPE,A.SUBMISSION_DATE,A.APPROVAL_DATE,A.INSPECTION_DATE,");
                        //query.Append(" C.COMPANY_NAME, B.NL_ID,B.ANNUAL_QUOTA, B.GENERIC_CODE, B.BRAND_NAME, B.IMPORT_PURPOSE,B.IMPORT_QTY,B.IMPORT_QTY_CUR_YR,");
                        //query.Append(" B.IMPORT_QTY_PRE_YR, B.IS_DELETE,TO_CHAR (B.APPROVE_DGDA, 'dd/mm/yyyy') APPROVE_DGDA, TO_CHAR (B.DELIVERED_TO_IMD, 'dd/mm/yyyy') DELIVERED_TO_IMD,");
                        //query.Append(" TO_CHAR (B.DIV_TO_DNC, 'dd/mm/yyyy') DIV_TO_DNC,TO_CHAR (B.FINAL_IMP_PERMIT, 'dd/mm/yyyy') FINAL_IMP_PERMIT,");
                        //query.Append(" TO_CHAR (B.FINAL_PERMIT, 'dd/mm/yyyy') FINAL_PERMIT,TO_CHAR (B.IMPORT_DATE, 'dd/mm/yyyy') IMPORT_DATE,");
                        //query.Append(" TO_CHAR (B.INST_RPT_RECV_NARC, 'dd/mm/yyyy') INST_RPT_RECV_NARC,TO_CHAR (B.INST_SAMPLE_CALL, 'dd/mm/yyyy') INST_SAMPLE_CALL,");
                        //query.Append(" TO_CHAR (B.PERMIT_APPROVAL_DATE, 'dd/mm/yyyy') PERMIT_APPROVAL_DATE,TO_CHAR (B.PPIC_APPLY_NARC, 'dd/mm/yyyy') PPIC_APPLY_NARC,");
                        //query.Append(" TO_CHAR (B.PPIC_LOCAL_APP, 'dd/mm/yyyy') PPIC_LOCAL_APP,TO_CHAR (B.PPIC_SENT, 'dd/mm/yyyy') PPIC_SENT,");
                        //query.Append(" TO_CHAR (B.RECV_SENT_NARC, 'dd/mm/yyyy') RECV_SENT_NARC,TO_CHAR (B.RM_IMPORT_QTY, 'dd/mm/yyyy') RM_IMPORT_QTY,");
                        //query.Append(" TO_CHAR (B.RPT_DISPATCH, 'dd/mm/yyyy') RPT_DISPATCH,TO_CHAR (B.RPT_FORWARDING_RECV, 'dd/mm/yyyy') RPT_FORWARDING_RECV,");
                        //query.Append(" TO_CHAR (B.SAMPLE_RECV_NARC, 'dd/mm/yyyy') SAMPLE_RECV_NARC, TO_CHAR (B.SEND_TO_PPIC, 'dd/mm/yyyy') SEND_TO_PPIC,");
                        //query.Append(" TO_CHAR (B.SUB_DGDA, 'dd/mm/yyyy') SUB_DGDA,TO_CHAR (B.SUB_INST_RPT_NARC, 'dd/mm/yyyy') SUB_INST_RPT_NARC,");
                        //query.Append(" TO_CHAR (B.SUB_INST_RPT_NHQ, 'dd/mm/yyyy') SUB_INST_RPT_NHQ, TO_CHAR (B.SUB_INST_RPT_NARC, 'dd/mm/yyyy') SUB_INST_RPT_NARC,");
                        //query.Append(" TO_CHAR (B.SUB_NERCOTIC, 'dd/mm/yyyy') SUB_NERCOTIC FROM NARCOTIC_LICENSE A");
                        //query.Append(" LEFT JOIN COMPANY_INFO C ON C.COMPANY_CODE = A.COMPANY_CODE LEFT JOIN NARCOTIC_LICENSE_DETAIL B ON A.ID = B.NL_ID");
                        //query.Append(" WHERE A.IS_DELETE <> 'Y'  AND A.LICENSE_TYPE = 'Export'");
                        query.Append("SELECT * FROM VW_EXPORTNARCOTICSSTATUSRPT WHERE 1 = 1");
                        if (!string.IsNullOrEmpty(model.CompanyCode))
                        {
                            query.Append(" AND COMPANY_CODE ='" + model.CompanyCode + "'");
                        }
                        if (!string.IsNullOrEmpty(model.SubmissionType))
                        {
                            query.Append(" AND SUBMISSION_TYPE ='" + model.SubmissionType + "'");
                        }

                        if (!string.IsNullOrEmpty(model.ChooseOption))
                        {
                            if (model.ChooseOption == "SubmissionDate")
                            {
                                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                                {
                                    query.Append(" AND SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                                }
                            }
                            else if (model.ChooseOption == "ApprovalDate")
                            {
                                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                                {
                                    query.Append(" AND APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                                {
                                    query.Append(" AND INSPECTION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                            {
                                query.Append(" AND ( SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                                query.Append(" OR INSPECTION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                                query.Append(" OR APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                            }
                        }


                        query.Append("ORDER BY ID DESC");

                        break;
                    case "ImportNarcotics":
                        //query.Append(" SELECT A.ID,A.COMPANY_CODE,A.LICENSE_TYPE,A.SUBMISSION_TYPE,A.SUBMISSION_DATE,A.APPROVAL_DATE,A.INSPECTION_DATE,");
                        //query.Append(" C.COMPANY_NAME, B.NL_ID,B.ANNUAL_QUOTA, B.GENERIC_CODE, B.BRAND_NAME, B.IMPORT_PURPOSE,B.IMPORT_QTY,B.IMPORT_QTY_CUR_YR,");
                        //query.Append(" B.IMPORT_QTY_PRE_YR, B.IS_DELETE,TO_CHAR (B.APPROVE_DGDA, 'dd/mm/yyyy') APPROVE_DGDA, TO_CHAR (B.DELIVERED_TO_IMD, 'dd/mm/yyyy') DELIVERED_TO_IMD,");
                        //query.Append(" TO_CHAR (B.DIV_TO_DNC, 'dd/mm/yyyy') DIV_TO_DNC,TO_CHAR (B.FINAL_IMP_PERMIT, 'dd/mm/yyyy') FINAL_IMP_PERMIT,");
                        //query.Append(" TO_CHAR (B.FINAL_PERMIT, 'dd/mm/yyyy') FINAL_PERMIT,TO_CHAR (B.IMPORT_DATE, 'dd/mm/yyyy') IMPORT_DATE,");
                        //query.Append(" TO_CHAR (B.INST_RPT_RECV_NARC, 'dd/mm/yyyy') INST_RPT_RECV_NARC,TO_CHAR (B.INST_SAMPLE_CALL, 'dd/mm/yyyy') INST_SAMPLE_CALL,");
                        //query.Append(" TO_CHAR (B.PERMIT_APPROVAL_DATE, 'dd/mm/yyyy') PERMIT_APPROVAL_DATE,TO_CHAR (B.PPIC_APPLY_NARC, 'dd/mm/yyyy') PPIC_APPLY_NARC,");
                        //query.Append(" TO_CHAR (B.PPIC_LOCAL_APP, 'dd/mm/yyyy') PPIC_LOCAL_APP,TO_CHAR (B.PPIC_SENT, 'dd/mm/yyyy') PPIC_SENT,");
                        //query.Append(" TO_CHAR (B.RECV_SENT_NARC, 'dd/mm/yyyy') RECV_SENT_NARC,TO_CHAR (B.RM_IMPORT_QTY, 'dd/mm/yyyy') RM_IMPORT_QTY,");
                        //query.Append(" TO_CHAR (B.RPT_DISPATCH, 'dd/mm/yyyy') RPT_DISPATCH,TO_CHAR (B.RPT_FORWARDING_RECV, 'dd/mm/yyyy') RPT_FORWARDING_RECV,");
                        //query.Append(" TO_CHAR (B.SAMPLE_RECV_NARC, 'dd/mm/yyyy') SAMPLE_RECV_NARC, TO_CHAR (B.SEND_TO_PPIC, 'dd/mm/yyyy') SEND_TO_PPIC,");
                        //query.Append(" TO_CHAR (B.SUB_DGDA, 'dd/mm/yyyy') SUB_DGDA,TO_CHAR (B.SUB_INST_RPT_NARC, 'dd/mm/yyyy') SUB_INST_RPT_NARC,");
                        //query.Append(" TO_CHAR (B.SUB_INST_RPT_NHQ, 'dd/mm/yyyy') SUB_INST_RPT_NHQ, TO_CHAR (B.SUB_INST_RPT_NARC, 'dd/mm/yyyy') SUB_INST_RPT_NARC,");
                        //query.Append(" TO_CHAR (B.SUB_NERCOTIC, 'dd/mm/yyyy') SUB_NERCOTIC FROM NARCOTIC_LICENSE A");
                        //query.Append(" LEFT JOIN COMPANY_INFO C ON C.COMPANY_CODE = A.COMPANY_CODE LEFT JOIN NARCOTIC_LICENSE_DETAIL B ON A.ID = B.NL_ID");
                        //query.Append(" WHERE A.IS_DELETE <> 'Y'  AND A.LICENSE_TYPE = 'Import Permit'");
                        query.Append("SELECT * FROM VW_IMPORTNARCOTICSSTATUSRPT WHERE 1 = 1");
                        if (!string.IsNullOrEmpty(model.CompanyCode))
                        {
                            query.Append(" AND COMPANY_CODE ='" + model.CompanyCode + "'");
                        }
                        if (!string.IsNullOrEmpty(model.SubmissionType))
                        {
                            query.Append(" AND SUBMISSION_TYPE ='" + model.SubmissionType + "'");
                        }

                        if (!string.IsNullOrEmpty(model.ChooseOption))
                        {
                            if (model.ChooseOption == "SubmissionDate")
                            {
                                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                                {
                                    query.Append(" AND SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                                }
                            }
                            else if (model.ChooseOption == "ApprovalDate")
                            {
                                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                                {
                                    query.Append(" AND APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                                {
                                    query.Append(" AND INSPECTION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                            {
                                query.Append(" AND ( SUBMISSION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                                query.Append(" OR INSPECTION_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd') ");
                                query.Append(" OR APPROVAL_DATE BETWEEN TO_DATE('" + General.SetDateStrYYYYMMDD(model.FromDate) + "','yyyy/mm/dd') AND TO_DATE('" + General.SetDateStrYYYYMMDD(model.ToDate) + "','yyyy/mm/dd')) ");
                            }
                        }


                        query.Append("ORDER BY ID DESC");

                        break;
                }

                dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.CompanyCode, model.SubmissionType, model.ChooseOption));

                //dt = _dbHelper.GetDataTable(string.Format(query.ToString()));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
    }
}