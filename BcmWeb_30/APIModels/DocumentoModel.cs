namespace BcmWeb_30 .APIModels
{
    public class DocumentoModel
    {
        public long Id { get; set; }
        public string Modulo { get; set; }
        public long IdTipoDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public string NombreDocumento { get; set; }
        public long NroDocumento { get; set; }
        public bool Negocios { get; set; }
        public int NroVersion { get; set; }
        public int VersionOriginal { get; set; }
        public string PdfRoute { get; set; }
    }

}