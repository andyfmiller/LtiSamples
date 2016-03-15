using System;
using System.Runtime.Remoting.Channels;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace SimpleLti
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);            
        }

        public override void Init()
        {
            base.Init();
            this.AcquireRequestState += ShowRouteValues;
        }

        protected void ShowRouteValues(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            if (context == null) return;
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context));
        }
    }
}