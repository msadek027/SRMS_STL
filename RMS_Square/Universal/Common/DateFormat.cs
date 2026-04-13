using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RMS_Square.DAL.Common
{
    public class DateFormat
    {
          string date="";

          string CntDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture);
          public string MonthNameYear(string ddMMyyyy)
          {
              if (ddMMyyyy.Length == 10)
              {
                  string monthName = "";
                  string[] monthNumber = ddMMyyyy.Split('-');

                  if ((monthNumber[1] == "01") || (monthNumber[1] == "1"))
                      monthName = "January";
                  else if ((monthNumber[1] == "02") || (monthNumber[1] == "2"))
                      monthName = "February";
                  else if ((monthNumber[1] == "03") || (monthNumber[1] == "3"))
                      monthName = "March";
                  else if ((monthNumber[1] == "04") || (monthNumber[1] == "4"))
                      monthName = "April";
                  else if ((monthNumber[1] == "05") || (monthNumber[1] == "5"))
                      monthName = "May";
                  else if ((monthNumber[1] == "06") || (monthNumber[1] == "6"))
                      monthName = "June";
                  else if ((monthNumber[1] == "07") || (monthNumber[1] == "7"))
                      monthName = "July";
                  else if ((monthNumber[1] == "08") || (monthNumber[1] == "8"))
                      monthName = "Auguest";
                  else if ((monthNumber[1] == "09") || (monthNumber[1] == "9"))
                      monthName = "September";
                  else if (monthNumber[1] == "10")
                      monthName = "October";
                  else if (monthNumber[1] == "11")
                      monthName = "November";
                  else if (monthNumber[1] == "12")
                      monthName = "December";
                  else
                      monthName = string.Empty;

                  ddMMyyyy =  monthName + monthNumber[2];
                  date = ddMMyyyy;
              }
              else
              {
                  date = DateTime.Now.ToString("dd/MM/yyyy");
              }
              return date;
          }

          public string DDMonYear(string ddMMyyyy)
          {
              if (ddMMyyyy.Length == 10)
              {
                  string monthName = "";
                  string[] monthNumber = ddMMyyyy.Split('/');

                  if ((monthNumber[1] == "01") || (monthNumber[1] == "1"))
                      monthName = "Jan";
                  else if ((monthNumber[1] == "02") || (monthNumber[1] == "2"))
                      monthName = "Feb";
                  else if ((monthNumber[1] == "03") || (monthNumber[1] == "3"))
                      monthName = "Mar";
                  else if ((monthNumber[1] == "04") || (monthNumber[1] == "4"))
                      monthName = "Apr";
                  else if ((monthNumber[1] == "05") || (monthNumber[1] == "5"))
                      monthName = "May";
                  else if ((monthNumber[1] == "06") || (monthNumber[1] == "6"))
                      monthName = "Jun";
                  else if ((monthNumber[1] == "07") || (monthNumber[1] == "7"))
                      monthName = "Jul";
                  else if ((monthNumber[1] == "08") || (monthNumber[1] == "8"))
                      monthName = "Aug";
                  else if ((monthNumber[1] == "09") || (monthNumber[1] == "9"))
                      monthName = "Sep";
                  else if (monthNumber[1] == "10")
                      monthName = "Oct";
                  else if (monthNumber[1] == "11")
                      monthName = "Nov";
                  else if (monthNumber[1] == "12")
                      monthName = "Dec";
                  else
                      monthName = string.Empty;

                  ddMMyyyy = monthNumber[0] + "-" + monthName + "-" + monthNumber[2];
                  date = ddMMyyyy;
              }
              else
              {
                  date = DateTime.Now.ToString("dd/MM/yyyy");
              }
              return date;
          }

        public string CurrentDdMmYyyy()
          {
            string day = DateTime.Now.Day.ToString().Length == 1 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            string month = DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            string ddMMyyyy = day +"/"+ month+ "/"+ DateTime.Now.Year.ToString();
            return ddMMyyyy;
          }

    }
}