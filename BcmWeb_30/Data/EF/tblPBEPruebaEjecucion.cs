//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BcmWeb_30.Data.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblPBEPruebaEjecucion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPBEPruebaEjecucion()
        {
            this.tblPBEPruebaEjecucionEjercicio = new HashSet<tblPBEPruebaEjecucionEjercicio>();
            this.tblPBEPruebaEjecucionParticipante = new HashSet<tblPBEPruebaEjecucionParticipante>();
            this.tblPBEPruebaEjecucionResultado = new HashSet<tblPBEPruebaEjecucionResultado>();
        }
    
        public long IdEmpresa { get; set; }
        public long IdPlanificacion { get; set; }
        public System.DateTime FechaInicio { get; set; }
        public string Lugar { get; set; }
    
        public virtual tblEmpresa tblEmpresa { get; set; }
        public virtual tblPBEPruebaPlanificacion tblPBEPruebaPlanificacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPBEPruebaEjecucionEjercicio> tblPBEPruebaEjecucionEjercicio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPBEPruebaEjecucionParticipante> tblPBEPruebaEjecucionParticipante { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPBEPruebaEjecucionResultado> tblPBEPruebaEjecucionResultado { get; set; }
    }
}
