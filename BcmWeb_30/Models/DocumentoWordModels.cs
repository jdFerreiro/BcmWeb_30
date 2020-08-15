using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcmWeb_30 .Models
{
    public class datosFicha
    {
        public long IdPersonaResponsable { get; set; }
    }
    public class datosEntrevista
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
    }
    public class datosPersonaEntrevista
    {
        public bool Entrevistador { get; set; }
        public string Nombre { get; set; }
        public string Empresa { get; set; }
    }
    public class datosPersonaClave
    {
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string TelefonoOficina { get; set; }
        public string TelefonoHabitacion { get; set; }
        public string TelefonoMovil { get; set; }
        public string CorreoElectronico { get; set; }
        public string DireccionHabitacion { get; set; }
        public bool Principal { get; set; }
    }
    public class dataPersona
    {
        public string Nombre { get; set; }
        public string Identificacion { get; set; }
        public long IdUnidadOrganizativa { get; set; }
        public long IdCargo { get; set; }
        public long IdUsuario { get; set; }
    }
    public class dataPersonaCorreo
    {
        public string Correo { get; set; }
        public eTipoEmail TipoCorreo { get; set; }
    }
    public class dataPersonaDireccion
    {
        public string MyProperty { get; set; }
    }
}