using BcmWeb_30.Models;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BcmWeb_30 .Controllers
{

    [Authorize]
    public class EventosController : Controller
    {

        [SessionExpire]
        [HandleError]
        public ActionResult Index(long IdModulo)
        {
            FirstModuloSelected firstModulo = Metodos.GetFirstUserModulo(IdModulo);

            return RedirectToAction(firstModulo.Action, firstModulo.Controller, new { modId = firstModulo.IdModulo });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Dispositivos(long modId)
        {

            Session["modId"] = modId;

            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            DispositivosEmpresaModel model = new DispositivosEmpresaModel
            {
                IdModulo = IdModulo,
                IdModuloActual = modId,
                Perfil = Metodos.GetPerfilData(),
                PageTitle = Metodos.GetModuloName(modId),
                returnPage = Url.Action("Index", "Documento", new { IdModulo }),
            };

            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            Auditoria.RegistarAccion(eTipoAccion.ConsultarDispositivos);
            Session["GridViewData"] = Metodos.GetDispositivosMoviles();
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ExportDispositivos()
        {
            string _modId = Session["modId"].ToString();
            long modId = long.Parse(_modId);
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            //model.IdModulo = IdModulo;
            //model.IdModuloActual = modId;
            //model.Perfil = Metodos.GetPerfilData();
            //model.PageTitle = Metodos.GetModuloName(modId);
            //ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            Session["GridViewData"] = Metodos.GetDispositivosMoviles();
            return GridViewExportIniciativas.FormatConditionsExportFormatsInfo[GridViewExportFormat.Xlsx](GridViewExportIniciativas.FormatConditionsExportGridViewSettings, Metodos.GetDispositivosMoviles());

        }
        [SessionExpire]
        [HandleError]
        public ActionResult DispositivosPartialView()
        {
            Session["GridViewData"] = Metodos.GetDispositivosMoviles();
            return PartialView("DispositivosPartialView");
        }
        [SessionExpire]
        [HandleError]
        [ValidateInput(false)]
        public ActionResult ConexionesPartial(string IdDispositivo)
        {
            if (IdDispositivo != null)
                Session["IdDispositivo"] = IdDispositivo;
            Session["gvConexionesData"] = Metodos.GetConexionesDispositivo(IdDispositivo);
            return PartialView("ConexionesPartial");
        }

        [SessionExpire]
        [HandleError]
        public ActionResult Activar(long modId)
        {

            Session["modId"] = modId;

            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;
            Nullable<long> _idSubmodulo = Metodos.GetEscenariosEmpresa().FirstOrDefault().Id;

            EventoRegistroEmpresaModel model = new EventoRegistroEmpresaModel
            {
                IdSubmoduloSelected = _idSubmodulo ?? 0,
                IdModulo = IdModulo,
                IdModuloActual = modId,
                Perfil = Metodos.GetPerfilData(),
                PageTitle = Metodos.GetModuloName(modId),
                Dispositivos = Metodos.GetDispositivosMovilEvento(_idSubmodulo ?? 0),
            };

            ViewBag.SelectAllCheckBoxMode = (model.Dispositivos.Count(x => x.FechaEnvio == null) == 0 ? GridViewSelectAllCheckBoxMode.None : GridViewSelectAllCheckBoxMode.AllPages);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            Auditoria.RegistarAccion(eTipoAccion.ConsultarDispositivos);
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult Activar(EventoRegistroEmpresaModel model)
        {

            string _modId = Session["modId"].ToString();
            long modId = long.Parse(_modId);
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            model.Dispositivos = Metodos.GetDispositivosMovilEvento(model.IdSubmoduloSelected);

            ViewBag.SelectAllCheckBoxMode = (model.Dispositivos.Count(x => x.FechaEnvio == null) == 0 ? GridViewSelectAllCheckBoxMode.None : GridViewSelectAllCheckBoxMode.AllPages);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ComboBoxEvento(long Id)
        {
            EventoEmpresaModel model = new EventoEmpresaModel();
            model.IdSubmoduloSelected = Id;
            return PartialView(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult DispEventPartialView()
        {
            
            string _selectedIDs = Request.Params["selectedIDs"];
            string _idModuloSelected = Request.Params["idModuloSelected"];

            string _modId = Session["modId"].ToString();
            long modId = long.Parse(_modId);
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;
            Nullable<long> _idSubmodulo = long.Parse(_idModuloSelected);

            EventoRegistroEmpresaModel model = new EventoRegistroEmpresaModel
            {
                IdSubmoduloSelected = _idSubmodulo ?? 0,
                IdModulo = IdModulo,
                IdModuloActual = modId,
                Perfil = Metodos.GetPerfilData(),
                PageTitle = Metodos.GetModuloName(modId),
                Dispositivos = Metodos.GetDispositivosMovilEvento(_idSubmodulo ?? 0),
            };

            ViewBag.SelectAllCheckBoxMode = (model.Dispositivos.Count(x => x.FechaEnvio == null) == 0 ? GridViewSelectAllCheckBoxMode.None : GridViewSelectAllCheckBoxMode.AllPages);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return PartialView(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult SaveEvento(string IDs, long idSubmodulo)
        {
            Metodos.InsertEvento(idSubmodulo, IDs);

            string _modId = Session["modId"].ToString();
            long modId = long.Parse(_modId);

            return RedirectToAction("Activar", new { modId });
        }
    }
}