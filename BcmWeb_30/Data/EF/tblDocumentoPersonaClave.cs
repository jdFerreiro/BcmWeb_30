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
    
    public partial class tblDocumentoPersonaClave
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblDocumentoPersonaClave()
        {
            this.tblBIAPersonaClave = new HashSet<tblBIAPersonaClave>();
        }
    
        public long IdEmpresa { get; set; }
        public long IdDocumento { get; set; }
        public long IdTipoDocumento { get; set; }
        public long IdPersona { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string TelefonoOficina { get; set; }
        public string TelefonoCelular { get; set; }
        public string TelefonoHabitacion { get; set; }
        public string Correo { get; set; }
        public string DireccionHabitacion { get; set; }
        public Nullable<bool> Principal { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBIAPersonaClave> tblBIAPersonaClave { get; set; }
        public virtual tblDocumento tblDocumento { get; set; }
        public virtual tblPersona tblPersona { get; set; }
        public virtual tblEmpresa tblEmpresa { get; set; }
    }
}
