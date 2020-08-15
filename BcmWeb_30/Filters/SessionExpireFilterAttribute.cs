using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BcmWeb_30 
{
    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            HttpContext ctx = HttpContext.Current;

            if (ctx.Session["UserId"] == null)
            {

                filterContext.Result = new RedirectResult("~/Account/Login");

                IPrincipal user = ctx.User;
                IIdentity userIdentity = user.Identity;

                if (userIdentity.IsAuthenticated)
                {
                    FormsIdentity id = (FormsIdentity)user.Identity;
                    FormsAuthenticationTicket ticket = id.Ticket;

                    if (ticket != null)
                    {
                        long IdUser = long.Parse(ticket.UserData);
                        if (IdUser > 0)
                        {
                            Metodos.Logout(IdUser);

                            FormsAuthentication.SignOut();
                            return;
                        }
                    }
                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}