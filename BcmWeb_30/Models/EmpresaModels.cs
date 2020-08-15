using BcmWeb_30.Data.EF;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BcmWeb_30 .Models
{
    public class EmpresaModel : ModulosUserModel
    {
        private static string Culture = HttpContext.Current.Request.UserLanguages[0];

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionNombreFiscal", ResourceType = typeof(Resources.AdministracionResource))]
        [StringLength(250, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string NombreFiscal { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionNombreComercial", ResourceType = typeof(Resources.AdministracionResource))]
        [StringLength(250, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string NombreComercial { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionRegistroFiscal", ResourceType = typeof(Resources.AdministracionResource))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string RegistroFiscal { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionDireccion", ResourceType = typeof(Resources.AdministracionResource))]
        public string Direccion { get; set; }
        public long IdEstatus { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionPais", ResourceType = typeof(Resources.AdministracionResource))]
        public long IdPais { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionCiudad", ResourceType = typeof(Resources.AdministracionResource))]
        public long IdCiudad { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionEstado", ResourceType = typeof(Resources.AdministracionResource))]
        public long IdEstado { get; set; }
        [Display(Name = "captionEstatus", ResourceType = typeof(Resources.AdministracionResource))]
        public string Estatus
        {
            get
            {
                using (Entities db = new Entities())
                {
                    string _Estatus = db.tblEmpresa.FirstOrDefault(x => x.IdEmpresa == IdEmpresa).tblEstadoEmpresa.tblCultura_EstadoEmpresa.FirstOrDefault(x => x.Culture == Culture || x.Culture == "es-VE").Descripcion ?? string.Empty;
                    return _Estatus;
                }
            }
        }
        [Display(Name = "captionPais", ResourceType = typeof(Resources.AdministracionResource))]
        public string Pais
        {
            get
            {
                using (Entities db = new Entities())
                {
                    string _result = db.tblEmpresa.FirstOrDefault(x => x.IdEmpresa == IdEmpresa).tblPais.tblCultura_Pais.FirstOrDefault(x => x.Culture == Culture || x.Culture == "es-VE").Nombre ?? string.Empty;
                    return _result;
                }
            }
        }
        [Display(Name = "captionCiudad", ResourceType = typeof(Resources.AdministracionResource))]
        public string Ciudad
        {
            get
            {
                using (Entities db = new Entities())
                {
                    string _result = db.tblEmpresa.FirstOrDefault(x => x.IdEmpresa == IdEmpresa).tblCiudad.tblCultura_Ciudad.FirstOrDefault(x => x.Culture == Culture || x.Culture == "es-VE").Nombre ?? string.Empty;
                    return _result;
                }
            }
        }
        [Display(Name = "captionEstado", ResourceType = typeof(Resources.AdministracionResource))]
        public string Estado
        {
            get
            {
                using (Entities db = new Entities())
                {
                    string _result = db.tblEmpresa.FirstOrDefault(x => x.IdEmpresa == IdEmpresa).tblEstado.tblCultura_Estado.FirstOrDefault(x => x.Culture == Culture || x.Culture == "es-VE").Nombre ?? string.Empty;
                    return _result;
                }
            }
        }
        [Display(Name = "captionFechaUltimoEstado", ResourceType = typeof(Resources.AdministracionResource))]
        public DateTime FechaUltimoEstado { get; set; }
        [Display(Name = "captionLogo", ResourceType = typeof(Resources.AdministracionResource))]
        public string LogoUrl { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionFechaInicio", ResourceType = typeof(Resources.AdministracionResource))]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        public bool CanDelete { get; set; }
        public UploadedFile[] ImagesProperty { get; set; }
        public List<ModuloAsignadoModel> ModulosAsignados { get; set; }
        public string ErrorUpdating { get; set; }

    }
    public class ModuloAsignadoModel
    {
        public long IdEmpresa { get; set; }
        public long IdModuloAsignado { get; set; }
        public long IdUsuarioPermiso { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Selected { get; set; }
        public bool Actualiza { get; set; }
        public bool Elimina { get; set; }
    }
}