using BcmWeb_30.Models;
using DevExpress.Web.Mvc;
using System;
using System.Web.Mvc;

namespace BcmWeb_30 .Controllers
{
    [Authorize]
    public class PlanTrabajoController : Controller
    {
        public object RootFolder = "~/Content/FileManager/AnexosPlanTrabajo";

        [SessionExpire]
        [HandleError]
        public ActionResult Index(long IdModulo)
        {
            FirstModuloSelected firstModulo = Metodos.GetFirstUserModulo(IdModulo);

            return RedirectToAction(firstModulo.Action, firstModulo.Controller, new { modId = firstModulo.IdModulo });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Iniciativas(long modId)
        {

            Session["modId"] = modId;

            IniciativaModel model = new IniciativaModel();
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            Auditoria.RegistarAccion(eTipoAccion.MostrarIniciativa);
            Session["GridViewData"] = Metodos.GetIniciativas();
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ExportIniciativas(IniciativaModel model)
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

            Session["GridViewData"] = Metodos.GetIniciativas();
            return GridViewExportIniciativas.FormatConditionsExportFormatsInfo[GridViewExportFormat.Xlsx](GridViewExportIniciativas.FormatConditionsExportGridViewSettings, Metodos.GetIniciativas());

        }
        [SessionExpire]
        [HandleError]
        //[ValidateInput(false)]
        public ActionResult IniciativaPartialView()
        {
            Session["GridViewData"] = Metodos.GetIniciativas();
            return PartialView("IniciativaPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditIniciativaAddNewPartial(IniciativaModel Iniciativa)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.InsertIniciativa(Iniciativa);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableIniciativa"] = Iniciativa;
            }
            Session["GridViewData"] = Metodos.GetIniciativas();
            return PartialView("IniciativaPartialView", Metodos.GetIniciativas());
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditIniciativaUpdatePartial(IniciativaModel Iniciativa)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string DatosActualizados = Metodos.GetDatosIniciativaActualizados(Iniciativa);
                    Metodos.UpdateIniciativa(Iniciativa);
                    Auditoria.RegistarIniciativa(eTipoAccion.ActualizarIniciativa, Iniciativa.IdIniciativa, Iniciativa.Nombre, DatosActualizados);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableIniciativa"] = Iniciativa;
            }

            Session["GridViewData"] = Metodos.GetIniciativas();
            return PartialView("IniciativaPartialView", Metodos.GetIniciativas());
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditIniciativaDeletePartial(int IdIniciativa)
        {
            if (IdIniciativa > 0)
            {
                try
                {
                    Metodos.DeleteIniciativa(IdIniciativa);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            Session["GridViewData"] = Metodos.GetIniciativas();
            return PartialView("IniciativaPartialView", Metodos.GetIniciativas());
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AnexosIniciativaPartialView(long IdIniciativa)
        {
            ViewData["IdIniciativa"] = IdIniciativa;
            return PartialView("AnexosIniciativaPartialView", Metodos.GetAnexosIniciativas(IdIniciativa));
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult AnexosIniciativaPartialView(AnexosIniciativaModel Iniciativa)
        {
            long IdIniciativa = Iniciativa.IdIniciativa;
            ViewData["IdIniciativa"] = IdIniciativa;
            return PartialView("AnexosIniciativaPartialView", Metodos.GetAnexosIniciativas(IdIniciativa));
        }
        [SessionExpire]
        [HandleError]
        [ValidateInput(false)]
        public ActionResult FileManagerPartial(string IdIniciativa)
        {
            if (IdIniciativa != null)
                Session["IdIniciativa"] = IdIniciativa;
            return PartialView("FileManagerPartial", FileManagerPlanTrabajoControllerFileManagerSettings.Model);
        }
        [SessionExpire]
        [HandleError]
        public FileStreamResult FileManagerPartialDownload()
        {
            return FileManagerExtension.DownloadFiles(FileManagerPlanTrabajoControllerFileManagerSettings.CreateFileManagerDownloadSettings(),
                                                      FileManagerPlanTrabajoControllerFileManagerSettings.Model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public JsonResult Start(long IdIniciativa)
        {
            PDFIniciativa _pdfManager = new PDFIniciativa();
            string _rutaDocumento = _pdfManager.Generar_Documento(true, IdIniciativa);
            return Json(new { _rutaDocumento });
        }

    }
}