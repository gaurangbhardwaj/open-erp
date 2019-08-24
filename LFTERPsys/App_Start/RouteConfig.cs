using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LFTERPsys
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{sessionid}/{id}",
                defaults: new { controller = "Login", action = "Signin", sessionid = UrlParameter.Optional, id = UrlParameter.Optional }
            );
        }
    }
}
