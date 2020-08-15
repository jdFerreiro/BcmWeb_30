using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BcmWeb_30 .Models
{
    public class ContactoViewModel : ModulosUserModel
    {
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "labelNombre", ResourceType = typeof(Resources.ContactResource))]
        [StringLength(250, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Nombre { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "labelEmailContacto", ResourceType = typeof(Resources.ContactResource))]
        public string Email { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "labelAsunto", ResourceType = typeof(Resources.ContactResource))]
        [StringLength(500, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Asunto { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "labelMensaje", ResourceType = typeof(Resources.ContactResource))]
        public string Mensaje { get; set; }
    }
}