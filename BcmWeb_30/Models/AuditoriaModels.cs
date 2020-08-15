using BcmWeb_30.Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcmWeb_30 .Models
{

    public class AuditoriaModels : ModulosUserModel
    {
        public List<AuditoriaModel> Auditoria { get; set; }
    }
    public class AuditoriaModel : ModulosUserModel
    {
        public long Id { get; set; }
        public string Empresa { get; set; }
        public long IdDocumento { get; set; }
        public long NroDocumento { get; set; }
        public long IdTipoDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string DireccionIP { get; set; }
        public string Mensaje { get; set; }
        public string Accion { get; set; }
        public long IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public bool Negocios { get; set; }
        public string datosmodificados { get; set; }
    }
}