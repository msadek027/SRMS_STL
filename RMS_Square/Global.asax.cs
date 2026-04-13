using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace RMS_Square
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            //for example
            //var httpException = ex as HttpException ?? ex.InnerException as HttpException;
            //if (httpException == null) return;

            //if (httpException.WebEventCode == WebEventCodes.RuntimeErrorPostTooLarge)
            //{
            //    //handle the error
            //    Response.Write("Too big a file, dude"); //for example
            //}
        }

        protected void Session_End(object sender, EventArgs e)
        {
            
        }

        protected void Application_End(object sender, EventArgs e)
        {
           
        }
        
        protected void Application_Disposed(object sender, EventArgs e)
        {
           
        }

    }
}