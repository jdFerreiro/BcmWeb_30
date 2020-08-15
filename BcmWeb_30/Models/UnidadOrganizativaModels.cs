using BcmWeb_30.Data.EF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace BcmWeb_30 .Models
{

    public class UnidadOrganizativaModel
    {
        public long IdUnidad { get; set; }
        public long IdUnidadPadre { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionUO", ResourceType = typeof(Resources.FichaResource))]
        public string NombreUnidadOrganizativa { get; set; }
        public string NombreCompleto
        {
            get
            {
                return Metodos.GetNombreUnidadCompleto(this.IdUnidad);
            }
        }
        public UnidadOrganizativaModel() { }
        //public UnidadOrganizativaModel(long idUnidad, long idUnidadPadre, string nombreUnidad)
        //{
        //    IdUnidadPadre = idUnidadPadre;
        //    IdUnidad = idUnidad;
        //    NombreUnidadOrganizativa = nombreUnidad;
        //}
    }
}
