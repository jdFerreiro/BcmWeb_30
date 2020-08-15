using System.Collections.Generic;

namespace BcmWeb_30 .APIModels
{
    public class ModuloModel
    {
        public long Id { get; set; }
        public long IdPadre { get; set; }
        public string Nombre { get; set; }
        public short IdTipoElemento { get; set; }
        public int IdCodigoModulo { get; set; }
        public bool Negocios { get; set; }
        public bool Tecnologia { get; set; }

        public List<DocumentoModel> Documentos { get; set; }
    }
}