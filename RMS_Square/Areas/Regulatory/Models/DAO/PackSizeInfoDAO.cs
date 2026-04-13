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
    public class PackSizeInfoDAO : ReturnData
    {

        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        IDGenerated idGenerated = new IDGenerated();
        public List<PackSizeInfoBEL> GetPackSizeList()
        {
            string Qry = "SELECT PACK_SIZE_CODE,PACK_SIZE_NAME,STATUS from PACK_SIZE_INFO";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<PackSizeInfoBEL> item;

            item = (from DataRow row in dt.Rows
                    select new PackSizeInfoBEL
                    {
                        PackSizeCode = row["PACK_SIZE_CODE"].ToString(),
                        PackSizeName = row["PACK_SIZE_NAME"].ToString(),
                        Status = row["STATUS"].ToString()

                    }).ToList();
            return item;
        }
        public bool SaveUpdate(PackSizeInfoBEL master, string userId)
        {
            try
            {
               // var packSize=master.PackSizeName.Split('\'');
               /* int index = master.PackSizeName.IndexOfAny(spcicalchar);
                string packSize=null;
                if (index != 0)
                {
                     packSize = master.PackSizeName.Replace("'", "''").Trim();
                }*/

              //  string strPackSize = packSize[0] + "''" + packSize[1];

                String setBy = userId;
                string setOn = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                String updateBy = userId;
                String updateDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                string Qry = "";

                if (master.PackSizeCode == null || master.PackSizeCode == "")
                {
                    //I for Insert  
                    MaxID = idGenerated.getMAXID("PACK_SIZE_INFO", "PACK_SIZE_CODE", "fm0000");
                    IUMode = "I";

                    Qry = "Insert into PACK_SIZE_INFO(PACK_SIZE_CODE,PACK_SIZE_NAME,STATUS,SET_BY,SET_ON) Values('" + MaxID + "','" + master.PackSizeName.Replace("'", "''").Trim() +"','" + master.Status + "','" + setBy + "', TO_DATE('" + setOn + "','dd/MM/yyyy HH24:mi:ss'))";
                }
                else
                {
                    //U for Update
                    MaxID = master.PackSizeCode;
                    IUMode = "U";

                    Qry = "Update PACK_SIZE_INFO set PACK_SIZE_NAME='" + master.PackSizeName.Replace("'", "''").Trim() + "', STATUS='" + master.Status + "', UPDATE_BY='" + updateBy + "', UPDATE_DATE= TO_DATE('" + updateDate + "','dd/MM/yyyy HH24:mi:ss')  Where PACK_SIZE_CODE='" + master.PackSizeCode + "'";
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


        //public char[] spcicalchar ="!@#$%^&*()'".ToCharArray();
    }
}
