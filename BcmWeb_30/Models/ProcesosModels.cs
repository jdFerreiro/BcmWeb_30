using System.Collections.Generic;

namespace BcmWeb_30 .Models
{
    public class CriticoModel : ModulosUserModel
    {
        public string ImpactoFinancieroSelected { get; set; }
        public string ImpactoOperacionalSelected { get; set; }
        public string MTDSelected { get; set; }
        public string RTOSelected { get; set; }
        public string RPOSelected { get; set; }
        public string WRTSelected { get; set; }
        public IList<DocumentoProcesoModel> Procesos { get; set; }
    }
    public class ProcesoValoresModel : ModulosUserModel
    {
        public long IdProceso { get; set; }
        public long IdDocumentoBIA { get; set; }
        public long IdImpacto { get; set; }
        public int NroProceso { get; set; }
        public string NombreProceso { get; set; }
        public string Impacto { get; set; }
        public long IdTipoEscala { get; set; }
    }

    public class ProcesoImpactoModel : ModulosUserModel
    {
        public string Impacto { get; set; }
        public long IdEscala { get; set; }
    }

}