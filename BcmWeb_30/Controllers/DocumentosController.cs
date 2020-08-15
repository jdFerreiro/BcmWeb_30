using BcmWeb_30.Data.EF;
using BcmWeb_30.Models;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Web.Office;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace BcmWeb_30 .Controllers
{
    [Authorize]
    public class DocumentosController : Controller
    {
        [SessionExpire]
        [HandleError]
        public ActionResult Ficha(long modId)
        {

            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;
            Session["IdTipoDocumento"] = IdTipoDocumento;
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            int IdVersion = int.Parse(Session["IdVersion"].ToString());

            DocumentoModel model = new DocumentoModel();
            FirstModuloSelected firstModulo = Metodos.GetFirstUserModulo(IdModulo);

            long IdResponsable = (Session["IdResponsable"] != null ? long.Parse(Session["IdResponsable"].ToString()) : 0);

            if (IdDocumento == 0)
            {
                model = new DocumentoModel
                {
                    Anexos = new List<DocumentoAnexoModel>(),
                    Aprobaciones = new List<DocumentoAprobacionModel>(),
                    Auditoria = new List<DocumentoAuditoriaModel>(),
                    Certificaciones = new List<DocumentoCertificacionModel>(),
                    Contenido = new List<DocumentoContenidoModel>(),
                    Entrevistas = new List<DocumentoEntrevistaModel>(),
                    Estatus = "Cargando",
                    FechaCreacion = DateTime.UtcNow,
                    FechaEstadoDocumento = DateTime.UtcNow,
                    FechaUltimaModificacion = DateTime.UtcNow,
                    IdClaseDocumento = IdClaseDocumento,
                    IdDocumento = IdDocumento,
                    IdEmpresa = long.Parse(Session["IdEmpresa"].ToString()),
                    IdEstatus = 1,
                    IdModulo = IdModulo,
                    IdModuloActual = modId,
                    IdPersonaResponsable = IdResponsable,
                    NroDocumento = Metodos.GetNextNroDocumento(IdClaseDocumento, IdTipoDocumento),
                    NroVersion = Metodos.GetNextVersion(IdClaseDocumento, IdTipoDocumento, IdVersion),
                    PersonasClave = new List<DocumentoPersonaClaveModel>(),
                    RequiereCertificacion = true,
                    returnPage = Url.Action("Index", "Documento", new { IdModulo }),

                    VersionOriginal = IdVersion
                };

            }
            else
            {
                model = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);
            }

            model.Negocios = (IdClaseDocumento == 1);
            model.IdTipoDocumento = IdTipoDocumento;
            model.PageTitle = Metodos.GetModuloName(firstModulo.IdModulo);
            return View(model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public ActionResult Ficha(DocumentoModel model)
        {
            if (model.IdPersonaResponsable > 0)
            {
                PersonaModel persona = Metodos.GetPersonaById(model.IdPersonaResponsable);

                List<DocumentoAprobacionModel> Aprobaciones = model.Aprobaciones.ToList();
                List<DocumentoCertificacionModel> Certificaciones = model.Certificaciones.ToList();
                List<DocumentoPersonaClaveModel> PersonasClave = model.PersonasClave.ToList();

                Aprobaciones.Add(new DocumentoAprobacionModel()
                {
                    Aprobado = false,
                    IdEmpresa = model.IdEmpresa,
                    IdPersona = model.IdPersonaResponsable,
                    Procesado = false,
                    Responsable = true
                });

                Certificaciones.Add(new DocumentoCertificacionModel()
                {
                    Certificado = false,
                    IdEmpresa = model.IdEmpresa,
                    IdPersona = model.IdPersonaResponsable,
                    Procesado = false,
                    Responsable = true
                });

                string Email = (persona.CorreosElectronicos.Where(x => x.IdTipoEmail == 1).FirstOrDefault() != null ? persona.CorreosElectronicos.Where(x => x.IdTipoEmail == 1).FirstOrDefault().Email : string.Empty);
                string Direccion = (persona.Direcciones.Where(x => x.IdTipoDireccion == 1).FirstOrDefault() != null ? persona.Direcciones.Where(x => x.IdTipoDireccion == 1).FirstOrDefault().Ubicación : string.Empty);
                string Oficina = (persona.Telefonos.Where(x => x.IdTipoTelefono == 1).FirstOrDefault() != null ? persona.Telefonos.Where(x => x.IdTipoTelefono == 1).FirstOrDefault().NroTelefono : string.Empty);
                string Habitacion = (persona.Telefonos.Where(x => x.IdTipoTelefono == 2).FirstOrDefault() != null ? persona.Telefonos.Where(x => x.IdTipoTelefono == 2).FirstOrDefault().NroTelefono : string.Empty);
                string Movil = (persona.Telefonos.Where(x => x.IdTipoTelefono == 3).FirstOrDefault() != null ? persona.Telefonos.Where(x => x.IdTipoTelefono == 3).FirstOrDefault().NroTelefono : string.Empty);

                PersonasClave.Add(new DocumentoPersonaClaveModel()
                {
                    Cédula = persona.Identificacion,
                    DireccionHabitacion = Direccion,
                    Email = Email,
                    IdEmpresa = model.IdEmpresa,
                    IdPersona = persona.IdPersona,
                    IdTipoDocumento = model.IdTipoDocumento,
                    Nombre = persona.Nombre,
                    Principal = false,
                    Responsable = true,
                    TelefonoCelular = Movil,
                    TelefonoHabitacion = Habitacion,
                    TelefonoOficina = Oficina
                });

                model.PersonasClave = PersonasClave;
                model.Certificaciones = Certificaciones;
                model.Aprobaciones = Aprobaciones;
            }

            if (model.Aprobaciones == null || model.Aprobaciones.Count() == 0)
                ModelState.AddModelError("Aprobaciones", Resources.ErrorResource.RequiredErrorFemale);
            if (model.RequiereCertificacion && (model.Certificaciones == null || model.Certificaciones.Count() == 0))
                ModelState.AddModelError("Certificaciones", Resources.ErrorResource.RequiredErrorFemale);
            if (model.IdClaseDocumento == 4 && (model.Procesos == null || model.Procesos.Count() == 0))
                ModelState.AddModelError("Procesos", Resources.ErrorResource.RequiredErrorMale);
            if (model.Entrevistas == null || model.Entrevistas.Count() == 0)
                ModelState.AddModelError("Entrevistas", Resources.ErrorResource.RequiredErrorFemale);
            if (model.PersonasClave == null || model.PersonasClave.Count() == 0)
                ModelState.AddModelError("PersonasClave", Resources.ErrorResource.RequiredErrorFemale);

            if (ModelState.IsValid)
            {

            }
            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult PersonaPartialView(long modId)
        {
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));

            long IdModulo = IdTipoDocumento * 1000000;
            Session["IdTipoDocumento"] = IdTipoDocumento;

            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            int IdVersion = int.Parse(Session["IdVersion"].ToString());

            PersonaModel model = new PersonaModel
            {
                Cargo = new CargoModel(),
                CorreosElectronicos = new List<PersonaEmail>(),
                Direcciones = new List<PersonaDireccion>(),
                EditDocumento = true,
                IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString()),
                IdCargoPersona = 0,
                IdEmpresa = long.Parse(Session["IdEmpresa"].ToString()),
                Identificacion = string.Empty,
                IdModulo = IdModulo,
                IdPersona = 0,
                IdUnidadOrganizativaPersona = 0,
                IdUsuario = 0,
                Nombre = string.Empty,
                returnPage = Url.Action("Ficha", "Documentos", new { modId }),
                Telefonos = new List<PersonaTelefono>(),
                UnidadOrganizativa = new UnidadOrganizativaModel()
            };
            model.PageTitle = "Registro de Personas";

            Session["IdPersona"] = model.IdPersona;
            Session["Persona"] = model;
            return View(model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public ActionResult PersonaPartialView(PersonaModel model)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());

            ModelState.Remove("UnidadOrganizativa.NombreUnidadOrganizativa");
            ModelState.Remove("Cargo.NombreCargo");
            if (ModelState.IsValid)
            {
                if (Session["IdPersona"] != null && long.Parse(Session["IdPersona"].ToString()) > 0)
                    model.IdPersona = long.Parse(Session["IdPersona"].ToString());

                using (Entities db = new Entities())
                {
                    tblPersona Persona;
                    if (model.IdPersona == 0)
                    {
                        Persona = new tblPersona
                        {
                            IdCargo = model.IdCargoPersona,
                            IdEmpresa = IdEmpresa,
                            Identificacion = model.Identificacion,
                            IdUnidadOrganizativa = model.IdUnidadOrganizativaPersona,
                            Nombre = model.Nombre
                        };
                        db.tblPersona.Add(Persona);
                    }
                    else
                    {
                        Persona = db.tblPersona.Where(x => x.IdEmpresa == IdEmpresa && x.IdPersona == model.IdPersona).FirstOrDefault();
                        Persona.IdCargo = model.IdCargoPersona;
                        Persona.Identificacion = model.Identificacion;
                        Persona.IdUnidadOrganizativa = model.IdUnidadOrganizativaPersona;
                        Persona.Nombre = model.Nombre;
                    }

                    db.SaveChanges();
                    model.IdPersona = Persona.IdPersona;
                }
            }
            Session["IdPersona"] = model.IdPersona;
            Session["Persona"] = model;

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult DireccionesPartialView()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult CorreosPartialView()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult TelefonosPartialView()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult NuevoCargoPartialView()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult NuevaUnidadOrganizativaPartialView()
        {
            return PartialView();
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public JsonResult NuevoCargo(string Texto)
        {
            string IdCargo = string.Empty;

            long _IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            bool success = false;

            if (Texto != null)
            {
                using (Entities db = new Entities())
                {
                    tblCargo newCargo = new tblCargo
                    {
                        IdEmpresa = _IdEmpresa,
                        Descripcion = Texto
                    };

                    db.tblCargo.Add(newCargo);
                    db.SaveChanges();
                    IdCargo = newCargo.IdCargo.ToString();
                    success = true;
                }
            }
            return Json(new { success, IdCargo });
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public JsonResult NuevaUnidad(string Texto, long idUnidadPadre)
        {
            string IdUnidad = string.Empty;

            long _IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            bool success = false;

            if (Texto != null)
            {
                using (Entities db = new Entities())
                {
                    tblUnidadOrganizativa newUnidad = new tblUnidadOrganizativa
                    {
                        IdEmpresa = _IdEmpresa,
                        IdUnidadPadre = idUnidadPadre,
                        Nombre = Texto
                    };

                    db.tblUnidadOrganizativa.Add(newUnidad);
                    db.SaveChanges();
                    IdUnidad = newUnidad.IdUnidadOrganizativa.ToString();
                    success = true;
                }
            }
            return Json(new { success, IdUnidad });
        }
        [SessionExpire]
        [HandleError]
        public JsonResult CheckEmails(IList<PersonaEmail> data)
        {
            bool isValid = data.Count() > 0;
            return Json(isValid, JsonRequestBehavior.AllowGet);
        }
        [SessionExpire]
        [HandleError]
        public JsonResult CheckTelefonos(IList<PersonaTelefono> data)
        {
            bool isValid = data.Count() > 0;
            return Json(isValid, JsonRequestBehavior.AllowGet);
        }
        [SessionExpire]
        [HandleError]
        public JsonResult GetDatosPersonaSelected(long IdPersona)
        {
            bool success = false;
            string Cargo = string.Empty;
            string UnidadOrganizativa = string.Empty;

            PersonaModel persona = Metodos.GetPersonaById(IdPersona);
            if (persona != null)
            {
                Cargo = persona.Cargo.NombreCargo;
                UnidadOrganizativa = persona.UnidadOrganizativa.NombreCompleto;
            }
            return Json(new { success, Cargo, UnidadOrganizativa });
        }
        [ValidateInput(false)]
        [SessionExpire]
        [HandleError]
        public ActionResult BatchEditingUpdateCorreo(MVCxGridViewBatchUpdateValues<PersonaEmail, long> updateValues)
        {

            foreach (var product in updateValues.Insert)
            {
                //if (updateValues.IsValid(product))
                //InsertProduct(product, updateValues);
            }
            foreach (var product in updateValues.Update)
            {
                //if (updateValues.IsValid(product))
                //UpdateProduct(product, updateValues);
            }
            foreach (var productID in updateValues.DeleteKeys)
            {
                //DeleteProduct(productID, updateValues);
            }
            return PartialView("CorreosPartialView");
        }
        [ValidateInput(false)]
        [SessionExpire]
        [HandleError]
        public ActionResult BatchEditingUpdateTelefono(MVCxGridViewBatchUpdateValues<PersonaTelefono, long> updateValues)
        {
            foreach (var product in updateValues.Insert)
            {
                //if (updateValues.IsValid(product))
                //InsertProduct(product, updateValues);
            }
            foreach (var product in updateValues.Update)
            {
                //if (updateValues.IsValid(product))
                //UpdateProduct(product, updateValues);
            }
            foreach (var productID in updateValues.DeleteKeys)
            {
                //DeleteProduct(productID, updateValues);
            }
            return PartialView("TelefonosPartialView");
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Editor(long modId)
        {
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            int IdVersion = int.Parse(Session["IdVersion"].ToString());
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));
            long IdModulo = IdTipoDocumento * 1000000;
            Session["IdTipoDocumento"] = IdTipoDocumento;
            Session["modId"] = modId;

            DocumentoModel model = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);
            if (model == null)
                model = new DocumentoModel();
            byte[] Contenido = Metodos.GetContenidoDocumento(modId);

            model.returnPage = Url.Action("Index", "Documento", new { IdModulo });
            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            string uniqueId = model.IdDocumento.ToString().Trim() + model.IdModuloActual.ToString().Trim();

            Session["Editable"] = (model.IdEstatus != 6) && (Session["OnlyVisible"] != null && ((bool)Session["OnlyVisible"] == false));
            Session["Saved"] = true;
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
            if (IdDocumento > 0) Auditoria.RegistarAccion(eTipoAccion.Mostrar);

            return View(model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public ActionResult Editor(DocumentoContenidoModel model)
        {

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult EditorPartialView()
        {
            return PartialView("EditorPartialView");
        }
        [HttpGet]
        [SessionExpire]
        [HandleError]
        public ActionResult GetOpenDialog(long currentDocumentId)
        {
            var viewModel = new DocumentoModel
            {
                IdDocumento = currentDocumentId
            };
            return PartialView("OpenDialog", viewModel);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult UploadControlCallback(long currentDocumentId)
        {
            Session["uploadedFile"] = UploadControlExtension.GetUploadedFiles("UploadControl").First();

            return new EmptyResult();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Open(string Accion)
        {
            long currentDocumentId = (long)Session["IdDocumento"];
            long IdModulo = long.Parse(Session["modId"].ToString());
            byte[] Contenido;

            switch (Accion)
            {
                case "Open":

                    string DocUploadDirectory = "~/Content/DocsFolder/";

                    UploadedFile uploadedFile = (UploadedFile)Session["uploadedFile"];
                    string currentId = Session["UniqueId"].ToString();

                    DocumentManager.CloseDocument(currentId);
                    string[] spltFile = uploadedFile.FileName.Split('.');
                    string fileExtension = spltFile.Last();
                    Contenido = uploadedFile.FileBytes;

                    Metodos.UpdateContenidoDocumento(IdModulo, Contenido, eTipoAccion.AbrirDocumento);

                    Session["Contenido"] = Convert.ToBase64String(Contenido);
                    string resultFilePath = HttpContext.Request.MapPath(DocUploadDirectory + uploadedFile.FileName);

                    if (System.IO.File.Exists(resultFilePath))
                        System.IO.File.Delete(resultFilePath);

                    break;
                case "Save":
                    Contenido = RichEditExtension.SaveCopy("RichEdit", DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    var reString = Encoding.Default.GetString(Contenido);
                    Metodos.UpdateContenidoDocumento(IdModulo, Contenido, eTipoAccion.Actualizar);
                    long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
                    break;

            }
            // return PartialView("EditorPartialView");
            return RedirectToAction("Editor", new { modId = IdModulo });
        }
        [SessionExpire]
        public JsonResult SaveDocument()
        {
            bool success = false;

            long IdModulo = long.Parse(Session["modId"].ToString());
            byte[] Contenido = RichEditExtension.SaveCopy("RichEdit", DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
            var reString = Encoding.Default.GetString(Contenido);
            success = Metodos.UpdateContenidoDocumento(IdModulo, Contenido, eTipoAccion.Actualizar);

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
        public ActionResult GenerarPDF()
        {
            int IdTipoDocumento = int.Parse(Session["IdTipoDocumento"].ToString());
            long IdModulo = IdTipoDocumento * 1000000;
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            int IdVersion = int.Parse(Session["IdVersion"].ToString());

            DocumentoModel model = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);
            model.returnPage = Url.Action("Index", "Documento", new { IdModulo });
            model.IdModulo = IdTipoDocumento * 1000000;
            model.IdModuloActual = IdTipoDocumento * 1000000;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(99010300);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            return View(model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public JsonResult Start()
        {
            Auditoria.RegistarAccion(eTipoAccion.GenerarPDF);
            PDFManager _pdfManager = new PDFManager();
            string _rutaDocumento = _pdfManager.GenerarPDF_Documento(true);
            return Json(new { _rutaDocumento });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ControlCambios(long modId)
        {
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(Session["IdTipoDocumento"].ToString());
            long IdModulo = IdTipoDocumento * 1000000;
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            int IdVersion = int.Parse(Session["IdVersion"].ToString());

            DocumentoModel model = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            model.IdModulo = IdModulo;
            model.Perfil = Metodos.GetPerfilData();
            model.IdModuloActual = IdModulo;
            model.returnPage = Url.Action("Index", "Documento", new { IdModulo });
            Auditoria.RegistarAccion(eTipoAccion.ConsultarCambios);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ControlCambiosGridPartialView()
        {

            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult IniciarAprobacion(long modId)
        {
            int IdTipoDocumento = int.Parse(Session["IdTipoDocumento"].ToString());
            long IdModulo = IdTipoDocumento * 1000000;
            Metodos.IniciarAprobacion();

            return RedirectToAction("Index", "Documento", new { IdModulo });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Aprobacion(long modId)
        {
            int IdTipoDocumento = int.Parse(Session["IdTipoDocumento"].ToString());
            long IdModulo = IdTipoDocumento * 1000000;
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            long IdPersona = Metodos.GetUser_IdPersona();

            Session["IdPersona"] = IdPersona;

            DocumentoModel model = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            model.IdModulo = modId;
            model.IdModuloActual = IdModulo;
            model.returnPage = Url.Action("Index", "Documento", new { IdModulo });
            model.Perfil = Metodos.GetPerfilData();

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AprobacionGridPartialView()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Certificacion(long modId)
        {
            int IdTipoDocumento = int.Parse(Session["IdTipoDocumento"].ToString());
            long IdModulo = IdTipoDocumento * 1000000;
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            long IdPersona = Metodos.GetUser_IdPersona();

            Session["IdPersona"] = IdPersona;

            DocumentoModel model = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);
            model.PageTitle = Metodos.GetModuloName(modId);
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);
            model.IdModulo = modId;
            model.IdModuloActual = IdModulo;
            model.returnPage = Url.Action("Index", "Documento", new { IdModulo });
            model.Perfil = Metodos.GetPerfilData();

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult CertificacionGridPartialView()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult AprobarDocumento(long IdDocumento, long IdTipoDocumento, long IdModulo)
        {
            Metodos.AprobarDocumento(IdDocumento, IdTipoDocumento);
            return RedirectToAction("Aprobacion", new { modId = IdModulo });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult CertificarDocumento(long IdDocumento, long IdTipoDocumento, long IdModulo)
        {
            Metodos.CertificarDocumento(IdDocumento, IdTipoDocumento);
            return RedirectToAction("Certificacion", new { modId = IdModulo });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Anexar(long modId)
        {
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(Session["IdTipoDocumento"].ToString());

            long IdModulo = IdTipoDocumento * 1000000;
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());

            Session["IdDocumento"] = IdDocumento;
            Session["IdModulo"] = IdModulo;
            Session["modId"] = modId;

            int IdVersion = int.Parse(Session["IdVersion"].ToString());

            DocumentoModel model = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);
            if (model == null)
                model = new DocumentoModel();
            byte[] Contenido = Metodos.GetContenidoDocumento(modId);

            model.returnPage = Url.Action("Index", "Documento", new { IdModulo });
            model.IdModulo = IdModulo;
            model.IdModuloActual = IdModulo;
            model.PageTitle = Metodos.GetModuloName(modId);
            model.Perfil = Metodos.GetPerfilData();
            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            string uniqueId = model.IdDocumento.ToString().Trim() + model.IdModuloActual.ToString().Trim();

            Session["Editable"] = (model.IdEstatus != 6) && (Session["OnlyVisible"] != null && ((bool)Session["OnlyVisible"] == false));
            Auditoria.RegistarAccion(eTipoAccion.AccederAnexoDocumentoWeb);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult FileManagerPartial()
        {
            return PartialView("FileManagerPartial", FileManagerDocumentoControllerFileManagerSettings.Model);
        }
        [SessionExpire]
        [HandleError]
        public FileStreamResult FileManagerPartialDownload()
        {
            return FileManagerExtension.DownloadFiles(FileManagerControllerFileManagerSettings.CreateFileManagerDownloadSettings(), FileManagerDocumentoControllerFileManagerSettings.Model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public JsonResult RegistrarOperacion(string Tipo, string nombre, string viejo = "")
        {

            bool success = true;
            Auditoria.RegistarOperacionAnexoModulo(Tipo, nombre, viejo, true);

            return Json(new { success });
        }
        [SessionExpire]
        [HandleError]
        public ActionResult DiagramaBIA(long modId)
        {
            DocumentoDiagrama model = new DocumentoDiagrama();

            string _modId = modId.ToString();
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            int IdVersion = int.Parse(Session["IdVersion"].ToString());
            int IdTipoDocumento = int.Parse(Session["IdTipoDocumento"].ToString());
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            long IdModulo = IdTipoDocumento * 1000000;
            Session["modId"] = modId;

            DocumentoModel doc = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);
            List<DocumentoProcesoModel> _Procesos = Metodos.GetProcesosDocumento();

            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdDocumento = IdDocumento;
            model.IdTipoDocumento = IdTipoDocumento;
            model.returnPage = Url.Action("Index", "Documento", new { IdModulo });
            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            model.IdProceso = 0;
            model.FechaCreacion = doc.FechaCreacion;
            model.FechaEstadoDocumento = doc.FechaEstadoDocumento;
            model.FechaUltimaModificacion = doc.FechaUltimaModificacion;
            model.IdClaseDocumento = doc.IdClaseDocumento;
            model.IdEstatus = doc.IdEstatus;
            model.Negocios = doc.Negocios;
            model.NroDocumento = doc.NroDocumento;
            model.NroVersion = doc.NroVersion;
            model.Version = doc.Version;
            model.VersionOriginal = doc.VersionOriginal;

            if (_Procesos != null && _Procesos.Count > 0)
            {
                DocumentoProcesoModel Proceso = _Procesos.First();
                model.IdProceso = Proceso.IdProceso;
                model.Interdependencias = Metodos.GetInterdependenciasDiagrama(model.IdProceso);
                model.Clientes_Productos = Metodos.GetClientesProductosDiagrama(model.IdProceso);
                model.Entradas = Metodos.GetEntradasDiagrama(model.IdProceso);
                model.PersonalClave = Metodos.GetPersonasClaveDiagrama(model.IdProceso);
                model.Proveedores = Metodos.GetProveedoresDiagrama(model.IdProceso);
                model.Tecnologia = Metodos.GetTecnologiaDiagrama(model.IdProceso);
                model.NroProceso = Proceso.NroProceso.ToString();
                model.Nombre = Proceso.Nombre;
                model.Descripcion = Proceso.Descripcion;
            }

            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult DiagramaBIA(DocumentoDiagrama model)
        {
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            int IdVersion = int.Parse(Session["IdVersion"].ToString());
            int IdTipoDocumento = int.Parse(Session["IdTipoDocumento"].ToString());
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            long IdModulo = IdTipoDocumento * 1000000;
            long modId = long.Parse(Session["modId"].ToString());

            DocumentoModel doc = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);

            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdDocumento = IdDocumento;
            model.IdTipoDocumento = IdTipoDocumento;
            model.returnPage = Url.Action("Index", "Documento", new { IdModulo });
            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            model.FechaCreacion = doc.FechaCreacion;
            model.FechaEstadoDocumento = doc.FechaEstadoDocumento;
            model.FechaUltimaModificacion = doc.FechaUltimaModificacion;
            model.IdClaseDocumento = doc.IdClaseDocumento;
            model.IdEstatus = doc.IdEstatus;
            model.Negocios = doc.Negocios;
            model.NroDocumento = doc.NroDocumento;
            model.NroVersion = doc.NroVersion;
            model.Version = doc.Version;
            model.VersionOriginal = doc.VersionOriginal;

            DocumentoProcesoModel Proceso = Metodos.GetProceso(model.IdProceso);
            model.Interdependencias = Metodos.GetInterdependenciasDiagrama(model.IdProceso);
            model.Clientes_Productos = Metodos.GetClientesProductosDiagrama(model.IdProceso);
            model.Entradas = Metodos.GetEntradasDiagrama(model.IdProceso);
            model.PersonalClave = Metodos.GetPersonasClaveDiagrama(model.IdProceso);
            model.Proveedores = Metodos.GetProveedoresDiagrama(model.IdProceso);
            model.Tecnologia = Metodos.GetTecnologiaDiagrama(model.IdProceso);
            model.NroProceso = Proceso.NroProceso.ToString();
            model.Nombre = Proceso.Nombre;
            model.Descripcion = Proceso.Descripcion;

            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult DiagramaBCP(long modId)
        {
            DocumentoDiagrama model = new DocumentoDiagrama();

            string _modId = modId.ToString();
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            int IdVersion = int.Parse(Session["IdVersion"].ToString());
            int IdTipoDocumento = int.Parse(Session["IdTipoDocumento"].ToString());
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            long IdModulo = IdTipoDocumento * 1000000;
            Session["modId"] = modId;

            DocumentoModel doc = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);

            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdDocumento = IdDocumento;
            model.IdTipoDocumento = IdTipoDocumento;
            model.returnPage = Url.Action("Index", "Documento", new { IdModulo });
            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            model.IdProceso = (doc.DatosBCP == null ? 0 : doc.DatosBCP.IdProceso);
            model.FechaCreacion = doc.FechaCreacion;
            model.FechaEstadoDocumento = doc.FechaEstadoDocumento;
            model.FechaUltimaModificacion = doc.FechaUltimaModificacion;
            model.IdClaseDocumento = doc.IdClaseDocumento;
            model.IdEstatus = doc.IdEstatus;
            model.Negocios = doc.Negocios;
            model.NroDocumento = doc.NroDocumento;
            model.NroVersion = doc.NroVersion;
            model.Version = doc.Version;
            model.VersionOriginal = doc.VersionOriginal;

            if (doc.DatosBCP != null)
            {
                DocumentoProcesoModel Proceso = Metodos.GetProceso(model.IdProceso);
                model.Interdependencias = Metodos.GetInterdependenciasDiagrama(model.IdProceso);
                model.Clientes_Productos = Metodos.GetClientesProductosDiagrama(model.IdProceso);
                model.Entradas = Metodos.GetEntradasDiagrama(model.IdProceso);
                model.PersonalClave = Metodos.GetPersonasClaveDiagrama(model.IdProceso);
                model.Proveedores = Metodos.GetProveedoresDiagrama(model.IdProceso);
                model.Tecnologia = Metodos.GetTecnologiaDiagrama(model.IdProceso);
                model.NroProceso = Proceso.NroProceso.ToString();
                model.Nombre = Proceso.Nombre;
                model.Descripcion = Proceso.Descripcion;
            }

            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult DiagramaBCP(DocumentoDiagrama model)
        {
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            int IdVersion = int.Parse(Session["IdVersion"].ToString());
            int IdTipoDocumento = int.Parse(Session["IdTipoDocumento"].ToString());
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            long IdModulo = IdTipoDocumento * 1000000;
            long modId = long.Parse(Session["modId"].ToString());

            DocumentoModel doc = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);
            DocumentoProcesoModel Proceso = Metodos.GetProceso(doc.DatosBCP.IdProceso);
            model.Interdependencias = Metodos.GetInterdependenciasDiagrama(Proceso.IdProceso);
            model.Clientes_Productos = Metodos.GetClientesProductosDiagrama(Proceso.IdProceso);
            model.Entradas = Metodos.GetEntradasDiagrama(Proceso.IdProceso);
            model.PersonalClave = Metodos.GetPersonasClaveDiagrama(Proceso.IdProceso);
            model.Proveedores = Metodos.GetProveedoresDiagrama(Proceso.IdProceso);
            model.Tecnologia = Metodos.GetTecnologiaDiagrama(Proceso.IdProceso);
            model.NroProceso = Proceso.NroProceso.ToString();
            model.Nombre = Proceso.Nombre;
            model.Descripcion = Proceso.Descripcion;

            model.IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            model.IdDocumento = IdDocumento;
            model.IdTipoDocumento = IdTipoDocumento;
            model.returnPage = Url.Action("Index", "Documento", new { IdModulo });
            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
            model.FechaCreacion = doc.FechaCreacion;
            model.FechaEstadoDocumento = doc.FechaEstadoDocumento;
            model.FechaUltimaModificacion = doc.FechaUltimaModificacion;
            model.IdClaseDocumento = doc.IdClaseDocumento;
            model.IdEstatus = doc.IdEstatus;
            model.Negocios = doc.Negocios;
            model.NroDocumento = doc.NroDocumento;
            model.NroVersion = doc.NroVersion;
            model.Version = doc.Version;
            model.VersionOriginal = doc.VersionOriginal;

            ViewBag.Title = string.Format("{0} - {1}", model.PageTitle, Resources.BCMWebPublic.labelAppTitle);

            return View(model);
        }
        [SessionExpire]
        [HandleError]
        public ActionResult PartialProcesosView()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult PartialProcesosView(DocumentoDiagrama model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult PartialDiagramaProceso()
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult PartialDiagramaProceso(DocumentoDiagrama model)
        {
            return PartialView();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult Programacion(long modId)
        {
            string _modId = modId.ToString();
            int IdTipoDocumento = int.Parse(_modId.Substring(0, (_modId.Length == 7 ? 1 : 2)));
            Session["IdTipoDocumento"] = IdTipoDocumento;
            Session["modId"] = modId;

            long IdModulo = IdTipoDocumento * 1000000;
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            int IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
            int IdVersion = int.Parse(Session["IdVersion"].ToString());

            DocumentoModel model = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);
            if (model == null)
                model = new DocumentoModel();
            byte[] Contenido = Metodos.GetContenidoDocumento(modId);

            model.returnPage = Url.Action("Index", "Documento", new { IdModulo });
            model.IdModulo = IdModulo;
            model.IdModuloActual = modId;
            model.Perfil = Metodos.GetPerfilData();
            model.PageTitle = Metodos.GetModuloName(modId);
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
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ProgramacionDocumentosPartialView(ProgramacionModel model)
        {

            return PartialView("ProgramacionDocumentosPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost]
        public ActionResult ProgramacionUsuariosPartialView(ProgramacionModel model)
        {

            return PartialView("ProgramacionUsuariosPartialView");
        }
        [SessionExpire]
        [HandleError]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditProgramacionAddNewPartial(ProgramacionModel Programacion)
        {
            if (ModelState.IsValid)
            {
                try
                {
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
        public ActionResult EditProgramacionDeletePartial(int IdProgramacion)
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

    }
    public class FileManagerDocumentoControllerFileManagerSettings
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        private static HttpServerUtility Server { get { return HttpContext.Current.Server; } }

        private static long IdEmpresa;
        private static int IdTipoDocumento;
        private static int IdDocumento;
        private static long IdModulo;


        public static string RootFolder = @"~\Content\FileManager";

        public static string Model
        {
            get
            {

                IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
                IdTipoDocumento = int.Parse(Session["IdTipoDocumento"].ToString());
                IdDocumento = int.Parse(Session["IdDocumento"].ToString());
                IdModulo = long.Parse(Session["IdModulo"].ToString());

                DocumentoModel docModel = Metodos.GetDocumento(IdDocumento, IdTipoDocumento);
                FileManagerModel model = new FileManagerModel();

                string _Modulo = Metodos.GetModuloName(IdModulo);
                string _docFolder = string.Format("{0}_{1}_{2}", IdTipoDocumento.ToString(), docModel.NroDocumento.ToString("000"), docModel.Version);
                string _rootFolder = string.Format("~\\Content\\FileManager\\E{0}\\Documentos\\{1}", IdEmpresa.ToString("000"), _docFolder);
                string path = Server.MapPath(_rootFolder);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return _rootFolder;
            }
        }
    }
}