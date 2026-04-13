using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using RMS_Square.Areas.SA.Models.BEL;
using RMS_Square.DAL.Gateway;
using RMS_Square.Universal.Gateway;
using Systems.Universal;

namespace RMS_Square.Areas.SA.Models.DAL.DAO
{
    public class FormDAO:ReturnData
    {
        DBConnection dbConn = new DBConnection();
       // SaHelper saHelper = new SaHelper();
        DBHelper saHelper = new DBHelper();
        public Boolean SaveUpdate(FormBEL formBEL)
        {
            try
            {
                string Qry = "Select MAX(FormID) ID from Sa_Form";
                formBEL.FormID = (formBEL.FormID == "" || formBEL.FormID == null) ? "0" : formBEL.FormID;  
                var tuple = saHelper.ProcedureExecuteTFn4(dbConn.SAConnStrReader(), Qry, "Sa_Form_SSP", "p_ID", "p_Name", "p_FormURL", "p_IsActive", formBEL.FormID, formBEL.FormName, formBEL.FormURL, formBEL.IsActive.ToString());
                if (tuple.Item1)
                {
                    MaxID = tuple.Item2;
                    IUMode = tuple.Item3;
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



        public bool DeleteExecute(FormBEL formBEL)
        {
            string Qry = " Delete from Sa_Form Where FormID='" + formBEL.FormID + "'";
            if (saHelper.ExecuteFn(dbConn.SAConnStrReader(),Qry))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<FormBEL> GetFormList()
        {
            string Qry = "SELECT FormID,FormName,FormURL,IsActive FROM Sa_Form ";
            DataTable dt = saHelper.DataTableFn(dbConn.SAConnStrReader(),Qry);
            List<FormBEL> item;

            item = (from DataRow row in dt.Rows
                    select new FormBEL
                    {
                        FormID = row["FormID"].ToString(),
                        FormName = row["FormName"].ToString(),
                        FormURL = row["FormURL"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString())

                    }).ToList();
            return item;
        }
    }
}