using BcmWeb_30.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BcmWeb_30 .Controllers
{
    [Authorize]
    public class PMTController : Controller
    {
        [SessionExpire]
        [HandleError]
        public ActionResult Index(long IdClaseDocumento, long modId)
        {
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));
            Session["IdClaseDocumento"] = IdClaseDocumento;
            Session["modId"] = modId;

            long IdModulo = IdTipoDocumento * 1000000;

            ProgramacionModel model = new ProgramacionModel();
            model.IdModuloActual = modId;
            model.IdModulo = IdModulo;
            model.PageTitle = Resources.BCMWebPublic.captionBotonMantenimientos;
            model.IdClaseDocumento = (int)IdClaseDocumento;
            model.returnPage = Url.Action("Index", "Documento", new { IdModulo = modId });
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ExportProgramacion(ProgramacionModel model)
        {
            string _modId = Session["modId"].ToString();
            long modId = long.Parse(_modId);
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            Session["GridViewData"] = Metodos.GetProgramaciones();
            return GridViewExportProgramaciones.FormatConditionsExportFormatsInfo[GridViewExportFormat.Xlsx](GridViewExportProgramaciones.FormatConditionsExportGridViewSettings, Metodos.GetProgramaciones());

        }
        [SessionExpire]
        [HandleError]
        public ActionResult ProgramacionPartialView()
        {
            return PartialView("ProgramacionPartialView");
        }
        //[SessionExpire]
        //[HandleError]
        //[HttpPost]
        //public ActionResult ProgramacionPartialView(ProgramacionModel model)
        //{
        //    return PartialView("ProgramacionPartialView");
        //}
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditProgramacionAddNewPartial(ProgramacionModel Programacion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string IdModuloProgramacion = Programacion.IdModuloProgramacion.ToString();
                    Programacion.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
                    Programacion.IdTipoDocumento = int.Parse(IdModuloProgramacion.Substring(0, (IdModuloProgramacion.Length == 7 ? 1 : 2)));
                    Programacion.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
                    Programacion.IdEstadoProgramacion = 1;
                    Programacion.IdTipoActualizacion = 1;
                    Metodos.InsertProgramacion(Programacion);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableProgramacion"] = Programacion;
            }
            Session["GridViewData"] = Metodos.GetProgramaciones();
            return PartialView("ProgramacionPartialView", Metodos.GetProgramaciones());
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditProgramacionUpdatePartial(ProgramacionModel Programacion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.UpdateProgramacion(Programacion);
                    Auditoria.RegistarProgramacion(eTipoAccion.ActualizarProgramacion, Programacion.IdProgramacion, Programacion.IdModuloProgramacion, string.Empty);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableProgramacion"] = Programacion;
            }

            Session["GridViewData"] = Metodos.GetProgramaciones();
            return PartialView("ProgramacionPartialView", Metodos.GetProgramaciones());
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditProgramacionDeletePartial(long IdProgramacion)
        {
            if (IdProgramacion > 0)
            {
                try
                {
                    Metodos.DeleteProgramacion(IdProgramacion);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            Session["GridViewData"] = Metodos.GetProgramaciones();
            return PartialView("ProgramacionPartialView", Metodos.GetProgramaciones());
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Documentos(long IdProgramacion)
        {
            //Session["IdClaseDocumento"] = IdClaseDocumento;
            //Session["modId"] = modId;

            //long IdModulo = IdTipoDocumento * 1000000;

            ProgramacionModel reg = Metodos.GetEditableProgramacion(IdProgramacion);
            //model.IdModuloActual = modId;
            //model.IdModulo = IdModulo;
            //model.PageTitle = Metodos.GetModuloName(modId);
            //model.IdClaseDocumento = (int)IdClaseDocumento;
            //model.returnPage = Url.Action("Index", "Documento", new { IdTipoDocumento, modId });
            //model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            //ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            ProgramacionDocumentoModel model = new ProgramacionDocumentoModel
            {
                IdModulo = reg.IdModuloProgramacion,
            };

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult Documentos(ProgramacionDocumentoModel model)
        {
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Usuarios(long IdProgramacion)
        {
            ProgramacionUsuarioModel model = new ProgramacionUsuarioModel
            {

            };

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult Usuarios(ProgramacionUsuarioModel model)
        {
            return View(model);
        }
    }
}