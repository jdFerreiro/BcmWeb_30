using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BcmWeb_30 
{
    public class CultureAwareControllerActivator : IControllerActivator
    {
        public IController Create(RequestContext requestContext, Type controllerType)
        {

            string Culture = HttpContext.Current.Request.UserLanguages[0];
            CultureInfo culture = CultureInfo.GetCultureInfo("es-VE");
            //Get the {language} parameter in the RouteData
            try
            {
                string language = requestContext.RouteData.Values["language"] == null ?
                    Culture : requestContext.RouteData.Values["language"].ToString();

                //Get the culture info of the language code
                culture = CultureInfo.GetCultureInfo(language);
            }
            catch
            {
                culture = CultureInfo.GetCultureInfo("es-VE");
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            return DependencyResolver.Current.GetService(controllerType) as IController;
        }
    }
}