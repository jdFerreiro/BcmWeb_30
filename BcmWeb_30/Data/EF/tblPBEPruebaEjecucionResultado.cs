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
    
    public partial class tblPBEPruebaEjecucionResultado
    {
        public long IdEmpresa { get; set; }
        public long IdPlanificacion { get; set; }
        public long IdContenido { get; set; }
        public byte[] Contenido { get; set; }
    
        public virtual tblPBEPruebaEjecucion tblPBEPruebaEjecucion { get; set; }
    }
}
