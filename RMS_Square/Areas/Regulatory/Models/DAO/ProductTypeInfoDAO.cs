using RMS_Square.Areas.Regulatory.Models.BEL;
using RMS_Square.DAL.Gateway;
using RMS_Square.Universal.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.DAO
{
    public class ProductTypeInfoDAO:ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();
        public List<ProductTypeInfoBEL> GetProductTypeList()
        {
            string Qry = "SELECT PRODUCT_TYPE_CODE,PRODUCT_TYPE_NAME,STATUS from PRODUCT_TYPE_INFO";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<ProductTypeInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new ProductTypeInfoBEL
                    {
                        ProductTypeCode = row["PRODUCT_TYPE_CODE"].ToString(),
                        ProductTypeName = row["PRODUCT_TYPE_NAME"].ToString(),
                        Status = row["STATUS"].ToString()

                    }).ToList();
            return item;
        }
        public bool SaveUpdate(ProductTypeInfoBEL master)
        {
            try
            {
                string Qry = "";
                //string setON = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
             //   string setOndate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                // string setON = DateTime.Now.ToString("dd/MM/yyyy");
                // DateTime setONDT=Convert.ToDateTime(setON);

                if (master.ProductTypeCode == null || master.ProductTypeCode == "")
                {//I for Insert  
                    MaxID = idGenerated.getMAXID("PRODUCT_TYPE_INFO", "PRODUCT_TYPE_CODE", "fm0000");
                    IUMode = "I";

                    Qry = "Insert into PRODUCT_TYPE_INFO(PRODUCT_TYPE_CODE,PRODUCT_TYPE_NAME, STATUS) Values('" + MaxID + "','" + master.ProductTypeName + "','" + master.Status + "')";
                }
                else
                {//U for Insert
                    MaxID = master.ProductTypeCode;
                    IUMode = "U";
                    Qry = "Update PRODUCT_TYPE_INFO set PRODUCT_TYPE_NAME='" + master.ProductTypeName + "',STATUS='" + master.Status + "' Where PRODUCT_TYPE_CODE='" + master.ProductTypeCode + "'";
                }
                if (dbHelper.CmdExecute(dbConn.SAConnStrReader(), Qry))
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
    }
}