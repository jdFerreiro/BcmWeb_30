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
    
    public partial class tblPMTProgramacionTipoNotificacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPMTProgramacionTipoNotificacion()
        {
            this.tblCultura_PMTProgramacionTipoNotificacion = new HashSet<tblCultura_PMTProgramacionTipoNotificacion>();
            this.tblPMTProgramacionUsuario = new HashSet<tblPMTProgramacionUsuario>();
        }
    
        public short IdTipoNotificacion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCultura_PMTProgramacionTipoNotificacion> tblCultura_PMTProgramacionTipoNotificacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPMTProgramacionUsuario> tblPMTProgramacionUsuario { get; set; }
    }
}
