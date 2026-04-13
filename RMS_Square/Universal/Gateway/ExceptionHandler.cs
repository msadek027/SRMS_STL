using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMS_Square.DAL.Gateway
{
    public class ExceptionHandler: Controller
    {
        public ActionResult ErrorMsg(Exception e)
        {
            if (e.Message.Substring(0, 9) == "ORA-00001")
                return Json(new { Status = "Error:ORA-00001,Data already exists!" });//Unique Identifier.
            else if (e.Message.Substring(0, 9) == "ORA-02292")
                return Json(new { Status = "Error:ORA-02292,Data already exists!" });//Child Record Found.
            else if (e.Message.Substring(0, 9) == "ORA-12899")
                return Json(new { Status = "Error:ORA-12899,Data Value Too Large!" });//Value Too Large.
            else
                return Json(new { Status = "! Error :" + e.Message });//Other Wise Error Found
        }

    }
}