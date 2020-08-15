using BcmWeb_30;
using BcmWeb_30.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BcmWeb_30 .Controllers
{
    [Authorize]
    public class DocumentoController : Controller
    {
        // GET: Documento
        [SessionExpire]
        [HandleError]
        public ActionResult Index(long IdModulo)
        {

            string _IdModulo = IdModulo.ToString();
            int IdTipoDocumento = int.Parse(_IdModulo.Substring(0, (_IdModulo.Length == 7 ? 1 : 2)));

            Session["modId"] = IdModulo;

            DocumentosModel model = new DocumentosModel();
            model.EditDocumento = false;
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdDocumentoSelected = 0;
            model.IdClaseDocumento = 1;
            model.IdModulo = IdModulo;
            model.IdModuloActual = IdModulo;
            model.Documentos = Metodos.GetDocumentosModulo(IdTipoDocumento, (model.IdClaseDocumento == 1));
            model.ModulosPrincipales = Metodos.GetModulosPrincipalesEmpresaUsuario();
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Resources.DocumentoResource.DocumentosPageTitle;
            model.EditActive = Metodos.EditDocumentoActivo(IdTipoDocumento, (model.IdClaseDocumento == 1));
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            Auditoria.RegistarAccion(eTipoAccion.AccesoModuloWeb);

            Session["IdClaseDocumento"] = model.IdClaseDocumento;
            Session["IdTipoDocumento"] = IdTipoDocumento;

            return View(model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public ActionResult Index(DocumentosModel model)
        {
            string _IdModulo = Session["modId"].ToString();
            long IdModulo = long.Parse(_IdModulo);
            int IdTipoDocumento = int.Parse(_IdModulo.Substring(0, (_IdModulo.Length == 7 ? 1 : 2)));
            model.EditDocumento = false;
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdDocumentoSelected = 0;
            model.IdModulo = IdModulo;
            model.IdModuloActual = IdModulo;
            model.EditActive = Metodos.EditDocumentoActivo(IdTipoDocumento, (model.IdClaseDocumento == 1));
            model.Perfil = Metodos.GetPerfilData();
            model.ModulosPrincipales = Metodos.GetModulosPrincipalesEmpresaUsuario();
            model.PageTitle = Resources.DocumentoResource.DocumentosPageTitle;
            model.IdModuloActual = long.Parse(_IdModulo);
            model.Documentos = Metodos.GetDocumentosModulo(IdTipoDocumento, (model.IdClaseDocumento == 1));
            /*
             * Aplicar filtro a los documentos 
            */
            if (model.FilterEstadoDocumento > 0)
                model.Documentos = model.Documentos.Where(x => x.IdEstatus == model.FilterEstadoDocumento).ToList().AsQueryable();
            if (model.FilterIdProceso > 0)
            {
                model.Documentos = model.Documentos.Where(x => x.Procesos != null && x.Procesos.Where(p => p.IdProceso == model.FilterIdProceso).Count() > 0).ToList().AsQueryable();
            }
            if (model.FilterIdUnidadOrganizativa > 0)
            {
                IList<long> UOIds = Metodos.GetRelatedUOIds(model.FilterIdUnidadOrganizativa);

                if (IdTipoDocumento == 4)
                    model.Documentos = model.Documentos.Where(x => x.DatosBIA != null && UOIds.Contains(x.DatosBIA.IdUnidadOrganizativa)).ToList().AsQueryable();
                else
                    model.Documentos = model.Documentos.Where(x => x.DatosBCP != null && UOIds.Contains(x.DatosBCP.IdUnidadOrganizativa)).ToList().AsQueryable();
            }
            if (model.FilterNroDocumento > 0)
                model.Documentos = model.Documentos.Where(x => x.NroDocumento == model.FilterNroDocumento).ToList().AsQueryable();
            if (model.FilterProcesoCritico)
                model.Documentos = model.Documentos.Where(x => x.Procesos != null && x.Procesos.Where(p => p.Critico == model.FilterProcesoCritico).Count() > 0).ToList().AsQueryable();
            if (model.FilterResponsable > 0)
                model.Documentos = model.Documentos.Where(x => x.IdPersonaResponsable == model.FilterResponsable).ToList().AsQueryable();

            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public ActionResult FilterBiaDocs(DocumentosModel model)
        {
            return PartialView(model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public ActionResult DocumentoPartialView(DocumentosModel model)
        {
            string _IdModulo = Session["modId"].ToString();
            int IdTipoDocumento = int.Parse(_IdModulo.Substring(0, (_IdModulo.Length == 7 ? 1 : 2)));
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.Documentos = Metodos.GetDocumentosModulo(IdTipoDocumento, (model.IdClaseDocumento == 1));
            model.Perfil = Metodos.GetPerfilData();
            model.ModulosPrincipales = Metodos.GetModulosPrincipalesEmpresaUsuario();
            model.PageTitle = Resources.DocumentoResource.DocumentosPageTitle;
            model.IdModulo = long.Parse(_IdModulo);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return PartialView(model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public ActionResult DocumentoBIAPartialView(DocumentosModel model)
        {
            string _IdModulo = Session["modId"].ToString();
            int IdTipoDocumento = int.Parse(_IdModulo.Substring(0, (_IdModulo.Length == 7 ? 1 : 2)));
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.Documentos = Metodos.GetDocumentosModulo(IdTipoDocumento, (model.IdClaseDocumento == 1));
            model.Perfil = Metodos.GetPerfilData();
            model.ModulosPrincipales = Metodos.GetModulosPrincipalesEmpresaUsuario();
            model.PageTitle = Resources.DocumentoResource.DocumentosPageTitle;
            model.IdModulo = long.Parse(_IdModulo);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return PartialView(model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public ActionResult DocumentoBCPPartialView(DocumentosModel model)
        {
            string _IdModulo = Session["modId"].ToString();
            int IdTipoDocumento = int.Parse(_IdModulo.Substring(0, (_IdModulo.Length == 7 ? 1 : 2)));
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.Documentos = Metodos.GetDocumentosModulo(IdTipoDocumento, (model.IdClaseDocumento == 1));
            model.Perfil = Metodos.GetPerfilData();
            model.ModulosPrincipales = Metodos.GetModulosPrincipalesEmpresaUsuario();
            model.PageTitle = Resources.DocumentoResource.DocumentosPageTitle;
            model.IdModulo = long.Parse(_IdModulo);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return PartialView(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult EditarDocumento(long IdDocumento, int IdClaseDocumento, long IdModulo, int IdVersion = 0)
        {

            Session["IdDocumento"] = IdDocumento;
            Session["IdClaseDocumento"] = IdClaseDocumento;
            Session["IdVersion"] = IdVersion;
            Session["OnlyVisible"] = false;

            FirstModuloSelected firstModulo = Metodos.GetFirstUserModulo(IdModulo);

            return RedirectToAction(firstModulo.Action, firstModulo.Controller, new { modId = firstModulo.IdModulo });

        }
        [SessionExpire]
        [HandleError]
        public ActionResult EliminarDocumento(long IdDocumento, long IdModulo)
        {
            string _IdModulo = IdModulo.ToString();
            int IdTipoDocumento = int.Parse(_IdModulo.Substring(0, (_IdModulo.Length == 7 ? 1 : 2)));

            Metodos.EliminarDocumento(IdDocumento, IdTipoDocumento);
            return RedirectToAction("Index", new { IdModulo = IdModulo });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult VerDocumento(long IdDocumento, int IdClaseDocumento, long IdModulo, int IdVersion = 0)
        {

            Session["IdDocumento"] = IdDocumento;
            Session["IdClaseDocumento"] = IdClaseDocumento;
            Session["IdVersion"] = IdVersion;
            Session["OnlyVisible"] = true;

            FirstModuloSelected firstModulo = Metodos.GetFirstUserModulo(IdModulo);

            return RedirectToAction(firstModulo.Action, firstModulo.Controller, new { modId = firstModulo.IdModulo });

        }
        [SessionExpire]
        [HandleError]
        public ActionResult NuevaVersionDocumento(long IdDocumento, int IdClaseDocumento, long IdModulo, int IdVersionActual, int NroVersion)
        {
            int IdVersion = NroVersion++;
            string _IdModulo = IdModulo.ToString();
            Session["IdTipoDocumento"] = _IdModulo.Substring(0, (_IdModulo.Length == 7 ? 1 : 2));
            long IdNuevoDocumento = Metodos.GenerarNuevaVersionDocumento(IdDocumento, IdVersionActual, NroVersion);

            return RedirectToAction("EditarDocumento", new { IdDocumento = IdNuevoDocumento, IdClaseDocumento, IdModulo, IdVersion });
        }

    }
}