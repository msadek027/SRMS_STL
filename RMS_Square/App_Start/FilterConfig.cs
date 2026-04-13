using System;
using System.Web;
using System.Web.Mvc;
using Systems.ActionFilter;


namespace RMS_Square.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ActionAuth());
        }
    }
}