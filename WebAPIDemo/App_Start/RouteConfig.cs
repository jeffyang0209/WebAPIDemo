using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebAPIDemo
{
    public class RouteConfig
    {
        // MVC路由
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                // url: "{controller}/{action}/{id}",
                // 讓MVC路由指使用HOME進行比對，讓錯誤導轉至IIS
                url: "{Home}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
