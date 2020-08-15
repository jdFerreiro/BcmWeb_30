using BcmWeb_30.APIModels;
using BcmWeb_30.Data.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BcmWeb_30 .APIController
{
    [Authorize]
    [RoutePrefix("api/documento")]
    public class tblDocumentoController : ApiController
    {
        private Entities db = new Entities();
        private string Culture;
        private string GetRouteFile(tblDocumento x)
        {
            eSystemModules Modulo = (eSystemModules)x.IdTipoDocumento;
            eEstadoDocumento EstadoDocumento = (eEstadoDocumento)x.IdEstadoDocumento;

            string _CodigoInforme = string.Format("{0}_{1}_{2}_{3}_{4}.{5}", Modulo.ToString(), x.IdEmpresa.ToString(), x.NroDocumento.ToString("#000"), (EstadoDocumento == eEstadoDocumento.Certificado ? x.FechaEstadoDocumento.ToString("MM-yyyy") : DateTime.Now.ToString("MM-yyyy")), x.VersionOriginal ?? 1, x.NroVersion);
            string _FileName = string.Format("{0}.pdf", _CodigoInforme.Replace("-", "_"));
            //string _pathFile = string.Format("https://www.BcmWeb_30.net/PDFDocs/{0}", _FileName);
            HttpServerUtility _Server = HttpContext.Current.Server;
            string _MapPath = _Server.MapPath(".");
            string _ServerPath = _MapPath.Substring(0, _MapPath.ToLowerInvariant().IndexOf("api"));
            string _pathFile = String.Format("{0}PDFDocs\\{1}", _ServerPath, _FileName);

            if (!File.Exists(_pathFile))
                _pathFile = string.Empty;

            return _pathFile;
        }

        public tblDocumentoController()
        {
            string[] _userLanguages = HttpContext.Current.Request.UserLanguages;
            Culture = (_userLanguages == null || _userLanguages.Count() == 0 ? "es-VE" : _userLanguages[0]);
        }

        [Route("getall")]
        [HttpGet]
        public async Task<IList<DocumentoModel>> getall()
        {
            List<DocumentoModel> _documentos = new List<DocumentoModel>();
            List<tblDocumento> docs = await db.tblDocumento.ToListAsync();
            _documentos = docs.AsEnumerable()
                .Select(x => new DocumentoModel
                {
                    Id = x.IdDocumento,
                    IdTipoDocumento = x.IdTipoDocumento,
                    Negocios = x.Negocios,
                    NombreDocumento = (x.IdTipoDocumento == 7 
                            ? (x.tblBCPDocumento != null && x.tblBCPDocumento.Count > 0 && string.IsNullOrEmpty(x.tblBCPDocumento.FirstOrDefault().SubProceso.Trim()) 
                                    ? x.tblBCPDocumento.FirstOrDefault().Proceso 
                                    : string.Format("{0}({1})", x.tblBCPDocumento.FirstOrDefault().Proceso, x.tblBCPDocumento.FirstOrDefault().SubProceso)) 
                            : string.Empty),
                    NroDocumento = x.NroDocumento,
                    NroVersion = x.NroVersion,
                    PdfRoute = GetRouteFile(x),
                    TipoDocumento = db.tblModulo.FirstOrDefault(m => m.IdEmpresa == x.IdEmpresa
                                        && m.IdCodigoModulo == x.IdTipoDocumento
                                        && m.IdModuloPadre == 0).Nombre,
                    VersionOriginal = x.VersionOriginal ?? 1,
                })
                .ToList();

            return _documentos.Where(x => !string.IsNullOrEmpty(x.PdfRoute)).ToList();
        }

        [Route("getbyuser/{id:long}/{idEmpresa:long}")]
        [HttpGet]
        public async Task<IList<DocumentoModel>> getbyuser(long id, long idEmpresa)
        {
            tblUsuario tblUsuario = db.tblUsuario.FirstOrDefault(x => x.IdUsuario == id);
            List<DocumentoModel> _documentos = new List<DocumentoModel>();
            tblEmpresaUsuario _empresaUsuario = await db.tblEmpresaUsuario.FirstOrDefaultAsync(x => x.IdEmpresa == idEmpresa && x.IdUsuario == id);
            eNivelUsuario _nivelUsuario = (eNivelUsuario)_empresaUsuario.IdNivelUsuario;
            List<tblDocumento> docs = new List<tblDocumento>();
            try
            {
                if (_nivelUsuario == eNivelUsuario.ConsultorAdministrador || _nivelUsuario == eNivelUsuario.ComiteContinuidad || _nivelUsuario == eNivelUsuario.Administrador)
                {
                    //docs = await db.tblDocumento
                    //    .Where(x => x.IdEmpresa == idEmpresa && x.IdEstadoDocumento == (long)eEstadoDocumento.Certificado)
                    //    .ToListAsync();
                    _documentos = db.tblDocumento
                        .Where(x => x.IdEmpresa == idEmpresa && x.IdEstadoDocumento == (long)eEstadoDocumento.Certificado)
                        .AsEnumerable()
                        .Select(x => new DocumentoModel
                        {
                            Id = x.IdDocumento,
                            IdTipoDocumento = x.IdTipoDocumento,
                            Negocios = x.Negocios,
                            NombreDocumento = (x.IdTipoDocumento == 7 ? (x.tblBCPDocumento != null && x.tblBCPDocumento.Count > 0 && string.IsNullOrEmpty(x.tblBCPDocumento.FirstOrDefault().SubProceso.Trim()) ? x.tblBCPDocumento.FirstOrDefault().Proceso : string.Format("{0}({1})", x.tblBCPDocumento.FirstOrDefault().Proceso, x.tblBCPDocumento.FirstOrDefault().SubProceso)) : string.Empty),
                            NroDocumento = x.NroDocumento,
                            NroVersion = x.NroVersion,
                            PdfRoute = GetRouteFile(x),
                            TipoDocumento = db.tblModulo.FirstOrDefault(m => m.IdEmpresa == x.IdEmpresa
                                                && m.IdCodigoModulo == x.IdTipoDocumento
                                                && m.IdModuloPadre == 0).Nombre,
                            VersionOriginal = x.VersionOriginal ?? 1,
                        })
                        .ToList();
                }
                else
                {
                    _documentos = db.tblDocumento
                       .Where(x => x.IdEmpresa == idEmpresa &&
                                   x.IdEstadoDocumento == (long)eEstadoDocumento.Certificado &&
                                   ((x.IdTipoDocumento != 7 && x.IdTipoDocumento != 4) ||
                                    (x.IdPersonaResponsable == id ||
                                     x.tblDocumentoCertificacion.Where(cx => cx.tblPersona.IdUsuario == id).Count() > 0 || 
                                     x.tblDocumentoAprobacion.Where(cx => cx.tblPersona.IdUsuario == id).Count() > 0)))
                       .AsEnumerable()
                       .Select(x => new DocumentoModel
                       {
                           Id = x.IdDocumento,
                           IdTipoDocumento = x.IdTipoDocumento,
                           Negocios = x.Negocios,
                           NombreDocumento = (x.IdTipoDocumento == 7 ? (x.tblBCPDocumento != null && x.tblBCPDocumento.Count > 0 && string.IsNullOrEmpty(x.tblBCPDocumento.FirstOrDefault().SubProceso.Trim()) ? x.tblBCPDocumento.FirstOrDefault().Proceso : string.Format("{0}({1})", x.tblBCPDocumento.FirstOrDefault().Proceso, x.tblBCPDocumento.FirstOrDefault().SubProceso)) : string.Empty),
                           NroDocumento = x.NroDocumento,
                           NroVersion = x.NroVersion,
                           PdfRoute = GetRouteFile(x),
                           TipoDocumento = db.tblModulo.FirstOrDefault(m => m.IdEmpresa == x.IdEmpresa
                                               && m.IdCodigoModulo == x.IdTipoDocumento
                                               && m.IdModuloPadre == 0).Nombre,
                           VersionOriginal = x.VersionOriginal ?? 1,
                       })
                       .ToList();
                }

                //_documentos = docs.AsEnumerable()
                //    .Select(x => new DocumentoModel
                //    {
                //        Id = x.IdDocumento,
                //        IdTipoDocumento = x.IdTipoDocumento,
                //        Negocios = x.Negocios,
                //        NroDocumento = x.NroDocumento,
                //        NroVersion = x.NroVersion,
                //        PdfRoute = GetRouteFile(x),
                //        TipoDocumento = db.tblModulo.FirstOrDefault(m => m.IdEmpresa == x.IdEmpresa
                //                            && m.IdCodigoModulo == x.IdTipoDocumento
                //                            && m.IdModuloPadre == 0).Nombre,
                //        VersionOriginal = x.VersionOriginal ?? 1,
                //    })
                //    .ToList();
            }
            catch (Exception ex)
            {
                string _message = ex.Message;
                docs = new List<tblDocumento>();
            }

            return _documentos.Where(x => !string.IsNullOrEmpty(x.PdfRoute)).ToList();
        }
        [Route("getbytypeclass/{id:long}/{idEmpresa:long}/{idClaseDoc:int}/{idTypeDoc:long}")]
        [HttpGet]
        public async Task<IList<DocumentoModel>> getbytypeclass(long id, long idEmpresa, int idClaseDoc, long idTypeDoc)
        {
            bool negocios = (idClaseDoc == 1);
            tblUsuario tblUsuario = db.tblUsuario.FirstOrDefault(x => x.IdUsuario == id);
            List<DocumentoModel> _documentos = new List<DocumentoModel>();
            tblEmpresaUsuario _empresaUsuario = await db.tblEmpresaUsuario.FirstOrDefaultAsync(x => x.IdEmpresa == idEmpresa && x.IdUsuario == id);
            eNivelUsuario _nivelUsuario = (eNivelUsuario)_empresaUsuario.IdNivelUsuario;
            List<tblDocumento> docs = new List<tblDocumento>();
            if (_nivelUsuario == eNivelUsuario.ConsultorAdministrador || _nivelUsuario == eNivelUsuario.ComiteContinuidad || _nivelUsuario == eNivelUsuario.Administrador)
            {
                docs = await db.tblDocumento
                    .Where(x => x.IdEmpresa == idEmpresa &&
                            x.IdEstadoDocumento == (long)eEstadoDocumento.Certificado &&
                            x.Negocios == negocios &&
                            x.IdTipoDocumento == idTypeDoc)
                    .ToListAsync();
            }
            else
            {
                docs = await db.tblDocumento
                    .Where(x => x.IdEmpresa == idEmpresa &&
                                x.IdEstadoDocumento == (long)eEstadoDocumento.Certificado &&
                                ((x.IdTipoDocumento != 7 && x.IdTipoDocumento != 4) ||
                                    (x.IdPersonaResponsable == id ||
                                     x.tblDocumentoCertificacion.Where(cx => cx.tblPersona.IdUsuario == id).Count() > 0 ||
                                     x.tblDocumentoAprobacion.Where(cx => cx.tblPersona.IdUsuario == id).Count() > 0)) &&
                                x.Negocios == negocios &&
                                x.IdTipoDocumento == idTypeDoc)
                    .ToListAsync();
            }

            try
            {
                _documentos = docs.AsEnumerable()
                    .Select(x => new DocumentoModel
                    {
                        Id = x.IdDocumento,
                        IdTipoDocumento = x.IdTipoDocumento,
                        Negocios = x.Negocios,
                        NombreDocumento = (x.IdTipoDocumento == 7 ? (x.tblBCPDocumento != null && x.tblBCPDocumento.Count > 0 && string.IsNullOrEmpty(x.tblBCPDocumento.FirstOrDefault().SubProceso.Trim()) ? x.tblBCPDocumento.FirstOrDefault().Proceso : string.Format("{0}({1})", x.tblBCPDocumento.FirstOrDefault().Proceso, x.tblBCPDocumento.FirstOrDefault().SubProceso)) : string.Empty),
                        NroDocumento = x.NroDocumento,
                        NroVersion = x.NroVersion,
                        PdfRoute = GetRouteFile(x),
                        TipoDocumento = db.tblModulo.FirstOrDefault(m => m.IdEmpresa == x.IdEmpresa
                                            && m.IdCodigoModulo == x.IdTipoDocumento
                                            && m.IdModuloPadre == 0).Nombre,
                        VersionOriginal = x.VersionOriginal ?? 1,
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                string _message = ex.Message;
                docs = new List<tblDocumento>();
            }

            return _documentos.Where(x => !string.IsNullOrEmpty(x.PdfRoute)).ToList();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}