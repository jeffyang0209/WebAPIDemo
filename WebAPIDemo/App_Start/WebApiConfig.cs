using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebAPIDemo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();
            // 注意，domin後不能有斜線
            var cors = new EnableCorsAttribute(origins: "http://localhost:25536", headers: "*", methods: "*"); config.EnableCors(cors);

            /*DELETE動詞的重要性
              AJAX在不同源政策之下，預設只支持GET、POST
              而DELETE、PUT等其他動詞，會先送出Options，並且回傳該Server是否接受請求，若是接受才會正式送出。
              因此，若是在API設計上有區分動詞，API的結構會較為安全，因為GET、POST以外的動詞會需要伺服器授權才能執行
             */

            // Web API 設定和服務

            // Web API 屬性路由
            config.MapHttpAttributeRoutes();

            // 傳統路由
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
