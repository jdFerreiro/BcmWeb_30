using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcmWeb_30 .APIModels
{
    public class DispositivoRegistradoModel
    {
        public long Id { get; set; }
        public Nullable<long> IdUsuario { get; set; }
        public Nullable<DateTime> FechaRegistro { get; set; }
        public string IMEI_Dispositivo { get; set; }
        public string Nombre { get; set; }
        public string Fabricante { get; set; }
        public string Modelo { get; set; }
        public string Plataforma { get; set; }
        public string Version { get; set; }
        public string Tipo { get; set; }
        public string Token { get; set; }
        public string Usuario { get; set; }
    }
    public class DispositivoRegistradoConexion
    {
        public Nullable<long> IdEmpresa { get; set; }
        public long IdDispositivo { get; set; }
        public long IdUsuario { get; set; }
        public DateTime FechaConexion { get; set; }
        public string DireccionIP { get; set; }
        public Nullable<long> IdSubModulo { get; set; }
    }
    public class EnvioPendienteModel
    {
        public long IdDocumento { get; set; }
        public long IdEmpresa { get; set; }
        public string NombreDocumento { get; set; }
        public string RutaDocumento { get; set; }
    }

}