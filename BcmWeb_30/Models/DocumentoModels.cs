using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BcmWeb_30 .Models
{

    public class DocumentosModel : ModulosUserModel
    {
        public long IdDocumentoSelected { get; set; }
        public IQueryable<DocumentoModel> Documentos { get; set; }
        public bool EditActive { get; set; }
        public DocumentosModel()
        {
            Documentos = new List<DocumentoModel>().AsQueryable();
        }
    }
    public class DocumentoModel : ModulosUserModel
    {
        [Display(Name = "captionDocumento", ResourceType = typeof(Resources.DocumentoResource))]
        public long IdDocumento { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionTipoDocumento", ResourceType = typeof(Resources.DocumentoResource))]
        public long IdTipoDocumento { get; set; }
        [Display(Name = "captionNroDocumento", ResourceType = typeof(Resources.DocumentoResource))]
        public long NroDocumento { get; set; }
        [Display(Name = "captionFechaCreacion", ResourceType = typeof(Resources.DocumentoResource))]
        public DateTime FechaCreacion { get; set; }
        [Display(Name = "captionFechaUltimaModificacion", ResourceType = typeof(Resources.DocumentoResource))]
        public DateTime FechaUltimaModificacion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionEstatus", ResourceType = typeof(Resources.DocumentoResource))]
        public long IdEstatus { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionEstatus", ResourceType = typeof(Resources.DocumentoResource))]
        public string Estatus { get; set; }
        [Display(Name = "captionFechaEstatus", ResourceType = typeof(Resources.DocumentoResource))]
        public DateTime FechaEstadoDocumento { get; set; }
        public bool Negocios { get; set; }
        [Display(Name = "captionVersion", ResourceType = typeof(Resources.DocumentoResource))]
        public int NroVersion { get; set; }
        [Display(Name = "captionVersionAnterior", ResourceType = typeof(Resources.DocumentoResource))]
        public int VersionOriginal { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionResponsable", ResourceType = typeof(Resources.DocumentoResource))]
        [Range(1, long.MaxValue, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public long IdPersonaResponsable { get; set; }
        [Display(Name = "captionResponsable", ResourceType = typeof(Resources.DocumentoResource))]
        public PersonaModel Responsable
        {
            get
            {
                return Metodos.GetShortPersonaById(IdPersonaResponsable);
            }
        }
        [Display(Name = "captionRequiereCertificacion", ResourceType = typeof(Resources.DocumentoResource))]
        public bool RequiereCertificacion { get; set; }
        public bool Editable { get; set; }
        public bool Eliminable { get; set; }
        public string RutaDocumentoPDF
        {
            get
            {
                Uri _ContextUrl = HttpContext.Current.Request.Url;
                string _AppUrl = _ContextUrl.AbsoluteUri.Replace(_ContextUrl.PathAndQuery, string.Empty);
                HttpServerUtility _Server = HttpContext.Current.Server;
                string _ServerPath = _Server.MapPath(".").Replace("\\Account", string.Empty);
                eSystemModules Modulo = (eSystemModules)IdTipoDocumento;
                string TipoDocumento = Modulo.ToString();
                eEstadoDocumento EstadoDocumento = (eEstadoDocumento)IdEstatus;
                string _CodigoInforme = string.Format("{0}_{1}_{2}_{3}_{4}.{5}", TipoDocumento, IdEmpresa.ToString(), NroDocumento.ToString("#000"), (EstadoDocumento == eEstadoDocumento.Certificado ? FechaEstadoDocumento.ToString("MM-yyyy") : DateTime.Now.ToString("MM-yyyy")), VersionOriginal, NroVersion).Replace("-", "_");
                string _FileName = string.Format("{0}.pdf", _CodigoInforme.Replace("-", "_"));
                string _pathFile = String.Format("{0}\\PDFDocs\\{1}", _ServerPath.Replace("\\Documento",""), _FileName);

                if (System.IO.File.Exists(_pathFile))
                    return String.Format("{0}/PDFDocs/{1}", _AppUrl, _FileName);
                else
                    return string.Empty;
            }
        }
        public string Version { get; set; }
        public bool HasVersion { get; set; }
        public DocumentoBIA DatosBIA { get; set; }
        public DocumentoBCP DatosBCP { get; set; }
        public List<DocumentoAnexoModel> Anexos { get; set; }
        public List<DocumentoAprobacionModel> Aprobaciones { get; set; }
        public List<DocumentoAuditoriaModel> Auditoria { get; set; }
        public List<DocumentoCertificacionModel> Certificaciones { get; set; }
        public List<DocumentoContenidoModel> Contenido { get; set; }
        public List<DocumentoEntrevistaModel> Entrevistas { get; set; }
        public List<DocumentoPersonaClaveModel> PersonasClave { get; set; }
        public List<DocumentoProcesoModel> Procesos { get; set; }

        public bool Updated { get; set; }
        public DocumentoModel()
        {
            this.Anexos = new List<DocumentoAnexoModel>();
            this.Aprobaciones = new List<DocumentoAprobacionModel>();
            this.Auditoria = new List<DocumentoAuditoriaModel>();
            this.Certificaciones = new List<DocumentoCertificacionModel>();
            this.Contenido = new List<DocumentoContenidoModel>();
            this.Entrevistas = new List<DocumentoEntrevistaModel>();
            this.PersonasClave = new List<DocumentoPersonaClaveModel>();
            this.Procesos = new List<DocumentoProcesoModel>();
            this.DatosBIA = new DocumentoBIA();
            this.Updated = false;
        }

    }
    public class DocumentoBCP
    {
        public long IdProceso { get; set; }
        public string Proceso { get; set; }
        public string Subproceso { get; set; }
        public long IdUnidadOrganizativa { get; set; }
        public string UnidadOrganizativa
        {
            get
            {
                string _unidadOrganizativa = string.Empty;
                _unidadOrganizativa = Metodos.GetUnidadOrganizativaById(IdUnidadOrganizativa).NombreUnidadOrganizativa;
                return _unidadOrganizativa;
            }
        }
        public string Responsable { get; set; }
    }
    public class DocumentoBIA
    {
        public long IdUnidadOrganizativa { get; set; }
        public string UnidadOrganizativa
        {
            get
            {
                string _unidadOrganizativa = string.Empty;
                UnidadOrganizativaModel UOModel = Metodos.GetUnidadOrganizativaById(IdUnidadOrganizativa);
                _unidadOrganizativa = (UOModel == null ? string.Empty : UOModel.NombreUnidadOrganizativa ?? string.Empty);
                return _unidadOrganizativa;
            }
        }
        public long IdCadenaServicio { get; set; }
        public string CadenaServicio { get; set; }
    }
    public class DocumentoAnexoModel
    {
        public long IdEmpresa { get; set; }
        public long IdDocumento { get; set; }
        public long IdTipoDocumento { get; set; }
        public long IdAnexo { get; set; }
        public string NombreArchivo { get; set; }
        public string RutaArchivo { get; set; }
    }
    public class DocumentoAprobacionModel : ModulosUserModel
    {
        public long IdDocumento { get; set; }
        public long IdTipoDocumento { get; set; }
        public long IdAprobacion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionFechaAprobacion", ResourceType = typeof(Resources.DocumentoResource))]
        [DataType(DataType.DateTime, ErrorMessageResourceName = "InvalidoErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public Nullable<DateTime> FechaAprobacion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionAprobador", ResourceType = typeof(Resources.DocumentoResource))]
        public long IdPersona { get; set; }
        [Display(Name = "captionAprobador", ResourceType = typeof(Resources.DocumentoResource))]
        public PersonaModel Persona
        {
            get
            {
                PersonaModel _persona = Metodos.GetPersonaById(this.IdPersona);
                if ((_persona == null || string.IsNullOrEmpty(_persona.Nombre)) && this.IdPersona > 0)
                {
                    _persona = Metodos.ConvertUsuarioToPersona(this.IdPersona);
                }
                return _persona;
            }
        }
        [Display(Name = "captionAprobado", ResourceType = typeof(Resources.DocumentoResource))]
        public bool Aprobado { get; set; }
        [Display(Name = "captionProcesado", ResourceType = typeof(Resources.DocumentoResource))]
        public bool Procesado { get; set; }
        public bool Responsable { get; set; }
    }
    public class DocumentoAuditoriaModel
    {
        public long IdEmpresa { get; set; }
        public long IdDocumento { get; set; }
        public long IdTipoDocumento { get; set; }
        public long IdAuditoria { get; set; }
        [Display(Name = "captionFechaRegistro", ResourceType = typeof(Resources.DocumentoResource))]
        public DateTime FechaRegistro { get; set; }
        [Display(Name = "captionDireccionIP", ResourceType = typeof(Resources.DocumentoResource))]
        public string DireccionIP { get; set; }
        [Display(Name = "captionMensaje", ResourceType = typeof(Resources.DocumentoResource))]
        public string Mensaje { get; set; }
        [Display(Name = "captionAccion", ResourceType = typeof(Resources.DocumentoResource))]
        public string Accion { get; set; }
        [Display(Name = "captionUsuario", ResourceType = typeof(Resources.DocumentoResource))]
        public long IdUsuario { get; set; }
        [Display(Name = "captionUsuario", ResourceType = typeof(Resources.DocumentoResource))]
        public PersonaModel Usuario { get; set; }
        public bool Negocios { get; set; }

    }
    public class DocumentoCertificacionModel : ModulosUserModel
    {
        public long IdDocumento { get; set; }
        public long IdTipoDocumento { get; set; }
        public long IdCertificacion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionFechaCertificacion", ResourceType = typeof(Resources.DocumentoResource))]
        [DataType(DataType.DateTime, ErrorMessageResourceName = "InvalidoErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public DateTime? FechaCertificacion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionCertificador", ResourceType = typeof(Resources.DocumentoResource))]
        public long IdPersona { get; set; }
        [Display(Name = "captionCertificador", ResourceType = typeof(Resources.DocumentoResource))]
        public PersonaModel Persona
        {
            get
            {
                PersonaModel _persona = Metodos.GetPersonaById(this.IdPersona);
                if ((_persona == null || string.IsNullOrEmpty(_persona.Nombre)) && this.IdPersona > 0)
                {
                    _persona = Metodos.ConvertUsuarioToPersona(this.IdPersona);
                }
                return _persona;
            }
        }
        [Display(Name = "captionCertificado", ResourceType = typeof(Resources.DocumentoResource))]
        public bool Certificado { get; set; }
        [Display(Name = "captionProcesado", ResourceType = typeof(Resources.DocumentoResource))]
        public bool Procesado { get; set; }
        public bool Responsable { get; set; }
    }
    public class DocumentoContenidoModel : ModulosUserModel
    {
        public long IdDocumento { get; set; }
        public long IdTipoDocumento { get; set; }
        public byte[] Contenido { get; set; }
        [Display(Name = "captionFechaCreacion", ResourceType = typeof(Resources.DocumentoResource))]
        public DateTime FechaCreacion { get; set; }
        [Display(Name = "captionFechaUltimaModificacion", ResourceType = typeof(Resources.DocumentoResource))]
        public DateTime FechaUltimaModificacion { get; set; }
        public string UniqueId
        {
            get
            {
                string uId = this.IdDocumento.ToString().Trim() + this.IdModulo.ToString().Trim();
                return uId;
            }
        }
        public string DocExtension { get; set; }
        //public DocumentFormat docFormat
        //{
        //    get
        //    {
        //        switch (this.DocExtension)
        //        {
        //            case "docx":
        //                return DevExpress.Xpf.RichEdit.d.XtraRichEdit.DocumentFormat.DocX;
        //            case "doc":
        //                return DevExpress.XtraRichEdit.DocumentFormat.Doc;
        //            case "html":
        //                return DevExpress.XtraRichEdit.DocumentFormat.Html;
        //            case "rtf":
        //                return DevExpress.XtraRichEdit.DocumentFormat.Rtf;
        //            default:
        //                return DevExpress.XtraRichEdit.DocumentFormat.PlainText;
        //        }
        //    }
        //}
        public bool Saved { get; set; }
        public string Accion { get; set; }
        public bool CanSave { get; set; }
    }
    public class DocumentoEntrevistaModel
    {
        public long IdEmpresa { get; set; }
        public long IdDocumento { get; set; }
        public long IdTipoDocumento { get; set; }
        public long IdEntrevista { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionFechaInicio", ResourceType = typeof(Resources.DocumentoResource))]
        [DataType(DataType.DateTime, ErrorMessageResourceName = "InvalidoErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public DateTime Inicio { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionFechaFinal", ResourceType = typeof(Resources.DocumentoResource))]
        [DataType(DataType.DateTime, ErrorMessageResourceName = "InvalidoErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public DateTime Final { get; set; }
        [Display(Name = "captionParticipantes", ResourceType = typeof(Resources.DocumentoResource))]
        public List<DocumentoEntrevistaPersonaModel> Personas { get; set; }

        public DocumentoEntrevistaModel()
        {
            Personas = new List<DocumentoEntrevistaPersonaModel>();
        }
    }
    public class DocumentoEntrevistaPersonaModel
    {
        public long IdEmpresa { get; set; }
        public long IdDocumento { get; set; }
        public long IdTipoDocumento { get; set; }
        public long IdEntrevista { get; set; }
        public long IdPersona { get; set; }
        [Display(Name = "captionEntrevistador", ResourceType = typeof(Resources.DocumentoResource))]
        public bool Entrevistador { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionNombreParticipante", ResourceType = typeof(Resources.DocumentoResource))]
        [StringLength(1500, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Nombre { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionEmpresaParticipante", ResourceType = typeof(Resources.DocumentoResource))]
        [StringLength(1500, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Empresa { get; set; }
    }
    public class DocumentoPersonaClaveModel
    {
        public long IdEmpresa { get; set; }
        public long IdDocumento { get; set; }
        public long IdTipoDocumento { get; set; }
        public long IdPersona { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionNombrePersonaClave", ResourceType = typeof(Resources.DocumentoResource))]
        [StringLength(450, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Nombre { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionIdentificacionPersonaClave", ResourceType = typeof(Resources.DocumentoResource))]
        [StringLength(450, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Cédula { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionTelefonoOficina", ResourceType = typeof(Resources.DocumentoResource))]
        [StringLength(450, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string TelefonoOficina { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionTelefonoHabitacion", ResourceType = typeof(Resources.DocumentoResource))]
        [StringLength(450, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string TelefonoHabitacion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionTelefonoCelular", ResourceType = typeof(Resources.DocumentoResource))]
        [StringLength(450, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string TelefonoCelular { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionEmailPersonaClave", ResourceType = typeof(Resources.DocumentoResource))]
        [StringLength(450, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        [RegularExpression("^[\\w!#$%&'*+\\-/=?\\^_`{|}~]+(\\.[\\w!#$%&'*+\\-/=?\\^_`{|}~]+)*@((([\\-\\w]+\\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\\.){3}[0-9]{1,3}))$", ErrorMessageResourceName = "RegexErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionDireccion", ResourceType = typeof(Resources.DocumentoResource))]
        public string DireccionHabitacion { get; set; }
        [Display(Name = "captionPrincipalPersonaClave", ResourceType = typeof(Resources.DocumentoResource))]
        public bool Principal { get; set; }
        public bool Responsable { get; set; }

    }
    public class DocumentoProcesoModel
    {
        [Display(Name = "captionProceso", ResourceType = typeof(Resources.DocumentoResource))]
        public long IdProceso { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionNombreProceso", ResourceType = typeof(Resources.DocumentoResource))]
        [StringLength(450, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Nombre { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionDescripcionProceso", ResourceType = typeof(Resources.DocumentoResource))]
        public string Descripcion { get; set; }
        [Display(Name = "captionNroProceso", ResourceType = typeof(Resources.DocumentoResource))]
        public int NroProceso { get; set; }
        [Display(Name = "captionFechaCreacionProceso", ResourceType = typeof(Resources.DocumentoResource))]
        public DateTime FechaCreacion { get; set; }
        [Display(Name = "captionCriticoProceso", ResourceType = typeof(Resources.DocumentoResource))]
        public bool Critico { get; set; }
        [Display(Name = "captionEstatusProceso", ResourceType = typeof(Resources.DocumentoResource))]
        public long IdEstatus { get; set; }
        [Display(Name = "captionEstatusProceso", ResourceType = typeof(Resources.DocumentoResource))]
        public string Estatus
        {
            get
            {
                return Metodos.GetEstatusProceso(this.IdEstatus);
            }
        }
        [Display(Name = "captionFechaEstatusProceso", ResourceType = typeof(Resources.DocumentoResource))]
        public DateTime FechaEstatus { get; set; }
        public bool Selected { get; set; }
        public string ImpactoFinanciero { get; set; }
        public string ImpactoOperacional { get; set; }
        public string MTD { get; set; }
        public string RTO { get; set; }
        public string RPO { get; set; }
        public string WRT { get; set; }
        public string Evento { get; set; }

    }
    public class DocumentoDiagrama : DocumentoModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionNombreProceso", ResourceType = typeof(Resources.DocumentoResource))]
        public long IdProceso { get; set; }
        public string NroProceso { get; set; }
        public string Descripcion { get; set; }
        public string Nombre { get; set; }
        public string Interdependencias { get; set; }
        public string Clientes_Productos { get; set; }
        public string Entradas { get; set; }
        public string Proveedores { get; set; }
        public string PersonalClave { get; set; }
        public string Tecnologia { get; set; }
    }
    public class FrecuenciaModel : ModulosUserModel
    {
        [Display(Name = "captionFrecuencia", ResourceType = typeof(Resources.DocumentoResource))]
        public long IdFrecuencia { get; set; }
        [Display(Name = "captionFrecuencia", ResourceType = typeof(Resources.DocumentoResource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public long TipoFrecuencia { get; set; }
        [Display(Name = "captionModulo", ResourceType = typeof(Resources.DocumentoResource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public long IdModuloFrecuencia { get; set; }
        [Display(Name = "captionParticipante", ResourceType = typeof(Resources.DocumentoResource))]
        public List<long> Participantes { get; set; }
    }
    public class ProgramacionModel : ModulosUserModel
    {
        [Display(Name = "captionProgramacion", ResourceType = typeof(Resources.DocumentoResource))]
        public long IdProgramacion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionProgramacionModulo", ResourceType = typeof(Resources.DocumentoResource))]
        [Range(1, long.MaxValue, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public long IdModuloProgramacion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionProgramacionFechaInicio", ResourceType = typeof(Resources.DocumentoResource))]
        [DataType(DataType.DateTime, ErrorMessageResourceName = "InvalidoErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public DateTime FechaInicio { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionProgramacionFechaFinal", ResourceType = typeof(Resources.DocumentoResource))]
        [DataType(DataType.DateTime, ErrorMessageResourceName = "InvalidoErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public DateTime FechaFinal { get; set; }
        [Display(Name = "captionProgramacionEstadoProgramacion", ResourceType = typeof(Resources.DocumentoResource))]
        public long IdEstadoProgramacion { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionProgramacionTipoActualizacion", ResourceType = typeof(Resources.DocumentoResource))]
        //[Range(1, short.MaxValue, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public short IdTipoActualizacion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionFrecuencia", ResourceType = typeof(Resources.DocumentoResource))]
        [Range(1, long.MaxValue, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public long IdTipoFrecuencia { get; set; }
        public long IdTipoDocumento { get; set; }
        [Display(Name = "captionStatusMantenimiento", ResourceType = typeof(Resources.DocumentoResource))]
        public bool statusMantenimiento { get; set; }
        [Display(Name = "captionStatusDocumento", ResourceType = typeof(Resources.DocumentoResource))]
        public bool statusDocumento { get; set; }
        public string statusMantenimientoDescripcion { get; set; }
        public string statusDocumentoDescripcion { get; set; }
    }
    public class ProgramacionUsuarioModel
    {
        public long IdProgramacion { get; set; }
        public long IdUsuario { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaNotificacion { get; set; }
        public short IdTipoNotificacion { get; set; }
        public string TipoNotificacion { get; set; }
        public bool Selected { get; set; }
    }
    public class ProgramacionDocumentoModel
    {
        public long IdProgramacion { get; set; }
        public long IdDocumento { get; set; }
        public string NombreDocumento { get; set; }
        public long NroDocumento { get; set; }
        public long VersionOriginal { get; set; }
        public long NroVersion { get; set; }
        public string Documento
        {
            get
            {
                return string.Format("{0} - No. {1} V:{2}", NombreDocumento, NroDocumento.ToString(), string.Format("{0}.{1}", VersionOriginal.ToString(), NroVersion.ToString()));
            }
        }
        public eEstadoDocumento Estado { get; set; }
        public DateTime? FechaUltimoEstado { get; set; }
        public long IdModulo { get; set; }
        public long IdTipoDocumento { get; set; }
        public bool Selected { get; set; }
    }

}