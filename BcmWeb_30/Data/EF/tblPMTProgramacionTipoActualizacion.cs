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
    
    public partial class tblPMTProgramacionTipoActualizacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPMTProgramacionTipoActualizacion()
        {
            this.tblCultura_PMTProgramacionTipoActualizacion = new HashSet<tblCultura_PMTProgramacionTipoActualizacion>();
            this.tblPMTProgramacion = new HashSet<tblPMTProgramacion>();
        }
    
        public short IdTipoActualizacion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCultura_PMTProgramacionTipoActualizacion> tblCultura_PMTProgramacionTipoActualizacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPMTProgramacion> tblPMTProgramacion { get; set; }
    }
}