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

    public class MeetingInfoDAO : ReturnData
    {
        private DBConnection _dbConn = null;
        private DBHelper _dbHelper = null;
        private IDGenerated _idGenerated = null;
        public MeetingInfoDAO()
        {
            _dbConn = new DBConnection();
            _dbHelper = new DBHelper();
            _idGenerated = new IDGenerated();
        }
        public bool SaveUpdate(MeetingInfoBEL model, string userId)
        {
            try
            {
                var query = new StringBuilder();
                if (model.ID > 0)
                {
                    //U for update
                    ReturnMaxID = model.ID;
                    IUMode = "U";
                    query.Append(" UPDATE MEETING_INFO SET MEETING_NAME='" + model.MeetingSubject + "',MEETING_DATE= (TO_DATE('" + model.MeetingDate + "','dd/MM/yyyy')) ,MEETING_TYPE='" + model.MeetingType + "', REMARKS='" + model.Remarks + "', ");
                    query.Append(" SET_ON =(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')),SET_BY='" + userId + "'");
                    query.Append(" WHERE ID='" + model.ID + "'");
                }
                else
                { //I for Insert  
                    ReturnMaxID = _idGenerated.getMAXSL("MEETING_INFO", "ID");
                    IUMode = "I";
                    query.Append(" INSERT INTO MEETING_INFO(ID,MEETING_NAME, REMARKS, MEETING_TYPE,MEETING_DATE,SET_BY,SET_ON) ");
                    query.Append(" VALUES( '" + ReturnMaxID + "','" + model.MeetingSubject + "','" + model.Remarks + "', '"+ model.MeetingType +"' ,(TO_DATE('" +model.MeetingDate + "','dd/MM/yyyy')),");
                    query.Append(" '" + userId + "',(TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','dd/MM/yyyy HH24:mi:ss')))");
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
        public IList<MeetingInfoBEL> GetAllInfo(MeetingInfoBEL model, string orderBy)
        {
            var query = new StringBuilder();
            query.Append(" SELECT D.ID, D.MEETING_TYPE, D.REMARKS, D.MEETING_NAME,TO_CHAR(D.MEETING_DATE, 'dd/mm/yyyy')MEETING_DATE, TO_CHAR(D.MEETING_DATE,'RRRR') MEETING_YEAR,TO_CHAR(D.SET_ON, 'dd/mm/yyyy')SET_ON ");
            query.Append(" FROM MEETING_INFO D");
            query.Append(" WHERE 1=1 ");

            if (!string.IsNullOrEmpty(model.MeetingType))
            {
                query.Append(" AND D.MEETING_TYPE='{0}'");
            }
            if (!string.IsNullOrEmpty(model.MeetingSubject))
            {
                query.Append(" AND D.MEETING_NAME='{1}'");
            }

            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                query.Append(" AND D.MEETING_DATE BETWEEN TO_DATE('" + model.FromDate + "','dd/MM/yyyy') AND TO_DATE('" + model.ToDate + "','dd/MM/yyyy') ");
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Append(" ORDER BY  D.ID " + orderBy);
            }
            DataTable dt = _dbHelper.GetDataTable(_dbConn.SAConnStrReader(), string.Format(query.ToString(), model.MeetingType, model.MeetingSubject));

            var item = (from DataRow row in dt.Rows
                        select new MeetingInfoBEL
                        {
                            ID = Convert.ToInt64(row["ID"]),
                            MeetingSubject = row["MEETING_NAME"].ToString(),
                            Remarks = row["REMARKS"].ToString(),
                            MeetingType = row["MEETING_TYPE"].ToString(),
                            MeetingDate = row["MEETING_DATE"].ToString(),
                            MeetingYear = row["MEETING_YEAR"].ToString(),
                            SetOn = row["SET_ON"].ToString()
                        }).ToList();
            return item;
        }
    }
}