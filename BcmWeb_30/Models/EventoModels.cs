using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcmWeb_30 .Models
{

    public class DispositivosEmpresaModel : ModulosUserModel
    {
        public List<DispositivoModel> Dispositivos { get; set; }
    }
    public class DispositivoModel : ModulosUserModel
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
        public Nullable<DateTime> fechaUltimaConexion { get; set; }
        public List<DispositivoConexion> Conexiones { get; set; }
        public bool Selected { get; set; }
        public Nullable<DateTime> FechaEnvio { get; set; }
        public Nullable<DateTime> FechaDescarga { get; set; }
    }
    public class DispositivoConexion : ModulosUserModel
    {
        public string Empresa { get; set; }
        public long IdDispositivo { get; set; }
        public DateTime FechaConexion { get; set; }
        public string DireccionIP { get; set; }
        public long IdUsuario { get; set; }
        public string Usuario { get; set; }
    }
    public class EventoEmpresaModel : ModulosUserModel
    {
        public long IdSubmoduloSelected { get; set; }
        public long countSelected { get; set; }
        public string selectedIds { get; set; }
        public bool isSaving { get; set; }
        public List<DispositivoModel> Dispositivos { get; set; }
    }
    public class EventoModel : ModulosUserModel
    {
        public string IdDispositivos { get; set; }
        public long IdSubmodulo { get; set; }
        public string evento { get; set; }
        public long IdUsuarioEnvia { get; set; }
        public bool Descargado { get; set; }
    }


    /* Nuevos Objetos para Dispositivos '*/
    public class DispositivosRegistradosEmpresaModel : ModulosUserModel
    {
        public List<DispositivoRegistradoModel> Dispositivos { get; set; }
    }
    public class DispositivoRegistradoModel : ModulosUserModel
    {
        public long Id { get; set; }
        public long IdUsuario { get; set; }
        public Nullable<DateTime> FechaRegistro { get; set; }
        public string IMEIDispositivo { get; set; }
        public string NombreDispositivo { get; set; }
        public string NombreFabricante { get; set; }
        public string ModeloDispositivo { get; set; }
        public string PlataformaDispositivo { get; set; }
        public string VersionSO { get; set; }
        public string TipoDispositivo { get; set; }
        public Nullable<DateTime> FechaUltimaConexion { get; set; }
        public List<DispositivoRegistradoConexion> Conexiones { get; set; }
        public bool Selected { get; set; }
        public Nullable<DateTime> FechaEnvio { get; set; }
        public Nullable<DateTime> FechaDescarga { get; set; }
        public string Usuario { get; set; }
    }
    public class DispositivoRegistradoConexion : ModulosUserModel
    {
        public string Empresa { get; set; }
        public long IdDispositivo { get; set; }
        public long IdUsuario { get; set; }
        public DateTime FechaConexion { get; set; }
        public string DireccionIP { get; set; }
    }
    public class EventoRegistroEmpresaModel : ModulosUserModel
    {
        public long IdSubmoduloSelected { get; set; }
        public long countSelected { get; set; }
        public string selectedIds { get; set; }
        public bool isSaving { get; set; }
        public List<DispositivoRegistradoModel> Dispositivos { get; set; }
    }
    public class DispositivoRegistradoEventoModel : ModulosUserModel
    {
        public long IdDispositivo { get; set; }
        public long IdUsuario { get; set; }
        public long IdSubmodulo { get; set; }
        public long IdUsuarioEnvia { get; set; }
        public bool Descargado { get; set; }
        public string evento { get; set; }
    }

}