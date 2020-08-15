using BcmWeb_30.APIModels;
using BcmWeb_30.Data.EF;
using BcmWeb_30.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace BcmWeb_30 .APIController
{
    [Authorize]
    [RoutePrefix("api/Usuario")]
    public class tblUsuarioController : ApiController
    {
        private Encriptador _Encriptador = new Encriptador();
        private Entities db = new Entities();
        private string Culture;

        public tblUsuarioController()
        {
            string[] _userLanguages = HttpContext.Current.Request.UserLanguages;
            Culture = (_userLanguages == null || _userLanguages.Count() == 0 ? "es-VE" : _userLanguages[0]);
        }

        [Route("getall")]
        [HttpGet]
        public async Task<IList<UsuarioModel>> GetUsuario()
        {

            List<UsuarioModel> data = await db.tblUsuario.Select(x => new UsuarioModel
            {
                Email = x.Email,
                Id = x.IdUsuario,
                Nombre = x.Nombre,
            }).ToListAsync();

            return data;
        }
        [ResponseType(typeof(UsuarioModel))]
        [Route("GetById/{id:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetById(long id)
        {
            tblUsuario tblUsuario = await db.tblUsuario.FindAsync(id);
            if (tblUsuario == null)
            {
                return NotFound();
            }

            UsuarioModel data = new UsuarioModel
            {
                Email = tblUsuario.Email,
                Id = tblUsuario.IdUsuario,
                Nombre = tblUsuario.Nombre,
            };

            return Ok(data);
        }
        [ResponseType(typeof(UsuarioModel))]
        [Route("GetByCredentials/{codigo}/{password}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetByCredentials(string codigo, string password)
        {
            string _encriptedPassword = _Encriptador.Encriptar(password, Encriptador.HasAlgorimt.SHA1, Encriptador.Keysize.KS256);

            tblUsuario tblUsuario = await db.tblUsuario.FirstOrDefaultAsync(x => x.Email == codigo && x.ClaveUsuario == _encriptedPassword);
            if (tblUsuario == null)
            {
                return NotFound();
            }

            UsuarioModel data = new UsuarioModel
            {
                Email = tblUsuario.Email,
                Id = tblUsuario.IdUsuario,
                Nombre = tblUsuario.Nombre,
            };

            return Ok(data);
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