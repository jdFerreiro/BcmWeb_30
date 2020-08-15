using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BcmWeb_30 .Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        [HandleError]
        public ActionResult Index()
        {
            ViewBag.Title = string.Format("{0} - {1}", Resources.BCMWebPublic.labelAppTitle, Resources.ErrorPageResource.ErrorPageTitle);
            return View();
        }
    }
}