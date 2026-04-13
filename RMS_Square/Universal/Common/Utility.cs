using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using Systems.Universal;

namespace RMS_Square.DAL.Common
{
    public static class Utility
    {
        public static string ConvertNumberE2B(Int64 EngNumber)
        {
            EngNumber = 1234567890;
            string BanglaText = string.Concat(EngNumber.ToString().Select(c => (char)('\u09E6' + c - '0'))); // "১২৩৪৫৬৭৮৯০"
            string EnglishText = string.Concat(BanglaText.Select(c => (char)('0' + c - '\u09E6'))); // "1234567890"
            int parsed_number = int.Parse(EnglishText); // 1234567890

            return BanglaText;
        }
        public static string ConvertNumberB2E(string BanglaText)
        {
            BanglaText = "১২৩৪৫৬৭৮৯০";     
            string EnglishText = string.Concat(BanglaText.Select(c => (char)('0' + c - '\u09E6'))); // "1234567890"
            int parsed_number = int.Parse(EnglishText); // 1234567890

            return EnglishText;
        }

        public static String GetLANIP()
        {
            string ip = string.Empty;
            //client connecting to a web server through an HTTP proxy or load balancer
            //ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //if (string.IsNullOrEmpty(ip))
            //{
            //    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //}
            //if (ip == "::1")
            //{
            //    ip = "127.0.0.1";
            //}
            return ip;
        }

        public static List<T> TableToList<T>(DataTable table)
        {
            List<T> rez = new List<T>();
            foreach (DataRow rw in table.Rows)
            {
                T item = Activator.CreateInstance<T>();
                foreach (DataColumn cl in table.Columns)
                {
                    PropertyInfo pi = typeof(T).GetProperty(cl.ColumnName);

                    if (pi != null && rw[cl] != DBNull.Value)
                    {
                        var propType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                        pi.SetValue(item, Convert.ChangeType(rw[cl], propType), new object[0]);
                    }

                }
                rez.Add(item);
            }
            return rez;
        }
        public static string  GetServerPath()
        {
            return Encryption.GetServerPathFromSystem();
            
        }
    }
}