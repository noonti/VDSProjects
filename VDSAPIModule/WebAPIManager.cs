using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using VDSCommon;

namespace VDSAPIModule
{
    public class WebAPIManager
    {
        HttpSelfHostConfiguration config;
        HttpSelfHostServer server;

        private void StartHTTPService(String url)
        {
            config = new HttpSelfHostConfiguration(url);
            config.Routes.MapHttpRoute(
            name: "ControllerAndActionOnly",
            routeTemplate: "api/{controller}/{action}",
            defaults: new { },
            constraints: new { action = @"^[a-zA-Z]+([\s][a-zA-Z]+)*$" });

            server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();

            
        }

        private void StopHTTPService()
        {
            if(server!=null)
                server.CloseAsync().Wait();
        }

        public int StartService()
        {
            String url = String.Format($"http://{VDSConfig.controllerConfig.IpAddress}:{VDSConfig.controllerConfig.ApiPort}");
            StartHTTPService(url); 
            return 1;
        }

        public  int StopService()
        {
            StopHTTPService();

            return 1;
        }

    }
}
