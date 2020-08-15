using System.ComponentModel;
using System.Runtime.Serialization;

namespace BcmWeb_30 
{
    [DataContract(Name = "TipoDirección")]
    public enum eTipoDireccion : int
    {
        [EnumMember()]
        [Description("Oficina")]
        Oficina = 1,
        [EnumMember()]
        [Description("Habitación")]
        Habitacion = 2,
    }
    [DataContract(Name = "MesesGranImpacto")]
    public enum eMesesGI : int
    {
        [EnumMember()]
        [Description("Enero")]
        Enero = 1,
        [EnumMember()]
        [Description("Febrero")]
        Febrero = 2,
        [EnumMember()]
        [Description("Marzo")]
        Marzo = 3,
        [EnumMember()]
        [Description("Abril")]
        Abril = 4,
        [EnumMember()]
        [Description("Mayo")]
        Mayo = 5,
        [EnumMember()]
        [Description("Junio")]
        Junio = 6,
        [EnumMember()]
        [Description("Julio")]
        Julio = 7,
        [EnumMember()]
        [Description("Agosto")]
        Agosto = 8,
        [EnumMember()]
        [Description("Septiembre")]
        Septiembre = 9,
        [EnumMember()]
        [Description("Octubre")]
        Octubre = 10,
        [EnumMember()]
        [Description("Noviembre")]
        Noviembre = 11,
        [EnumMember()]
        [Description("Diciembre")]
        Diciembre = 12,
        [EnumMember()]
        [Description("Todo el Año")]
        Anual = 13,
    }
    [DataContract(Name = "TipoTeléfono")]
    public enum eTipoTelefono : int
    {
        [EnumMember()]
        [Description("Corporativo")]
        Corporativo = 1,
        [EnumMember()]
        [Description("Habitación")]
        Habitacion = 2,
        [EnumMember()]
        [Description("Móvil")]
        Movil = 3,
    }
    [DataContract(Name = "TipoEmail")]
    public enum eTipoEmail : int
    {
        [EnumMember()]
        [Description("Corporativo")]
        Corporativo = 1,
        [EnumMember()]
        [Description("Personal")]
        Personal = 2,
    }
    [DataContract(Name = "EstadoDocumento")]
    public enum eEstadoDocumento : int
    {
        [EnumMember()]
        [Description("Cargando")]
        Cargando = 1,
        [EnumMember()]
        [Description("Por Aprobar")]
        PorAprobar = 2,
        [EnumMember()]
        [Description("En Proceso de Aprobación")]
        Aprobando = 3,
        [EnumMember()]
        [Description("Por Certificar")]
        PorCertificar = 4,
        [EnumMember()]
        [Description("Certificando")]
        Certificando = 5,
        [EnumMember()]
        [Description("Certificado")]
        Certificado = 6,
    }
    [DataContract(Name = "EstadoEmpresa")]
    public enum eEstadoEmpresa : int
    {
        [EnumMember()]
        [Description("Activa")]
        Activa = 1,
        [EnumMember()]
        [Description("Bloqueada")]
        Bloqueada = 2,
    }
    [DataContract(Name = "EstadoUsuario")]
    public enum eEstadoUsuario : int
    {
        [EnumMember()]
        [Description("Activo")]
        Activo = 1,
        [EnumMember()]
        [Description("Conectado")]
        Conectado = 2,
        [EnumMember()]
        [Description("Bloqueado")]
        Bloqueado = 3,
        [EnumMember()]
        [Description("Bloqueado")]
        Eliminado = 4,
    }
    [DataContract(Name = "Leyenda")]
    public enum eLeyenda : int
    {
        [EnumMember()]
        [Description("Impacto Financiero")]
        Impacto_Financiero = 1,
        [EnumMember()]
        [Description("Impacto Operacional")]
        Impacto_Operacional = 2,
        [EnumMember()]
        [Description("MTD")]
        MTD = 3,
        [EnumMember()]
        [Description("RTO")]
        RTO = 4,
        [EnumMember()]
        [Description("RPO")]
        RPO = 5,
        [EnumMember()]
        [Description("WRT")]
        WRT = 6,
    }
    [DataContract(Name = "Modo_Actualizacion")]
    public enum eModoActualizacion : int
    {
        [EnumMember()]
        [Description("Tabla")]
        Tabla = -1,
        [EnumMember()]
        [Description("Texto")]
        Texto = 0
    }
    [DataContract(Name = "NivelUsuario")]
    public enum eNivelUsuario : int
    {
        [EnumMember()]
        [Description("Super Admninistrador")]
        SuperAdministrador = 0,
        [EnumMember()]
        [Description("Administrador")]
        Administrador = 1,
        [EnumMember()]
        [Description("Entrevistador")]
        Entrevistador = 2,
        [EnumMember()]
        [Description("Dueño de Procesos")]
        DueñoProcesos = 3,
        [EnumMember()]
        [Description("Comité de Continuidad")]
        ComiteContinuidad = 4,
        [EnumMember()]
        [Description("Certificador")]
        Certificador = 5,
        [EnumMember()]
        [Description("Consultor Administrador")]
        ConsultorAdministrador = 6,
        [EnumMember()]
        [Description("Aprobador")]
        Aprobador = 7,
        [EnumMember()]
        [Description("Entrevistado")]
        Entrevistado = 8,
        [EnumMember()]
        [Description("Persona Clave")]
        PersonaClave = 9,
        [EnumMember()]
        [Description("Solo Consulta")]
        Solo_Consulta = 10,
        [EnumMember()]
        [Description("Actualizador")]
        Actualizador = 11,
    }
    [DataContract(Name = "Tipo_Elemento")]
    public enum eTipoElemento : int
    {
        [EnumMember()]
        [Description("Título")]
        Header = 1,
        [EnumMember()]
        [Description("Subtítulo")]
        SubHeader = 2,
        [EnumMember()]
        [Description("Segundo Subtítulo")]
        SubSubHeader = 3,
        [EnumMember()]
        [Description("Elemento")]
        Elemento = 4,
    }
    [DataContract(Name = "TipoImpacto")]
    public enum eTipoImpacto : int
    {
        [EnumMember()]
        [Description("Financiero")]
        Financiero = 1,
        [EnumMember()]
        [Description("Operacional")]
        Operacional = 2,
    }
    [DataContract(Name = "TipoPersona")]
    public enum eTipoPersona : int
    {
        [EnumMember()]
        [Description("Empleado")]
        Empleado = 1,
        [EnumMember()]
        [Description("Consultor")]
        Consultor = 2,
        [EnumMember()]
        [Description("Proveedor")]
        Proveedor = 3,
        [EnumMember()]
        [Description("Responsable Entrada")]
        ResponsableEntrada = 4,
        [EnumMember()]
        [Description("Personal Clave BIA")]
        PersonalClaveBIA = 5,
    }
    [DataContract(Name = "TipoProceso")]
    public enum eTipoProceso : int
    {
        [EnumMember()]
        Normal = 1,
        [EnumMember()]
        Crítico = 2,
    }
    [DataContract(Name = "TipoUbicacionInformacion")]
    public enum eTipoUbicacionInformacion : int
    {
        [EnumMember()]
        [Description("Física")]
        Fisica = 1,
        [EnumMember()]
        [Description("Electrónica")]
        Electrónica = 2,
    }
    [DataContract(Name = "UnidadTiempo")]
    public enum eUnidadTiempo : int
    {
        [EnumMember()]
        [Description("Hora")]
        Hora = 1,
        [EnumMember()]
        [Description("Diario")]
        Diario = 2,
        [EnumMember()]
        [Description("Semanal")]
        Semanal = 3,
        [EnumMember()]
        [Description("Quincenal")]
        Quincenal = 4,
        [EnumMember()]
        [Description("Mensual")]
        Mensual = 5,
        [EnumMember()]
        [Description("Bimensual")]
        Bimensual = 6,
        [EnumMember()]
        [Description("Trimestral")]
        Trimestral = 7,
        [EnumMember()]
        [Description("Semestral")]
        Semenstral = 8,
        [EnumMember()]
        [Description("Anual")]
        Anual = 9,
    }
    [DataContract(Name = "NivelImpacto")]
    public enum eNivelImpacto : int
    {
        [EnumMember()]
        [Description("Bajo")]
        Bajo = 1,
        [EnumMember()]
        [Description("Medio")]
        Medio = 2,
        [EnumMember()]
        [Description("Alto")]
        Alto = 3,
    }
    [DataContract(Name = "TipoRespaldo")]
    public enum eTipoRespaldo : int
    {
        [EnumMember()]
        [Description("Principal")]
        Principal = 1,
        [EnumMember()]
        [Description("Secundario")]
        Secundario = 2,
    }
    [DataContract(Name = "TipoInterdependencia")]
    public enum eTipoInterdependencia : int
    {
        [EnumMember()]
        [Description("Unidad Organizativa")]
        UnidadOrganizativa = 1,
        [EnumMember()]
        [Description("Vicepresidencia")]
        Vicepresidencia = 2,
    }
    [DataContract(Name = "TipoEscala")]
    public enum eTipoEscala : int
    {
        [EnumMember()]
        [Description("Impacto Financiero")]
        ImpactoFinanciero = 1,
        [EnumMember()]
        [Description("Impacto Operacional")]
        ImpactoOperacional = 2,
        [EnumMember()]
        [Description("TMC")]
        MTD = 3,
        [EnumMember()]
        [Description("TRO")]
        RTO = 4,
        [EnumMember()]
        [Description("PRO")]
        RPO = 5,
        [EnumMember()]
        [Description("TRT")]
        WRT = 6,
    }
    [DataContract(Name = "Módulos")]
    public enum eSystemModules : int
    {
        [EnumMember()]
        [Description("Políticas de Continuidad")]
        PLC = 1,
        [EnumMember()]
        [Description("Estructura de Continuidad")]
        ETC = 2,
        [EnumMember()]
        [Description("Análisis de Riesgo")]
        RSG = 3,
        [EnumMember()]
        [Description("Análisis de Impacto al Negocio")]
        BIA = 4,
        [EnumMember()]
        [Description("Estrategia de Continuidad")]
        ETG = 5,
        [EnumMember()]
        [Description("Procedimientos de Continuidad")]
        BCP = 7,
        [EnumMember()]
        [Description("Plan para Manejo de Incidentes")]
        PMI = 6,
        [EnumMember()]
        [Description("Plan de Pruebas y Ejercicios")]
        PPE = 8,
        [EnumMember()]
        [Description("Plan de Mantenimiento")]
        PMT = 9,
        [EnumMember()]
        [Description("Plan de Adiestramiento")]
        PAD = 10,
        [EnumMember()]
        [Description("Módulo de Administracion")]
        ADM = 11,
        [EnumMember()]
        [Description("Acceso desde dispositivo Móvil")]
        Mobile = 12,
        [EnumMember()]
        [Description("Reportes")]
        RPT = 13,
        [EnumMember()]
        [Description("Pruebas y Ejercicios")]
        PBE = 14,
    }
    [DataContract(Name = "TipoResultadoPrueba")]
    public enum eTipoResultadoPrueba : int
    {
        [EnumMember()]
        [Description("Hallazgos")]
        Hallazgo = 1,
        [EnumMember()]
        [Description("Acción")]
        Accion = 2,
    }
    [DataContract(Name = "TipoTablaContenido")]
    public enum eTipoTablaContenido
    {
        [EnumMember()]
        [Description("Entradas")]
        Entradas,
        [EnumMember()]
        [Description("Proveedores")]
        Proveedores,
        [EnumMember()]
        [Description("Interdependencias")]
        Interdependencias,
        [EnumMember()]
        [Description("Productos")]
        Productos,
        [EnumMember()]
        [Description("Tecnología")]
        Tecnología,
        [EnumMember()]
        [Description("Información Esencial")]
        Información_Esencial,
        [EnumMember()]
        [Description("Personal Clave")]
        Personal_Clave,
        [EnumMember()]
        [Description("No Asignado")]
        No_Asignado,
    }
    [DataContract(Name = "TipoAcción")]
    public enum eTipoAccion
    {
        [EnumMember()]
        [Description("Mostrar")]
        Mostrar,
        [EnumMember()]
        [Description("Actualizar")]
        Actualizar,
        [EnumMember()]
        [Description("Eliminar")]
        Eliminar,
        [EnumMember()]
        [Description("Abrir PDF Web")]
        AbrirPDFWeb,
        [EnumMember()]
        [Description("Abrir PDF Móvil")]
        AbrirPDFMovil,
        [EnumMember()]
        [Description("Generar PDF")]
        GenerarPDF,
        [EnumMember()]
        [Description("Generar versión")]
        GenerarVersion,
        [EnumMember()]
        [Description("Accesar al Módulo desde la Web")]
        AccesoModuloWeb,
        [EnumMember()]
        [Description("Accesar al Módulo desde el Móvil")]
        AccesoModuloMovil,
        [EnumMember()]
        [Description("Iniciar proceso de Aprobación")]
        IniciarAprobacion,
        [EnumMember()]
        [Description("Aprobar Documento")]
        AprobarDocumento,
        [EnumMember()]
        [Description("Certificar Documento")]
        CertificarDocumento,
        [EnumMember()]
        [Description("Consultar Cambios")]
        ConsultarCambios,
        [EnumMember()]
        [Description("Generar Copia del Documento")]
        GenearCopiaDocumento,
        [EnumMember()]
        [Description("Agregar Anexo al documento")]
        AgregarAnexoDocumento,
        [EnumMember()]
        [Description("Agregar Anexo al módulo")]
        AgregarAnexoModulo,
        [EnumMember()]
        [Description("Abrir Documento")]
        AbrirDocumento,
        [EnumMember()]
        [Description("Eliminar Anexo Documento")]
        EliminarAnexoDocumento,
        [EnumMember()]
        [Description("Eliminar Anexo Módulo")]
        EliminarAnexoModulo,
        [EnumMember()]
        [Description("Acceder Anexo Módulo Movil")]
        AccederAnexoModuloMovil,
        [EnumMember()]
        [Description("Acceder Anexo Módulo Web")]
        AccederAnexoModuloWeb,
        [EnumMember()]
        [Description("Acceder Anexo Documento Web")]
        AccederAnexoDocumentoWeb,
        [EnumMember()]
        [Description("Acceder Anexo Documento Movil")]
        AccederAnexoDocumentoMovil,
        [EnumMember()]
        [Description("Acceder Iniciativas")]
        MostrarIniciativa,
        [EnumMember()]
        [Description("Agregar Iniciativa")]
        AgregarIniciativa,
        [EnumMember()]
        [Description("Actualizar Iniciativa")]
        ActualizarIniciativa,
        [EnumMember()]
        [Description("Eliminar Iniciativa")]
        EliminarIniciativa,
        [EnumMember()]
        [Description("Agregar Anexo Iniciativa ")]
        AgregarAnexoIniciativa,
        [EnumMember()]
        [Description("Agregar Anexo Iniciativa ")]
        EliminarAnexoIniciativa,
        [EnumMember()]
        [Description("Agregar Frecuencia")]
        AgregarFrecuencia,
        [EnumMember()]
        [Description("Actualizar Frecuencia")]
        ActualizarFrecuencia,
        [EnumMember()]
        [Description("Eliminar Frecuencia")]
        EliminarFrecuencia,
        [EnumMember()]
        [Description("Agregar Programación")]
        AgregarProgramacion,
        [EnumMember()]
        [Description("Actualizar Programación")]
        ActualizarProgramacion,
        [EnumMember()]
        [Description("Eliminar Programación")]
        EliminarProgramacion,
        [EnumMember()]
        [Description("Agregar Notificación")]
        AgregarNotificacion,
        [EnumMember()]
        [Description("Actualizar Notificación")]
        ActualizarNotificacion,
        [EnumMember()]
        [Description("Eliminar Notificación")]
        EliminarNotificacion,
        [EnumMember()]
        [Description("Agregar Empresa")]
        AgregarEmpresa,
        [EnumMember()]
        [Description("Actualizar Empresa")]
        ActualizarEmpresa,
        [EnumMember()]
        [Description("Eliminar Empresa")]
        EliminarEmpresa,
        [EnumMember()]
        [Description("Bloquear Empresa")]
        BloquearEmpresa,
        [EnumMember()]
        [Description("Activar Empresa")]
        ActivarEmpresa,
        [EnumMember()]
        [Description("Agregar Usuario")]
        AgregarUsuario,
        [EnumMember()]
        [Description("Actualizar Usuario")]
        ActualizarUsuario,
        [EnumMember()]
        [Description("Eliminar Usuario")]
        EliminarUsuario,
        [EnumMember()]
        [Description("Bloquear Usuario")]
        BloquearUsuario,
        [EnumMember()]
        [Description("Activar Usuario")]
        ActivarUsuario,
        [EnumMember()]
        [Description("Actualizar Módulos Usuario")]
        ActualizarModulosUsuario,
        [EnumMember()]
        [Description("Agregar Incidente")]
        AgregarIncidente,
        [EnumMember()]
        [Description("Actualizar Incidente")]
        ActualizarIncidente,
        [EnumMember()]
        [Description("Eliminar Incidente")]
        EliminarIncidente,
        [EnumMember()]
        [Description("Consultar Dispositivos")]
        ConsultarDispositivos,
        [EnumMember()]
        [Description("Activar Evento")]
        ActivarEvento,
        [EnumMember()]
        [Description("Eliminar Evento")]
        EliminarEvento,

    }
    [DataContract(Name = "Secciones de Ficha")]
    public enum eSeccionFicha
    {
        [EnumMember()]
        [Description("Información General")]
        InformacionGeneral,
        [EnumMember()]
        [Description("Empresas y Unidades")]
        Empresas_Unidades,
        [EnumMember()]
        [Description("Responsable")]
        Responsable,
        [EnumMember()]
        [Description("Certificador")]
        Certificador,
        [EnumMember()]
        [Description("Uso")]
        UsoDocumento,
        [EnumMember()]
        [Description("Control")]
        Control,
        [EnumMember()]
        [Description("Sin Definir")]
        SinDefinir,
    }
    [DataContract(Name = "Estado Proceso")]
    public enum eEstadoProceso : int
    {
        [EnumMember()]
        [Description("Activo")]
        Activo = 1,
        [EnumMember()]
        [Description("Inactivo")]
        Inactivo = 2,
        [EnumMember()]
        [Description("Inactivo")]
        Eliminado = 3,
    }

}