using System;

namespace BcmWeb_30 .Models
{
    public class BiaDocsFilterModel
    {
        public long NroDocumento { get; set; }
        public DateTime fromCreacion { get; set; }
        public DateTime toCreacion { get; set; }
        public long EstadoDocumento { get; set; }
        public long IdProceso { get; set; }
        public bool ProcesoCritico { get; set; }
        public long IdUnidadOrganizativa { get; set; }
    }
}