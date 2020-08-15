using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BcmWeb_30 .Models
{
    public class PersonaModel : ModulosUserModel
    {
        [Display(Name = "captionNombre", ResourceType = typeof(Resources.FichaResource))]
        public long IdPersona { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionNombre", ResourceType = typeof(Resources.FichaResource))]
        [StringLength(250, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Nombre { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionIdentificacion", ResourceType = typeof(Resources.FichaResource))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public string Identificacion { get; set; }
        [Display(Name = "captionUO", ResourceType = typeof(Resources.FichaResource))]
        [Range(1, long.MaxValue, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public long IdUnidadOrganizativaPersona { get; set; }
        [Display(Name = "captionUO", ResourceType = typeof(Resources.FichaResource))]
        public UnidadOrganizativaModel UnidadOrganizativa { get; set; }
        [Display(Name = "captionCargo", ResourceType = typeof(Resources.FichaResource))]
        [Range(1, long.MaxValue, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public long IdCargoPersona { get; set; }
        [Display(Name = "captionCargo", ResourceType = typeof(Resources.FichaResource))]
        public CargoModel Cargo { get; set; }
        public long IdUsuario { get; set; }
        [Remote("CheckEmails", "Documentos", ErrorMessageResourceType = typeof(Resources.ErrorResource), ErrorMessageResourceName = "RemoteEmail")]
        public List<PersonaEmail> CorreosElectronicos { get; set; }
        public List<PersonaDireccion> Direcciones { get; set; }
        [Remote("CheckTelefonos", "Documentos", ErrorMessageResourceType = typeof(Resources.ErrorResource), ErrorMessageResourceName = "RemoteEmail")]
        public List<PersonaTelefono> Telefonos { get; set; }

        public PersonaModel()
        {
            this.CorreosElectronicos = new List<PersonaEmail>();
            this.Direcciones = new List<PersonaDireccion>();
            this.Telefonos = new List<PersonaTelefono>();
        }
    }
    public class PersonaEmail
    {
        [Display(Name = "captionEmail", ResourceType = typeof(Resources.FichaResource))]
        public long IdPersonaEmail { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionEmail", ResourceType = typeof(Resources.FichaResource))]
        [StringLength(450, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        [RegularExpression("^[\\w!#$%&'*+\\-/=?\\^_`{|}~]+(\\.[\\w!#$%&'*+\\-/=?\\^_`{|}~]+)*@((([\\-\\w]+\\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\\.){3}[0-9]{1,3}))$", ErrorMessageResourceName = "InvalidoErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionTipoEmail", ResourceType = typeof(Resources.FichaResource))]
        public long IdTipoEmail { get; set; }
        [Display(Name = "captionTipoEmail", ResourceType = typeof(Resources.FichaResource))]
        public string TipoEmail { get; set; }

    }
    public class PersonaDireccion
    {
        [Display(Name = "captionIdPersonaDireccion", ResourceType = typeof(Resources.FichaResource))]
        public long IdPersonaDireccion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionTipoDireccion", ResourceType = typeof(Resources.FichaResource))]
        public long IdTipoDireccion { get; set; }
        [Display(Name = "captionTipoDireccion", ResourceType = typeof(Resources.FichaResource))]
        public string TipoDireccion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionUbicacion", ResourceType = typeof(Resources.FichaResource))]
        [StringLength(450, ErrorMessageResourceName = "StringLengthErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Ubicación { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionPais", ResourceType = typeof(Resources.FichaResource))]
        public long IdPais { get; set; }
        [Display(Name = "captionPais", ResourceType = typeof(Resources.FichaResource))]
        public string Pais
        {
            get
            {
                return Metodos.GetPais(this.IdPais);
            }
        }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionEstado", ResourceType = typeof(Resources.FichaResource))]
        public long IdEstado { get; set; }
        [Display(Name = "captionEstado", ResourceType = typeof(Resources.FichaResource))]
        public string Estado
        {
            get
            {
                return Metodos.GetEstado(this.IdPais, this.IdEstado);
            }
        }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionCiudad", ResourceType = typeof(Resources.FichaResource))]
        public long IdCiudad { get; set; }
        [Display(Name = "captionCiudad", ResourceType = typeof(Resources.FichaResource))]
        public string Ciudad
        {
            get
            {
                return Metodos.GetCiudad(this.IdPais, this.IdEstado, this.IdCiudad);
            }
        }


    }
    public class PersonaTelefono
    {
        [Display(Name = "captionIdTelefono", ResourceType = typeof(Resources.FichaResource))]
        public long IdPersonaTelefono { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionTipoTelefono", ResourceType = typeof(Resources.FichaResource))]
        public long IdTipoTelefono { get; set; }
        [Display(Name = "captionTipoTelefono", ResourceType = typeof(Resources.FichaResource))]
        public string TipoTelefono { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionNroTelefono", ResourceType = typeof(Resources.FichaResource))]
        public string NroTelefono { get; set; }
    }
    public class CargoModel
    {
        public long IdCargo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionCargo", ResourceType = typeof(Resources.FichaResource))]
        public string NombreCargo { get; set; }
    }
}