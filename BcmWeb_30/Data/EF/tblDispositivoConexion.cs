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
    
    public partial class tblDispositivoConexion
    {
        public long IdDispositivo { get; set; }
        public long IdUsuario { get; set; }
        public System.DateTime FechaConexion { get; set; }
        public Nullable<long> IdEmpresa { get; set; }
        public Nullable<long> IdSubModulo { get; set; }
        public string DirecciónIP { get; set; }
    
        public virtual tblDispositivo tblDispositivo { get; set; }
        public virtual tblEmpresa tblEmpresa { get; set; }
    }
}
