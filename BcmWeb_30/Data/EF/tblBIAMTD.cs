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
    
    public partial class tblBIAMTD
    {
        public long IdEmpresa { get; set; }
        public long IdDocumentoBIA { get; set; }
        public long IdProceso { get; set; }
        public long IdMTD { get; set; }
        public string Observacion { get; set; }
        public Nullable<long> IdTipoFrecuencia { get; set; }
        public Nullable<long> IdEscala { get; set; }
    
        public virtual tblBIAProceso tblBIAProceso { get; set; }
        public virtual tblEscala tblEscala { get; set; }
        public virtual tblTipoFrecuencia tblTipoFrecuencia { get; set; }
    }
}
