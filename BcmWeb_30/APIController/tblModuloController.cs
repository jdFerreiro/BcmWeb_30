using BcmWeb_30.APIModels;
using BcmWeb_30.Data.EF;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BcmWeb_30 .APIController
{
    [Authorize]
    [RoutePrefix("api/modulo")]
    public class tblModuloController : ApiController
    {
        private Entities db = new Entities();
        private string Culture;

        public tblModuloController()
        {
            string[] _userLanguages = HttpContext.Current.Request.UserLanguages;
            Culture = (_userLanguages == null || _userLanguages.Count() == 0 ? "es-VE" : _userLanguages[0]);
        }

        [Route("GetAllNegociosByEmpresa/{idEmpresa:long}")]
        [HttpGet]
        public async Task<IList<ModuloModel>> GetAllNegociosByEmpresa(long idEmpresa)
        {
            List<ModuloModel> data = new List<ModuloModel>();
            List<tblModulo> _modulosEmpresa = await db.tblModulo
                .Where(x => x.IdEmpresa == idEmpresa && x.Negocios == true && x.IdCodigoModulo != 3 && x.IdCodigoModulo != 4 && x.IdCodigoModulo < 8)
                .ToListAsync();

            data = _modulosEmpresa
                .AsEnumerable()
                .Select(x => new ModuloModel
                {
                    Id = x.IdModulo,
                    IdCodigoModulo = x.IdCodigoModulo ?? 0,
                    IdPadre = x.IdModuloPadre,
                    IdTipoElemento = x.IdTipoElemento,
                    Negocios = x.Negocios,
                    Nombre = x.Nombre,
                    Tecnologia = x.Tecnologia,
                })
                .ToList();

            return data;
        }
        [Route("GetAllTecnologiaByEmpresa/{idEmpresa:long}")]
        [HttpGet]
        public async Task<IList<ModuloModel>> GetAllTecnologiaByEmpresa(long idEmpresa)
        {
            List<ModuloModel> data = new List<ModuloModel>();
            List<tblModulo> _modulosEmpresa = await db.tblModulo
                .Where(x => x.IdEmpresa == idEmpresa && x.Tecnologia == true && x.IdCodigoModulo != 3 && x.IdCodigoModulo != 4 && x.IdCodigoModulo < 8)
                .ToListAsync();

            data = _modulosEmpresa
                .AsEnumerable()
                .Select(x => new ModuloModel
                {
                    Id = x.IdModulo,
                    IdCodigoModulo = x.IdCodigoModulo ?? 0,
                    IdPadre = x.IdModuloPadre,
                    IdTipoElemento = x.IdTipoElemento,
                    Negocios = x.Negocios,
                    Nombre = x.Nombre,
                    Tecnologia = x.Tecnologia,
                })
                .ToList();

            return data;
        }
        [Route("GetPrincipalNegocioByEmpresa/{idEmpresa:long}")]
        [HttpGet]
        public async Task<IList<ModuloModel>> GetPrincipalNegocioByEmpresa(long idEmpresa)
        {
            List<ModuloModel> data = new List<ModuloModel>();
            List<tblModulo> _modulosEmpresa = await db.tblModulo
                .Where(x => x.IdEmpresa == idEmpresa && x.Negocios == true && x.IdModuloPadre == 0 && x.IdCodigoModulo != 3 && x.IdCodigoModulo != 4 && x.IdCodigoModulo < 8)
                .ToListAsync();

            data = _modulosEmpresa
                .AsEnumerable()
                .Select(x => new ModuloModel
                {
                    Id = x.IdModulo,
                    IdCodigoModulo = x.IdCodigoModulo ?? 0,
                    IdPadre = x.IdModuloPadre,
                    IdTipoElemento = x.IdTipoElemento,
                    Negocios = x.Negocios,
                    Nombre = x.Nombre,
                    Tecnologia = x.Tecnologia,
                })
                .ToList();

            return data;
        }
        [Route("GetPrincipalTecnologiaByEmpresa/{idEmpresa:long}")]
        [HttpGet]
        public async Task<IList<ModuloModel>> GetPrincipalTecnologiaByEmpresa(long idEmpresa)
        {
            List<ModuloModel> data = new List<ModuloModel>();
            List<tblModulo> _modulosEmpresa = await db.tblModulo
                .Where(x => x.IdEmpresa == idEmpresa && x.Tecnologia == true && x.IdModuloPadre == 0 && x.IdCodigoModulo != 3 && x.IdCodigoModulo != 4 && x.IdCodigoModulo < 8)
                .ToListAsync();

            data = _modulosEmpresa
                .AsEnumerable()
                .Select(x => new ModuloModel
                {
                    Id = x.IdModulo,
                    IdCodigoModulo = x.IdCodigoModulo ?? 0,
                    IdPadre = x.IdModuloPadre,
                    IdTipoElemento = x.IdTipoElemento,
                    Negocios = x.Negocios,
                    Nombre = x.Nombre,
                    Tecnologia = x.Tecnologia,
                })
                .ToList();

            return data;
        }
        [Route("GetPrincipalNegocioByEmpresa_Usuario/{idEmpresa:long}/{idUsuario:long}")]
        [HttpGet]
        public async Task<IList<ModuloModel>> GetPrincipalNegocioByEmpresa_Usuario(long idEmpresa, long idUsuario)
        {
            List<ModuloModel> data = new List<ModuloModel>();
            List<tblModulo> _modulosEmpresa = await db.tblModulo_Usuario
                .Where(x => x.IdEmpresa == idEmpresa && x.tblModulo.Negocios == true
                         && x.tblModulo.IdModuloPadre == 0 && x.IdUsuario == idUsuario
                         && x.tblModulo.IdCodigoModulo != 3 && x.tblModulo.IdCodigoModulo != 4
                         && x.tblModulo.IdCodigoModulo < 8)
                .Select(x => x.tblModulo)
                .ToListAsync();

            data = _modulosEmpresa
                .AsEnumerable()
                .Select(x => new ModuloModel
                {
                    Id = x.IdModulo,
                    IdCodigoModulo = x.IdCodigoModulo ?? 0,
                    IdPadre = x.IdModuloPadre,
                    IdTipoElemento = x.IdTipoElemento,
                    Negocios = x.Negocios,
                    Nombre = x.Nombre,
                    Tecnologia = x.Tecnologia,
                })
                .ToList();

            return data;
        }
        [Route("GetPrincipalTecnologiaByEmpresa_Usuario/{idEmpresa:long}/{idUsuario:long}")]
        [HttpGet]
        public async Task<IList<ModuloModel>> GetPrincipalTecnologiaByEmpresa_Usuario(long idEmpresa, long idUsuario)
        {
            List<ModuloModel> data = new List<ModuloModel>();
            List<tblModulo> _modulosEmpresa = await db.tblModulo_Usuario
                .Where(x => x.IdEmpresa == idEmpresa && x.tblModulo.Tecnologia == true
                         && x.tblModulo.IdModuloPadre == 0 && x.IdUsuario == idUsuario
                         && x.tblModulo.IdCodigoModulo != 3 && x.tblModulo.IdCodigoModulo != 4
                         && x.tblModulo.IdCodigoModulo < 8)
                .Select(x => x.tblModulo)
                .ToListAsync();

            data = _modulosEmpresa
                .AsEnumerable()
                .Select(x => new ModuloModel
                {
                    Id = x.IdModulo,
                    IdCodigoModulo = x.IdCodigoModulo ?? 0,
                    IdPadre = x.IdModuloPadre,
                    IdTipoElemento = x.IdTipoElemento,
                    Negocios = x.Negocios,
                    Nombre = x.Nombre,
                    Tecnologia = x.Tecnologia,
                })
                .ToList();

            return data;
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