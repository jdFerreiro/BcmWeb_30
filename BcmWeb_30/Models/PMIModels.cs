using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BcmWeb_30 .Models
{
    public class PMIModel : ModulosUserModel
    {
        public long IdIncidente { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionTipoIncidente", ResourceType = typeof(Resources.PMIResource))]
        public int IdTipoIncidente { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionDescripcion", ResourceType = typeof(Resources.PMIResource))]
        public string Descripcion { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionNaturalezaIncidente", ResourceType = typeof(Resources.PMIResource))]
        public int IdNaturalezaIncidente { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionFuenteIncidente", ResourceType = typeof(Resources.PMIResource))]
        public int IdFuenteIncidente { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionFechaIncidente", ResourceType = typeof(Resources.PMIResource))]
        [DataType(DataType.DateTime, ErrorMessageResourceName = "InvalidoErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [FechaMayorIgualAHoy(ErrorMessageResourceName = "FechaMayorIgualAHoy", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public Nullable<DateTime> FechaIncidente { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionLugarIncidente", ResourceType = typeof(Resources.PMIResource))]
        public string LugarIncidente { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionDuracion", ResourceType = typeof(Resources.PMIResource))]
        public int Duracion { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionNombreReportero", ResourceType = typeof(Resources.PMIResource))]
        [StringLength(500, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public string NombreReportero { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionNombreSolucionador", ResourceType = typeof(Resources.PMIResource))]
        [StringLength(500, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public string NombreSolucionador { get; set; }
        [Display(Name = "captionObservacion", ResourceType = typeof(Resources.PMIResource))]
        [StringLength(2000, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public string Observacion { get; set; }
        public string RutaDocumentoPDF
        {
            get
            {
                Uri _ContextUrl = HttpContext.Current.Request.Url;
                string _AppUrl = _ContextUrl.AbsoluteUri.Replace(_ContextUrl.PathAndQuery, string.Empty);
                HttpServerUtility _Server = HttpContext.Current.Server;
                string _ServerPath = _Server.MapPath(".").Replace("\\Account", string.Empty);
                string _FileName = string.Format("INC{0}{1}.pdf", IdEmpresa.ToString(), IdIncidente.ToString());
                string _pathFile = String.Format("{0}\\PDFDocs\\{1}", _ServerPath, _FileName);

                if (System.IO.File.Exists(_pathFile))
                    return String.Format("{0}/PDFDocs/{1}", _AppUrl, _FileName);
                else
                    return string.Empty;
            }
        }
    }
}