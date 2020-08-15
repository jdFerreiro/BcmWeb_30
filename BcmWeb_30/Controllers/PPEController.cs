using BcmWeb_30.Models;
using DevExpress.Web.Mvc;
using DevExpress.Web.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
    
namespace BcmWeb_30 .Controllers
{
    [Authorize]
    public class PPEController : Controller
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

            PruebaModel model = new PruebaModel();
            model.IdModuloActual = modId;
            model.IdModulo = IdModulo;
            model.PageTitle = Metodos.GetModuloName(modId);
            model.IdClaseDocumento = (int)IdClaseDocumento;
            model.returnPage = Url.Action("Index", "Documento", new { IdModulo = modId });
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());



            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View();
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ExportPrueba(PruebaModel model)
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

            Session["GridViewData"] = Metodos.GetPruebas();
            return GridViewExportProgramaciones.FormatConditionsExportFormatsInfo[GridViewExportFormat.Xlsx](GridViewExportProgramaciones.FormatConditionsExportGridViewSettings, Metodos.GetProgramaciones());

        }
        [SessionExpire]
        [HandleError]
        public ActionResult PruebasPartialView()
        {
            return PartialView("PruebasPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult PruebasPartialView(PruebaModel model)
        {
            return PartialView("PruebasPartialView");
        }
        [SessionExpire]
        [HandleError]
        public ActionResult EditarPrueba(long IdPrueba, bool Ejecucion)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());
            Session["IdPrueba"] = IdPrueba;

            string UserTimeZone = Session["UserTimeZone"].ToString();
            int Horas = int.Parse(UserTimeZone.Split(':').First());
            int Minutos = (Math.Abs(Horas) * 60) + int.Parse(UserTimeZone.Split(':').Last());
            if (Horas < 0) Minutos *= -1;

            PruebaModel model = new PruebaModel();
            model.PageTitle = Metodos.GetModuloName(modId);

            if (IdPrueba == -1)
            {
                model.FechaInicio = DateTime.UtcNow.AddMinutes(Minutos);
                model.IdClaseDocumento = IdClaseDocumento;
                model.IdEmpresa = IdEmpresa;
                model.IdPrueba = -1;
                model.Nombre = string.Empty;
                model.Proposito = string.Empty;
                model.Ubicacion = string.Empty;
            }
            else
            {
                model = Metodos.GetPrueba(IdPrueba);
            }

            model.returnPage = Url.Action("Index", "PPE", new { IdClaseDocumento, modId });
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult EditarPrueba(PruebaModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.IdPrueba == -1)
                {
                    model.IdPrueba = Metodos.AddPrueba(model);
                    Session["IdPrueba"] = model.IdPrueba;
                }
                else
                {
                    Metodos.UpdatePrueba(model);
                }
            }

            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());
            model.PageTitle = Metodos.GetModuloName(modId);
            model.returnPage = Url.Action("Index", "PPE", new { IdClaseDocumento, modId });
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Participantes(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            List<PruebaParticipanteModel> model = Metodos.GetPruebaParticipantes(IdPrueba);
            return View();
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ParticipantesPartialView()
        {
            return PartialView("ParticipantesPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult ParticipanteAddNewPartial(PruebaParticipanteModel data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.AddParticipante(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("ParticipantesPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult ParticipanteUpdatePartial(PruebaParticipanteModel data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.UpdateParticipante(data);
                    // Auditoria.RegistarParticipante(eTipoAccion.ActualizarParticipante, Participante.IdParticipante, Participante.Nombre, DatosActualizados);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("ParticipantesPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult ParticipanteDeletePartial(int IdParticipante)
        {
            if (IdParticipante > 0)
            {
                try
                {
                    Metodos.DeleteParticipante(IdParticipante);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("ParticipantesPartialView");
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Ejercicios(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            return View();
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult EjerciciosPartialView()
        {
            return PartialView("EjerciciosPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EjercicioAddNewPartial(PruebaEjercicioModel data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.AddEjercicio(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("EjerciciosPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EjercicioUpdatePartial(PruebaEjercicioModel data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.UpdateEjercicio(data);
                    // Auditoria.RegistarEjercicio(eTipoAccion.ActualizarEjercicio, Ejercicio.IdEjercicio, Ejercicio.Nombre, DatosActualizados);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("EjerciciosPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EjercicioDeletePartial(int IdEjercicio)
        {
            if (IdEjercicio > 0)
            {
                try
                {
                    Metodos.DeleteEjercicio(IdEjercicio);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("EjerciciosPartialView");
        }
        [SessionExpire]
        [HttpPost]
        public ActionResult ComboBoxEjercicioPartial()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ParticipantesEjercicio(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            List<PruebaEjercicioModel> Ejercicios = Metodos.GetPruebaEjercicios(IdPrueba);
            long idEjercicio = (Ejercicios.Count > 0 ? Ejercicios.FirstOrDefault().IdEjercicio : 0);
            Session["IdEjercicio"] = idEjercicio;
            PruebaEjercicioModel model = new PruebaEjercicioModel
            {
                IdEmpresa = IdEmpresa,
                IdClaseDocumento = IdClaseDocumento,
                IdModulo = modId,
                IdPrueba = IdPrueba,
                IdEjercicio = idEjercicio,
            };
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ParticipantesEjercicio(PruebaEjercicioModel model)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());
            Session["IdEjercicio"] = model.IdEjercicio;

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ParticipantesEjercicioPartialView(PruebaEjercicioModel model)
        {
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            return PartialView("ParticipantesEjercicioPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult ParticipantesEjercicioAddNewPartial(PruebaParticipanteEjercicioModel data)
        {
            PruebaEjercicioModel model = new PruebaEjercicioModel();
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    data.IdEjercicio = model.IdEjercicio;
                    Metodos.AddParticipanteEjercicio(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("ParticipantesEjercicioPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult ParticipantesEjercicioUpdatePartial(PruebaParticipanteEjercicioModel data)
        {
            PruebaEjercicioModel model = new PruebaEjercicioModel();
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.UpdateParticipanteEjercicio(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("ParticipantesEjercicioPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult ParticipantesEjercicioDeletePartial(int IdEjercicio, int idParticipante)
        {
            PruebaEjercicioModel model = new PruebaEjercicioModel();
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            if (IdEjercicio > 0)
            {
                try
                {
                    Metodos.DeleteParticipanteEjercicio(IdEjercicio, idParticipante);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("ParticipantesEjercicioPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult RecursosEjercicio(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            List<PruebaEjercicioModel> Ejercicios = Metodos.GetPruebaEjercicios(IdPrueba);
            long idEjercicio = (Ejercicios.Count > 0 ? Ejercicios.FirstOrDefault().IdEjercicio : 0);
            Session["IdEjercicio"] = idEjercicio;
            PruebaEjercicioModel model = new PruebaEjercicioModel
            {
                IdEmpresa = IdEmpresa,
                IdClaseDocumento = IdClaseDocumento,
                IdModulo = modId,
                IdPrueba = IdPrueba,
                IdEjercicio = idEjercicio,
            };
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult RecursosEjercicio(PruebaEjercicioModel model)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());
            Session["IdEjercicio"] = model.IdEjercicio;

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult RecursosEjercicioPartialView(PruebaEjercicioModel model)
        {
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());
            model.CanActivate = Metodos.CanActivate(model.IdPrueba);

            return PartialView("RecursosEjercicioPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult RecursosEjercicioAddNewPartial(PruebaRecursoEjercicioModel data)
        {
            PruebaEjercicioModel model = new PruebaEjercicioModel();
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    data.IdEjercicio = model.IdEjercicio;
                    Metodos.AddRecursoEjercicio(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("RecursosEjercicioPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult RecursosEjercicioUpdatePartial(PruebaRecursoEjercicioModel data)
        {
            PruebaEjercicioModel model = new PruebaEjercicioModel();
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.UpdateRecursoEjercicio(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("RecursosEjercicioPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult RecursosEjercicioDeletePartial(int IdEjercicio, int idRecurso)
        {
            PruebaEjercicioModel model = new PruebaEjercicioModel();
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            if (IdEjercicio > 0)
            {
                try
                {
                    Metodos.DeleteRecursoEjercicio(IdEjercicio, idRecurso);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("RecursosEjercicioPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ActivarPlan(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            Metodos.ActivarPrueba(IdPrueba);
            return RedirectToAction("Index", new { IdClaseDocumento, modId });
        }

        [SessionExpire]
        [HandleError]
        public ActionResult EditarEjecucion(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            string UserTimeZone = Session["UserTimeZone"].ToString();
            int Horas = int.Parse(UserTimeZone.Split(':').First());
            int Minutos = (Math.Abs(Horas) * 60) + int.Parse(UserTimeZone.Split(':').Last());
            if (Horas < 0) Minutos *= -1;

            PruebaEjecucionModel model = Metodos.GetEjecucion(IdPrueba);
            model.PageTitle = Metodos.GetModuloName(modId);
            model.returnPage = Url.Action("Index", "PPE", new { IdClaseDocumento, modId });
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult EditarEjecucion(PruebaEjecucionModel model)
        {
            if (ModelState.IsValid)
            {
                Metodos.UpdateEjecucion(model);
                return RedirectToAction("ParticipantesEjecucion", new { IdPrueba = model.IdPrueba });
            }
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());
            model.PageTitle = Metodos.GetModuloName(modId);
            model.returnPage = Url.Action("Index", "PPE", new { IdClaseDocumento, modId });
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ParticipantesEjecucion(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());
            Session["IdPrueba"] = IdPrueba;

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            List<PruebaParticipanteModel> model = Metodos.GetPruebaEjecucionParticipantes(IdPrueba);
            return View();
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ParticipantesEjecucionPartialView()
        {
            return PartialView("ParticipantesEjecucionPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult ParticipanteEjecucionAddNewPartial(PruebaParticipanteModel data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.AddParticipanteEjecucion(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("ParticipantesEjecucionPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult ParticipanteEjecucionUpdatePartial(PruebaParticipanteModel data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.UpdateParticipanteEjecucion(data);
                    // Auditoria.RegistarParticipante(eTipoAccion.ActualizarParticipante, Participante.IdParticipante, Participante.Nombre, DatosActualizados);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("ParticipantesEjecucionPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult ParticipanteEjecucionDeletePartial(int IdParticipante)
        {
            if (IdParticipante > 0)
            {
                try
                {
                    Metodos.DeleteParticipanteEjecucion(IdParticipante);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("ParticipantesEjecucionPartialView");
        }
        [SessionExpire]
        [HandleError]
        public ActionResult EjerciciosEjecucion(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            return View();
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult EjerciciosEjecucionPartialView()
        {
            return PartialView("EjerciciosEjecucionPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EjercicioEjecucionAddNewPartial(PruebaEjercicioEjecucionModel data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.AddEjercicioEjecucion(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("EjerciciosEjecucionPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EjercicioEjecucionUpdatePartial(PruebaEjercicioEjecucionModel data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (data.IdStatus > 0) data.Ejecutado = true;
                    Metodos.UpdateEjercicioEjecucion(data);
                    // Auditoria.RegistarEjercicio(eTipoAccion.ActualizarEjercicio, Ejercicio.IdEjercicio, Ejercicio.Nombre, DatosActualizados);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("EjerciciosEjecucionPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EjercicioEjecucionDeletePartial(int IdEjercicio)
        {
            if (IdEjercicio > 0)
            {
                try
                {
                    Metodos.DeleteEjercicioEjecucion(IdEjercicio);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("EjerciciosEjecucionPartialView");
        }
        [SessionExpire]
        [HttpPost]
        public ActionResult ComboBoxEjercicioEjecucionPartial()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ParticipantesEjecucionEjercicio(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            List<PruebaEjercicioEjecucionModel> Ejercicios = Metodos.GetPruebaEjerciciosEjecucion(IdPrueba);
            PruebaEjercicioModel model = new PruebaEjercicioModel
            {
                IdEmpresa = IdEmpresa,
                IdClaseDocumento = IdClaseDocumento,
                IdModulo = modId,
                IdPrueba = IdPrueba,
                IdEjercicio = (Ejercicios.Count > 0 ? Ejercicios.FirstOrDefault().IdEjercicio : 0),
            };
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ParticipantesEjecucionEjercicio(PruebaEjercicioModel model)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());
            Session["IdEjercicio"] = model.IdEjercicio;

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ParticipantesEjercicioEjecucionPartialView(PruebaEjercicioModel model)
        {
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            return PartialView("ParticipantesEjercicioEjecucionPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult ParticipantesEjercicioEjecucionAddNewPartial(PruebaParticipanteEjercicioModel data)
        {
            PruebaEjercicioModel model = new PruebaEjercicioModel();
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    data.IdEjercicio = model.IdEjercicio;
                    Metodos.AddParticipanteEjercicioEjecucion(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("ParticipantesEjercicioEjecucionPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult ParticipantesEjercicioEjecucionUpdatePartial(PruebaParticipanteEjercicioModel data)
        {
            PruebaEjercicioModel model = new PruebaEjercicioModel();
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.UpdateParticipanteEjercicioEjecucion(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("ParticipantesEjercicioEjecucionPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult ParticipantesEjercicioEjecucionDeletePartial(int IdEjercicio, int idParticipante)
        {
            PruebaEjercicioModel model = new PruebaEjercicioModel();
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            if (IdEjercicio > 0)
            {
                try
                {
                    Metodos.DeleteParticipanteEjercicioEjecucion(IdEjercicio, idParticipante);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("ParticipantesEjercicioEjecucionPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult RecursosEjecucionEjercicio(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            List<PruebaEjercicioEjecucionModel> Ejercicios = Metodos.GetPruebaEjerciciosEjecucion(IdPrueba);
            PruebaEjercicioModel model = new PruebaEjercicioModel
            {
                IdEmpresa = IdEmpresa,
                IdClaseDocumento = IdClaseDocumento,
                IdModulo = modId,
                IdPrueba = IdPrueba,
                IdEjercicio = (Ejercicios.Count > 0 ? Ejercicios.FirstOrDefault().IdEjercicio : 0),
            };
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult RecursosEjecucionEjercicio(PruebaEjercicioModel model)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());
            Session["IdEjercicio"] = model.IdEjercicio;

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult RecursosEjecucionEjercicioPartialView(PruebaEjercicioModel model)
        {
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            return PartialView("RecursosEjercicioEjecucionPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult RecursosEjecucionEjercicioAddNewPartial(PruebaRecursoEjercicioModel data)
        {
            PruebaEjercicioModel model = new PruebaEjercicioModel();
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    data.IdEjercicio = model.IdEjercicio;
                    Metodos.AddRecursoEjecucionEjercicio(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("RecursosEjercicioEjecucionPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult RecursosEjecucionEjercicioUpdatePartial(PruebaRecursoEjercicioModel data)
        {
            PruebaEjercicioModel model = new PruebaEjercicioModel();
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.UpdateRecursoEjecucionEjercicio(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = Resources.ErrorPageResource.AllErrors;
                ViewData["EditableReg"] = data;
            }

            return PartialView("RecursosEjercicioEjecucionPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult RecursosEjecucionEjercicioDeletePartial(int IdEjercicio, int idRecurso)
        {
            PruebaEjercicioModel model = new PruebaEjercicioModel();
            model.IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdModulo = long.Parse(Session["modId"].ToString());
            model.IdEjercicio = long.Parse(Session["IdEjercicio"].ToString());
            model.IdPrueba = long.Parse(Session["IdPrueba"].ToString());

            if (IdEjercicio > 0)
            {
                try
                {
                    Metodos.DeleteRecursoEjecucionEjercicio(IdEjercicio, idRecurso);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("RecursosEjercicioEjecucionPartialView", model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult EditorHallazgos(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());
            Session["IdPrueba"] = IdPrueba;
            Session["IdContenido"] = 1;

            PruebaEjecucionModel ejecucion = Metodos.GetEjecucion(IdPrueba);
            byte[] Contenido = Metodos.GetHallazgoEjecucion(IdPrueba);

            string pageTitle = string.Format(Resources.PPEResource.TitleHallazgosEjecucion, ejecucion.Nombre);
            ViewBag.Title = string.Format("{0} - {1}", pageTitle, Resources.BCMWebPublic.labelAppTitle);

            string uniqueId = ejecucion.IdEmpresa.ToString().Trim() + ejecucion.IdPrueba.ToString().Trim() + "1";
            Session["Editable"] = true;
            Session["Saved"] = true;
            ViewBag.pageTitle = pageTitle;
            if (Contenido == null)
            {
                Session["Contenido"] = Resources.BCMWebPublic.stringEmptyContenido;
            }
            else
            {
                Session["Contenido"] = Convert.ToBase64String(Contenido);
            }
            Session["UniqueId"] = uniqueId;
            DocumentManager.CloseDocument(uniqueId);

            return View();
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public ActionResult EditorHallazgos()
        {
            long IdPrueba = long.Parse(Session["IdPrueba"].ToString());
            PruebaEjecucionModel ejecucion = Metodos.GetEjecucion(IdPrueba);
            string pageTitle = string.Format(Resources.PPEResource.TitleHallazgosEjecucion, ejecucion.Nombre);
            ViewBag.pageTitle = pageTitle;
            return View();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult EditorAcciones(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());
            Session["IdPrueba"] = IdPrueba;
            Session["IdContenido"] = 2;

            PruebaEjecucionModel ejecucion = Metodos.GetEjecucion(IdPrueba);
            byte[] Contenido = Metodos.GetAccionesEjecucion(IdPrueba);

            string pageTitle = string.Format(Resources.PPEResource.TitleAccionesEjecucion, ejecucion.Nombre);
            ViewBag.Title = string.Format("{0} - {1}", pageTitle, Resources.BCMWebPublic.labelAppTitle);

            string uniqueId = ejecucion.IdEmpresa.ToString().Trim() + ejecucion.IdPrueba.ToString().Trim() + "2";
            Session["Editable"] = true;
            Session["Saved"] = true;
            ViewBag.pageTitle = pageTitle;
            if (Contenido == null)
            {
                Session["Contenido"] = Resources.BCMWebPublic.stringEmptyContenido;
            }
            else
            {
                Session["Contenido"] = Convert.ToBase64String(Contenido);
            }
            Session["UniqueId"] = uniqueId;
            DocumentManager.CloseDocument(uniqueId);

            return View();
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public ActionResult EditorAcciones()
        {
            long IdPrueba = long.Parse(Session["IdPrueba"].ToString());
            PruebaEjecucionModel ejecucion = Metodos.GetEjecucion(IdPrueba);
            string pageTitle = string.Format(Resources.PPEResource.TitleHallazgosEjecucion, ejecucion.Nombre);
            ViewBag.pageTitle = pageTitle;
            return View();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult EditorPartialView()
        {
            return PartialView("EditorPartialView");
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Open(string Accion)
        {
            long IdPrueba = long.Parse(Session["IdPrueba"].ToString());
            long IdContenido = long.Parse(Session["IdContenido"].ToString());
            byte[] Contenido;

            switch (Accion)
            {
                case "Save":
                    Contenido = RichEditExtension.SaveCopy("RichEdit", DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    var reString = Encoding.Default.GetString(Contenido);
                    Metodos.UpdateResultadoEjecucion(IdPrueba, IdContenido, Contenido);
                    break;

            }
            // return PartialView("EditorPartialView");
            if (IdContenido == 1)
                return RedirectToAction("EditorHallazgos", new { IdPrueba });
            else
                return RedirectToAction("EditorAcciones", new { IdPrueba });

        }
        [SessionExpire]
        public JsonResult SaveDocument()
        {
            bool success = false;

            byte[] Contenido = RichEditExtension.SaveCopy("RichEdit", DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
            var reString = Encoding.Default.GetString(Contenido);
            long IdPrueba = long.Parse(Session["IdPrueba"].ToString());
            long IdContenido = long.Parse(Session["IdContenido"].ToString());
            success = Metodos.UpdateResultadoEjecucion(IdPrueba, IdContenido, Contenido);

            return Json(new { success });
        }
        [SessionExpire]
        [HandleError]
        public JsonResult docChange()
        {
            Session["Saved"] = false;
            bool success = true;
            return Json(new { success });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult FinalizarPrueba(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            Metodos.FinalizarPrueba(IdPrueba);
            return RedirectToAction("GenerarInforme", new { IdPrueba, InformeFinal = true });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult SuspenderPrueba(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            Metodos.SuspenderPrueba(IdPrueba);
            return RedirectToAction("Index", new { IdClaseDocumento, modId });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult GenerarInforme(long IdPrueba, bool InformeFinal = false)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            Session["IdPrueba"] = IdPrueba;
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            PDFPrueba _pdfManager = new PDFPrueba();
            if (!InformeFinal)
                _pdfManager.GenerarPBE_Preliminar(true);
            else
                _pdfManager.GenerarPBE_Documento(true);


            string _ServerPath = HttpContext.Server.MapPath(".").Replace("\\PPE", string.Empty);
            string _FileName = string.Format("PBE{0}{1}.pdf", IdEmpresa.ToString(), IdPrueba.ToString());
            string _pathFile = string.Format("{0}\\PDFDocs\\{1}", _ServerPath, _FileName);

            return RedirectToAction("Index", new { IdClaseDocumento, modId });
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public JsonResult Start(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            Session["IdPrueba"] = IdPrueba;
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            PDFPrueba _pdfManager = new PDFPrueba();
            string _pathFile = _pdfManager.GenerarPBE_Documento(false);
            return Json(new { _pathFile });
        }

        [SessionExpire]
        [HandleError]
        public ActionResult EliminarPrueba(long IdPrueba)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            long modId = long.Parse(Session["modId"].ToString());

            ViewBag.Title = string.Format("{0} - {1}", Metodos.GetModuloName(modId), Resources.BCMWebPublic.labelAppTitle);
            Metodos.EliminarPrueba(IdPrueba);
            return RedirectToAction("Index", new { IdClaseDocumento, modId });
        }

    }
}
