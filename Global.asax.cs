using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(RealTimeChatHub.Startup))]

namespace RealTimeChatHub
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register); // Fully qualified reference
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config); // Use Web API routing

            // Configure SignalR
            app.MapSignalR(); // Map SignalR hubs

            // Get connection string from Web.config
            string connectionString = ConfigurationManager.ConnectionStrings["ChatBot"].ConnectionString;

            // Configure Hangfire
            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString); // Fully qualified reference for Hangfire
            app.UseHangfireDashboard(); // Enable Hangfire dashboard
            app.UseHangfireServer(); // Start the Hangfire processing server
        }
    }
}
