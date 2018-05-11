using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using IBISA.Controllers;

namespace IBISA
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void Application_Error(object sender, EventArgs e)
        {
            //try-catch added to aviod process on process unnecessary exception
            try
            {
                var exception = Server.GetLastError();
                var httpException = exception as HttpException;
                Response.Clear();
                Server.ClearError();
                var routeData = new RouteData();
                routeData.Values["controller"] = "Errors";
                routeData.Values["action"] = "General";
                routeData.Values["exception"] = exception;

                //Response.StatusCode = 500;
                if (httpException != null)
                {
                    Response.StatusCode = httpException.GetHttpCode();
                    Response.TrySkipIisCustomErrors = true;
                    switch (Response.StatusCode)
                    {
                        case 500:
                            {
                                routeData.Values["action"] = "Http500";
                                routeData.Values["exception"] = exception;
                                break;
                            }
                        case 403:
                            {
                                routeData.Values["action"] = "Http403";
                                routeData.Values["exception"] = exception;
                                break;
                            }
                        case 404:
                            {
                                routeData.Values["action"] = "Http404";
                                routeData.Values["exception"] = exception;
                                break;
                            }
                    }
                }

                IController errorsController = new ErrorsController();
                var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
                errorsController.Execute(rc);

            }
            catch (Exception)
            {
                //  throw;
            }
        }
    }
}
