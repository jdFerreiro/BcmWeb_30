using BcmWeb_30.APIModels;
using BcmWeb_30.Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace BcmWeb_30 .APIController
{
    [Authorize]
    [RoutePrefix("api/auditoria")]

    public class tblAuditoriaController : ApiController
    {
        private Entities db = new Entities();

        [Route("getall")]
        [HttpGet]
        public IQueryable<AuditoriaModel> GettblAuditoria()
        {
            List<tblAuditoria> regs = db.tblAuditoria.ToList();
            List<AuditoriaModel> data = regs.Select(x => new AuditoriaModel
            {
                Accion = x.Accion,
                Id = x.IdAuditoria,
                IdDocumento = x.IdDocumento ?? 0,
                IdEmpresa = x.IdEmpresa ?? 0,
                IdTipoDocumento = x.IdTipoDocumento ?? 0,
                IdUsuario = x.IdUsuario ?? 0,
                IpAddress = x.DireccionIP,
                Mensaje = x.Mensaje,
                Negocios = x.Negocios,
            }).ToList();

            return data.AsQueryable();
        }

        [Route("getbyid/{id:long}")]
        [HttpGet]
        [ResponseType(typeof(AuditoriaModel))]
        public async Task<IHttpActionResult> GettblAuditoria(long id)
        {
            tblAuditoria x = await db.tblAuditoria.FindAsync(id);
            if (x == null)
            {
                return NotFound();
            }

            AuditoriaModel _reg = new AuditoriaModel
            {
                Accion = x.Accion,
                Id = x.IdAuditoria,
                IdDocumento = x.IdDocumento ?? 0,
                IdEmpresa = x.IdEmpresa ?? 0,
                IdTipoDocumento = x.IdTipoDocumento ?? 0,
                IdUsuario = x.IdUsuario ?? 0,
                IpAddress = x.DireccionIP,
                Mensaje = x.Mensaje,
                Negocios = x.Negocios,
            };

            return Ok(_reg);
        }

        [Route("add")]
        [HttpPost]
        [ResponseType(typeof(AuditoriaModel))]
        public async Task<IHttpActionResult> PosttblAuditoria(AuditoriaModel reg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            tblAuditoria tblAuditoria = new tblAuditoria
            {
                Accion = reg.Accion,
                DatosModificados = string.Empty,
                DireccionIP = reg.IpAddress,
                FechaRegistro = DateTime.UtcNow,
                IdDocumento = reg.IdDocumento,
                IdEmpresa = reg.IdEmpresa,
                IdTipoDocumento = reg.IdTipoDocumento,
                IdUsuario = reg.IdUsuario,
                Mensaje = reg.Mensaje,
                Negocios = reg.Negocios,
            };

            try
            {
                db.tblAuditoria.Add(tblAuditoria);
                await db.SaveChangesAsync();
                reg.Id = tblAuditoria.IdAuditoria;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return Ok(reg);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tblAuditoriaExists(long id)
        {
            return db.tblAuditoria.Count(e => e.IdAuditoria == id) > 0;
        }
    }
}