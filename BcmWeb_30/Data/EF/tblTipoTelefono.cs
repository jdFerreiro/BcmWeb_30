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
    
    public partial class tblTipoTelefono
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblTipoTelefono()
        {
            this.tblCultura_TipoTelefono = new HashSet<tblCultura_TipoTelefono>();
            this.tblPersonaTelefono = new HashSet<tblPersonaTelefono>();
        }
    
        public long IdTipoTelefono { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCultura_TipoTelefono> tblCultura_TipoTelefono { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPersonaTelefono> tblPersonaTelefono { get; set; }
    }
}