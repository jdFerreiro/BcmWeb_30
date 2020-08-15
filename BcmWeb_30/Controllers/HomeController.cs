using BcmWeb_30.Models;
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace BcmWeb_30 .Controllers
{
    public class HomeController : Controller
    {

        [HandleError]
        public ActionResult Index()
        {
            // DXCOMMENT: Pass a data model for GridView

            if (Session["UserId"] != null)
            {
                long UserId = long.Parse(Session["UserId"].ToString());
                Metodos.Logout(UserId);
                Session.Abandon();
                Session.RemoveAll();
                FormsAuthentication.SignOut();
                Session["UserId"] = null;
                return RedirectToAction("Index", "Home");
            }
            else if (Request.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Title = string.Format("{0} - {1}", Resources.BCMWebPublic.labelAppTitle, Resources.BCMWebPublic.labelAppSlogan);
            ViewBag.PageTitle = Resources.BCMWebPublic.labelAppSlogan;
            return View();
        }

        [HandleError]
        public ActionResult GridViewPartialView() 
        {
            // DXCOMMENT: Pass a data model for GridView in the PartialView method's second parameter
            return PartialView("GridViewPartialView");
        }
        [HandleError]
        public ActionResult About()
        {
            ViewBag.Title = string.Format("{0} - {1}", Resources.BCMWebPublic.labelAppTitle, Resources.BCMWebPublic.AboutPageTitle);
            ViewBag.PageTitle = Resources.BCMWebPublic.AboutPageTitle;
            return View();
        }
        [HandleError]
        public ActionResult BSMMovil()
        {
            ViewBag.Title = string.Format("{0} - {1}", Resources.BCMWebPublic.labelAppTitle, Resources.BCMWebPublic.BSMMovilPageTitle);
            ViewBag.PageTitle = Resources.BCMWebPublic.BSMMovilPageTitle;
            return View();
        }
        [HandleError]
        public ActionResult Confidencialidad()
        {
            ViewBag.Title = string.Format("{0} - {1}", Resources.BCMWebPublic.labelAppTitle, Resources.BCMWebPublic.ConfidencialidadPageTitle);
            ViewBag.PageTitle = Resources.BCMWebPublic.ConfidencialidadPageTitle;
            return View();
        }
        [HandleError]
        public ActionResult FAQ()
        {
            ViewBag.Title = string.Format("{0} - {1}", Resources.BCMWebPublic.labelAppTitle, Resources.BCMWebPublic.FAQPageTitle);
            ViewBag.PageTitle = Resources.BCMWebPublic.FAQPageTitle;
            return View();
        }
        [HandleError]
        public ActionResult ProcedimientoBCP()
        {
            ViewBag.Title = string.Format("{0} - {1}", Resources.BCMWebPublic.labelAppTitle, Resources.BCMWebPublic.ProcedimientoBCPPageTitle);
            ViewBag.PageTitle = Resources.BCMWebPublic.ProcedimientoBCPPageTitle;
            return View();
        }
        [HandleError]
        public ActionResult ProcedimientoBIA()
        {
            ViewBag.Title = string.Format("{0} - {1}", Resources.BCMWebPublic.labelAppTitle, Resources.BCMWebPublic.ProcedimientoBIAPageTitle);
            ViewBag.PageTitle = Resources.BCMWebPublic.ProcedimientoBIAPageTitle;
            return View();
        }
        [HandleError]
        public ActionResult Contact()
        {
            ViewBag.Title = string.Format("{0} - {1}", Resources.BCMWebPublic.labelAppTitle, Resources.BCMWebPublic.ContactPageTitle);
            ViewBag.PageTitle = Resources.BCMWebPublic.ContactPageTitle;
            ContactoViewModel model = new ContactoViewModel();
            return View(model);
        }
        [HandleError]
        [HttpPost]
        public ActionResult Contact(ContactoViewModel model)
        {
            if (ModelState.IsValid && CaptchaExtension.GetIsValid("captcha"))
            {
                string _esquema = Request.Url.Scheme;
                string _hostName = Request.Url.Host;

                EmailManager.EnviarEmailContacto(model.Nombre, model.Email, model.Asunto, model.Mensaje, this.ControllerContext, RouteTable.Routes, _esquema, _hostName);
                return RedirectToAction("Index");
            }

            ViewBag.Title = string.Format("{0} - {1}", Resources.BCMWebPublic.labelAppTitle, Resources.BCMWebPublic.ContactPageTitle);
            ViewBag.PageTitle = Resources.BCMWebPublic.ContactPageTitle;
            return View(model);
        }
        [HandleError]
        public ActionResult MensajeContactoPartialView()
        {
            return PartialView();
        }
    }
}

public enum HeaderViewRenderMode { Full, Title }