using BcmWeb_30.Data.EF;
using BcmWeb_30.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BcmWeb_30.Controllers
{
    [Authorize]
    public class AdministracionController : Controller
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
        public ActionResult ComboBoxEmpresa(long IdEmpresa)
        {
            AdministracionModel model = new AdministracionModel();
            model.IdEmpresaSelected = IdEmpresa;
            return PartialView(model);
        }
        public ActionResult ComboBoxModulo(long IdEmpresa, long IdModulo)
        {
            AdministracionModel model = new AdministracionModel();
            model.IdEmpresa = IdEmpresa;
            model.IdModuloActualiza = IdModulo;
            return PartialView(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Auditar(long modId)
        {

            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Length == 7 ? _modId.Substring(0, 1) : _modId.Substring(0, 2));
            long IdModulo = IdTipoDocumento * 1000000;

            string UserTimeZone = Session["UserTimeZone"].ToString();
            int Horas = int.Parse(UserTimeZone.Split(':').First());
            int Minutos = (Math.Abs(Horas) * 60) + int.Parse(UserTimeZone.Split(':').Last());
            if (Horas < 0) Minutos *= -1;

            AuditoriaAdminModel model = new AuditoriaAdminModel();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            model.IdModulo = IdModulo;
            model.Perfil = Metodos.GetPerfilData();
            model.IdModuloActual = modId;
            model.FechaDesde = DateTime.MinValue.AddYears(99);
            model.FechaHasta = DateTime.UtcNow.AddMinutes(Minutos);
            model.Data = Metodos.GetAuditoria(model.FechaDesde
                                                , model.FechaHasta
                                                , (model.IdUsuario < 0 ? null : model.IdUsuario));
            model.FechaDesde = model.Data.Min(x => x.FechaRegistro);
            model.IdUsuario = -1;

            Session["Data"] = model.Data;
            Session["IdModulo"] = modId;
            Session["modId"] = modId;
            Session["IdUsuarioAuditoria"] = model.IdUsuario;
            Auditoria.RegistarAccion(eTipoAccion.AccesoModuloWeb);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult Auditar(AuditoriaAdminModel model)
        {
            string _modId = model.IdModuloActual.ToString();
            int IdTipoDocumento = int.Parse(_modId.Length == 7 ? _modId.Substring(0, 1) : _modId.Substring(0, 2));
            long IdModulo = IdTipoDocumento * 1000000;

            string UserTimeZone = Session["UserTimeZone"].ToString();
            int Horas = int.Parse(UserTimeZone.Split(':').First());
            int Minutos = (Math.Abs(Horas) * 60) + int.Parse(UserTimeZone.Split(':').Last());
            if (Horas < 0) Minutos *= -1;

            model.PageTitle = Metodos.GetModuloName(model.IdModuloActual);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            model.IdModulo = IdModulo;
            model.Perfil = Metodos.GetPerfilData();
            if (model.FechaHasta == DateTime.MinValue)
                model.FechaHasta = DateTime.MaxValue;

            DateTime fechaDesde = (model.FechaDesde != DateTime.MinValue ? model.FechaDesde.AddMinutes(Minutos * -1) : model.FechaDesde);
            DateTime fechaHasta = (model.FechaHasta != DateTime.MaxValue ? model.FechaHasta.AddMinutes(Minutos * -1) : model.FechaHasta);

            model.Data = Metodos.GetAuditoria(fechaDesde, fechaHasta, (model.IdUsuario < 0 ? null : model.IdUsuario));
            Session["Data"] = model.Data;
            Session["IdUsuarioAuditoria"] = model.IdUsuario;
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AuditoriaPartialView()
        {

            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ComboBoxUsuariosPartialView()
        {

            AuditoriaAdminModel model = new AuditoriaAdminModel();
            model.IdUsuario = long.Parse(Session["IdUsuarioAuditoria"].ToString());

            return PartialView(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Empresa(long modId)
        {
            EmpresaModel model = new EmpresaModel();
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Length == 7 ? _modId.Substring(0, 1) : _modId.Substring(0, 2));
            long IdModulo = IdTipoDocumento * 1000000;
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            model.IdModulo = IdModulo;
            model.Perfil = Metodos.GetPerfilData();
            model.IdModuloActual = modId;
            Session["modId"] = modId;
            Auditoria.RegistarAccion(eTipoAccion.AccesoModuloWeb);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult EmpresaPartialView()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult EditarEmpresa(long IdEmpresa)
        {
            EmpresaModel model = new EmpresaModel();
            model.IdEmpresa = IdEmpresa;

            if (IdEmpresa != -1)
            {
                model = Metodos.GetEmpresa(IdEmpresa);
                Auditoria.RegistarAccion(eTipoAccion.AgregarEmpresa);
            }
            else
                Auditoria.RegistarAccion(eTipoAccion.ActualizarEmpresa);

            model.returnPage = Url.Action("Empresa", "Administracion", new { modId = 11010300 });
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult EditarEmpresa(EmpresaModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImagesProperty.Length > 0 && model.ImagesProperty[0].ContentLength > 0)
                {
                    model.LogoUrl = string.Format("~/Content/LogosEmpresa/{0}", model.ImagesProperty[0].FileName);
                    model.ImagesProperty[0].SaveAs(Server.MapPath(model.LogoUrl));
                }
                model = Metodos.ActualizarEmpresa(model);
                if (!string.IsNullOrEmpty(model.ErrorUpdating))
                {
                    ModelState.AddModelError("", model.ErrorUpdating);
                }
                else
                {
                    Auditoria.RegistarAccion(eTipoAccion.Actualizar);
                }
            }
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult BloquearEmpresa(long IdEmpresa)
        {
            Metodos.BloquearEmpresa(IdEmpresa);
            Auditoria.RegistarAccion(eTipoAccion.BloquearEmpresa);
            return RedirectToAction("Empresa", "Administracion", new { modId = 11010300 });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ActivarEmpresa(long IdEmpresa)
        {
            Metodos.ActivarEmpresa(IdEmpresa);
            Auditoria.RegistarAccion(eTipoAccion.ActivarEmpresa);
            return RedirectToAction("Empresa", "Administracion", new { modId = 11010300 });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult EliminarEmpresa(long IdEmpresa)
        {
            Metodos.DeleteEmpresa(IdEmpresa);
            Auditoria.RegistarAccion(eTipoAccion.EliminarEmpresa);
            return RedirectToAction("Empresa", "Administracion", new { modId = 11010300 });
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult PaisPartialView(EmpresaModel model)
        {
            return PartialView(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult EstadoPartialView(EmpresaModel model)
        {
            //int IdPais = (Request.Params["IdPais"] != null) ? int.Parse(Request.Params["IdPais"]) : -1;
            return PartialView(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult CiudadPartialView(EmpresaModel model)
        {
            //int IdPais = (Request.Params["IdPais"] != null) ? int.Parse(Request.Params["IdPais"]) : -1;
            //int IdEstado = (Request.Params["IdEstado"] != null) ? int.Parse(Request.Params["IdEstado"]) : -1;
            return PartialView(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ModulosAccesoEmpresa(long IdEmpresa)
        {
            EmpresaModel model = new EmpresaModel();
            string _modId = Session["modId"].ToString();
            long modId = long.Parse(_modId);
            int IdTipoDocumento = int.Parse(_modId.Length == 7 ? _modId.Substring(0, 1) : _modId.Substring(0, 2));
            long IdModulo = IdTipoDocumento * 1000000;
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            model.IdModulo = IdModulo;
            model.Perfil = Metodos.GetPerfilData();
            model.IdModuloActual = modId;
            model.IdEmpresa = IdEmpresa;
            Session["IdEmpresaAsignada"] = IdEmpresa;
            model.ModulosAsignados = Metodos.GetModulosEmpresa(IdEmpresa);
            model.returnPage = Url.Action("Empresa", "Administracion", new { modId = 11010300 });

            return View(model);
        }
        [ValidateInput(false)]
        public ActionResult ModulosAccesoEmpresaPartialView()
        {
            long IdEmpresa = long.Parse(Session["IdEmpresaAsignada"].ToString());
            return PartialView("ModulosAccesoEmpresaPartialView", Metodos.GetModulosEmpresa(IdEmpresa));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditModulosEmpresaUpdatePartial(ModuloAsignadoModel data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Metodos.ActualizarModuloEmpresa(data);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = Resources.ErrorResource.AllErrors;
            return PartialView("ModulosAccesoEmpresaPartialView", Metodos.GetModulosEmpresa(data.IdEmpresa));
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Usuarios(long modId)
        {
            UsuariosModel model = new UsuariosModel();
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Length == 7 ? _modId.Substring(0, 1) : _modId.Substring(0, 2));
            long IdModulo = IdTipoDocumento * 1000000;
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            model.IdModulo = IdModulo;
            model.Perfil = Metodos.GetPerfilData();
            model.IdModuloActual = modId;
            model.IdEmpresaSelected = 0;
            Session["modId"] = modId;

            Auditoria.RegistarAccion(eTipoAccion.AccesoModuloWeb);
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult Usuarios(UsuariosModel model)
        {
            long modId = long.Parse(Session["modId"].ToString());
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Length == 7 ? _modId.Substring(0, 1) : _modId.Substring(0, 2));
            long IdModulo = IdTipoDocumento * 1000000;
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            model.IdModulo = IdModulo;
            model.Perfil = Metodos.GetPerfilData();
            model.IdModuloActual = modId;
            Auditoria.RegistarAccion(eTipoAccion.AccesoModuloWeb);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ComboBoxEmpresaUsuario(long IdEmpresa)
        {
            AdministracionModel model = new AdministracionModel();
            model.IdEmpresaSelected = IdEmpresa;
            return PartialView(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult UsuarioPartialView(UsuariosModel model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult EditarUsuario(long IdEmpresa, long IdUsuario)
        {
            PerfilModelView model = new PerfilModelView();
            model.IdUsuario = IdUsuario;

            if (IdUsuario != -1)
            {
                model.IdEmpresa = IdEmpresa;
                Auditoria.RegistarAccion(eTipoAccion.AgregarUsuario);
                model = Metodos.GetUsuarioByEmpresaAndId(IdEmpresa, IdUsuario);
            }
            else
            {
                Auditoria.RegistarAccion(eTipoAccion.ActualizarUsuario);
            }
            model.returnPage = Url.Action("Usuarios", "Administracion", new { modId = 11010100 });
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult EditarUsuario(PerfilModelView model)
        {
            if (ModelState.IsValid)
            {
                model = Metodos.ActualizarUsuario(model);
                if (!string.IsNullOrEmpty(model.ErrorUpdating))
                {
                    ModelState.AddModelError("", model.ErrorUpdating);
                }
                else
                {
                    Auditoria.RegistarAccion(eTipoAccion.Actualizar);
                }
            }
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult BloquearUsuario(long IdUsuario)
        {
            //Metodos.BloquearUsuario(IdUsuario);
            Auditoria.RegistarAccion(eTipoAccion.BloquearUsuario);
            return RedirectToAction("Usuario", "Administracion", new { modId = 11010300 });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ActivarUsuario(long IdUsuario)
        {
            //Metodos.ActivarUsuario(IdUsuario);
            Auditoria.RegistarAccion(eTipoAccion.ActualizarUsuario);
            return RedirectToAction("Usuario", "Administracion", new { modId = 11010300 });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult EliminarUsuario(long IdUsuario)
        {
            //Metodos.DeleteUsuario(IdUsuario);
            Auditoria.RegistarAccion(eTipoAccion.EliminarUsuario);
            return RedirectToAction("Usuario", "Administracion", new { modId = 11010300 });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ModulosAccesoUsuario(long IdEmpresa, long IdUsuario, long IdNivelUsuario)
        {
            ModulosUsuario model = new ModulosUsuario();
            model.IdUsuario = IdUsuario;
            model.IdEmpresaSelected = IdEmpresa;
            model.IdNivelUsuario = IdNivelUsuario;
            model.returnPage = Url.Action("Usuarios", "Administracion", new { modId = 11010100 });
            model.ModuloUsuario = Metodos.GetModulosUsuario(IdEmpresa, IdUsuario);
            Auditoria.RegistarActualizarModulos(eTipoAccion.ActualizarModulosUsuario, IdEmpresa, IdUsuario);

            return View(model);
        }
        //[SessionExpire]
        //[HandleError]
        //[HttpPost]
        //public ActionResult ModulosAccesoUsuario(ModulosUsuario model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        model = Metodos.ActualizarUsuario(model);
        //        if (!string.IsNullOrEmpty(model.ErrorUpdating))
        //        {
        //            ModelState.AddModelError("", model.ErrorUpdating);
        //        }
        //        else
        //        {
        //            Auditoria.RegistarAccion(eTipoAccion.Actualizar);
        //        }
        //    }
        //    return View(model);
        //}
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ModulosAccesoPartialView(ModulosUsuario model)
        {
            model.returnPage = Url.Action("Usuarios", "Administracion", new { modId = 11010100 });
            model.ModuloUsuario = Metodos.GetModulosUsuario(model.IdEmpresaSelected, model.IdUsuario);
            return PartialView(model);

        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public JsonResult SeleccionarModuloUsuario(string idEmpresa, string idModulo, string idUsuario, string Checked)
        {
            string result = string.Empty;
            bool success = false;

            long IdEmpresa = long.Parse(idEmpresa);
            long IdModulo = long.Parse(idModulo);
            long IdUsuario = long.Parse(idUsuario);
            bool _Checked = bool.Parse(Checked);

            using (Entities db = new Entities())
            {

                tblModulo_Usuario moduloUsuario = db.tblModulo_Usuario
                    .FirstOrDefault(x => x.IdEmpresa == IdEmpresa && x.IdModulo == IdModulo && x.IdUsuario == IdUsuario);

                if (_Checked)
                {
                    if (moduloUsuario == null)
                    {
                        moduloUsuario = new tblModulo_Usuario
                        {
                            Actualizar = false,
                            Eliminar = false,
                            IdEmpresa = IdEmpresa,
                            IdModulo = IdModulo,
                            IdUsuario = IdUsuario,
                        };

                        db.tblModulo_Usuario.Add(moduloUsuario);
                    }
                }
                else
                {
                    if (moduloUsuario != null)
                    {
                        db.tblModulo_Usuario.Remove(moduloUsuario);
                    }
                }

                tblModulo modulo = db.tblModulo.FirstOrDefault(x => x.IdEmpresa == IdEmpresa && x.IdModulo == IdModulo);

                try
                {
                    db.SaveChanges();
                    if (modulo.IdModuloPadre > 0)
                    {
                        ProcesarModuloPadre(IdEmpresa, modulo.IdModuloPadre, IdUsuario, _Checked, false, false);
                    }

                    ProcesarModulosHijos(IdEmpresa, modulo.IdModulo, IdUsuario, _Checked, false, false);
                    result = string.Empty;
                    success = true;
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                    success = false;
                }

            }
            return Json(new
            {
                success,
                result
            });
        }

        private void ProcesarModulosHijos(long idEmpresa, long idModulo, long idUsuario, bool Selected, bool Actualiza, bool Elimina)
        {
            using (Entities db = new Entities())
            {

                List<tblModulo> _modulosHijos = db.tblModulo.Where(x => x.IdEmpresa == idEmpresa && x.IdModuloPadre == idModulo).ToList();

                foreach (tblModulo hijo in _modulosHijos)
                {
                    tblModulo_Usuario moduloUsuario = db.tblModulo_Usuario
                        .FirstOrDefault(x => x.IdEmpresa == idEmpresa && x.IdModulo == hijo.IdModulo && x.IdUsuario == idUsuario);

                    if (Selected)
                    {
                        if (moduloUsuario == null)
                        {
                            moduloUsuario = new tblModulo_Usuario
                            {
                                IdEmpresa = idEmpresa,
                                IdModulo = hijo.IdModulo,
                                IdUsuario = idUsuario,
                            };

                            db.tblModulo_Usuario.Add(moduloUsuario);
                        }

                        moduloUsuario.Actualizar = Actualiza;
                        moduloUsuario.Eliminar = Elimina;
                    }
                    else
                    {
                        if (moduloUsuario != null)
                        {
                            db.tblModulo_Usuario.Remove(moduloUsuario);
                        }
                    }

                    db.SaveChanges();
                    ProcesarModulosHijos(idEmpresa, hijo.IdModulo, idUsuario, Selected, Actualiza, Elimina);

                }
            }
        }
        private void ProcesarModuloPadre(long idEmpresa, long idModulo, long idUsuario, bool Selected, bool Actualiza, bool Elimina)
        {
            using (Entities db = new Entities())
            {

                tblModulo _padre = db.tblModulo.FirstOrDefault(x => x.IdEmpresa == idEmpresa && x.IdModulo == idModulo);

                if (_padre != null)
                {

                    tblModulo_Usuario moduloUsuario = db.tblModulo_Usuario
                        .FirstOrDefault(x => x.IdEmpresa == idEmpresa && x.IdModulo == _padre.IdModulo && x.IdUsuario == idUsuario);

                    if (Selected)
                    {
                        if (moduloUsuario == null)
                        {
                            moduloUsuario = new tblModulo_Usuario
                            {
                                IdEmpresa = idEmpresa,
                                IdModulo = _padre.IdModulo,
                                IdUsuario = idUsuario,
                            };

                            db.tblModulo_Usuario.Add(moduloUsuario);
                        }

                        moduloUsuario.Actualizar = Actualiza;
                        moduloUsuario.Eliminar = Elimina;
                    }
                    else
                    {
                        if (moduloUsuario != null)
                        {
                            db.tblModulo_Usuario.Remove(moduloUsuario);
                        }
                    }

                    db.SaveChanges();
                    if (_padre.IdModuloPadre > 0)
                        ProcesarModuloPadre(idEmpresa, _padre.IdModuloPadre, idUsuario, Selected, Actualiza, Elimina);

                }
            }
        }

        [HttpPost]
        [SessionExpire]
        [HandleError]
        public JsonResult ActualizarModuloUsuario(long idEmpresa, long idModulo, long idUsuario, bool Checked)
        {
            string result = string.Empty;
            bool success = false;

            using (Entities db = new Entities())
            {

                tblModulo_Usuario moduloUsuario = db.tblModulo_Usuario
                    .FirstOrDefault(x => x.IdEmpresa == idEmpresa && x.IdModulo == idModulo && x.IdUsuario == idUsuario);

                if (moduloUsuario == null)
                {
                    moduloUsuario = new tblModulo_Usuario
                    {
                        Eliminar = false,
                        IdEmpresa = idEmpresa,
                        IdModulo = idModulo,
                        IdUsuario = idUsuario,
                    };

                    db.tblModulo_Usuario.Add(moduloUsuario);
                }

                moduloUsuario.Actualizar = Checked;
                tblModulo modulo = db.tblModulo.FirstOrDefault(x => x.IdEmpresa == idEmpresa && x.IdModulo == idModulo);

                try
                {
                    db.SaveChanges();
                    result = string.Empty;
                    success = false;
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                    success = true;
                }

            }
            return Json(new
            {
                success,
                result
            });
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public JsonResult EliminarModuloUsuario(long idEmpresa, long idModulo, long idUsuario, bool Checked)
        {
            string result = string.Empty;
            bool success = false;

            using (Entities db = new Entities())
            {

                tblModulo_Usuario moduloUsuario = db.tblModulo_Usuario
                    .FirstOrDefault(x => x.IdEmpresa == idEmpresa && x.IdModulo == idModulo && x.IdUsuario == idUsuario);

                if (moduloUsuario == null)
                {
                    moduloUsuario = new tblModulo_Usuario
                    {
                        IdEmpresa = idEmpresa,
                        IdModulo = idModulo,
                        IdUsuario = idUsuario,
                    };

                    db.tblModulo_Usuario.Add(moduloUsuario);
                }

                moduloUsuario.Actualizar = Checked;
                moduloUsuario.Eliminar = Checked;

                try
                {
                    db.SaveChanges();
                    result = string.Empty;
                    success = false;
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                    success = true;
                }

            }
            return Json(new
            {
                success,
                result
            });
        }

    }
}