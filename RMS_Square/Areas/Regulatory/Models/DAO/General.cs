using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS_Square.Areas.Regulatory.Models.DAO
{
    public class General
    {
        public static string SetDateStrYYYYMMDD(string strDate)
        {
            var newDateStr = "";
            //var strDatepart = strDate.Substring(0, 10);
            if (strDate.Contains('/'))
            {
                var str = strDate.Split('/');
                newDateStr = str[2] + "/" + str[1] + "/" + str[0];
            }
            return newDateStr;
        }
    }
}