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
    
    public partial class tblPBEPruebaEstatus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPBEPruebaEstatus()
        {
            this.tblCultura_PBEPruebaEstatus = new HashSet<tblCultura_PBEPruebaEstatus>();
            this.tblPBEPruebaEjecucionEjercicio = new HashSet<tblPBEPruebaEjecucionEjercicio>();
        }
    
        public short IdEstatus { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCultura_PBEPruebaEstatus> tblCultura_PBEPruebaEstatus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPBEPruebaEjecucionEjercicio> tblPBEPruebaEjecucionEjercicio { get; set; }
    }
}