namespace BcmWeb_30 .APIModels
{
    public class AuditoriaModel
    {
        public long Id { get; set; }
        public long IdEmpresa { get; set; }
        public long IdDocumento { get; set; }
        public long IdTipoDocumento { get; set; }
        public string IpAddress { get; set; }
        public string Mensaje { get; set; }
        public string Accion { get; set; }
        public long IdUsuario { get; set; }
        public bool Negocios { get; set; }
    }
}