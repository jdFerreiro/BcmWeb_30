using BcmWeb_30.Models;
using BcmWeb_30.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace BcmWeb_30 .Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        // GET: Menu
        [SessionExpire]
        [HandleError]
        public ActionResult Index()
        {
            ViewBag.Title = string.Format("{0} - {1}", Resources.BCMWebPublic.labelAppTitle, Resources.BCMWebPublic.labelAppSlogan);
            ModulosUserModel model = new ModulosUserModel();
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.ModulosPrincipales = Metodos.GetModulosPrincipalesEmpresaUsuario();
            model.Perfil = Metodos.GetPerfilData();
            Session["IdNivelUsuario"] = Metodos.GetNivelUsuarioByUserId().ToString();
            return View(model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public ActionResult Index(ModulosUserModel model) 
        {
            ViewBag.Title = string.Format("{0} - {1}", Resources.BCMWebPublic.labelAppTitle, Resources.BCMWebPublic.labelAppSlogan);
            Session["IdEmpresa"] = model.IdEmpresa;
            model.Perfil = Metodos.GetPerfilData();
            model.ModulosPrincipales = Metodos.GetModulosPrincipalesEmpresaUsuario();
            Session["IdNivelUsuario"] = Metodos.GetNivelUsuarioByUserId().ToString();

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ComboBoxEmpresaPartial()
        {
            return PartialView();
        }
        //[SessionExpire]
        //[HandleError]
        //public ActionResult _PerfilPartialView(ModulosUserModel model)
        //{
        //    return PartialView(model);
        //}
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult PerfilPartial(ModulosUserModel model)
        {
            if (ModelState.IsValid)
            {
                var authTicket = new FormsAuthenticationTicket(1, model.Perfil.Nombre, DateTime.Now, DateTime.Now.AddMinutes(20), false, model.Perfil.IdUsuario.ToString());
                string encriptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encriptedTicket);
                HttpContext.Response.Cookies.Add(authCookie);
                Metodos.SavePerfil(model.Perfil);
                string _esquema = Request.Url.Scheme;
                string _hostName = Request.Url.Host;

                EmailManager.EnviarEmailUpdatePerfil(model.Perfil.Email, this.ControllerContext, RouteTable.Routes, _esquema, _hostName, model.Perfil);
            }
            return RedirectToAction("Index");
            // return PartialView("_PerfilPartialView", model);
        }

    }
}