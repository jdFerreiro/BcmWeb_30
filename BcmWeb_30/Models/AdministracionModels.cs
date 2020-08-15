using System;
using System.Collections.Generic;
using System.Linq;

namespace BcmWeb_30 .Models
{
    public class AdministracionModel : ModulosUserModel
    {
        public long IdModuloActualiza { get; set; }
        public long IdEmpresaSelected { get; set; }
        public IList<ModuloModel> Modulos { get; set; }
    }
    public class AuditoriaAdminModel : ModulosUserModel
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public long? IdUsuario { get; set; }
        public TablasGeneralesModel Usuarios { get; set; }
        public IQueryable<AuditoriaModel> Data { get; set; }
    }
    public class ModulosUsuario : ModulosUserModel
    {
        public long IdEmpresaSelected { get; set; }
        public long IdUsuario { get; set; }
        public long IdNivelUsuario { get; set; }
        public List<dataModulosUsuario> ModuloUsuario { get; set; }
    }
    public class dataModulosUsuario
    {
        public long IdModulo { get; set; }
        public string Nombre { get; set; }
        public long IdPadre { get; set; }
        public short TipoElemento { get; set; }
        public bool Selected { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }

    }
}