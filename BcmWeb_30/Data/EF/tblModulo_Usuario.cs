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
    
    public partial class tblModulo_Usuario
    {
        public long IdEmpresa { get; set; }
        public long IdModulo { get; set; }
        public long IdUsuario { get; set; }
        public bool Actualizar { get; set; }
        public bool Eliminar { get; set; }
    
        public virtual tblModulo tblModulo { get; set; }
        public virtual tblUsuario tblUsuario { get; set; }
    }
}
