using BcmWeb_30.Data.EF;
using BcmWeb_30.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BcmWeb_30 .Controllers
{
    [Authorize]
    public class ProcesosController : Controller
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
        public ActionResult AjustarIF(long modId)
        {

            Session["modId"] = modId;

            ProcesoValoresModel model = new ProcesoValoresModel();
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));
            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            Auditoria.RegistarAccion(eTipoAccion.Mostrar);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult AjustarIF(ProcesoValoresModel model)
        {
            long modId = long.Parse(Session["modId"].ToString());
            string _modId = Session["modId"].ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AjustarIFPartialView(ProcesoValoresModel model)
        {
            return PartialView();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditIfUpdatePartial(ProcesoValoresModel dataImpacto)
        {
            if (ModelState.IsValid)
            {
                using (Entities db = new Entities())
                {
                    if (dataImpacto.IdTipoEscala > 0)
                    {
                        tblBIAProceso _proceso = db.tblBIAProceso.FirstOrDefault(x => x.IdEmpresa == dataImpacto.IdEmpresa
                                                                            && x.IdProceso == dataImpacto.IdProceso);

                        if (_proceso != null)
                        {
                            var regs = db.tblBIAImpactoFinanciero.FirstOrDefault(x => x.IdEmpresa == dataImpacto.IdEmpresa
                                                                              && x.IdDocumentoBIA == _proceso.IdDocumentoBia
                                                                              && x.IdProceso == dataImpacto.IdProceso);
                            regs.IdEscala = dataImpacto.IdTipoEscala;
                            db.SaveChanges();
                        }
                    }
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("AjustarIFPartialView");
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AjustarIo(long modId)
        {

            Session["modId"] = modId;

            ProcesoImpactoModel model = new ProcesoImpactoModel();
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));
            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            Auditoria.RegistarAccion(eTipoAccion.Mostrar);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult AjustarIo(ProcesoImpactoModel model)
        {
            long modId = long.Parse(Session["modId"].ToString());
            string _modId = Session["modId"].ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AjustarIoPartialView(ProcesoImpactoModel model)
        {
            return PartialView();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditIoUpdatePartial(ProcesoImpactoModel dataImpacto)
        {
            if (ModelState.IsValid)
            {
                using (Entities db = new Entities())
                {
                    if (dataImpacto.IdEscala > 0)
                    {
                        var regs = db.tblBIAImpactoOperacional.Where(x => x.ImpactoOperacional == dataImpacto.Impacto).ToList();
                        regs.ForEach(x => x.IdEscala = dataImpacto.IdEscala);
                        db.SaveChanges();
                    }
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("AjustarIoPartialView");
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AjustarMTD(long modId)
        {

            Session["modId"] = modId;

            ProcesoImpactoModel model = new ProcesoImpactoModel();
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));
            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            Auditoria.RegistarAccion(eTipoAccion.Mostrar);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult AjustarMTD(ProcesoImpactoModel model)
        {
            long modId = long.Parse(Session["modId"].ToString());
            string _modId = Session["modId"].ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AjustarMTDPartialView(ProcesoImpactoModel model)
        {
            return PartialView();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditMTDUpdatePartial(ProcesoValoresModel dataImpacto)
        {
            if (ModelState.IsValid)
            {
                using (Entities db = new Entities())
                {
                    if (dataImpacto.IdImpacto == 0)
                    {
                        tblBIAMTD reg = new tblBIAMTD()
                        {
                            Observacion = (dataImpacto.Impacto == null ? string.Empty : dataImpacto.Impacto),
                            IdDocumentoBIA = dataImpacto.IdDocumentoBIA,
                            IdEmpresa = dataImpacto.IdEmpresa,
                            IdEscala = dataImpacto.IdTipoEscala,
                            IdProceso = dataImpacto.IdProceso,
                            IdTipoFrecuencia = 9,
                        };

                        db.tblBIAMTD.Add(reg);
                    }
                    else
                    {
                        tblBIAMTD reg = db.tblBIAMTD.Where(x => x.IdEmpresa == dataImpacto.IdEmpresa
                                                            && x.IdDocumentoBIA == dataImpacto.IdDocumentoBIA
                                                            && x.IdProceso == dataImpacto.IdProceso
                                                            && x.IdMTD == dataImpacto.IdImpacto).FirstOrDefault();

                        reg.IdEscala = dataImpacto.IdTipoEscala;
                    }

                    db.SaveChanges();
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("AjustarMTDPartialView");
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AjustarRTO(long modId)
        {

            Session["modId"] = modId;

            ProcesoImpactoModel model = new ProcesoImpactoModel();
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));
            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            Auditoria.RegistarAccion(eTipoAccion.Mostrar);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult AjustarRTO(ProcesoImpactoModel model)
        {
            long modId = long.Parse(Session["modId"].ToString());
            string _modId = Session["modId"].ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AjustarRTOPartialView(ProcesoImpactoModel model)
        {
            return PartialView();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditRTOUpdatePartial(ProcesoValoresModel dataImpacto)
        {
            if (ModelState.IsValid)
            {
                using (Entities db = new Entities())
                {
                    if (dataImpacto.IdImpacto == 0)
                    {
                        tblBIARTO reg = new tblBIARTO()
                        {
                            Observacion = (dataImpacto.Impacto == null ? string.Empty : dataImpacto.Impacto),
                            IdDocumentoBIA = dataImpacto.IdDocumentoBIA,
                            IdEmpresa = dataImpacto.IdEmpresa,
                            IdEscala = dataImpacto.IdTipoEscala,
                            IdProceso = dataImpacto.IdProceso,
                            IdTipoFrecuencia = 9,
                        };

                        db.tblBIARTO.Add(reg);
                    }
                    else
                    {
                        tblBIARTO reg = db.tblBIARTO.Where(x => x.IdEmpresa == dataImpacto.IdEmpresa
                                                            && x.IdDocumentoBIA == dataImpacto.IdDocumentoBIA
                                                            && x.IdProceso == dataImpacto.IdProceso
                                                            && x.IdRTO == dataImpacto.IdImpacto).FirstOrDefault();

                        reg.IdEscala = dataImpacto.IdTipoEscala;
                    }

                    db.SaveChanges();
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("AjustarRTOPartialView");
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AjustarRPO(long modId)
        {

            Session["modId"] = modId;

            ProcesoImpactoModel model = new ProcesoImpactoModel();
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));
            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            Auditoria.RegistarAccion(eTipoAccion.Mostrar);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult AjustarRPO(ProcesoImpactoModel model)
        {
            long modId = long.Parse(Session["modId"].ToString());
            string _modId = Session["modId"].ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AjustarRPOPartialView(ProcesoImpactoModel model)
        {
            return PartialView();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditRPOUpdatePartial(ProcesoValoresModel dataImpacto)
        {
            if (ModelState.IsValid)
            {
                using (Entities db = new Entities())
                {
                    if (dataImpacto.IdImpacto == 0)
                    {
                        tblBIARPO reg = new tblBIARPO()
                        {
                            Observacion = (dataImpacto.Impacto == null ? string.Empty : dataImpacto.Impacto),
                            IdDocumentoBIA = dataImpacto.IdDocumentoBIA,
                            IdEmpresa = dataImpacto.IdEmpresa,
                            IdEscala = dataImpacto.IdTipoEscala,
                            IdProceso = dataImpacto.IdProceso,
                            IdTipoFrecuencia = 9,
                        };

                        db.tblBIARPO.Add(reg);
                    }
                    else
                    {
                        tblBIARPO reg = db.tblBIARPO.Where(x => x.IdEmpresa == dataImpacto.IdEmpresa
                                                            && x.IdDocumentoBIA == dataImpacto.IdDocumentoBIA
                                                            && x.IdProceso == dataImpacto.IdProceso
                                                            && x.IdRPO == dataImpacto.IdImpacto).FirstOrDefault();

                        reg.IdEscala = dataImpacto.IdTipoEscala;
                    }

                    db.SaveChanges();
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("AjustarRPOPartialView");
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AjustarWRT(long modId)
        {

            Session["modId"] = modId;

            ProcesoImpactoModel model = new ProcesoImpactoModel();
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));
            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            Auditoria.RegistarAccion(eTipoAccion.Mostrar);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult AjustarWRT(ProcesoImpactoModel model)
        {
            long modId = long.Parse(Session["modId"].ToString());
            string _modId = Session["modId"].ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AjustarWRTPartialView(ProcesoImpactoModel model)
        {
            return PartialView();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditWRTUpdatePartial(ProcesoValoresModel dataImpacto)
        {
            if (ModelState.IsValid)
            {
                using (Entities db = new Entities())
                {
                    if (dataImpacto.IdImpacto == 0)
                    {
                        tblBIAWRT reg = new tblBIAWRT()
                        {
                            Observacion = (dataImpacto.Impacto == null ? string.Empty : dataImpacto.Impacto),
                            IdDocumentoBIA = dataImpacto.IdDocumentoBIA,
                            IdEmpresa = dataImpacto.IdEmpresa,
                            IdEscala = dataImpacto.IdTipoEscala,
                            IdProceso = dataImpacto.IdProceso,
                            IdTipoFrecuencia = 9,
                        };

                        db.tblBIAWRT.Add(reg);
                    }
                    else
                    {
                        tblBIAWRT reg = db.tblBIAWRT.Where(x => x.IdEmpresa == dataImpacto.IdEmpresa
                                                            && x.IdDocumentoBIA == dataImpacto.IdDocumentoBIA
                                                            && x.IdProceso == dataImpacto.IdProceso
                                                            && x.IdWRT == dataImpacto.IdImpacto).FirstOrDefault();

                        reg.IdEscala = dataImpacto.IdTipoEscala;
                    }

                    db.SaveChanges();
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("AjustarWRTPartialView");
        }

        [SessionExpire]
        [HandleError]
        public ActionResult Criticos(long modId)
        {

            Session["modId"] = modId;

            CriticoModel model = new CriticoModel();
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));
            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            Auditoria.RegistarAccion(eTipoAccion.Mostrar);

            Session["ValoresIF"] = null;
            Session["ValoresIO"] = null;
            Session["ValoresMTD"] = null;
            Session["ValoresRTO"] = null;
            Session["ValoresRPO"] = null;
            Session["ValoresWRT"] = null;

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult Criticos(CriticoModel model)
        {
            long modId = long.Parse(Session["modId"].ToString());
            string _modId = Session["modId"].ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            Session["ValoresIF"] = model.ImpactoFinancieroSelected;
            Session["ValoresIO"] = model.ImpactoOperacionalSelected;
            Session["ValoresMTD"] = model.MTDSelected;
            Session["ValoresRTO"] = model.RTOSelected;
            Session["ValoresRPO"] = model.RPOSelected;
            Session["ValoresWRT"] = model.WRTSelected;

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult UpdateCriticos(CriticoModel model)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            long modId = long.Parse(Session["modId"].ToString());
            string _modId = Session["modId"].ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            IList<string> ValoresIF = (Session["ValoresIF"] != null ? Session["ValoresIF"].ToString().Split(',').ToList() : new List<string>());
            IList<string> ValoresIO = (Session["ValoresIO"] != null ? Session["ValoresIO"].ToString().Split(',').ToList() : new List<string>());
            IList<string> ValoresMTD = (Session["ValoresMTD"] != null ? Session["ValoresMTD"].ToString().Split(',').ToList() : new List<string>());
            IList<string> ValoresRTO = (Session["ValoresRTO"] != null ? Session["ValoresRTO"].ToString().Split(',').ToList() : new List<string>());
            IList<string> ValoresRPO = (Session["ValoresRPO"] != null ? Session["ValoresRPO"].ToString().Split(',').ToList() : new List<string>());
            IList<string> ValoresWRT = (Session["ValoresWRT"] != null ? Session["ValoresWRT"].ToString().Split(',').ToList() : new List<string>());

            IQueryable<DocumentoProcesoModel> _Procesos = Metodos.GetProcesosByImpacto().AsQueryable();
            using (Entities db = new Entities())
            {
                foreach (DocumentoProcesoModel _proceso in _Procesos)
                {
                    tblBIAProceso proceso = db.tblBIAProceso.FirstOrDefault(x => x.IdEmpresa == IdEmpresa && x.IdProceso == _proceso.IdProceso);
                    if (proceso != null)
                    {
                        proceso.Critico = _proceso.Selected;
                    }

                }

                if (ValoresIF.Count() > 0)
                {
                    foreach (string valor in ValoresIF)
                    {
                        long _valor = long.Parse(valor);
                        tblEscala _escala = db.tblEscala.FirstOrDefault(x => x.IdEmpresa == IdEmpresa && x.IdTipoEscala == 1 && x.IdEscala == _valor);
                        string _Descripcion = _escala.Descripcion;
                        tblCriticidad _criticidad = new tblCriticidad
                        {
                            DescripcionEscala = _Descripcion,
                            IdEmpresa = IdEmpresa,
                            IdTipoEscala = _escala.IdEscala,
                            FechaAplicacion = DateTime.UtcNow,
                        };
                        db.tblCriticidad.Add(_criticidad);
                    }
                }
                if (ValoresIO.Count() > 0)
                {
                    foreach (string valor in ValoresIO)
                    {
                        long _valor = long.Parse(valor);
                        tblEscala _escala = db.tblEscala.FirstOrDefault(x => x.IdEmpresa == IdEmpresa && x.IdTipoEscala == 2 && x.IdEscala == _valor);
                        string _Descripcion = _escala.Descripcion;
                        tblCriticidad _criticidad = new tblCriticidad
                        {
                            DescripcionEscala = _Descripcion,
                            IdEmpresa = IdEmpresa,
                            IdTipoEscala = _escala.IdEscala,
                            FechaAplicacion = DateTime.UtcNow,
                        };
                        db.tblCriticidad.Add(_criticidad);
                    }
                }
                if (ValoresMTD.Count() > 0)
                {
                    foreach (string valor in ValoresMTD)
                    {
                        long _valor = long.Parse(valor);
                        tblEscala _escala = db.tblEscala.FirstOrDefault(x => x.IdEmpresa == IdEmpresa && x.IdTipoEscala == 3 && x.IdEscala == _valor);
                        string _Descripcion = _escala.Descripcion;
                        tblCriticidad _criticidad = new tblCriticidad
                        {
                            DescripcionEscala = _Descripcion,
                            IdEmpresa = IdEmpresa,
                            IdTipoEscala = _escala.IdEscala,
                            FechaAplicacion = DateTime.UtcNow,
                        };
                        db.tblCriticidad.Add(_criticidad);
                    }
                }
                if (ValoresRTO.Count() > 0)
                {
                    foreach (string valor in ValoresRTO)
                    {
                        long _valor = long.Parse(valor);
                        tblEscala _escala = db.tblEscala.FirstOrDefault(x => x.IdEmpresa == IdEmpresa && x.IdTipoEscala == 4 && x.IdEscala == _valor);
                        string _Descripcion = _escala.Descripcion;
                        tblCriticidad _criticidad = new tblCriticidad
                        {
                            DescripcionEscala = _Descripcion,
                            IdEmpresa = IdEmpresa,
                            IdTipoEscala = _escala.IdEscala,
                            FechaAplicacion = DateTime.UtcNow,
                        };
                        db.tblCriticidad.Add(_criticidad);
                    }
                }
                if (ValoresRPO.Count() > 0)
                {
                    foreach (string valor in ValoresRPO)
                    {
                        long _valor = long.Parse(valor);
                        tblEscala _escala = db.tblEscala.FirstOrDefault(x => x.IdEmpresa == IdEmpresa && x.IdTipoEscala == 5 && x.IdEscala == _valor);
                        string _Descripcion = _escala.Descripcion;
                        tblCriticidad _criticidad = new tblCriticidad
                        {
                            DescripcionEscala = _Descripcion,
                            IdEmpresa = IdEmpresa,
                            IdTipoEscala = _escala.IdEscala,
                            FechaAplicacion = DateTime.UtcNow,
                        };
                        db.tblCriticidad.Add(_criticidad);
                    }
                }
                if (ValoresWRT.Count() > 0)
                {
                    foreach (string valor in ValoresWRT)
                    {
                        long _valor = long.Parse(valor);
                        tblEscala _escala = db.tblEscala.FirstOrDefault(x => x.IdEmpresa == IdEmpresa && x.IdTipoEscala == 6 && x.IdEscala == _valor);
                        string _Descripcion = _escala.Descripcion;
                        tblCriticidad _criticidad = new tblCriticidad
                        {
                            DescripcionEscala = _Descripcion,
                            IdEmpresa = IdEmpresa,
                            IdTipoEscala = _escala.IdEscala,
                            FechaAplicacion = DateTime.UtcNow,
                        };
                        db.tblCriticidad.Add(_criticidad);
                    }
                }

                db.SaveChanges();
            }

            return RedirectToAction("Criticos", new
            {
                modId
            });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ImpactoFinancieroPartialView()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ImpactoOperacionalPartialView(CriticoModel model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult MTDPartialView(CriticoModel model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult RPOPartialView(CriticoModel model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult RTOPartialView(CriticoModel model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult WRTPartialView(CriticoModel model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult CriticosPartialView(CriticoModel model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Riesgo(long modId)
        {

            Session["modId"] = modId;

            CriticoModel model = new CriticoModel();
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));
            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            Auditoria.RegistarAccion(eTipoAccion.Mostrar);

            Session["ValoresProbabilidad"] = "";
            Session["ValoresImpacto"] = "";
            Session["ValoresSeveridad"] = "";
            Session["ValoresFuente"] = "";
            Session["ValoresControl"] = "";

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult Riesgo(CriticoModel model)
        {
            long modId = long.Parse(Session["modId"].ToString());
            string _modId = Session["modId"].ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;

            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            Session["ValoresProbabilidad"] = model.ImpactoFinancieroSelected;
            Session["ValoresImpacto"] = model.ImpactoOperacionalSelected;
            Session["ValoresSeveridad"] = model.MTDSelected;
            Session["ValoresFuente"] = model.RPOSelected;
            Session["ValoresControl"] = model.RTOSelected;

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ProbabilidadPartialView()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ImpactoPartialView(CriticoModel model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult SeveridadPartialView(CriticoModel model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult FuentePartialView(CriticoModel model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ControlPartialView(CriticoModel model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult RiesgoPartialView(CriticoModel model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ExportCriticos()
        {
            return GridViewExportCriticos.FormatConditionsExportFormatsInfo[GridViewExportFormat.Xlsx](GridViewExportCriticos.FormatConditionsExportGridViewSettings, Metodos.GetProcesosByImpacto());

        }
        [SessionExpire]
        [HandleError]
        public ActionResult ExportRiesgo()
        {
            return GridViewExportRiesgo.FormatConditionsExportFormatsInfo[GridViewExportFormat.Xlsx](GridViewExportRiesgo.FormatConditionsExportGridViewSettings, Metodos.GetProcesosByRiesgo());

        }
    }
}