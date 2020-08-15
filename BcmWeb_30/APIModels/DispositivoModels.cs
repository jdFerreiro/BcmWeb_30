using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcmWeb_30 .APIModels
{
    public class DispositivoModel
    {
        public long Id { get; set; }
        public Nullable<DateTime> FechaRegistro { get; set; }
        public string IdUnicoDispositivo { get; set; }
        public string nombre { get; set; }
        public string fabricante { get; set; }
        public string modelo { get; set; }
        public string plataforma { get; set; }
        public string version { get; set; }
        public string tipo { get; set; }
        public string token { get; set; }
    }
    public class DispositivoConexion
    {
        public long IdEmpresa { get; set; }
        public long IdDispositivo { get; set; }
        public long IdUsuario { get; set; }
        public DateTime FechaConexion { get; set; }
        public string DireccionIP { get; set; }
    }
    public class DocumentosPendientes
    {
        public long IdDocumento { get; set; }
        public long IdEmpresa { get; set; }
        public string NombreDocumento { get; set; }
        public string RutaDocumento { get; set; }
    }


}