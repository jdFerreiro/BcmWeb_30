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
    
    public partial class tblProveedor
    {
        public long IdEmpresa { get; set; }
        public long IdProveedor { get; set; }
        public string Nombre { get; set; }
    
        public virtual tblEmpresa tblEmpresa { get; set; }
    }
}
