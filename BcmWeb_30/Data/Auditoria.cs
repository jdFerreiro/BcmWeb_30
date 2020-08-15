using BcmWeb_30.Data.EF;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace BcmWeb_30 
{
    public static class Auditoria
    {
        internal static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        internal static HttpRequest Request { get { return HttpContext.Current.Request; } }

        [SessionExpire]
        [HandleError]
        public static void RegistrarLogin()
        {
            long IdUser = long.Parse(Session["UserId"].ToString());
            using (Entities db = new Entities())
            {
                tblUsuario usuario = db.tblUsuario.Where(x => x.IdUsuario == IdUser).FirstOrDefault();

                tblAuditoria regAuditoria = new tblAuditoria
                {
                    Accion = string.Format(Resources.AuditoriaResource.LoginAccionMessage, usuario.Nombre),
                    DireccionIP = Request.UserHostAddress,
                    FechaRegistro = DateTime.UtcNow,
                    IdUsuario = IdUser,
                    Mensaje = string.Empty,
                };

                db.tblAuditoria.Add(regAuditoria);
                usuario.FechaUltimaConexion = DateTime.UtcNow;
                usuario.EstadoUsuario = 2;
                db.SaveChanges();
            }
        }
        [SessionExpire]
        [HandleError]
        public static void RegistrarLogout(long IdUser = 0)
        {
            if (IdUser == 0)
                IdUser = long.Parse(Session["UserId"].ToString());

            using (Entities db = new Entities())
            {
                tblUsuario usuario = db.tblUsuario.Where(x => x.IdUsuario == IdUser).FirstOrDefault();

                tblAuditoria regAuditoria = new tblAuditoria
                {
                    Accion = string.Format(Resources.AuditoriaResource.LogOutAccionMessage, usuario.Nombre),
                    DireccionIP = Request.UserHostAddress,
                    FechaRegistro = DateTime.UtcNow,
                    IdUsuario = IdUser,
                    Mensaje = string.Empty,
                };

                db.tblAuditoria.Add(regAuditoria);
                usuario.FechaUltimaConexion = DateTime.UtcNow;
                db.SaveChanges();
            }
        }
        [SessionExpire]
        [HandleError]
        public static void RegistarAccion(eTipoAccion Accion)
        {
            long IdUser = long.Parse(Session["UserId"].ToString());
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            long? IdDocumento = Session["IdDocumento"] != null && Session["IdDocumento"].ToString() != "0" ? (long?)long.Parse(Session["IdDocumento"].ToString()) : null;
            long IdTipoDocumento = (Session["IdTipoDocumento"] != null ? long.Parse(Session["IdTipoDocumento"].ToString()) : 0);
            long IdModuloActivo = (Session["modId"] != null ? long.Parse(Session["modId"].ToString()) : 0);
            long IdModuloPrincipal = IdTipoDocumento * 1000000;
            int IdClaseDocumento = (Session["IdClaseDocumento"] != null ? int.Parse(Session["IdClaseDocumento"].ToString()) : 0);
            string NombreAnexo = (Session["Anexo"] != null ? Session["Anexo"].ToString() : string.Empty);

            using (Entities db = new Entities())
            {
                string AccionMessage = string.Empty;
                string NombreModulo = string.Empty;
                tblModulo moduloPrincipal = db.tblModulo.Where(x => x.IdEmpresa == IdEmpresa && x.IdModulo == IdModuloPrincipal).FirstOrDefault();
                tblModulo moduloActivo = db.tblModulo.Where(x => x.IdEmpresa == IdEmpresa && x.IdModulo == IdModuloActivo).FirstOrDefault();

                switch (Accion)
                {
                    case eTipoAccion.AbrirDocumento:
                        AccionMessage = Resources.AuditoriaResource.AbrirDocumentoMessage;
                        NombreModulo = moduloPrincipal.Nombre;
                        break;
                    case eTipoAccion.AbrirPDFMovil:
                        AccionMessage = Resources.AuditoriaResource.AbrirPDFMovilMessage;
                        NombreModulo = moduloPrincipal.Nombre;
                        break;
                    case eTipoAccion.AbrirPDFWeb:
                        AccionMessage = Resources.AuditoriaResource.AbrirPDFWebMessage;
                        NombreModulo = moduloPrincipal.Nombre;
                        break;
                    case eTipoAccion.AccesoModuloMovil:
                        AccionMessage = Resources.AuditoriaResource.AccesoModuloMovilMessage;
                        NombreModulo = moduloActivo.Nombre;
                        break;
                    case eTipoAccion.AccesoModuloWeb:
                        AccionMessage = Resources.AuditoriaResource.AccesoModuloWebMessage;
                        NombreModulo = moduloActivo.Nombre;
                        break;
                    case eTipoAccion.Actualizar:
                        AccionMessage = Resources.AuditoriaResource.ActualizarAccionMessage;
                        NombreModulo = moduloActivo.Nombre;
                        break;
                    case eTipoAccion.AprobarDocumento:
                        AccionMessage = Resources.AuditoriaResource.AprobarDocumentoMessage;
                        NombreModulo = moduloPrincipal.Nombre;
                        break;
                    case eTipoAccion.CertificarDocumento:
                        AccionMessage = Resources.AuditoriaResource.CertificarDocumentoMessage;
                        NombreModulo = moduloPrincipal.Nombre;
                        break;
                    case eTipoAccion.ConsultarCambios:
                        AccionMessage = Resources.AuditoriaResource.ConsultarCambiosMessage;
                        NombreModulo = moduloPrincipal.Nombre;
                        break;
                    case eTipoAccion.Eliminar:
                        AccionMessage = Resources.AuditoriaResource.EliminarAccionMessage;
                        NombreModulo = moduloActivo.Nombre;
                        break;
                    case eTipoAccion.GenearCopiaDocumento:
                        AccionMessage = Resources.AuditoriaResource.GenerarCopiaDocumentoMessage;
                        NombreModulo = moduloPrincipal.Nombre;
                        break;
                    case eTipoAccion.GenerarPDF:
                        AccionMessage = Resources.AuditoriaResource.GenerarPDFMessage;
                        NombreModulo = moduloPrincipal.Nombre;
                        break;
                    case eTipoAccion.GenerarVersion:
                        AccionMessage = Resources.AuditoriaResource.GenerarVersionMessage;
                        NombreModulo = moduloPrincipal.Nombre;
                        break;
                    case eTipoAccion.Mostrar:
                        AccionMessage = Resources.AuditoriaResource.MostrarAccionMessage;
                        NombreModulo = moduloActivo.Nombre;
                        break;
                    case eTipoAccion.MostrarIniciativa:
                        AccionMessage = Resources.AuditoriaResource.MostrarIniciativaMessage;
                        NombreModulo = moduloActivo.Nombre;
                        break;
                }

                tblUsuario usuario = db.tblUsuario.Where(x => x.IdUsuario == IdUser).FirstOrDefault();
                tblDocumento Documento = db.tblDocumento.Where(x => x.IdEmpresa == IdEmpresa && x.IdDocumento == IdDocumento && x.IdTipoDocumento == IdTipoDocumento).FirstOrDefault();

                string docVersion = string.Empty;
                string NroDocumento = string.Empty;
                string VersionActual = (Documento != null ? Documento.VersionOriginal.ToString() : string.Empty);

                if (Documento != null)
                {
                    docVersion = (Documento.VersionOriginal > 0 ? string.Format("{0}.{1}", Documento.VersionOriginal, Documento.NroVersion) : Documento.NroVersion.ToString());
                    NroDocumento = Documento.NroDocumento.ToString();
                }

                string _Accion = string.Format(AccionMessage, NombreModulo, NroDocumento, docVersion, NombreAnexo, VersionActual);

                tblAuditoria regAuditoria = new tblAuditoria
                {
                    Accion = _Accion,
                    DireccionIP = Request.UserHostAddress,
                    FechaRegistro = DateTime.UtcNow,
                    IdDocumento = IdDocumento,
                    IdEmpresa = IdEmpresa,
                    IdTipoDocumento = IdTipoDocumento,
                    IdUsuario = IdUser,
                    Mensaje = string.Empty,
                    Negocios = (IdClaseDocumento == 1),
                };

                db.tblAuditoria.Add(regAuditoria);
                usuario.FechaUltimaConexion = DateTime.UtcNow;
                usuario.EstadoUsuario = 2;
                db.SaveChanges();
            }
        }
        [SessionExpire]
        [HandleError]
        public static void RegistarOperacionAnexoModulo(string Operacion, string nombre, string viejo, bool Web = true)
        {
            string AccionMessage = string.Empty;
            string NombreModulo = string.Empty;

            long IdUser = long.Parse(Session["UserId"].ToString());
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            long IdDocumento = (Session["IdDocumento"] != null ? long.Parse(Session["IdDocumento"].ToString()) : 0);
            long IdTipoDocumento = (Session["IdTipoDocumento"] != null ? long.Parse(Session["IdTipoDocumento"].ToString()) : 0);
            long IdModuloActivo = (Session["modId"] != null ? long.Parse(Session["modId"].ToString()) : 0);
            long IdModuloPrincipal = IdTipoDocumento * 1000000;
            int IdClaseDocumento = (Session["IdClaseDocumento"] != null ? int.Parse(Session["IdClaseDocumento"].ToString()) : 0);
            long IdModulo = long.Parse(Session["IdModulo"].ToString());
            NombreModulo = Metodos.GetNombreModulo(IdModulo);

            switch (Operacion)
            {
                case "FolderCreated":
                    AccionMessage = Resources.AuditoriaResource.AnexoAgregarCarpetaModuloMessage;
                    break;
                case "CurrentFolderChanged":
                    break;
                case "FileDownloading":
                    AccionMessage = Resources.AuditoriaResource.AnexoDescargarModuloWebMessage;
                    break;
                case "FileUploaded":
                    AccionMessage = Resources.AuditoriaResource.AnexoCargarModuloWebMessage;
                    break;
                case "ItemCopied":
                    AccionMessage = Resources.AuditoriaResource.AnexoCopiarItemModuloMessage;
                    break;
                case "ItemDeleted":
                    AccionMessage = Resources.AuditoriaResource.AnexoEliminarItemModuloMessage;
                    break;
                case "ItemMoved":
                    AccionMessage = Resources.AuditoriaResource.AnexoMoverItemModuloMessage;
                    break;
                case "ItemRenamed":
                    AccionMessage = Resources.AuditoriaResource.AnexoRenombrarItemModuloMessage;
                    break;
            }

            using (Entities db = new Entities())
            {
                if (!string.IsNullOrEmpty(AccionMessage))
                {
                    tblModulo moduloPrincipal = db.tblModulo.Where(x => x.IdEmpresa == IdEmpresa && x.IdModulo == IdModuloPrincipal).FirstOrDefault();
                    tblModulo moduloActivo = db.tblModulo.Where(x => x.IdEmpresa == IdEmpresa && x.IdModulo == IdModuloActivo).FirstOrDefault();

                    tblUsuario usuario = db.tblUsuario.Where(x => x.IdUsuario == IdUser).FirstOrDefault();
                    tblDocumento Documento = db.tblDocumento.Where(x => x.IdEmpresa == IdEmpresa && x.IdDocumento == IdDocumento && x.IdTipoDocumento == IdTipoDocumento).FirstOrDefault();

                    string docVersion = string.Empty;
                    string NroDocumento = string.Empty;
                    string VersionActual = (Documento != null ? Documento.VersionOriginal.ToString() : string.Empty);

                    if (Documento != null)
                    {
                        docVersion = (Documento.VersionOriginal > 0 ? string.Format("{0}.{1}", Documento.VersionOriginal, Documento.NroVersion) : Documento.NroVersion.ToString());
                        NroDocumento = Documento.NroDocumento.ToString();
                    }

                    string _Accion = string.Format(AccionMessage, NombreModulo, NroDocumento, docVersion, nombre, viejo);

                    tblAuditoria regAuditoria = new tblAuditoria
                    {
                        Accion = _Accion,
                        DireccionIP = Request.UserHostAddress,
                        FechaRegistro = DateTime.UtcNow,
                        IdDocumento = IdDocumento,
                        IdEmpresa = IdEmpresa,
                        IdTipoDocumento = IdTipoDocumento,
                        IdUsuario = IdUser,
                        Mensaje = string.Empty,
                        Negocios = (IdClaseDocumento == 1),
                    };

                    db.tblAuditoria.Add(regAuditoria);
                    usuario.FechaUltimaConexion = DateTime.UtcNow;
                    usuario.EstadoUsuario = 2;
                    db.SaveChanges();
                }
            }

        }
        [SessionExpire]
        [HandleError]
        public static void RegistarIniciativa(eTipoAccion Accion, long IdIniciativa, string NombreIniciativa, string DatosActualizados)
        {
            long IdUser = long.Parse(Session["UserId"].ToString());
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());

            using (Entities db = new Entities())
            {
                tblModulo moduloPrincipal = db.tblModulo.Where(x => x.IdEmpresa == IdEmpresa && x.IdModulo == 14000000).FirstOrDefault();
                tblModulo moduloActivo = db.tblModulo.Where(x => x.IdEmpresa == IdEmpresa && x.IdModulo == 14010100).FirstOrDefault();
                string NroIniciativa = string.Empty;
                string AccionMessage = string.Empty;
                string NombreModulo = moduloActivo.Nombre;
                tblIniciativas reg = null;

                switch (Accion)
                {
                    case eTipoAccion.AgregarIniciativa:
                        AccionMessage = Resources.AuditoriaResource.AgregarIniciativaMessage;
                        NombreModulo = moduloActivo.Nombre;
                        reg = db.tblIniciativas.Where(x => x.IdEmpresa == IdEmpresa && x.IdIniciativa == IdIniciativa).FirstOrDefault();

                        if (reg != null)
                        {
                            NroIniciativa = reg.NroIniciativa.ToString();
                        }
                        break;
                    case eTipoAccion.ActualizarIniciativa:
                        AccionMessage = Resources.AuditoriaResource.ModificarIniciativaMessage;
                        NombreModulo = moduloActivo.Nombre;
                        reg = db.tblIniciativas.Where(x => x.IdEmpresa == IdEmpresa && x.IdIniciativa == IdIniciativa).FirstOrDefault();

                        if (reg != null)
                        {
                            NroIniciativa = reg.NroIniciativa.ToString();
                        }
                        break;
                    case eTipoAccion.EliminarIniciativa:
                        AccionMessage = Resources.AuditoriaResource.EliminarIniciativaMessage;
                        NombreModulo = moduloActivo.Nombre;
                        break;
                }

                tblUsuario usuario = db.tblUsuario.Where(x => x.IdUsuario == IdUser).FirstOrDefault();

                string _Accion = string.Format(AccionMessage, NombreModulo, NombreIniciativa, NroIniciativa);

                tblAuditoria regAuditoria = new tblAuditoria
                {
                    Accion = _Accion,
                    DireccionIP = Request.UserHostAddress,
                    FechaRegistro = DateTime.UtcNow,
                    IdDocumento = 0,
                    IdEmpresa = IdEmpresa,
                    IdTipoDocumento = 0,
                    IdUsuario = IdUser,
                    Mensaje = string.Empty,
                    Negocios = true,
                    DatosModificados = DatosActualizados
                };

                db.tblAuditoria.Add(regAuditoria);
                usuario.FechaUltimaConexion = DateTime.UtcNow;
                usuario.EstadoUsuario = 2;
                db.SaveChanges();
            }
        }
        [SessionExpire]
        [HandleError]
        public static void RegistarOperacionAnexoIniciativa(string Operacion, string nombre, string viejo, bool Web = true)
        {
            string AccionMessage = string.Empty;
            string NombreModulo = string.Empty;

            long IdUser = long.Parse(Session["UserId"].ToString());
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            long IdIniciativa = long.Parse(Session["IdIniciativa"].ToString());

            switch (Operacion)
            {
                case "FolderCreated":
                    AccionMessage = Resources.AuditoriaResource.AnexoAgregarCarpetaIniciativaMessage;
                    break;
                case "CurrentFolderChanged":
                    break;
                case "FileDownloading":
                    AccionMessage = Resources.AuditoriaResource.AnexoDescargarIniciativaWebMessage;
                    break;
                case "FileUploaded":
                    AccionMessage = Resources.AuditoriaResource.AnexoCargarDocumentoIniciativaMessage;
                    break;
                case "ItemCopied":
                    AccionMessage = Resources.AuditoriaResource.AnexoCopiarItemIniciativaMessage;
                    break;
                case "ItemDeleted":
                    AccionMessage = Resources.AuditoriaResource.AnexoEliminarItemIniciativaMessage;
                    break;
                case "ItemMoved":
                    AccionMessage = Resources.AuditoriaResource.AnexoMoverItemIniciativaMessage;
                    string[] dataNombre = nombre.Replace("//", "\\").Split('\\');
                    string[] dataViejo = viejo.Split('\\');
                    string _nombre = dataNombre.Last();
                    string _viejo = string.Format("{0} a {1}", dataViejo[dataViejo.Length - 2], dataNombre[dataNombre.Length - 2]);
                    nombre = _nombre;
                    viejo = _viejo;
                    break;
                case "ItemRenamed":
                    AccionMessage = Resources.AuditoriaResource.AnexoRenombrarItemIniciativaMessage;
                    break;
            }

            using (Entities db = new Entities())
            {
                if (!string.IsNullOrEmpty(AccionMessage))
                {
                    tblUsuario usuario = db.tblUsuario.Where(x => x.IdUsuario == IdUser).FirstOrDefault();
                    tblIniciativas iniciativa = db.tblIniciativas.Where(x => x.IdEmpresa == IdEmpresa && x.IdIniciativa == IdIniciativa).FirstOrDefault();
                    string _NombreIniciativa = string.Format("{0} - {1}", iniciativa.NroIniciativa.ToString(), iniciativa.Nombre);
                    string _Accion = string.Format(AccionMessage, _NombreIniciativa, nombre, viejo);

                    tblAuditoria regAuditoria = new tblAuditoria
                    {
                        Accion = _Accion,
                        DireccionIP = Request.UserHostAddress,
                        FechaRegistro = DateTime.UtcNow,
                        IdDocumento = 0,
                        IdEmpresa = IdEmpresa,
                        IdTipoDocumento = 0,
                        IdUsuario = IdUser,
                        Mensaje = string.Empty,
                        Negocios = true,
                    };

                    db.tblAuditoria.Add(regAuditoria);
                    usuario.FechaUltimaConexion = DateTime.UtcNow;
                    usuario.EstadoUsuario = 2;
                    db.SaveChanges();
                }
            }

        }
        [SessionExpire]
        [HandleError]
        public static void RegistarProgramacion(eTipoAccion Accion, long IdProgramacion, long IdModuloProgramacion, string DatosActualizados)
        {
            long IdUser = long.Parse(Session["UserId"].ToString());
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());

            using (Entities db = new Entities())
            {
                tblModulo moduloPrincipal = db.tblModulo.Where(x => x.IdEmpresa == IdEmpresa && x.IdModulo == 9000000).FirstOrDefault();
                tblModulo moduloActivo = db.tblModulo.Where(x => x.IdEmpresa == IdEmpresa && x.IdModulo == 9060100).FirstOrDefault();
                string AccionMessage = string.Empty;
                string NombreModulo = moduloActivo.Nombre;
                tblPMTProgramacion reg = null;

                switch (Accion)
                {
                    case eTipoAccion.AgregarProgramacion:
                        AccionMessage = Resources.AuditoriaResource.AgregarProgramacionMessage;
                        NombreModulo = moduloActivo.Nombre;
                        reg = db.tblPMTProgramacion.Where(x => x.IdEmpresa == IdEmpresa && x.IdPMTProgramacion == IdProgramacion).FirstOrDefault();
                        break;
                    case eTipoAccion.ActualizarProgramacion:
                        AccionMessage = Resources.AuditoriaResource.ModificarProgramacionMessage;
                        NombreModulo = moduloActivo.Nombre;
                        reg = db.tblPMTProgramacion.Where(x => x.IdEmpresa == IdEmpresa && x.IdPMTProgramacion == IdProgramacion).FirstOrDefault();
                        break;
                    case eTipoAccion.EliminarProgramacion:
                        AccionMessage = Resources.AuditoriaResource.EliminarProgramacionMessage;
                        NombreModulo = moduloActivo.Nombre;
                        break;
                }

                tblUsuario usuario = db.tblUsuario.Where(x => x.IdUsuario == IdUser).FirstOrDefault();

                string _Accion = string.Format(AccionMessage, reg.tblModulo.Nombre);

                tblAuditoria regAuditoria = new tblAuditoria
                {
                    Accion = _Accion,
                    DireccionIP = Request.UserHostAddress,
                    FechaRegistro = DateTime.UtcNow,
                    IdDocumento = 0,
                    IdEmpresa = IdEmpresa,
                    IdTipoDocumento = 0,
                    IdUsuario = IdUser,
                    Mensaje = string.Empty,
                    Negocios = true,
                    DatosModificados = DatosActualizados
                };

                db.tblAuditoria.Add(regAuditoria);
                usuario.FechaUltimaConexion = DateTime.UtcNow;
                usuario.EstadoUsuario = 2;
                db.SaveChanges();
            }
        }
        [SessionExpire]
        [HandleError]
        public static void RegistarActualizarModulos(eTipoAccion actualizarModulosUsuario, long idEmpresa, long idUsuario)
        {
            long IdUser = long.Parse(Session["UserId"].ToString());
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());

            using (Entities db = new Entities())
            {
                string AccionMessage = Resources.AuditoriaResource.ActualizarModulosUsuario;
                string nombreEmpresa = db.tblEmpresa.FirstOrDefault(x => x.IdEmpresa == idEmpresa).NombreComercial;
                string nombreUsuario = db.tblUsuario.FirstOrDefault(x => x.IdUsuario == idUsuario).Nombre;
                tblUsuario usuario = db.tblUsuario.FirstOrDefault(x => x.IdUsuario == IdUser);

                string _Accion = string.Format(AccionMessage, nombreUsuario, nombreEmpresa);

                tblAuditoria regAuditoria = new tblAuditoria
                {
                    Accion = _Accion,
                    DireccionIP = Request.UserHostAddress,
                    FechaRegistro = DateTime.UtcNow,
                    IdDocumento = 0,
                    IdEmpresa = IdEmpresa,
                    IdTipoDocumento = 0,
                    IdUsuario = IdUser,
                    Mensaje = string.Empty,
                    Negocios = true,
                    DatosModificados = string.Empty,
                };

                db.tblAuditoria.Add(regAuditoria);
                usuario.FechaUltimaConexion = DateTime.UtcNow;
                usuario.EstadoUsuario = 2;
                db.SaveChanges();
            }
        }
        [SessionExpire]
        [HandleError]
        public static void RegistarIncidente(eTipoAccion Accion, long IdIncidente, string TipoIncidente, string DatosActualizados)
        {
            long IdUser = long.Parse(Session["UserId"].ToString());
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());

            using (Entities db = new Entities())
            {
                string AccionMessage = string.Empty;
                string NombreModulo = Resources.PMIResource.captionModulo;
                tblIncidentes reg = null;

                switch (Accion)
                {
                    case eTipoAccion.AgregarIncidente:
                        AccionMessage = Resources.AuditoriaResource.AgregarIncidenteMessage;
                        reg = db.tblIncidentes.Where(x => x.IdEmpresa == IdEmpresa && x.IdIncidente == IdIncidente).FirstOrDefault();
                        break;
                    case eTipoAccion.ActualizarIncidente:
                        AccionMessage = Resources.AuditoriaResource.ActualizarIncidenteMessage;
                        reg = db.tblIncidentes.Where(x => x.IdEmpresa == IdEmpresa && x.IdIncidente == IdIncidente).FirstOrDefault();
                        break;
                    case eTipoAccion.EliminarIncidente:
                        AccionMessage = Resources.AuditoriaResource.EliminarIncidenteMessage;
                        break;
                }

                tblUsuario usuario = db.tblUsuario.Where(x => x.IdUsuario == IdUser).FirstOrDefault();

                string _Accion = AccionMessage;

                tblAuditoria regAuditoria = new tblAuditoria
                {
                    Accion = _Accion,
                    DireccionIP = Request.UserHostAddress,
                    FechaRegistro = DateTime.UtcNow,
                    IdDocumento = 0,
                    IdEmpresa = IdEmpresa,
                    IdTipoDocumento = 0,
                    IdUsuario = IdUser,
                    Mensaje = string.Empty,
                    Negocios = true,
                    DatosModificados = DatosActualizados
                };

                db.tblAuditoria.Add(regAuditoria);
                usuario.FechaUltimaConexion = DateTime.UtcNow;
                usuario.EstadoUsuario = 2;
                db.SaveChanges();
            }
        }
        [SessionExpire]
        [HandleError]
        public static void ProcesarEvento(eTipoAccion Accion, long IdDispositivo, long idSubModulo, string nombre, string DatosActualizados)
        {
            long IdUser = long.Parse(Session["UserId"].ToString());
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());

            using (Entities db = new Entities())
            {
                string AccionMessage = string.Empty;
                string NombreModulo = Resources.PMIResource.captionModulo;
                tblModulo regModulo = db.tblModulo.FirstOrDefault(x => x.IdEmpresa == IdEmpresa && x.IdModulo == idSubModulo);
                tblDispositivo regDisp = db.tblDispositivo.FirstOrDefault(x => x.IdDispositivo == IdDispositivo);

                switch (Accion)
                {
                    case eTipoAccion.ActivarEvento:
                        AccionMessage = string.Format(Resources.AuditoriaResource.ActivarEvento, regModulo.Nombre);
                        break;
                    case eTipoAccion.EliminarEvento:
                        AccionMessage = string.Format(Resources.AuditoriaResource.EliminarEvento, regDisp.NombreDispositivo, regModulo.Nombre);
                        break;
                }

                tblUsuario usuario = db.tblUsuario.Where(x => x.IdUsuario == IdUser).FirstOrDefault();

                string _Accion = AccionMessage;

                tblAuditoria regAuditoria = new tblAuditoria
                {
                    Accion = _Accion,
                    DireccionIP = Request.UserHostAddress,
                    FechaRegistro = DateTime.UtcNow,
                    IdDocumento = 0,
                    IdEmpresa = IdEmpresa,
                    IdTipoDocumento = 0,
                    IdUsuario = IdUser,
                    Mensaje = string.Empty,
                    Negocios = true,
                    DatosModificados = DatosActualizados
                };

                db.tblAuditoria.Add(regAuditoria);
                usuario.FechaUltimaConexion = DateTime.UtcNow;
                usuario.EstadoUsuario = 2;
                db.SaveChanges();
            }
        }
    }
}