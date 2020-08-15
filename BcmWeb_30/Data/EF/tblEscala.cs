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
    
    public partial class tblEscala
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblEscala()
        {
            this.tblBIAImpactoFinanciero = new HashSet<tblBIAImpactoFinanciero>();
            this.tblBIAImpactoOperacional = new HashSet<tblBIAImpactoOperacional>();
            this.tblBIAMTD = new HashSet<tblBIAMTD>();
            this.tblBIARPO = new HashSet<tblBIARPO>();
            this.tblBIARTO = new HashSet<tblBIARTO>();
            this.tblBIAWRT = new HashSet<tblBIAWRT>();
            this.tblCriticidad = new HashSet<tblCriticidad>();
        }
    
        public long IdEmpresa { get; set; }
        public long IdEscala { get; set; }
        public long IdTipoEscala { get; set; }
        public short Valor { get; set; }
        public string Descripcion { get; set; }
        public Nullable<System.DateTime> FechaRegistro { get; set; }
        public Nullable<int> PosicionEscala { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBIAImpactoFinanciero> tblBIAImpactoFinanciero { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBIAImpactoOperacional> tblBIAImpactoOperacional { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBIAMTD> tblBIAMTD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBIARPO> tblBIARPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBIARTO> tblBIARTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBIAWRT> tblBIAWRT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCriticidad> tblCriticidad { get; set; }
        public virtual tblEmpresa tblEmpresa { get; set; }
        public virtual tblTipoEscala tblTipoEscala { get; set; }
    }
}
