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
    
    public partial class tblPBEPruebaPlanificacionEjercicioParticipante
    {
        public long IdEmpresa { get; set; }
        public long IdPlanificacion { get; set; }
        public long IdEjercicio { get; set; }
        public long IdParticipante { get; set; }
        public bool Responsable { get; set; }
    
        public virtual tblPBEPruebaPlanificacionEjercicio tblPBEPruebaPlanificacionEjercicio { get; set; }
        public virtual tblPBEPruebaPlanificacionParticipante tblPBEPruebaPlanificacionParticipante { get; set; }
    }
}
