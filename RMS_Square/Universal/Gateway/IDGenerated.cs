using RMS_Square.DAL.Gateway;
using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Web;

namespace RMS_Square.Universal.Gateway
{
    public class IDGenerated
    {
        DBConnection dbConnection = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        public Int64 getMAXSL( string tableName,string columnName)
        {
            Int64 MAXID = 0;
            string QueryString = "select NVL(MAX(" + columnName + "),0)+1 id from " + tableName + "";
            using (OracleConnection oracleConnection = new OracleConnection(dbConnection.SAConnStrReader()))
            {
                oracleConnection.Open();
                using (OracleCommand oracleCommand = new OracleCommand(QueryString, oracleConnection))
                {
                    using (OracleDataReader rdr = oracleCommand.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            MAXID = Convert.ToInt64(rdr["id"].ToString());
                        }
                    }
                }
            }
            return MAXID;
        }
        public string getMAXID(string tableName, string columnName, string fm9)
        {
            string MAXID = "";
            string QueryString = "select to_char((select NVL(MAX(" + columnName + "),0)+1 from " + tableName + " ), '" + fm9 + "') id from dual";
            using (OracleConnection oracleConnection = new OracleConnection(dbConnection.SAConnStrReader()))
            {
                oracleConnection.Open();
                using (OracleCommand oracleCommand = new OracleCommand(QueryString, oracleConnection))
                {
                    using (OracleDataReader rdr = oracleCommand.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            MAXID = rdr[0].ToString();
                        }
                    }
                }
            }
            return MAXID;
        }


        public String GetLanIPAddress()
        {
            //The X-Forwarded-For (XFF) HTTP header field is a de facto standard for identifying the originating IP address of a
            //client connecting to a web server through an HTTP proxy or load balancer
            String ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (ip == "::1")
            {
                ip = "127.0.0.1";
            }
            return ip;
        }


    }
}