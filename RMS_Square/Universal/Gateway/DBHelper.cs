using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using RMS_Square.Models;
using Systems.Universal;


namespace RMS_Square.DAL.Gateway
{

    public class DBHelper : SaHelper
    {

        DBConnection dbConnection = new DBConnection();
        public Boolean CmdExecute(string ConnString, string Qry)
        {
            bool isTrue = false;
            try
            {

                using (OracleConnection con = new OracleConnection(ConnString))
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand(Qry, con))
                    {
                        int noOfRows = cmd.ExecuteNonQuery();

                        if (noOfRows > 0)
                        {
                            isTrue = true;

                        }
                    }
                    ////
                    //OracleCommand cmd = new OracleCommand(Qry, con);
                    //con.Open();
                    //int noOfRows = cmd.ExecuteNonQuery();           

                    //if (noOfRows > 0)
                    //{
                    //    isTrue = true;

                    //}

                    ////
                }
                return isTrue;
            }
            catch (Exception ex)
            {
                return isTrue;
            }

        }
        public DataTable GetDataTable(string qry)
        {
            OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(qry, dbConnection.SAConnStrReader());
            DataTable dt = new DataTable();
            oracleDataAdapter.Fill(dt);
            return dt;
        }
        public DataSet GetDataSet(string Qry)
        {
            OracleDataAdapter odbcDataAdapter = new OracleDataAdapter(Qry, dbConnection.SAConnStrReader());
            DataSet ds = new DataSet();
            odbcDataAdapter.Fill(ds, "Results");
            return ds;
        }
        public DataTable GetDataTable(string ConnString, string Qry)
        {
            DataTable dt = new DataTable();
            using (OracleConnection objConn = new OracleConnection(ConnString))
            {

                OracleCommand objCmd = new OracleCommand();
                objCmd.CommandText = Qry;
                objCmd.Connection = objConn;
                objConn.Open();
                objCmd.ExecuteNonQuery();
                using (OracleDataReader rdr = objCmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        dt.Load(rdr);
                    }
                }

            }

            return dt;
        }





        public string GetValue(string qry)
        {
            string value = "";
            using (OracleConnection odbcConnection = new OracleConnection(dbConnection.SAConnStrReader()))
            {
                odbcConnection.Open();
                using (OracleCommand odbcCommand = new OracleCommand(qry, odbcConnection))
                {
                    using (OracleDataReader rdr = odbcCommand.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            value = rdr[0].ToString();
                        }
                        rdr.Close();

                    }
                }
                odbcConnection.Close();
            }
            return value;
        }

        public DataTable GetDataTableRefCursorF1(string funName, string FieldName, string FieldValue)
        {
            DataTable dt = new DataTable();
            using (OracleConnection objConn = new OracleConnection(dbConnection.SAConnStrReader()))
            {
                using (OracleCommand objCmd = new OracleCommand())
                {
                    objCmd.Connection = objConn;
                    objCmd.CommandText = funName;
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add(FieldName, OracleType.VarChar).Value = FieldValue;
                    objCmd.Parameters.Add("ReturnValue", OracleType.Cursor).Direction = ParameterDirection.ReturnValue;
                    objConn.Open();
                    objCmd.ExecuteNonQuery();
                    using (OracleDataReader rdr = objCmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            dt.Load(rdr);
                        }
                    }
                }
            }
            return dt;
        }
        public DataTable GetDataTableRefCursorF2(string funName, string FieldName1, string FieldName2, string FieldValue1, string FieldValue2)
        {
            DataTable dt = new DataTable();
            using (OracleConnection objConn = new OracleConnection(dbConnection.SAConnStrReader()))
            {

                using (OracleCommand objCmd = new OracleCommand())
                {
                    objCmd.Connection = objConn;
                    objCmd.CommandText = funName;
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add(FieldName1, OracleType.VarChar).Value = FieldValue1;
                    objCmd.Parameters.Add(FieldName2, OracleType.VarChar).Value = FieldValue2;
                    objCmd.Parameters.Add("ReturnValue", OracleType.Cursor).Direction = ParameterDirection.ReturnValue;
                    objConn.Open();
                    objCmd.ExecuteNonQuery();
                    using (OracleDataReader rdr = objCmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            dt.Load(rdr);
                        }
                    }
                }
            }
            return dt;
        }


        public DataTable GetDataTableRefCursorF3(string funName, string FieldName1, string FieldName2, string FieldName3, string FieldValue1, string FieldValue2, string FieldValue3)
        {
            DataTable dt = new DataTable();
            using (OracleConnection objConn = new OracleConnection(dbConnection.SAConnStrReader()))
            {

                using (OracleCommand objCmd = new OracleCommand())
                {
                    objCmd.Connection = objConn;
                    objCmd.CommandText = funName;
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add(FieldName1, OracleType.VarChar).Value = FieldValue1;
                    objCmd.Parameters.Add(FieldName2, OracleType.VarChar).Value = FieldValue2;
                    objCmd.Parameters.Add(FieldName3, OracleType.VarChar).Value = FieldValue3;
                    objCmd.Parameters.Add("ReturnValue", OracleType.Cursor).Direction = ParameterDirection.ReturnValue;
                    objConn.Open();
                    objCmd.ExecuteNonQuery();
                    using (OracleDataReader rdr = objCmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            dt.Load(rdr);
                        }
                    }
                }
            }
            return dt;
        }


        public Boolean CmdProcedureF1(string Qry, string SPName, string FieldName, string FieldValue)
        {
            bool isTrue = false;
            using (OracleConnection oracleConnection = new OracleConnection(dbConnection.SAConnStrReader()))
            {
                using (OracleCommand oracleCommand = new OracleCommand())
                {
                    oracleCommand.Connection = oracleConnection;
                    oracleCommand.CommandText = SPName;
                    oracleCommand.CommandType = CommandType.StoredProcedure;
                    oracleCommand.Parameters.Add(FieldName, OracleType.VarChar).Value = FieldValue;
                    oracleConnection.Open();
                    if (oracleCommand.ExecuteNonQuery() > 0)
                    {

                        isTrue = true;

                    }
                }
            }
            return isTrue;
        }

        public Boolean CmdProcedureF2(string SPName, string FieldName1, string FieldName2, string FieldValue1, string FieldValue2)
        {
            bool isTrue = false;
            using (OracleConnection oracleConnection = new OracleConnection(dbConnection.SAConnStrReader()))
            {
                using (OracleCommand oracleCommand = new OracleCommand())
                {
                    oracleCommand.Connection = oracleConnection;
                    oracleCommand.CommandText = SPName;
                    oracleCommand.CommandType = CommandType.StoredProcedure;
                    oracleCommand.Parameters.Add(FieldName1, OracleType.VarChar).Value = FieldValue1;
                    oracleCommand.Parameters.Add(FieldName2, OracleType.VarChar).Value = FieldValue2;

                    oracleConnection.Open();

                    if (oracleCommand.ExecuteNonQuery() > 0)
                    {
                        isTrue = true;
                    }
                }
            }
            return isTrue;
        }

        public Boolean CmdProcedureF3(string SPName, string FieldName1, string FieldName2, string FieldName3, string FieldValue1, string FieldValue2, string FieldValue3)
        {
            bool isTrue = false;
            using (OracleConnection oracleConnection = new OracleConnection(dbConnection.SAConnStrReader()))
            {
                using (OracleCommand oracleCommand = new OracleCommand())
                {
                    oracleCommand.Connection = oracleConnection;
                    oracleCommand.CommandText = SPName;
                    oracleCommand.CommandType = CommandType.StoredProcedure;
                    oracleCommand.Parameters.Add(FieldName1, OracleType.VarChar).Value = FieldValue1;
                    oracleCommand.Parameters.Add(FieldName2, OracleType.VarChar).Value = FieldValue2;
                    oracleCommand.Parameters.Add(FieldName3, OracleType.VarChar).Value = FieldValue3;
                    oracleConnection.Open();

                    if (oracleCommand.ExecuteNonQuery() > 0)
                    {
                        isTrue = true;
                    }
                }
            }
            return isTrue;
        }

         public DataTable dt { get; set; }
    }
}