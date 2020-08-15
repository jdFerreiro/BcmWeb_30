using System;
using System.Collections.Generic;

namespace BcmWeb_30 
{

    public class objImpactoOperacional
    {
        public long IdProceso { get; set; }
        public string Proceso { get; set; }
        public string ImpactoOperacional { get; set; }
        public int Escala { get; set; }
        public string DescEscala { get; set; }
    }
    public class objValorImpacto
    {
        public long IdProceso { get; set; }
        public string Proceso { get; set; }
        public int EscalaMTD { get; set; }
        public string DescEscalaMTD { get; set; }
        public int EscalaRPO { get; set; }
        public string DescEscalaRPO { get; set; }
        public int EscalaRTO { get; set; }
        public string DescEscalaRTO { get; set; }
        public int EscalaWRT { get; set; }
        public string DescEscalaWRT { get; set; }
    }
    public class objProcesoMes
    {
        public string Proceso_M01 { get; set; }
        public string Proceso_M02 { get; set; }
        public string Proceso_M03 { get; set; }
        public string Proceso_M04 { get; set; }
        public string Proceso_M05 { get; set; }
        public string Proceso_M06 { get; set; }
        public string Proceso_M07 { get; set; }
        public string Proceso_M08 { get; set; }
        public string Proceso_M09 { get; set; }
        public string Proceso_M10 { get; set; }
        public string Proceso_M11 { get; set; }
        public string Proceso_M12 { get; set; }
    }
    public class objCantidadIO
    {
        public long IdUnidad { get; set; }
        public string Unidad
        {
            get
            {
                return Metodos.GetNombreUnidadCompleto(IdUnidad);
            }
        }
        public IList<Int16> ValorEscala { get; set; }
        public IList<string> Escala { get; set; }
        public IList<Int32> CantidadEscala { get; set; }
    }
    public class objCantidadVI
    {
        public long IdUnidad { get; set; }
        public string Unidad
        {
            get
            {
                return Metodos.GetNombreUnidadCompleto(IdUnidad);
            }
        }
        public IList<string> EscalaMTD { get; set; }
        public IList<int> ValoresMTD { get; set; }
        public IList<string> EscalaRPO { get; set; }
        public IList<int> ValoresRPO { get; set; }
        public IList<string> EscalaRTO { get; set; }
        public IList<int> ValoresRTO { get; set; }
        public IList<string> EscalaWRT { get; set; }
        public IList<int> ValoresWRT { get; set; }
    }
    public class objCantidadGI
    {
        public long IdUnidad { get; set; }
        public string Unidad
        {
            get
            {
                return Metodos.GetNombreUnidadCompleto(IdUnidad);
            }
        }
        public IList<long> Mes { get; set; }
        public IList<string> nombreMes { get; set; }
        public IList<int> Valores { get; set; }
    }
    public class objGraphIO
    {
        public long IdUnidad { get; set; }
        public string Unidad { get; set; }
        public long IdEscala { get; set; }
        public string Escala { get; set; }
        public int Cantidad { get; set; }
    }
    public class objDataGrafico
    {
        public long IdUnidad { get; set; }
        public long IdEscala { get; set; }
        public string ValorEscala { get; set; }
        public int CantidadEscala { get; set; }
    }
    public class objGraphGI
    {
        public long IdUnidad { get; set; }
        public string Unidad { get; set; }
        public string Mes { get; set; }
        public int Cantidad { get; set; }
    }
    public class objAmenaza
    {
        public long IdProceso { get; set; }
        public long IdDocumentoBIA { get; set; }
        public long IdAmenaza { get; set; }
        public string Descripcion { get; set; }
        public string Evento { get; set; }
        public string Implantado { get; set; }
        public string Implantar { get; set; }
        public short Probabilidad { get; set; }
        public short Severidad { get; set; }
        public short Control { get; set; }
        public short Impacto { get; set; }
        public string Fuente { get; set; }
        public short Estado { get; set; }
    }
    public class objTablaAmenaza
    {
        public long IdUnidad { get; set; }
        public string UnidadOrganizativa { get; set; }
        public int CodigoProceso { get; set; }
        public string Proceso { get; set; }
        public string Amenaza { get; set; }
        public string Evento { get; set; }
        public int Probabilidad { get; set; }
        public int Severidad { get; set; }
        public int Control { get; set; }
        public int Impacto { get; set; }
        public string Fuente { get; set; }
        public int Estado { get; set; }
    }
    public class objCantidadFuente
    {
        public long IdUnidad { get; set; }
        public string Unidad
        {
            get
            {
                return Metodos.GetNombreUnidadCompleto(IdUnidad);
            }
        }
        public IList<string> ValorEscala { get; set; }
        public IList<string> Escala { get; set; }
        public IList<Int32> CantidadEscala { get; set; }
    }

}