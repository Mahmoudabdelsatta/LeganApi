using CommitteeApi.Repository;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace CommitteeApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            //   FirebaseApp.Create(new AppOptions()
            //   {

            //       //Credential = GoogleCredential.FromFile(@"D:\self\self projects\Committee\CommitteeApi\CommitteeApi\FireBase\legan-923c8-firebase-adminsdk-iekzq-071b49583d.json"),
            //   });
            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
          
        }
    }
}
