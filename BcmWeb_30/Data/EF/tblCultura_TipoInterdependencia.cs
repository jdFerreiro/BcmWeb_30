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
    
    public partial class tblCultura_TipoInterdependencia
    {
        public string Culture { get; set; }
        public int IdTipoInterdependencia { get; set; }
        public string Descripcion { get; set; }
    
        public virtual tblTipoInterdependencia tblTipoInterdependencia { get; set; }
    }
}
