using BcmWeb_30.Data.EF;
using BcmWeb_30.Security;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BcmWeb_30
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {

        /// <summary>
        /// 
        /// </summary>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // La siguiente instrucción no se utiliza, solo es para no continuar con el warning del async.
            // Si se elimina todo funciona igual, solo que aparece un Warning por no tener un proceso await en el task
            IFormCollection _data = await context.Request.ReadFormAsync();
            context.Validated();
        }
        /// <summary>
        /// 
        /// </summary>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Encriptador _Encriptador = new Encriptador();
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            tblUsuario user;

            using (Entities _repo = new Entities())
            {
                string _encriptedPassword = _Encriptador.Encriptar(context.Password, Encriptador.HasAlgorimt.SHA1, Encriptador.Keysize.KS256);
                //string _encriptedPassword = context.Password;
                user = await _repo.tblUsuario.FirstOrDefaultAsync(x => x.Email == context.UserName && x.ClaveUsuario == _encriptedPassword);

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }

            }


            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("Nombre", user.Nombre));
            identity.AddClaim(new Claim("Id", user.IdUsuario.ToString()));

            context.Validated(identity);

        }
    }
}