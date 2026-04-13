using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Systems.Universal;

namespace RMS_Square.DAL.Gateway
{
    public class DBConnection
    {
        string connectionString = "";
        public DBConnection()
        {
            SAConnStrReader();
        }
        public string SAConnStrReader()
        {
            //string st = "Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = 10.3.8.62)(PORT = 1521)))(CONNECT_DATA =(SID = silsqadb1)(SERVER = DEDICATED)));User Id=spl_srms;Password=splsrms";
            //return st;

            connectionString = new DataBaseConnection().SAConnStrReader();
            return connectionString;        
        }
    }
}