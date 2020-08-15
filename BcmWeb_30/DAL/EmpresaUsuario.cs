using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcmWeb_30 .Security
{
    public class EmpresaUsuario
    {
        public long IdEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
        public List<long> Modulos { get; set; }
    }
}