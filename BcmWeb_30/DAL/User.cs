using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcmWeb_30 .Security
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<EmpresaUsuario> Empresas { get; set; }
        public bool PrimeraVez { get; set; }
        public short Estatus { get; set; }
        public DateTime FechaUltimaConexion { get; set; }
    }
}