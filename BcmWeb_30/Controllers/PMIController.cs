using BcmWeb_30.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BcmWeb_30 .Controllers
{
    [Authorize]
    public class PMIController : Controller
    {
        [SessionExpire]
        [HandleError]
        public ActionResult Incidentes(long IdClaseDocumento, long modId)
        {
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));
            Session["IdClaseDocumento"] = IdClaseDocumento;
            Session["modId"] = modId;

            long IdModulo = IdTipoDocumento * 1000000;

            PMIModel model = new PMIModel
            {
                IdModuloActual = modId,
                IdModulo = IdModulo,
                PageTitle = Metodos.GetModuloName(modId),
                IdClaseDocumento = (int)IdClaseDocumento,
                returnPage = Url.Action("Index", "Documento", new { IdModulo = modId }),
                IdEmpresa = long.Parse(Session["IdEmpresa"].ToString())
            };
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            Session["GridViewData"] = Metodos.GetIncidentes();

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ExportIncidentes(PMIModel model)
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

            Session["GridViewData"] = Metodos.GetIncidentes();
            return GridViewExportIncidentes.FormatConditionsExportFormatsInfo[GridViewExportFormat.Xlsx](GridViewExportIncidentes.FormatConditionsExportGridViewSettings, Metodos.GetIncidentes());

        }
        [SessionExpire]
        [HandleError]
        public ActionResult PMIPartialView()
        {
            Session["GridViewData"] = Metodos.GetIncidentes();
            return PartialView("PMIPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditIncidenteAddNewPartial(PMIModel Incidente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.InsertIncidente(Incidente);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableIncidente"] = Incidente;
            }
            Session["GridViewData"] = Metodos.GetIncidentes();
            return PartialView("PMIPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditIncidenteUpdatePartial(PMIModel Incidente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string DatosActualizados = Metodos.GetDatosIncidenteActualizados(Incidente);
                    Metodos.UpdateIncidente(Incidente);
                    Auditoria.RegistarIncidente(eTipoAccion.ActualizarIncidente, Incidente.IdIncidente, string.Empty, DatosActualizados);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableIncidente"] = Incidente;
            }

            Session["GridViewData"] = Metodos.GetIncidentes();
            return PartialView("PMIPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditIncidenteDeletePartial(int IdIncidente)
        {
            if (IdIncidente > 0)
            {
                try
                {
                    Metodos.DeleteIncidente(IdIncidente);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            Session["GridViewData"] = Metodos.GetIncidentes();
            return PartialView("PMIPartialView");
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public JsonResult Start(long IdIncidente)
        {
            PDFpmi _pdfManager = new PDFpmi();
            string _rutaDocumento = _pdfManager.Generar_Documento(true, IdIncidente);
            return Json(new { _rutaDocumento });
        }
    }
}