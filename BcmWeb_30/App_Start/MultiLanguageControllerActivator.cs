using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;

namespace BcmWeb_30
{
    public class MultiLanguageControllerActivator : IControllerActivator
    {
        private string FallBackLanguage = "en-US";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="controllerType"></param>
        /// <returns></returns>
        public IController Create(RequestContext requestContext, Type controllerType)
        {
            if (requestContext.HttpContext.Request.UserLanguages != null)
            {
                FallBackLanguage = requestContext.HttpContext.Request.UserLanguages[0] ?? FallBackLanguage;
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(FallBackLanguage);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(FallBackLanguage);

            return DependencyResolver.Current.GetService(controllerType) as IController;
        }
    }
}