using System;
using System.ComponentModel.DataAnnotations;

namespace BcmWeb_30 .Models
{
    public class IniciativaModel : ModulosUserModel
    {
        [Display(Name = "IdIniciativa", ResourceType = typeof(Resources.IniciativaResource))]
        public long IdIniciativa { get; set; }
        [Display(Name = "NroIniciativa", ResourceType = typeof(Resources.IniciativaResource))]
        public int NroIniciativa { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "Nombre", ResourceType = typeof(Resources.IniciativaResource))]
        [StringLength(1500, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Nombre { get; set; }
        [Display(Name = "Descripcion", ResourceType = typeof(Resources.IniciativaResource))]
        public string Descripcion { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        //[Display(Name = "UnidadOrganizativa", ResourceType = typeof(Resources.IniciativaResource))]
        //[Range(1, long.MaxValue, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        //public long? IdUnidadOrganizativa { get; set; }
        //[Display(Name = "UnidadOrganizativa", ResourceType = typeof(Resources.IniciativaResource))]
        //public string UnidadOrganizativa
        //{
        //    get
        //    {
        //        return IdUnidadOrganizativa == null ? string.Empty : Metodos.GetNombreUnidadCompleto((long)IdUnidadOrganizativa);
        //    }
        //}
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        //[Display(Name = "Responsable", ResourceType = typeof(Resources.IniciativaResource))]
        //[Range(1, long.MaxValue, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        //public long? Responsable { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "UnidadOrganizativa", ResourceType = typeof(Resources.IniciativaResource))]
        [StringLength(1500, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string UnidadOrganizativa { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "Responsable", ResourceType = typeof(Resources.IniciativaResource))]
        [StringLength(1500, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Responsable { get; set; }
        [Display(Name = "FechaInicioEstimada", ResourceType = typeof(Resources.IniciativaResource))]
        public Nullable<DateTime> FechaInicioEstimada { get; set; }
        [Display(Name = "FechaCierreEstimada", ResourceType = typeof(Resources.IniciativaResource))]
        public Nullable<DateTime> FechaCierreEstimada { get; set; }
        [Display(Name = "FechaInicioReal", ResourceType = typeof(Resources.IniciativaResource))]
        public Nullable<DateTime> FechaInicioReal { get; set; }
        [Display(Name = "FechaCierreReal", ResourceType = typeof(Resources.IniciativaResource))]
        public Nullable<DateTime> FechaCierreReal { get; set; }
        [Display(Name = "PresupuestoEstimado", ResourceType = typeof(Resources.IniciativaResource))]
        public Nullable<decimal> PresupuestoEstimado { get; set; }
        [Display(Name = "PresupuestoReal", ResourceType = typeof(Resources.IniciativaResource))]
        public Nullable<decimal> PresupuestoReal { get; set; }
        [Display(Name = "MontoAbonado", ResourceType = typeof(Resources.IniciativaResource))]
        public Nullable<decimal> MontoAbonado { get; set; }
        [Display(Name = "MontoPendiente", ResourceType = typeof(Resources.IniciativaResource))]
        public Nullable<decimal> MontoPendiente { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "Estatus", ResourceType = typeof(Resources.IniciativaResource))]
        [Range(1, short.MaxValue, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public short? IdEstatus { get; set; }
        [Display(Name = "Estatus", ResourceType = typeof(Resources.IniciativaResource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public string Estatus
        {
            get
            {
                return (IdEstatus == null ? string.Empty : Metodos.GetEstatusIniciativa((short)IdEstatus));
            }
        }
        [Display(Name = "Urgente", ResourceType = typeof(Resources.IniciativaResource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public Nullable<short> Urgente { get; set; }
        [Display(Name = "Urgente", ResourceType = typeof(Resources.IniciativaResource))]
        public string strUrgente
        {
            get
            {
                if (Urgente != null)
                {
                    return Metodos.GetSeveridad(Urgente);
                }
                else
                {
                    return Resources.IniciativaResource.textoNormal;
                }
            }
        }
        [Display(Name = "Observacion", ResourceType = typeof(Resources.IniciativaResource))]
        public string Observacion { get; set; }
        [Display(Name = "PorcentajeAvance", ResourceType = typeof(Resources.IniciativaResource))]
        public Nullable<decimal> PorcentajeAvance { get; set; }
        [Display(Name = "HorasEstimadas", ResourceType = typeof(Resources.IniciativaResource))]
        public Nullable<int> HorasEstimadas { get; set; }
        [Display(Name = "HorasInvertidas", ResourceType = typeof(Resources.IniciativaResource))]
        public Nullable<int> HorasInvertidas { get; set; }
        public bool? hasFiles { get; set; }

        public string MyProperty { get; set; }
    }
    public class AnexosIniciativaModel : ModulosUserModel
    {
        [Display(Name = "captionIdAnexo", ResourceType = typeof(Resources.IniciativaResource))]
        public long IdIniciativa { get; set; }
        public long id { get; set; }
        [Display(Name = "captionNombreAnexo", ResourceType = typeof(Resources.IniciativaResource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [StringLength(1500, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Nombre { get; set; }
        [Display(Name = "captionRutaAnexo", ResourceType = typeof(Resources.IniciativaResource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [StringLength(1500, ErrorMessageResourceName = "StringLengthErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 5)]
        public string Ruta { get; set; }
        [Display(Name = "captionFechaRegistro", ResourceType = typeof(Resources.IniciativaResource))]
        public DateTime? fechaRegistro { get; set; }
    }
}