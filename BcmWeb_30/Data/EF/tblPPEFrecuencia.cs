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
    
    public partial class tblPPEFrecuencia
    {
        public long IdEmpresa { get; set; }
        public long IdDocumento { get; set; }
        public long IdTipoDocumento { get; set; }
        public long IdPPEFrecuencia { get; set; }
        public string TipoPrueba { get; set; }
        public string Participantes { get; set; }
        public string Proposito { get; set; }
        public long IdTipoFrecuencia { get; set; }
    
        public virtual tblDocumento tblDocumento { get; set; }
        public virtual tblTipoFrecuencia tblTipoFrecuencia { get; set; }
    }
}