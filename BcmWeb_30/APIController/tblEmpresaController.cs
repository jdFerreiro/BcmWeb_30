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
    [RoutePrefix("api/empresa")]
    public class tblEmpresaController : ApiController
    {
        private Entities db = new Entities();
        private string Culture;

        public tblEmpresaController()
        {
            string[] _userLanguages = HttpContext.Current.Request.UserLanguages;
            Culture = (_userLanguages == null || _userLanguages.Count() == 0 ? "es-VE" : _userLanguages[0]);
        }

        [Route("getbyuser/{IdUsuario:long}")]
        [HttpGet]
        public async Task<IList<EmpresaModel>> getbyuser(long IdUsuario)
        {
            List<EmpresaModel> data = new List<EmpresaModel>();

            List<tblEmpresaUsuario> _empresaUsuario = await db.tblEmpresaUsuario.Where(x => x.IdUsuario == IdUsuario).ToListAsync();

            data = _empresaUsuario.AsEnumerable()
                .Select(x => new EmpresaModel
                {
                    Id = x.IdEmpresa,
                    IdNivelUsuario = x.IdNivelUsuario,
                    ImageUrl = Url.Content(x.tblEmpresa.LogoURL).ToString(),
                    Nombre = x.tblEmpresa.NombreComercial,
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